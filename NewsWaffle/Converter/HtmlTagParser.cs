using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

using NewsWaffle.Converter.Filter;
using NewsWaffle.Converter.Special;
using NewsWaffle.Models;
using NewsWaffle.Util;

namespace NewsWaffle.Converter
{
    /// <summary>
    /// parses HTML nodes into Section Items
    /// </summary>
    public class HtmlTagParser
    {
        private static readonly string[] blockElements = new string[] { "address", "article", "aside", "blockquote", "canvas", "dd", "div", "dl", "dt", "fieldset", "figcaption", "figure", "footer", "form", "h1", "h2", "h3", "h4", "h5", "h6", "header", "hr", "li", "main", "nav", "noscript", "ol", "p", "pre", "section", "table", "tfoot", "ul", "video" };

        private List<ContentItem> items = new List<ContentItem>();
        public List<MediaItem> Images = new List<MediaItem>();
        private List<HyperLink> linkBuffer = new List<HyperLink>();

        public LinkCollection BodyLinks = new LinkCollection();

        private int listDepth = 0;

        Buffer buffer = new Buffer();

        private bool inPreformatted = false;

        private int linkCounter = 0;

        public bool ShouldRenderHyperlinks { get; set; } = true;
        public bool AllowDuplicateLinks { get; set; } = false;

        public void Parse(INode current)
        {
            ParseHelper(current);
        }

        public List<ContentItem> GetItems()
        {
            FlushBuffer();           
            return items;
        }

        private void FlushBuffer()
        {
            FlushLinkBuffer();
            if (buffer.HasContent)
            {
                items.Add(new ContentItem { Content = buffer.Content });
                buffer.Reset();
            }
        }

        private void FlushLinkBuffer()
        {
            if(linkBuffer.Count > 0 && !buffer.InBlockquote)
            {
                buffer.EnsureAtLineStart();
                foreach(var link in linkBuffer)
                {
                    buffer.AppendLine($"=> {link.Url.AbsoluteUri} [{link.OrderDetected}] {link.Text}");
                }
                linkBuffer.Clear();
            }
        }

        private void ParseHelper(INode current)
        {
            switch (current.NodeType)
            {
                case NodeType.Text:
                    ProcessTextNode(current);
                    break;

                case NodeType.Element:
                    ProcessHtmlElement(current as HtmlElement);
                    break;
            }
        }

        private void ParseChildern(INode node)
        {
            foreach (var child in node.ChildNodes)
            {
                ParseHelper(child);
            }
        }

        private void ProcessTextNode(INode textNode)
        {
            if (inPreformatted)
            {
                buffer.Append(textNode.TextContent);
            }
            else
            {
                //if its not only whitespace add it.
                if (textNode.TextContent.Trim().Length > 0)
                {
                    var text = StringUtils.RemoveNewlines(textNode.TextContent);
                    if (buffer.AtLineStart)
                    {
                        buffer.Append(text.TrimStart());
                    }
                    else
                    {
                        buffer.Append(text);
                    }
                }
                //if its whitepsace, but doesn't have a newline
                else if (!textNode.TextContent.Contains('\n'))
                {
                    if (buffer.AtLineStart)
                    {
                        buffer.Append(textNode.TextContent.TrimStart());
                    }
                    else
                    {
                        buffer.Append(textNode.TextContent);
                    }
                }
            }
        }

        private void ProcessHtmlElement(HtmlElement element)
        {
            var nodeName = element?.NodeName.ToLower();

            if (!ShouldProcessElement(element, nodeName))
            {
                return;
            }

            switch (nodeName)
            {
                case "a":
                    ProcessAnchor(element);
                    break;

                case "blockquote":
                    buffer.EnsureAtLineStart();
                    buffer.InBlockquote = true;
                    ParseChildern(element);
                    buffer.InBlockquote = false;
                    break;

                case "br":
                    buffer.AppendLine();
                    break;

                case "dd":
                    buffer.EnsureAtLineStart();
                    buffer.SetLineStart("* ");
                    ParseChildern(element);
                    buffer.EnsureAtLineStart();
                    break;

                case "dt":
                    buffer.EnsureAtLineStart();
                    ParseChildern(element);
                    if (!buffer.AtLineStart)
                    {
                        buffer.AppendLine(":");
                    }
                    break;

                case "figure":
                    ProcessFigure(element);
                    break;


                case "h1":
                    buffer.EnsureAtLineStart();
                    buffer.SetLineStart("# ");
                    ParseChildern(element);
                    break;

                case "h2":
                    buffer.EnsureAtLineStart();
                    buffer.SetLineStart("## ");
                    ParseChildern(element);
                    break;

                case "h3":
                    buffer.EnsureAtLineStart();
                    buffer.SetLineStart("### ");
                    ParseChildern(element);
                    break;
                
                case "i":
                    if (ShouldUseItalics(element))
                    {
                        buffer.Append("\"");
                        ParseChildern(element);
                        buffer.Append("\"");
                    }
                    else
                    {
                        ParseChildern(element);
                    }
                    break;

                case "img":
                    ProcessImg(element);
                    break;

                case "li":
                    ProcessLi(element);
                    break;

                case "ol":
                    ProcessList(element);
                    break;

                case "p":
                    buffer.EnsureAtLineStart();
                    int size = buffer.Content.Length;
                    ParseChildern(element);
                    //make sure the paragraph ends with a new line
                    buffer.EnsureAtLineStart();
                    if (buffer.Content.Length > size)
                    {
                        FlushLinkBuffer();
                        //add another blank line if this paragraph had content
                        buffer.AppendLine();
                    }
                    break;

                case "pre":
                    buffer.EnsureAtLineStart();
                    buffer.AppendLine("```");
                    inPreformatted = true;
                    ParseChildern(element);
                    buffer.EnsureAtLineStart();
                    inPreformatted = false;
                    buffer.AppendLine("```");
                    break;

                case "sub":
                    ProcessSub(element);
                    break;

                case "sup":
                    ProcessSup(element);
                    break;

                case "u":
                    buffer.Append("_");
                    ParseChildern(element);
                    buffer.Append("_");
                    break;

                case "ul":
                    ProcessList(element);
                    break;

                //skipping
                case "table":
                case "noscript":
                    return;

                default:
                    ProcessGenericTag(element);
                    break;
            }
        }

        public bool ShouldProcessElement(HtmlElement element,string normalizedTagName)
        {
            //A MathElement is of type element, but it not an HtmlElement
            //so it will be null
            if (element == null)
            {
                return false;
            }

            //see if we are explicitly filtering
            if (!DomFilter.Global.IsElementAllowed(element, normalizedTagName))
            {
                return false;
            }

            //check the ARIA role
            if(ShouldSkipRole(element.GetAttribute("role")?.ToLower()))
            {
                return false;
            }

            //is it visible?
            if (IsInvisible(element))
            {
                return false;
            }

            return true;
        }

        private bool ShouldSkipRole(string role)
        {
            if(string.IsNullOrEmpty(role))
            {
                return false;
            }
            if(role is "button" or
                "checkbox" or
                "form" or
                "searchbox" or
                "search" or
                "slider" or
                "switch")
            {
                return true;
            }
            return false;
        }

        //should we use apply italic formatting around this element?
        private bool ShouldUseItalics(HtmlElement element)
        {
            var siblingTag = element.NextElementSibling?.NodeName?.ToLower() ?? "";
            if(siblingTag == "sub" || siblingTag == "sup")
            {
                return false;
            }
            return true;
        }

        private static bool IsInvisible(HtmlElement element)
           => element.GetAttribute("style")?.Contains("display:none") ?? false;

        private void ProcessAnchor(HtmlElement anchor)
        {
            ParseChildern(anchor);
            //
            //we only care about meaningful links
            //so we can check to see if this anchor had any non-whitespace text
            //(note, A tags with only an IMG inside is common, but we handle that
            //by have a link to media already. No reason to also have a hyperlink
            if (anchor.TextContent.Trim().Length > 0)
            {
                var link = CreateLink(anchor);
                if (link != null)
                {
                    if (ShouldRenderHyperlinks)
                    {
                        buffer.Append($"[{link.OrderDetected}]");
                        linkBuffer.Add(link);
                    }
                    BodyLinks.AddLink(link);
                }
            }
        }

        private void ProcessGenericTag(HtmlElement element)
        {
            if (ShouldDisplayAsBlock(element))
            {
                buffer.EnsureAtLineStart();
                ParseChildern(element);
                buffer.EnsureAtLineStart();
            }
            else
            {
                ParseChildern(element);
            }
        }

        private void ProcessFigure(HtmlElement figure)
        {
            var media = MediaConverter.ConvertFigure(figure);
            if (media != null && ShouldUseImage(media))
            {
                Images.Add(media);
                buffer.EnsureAtLineStart();
                buffer.AppendLine($"=> {GetMediaPath(media.Url)} Image: {media.Caption}");
            }
        }

        private void ProcessImg(HtmlElement img)
        {
            var media = MediaConverter.ConvertImg(img);
            if(media != null && ShouldUseImage(media))
            {
                Images.Add(media);
                buffer.EnsureAtLineStart();
                buffer.AppendLine($"=> {GetMediaPath(media.Url)} Image: {media.Caption}");
            }
        }

        private void ProcessLi(HtmlElement li)
        {
            if (listDepth == 1)
            {
                buffer.EnsureAtLineStart();
                buffer.SetLineStart("* ");
                ParseChildern(li);
                buffer.EnsureAtLineStart();
            }
            else
            {
                buffer.EnsureAtLineStart();
                buffer.SetLineStart("* * ");
                ParseChildern(li);
                buffer.EnsureAtLineStart();
            }
        }

        private void ProcessList(HtmlElement element)
        {
            //block element
            buffer.EnsureAtLineStart();
            listDepth++;
            ParseChildern(element);
            listDepth--;
            buffer.EnsureAtLineStart();
        }

        private void ProcessSub(HtmlElement element)
        {
            var content = element.TextContent.Trim();
            if (content.Length > 0) {
                var subConverter = new SubscriptConverter();
                if (subConverter.Convert(content))
                {
                    //we successfully converted everything
                    buffer.Append(subConverter.Converted);
                }
                //couldn't convert, fall back to using ⌄ ...
                else if (content.Length == 1)
                {
                    buffer.Append("˅");
                    buffer.Append(content);
                }
                else
                {
                    buffer.Append("˅(");
                    buffer.Append(content);
                    buffer.Append(")");
                }
            }
        }

        private void ProcessSup(HtmlElement element)
        {
            var content = element.TextContent.Trim();
            if (content.Length > 0)
            {
                var supConverter = new SuperscriptConverter();
                if (supConverter.Convert(content))
                {
                    //we successfully converted everything
                    buffer.Append(supConverter.Converted);
                }
                //couldn't convert, fall back to using ^...
                else if (content.Length == 1)
                {
                    buffer.Append("^");
                    buffer.Append(content);
                }
                else
                {
                    buffer.Append("^(");
                    buffer.Append(content);
                    buffer.Append(")");
                }
            }
        }

        public bool ShouldUseImage(MediaItem image)
            => (Images.Where(x => (x.Url == image.Url)).FirstOrDefault() == null);

        public static bool ShouldDisplayAsBlock(HtmlElement element)
        {
            var nodeName = element.NodeName.ToLower();
            if (!blockElements.Contains(nodeName))
            {
                return false;
            }
            //its a block, display it as inline?
            return !IsInline(element);
        }

        private HyperLink CreateLink(HtmlElement a)
        {
            Uri url = null;
            try
            {
                url = new Uri(a.GetAttribute("href"));
            } catch (Exception) {
                url = null;
            }
            if(url == null)
            {
                return null;
            }

            if (!AllowDuplicateLinks && BodyLinks.ContainsUrl(url))
            {
                return null;
            }

            linkCounter++;

            return new HyperLink
            {
                OrderDetected = linkCounter,
                Text = a.TextContent.Trim(),
                Url = url
            };
        }

        private string GetMediaPath(string url)
            => MediaRewriter.GetPath(url);

        private static bool IsInline(HtmlElement element)
            => element.GetAttribute("style")?.Contains("display:inline") ?? false;
    }
}


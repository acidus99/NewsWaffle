using System;
using System.Linq;
using System.Collections.Generic;

namespace NewsWaffle.Models
{
    public class LinkCollection
    {
        public int Count => links.Count;

        Dictionary<Uri, HyperLink> links = new Dictionary<Uri, HyperLink>();

        Dictionary<string, bool> seenText = new Dictionary<string, bool>();

        

        int counter = 0;

        public void AddLink(HyperLink link)
            => AddLink(link.Url, link.Text);

        public void AddLink(Uri url, string linkText)
        {
            string normalized = linkText.ToLower();

            //does this Url already exist?
            if (!ContainsUrl(url))
            {
                //has this link text already been used?
                if (!seenText.ContainsKey(normalized))
                {

                    seenText.Add(normalized, true);
                    //add it
                    counter++;
                    links.Add(url, new HyperLink
                    {
                        Text = linkText,
                        Url = url,
                        OrderDetected = counter
                    });
                }
            }
            else if (!seenText.ContainsKey(normalized))
            {
                //URL already exists, but link text doesn't.
                //so is this link text "better" that what we are using?
                if (links[url].Text.Length < linkText.Length)
                {
                    links[url].Text = linkText;
                    seenText.Add(normalized, true);
                }
            }
        }

        public bool ContainsUrl(Uri url)
            => links.ContainsKey(url);

        public void RemoveLinks(List<HyperLink> linksToRemove)
            => linksToRemove.ForEach(x => links.Remove(x.Url));

        public List<HyperLink> GetLinks()
            => links.Values.OrderBy(x => x.OrderDetected).ToList();
    }
}
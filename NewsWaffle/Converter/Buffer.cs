using System;
using System.Text;

using NewsWaffle.Models;

namespace NewsWaffle.Converter
{
    public class Buffer
    {
        public string Content => sb.ToString();

        public bool HasContent => (sb.Length > 0);

        public bool AtLineStart
            => !HasContent || Content.EndsWith('\n');

        public bool InBlockquote { get; set; } = false;

        private StringBuilder sb;

        private string lineStart = null;

        public Buffer()
        {
            sb = new StringBuilder();
        }

        public void Reset()
        {
            sb.Clear();
            lineStart = null;
        }

        public void SetLineStart(string s)
        {
            lineStart = s;
        }

        public void Append(string s)
        {
            if(s.Contains('\n'))
            {
                foreach(string sub in s.Split('\n'))
                {
                    AppendLine(sub);
                }
                return;
            }

            HandleLineStart(s);
            HandleBlockQuote(s);
            sb.Append(s);
        }

        public void AppendLine(string s = "")
        {
            HandleLineStart(s);
            HandleBlockQuote(s);
            sb.AppendLine(s);
        }

        public void EnsureAtLineStart()
        {
            if(AtLineStart && lineStart != null)
            {
                lineStart = null;
            }

            if (!AtLineStart)
            {
                sb.AppendLine();
            }
        }

        public void HandleLineStart(string s)
        {
            //if we are adding something that is not whitespace, and we have a prefix
            if(lineStart != null)
            {
                sb.Append(lineStart);
                lineStart = null;
            }
        }

        private void HandleBlockQuote(string s)
        {
            if (InBlockquote && AtLineStart && s.Trim().Length > 0)
            {
                sb.Append(">");
            }
        }
    }
}

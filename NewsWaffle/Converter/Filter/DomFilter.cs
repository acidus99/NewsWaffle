using System;
using System.Collections.Generic;

using System.Net;
using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;


namespace NewsWaffle.Converter.Filter

{
	public class DomFilter
	{

        public static DomFilter Global = new DomFilter();

		Dictionary<String, List<FilterRule>> TagFilters;

		List<FilterRule> JustClassRules;

        List<FilterRule> JustIDs;

		public DomFilter()
		{
            TagFilters = new Dictionary<string, List<FilterRule>>();
            JustClassRules = new List<FilterRule>();
            JustIDs = new List<FilterRule>();
		}

        public bool IsElementAllowed(HtmlElement element, string normalizedTagName)
        {
            //check for tag-specific rules
            if (TagFilters.ContainsKey(normalizedTagName))
            {
                foreach (var rule in TagFilters[normalizedTagName])
                {
                    if (rule.HasClass)
                    {
                        if (element.ClassList.Contains(rule.ClassName))
                        {
                            return false;
                        }
                    }
                    else if (rule.HasID)
                    {
                        if ((element.Id ?? "") == rule.ID)
                        {
                            return false;
                        }
                    }
                }
            }

            if (element.ClassList.Length > 0)
            {
                foreach (var rule in JustClassRules)
                {
                    if (element.ClassList.Contains(rule.ClassName))
                    {
                        return false;
                    }
                }
            }
            if(!string.IsNullOrEmpty(element.Id))
            {
                foreach (var rule in JustIDs)
                {
                    if (element.Id == rule.ID)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

		public void AddRule(string selector)
        {
			string tag = "";
			string cls ="";
			string id = "";

            if (selector.Contains("."))
            {
                tag = ClipBefore(selector, ".").ToLower();
                cls = ClipAfter(selector, ".");
            }
            else if (selector.Contains("#"))
            {
                tag = ClipBefore(selector, "#").ToLower();
                id = ClipAfter(selector, "#");
            }
            else
            {
                tag = selector.ToLower();
            }

            var rule = new FilterRule
            {
                TagName = tag,
                ClassName = cls,
                ID = id
            };

            if(rule.HasTag)
            {
                if(!TagFilters.ContainsKey(rule.TagName))
                {
                    TagFilters[rule.TagName] = new List<FilterRule>();
                }
                TagFilters[rule.TagName].Add(rule);
            } else if(rule.HasClass)
            {
                JustClassRules.Add(rule);
            } else if(rule.HasID)
            {
                JustIDs.Add(rule);
            }
        }

        private string ClipAfter(string s, string c)
        {
            int x = s.IndexOf(c);
            if (x >= 0 && x + 1 != s.Length)
            {
                return s.Substring(x + 1);
            }
            return string.Empty;
        }

        private string  ClipBefore(string s, string c)
        {
            int x = s.IndexOf(c);
            return x > 0 ? s.Substring(0, x) : String.Empty;
        }
    }
}


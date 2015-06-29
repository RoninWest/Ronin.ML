using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using HtmlAgilityPack;

namespace Ronin.ML.Util
{
	/// <summary>
	/// Given a URL, pull the text content and strip it clean of HTML marking if present
	/// </summary>
	class WebTextExtractor
	{
		public string Extract(Stream data)
        {
            var doc = new HtmlDocument();
            doc.Load(data);
            HtmlNode body = doc.DocumentNode.Descendants("body").FirstOrDefault();
            if (body != null && body.InnerText != null)
                return CleanParse(body);
            else
                return CleanParse(doc.DocumentNode); //return everything
        }

		/// <summary>
		/// clean first then return parsed result
		/// </summary>
		string CleanParse(HtmlNode node)
		{
			if (node == null)
				return null;

			Clean(node);

            return HttpUtility.HtmlEncode(node.InnerText);
		}

		/// <summary>
		/// Black list
		/// </summary>
		readonly string[] REMOVE_NODES = new[] { "script", "link", "style", "code", "nav", "menu", "header", "footer", "aside" };

		/// <summary>
		/// Recursive cleaning of unwanted content by tag black list
		/// </summary>
		void Clean(HtmlNode node)
		{
			if (node == null)
				return;

			foreach (string r in REMOVE_NODES)
			{
				HtmlNode cn;
				while ((cn = node.Element(r)) != null)
				{
					node.RemoveChild(cn);
				}
            }

			IEnumerable<HtmlNode> children = node.Descendants();
			if (children != null)
			{
				foreach (HtmlNode c in children)
				{
					Clean(c);
				}
			}
		}
    }
}

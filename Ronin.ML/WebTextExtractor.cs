using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RestSharp;
using HtmlAgilityPack;

namespace Ronin.ML
{
	/// <summary>
	/// Given a URL, pull the text content and strip it clean of HTML marking if present
	/// </summary>
	public class WebTextExtractor
	{
		/// <summary>
		/// Instanciate with base path URL
		/// </summary>
		/// <param name="basePath"></param>
		/// <remarks>base path is required due to rest-sharp work flow.  It is possible to add authentication support here later.</remarks>
		public WebTextExtractor(Uri basePath) 
			: this(basePath == null ? null : basePath.ToString())
		{
		}

		readonly RestClient _client;

		/// <summary>
		/// Instanciate with base path URL
		/// </summary>
		/// <param name="basePath"></param>
		/// <remarks>base path is required due to rest-sharp work flow.  It is possible to add authentication support here later.</remarks>
		public WebTextExtractor(string basePath)
		{
			if (string.IsNullOrWhiteSpace(basePath))
				throw new ArgumentException("basePath can not be null or blank!");
			if (!Uri.IsWellFormedUriString(basePath, UriKind.Absolute))
				throw new ArgumentOutOfRangeException("basePath is not an absolute URL!");

			_client = new RestClient(basePath);
        }

		/// <summary>
		/// Pull and clean text content using relative URL
		/// </summary>
		/// <remarks>If URL is not relative, this method will throw! Null is ok.</remarks>
		public string Get(Uri path)
		{
			return Get(path != null ? path.ToString() : null);
		}

		/// <summary>
		/// Does a simple get to base url given
		/// </summary>
		public string Get()
		{
			string s = null;
			return Get(s);
		}

		readonly Regex CONTENT_OK_RE = new Regex(@"(xml|xhtml|html|text)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <summary>
		/// Pull and clean text content using relative URL
		/// </summary>
		/// <remarks>If URL is not relative, this method will throw! Null is ok.</remarks>
		public string Get(string path)
		{
			if (!string.IsNullOrWhiteSpace(path) && !Uri.IsWellFormedUriString(path, UriKind.Absolute))
				throw new ArgumentOutOfRangeException("Path can only be null or relative. Absolute path is not supported.");

			var request = new RestRequest(path, Method.GET);
			IRestResponse response = _client.Execute(request);

			if (!string.IsNullOrWhiteSpace(response.Content) && !string.IsNullOrWhiteSpace(response.ContentType) && CONTENT_OK_RE.IsMatch(response.ContentType))
			{
				var doc = new HtmlDocument();
				doc.LoadHtml(response.Content);
				HtmlNode body = doc.DocumentNode.Descendants("body").FirstOrDefault();
				if (body != null && body.InnerText != null)
					return CleanParse(body);
				else
					return CleanParse(doc.DocumentNode); //return everything
            }
			else //return raw
				return response.Content;
		}

		string CleanParse(HtmlNode node)
		{
			if (node == null)
				return null;

			Clean(node);
			return node.InnerText;
		}

		void Clean(HtmlNode node)
		{
		}
    }
}

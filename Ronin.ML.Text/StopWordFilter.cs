using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Ronin.ML.Text
{
	/// <summary>
	/// Remove all common language specific stop words from the input
	/// <see cref="https://code.google.com/p/stop-words/"/>
	/// </summary>
	public class StopWordFilter : IgnoreWordFilter
	{
		/// <summary>
		/// Instantiate with language
		/// </summary>
		public StopWordFilter(IWordProcessor processor, TextLanguage lang)
			: base(processor, new WhiteSpaceTokenizer(), GetLanguageFiles(lang))
		{
		}

		static FileInfo[] GetLanguageFiles(TextLanguage lang)
		{
			string basePath = Path.Combine(AssemblyDirectory, @"StopWords");
			var root = new DirectoryInfo(basePath);
			if (root.Exists)
			{
				string defaultLang = TextLanguage.Default.ToString().ToLower();

				string languageName = lang.ToString().ToLower();
				if (languageName == defaultLang)
					languageName = "english";

				string searchExp = '*' + languageName + '*';
				return (from f in root.EnumerateFiles(searchExp, SearchOption.AllDirectories)
						select f).ToArray();
			}
			else
				return new FileInfo[0];
		}

		static string AssemblyDirectory
		{
			get
			{
				string codeBase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}

	}
}

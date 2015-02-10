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
	/// Remove all stop words and subsequent processing of them.
	/// <see cref="https://code.google.com/p/stop-words/"/>
	/// </summary>
	public class StopWordNormalizer : IWordNormalizer
	{
		/// <summary>
		/// Instantiate with language
		/// </summary>
		public StopWordNormalizer(IWordNormalizer processor, TextLanguage lang)
			: this(processor, new WhiteSpaceTokenizer(), GetLanguageFiles(lang))
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

				return (from f in root.EnumerateFiles('*' + languageName + '*', SearchOption.AllDirectories)
						//where ".txt" == f.Extension
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

		/*
		//TODO: later feature to let someone else manage this list if available remotely
		/// <summary>
		/// Instantiate with remote files
		/// </summary>
		public StopWordNormalizer(IWordNormalizer processor, IStringTokenizer chunker, params Uri[] files)
			: this(processor, ExtractWords(chunker, GetStreams(files)))
		{
		}

		static IEnumerable<Stream> GetStreams(params Uri[] paths)
		{
			throw new NotImplementedException();
		}
		*/

		/// <summary>
		/// Instantiate with local files
		/// </summary>
		public StopWordNormalizer(IWordNormalizer processor, IStringTokenizer chunker, params FileInfo[] files)
			: this(processor, ExtractWords(chunker, GetStreams(files)))
		{
		}

		static IEnumerable<Stream> GetStreams(params FileInfo[] paths)
		{
			return from p in paths
				   where p != null && p.Exists
				   let fs = p.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
				   select fs;
		}

		static ICollection<string> ExtractWords(IStringTokenizer chunker, IEnumerable<Stream> streams)
		{
			if (chunker == null)
				throw new ArgumentNullException("chunker");

			var words = new LinkedList<string>();
			streams.ForEach(s => AppendWords(words, s, chunker));
			return words;
		}

		static void AppendWords(ICollection<string> words, Stream s, IStringTokenizer chunker)
		{
			if (s == null || !s.CanRead)
				return;

			string txt;
			using (var sr = new StreamReader(s))
			{
				txt = sr.ReadToEnd();
				sr.Close();
			}

			IEnumerable<WordToken> tokens = chunker.Process(txt);
            foreach (WordToken wt in tokens)
			{
				words.Add(wt.Word);
			}
		}

		readonly HashSet<string> _stops = new HashSet<string>();
		readonly IWordNormalizer _processor;

		/// <summary>
		/// Instantiate with static stop words
		/// </summary>
		public StopWordNormalizer(IWordNormalizer processor, IEnumerable<string> words)
		{
			_processor = processor;
			if (processor != null)
			{
				words.ForEach(w =>
				{
					var wc = new WordContext(w);
					processor.Process(wc);
					if (!string.IsNullOrWhiteSpace(wc.Result))
						_stops.Add(wc.Result);
				});
			}
			else
				words.ForEach(w => _stops.Add(w));
		}

		/// <summary>
		/// Remove all stop words
		/// </summary>
		public void Process(WordContext word)
		{
			if (word.Result == null)
				return;

			if (_stops.Contains(word.Result))
				word.Result = null;
			else if (_processor != null)
				_processor.Process(word);
		}
	}
}

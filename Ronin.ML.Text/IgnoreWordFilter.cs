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
	/// Simply ignore any word that is loaded into this filter
	/// </summary>
	public class IgnoreWordFilter : IWordProcessor
	{
		/// <summary>
		/// Instantiate with local files
		/// </summary>
		public IgnoreWordFilter(IWordProcessor processor, IStringTokenizer chunker, params FileInfo[] files)
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

		/*
		//TODO: later feature to let someone else manage this list if available remotely
		/// <summary>
		/// Instantiate with remote files
		/// </summary>
		public IgnoreWordFilter(IWordNormalizer processor, IStringTokenizer chunker, params Uri[] files)
			: this(processor, ExtractWords(chunker, GetStreams(files)))
		{
		}

		static IEnumerable<Stream> GetStreams(params Uri[] paths)
		{
			throw new NotImplementedException();
		}
		*/

		readonly HashSet<string> _stops = new HashSet<string>();
		readonly IWordProcessor _processor;

		/// <summary>
		/// Instantiate with static stop words
		/// </summary>
		public IgnoreWordFilter(IWordProcessor processor, IEnumerable<string> words)
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
		public virtual void Process(WordContext word)
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

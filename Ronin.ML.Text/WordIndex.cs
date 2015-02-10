using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronin.ML.Text
{
	/// <summary>
	/// Inverted word index that contains the word itself as keys and relative positions (from the begining of the document) where they appears
	/// </summary>
	public class WordIndex : IDictionary<string, ICollection<int>>
	{
		readonly Dictionary<string, HashSet<int>> _m = new Dictionary<string, HashSet<int>>();

		public ICollection<int> this[string key]
		{
			get { return _m[key]; }
			set
			{
				_m[key] = new HashSet<int>(value);
			}
		}

		public int Count { get { return _m.Count; } }

		public bool IsReadOnly { get { return false; } }

		public ICollection<string> Keys { get { return _m.Keys; } }

		public ICollection<ICollection<int>> Values
		{
			get
			{
				return (from h in _m.Values select h as ICollection<int>).ToArray();
			}
		}

		public bool Add(string word, int position)
		{
			if (string.IsNullOrWhiteSpace(word))
				return false;

			if (_m.ContainsKey(word))
				return _m[word].Add(position);
			else
			{
				var h = new HashSet<int>();
				h.Add(position);
				_m.Add(word, h);
				return true;
			}
		}

		public void Add(KeyValuePair<string, ICollection<int>> item)
		{
			Add(item.Key, item.Value);
		}

		public void Add(string key, ICollection<int> value)
		{
			if (string.IsNullOrWhiteSpace(key))
				return;

			_m.Add(key, new HashSet<int>(value));
		}

		public void Clear()
		{
			_m.Clear();
		}

		public bool Contains(KeyValuePair<string, ICollection<int>> item)
		{
			throw new NotImplementedException();
		}

		public bool ContainsKey(string key)
		{
			return _m.ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<string, ICollection<int>>[] array, int arrayIndex)
		{
			_m.ToArray().CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<string, ICollection<int>>> GetEnumerator()
		{
			return (from p in _m select new KeyValuePair<string, ICollection<int>>(p.Key, p.Value)).GetEnumerator();
		}

		public bool Remove(KeyValuePair<string, ICollection<int>> item)
		{
			throw new NotImplementedException();
		}

		public bool Remove(string key)
		{
			if (string.IsNullOrWhiteSpace(key))
				return false;

			return _m.Remove(key);
		}

		public bool TryGetValue(string key, out ICollection<int> value)
		{
			HashSet<int> h;
			bool r = _m.TryGetValue(key, out h);
			value = h;
			return r;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}

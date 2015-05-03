using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intensity
{
    public class ListDictionary<TKey, TValue>
    {
        List<TValue> _list = new List<TValue>();
        Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public List<TValue> ToList()
        {
            return _list;
        }

        public bool ContainsKey(TKey b)
        {
            return _dictionary.ContainsKey(b);
        }

        public TValue this[TKey i]
        {
            get { return _dictionary[i]; }
            set { if (!_dictionary.ContainsKey(i)) { _list.Add(value); } _dictionary[i] = value; }
        }

        public ListDictionary<TKey, TValue> Where(Func<TValue, bool> func)
        {
            ListDictionary<TKey, TValue> ret = new ListDictionary<TKey, TValue>();
            foreach (var tkey in _dictionary.Keys)
            {
                var val = _dictionary[tkey];
                if (func(val)) { ret[tkey] = val; }
            }
            return ret;
        }

        public int Count { get { return _list.Count; } }
    }
}

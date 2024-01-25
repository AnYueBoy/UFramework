using System.Collections;
using System.Collections.Generic;

namespace UFramework
{
    public class ParamsCollection : IParams, IEnumerable<KeyValuePair<string, object>>
    {
        private readonly IDictionary<string, object> collection;

        public ParamsCollection()
        {
            collection = new Dictionary<string, object>();
        }

        public ParamsCollection(IDictionary<string, object> mapping)
        {
            collection = mapping;
        }

        public object this[string key]
        {
            get => collection[key];
            set => collection[key] = value;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string key, object value)
        {
            collection.Add(key, value);
        }

        public bool Remove(string key)
        {
            return collection.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return collection.TryGetValue(key, out value);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class BlackBoard
    {
        private Dictionary<string, object> data = new Dictionary<string, object>();

        public object this[string key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Set(key, value);
            }
        }

        public bool Isset(string key)
        {
            return this.data.ContainsKey(key);
        }

        public void Set(string key)
        {
            if (!Isset(key))
            {
                Set(key, null);
            }
        }

        public object Get(string key)
        {
            if (this.data.ContainsKey(key))
            {
                return data[key];
            }
            else
            {
                return null;
            }
        }

        public void Set(string key, object value)
        {
            if (!this.data.ContainsKey(key))
            {
                this.data[key] = value;
            }
            else
            {
                if ((this.data[key] == null && value != null) || (this.data[key] != null && !this.data[key].Equals(value)))
                {
                    this.data[key] = value;
                }
            }
        }
    }
}
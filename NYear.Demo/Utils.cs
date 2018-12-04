using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;


namespace NYear.Demo
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DemoAttribute : Attribute
    {
        public FuncType Demo { get; set; }
        public string MethodName { get; set; }
        public string MethodDescript { get; set; }
    }
    public enum FuncType
    {
        Insert,
        Delete,
        Update,
        Select,
        Procedure,
        Customize,
        Advantage,
    }

    public class DemoMethodInfo
    {
        public FuncType DemoFunc { get; set; }
        public string MethodName { get; set; }
        public string MethodDescript { get; set; }
        public MethodInfo DemoMethod { get; set; }
    } 

    public class OModel : DynamicObject, IEnumerable<KeyValuePair<string,object>>
    {
        private Dictionary<string, object> storage = new Dictionary<string, object>();
        public override bool TryGetMember(GetMemberBinder binder, out object result) 
        {
            if (storage.ContainsKey(binder.Name)) 
            {
                result = storage[binder.Name]; 
                return true; 
            } 
            result = null; 
            return false; 
        } 
        public override bool TrySetMember(SetMemberBinder binder, object value) 
        {
            string key = binder.Name; 
            if (storage.ContainsKey(key)) 
                storage[key] = value; 
            else 
                storage.Add(key, value); 
            return true; 
        } 
        public override string ToString() 
        {
            StringWriter message = new StringWriter(); 
            foreach (var item in storage)
                message.WriteLine("{0}:{1}", item.Key, item.Value); 
            return message.ToString(); 
        } 
        public int Count
        {
            get
            {
                return storage.Count; 
            }
        }

        public void Add(string key, object value)
        {
            storage.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return storage.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return storage.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return storage.TryGetValue(key,out value);
        }

        public void Clear()
        {
            storage.Clear();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, object>>)storage).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, object>>)storage).GetEnumerator();
        }
    }
}

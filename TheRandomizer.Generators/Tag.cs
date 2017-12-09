using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheRandomizer.Utility;

namespace TheRandomizer.Generators
{
    public class Tag : ObservableBase
    {

        public static implicit operator string(Tag value)
        {
            return value.Value;
        }

        public static implicit operator Tag(string value)
        {
            return new Tag(value);
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (obj.GetType() == typeof(Tag))
                {
                    return Value == ((Tag)obj).Value;
                }
                else if (obj.GetType() == typeof(string))
                {
                    return Value == (string)obj;
                }                    
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            if (Value == null) return 0;
            return Value.GetHashCode();
        }

        public Tag() { }
        public Tag(string value) { Value = value; }
        
        [XmlText]
        public string Value { get { return GetProperty<string>(); } set { SetProperty(value); } }
    }
}

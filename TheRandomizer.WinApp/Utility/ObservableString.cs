using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRandomizer.Utility;

namespace TheRandomizer.WinApp.Utility
{
    public class ObservableString : ObservableBase
    {
        public ObservableString()
        {
            Content = string.Empty;
        }

        public ObservableString(string content)
        {
            Content = content;
        }

        public string Content { get { return GetProperty<string>(); } set { SetProperty(value); } }

        public override bool Equals(object obj)
        {
            if (obj.Equals(null)) return false;
            if (obj.GetType() == typeof(ObservableString))
            {
                return Content == ((ObservableString)obj).Content;
            }
            else if (obj.GetType() == typeof(string))
            {
                return Content == (string)obj;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Content.GetHashCode();
        }

        public static explicit operator ObservableString(string content)
        {
            return new ObservableString(content);
        }

        public static explicit operator string(ObservableString value)
        {
            return value.Content;
        }

        public static bool operator ==(ObservableString op1, ObservableString op2)
        {
            if (op1.Equals(null) && op2.Equals(null)) return true;
            if (!op1.Equals(null)) return op1.Equals(op2);
            return false;
        }

        public static bool operator !=(ObservableString op1, ObservableString op2)
        {
            return !(op1 == op2);
        }

        public static bool operator ==(ObservableString op1, string op2)
        {
            if (op1.Equals(null) && string.IsNullOrEmpty(op2)) return true;
            if (!op1.Equals(null)) return op1.Equals(op2);
            return false;
        }

        public static bool operator !=(ObservableString op1, string op2)
        {
            return !(op1 == op2);
        }
    }
}

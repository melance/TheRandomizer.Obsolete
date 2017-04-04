using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace TheRandomizer.WebApp.HelperClasses
{
    public class AuthenticationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }

        [ConfigurationProperty("id", IsRequired = true)]
        public string Id
        {
            get
            {
                var enc = new Utility.SimpleEncryption();
                return enc.Decrypt(this["id"] as string);
            }
        }

        [ConfigurationProperty("secret", IsRequired = true)]
        public string Secret
        {
            get
            {
                var enc = new Utility.SimpleEncryption();
                return enc.Decrypt(this["secret"] as string);
            }
        }
    }
}
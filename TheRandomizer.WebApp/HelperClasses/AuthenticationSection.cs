using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace TheRandomizer.WebApp.HelperClasses
{
    public class AuthenticationSection : ConfigurationSection
    {
        [ConfigurationProperty("authenticationItems", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(AuthenticationCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public AuthenticationCollection AuthenticationItems
        {
            get
            {
                return (AuthenticationCollection)base["authenticationItems"];
            }
        }
    }
}
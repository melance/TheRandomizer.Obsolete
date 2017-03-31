﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace TheRandomizer.WebApp.HelperClasses
{
    public class AuthenticationCollection : ConfigurationElementCollection
    {
        public AuthenticationCollection()
        {

        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AuthenticationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AuthenticationElement)element).Name;
        }

        public AuthenticationElement this[int index]
        {
            get
            {
                return (AuthenticationElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public AuthenticationElement this[string name]
        {
            get
            {
                return (AuthenticationElement)BaseGet(name);
            }
        }
    }
}
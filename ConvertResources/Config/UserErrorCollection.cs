using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ConvertResources.Config
{
    /// <summary>
    /// Custom configuration collection class to represent a collection of error code to user error mapping.
    /// </summary>
    public class UserErrorCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public UserErrorCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        { }

        /// <summary>
        /// Gets the type of collection.
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        /// <summary>
        /// Creates a new configuration Element of type UserErrorElement.
        /// </summary>
        /// <returns>New configuration Element of type UserErrorElement.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new UserErrorElement();
        }

        /// <summary>
        /// Gets the element key for UserErrorElement.
        /// </summary>
        /// <param name="element">ConfigurationElement object.</param>
        /// <returns>Key with which the element is identified.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((UserErrorElement)element).ErrorCode;
        }

        /// <summary>
        /// Gets or sets the UserErrorElement.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <returns>Returns an instance of UserErrorElement specified by the error code key.</returns>
        public new UserErrorElement this[string errorCode]
        {
            get
            {
                return BaseGet((object)errorCode) as UserErrorElement;
            }
            set
            {
                if (BaseGet((object)errorCode) == null)
                {
                    BaseAdd(value);
                }
            }
        }
    }
}
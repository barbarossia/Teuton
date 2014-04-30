using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ConvertResources.Config
{
    /// <summary>
    /// Defines configuration section for userErrorMessage.
    /// </summary>
    public class UserErrorConfigSection : ConfigurationSection
    {
        private static UserErrorConfigSection _current = ConfigurationManager.GetSection(
            UserErrorConfigConstant.NodeName.UserErrorMessageConfiguration) as UserErrorConfigSection;

        /// <summary>
        /// Gets the instance of userErrorMessage configuration section.
        /// </summary>
        public static UserErrorConfigSection Current
        {
            get
            {
                return _current;
            }
        }

        /// <summary>
        /// Gets the UserErrorMessageCollection configuration collection.
        /// </summary>
        [ConfigurationProperty(UserErrorConfigConstant.NodeName.Errors)]
        [ConfigurationCollection(typeof(UserErrorCollection))]
        public UserErrorCollection Errors
        {
            get { return this[UserErrorConfigConstant.NodeName.Errors] as UserErrorCollection; }
        }
    }
}
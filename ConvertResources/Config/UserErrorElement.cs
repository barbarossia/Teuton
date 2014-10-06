using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ConvertResources.Config
{
    /// <summary>
    /// Custom configuration element class to represent the error code to user error message map.
    /// </summary>
    public class UserErrorElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [ConfigurationProperty(UserErrorConfigConstant.AttributeName.ErrorCode, IsKey = true, IsRequired = true)]
        public string ErrorCode
        {
            get
            {
                return (string)this[UserErrorConfigConstant.AttributeName.ErrorCode];
            }
            set
            {
                this[UserErrorConfigConstant.AttributeName.ErrorCode] = value;
            }
        }

        /// <summary>
        /// Gets or sets the user error message.
        /// </summary>
        [ConfigurationProperty(UserErrorConfigConstant.AttributeName.UserMessage, IsRequired = true)]
        public string UserMessage
        {
            get
            {
                return (string)this[UserErrorConfigConstant.AttributeName.UserMessage];
            }
            set
            {
                this[UserErrorConfigConstant.AttributeName.UserMessage] = value;
            }
        }

        /// <summary>
        /// Gets or sets the fault type associated with the error code.
        /// </summary>
        [ConfigurationProperty(UserErrorConfigConstant.AttributeName.ErrorName, IsRequired = true)]
        public string ErrorName
        {
            get
            {
                return (string)this[UserErrorConfigConstant.AttributeName.ErrorName];
            }
            set
            {
                this[UserErrorConfigConstant.AttributeName.ErrorName] = value;
            }
        }       
    }
}
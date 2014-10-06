using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Common
{
    public static class ConfigManager
    {
        public static NameValueCollection AppSettings 
        { 
            get
            {
                return ConfigurationManager.AppSettings;
            } 
        }
    }
}

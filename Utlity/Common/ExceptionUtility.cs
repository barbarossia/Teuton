using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Logging;

namespace Utility.Common
{
    public static class ExceptionUtility
    {
        public static T IgnorException<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch
            {
            }

            return default(T);
        }

        public static T IgnorException<T>(Func<T> func, ILogger logger)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                logger.Log(MessageLevel.Error, ex.Message, ex);
            }

            return default(T);
        }
    }
}

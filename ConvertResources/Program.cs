using ConvertResources.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;
using Utility.IO;

namespace ConvertResources
{
    class Program
    {
        static void Main(string[] args)
        {
            var userErrorConfigCollection = UserErrorConfigSection.Current.Errors;
            GengerateCode(userErrorConfigCollection);
            GenerateScipt(userErrorConfigCollection, "D:\\[dbo].[ErrorMessage].sql");
            //Transform();
            Console.ReadLine();
        }

        static void Transform()
        {
            XslTransform myXslTransform = new XslTransform();
            myXslTransform.Load(@"Resources\errorMessage.config.xslt");
            myXslTransform.Transform(@"Resources\Resources.xml", @"Resources\errorMessage.config");

            myXslTransform.Load(@"Resources\errorMessageWithName.config.xslt");
            myXslTransform.Transform(@"Resources\Resources.xml", @"Resources\errorMessageWithName.config");
        }

        static void GengerateCode(UserErrorCollection collection)
        {
            GenCode sample = new GenCode();
            UserErrorElement error;
            foreach (var item in collection)
            {
                error = (UserErrorElement)item;
                sample.AddField(error.ErrorName, error.ErrorCode);
            }
            sample.GenerateCSharpCode(@"ErrorCode.cs");
        }

        static void GenerateScipt(UserErrorCollection collection, string file)
        {
            UserErrorElement error;
            StringBuilder sb = new StringBuilder();
            int index = 1;
            string permissionLineFormat = @"INSERT INTO [dbo].[ErrorMessage]([Id],[Code],[Description],[Suggestion],[Links],[SoftDelete],[InsertedByUserAlias],[InsertedDateTime],[UpdatedByUserAlias],[UpdatedDateTime])
  VALUES ({0}, '{1}', '{2}', null,null, 0, 'setup', GETDATE(), 'setup', GETDATE())";
            sb.AppendLine("SET IDENTITY_INSERT [dbo].[ErrorMessage] ON")
                .AppendLine("GO");
            foreach (var item in collection)
            {
                error = (UserErrorElement)item;
                sb.AppendLine(string.Format(permissionLineFormat, index++, error.ErrorCode, error.UserMessage.Replace("'","''")));
            }
            sb.AppendLine("GO")
                .AppendLine("SET IDENTITY_INSERT [dbo].[ErrorMessage] OFF")
                .AppendLine("GO")
                .AppendLine();

            sb.ToString().ToFile(file);

        }

    }
}

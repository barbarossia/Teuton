using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.IO;

namespace GenerateScript
{
    public class ScriptBase : IScript
    {
        public string Source { get; private set; }
        public ScriptBase(string source)
        {
            this.Source = source;
        }

        public virtual void Update(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            var files = Source.GetFiles();
            foreach (var file in files)
            {
                sb.AppendLine(file.ToText());
            }

            sb.ToString().ToFile(fileName);
        }

        public void Rollback(string fileName)
        {
        }
    }
}

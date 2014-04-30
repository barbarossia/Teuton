﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.IO;

namespace GenerateScript
{
    public class StoreProcedueScript: ScriptBase, IScript
    {
        public StoreProcedueScript(string source)
            : base(source)
        {
        }

        public override void Update(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            var files = Source.GetFiles();
            foreach (var file in files)
            {
                sb.AppendLine(file.ToText());
                sb.AppendLine(string.Format("GRANT EXECUTE ON {0} TO [MarketplaceService];", file.GetFileNameWithoutExtension()));
                sb.AppendLine("GO");
            }
            sb.ToString().ToFile(fileName);
        }
    }
}

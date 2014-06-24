using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateScript
{
    public class TableScripte : ScriptBase, IScript
    {
        public TableScripte(string source)
            : base(source)
        {
            base.Content = "Tables";
        }
    }
}

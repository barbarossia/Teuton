using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMapping
{
    class Program
    {
        static void Main(string[] args)
        {
            MockTable table = new MockTable("Data Source=vmm-bobo;Initial Catalog=SingleDB;Integrated Security=False;User Id=sa;Password=User@123;Min Pool Size=10;Connection Lifetime=1200;Connect Timeout=30;", "errormessage");
            table.GenerateCSharpCode(@"ErrorCode.cs");
        }
    }
}

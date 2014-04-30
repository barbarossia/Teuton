using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.IO;

namespace GenerateScript
{
    class Program
    {
        static IScript tableScript;
        static IScript indexScript;
        static IScript sprocScript;
        static IScript dataScript;

        static void Main(string[] args)
        {
            tableScript = new TableScripte(@"D:\CWF5.0\Tables");
            indexScript = new TableScripte(@"D:\CWF5.0\Indexs");
            sprocScript = new StoreProcedueScript(@"D:\CWF5.0\Sprocs");
            dataScript = new DataScript(@"D:\CWF5.0\Data");
            Update(@"D:\CWF5.0\Update.sql");
            Console.ReadLine();
        }

        static void Update(string fileName)
        {
            tableScript.Update(fileName);
            Console.WriteLine("Tables done");
            indexScript.Update(fileName);
            Console.WriteLine("Indexs done");
            sprocScript.Update(fileName);
            Console.WriteLine("Sprocs done");
            dataScript.Update(fileName);
            Console.WriteLine("Data done");
        }

        static void Rollback()
        {
        }



    }
}

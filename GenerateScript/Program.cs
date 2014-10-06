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
            string root = @"D:\CWF5.0";
            tableScript = new TableScripte(root);
            indexScript = new IndexScript(root);
            sprocScript = new StoreProcedueScript(root);
            dataScript = new DataScript(root);
            Update();
            Rollback();
            Console.ReadLine();
        }

        static void Update()
        {
            tableScript.Update();
            Console.WriteLine("Tables update done");
            indexScript.Update();
            Console.WriteLine("Indexs update done");
            sprocScript.Update();
            Console.WriteLine("Sprocs update done");
            dataScript.Update();
            Console.WriteLine("Data update done");
        }

        static void Rollback()
        {
            tableScript.Rollback();
            Console.WriteLine("Tables rollback done");
            indexScript.Rollback();
            Console.WriteLine("Indexs rollback done");
            sprocScript.Rollback();
            Console.WriteLine("Sprocs rollback done");
            dataScript.Rollback();
            Console.WriteLine("Data rollback done");
        }
    }
}

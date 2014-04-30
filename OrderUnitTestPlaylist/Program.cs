using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OrderUnitTestPlaylist
{
    class Program
    {
        static Dictionary<string, string> unorderedList = new Dictionary<string,string>();
        static Dictionary<string, string> orderedList = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            string path = @"D:\UnitTestOrderByName.playlist";
            string xml = GetXml(path);
            Extract(xml);
            OrderByName();
            string xml1 = GetOrderedXml();
            Output(@"D:\output.playlist", xml1);

            Console.ReadLine();
        }

        static string GetXml(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer);
            }
        }

        static void Extract(string xml)
        {
            XElement x = XElement.Parse(xml);
            var nodes = (from n in x.Elements()
                         select n).OrderBy(m=>m.LastAttribute.Value).ToList();

            unorderedList = nodes.ToDictionary(k => k.LastAttribute.Value, v => GetKey(v.LastAttribute.Value));
        }

        static void OrderByName()
        {
            List<KeyValuePair<string, string>> myList = unorderedList.ToList();

            myList.Sort(
                delegate(KeyValuePair<string, string> firstPair,
                KeyValuePair<string, string> nextPair)
                {
                    return firstPair.Value.CompareTo(nextPair.Value);
                }
            );
            orderedList = myList.ToDictionary(k => k.Key, v => v.Value);
        }

        static string GetKey(string value)
        {
            string[] arr = value.Split('.');
            return arr.Last();
        }

        static string GetOrderedXml()
        {
            StringBuilder sb = new StringBuilder(@"<Playlist Version=""1.0"">");
            orderedList.Select(o => o.Key).ToList().ForEach(k => sb.Append(Format(k)));
            sb.Append(@"</Playlist>");
            return sb.ToString();
        }

        static void Output(string path, string xml)
        {
            //System.IO.StreamWriter file = new System.IO.StreamWriter(path);
            //file.WriteLine(xml);
            using (StreamWriter outfile = new StreamWriter(path))
            {
                outfile.Write(xml);
            }

            //new FileInfo(path).CreateText().WriteLine(xml);
        }

        static string Format(string value)
        {
            return string.Format(@"<Add Test=""{0}"" />", value);
        }
    }
}

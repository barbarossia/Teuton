using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;

namespace GeneratePermission {
    class Program {
        static void Main(string[] args) {
            string[] lines = new FileInfo("d:\\map.txt").OpenText().ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            List<Permission> pl = new List<Permission>();
            foreach (string line in lines) {
                if (!string.IsNullOrEmpty(line.Trim())) {
                    string[] parts = line.Split('\t');
                    Permission p = new Permission() {
                        Name = parts[0],
                        Value = long.Parse(parts[2])
                    };
                    p.SPNames = parts[1].Trim('"').Split('\n');
                    p.Envs = new int[][] {
                        parts[4].Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(i=>int.Parse(i.Replace("\"",""))).ToArray(),
                        parts[5].Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(i=>int.Parse(i.Replace("\"",""))).ToArray(),
                        parts[6].Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(i=>int.Parse(i.Replace("\"",""))).ToArray(),
                        parts[7].Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(i=>int.Parse(i.Replace("\"",""))).ToArray(),
                        parts[8].Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(i=>int.Parse(i.Replace("\"",""))).ToArray(),
                        parts[9].Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(i=>int.Parse(i.Replace("\"",""))).ToArray(),
                    };
                    pl.Add(p);
                }
            }
            pl.Sort(new Comparison<Permission>((p1, p2) => { return p1.Value.CompareTo(p2.Value); }));

            StringBuilder sb = new StringBuilder();
            foreach (Permission p in pl) {
                sb.AppendLine(string.Format("{0} = 0x{1},", p.Name, string.Join(string.Empty, BitConverter.GetBytes(p.Value).Reverse().Select(b => b.ToString("x2")))));
            }
            sb.AppendLine();

            string permissionLineFormat = @"INSERT INTO [dbo].[Permission]([Id],[Name],[SPName],[Value],[SoftDelete],[InsertedByUserAlias],[InsertedDateTime],[UpdatedByUserAlias],[UpdatedDateTime])
  VALUES ({0}, '{1}', '{2}', {3}, 0, 'setup', GETDATE(), 'setup', GETDATE())";
            string repLineFormat = @"INSERT INTO [dbo].[RoleEnvPermission]([Id],[RoleId],[EnvId],[Permission],[SoftDelete],[InsertedByUserAlias],[InsertedDateTime],[UpdatedByUserAlias],[UpdatedDateTime])
  VALUES ({0}, {1}, {2}, {3}, 0, 'setup', GETDATE(), 'setup', GETDATE())";
            sb.AppendLine("SET IDENTITY_INSERT [dbo].[Permission] ON")
                .AppendLine("GO");
            int index = 1;
            foreach (Permission p in pl) {
                foreach (string sp in p.SPNames) {
                    sb.AppendLine(string.Format(permissionLineFormat, index++, p.Name, sp == "EMPTY" ? string.Empty : sp, p.Value));
                }
            }
            sb.AppendLine("GO")
                .AppendLine("SET IDENTITY_INSERT [dbo].[Permission] OFF")
                .AppendLine("GO")
                .AppendLine();

            index = 1;
            sb.AppendLine("SET IDENTITY_INSERT [dbo].[RoleEnvPermission] ON")
                .AppendLine("GO");
            for (int r = 0; r < 6; r++) {
                for (int e = 1; e <= 4; e++) {
                    sb.AppendLine(string.Format(repLineFormat, index++, r + 1, e, pl.Where(p => p.Envs[r].Contains(e)).Sum(p => p.Value)));
                }
            }
            sb.AppendLine("GO")
                .AppendLine("SET IDENTITY_INSERT [dbo].[RoleEnvPermission] OFF")
                .AppendLine("GO");

            new FileInfo("D:\\r.txt").CreateText().WriteLine(sb.ToString());

            Console.WriteLine("finished");
            Console.ReadLine();
        }

        class Permission {
            public string Name { get; set; }
            public string[] SPNames { get; set; }
            public long Value { get; set; }
            public int[][] Envs { get; set; }

            public Permission() {
                //SPName = new List<string>();
            }
        }
    }
}

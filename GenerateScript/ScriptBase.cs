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
        public string Root { get; private set; }
        protected Action Action;
        protected string Content;
        public ScriptBase(string root)
        {
            this.Root = root;
        }

        public virtual void Update()
        {
            StringBuilder sb = new StringBuilder();
            var files = GetFilePath(Action.Update).GetFiles();
            foreach (var file in files)
            {
                sb.AppendLine(file.ToText());
            }

            sb.ToString().ToFile(Root.GetFullPath("Update.sql"));
        }

        public void Rollback()
        {
            StringBuilder sb = new StringBuilder();
            var files = GetFilePath(Action.Rollback).GetFiles();
            foreach (var file in files)
            {
                sb.AppendLine(file.ToText());
            }

            sb.ToString().ToFile(Root.GetFullPath("Rollback.sql"));
        }

        protected string GetFilePath(Action action)
        {
            return Root.GetFullPath(action.ToString()).GetFullPath(Content);
        }
    }
}

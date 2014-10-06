using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Data;
using System.IO;
using Utility.Common;

namespace ORMapping
{
    public class Table2Object
    {
        CodeCompileUnit targetUnit;
        CodeTypeDeclaration targetClass;
        private const string outputFileName = "SampleCode.cs";

        public Table2Object()
        {
            targetUnit = new CodeCompileUnit();
            CodeNamespace samples = new CodeNamespace("CodeDOMSample");
            samples.Imports.Add(new CodeNamespaceImport("System"));
            targetClass = new CodeTypeDeclaration("CodeDOMCreatedClass");
            targetClass.IsClass = true;
            targetClass.TypeAttributes =
                TypeAttributes.Public;
            samples.Types.Add(targetClass);
            targetUnit.Namespaces.Add(samples);
        }

        public void GenerateCSharpCode(string fileName)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StreamWriter sourceWriter = new StreamWriter(fileName))
            {
                provider.GenerateCodeFromCompileUnit(
                    targetUnit, sourceWriter, options);
            }
        }

        private void AddField(DataColumn col)
        {
            // Declare the widthValue field.
            CodeMemberField field = new CodeMemberField();
            field.Attributes = MemberAttributes.Private;
            field.Name = col.ColumnName.LowercaseFirst();
            field.Type = new CodeTypeReference(col.DataType);
            //widthValueField.Comments.Add(new CodeCommentStatement(
            //    "The width of the object."));
            //widthValueField.InitExpression = new CodePrimitiveExpression(value);
            targetClass.Members.Add(field);

        }

        private void AddProperty(DataColumn col)
        {
            CodeMemberProperty prop = new CodeMemberProperty();
            prop.Attributes =
                MemberAttributes.Public | MemberAttributes.Final;
            prop.Name = col.ColumnName.UppercaseFirst();
            prop.HasGet = true;
            prop.HasSet = true;
            prop.Type = new CodeTypeReference(col.DataType);
            prop.Comments.Add(new CodeCommentStatement(
                "The property for the object."));
            prop.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), col.ColumnName.LowercaseFirst())));
            prop.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), col.ColumnName.LowercaseFirst()),
                                                new CodePropertySetValueReferenceExpression()));

            targetClass.Members.Add(prop);
        }

        public void AddProperties(DataTable table)
        {
            foreach(DataColumn col in table.Columns)
            {
                AddField(col);
                AddProperty(col);
            }
        }

    }
}

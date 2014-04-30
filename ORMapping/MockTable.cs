using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ORMapping
{
    public class MockTable
    {
        CodeCompileUnit targetUnit;
        CodeTypeDeclaration targetClass;
        private const string outputFileName = "SampleCode.cs";
        private string sql = @"select top 1 * from {0}";
        public string ConnectString { get; private set; }
        public string Name { get; set; }

        public MockTable(string connectString, string name)
        {
            this.ConnectString = connectString;
            this.Name = name;

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
            DataTable table = GetTableInternal(string.Format(sql, Name));
            CreateTableMethod(table);
            CreateValidResponseMethod(table);
            GenerateCSharpCodeToFile(fileName);
        }

        private void CreateTableMethod(DataTable table)
        {
            CodeMemberMethod createTableMethod = new CodeMemberMethod();
            createTableMethod.Attributes =
                MemberAttributes.Private;
            createTableMethod.Name = "CreateTable";

            //DataTable table = new DataTable("Activity");
            CodeObjectCreateExpression tableCreate =
                new CodeObjectCreateExpression(
                new CodeTypeReference("DataTable"),
                new CodePrimitiveExpression(table.TableName));

            // Add the statement: 
            createTableMethod.Statements.Add(new CodeVariableDeclarationStatement(
                new CodeTypeReference("DataTable"), "table",
                tableCreate));

            //DataColumn column;
            CodeVariableDeclarationStatement colDeclaration = new CodeVariableDeclarationStatement(
                // Type of the variable to declare. 
                new CodeTypeReference("DataColumn"),
                // Name of the variable to declare. 
                "column");
            createTableMethod.Statements.Add(colDeclaration);
            
            foreach(DataColumn col in table.Columns)
            {
                //new DataColumn("ActivityCategoryName", typeof(String));
                CodeObjectCreateExpression columnCreate =
                    new CodeObjectCreateExpression(
                    new CodeTypeReference("DataColumn"),
                    new CodePrimitiveExpression(col.ColumnName),
                    new CodeTypeOfExpression(new CodeTypeReference(col.DataType)));

                //column = new DataColumn("ActivityCategoryName", typeof(String));
                CodeAssignStatement as1 = new CodeAssignStatement(
                    new CodeVariableReferenceExpression("column"),
                    columnCreate);

                createTableMethod.Statements.Add(as1);

                //table.Columns.Add(column);
                //table.Columns;
                CodePropertyReferenceExpression propertyRef1 =
                    new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("table"), "Columns");
                
                //Columns.Add(column);
                CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(
                    propertyRef1,
                    "Add",
                    new CodeVariableReferenceExpression("column"));
                createTableMethod.Statements.Add(methodInvoke);

            }
            targetClass.Members.Add(createTableMethod);
        }


        private void CreateValidResponseMethod(DataTable table)
        {
            CodeMemberMethod createValidResponseMethod = new CodeMemberMethod();
            createValidResponseMethod.Attributes =
                MemberAttributes.Private;
            createValidResponseMethod.Name = "createValidResponse";
            createValidResponseMethod.Parameters.Add(new CodeParameterDeclarationExpression("int", "itemCount"));

            createValidResponseMethod.ReturnType =
                new CodeTypeReference("DataTable");

            CodeVariableDeclarationStatement rowDeclaration = new CodeVariableDeclarationStatement(
                // Type of the variable to declare. 
                new CodeTypeReference("DataRow"),
                // Name of the variable to declare. 
                "row");
            createValidResponseMethod.Statements.Add(rowDeclaration);

            //DataTable table = CreateTable();
            //DataTable table;
            CodeMethodReferenceExpression methodRefCreqteTable = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "CreateTable");
            CodeVariableDeclarationStatement tableDeclaration = new CodeVariableDeclarationStatement(
                // Type of the variable to declare. 
                new CodeTypeReference("DataTable"),
                // Name of the variable to declare. 
                "table");
            //table = CreateTable();
            CodeAssignStatement assignTable = new CodeAssignStatement(new CodeVariableReferenceExpression("table"), methodRefCreqteTable);

            createValidResponseMethod.Statements.Add(assignTable);

            CodeVariableDeclarationStatement testInt = new CodeVariableDeclarationStatement(typeof(int), "i", new CodePrimitiveExpression(0));
            createValidResponseMethod.Statements.Add(testInt);

            //row = table.NewRow();
            CodeMethodInvokeExpression methodInvokeNewRow = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("table"), "NewRow");                                   
            CodeAssignStatement assignRow = new CodeAssignStatement(
                new CodeVariableReferenceExpression("row"), methodInvokeNewRow);

            //table.Rows.Add(row);
            //table.Rows;
            CodePropertyReferenceExpression propertyRefTable =
                new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("table"), "Rows");

            //Rows.Add(row);
            CodeMethodInvokeExpression methodInvokeAdd = new CodeMethodInvokeExpression(
                propertyRefTable,
                "Add",
                new CodeVariableReferenceExpression("row"));

            List<CodeStatement> states = new List<CodeStatement>();
            states.Add(assignRow);
            states.AddRange(SetFields(table));
            states.Add(new CodeExpressionStatement(methodInvokeAdd));

            // Creates a for loop that sets testInt to 0 and continues incrementing testInt by 1 each loop until testInt is not less than 10.
            CodeIterationStatement forLoop = new CodeIterationStatement(
                // initStatement parameter for pre-loop initialization. 
                new CodeAssignStatement(new CodeVariableReferenceExpression("i"), new CodePrimitiveExpression(0)),
                // testExpression parameter to test for continuation condition. 
                new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),
                    CodeBinaryOperatorType.LessThan, new CodeVariableReferenceExpression("itemCount")),
                // incrementStatement parameter indicates statement to execute after each iteration. 
                new CodeAssignStatement(new CodeVariableReferenceExpression("i"), new CodeBinaryOperatorExpression(
                    new CodeVariableReferenceExpression("i"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),
                // statements parameter contains the statements to execute during each interation of the loop. 
                // Each loop iteration the value of the integer is output using the Console.WriteLine method. 
                states.ToArray());


            createValidResponseMethod.Statements.Add(forLoop);

            CodeMethodReturnStatement returnStatement = new CodeMethodReturnStatement(new CodeVariableReferenceExpression("table"));
            targetClass.Members.Add(createValidResponseMethod);
        }

        private List<CodeAssignStatement> SetFields(DataTable table)
        {
            List<CodeAssignStatement> states = new List<CodeAssignStatement>();
            foreach (DataColumn col in table.Columns)
            {
                CodeAssignStatement state = SetField(col);
                states.Add(state);
            }
            return states;
        }

        private CodeAssignStatement SetField(DataColumn col)
        {
            CodeAssignStatement state = null;
            CodeArrayIndexerExpression ci1 = new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("row"),
                new CodePrimitiveExpression(col.ColumnName));
            if (col.DataType == typeof(string))
                state = new CodeAssignStatement(ci1, new CodePrimitiveExpression(Guid.NewGuid().ToString()));
            else if (col.DataType == typeof(int))
                state = new CodeAssignStatement(ci1, new CodePrimitiveExpression(0));
            else if (col.DataType == typeof(long))
                state = new CodeAssignStatement(ci1, new CodePrimitiveExpression(0L));
            else if (col.DataType == typeof(bool))
                state = new CodeAssignStatement(ci1, new CodePrimitiveExpression(true));
            else if (col.DataType == typeof(DateTime))
                state = new CodeAssignStatement(ci1, new CodeTypeReferenceExpression("DateTime"));
            else if (col.DataType == typeof(Guid))
                state = new CodeAssignStatement(ci1, new CodePrimitiveExpression(Guid.NewGuid()));
            else
                state = new CodeAssignStatement(ci1, new CodePrimitiveExpression(null));
            return state;

        }

        private DataTable GetTableInternal(string sql)
        {
            DataTable dataTable = new DataTable(Name);
            SqlConnection conn = new SqlConnection(ConnectString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataTable);
            conn.Close();
            da.Dispose();

            return dataTable;
        }

        private void GenerateCSharpCodeToFile(string fileName)
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
    }
}

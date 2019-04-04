using System.Collections.Generic;

namespace DefectDataManagment.App_Code
{
    public static class CodeElementsParams
    {
        public static string CommitId;
        public static string Filepath;
        public static string Filename;
        public static string Methodname;
        public static string MethodCodeElement;
        public static string ConstructorName;
        public static string ConstructorCodeElement;
        public static string Property;
        public static string PropertycodeElement;
       

        public static List<string> ignorelistitem = new List<string> { "CompilationUnit", "NamespaceDeclaration", "ClassDeclaration", "MethodDeclaration", "Block", "SimpleMemberAccessExpression" };

    }

   public struct FilesDetails
    {
        public string Filename;
        public string filePath;
        public static Dictionary<string, List<string>> FileSpan;

    };

    public enum FileOperation { DELETED,ADDED,MODIFIED};




}

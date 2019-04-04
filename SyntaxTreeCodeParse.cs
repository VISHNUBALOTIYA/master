using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using XAct;
using DefectDataManagment.App_Code;
using BlockSyntax = Roslyn.Compilers.CSharp.BlockSyntax;
using System.Threading;
using System.Text.RegularExpressions;
using DefectDataManagment.Logger;
using DefectDataManagment.App_Code;

namespace DefectDataManagment.App_Code
{
    public class SyntaxTreeCodeParse : Iparse
    {
        private readonly List<string> _ignorelist = new List<string> { "CompilationUnit", "NamespaceDeclaration", "ClassDeclaration", "MethodDeclaration", "Block" };
        private static readonly object SyntaxLock = new object();
        public static List<MethodDeclarationSyntax> allmethods = new List<MethodDeclarationSyntax>();

        public string CsfileCodeElement { get; private set; }

        public string Createsyntaxtree(string newcsfile, string oldCsfile, FilesDetails filedetails)
        {
            try
                
            {
                List<string> spans = new List<string>();
                FilesDetails.FileSpan.TryGetValue(filedetails.Filename, out spans);
                List<int> sp = spans.ConvertAll(int.Parse);

                SourceText oldtext = SourceText.From(oldCsfile);
                var newtree = CSharpSyntaxTree.ParseText(newcsfile);
                var oldTree = CSharpSyntaxTree.ParseText(oldCsfile);
                var newTreeroot = newtree.GetRoot();

               // var span1 = newtree.GetText().GetTextChanges(oldtext);

                foreach (var spindex in sp)
                {
                    var span = newtree.GetText().Lines[2689].Span;
                    var nodes = newTreeroot.DescendantNodes().Where(x => x.Span.IntersectsWith(span));
                    
                    //get based on nodes span:
                    try
                    {
                        var newElement = "";
                        newElement = "File Path: " + filedetails.filePath + "\n" + "File Name: " + filedetails.Filename + "\n";
                        var m = newTreeroot.FindNode(span).FirstAncestorOrSelf<MethodDeclarationSyntax>();
                        if (m != null)
                        {
                            string methoddetails = m.Modifiers + " " + m.ReturnType + " " +
                               m.Identifier + m.ParameterList;
                            string mname1 = "Method Name: " + methoddetails + "\n";
                            newElement = newElement + mname1;

                            var lnodes = nodes.ToList();
                            var blockindex =  lnodes.IndexOf(x => x.Kind().ToString() == "Block");


                            for (int i = blockindex; i < lnodes.Count; i++)
                            {
                                Console.WriteLine(lnodes[i].Kind());
                                if (lnodes[i].Kind().ToString() == "IdentifierName")
                                {
                                    break;
                                }

                               string kindtext = lnodes[i].Kind().ToString();
                               newElement = newElement +kindtext + "\n";

                            }

                        }

                        var p = newTreeroot.FindNode(span).FirstAncestorOrSelf<PropertyDeclarationSyntax>();
                        if (p != null)
                        {
                            string propertydeatils = p.Modifiers + " " + p.Identifier.ToString() + " " + p.ExpressionBody;
                            string PropertyName = "Property: " + propertydeatils + "\n";
                            newElement = newElement + PropertyName;

                            //Propperty to identifier


                        }

                        var c = newTreeroot.FindNode(span).FirstAncestorOrSelf<ConstructorDeclarationSyntax>();
                        if (c!= null)
                        {
                            string constrctordetails = c.Modifiers + " " + c.Identifier + " " + c.ParameterList;
                            string ConstructorName = "Constructor: " + constrctordetails + "\n";
                            newElement = newElement + ConstructorName; 
                            
                            //block to identifier                          
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }















                var difflst = newtree.GetChanges(oldTree);


                if (difflst.Count > 0)
                {
                    difflst.RemoveAll(x => x.NewText == "");
                    difflst.RemoveAll(x => x.NewText == "//");
                    difflst.RemoveAll(x => x.NewText == "        ");
                    difflst.RemoveAll(x => x.NewText == "       ");
                    difflst.RemoveAll(x => x.NewText == " ");
                }
                    foreach (var item in difflst)
                    {                       
                        var newElement = "";
                        newElement = "File Path: " + filedetails.filePath + "\n" + "File Name: " + filedetails.Filename + "\n";
                        string parentnode = string.Empty;


                        MethodDeclarationSyntax methodfirst = null;
                        try
                        {
                            methodfirst = newTreeroot.FindNode(item.Span).FirstAncestorOrSelf<MethodDeclarationSyntax>();
                        }
                        catch (Exception ex)  //Only case of Exception
                        {
                            LogEntry.LogMsg.Error(newElement, ex);
                            Console.WriteLine(item);
                            Console.WriteLine(newElement);
                            continue;
                        }
                        if (methodfirst != null)
                        {
                            parentnode = methodfirst.Kind().ToString();
                            string methoddetails = methodfirst.Modifiers + " " + methodfirst.ReturnType + " " +
                               methodfirst.Identifier + methodfirst.ParameterList;
                            string mname1 = "Method Name: " + methoddetails + "\n";
                            newElement = newElement + mname1;
                            newElement = newElement + "SyntaxNode Tree:" + "\n";
                        }

                        PropertyDeclarationSyntax prop = null;
                        try
                        {
                            prop = newTreeroot.FindNode(item.Span).FirstAncestorOrSelf<PropertyDeclarationSyntax>();
                        }
                        catch (Exception ex)
                        {
                            LogEntry.LogMsg.Error(newElement, ex);
                            Console.WriteLine(item);
                            Console.WriteLine(newElement);
                              continue;  
                        }
                            if (prop != null)
                            {
                                parentnode = prop.Kind().ToString();
                                string propertydeatils = prop.Modifiers + " " + prop.Identifier.ToString() + " " + prop.ExpressionBody;
                                string PropertyName = "Property: " + propertydeatils + "\n";
                                newElement = newElement + PropertyName + "SyntaxNode Tree:" + "\n"; ;
                                //newElement = newElement + "SyntaxNode Tree:" + "\n";
                            }
                        


                        //contructor
                        ConstructorDeclarationSyntax cons = null;
                        try
                        {
                             cons = newTreeroot.FindNode(item.Span).FirstAncestorOrSelf<ConstructorDeclarationSyntax>();
                        }
                        catch (Exception ex)
                        {
                            LogEntry.LogMsg.Error(newElement, ex);
                            Console.WriteLine(item);
                            Console.WriteLine(newElement);
                            continue;
                        }

                        if (cons != null)
                        {
                            parentnode = cons.Kind().ToString();
                            string constrctordetails = cons.Modifiers + " " + cons.Identifier + " " + cons.ParameterList;

                            string ConstructorName = "Constructor: " + constrctordetails + "\n";
                            newElement = newElement + ConstructorName;
                            newElement = newElement + "SyntaxNode Tree:" + "\n";

                        }

                        if (parentnode != string.Empty)
                        {
                            //Code Elements
                            var codeelementlist = newTreeroot.FindNode(item.Span).DescendantNodesAndSelf();
                            foreach (var CElement in codeelementlist)
                            {
                                if (!_ignorelist.Contains(CElement.ToString()))
                                {
                                    newElement = newElement + CElement.Kind().ToString() + "\n";
                                }
                            }

                        }
                        CsfileCodeElement = CsfileCodeElement + newElement + "\n";
                    }
                        
                         return CsfileCodeElement;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}



/*
if (difflst.Count > 0)
{
     //Remove unnecessary diff:
    difflst.RemoveAll(x => x.NewText == "");
    difflst.RemoveAll(x => x.NewText == "//");
    difflst.RemoveAll(x => x.NewText == "        ");

}


    foreach (var method in allmethods)
    {
        if (difflst[0].Span.Start < method.FullSpan.Start || difflst[difflst.Count()-1].Span.End < method.FullSpan.End)
        {
            allmethods.Remove(method);
        }
    }
    //Remove diff which is out of method limit(Execution time reduce)
    foreach (var diff in difflst)
    {
        if (diff.Span.Start < methodone && diff.Span.End < methodone)
        {
            difflst.Remove(diff);
        }
    }

    //Remove Extra diiff based on methods start and End 

    //foreach (var item in collection)
    //{

    //} 





   //for methods added and removed
    CheckSynatxChanges(newtree, oldTree);

        var extraxctedmethods=  GetDiffMethodList(newtree, difflst);
        if (extraxctedmethods.Count > 0)
        {
            string minfo=   MethodInfo(extraxctedmethods, difflst, filedetails);
        }
}


*/



//Properties 
//else
//{
//    var allproperties = newtree.GetRoot().DescendantNodesAndSelf().OfType<PropertyDeclarationSyntax>().ToList();
//    if (allproperties.Count > 0)
//        propertieslist = ProptiesInfo(allproperties, difflst);
//    return propertieslist;
//}



//}


/*
        public List<MethodDeclarationSyntax> GetDiffMethodList(SyntaxTree newtree, IList<TextChange> difflst)
        {
            try
            {
               // var startdiffindex = difflst[0].Span.Start;
               // var enddiffindex = difflst[difflst.Count - 1].Span.End;
                var totalmethods = newtree.GetRoot().DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().ToList();


                var minMethod = new List<MethodDeclarationSyntax>();
                var maxmethod = new List<MethodDeclarationSyntax>();
                var fm = new List<MethodDeclarationSyntax>();

                foreach (var ch in difflst)
                {
                    foreach (var m1 in totalmethods)
                    {
                        var st = ch.Span.Start;
                        var end = ch.Span.End;
                        if (m1.FullSpan.Start <= ch.Span.Start && m1.FullSpan.End >= ch.Span.End)
                        {

                            if (!fm.Contains(m1))
                            {
                                fm.Add(m1);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }

                    }
                }

                return fm;

                /*
                foreach (var m in allmethods)
                    if (m.FullSpan.Start <= startdiffindex)
                        minMethod.Add(m);
                    else
                        break;

                allmethods.Reverse();
                foreach (var m in allmethods)
                    if (enddiffindex <= m.FullSpan.End)
                        maxmethod.Add(m);
                    else
                        break;

                if (minMethod.Count > 0)
                    finalmethods.Add(minMethod.Last());
                
                if (maxmethod.Count > 0)
                    finalmethods.Add(maxmethod.First());

                return allmethods;
              

                }
            catch (Exception ex)
            {

                throw ex;
            }
        }
          */
//Get method info(MEthod name Signature and method code elements)

/*
public string MethodInfo(List<MethodDeclarationSyntax> diffmethodList, IList<TextChange> changes, FilesDetails filedetails)
    {
        try
        {
            var codeElemenedata = "";

            foreach (var change in changes)
                foreach (var syntaxnode in diffmethodList)
                    if (syntaxnode.FullSpan.Start <= change.Span.Start && change.Span.End <= syntaxnode.FullSpan.End)
                    {
                        CodeElementsParams.Methodname = syntaxnode.Modifiers + " " + syntaxnode.ReturnType + " " +
                                                  syntaxnode.Identifier + syntaxnode.ParameterList;

                        var methoditenm = syntaxnode.DescendantNodes().OfType<CSharpSyntaxNode>().ToList();

                        var csharpnodelist = new List<CSharpSyntaxNode>();
                        foreach (var item in methoditenm)
                            if (!CodeElementsParams.ignorelistitem.Contains(item.Kind().ToString())) csharpnodelist.Add(item);

                        var changeElementList = CodeElement(_ignorelist, changes, csharpnodelist);
                        if (changeElementList.Count() != 0)
                        { codeElemenedata = CheckAndGetCodeElement(codeElemenedata, changeElementList, filedetails); }
                        break;
                    }

            return codeElemenedata;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

/*
    public List<string> CodeElement(List<string> lst, IList<TextChange> changes, List<CSharpSyntaxNode> allnodeSharpSyntaxNodes)
    {
        try
        {
            List<StatementSyntax> dd = new List<StatementSyntax>();

            var changesTypes = new List<string>();
            foreach (var change in changes)
                foreach (var syntaxnode in allnodeSharpSyntaxNodes)
                {
                    try
                    {
                        var ds = syntaxnode.GetText().ToString();
                        Console.WriteLine(ds);
                        if (ds == change.ToString())
                        { }
                        Console.WriteLine(ds);
                        var tok = syntaxnode.FindToken(60803);
                        if (tok.FullSpan.End >= syntaxnode.FullSpan.End)
                        { break; }
                        Console.WriteLine(tok.Kind());
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }

            /*
                    if (syntaxnode.FullSpan.Start <= change.Span.Start && change.Span.End <= syntaxnode.FullSpan.End)
                    {
                        var currentnode = syntaxnode.DescendantNodesAndTokensAndSelf();
                        foreach (var s1 in currentnode)
                            if (s1.FullSpan.Start <= 60803 && 60803<= s1.FullSpan.End)
                            {

                                dd.Add(s1);
                            }
                                //var ds =  syntaxnode.ChildNodes();
                                changesTypes.Add(syntaxnode.Kind().ToString());

                    }



            //var itmes = changesTypes.Distinct().ToList();
            //if (itmes.Count >= 5) itmes = Enumerable.Reverse(itmes).Take(5).Reverse().ToList();
            return changesTypes;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }
       */
/*
        public string CheckAndGetCodeElement(string codeElemenedata, List<string> changeElementList, FilesDetails filedetails)
        {
            try
            {
                CodeElementsParams.MethodCodeElement = string.Join("\n", changeElementList.ToArray()) + "\n";
                var newElement = "File Path: " + filedetails.filePath + "\n" + "File Name: " + filedetails.Filename +
                                 "\n" + "Method Name: " + CodeElementsParams.Methodname + "\n" + "Method Code Elements: " + "\n" +
                                 CodeElementsParams.MethodCodeElement + "\n";

                if (codeElemenedata == string.Empty || codeElemenedata == " ")  //first time assignd for change-1
                {
                    codeElemenedata = codeElemenedata + newElement;
                }
                else
                {
                    var existrecord = codeElemenedata.Split(new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    existrecord = existrecord.ConvertAll(d => d.ToLower());
                    if (!existrecord.Contains(newElement.Trim().ToLower()))
                        codeElemenedata = codeElemenedata + newElement;
                }

                return codeElemenedata;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

/*
        private static void CheckSynatxChanges(SyntaxTree newtree, SyntaxTree oldTree)
        {
            var newmethods = newtree.GetRoot().DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().ToList();
            var oldmethods = oldTree.GetRoot().DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().ToList();
            List<string> nmethods = new List<string>();
            List<string> omethods = new List<string>();

            
            try
            {
                foreach (var methd in oldmethods)
                {
                    string oldm = methd.Modifiers + " " + methd.ReturnType + " " + methd.Identifier + methd.ParameterList;
                    omethods.Add(oldm);
                }

                foreach (var methd in newmethods)
                  {
                    string addedmethod = methd.Modifiers + " " + methd.ReturnType + " " + methd.Identifier + methd.ParameterList;
                    nmethods.Add(addedmethod);
                  }
                if (!(omethods.Equals(nmethods)))
                {
                    var x = omethods.Except(nmethods).ToList();
                    if (x.Count > 0)
                        syntaxChangesMethods("Method Deleted: ", x);

                    var y = nmethods.Except(omethods).ToList();
                    if (y.Count > 0)
                        syntaxChangesMethods("Method Added: ", y);
                }
                        
                  
            }

            catch (Exception ex)
            {

                throw ex;
            }
        }


/*

       private static void syntaxChangesMethods(string syntaxchangetype ,List<string> methodlist)
       {
           try
           {

               string method = string.Empty;
               foreach (var methodname in methodlist)
               {
                   if (!Program.defectcategory.ContainsKey(CodeElementsParams.CommitId))
                   {
                       if (methodlist.First() == methodname)
                       { method = syntaxchangetype + "\n" + methodname + "\n"; }
                       else
                       { method = methodname + "\n"; }
                       Program.defectcategory.Add(CodeElementsParams.CommitId, method);
                   }
                   else
                   {
                       var value = Program.defectcategory[CodeElementsParams.CommitId];
                       var dataexist = value.Split('\n').ToList();
                       List<string> allm = new List<string>();
                       foreach (var item in dataexist)
                       {
                           allm.Add(item.TrimStart().Split(':'));
                       }
                       if (!allm.Contains(methodname))
                       {
                           string combindedString = string.Join("\n", dataexist.ToArray());
                           var newentry = combindedString + "\n"+syntaxchangetype + methodname + "\n";
                       Program.defectcategory[CodeElementsParams.CommitId] = newentry;
                      }
                   }
               }
           }
           catch (Exception ex)
           {
               LogEntry.LogMsg.Info(ex.Message);
               throw ex;
           }
       }




public string ProptiesInfo(List<PropertyDeclarationSyntax> propertiesdeclarationSyntaxs, IList<TextChange> changes)
{
var codeElemenedata = "";
foreach (var change in changes)
{
  foreach (var syntaxnode in propertiesdeclarationSyntaxs)
  {
      if (syntaxnode.FullSpan.Start <= change.Span.Start && change.Span.End <= syntaxnode.FullSpan.End)
      {
          CodeElements.Property = syntaxnode.Modifiers + " " + syntaxnode.Identifier.ToString();

          var propItems = syntaxnode.DescendantNodes().OfType<CSharpSyntaxNode>().ToList();
          var changeElementList = CodeElement(_ignorelist, changes, propItems);

          codeElemenedata = CheckAndGetCodeElement(codeElemenedata, changeElementList);

          CodeElements.PropertycodeElement = string.Join("\n", changeElementList.ToArray()) + "\n";

          var newElement = "File Path: " + CodeElements.Filepath + "\n" + "File Name: " + CodeElements.Filename +
                           "\n" + "Property Name: " + CodeElements.Property + "\n" + "Property Code Elements: " + "\n" +
                           CodeElements.PropertycodeElement + "\n";


          if (codeElemenedata == string.Empty || codeElemenedata == " ")
          {
              codeElemenedata = codeElemenedata + newElement;
          }
          else
          {
              var existrecord = codeElemenedata.Split(new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries)
                  .ToList();
              existrecord = existrecord.ConvertAll(d => d.ToLower());
              if (!existrecord.Contains(newElement.Trim().ToLower())) codeElemenedata = codeElemenedata + newElement;
          }
          break;
      }

  }

}
return codeElemenedata;
}


public void DelegateInfo(List<DelegateDeclarationSyntax> delegateDeclarations, IList<TextChange> changes)
{
foreach (var change in changes)
{
  if (change.NewText != "")
  {
      foreach (var syntaxnode in delegateDeclarations)
      {
          if (syntaxnode.FullSpan.Start <= change.Span.Start && change.Span.End <= syntaxnode.FullSpan.End)
          {
              var del = syntaxnode.AncestorsAndSelf().OfType<ConstructorDeclarationSyntax>().First();
              string delegatedec = del.Identifier + " " + del.Identifier + del.ParameterList;
              //Console.WriteLine("Changes in Delegate Declaration" + delegatedec);

              var delItems = syntaxnode.DescendantNodes().OfType<CSharpSyntaxNode>().ToList();
              var chnagestype = CodeElement(_ignorelist, changes, delItems);

              if (chnagestype.Count() >= 3)
              {
                  chnagestype = Enumerable.Reverse(chnagestype).Take(3).Reverse().ToList();
              }
              break;
          }

      }
  }
}

}
public void ConstructorInfo(List<ConstructorDeclarationSyntax> constructorDeclarationSyntaxs, IList<TextChange> changes)
{
List<string> types = new List<string>();
Dictionary<string, List<string>> detailsinfo = new Dictionary<string, List<string>>();

foreach (var change in changes)
{
  if (change.NewText != "")
  {
      foreach (var syntaxnode in constructorDeclarationSyntaxs)
      {
          if (syntaxnode.FullSpan.Start <= change.Span.Start && change.Span.End <= syntaxnode.FullSpan.End)
          {
              var constr = syntaxnode.AncestorsAndSelf().OfType<ConstructorDeclarationSyntax>().First();
              string constructor = constr.Identifier + " " + constr.Identifier + constr.ParameterList;
              //Console.WriteLine("Changes in Constructor " + constructor);

              //my newly added code
              var constrItems = syntaxnode.DescendantNodes().OfType<CSharpSyntaxNode>().ToList();
              var chnagestype = CodeElement(_ignorelist, changes, constrItems);

              if (chnagestype.Count() >= 2)
              {
                  chnagestype = Enumerable.Reverse(chnagestype).Take(2).Reverse().ToList();
              }

              if (detailsinfo.ContainsKey(constructor))
              {
                  detailsinfo[constructor] = chnagestype;
              }
              else
              {
                  detailsinfo.Add(constructor, chnagestype);
              }
              break;
          }

      }
  }
}
}
*/





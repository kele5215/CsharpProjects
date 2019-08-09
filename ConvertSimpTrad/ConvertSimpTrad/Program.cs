using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ConvertSimpTrad
{
    static class Program
    {
        //private static Microsoft.Office.Interop.Word.ApplicationClass app = new Microsoft.Office.Interop.Word.ApplicationClass();

        //static  Document doc = null;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            //InitWordApp();

            //System.Console.WriteLine("历史");
            //System.Console.WriteLine(CHS2CHT("历史"));

            //Console.ReadLine(); 

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ConvertSimpTrad());
        }

        //static void InitWordApp()
        //{

        //    object optional = Missing.Value;
        //    object template = Missing.Value;
        //    object newTemplate = Missing.Value;
        //    object documentType = Missing.Value;
        //    object visible = false;
        //    doc = app.Documents.Add(ref template, ref newTemplate, ref documentType, ref visible);

        //}

        //static string CHS2CHT(string src)
        //{
        //    string des = "";
        //    _Application appWord = new Microsoft.Office.Interop.Word.Application();
        //    object template = Missing.Value;
        //    object newTemplate = Missing.Value;
        //    object docType = Missing.Value;
        //    object visible = true;
        //    Document doc = appWord.Documents.Add(ref template, ref newTemplate, ref docType, ref visible);
        //    appWord.Selection.TypeText(src);
        //    appWord.Selection.Range.TCSCConverter(WdTCSCConverterDirection.wdTCSCConverterDirectionSCTC, true, true);
        //    appWord.ActiveDocument.Select();
        //    des = appWord.Selection.Text;
        //    object saveChange = 0;
        //    object originalFormat = Missing.Value;
        //    object routeDocument = Missing.Value;
        //    appWord.Quit(ref saveChange, ref originalFormat, ref routeDocument);
        //    doc = null;
        //    appWord = null;
        //    GC.Collect();//进程资源释放

        //    return des;
        //}

        ///// <summary>
        ///// 将简体中文转换成繁体中文
        ///// </summary>
        ///// <param name="s"></param>
        ///// <returns></returns>
        //static string ConvertToSCTC(string s)
        //{
        //    if (s == "") return "";
        //    object first = 0;
        //    object last = doc.Characters.Count;
        //    doc.Range(ref first, ref last).Select();
        //    doc.Range(ref first, ref last).Text = s;
        //    last = doc.Characters.Count;
        //    doc.Range(ref first, ref last).TCSCConverter(Microsoft.Office.Interop.Word.WdTCSCConverterDirection.wdTCSCConverterDirectionSCTC, true, true);
        //    last = doc.Characters.Count;
        //    return doc.Range(ref first, ref last).Text.TrimEnd('\r');
        //}

    }
}

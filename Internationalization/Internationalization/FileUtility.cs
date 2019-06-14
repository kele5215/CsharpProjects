using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace Internationalization
{
    public class FileUtility
    {

        /// <summary>
        /// フォルダ内のファイル名をワイルドコードで検索し､更に正規表現で検索する｡
        /// </summary>
        /// <param name="path">対象フォルダ</param>
        /// <returns></returns>
        public static FileInfo[] FindFiles(string path)
        {

            // ファイル名用リスト
            List<string> lstFullName = new List<string>();

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = new string[] { "*.resx" }.SelectMany(i => dir.GetFiles(i, SearchOption.AllDirectories)).Distinct().ToArray();

            return files;
        }
    }
}

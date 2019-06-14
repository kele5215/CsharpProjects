using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace JapaneseDectector
{
    public partial class _Default : System.Web.UI.Page
    {
        List<string> lstNames = new List<string>();
        List<string> lstFiles = new List<string>();
        List<string> lstResult = new List<string>();
        bool multiLineCommentStarted = false;

        List<string> lstFilesChOrJp;

        protected void Page_Load(object sender, EventArgs e)
        {
            //this.txtPath.Text = @"C:\work\1.4-l10n-zh-CN";

            //lstNames.Add(".SQL");
            //lstNames.Add(".CMD");
            //lstNames.Add(".LOG");

            //-------------------------
            //lstNames.Add(".CS");
            //lstNames.Add(".ASPX");
            //lstNames.Add(".RESX");
            //lstNames.Add(".XAML");
            //lstNames.Add(".JS");
            //-------------------------
            //lstNames.Add(".CONFIG");
            //lstNames.Add(".INI");
            //lstNames.Add(".TXT");
            //lstNames.Add(".XML");
            //lstNames.Add(".HTM");
            //lstNames.Add(".HTML");
            //lstNames.Add(".H");
            //lstNames.Add(".CPP");

        }

        protected void btnRun_Click(object sender, EventArgs e)
        {
            int fileCount = 0;
            lblResult.Text = "";
            string strRootPath = this.txtPath.Text.Trim();
            getSelectedFileType();
            GetFilesMostDeep(strRootPath);

            StringBuilder sb = new StringBuilder();
            lstFilesChOrJp = new List<string>();
            foreach (string str in lstFiles)
            {
                GetJapaneseFiles(str, ref fileCount);
            }

            foreach (string strr in lstResult)
            {
                if (!string.IsNullOrEmpty(strr.Trim()))
                {
                    sb.Append(strr + "\r\n");
                }
            }

            lblResult.Text = string.Empty;
            lblResult.Text += "ファイル検索件数　　　　　 ： " + lstFiles.Count + "\r\n";
            lblResult.Text += "日本語文字含むファイル件数 ： " + fileCount + "\r\n";
            lblResult.Text += "日本語文字含む行数　　　　 ： " + lstResult.Count + "\r\n";
            lblResult.Text += sb.ToString();

            ViewState["FilesChOrJp"] = null;
            ViewState["FilesChOrJp"] = lstFilesChOrJp;
        }

        protected void btnRun_Click_C(object sender, EventArgs e)
        {
            int fileCount = 0;
            lblResult.Text = "";
            string strRootPath = this.txtPath.Text.Trim();
            getSelectedFileType();
            GetFilesMostDeep(strRootPath);

            StringBuilder sb = new StringBuilder();
            lstFilesChOrJp = new List<string>();
            foreach (string str in lstFiles)
            {
                GetChineseFiles(str, ref fileCount);
            }

            foreach (string strr in lstResult)
            {
                if (!string.IsNullOrEmpty(strr.Trim()))
                {
                    sb.Append(strr + "\r\n");
                }
            }
            lblResult.Text += "ファイル検索件数　　　　　 ： " + lstFiles.Count + "\r\n";
            lblResult.Text += "中国語文字含むファイル件数 ： " + fileCount + "\r\n";
            lblResult.Text += "中国語文字含む行数　　　　 ： " + lstResult.Count + "\r\n";
            lblResult.Text += sb.ToString();
        }

        protected void btnFileGet_Click(object sender, EventArgs e)
        {
            string strRootPath = this.txtPath.Text.Trim();

            string strDestPath = this.txtFileSave.Text.Trim();

            if (string.IsNullOrEmpty(strDestPath))
            {
                return;
            }

            if (ViewState["FilesChOrJp"] != null)
            {
                lstFilesChOrJp = (List<string>)ViewState["FilesChOrJp"];
            }
            else
            {
                return;
            }

            CopyDirectoryFile(strRootPath, strDestPath);

        }

        private void getSelectedFileType()
        {
            if (cbAll.Checked)
            {
                lstNames.Add("*");
            }
            else
            {
                if (cbSQL.Checked)
                {
                    lstNames.Add(cbSQL.Text.ToUpper());
                }
                if (cbCMD.Checked)
                {
                    lstNames.Add(cbCMD.Text.ToUpper());
                }
                if (cbLOG.Checked)
                {
                    lstNames.Add(cbLOG.Text.ToUpper());
                }
                if (cbCS.Checked)
                {
                    lstNames.Add(cbCS.Text.ToUpper());
                }
                if (cbASPX.Checked)
                {
                    lstNames.Add(cbASPX.Text.ToUpper());
                }
                if (cbRESX.Checked)
                {
                    lstNames.Add(cbRESX.Text.ToUpper());
                }
                if (cbXAML.Checked)
                {
                    lstNames.Add(cbXAML.Text.ToUpper());
                }
                if (cbJS.Checked)
                {
                    lstNames.Add(cbJS.Text.ToUpper());
                }
                if (cbCSS.Checked)
                {
                    lstNames.Add(cbCSS.Text.ToUpper());
                }
                if (cbCONFIG.Checked)
                {
                    lstNames.Add(cbCONFIG.Text.ToUpper());
                }
                if (cbINI.Checked)
                {
                    lstNames.Add(cbINI.Text.ToUpper());
                }
                if (cbTXT.Checked)
                {
                    lstNames.Add(cbTXT.Text.ToUpper());
                }
                if (cbXML.Checked)
                {
                    lstNames.Add(cbXML.Text.ToUpper());
                }
                if (cbHTM.Checked)
                {
                    lstNames.Add(cbHTM.Text.ToUpper());
                }
                if (cbHTML.Checked)
                {
                    lstNames.Add(cbHTML.Text.ToUpper());
                }
                if (cbH.Checked)
                {
                    lstNames.Add(cbH.Text.ToUpper());
                }
                if (cbCPP.Checked)
                {
                    lstNames.Add(cbCPP.Text.ToUpper());
                }
                if (cbISS.Checked)
                {
                    lstNames.Add(cbISS.Text.ToUpper());
                }
                if (cbXSLT.Checked)
                {
                    lstNames.Add(cbXSLT.Text.ToUpper());
                }
                if (cbVBS.Checked)
                {
                    lstNames.Add(cbVBS.Text.ToUpper());
                }
                if (cbWSF.Checked)
                {
                    lstNames.Add(cbWSF.Text.ToUpper());
                }
            }
        }

        private void GetFilesMostDeep(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                foreach (string fp in System.IO.Directory.GetFiles(path))
                {
                    string strDot = fp.Substring(fp.LastIndexOf(".")).ToUpper();
                    if ((lstNames.Contains(strDot) || lstNames.Contains("*")) && !fp.Contains("Assist")
                        && !fp.Contains("Test"))
                    {
                        lstFiles.Add(fp);
                    }
                }

                foreach (string dp in System.IO.Directory.GetDirectories(path))
                {
                    if (dp.Contains("svn") || dp.Contains("bin") || dp.Contains("obj"))
                    {
                        continue;
                    }
                    GetFilesMostDeep(dp);
                }
            }
        }

        private void GetJapaneseFiles(string fileName, ref int count)
        {
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            System.IO.StreamReader cReader = new System.IO.StreamReader(fileName, encoding);

            int cnt = 0;
            int idx = 0;
            bool exists = false;
            while (cReader.Peek() >= 0)
            {
                cnt++;
                string stBuffer = cReader.ReadLine();

                //空行以外の場合、処理を行う
                if (!string.IsNullOrEmpty(stBuffer.Trim()))
                {
                    string stBufferTmp = string.Empty;
                    if ((fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".CS") || fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".H") ||
                        fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".CPP") || fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".ISS")) &&
                        stBuffer.Trim().IndexOf("//") >= 0)
                    {
                        stBufferTmp = stBuffer.Trim().Substring(0, stBuffer.Trim().IndexOf("//")).Trim();
                    }
                    else if ((fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".ISS")) && stBuffer.Trim().IndexOf(";;;") >= 0)
                    {
                        stBufferTmp = stBuffer.Trim().Substring(0, stBuffer.Trim().IndexOf(";;;")).Trim();
                    }
                    else if ((fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".CS")) && stBuffer.Trim().IndexOf("#region") >= 0)
                    {
                        stBufferTmp = stBuffer.Trim().Substring(0, stBuffer.Trim().IndexOf("#region")).Trim();
                    }
                    else if ((fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".CS")) && stBuffer.Trim().IndexOf("#endregion") >= 0)
                    {
                        stBufferTmp = stBuffer.Trim().Substring(0, stBuffer.Trim().IndexOf("#endregion")).Trim();
                    }
                    else
                    {
                        if ((fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".SQL")) && stBuffer.Trim().IndexOf("--") >= 0)
                        {
                            stBufferTmp = stBuffer.Trim().Substring(0, stBuffer.Trim().IndexOf("--")).Trim();
                        }
                        else if ((fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".ASPX")))
                        {
                            if (stBuffer.Trim().IndexOf("<%--") >= 0)
                            {
                                stBufferTmp = stBuffer.Trim().Substring(0, stBuffer.Trim().IndexOf("<%--")).Trim();
                            }
                            else if (stBuffer.Trim().IndexOf("//") >= 0)
                            {
                                stBufferTmp = stBuffer.Trim().Substring(0, stBuffer.Trim().IndexOf("//")).Trim();
                            }
                            else
                            {
                                stBufferTmp = stBuffer.Trim();
                            }
                        }
                        else
                        {
                            stBufferTmp = stBuffer.Trim();
                        }
                    }

                    idx = 0;
                    foreach (char ch in stBufferTmp)
                    {
                        if (isJapaneseString(ch.ToString()))
                        {
                            if (!((fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".CS") || fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".H") ||
                                fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".CPP") || fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".SQL")) &&
                                isInMultiLineComment(stBufferTmp, idx)))
                            {
                                lstResult.Add(fileName + "(" + cnt + ") : " + stBuffer.Trim());
                                exists = true;
                                break;
                            }
                        }
                        idx++;
                    }
                }
            }

            cReader.Close();

            if (exists)
            {
                lstFilesChOrJp.Add(fileName);
                count++;
            }
        }

        private void GetChineseFiles(string fileName, ref int count)
        {
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            System.IO.StreamReader cReader = new System.IO.StreamReader(fileName, encoding);

            int cnt = 0;
            int idx = 0;
            bool exists = false;
            while (cReader.Peek() >= 0)
            {
                cnt++;
                string stBuffer = cReader.ReadLine();

                //空行以外の場合、処理を行う
                if (!string.IsNullOrEmpty(stBuffer.Trim()))
                {
                    string stBufferTmp = string.Empty;
                    if ((fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".CS") || fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".H") ||
                        fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".CPP") || fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".ISS")) &&
                        stBuffer.Trim().IndexOf("//") >= 0)
                    {
                        stBufferTmp = stBuffer.Trim().Substring(0, stBuffer.Trim().IndexOf("//")).Trim();
                    }
                    else if ((fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".ISS")) && stBuffer.Trim().IndexOf(";;;") >= 0)
                    {
                        stBufferTmp = stBuffer.Trim().Substring(0, stBuffer.Trim().IndexOf(";;;")).Trim();
                    }
                    else if ((fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".CS")) && stBuffer.Trim().IndexOf("#region") >= 0)
                    {
                        stBufferTmp = stBuffer.Trim().Substring(0, stBuffer.Trim().IndexOf("#region")).Trim();
                    }
                    else if ((fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".CS")) && stBuffer.Trim().IndexOf("#endregion") >= 0)
                    {
                        stBufferTmp = stBuffer.Trim().Substring(0, stBuffer.Trim().IndexOf("#endregion")).Trim();
                    }
                    else
                    {
                        if ((fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".SQL")) && stBuffer.Trim().IndexOf("--") >= 0)
                        {
                            stBufferTmp = stBuffer.Trim().Substring(0, stBuffer.Trim().IndexOf("--")).Trim();
                        }
                        else if ((fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".ASPX")))
                        {
                            if (stBuffer.Trim().IndexOf("<%--") >= 0)
                            {
                                stBufferTmp = stBuffer.Trim().Substring(0, stBuffer.Trim().IndexOf("<%--")).Trim();
                            }
                            else if (stBuffer.Trim().IndexOf("//") >= 0)
                            {
                                stBufferTmp = stBuffer.Trim().Substring(0, stBuffer.Trim().IndexOf("//")).Trim();
                            }
                            else
                            {
                                stBufferTmp = stBuffer.Trim();
                            }
                        }
                        else
                        {
                            stBufferTmp = stBuffer.Trim();
                        }
                    }

                    idx = 0;
                    foreach (char ch in stBufferTmp)
                    {
                        if (isChineseString(ch.ToString()))
                        {
                            if (!((fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".CS") || fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".H") ||
                                fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".CPP") || fileName.Trim().Substring(fileName.Trim().LastIndexOf(".")).ToUpper().Equals(".SQL")) &&
                                isInMultiLineComment(stBufferTmp, idx)))
                            {
                                lstResult.Add(fileName + "(" + cnt + ") : " + stBuffer.Trim());
                                exists = true;
                                break;
                            }
                        }
                        idx++;
                    }
                }
            }

            cReader.Close();

            if (exists)
            {
                lstFilesChOrJp.Add(fileName);
                count++;
            }
        }

        private bool isInMultiLineComment(string buffer, int idx)
        {
            bool result = false;
            int pos = -1;

            if (Regex.IsMatch(buffer, @"/\\*.*\\*/"))
            {
                return true;
            }

            if (multiLineCommentStarted)
            {
                if ("/".Equals(buffer.Substring(idx, 1)) && idx > 0)
                {
                    pos = buffer.IndexOf("*/", idx - 1);
                    if (pos == idx - 1)
                    {
                        multiLineCommentStarted = false;
                    }
                }
                result = true;
            }
            else
            {
                if ("*".Equals(buffer.Substring(idx, 1)) && idx > 0)
                {
                    pos = buffer.IndexOf("/*", idx - 1);
                    if (pos == idx - 1)
                    {
                        multiLineCommentStarted = true;
                    }
                }
            }

            return result;
        }

        private bool isInAspxComment(string str)
        {
            bool result = false;

            result = Regex.IsMatch(str, @"<%--.*?--%>");

            return result;

        }

        private bool isJapaneseString(string str)
        {
            bool result = false;

            result = !(isChineseString(str) || isEnglishString(str));

            return result;
        }

        private bool isEnglishString(string str)
        {
            bool result = false;

            result = Regex.IsMatch(str, "[\u0009\u0020-\u007E]");

            return result;
        }

        private bool isChineseString(string str)
        {
            bool result = false;

            result = isChineseWord(str) || isChineseSign(str);

            return result;
        }

        private bool isChineseWord(string str)
        {
            bool result = false;

            result = Regex.IsMatch(str, "[\u4e00-\u9fa5]");

            return result;
        }

        private bool isChineseSign(string str)
        {
            bool result = false;

            result = Regex.IsMatch(str, @"[，。？：；‘’！“”－（）【】《》〈〉…、]");

            return result;
        }

        private void CopyDirectoryFile(string sourceDirNm, string destDirNm)
        {
            string destDirSub = string.Empty;
            string sourceFileDir = string.Empty;
            foreach (string filePath in lstFilesChOrJp)
            {
                sourceFileDir = new System.IO.FileInfo(filePath).DirectoryName;
                destDirSub = destDirNm + sourceFileDir.Substring(sourceDirNm.Length);

                // コピー先のディレクトリがないとき
                if (!System.IO.Directory.Exists(destDirSub))
                {
                    //DirectoryInfoオブジェクトを作成する
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(destDirSub);

                    di.Create();
                }

                // コピー先のディレクトリ名の末尾に"\"をつける
                if (destDirSub[destDirSub.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                {
                    destDirSub += System.IO.Path.DirectorySeparatorChar;
                }

                // コピー先のディレクトリにあるファイルをコピー
                System.IO.File.Copy(filePath, destDirSub + System.IO.Path.GetFileName(filePath), true);
            }
        }

        private void CopyDirectory(string sourceDirNm, string destDirNm)
        {
            // コピー先のディレクトリがないとき
            if (!System.IO.Directory.Exists(destDirNm))
            {
                System.IO.Directory.CreateDirectory(destDirNm);

                // 属性もコピー
                System.IO.File.SetAttributes(destDirNm, System.IO.File.GetAttributes(sourceDirNm));
            }

            // コピー先のディレクトリ名の末尾に"\"をつける
            if (destDirNm[destDirNm.Length - 1] != System.IO.Path.DirectorySeparatorChar)
            {
                destDirNm += System.IO.Path.DirectorySeparatorChar;
            }

            // コピー先のディレクトリにあるファイルをコピー
            //string[] files = System.IO.Directory.GetFiles(sourceDirNm);
            //foreach (string file in files)
            //{
            //    if (lstFilesChOrJp .Contains(file)){
            //        System.IO.File.Copy(file, destDirNm + System.IO.Path.GetFileName(file), true);
            //    }
            //}

            // コピー先のディレクトリにあるディレクトリについて、再帰的に呼び出す
            string[] dirs = System.IO.Directory.GetDirectories(sourceDirNm);
            foreach (string dir in dirs)
                CopyDirectory(dir, destDirNm + System.IO.Path.GetFileName(dir));
        }

        private void CreatDirectory(string destDirNm)
        {
            // コピー先のディレクトリがないとき
            if (!System.IO.Directory.Exists(destDirNm))
            {
                System.IO.Directory.CreateDirectory(destDirNm);
            }

            // コピー先のディレクトリ名の末尾に"\"をつける
            if (destDirNm[destDirNm.Length - 1] != System.IO.Path.DirectorySeparatorChar)
            {
                destDirNm += System.IO.Path.DirectorySeparatorChar;
            }

            // コピー先のディレクトリにあるディレクトリについて、再帰的に呼び出す
            string[] dirs = System.IO.Directory.GetDirectories(destDirNm);
            foreach (string dir in dirs)
                CreatDirectory(destDirNm + System.IO.Path.GetFileName(dir));
        }
    }
}

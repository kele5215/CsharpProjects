using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace LogReplace
{
    /// <summary>
    /// 指定キーでログファイルから対応文字列の置換処理を行うクラスです
    /// </summary>
    public class LogReplace
    {
        /// <summary>
        /// ログ出力
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public void ReplaceContentForKey()
        {
            // ログ開始
            _logger.Info("********************* Program Start *********************");

            // 入力ファイルのディレクトリ名
            string strInputFilePath = string.Empty;

            // 入力ファイルのファイル名
            string strInputFileNm = string.Empty;

            // 出力ファイルパス
            string strOutFilePath = string.Empty;

            // 出力ファイル
            string strOutPathWithNm = string.Empty;

            // 置換ファイルカウント
            int iReplaceFileCnt = 0;

            // 入力ファイルの接頭語
            string strInFilePrefix = string.Empty;

            try
            {
                // 入力パラメータから入力ファイルのディレクトリ名情報を取得する
                strInputFilePath = ConfigurationManager.AppSettings["InputPath"];

                // 入力ファイルのフォルダ存在の妥当性チェック
                if (!Directory.Exists(strInputFilePath))
                {
                    _logger.Error("指定輸入路徑不存在！請確認！");
                    return;
                }

                // 入力パラメータから出力ファイルのディレクトリ名情報を取得する
                strOutFilePath = ConfigurationManager.AppSettings["OutputPath"];

                // 出力ファイルのフォルダ存在の妥当性チェック
                if (!Directory.Exists(strOutFilePath))
                {
                    // 出力ファイル用フォルダ作成
                    Directory.CreateDirectory(strOutFilePath);
                }

                // 入力パラメータからファイルの接頭語を取得する
                strInFilePrefix = ConfigurationManager.AppSettings["InputFilePrefix"];

                // ファイル名正規表示初期化
                string strRegexFileNm = doRegexFileNm(strInFilePrefix);

                // フォルダ内のファイル名をワイルドコードで検索し､更に正規表現で検索する｡
                string[] files = FindFiles(strInputFilePath, strRegexFileNm, "*.log", true);

                // 指定接頭辞ファイルが存在しない場合、処理終了
                if (files.Length == 0)
                {
                    // 指定接頭辞ファイルが存在しない場合、処理終了
                    _logger.Info("當前目錄不存在'" + strInFilePrefix + "'開頭的日誌文件，請確認！");
                    return;
                }

                // 検索したファイルをループして、置換後ファイルを出力する
                foreach (string file in files)
                {
                    // 入力ファイルパスからファイル名、ディレクトリ名情報を取得する
                    strInputFilePath = Path.GetDirectoryName(file);
                    strInputFileNm = Path.GetFileNameWithoutExtension(file);

                    // 出力ファイルを設定する
                    strOutPathWithNm = Path.Combine(strOutFilePath, strInputFileNm + ".log");

                    // 出力ファイルの存在妥当性チェック
                    if (File.Exists(strOutPathWithNm))
                    {
                        _logger.Warn("'" + strOutPathWithNm + "'已經存在！ 請確認！");
                        continue;
                    }

                    // 入力ファイルの存在妥当性チェック
                    if (!File.Exists(file))
                    {
                        _logger.Warn("'" + file + "'不存在或者已經被替換！ 請確認！");
                        continue;
                    }

                    // 指定文字列が存在する場合、置換処理を行う
                    if (ContainTextInFile(file))
                    {
                        // ファイルの1行ずつ読み込み・書き込み
                        // 指定キーで(sentence、written)対応内容を置換する
                        FileContentReplace(file, strOutPathWithNm);

                        // 置換した後、該当ファイルを削除する
                        File.Delete(file);

                        _logger.Info("'" + Path.GetFileName(file) + "'被替換！");

                        iReplaceFileCnt++;
                    }
                    else
                    {
                        // 該当ファイルを移動する
                        File.Move(file, strOutPathWithNm);

                        _logger.Info("掃描'" + Path.GetFileName(file) + "'未發現指定內容！");
                    }
                }

                // 正常に出力する場合、正常メッセージを出力する
                _logger.Info(iReplaceFileCnt + "個" + "日誌文件已經被替換！");

            }
            catch (Exception ex)
            {
                // エラーの場合、置換したファイルを削除する
                File.Delete(strOutPathWithNm);

                _logger.Error("預期以外錯誤發生，請查看下面詳細日誌內容！");
                _logger.Error(ex.Message);
            }
            finally
            {
                // ログ終了
                _logger.Info("********************* Program End ***********************");
            }
        }

        /// <summary>
        /// ファイルの1行ずつ読み込み・書き込み
        /// 指定文字列(sentence、written)を置換する
        /// </summary>
        /// <param name="strInputPath">入力ファイル</param>
        /// <param name="strOutPath">出力ファイル</param>
        private void FileContentReplace(string strInputPath, string strOutPath)
        {
            // 開始ログ
            _logger.Debug("********************* 讀取文件查找替換 Start *********************");

            // ファイルの１行
            string strReplaceLine = string.Empty;

            //BOM無しのUTF8でテキストファイルを作成する
            System.Text.Encoding enc = new System.Text.UTF8Encoding(false);

            StreamWriter writer = null;
            StreamReader reader = null;

            try
            {
                //出力ファイルをオープンする
                using (writer = new StreamWriter(strOutPath, false, enc))
                {
                    using (reader = new StreamReader(strInputPath, Encoding.GetEncoding("UTF-8")))
                    {
                        strReplaceLine = string.Empty;
                        string line = string.Empty;

                        // 読み取り可能文字が存在しない(ファイルの末尾に到着)すると-1が返される
                        while ((line = reader.ReadLine()) != null)
                        {
                            // 1行ずつ読み込む指定文字列を置換する
                            // この時改行コードはWindows標準のCRLFとなる
                            strReplaceLine = ReplaceLine(line);

                            writer.WriteLine(strReplaceLine);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("讀取文件【" + Path.GetFileName(strInputPath) + "】時發生預期以外錯誤！");
                _logger.Error(ex.Message);

                throw;
            }
            finally
            {
                // 終了ログ
                _logger.Debug("********************* 讀取文件查找替換 End *********************");

                // 入力ファイルを閉じる
                if (reader != null)
                {
                    reader.Dispose();
                }

                // 出力ファイルを閉じる
                if (writer != null)
                {
                    writer.Dispose();
                }
            }
        }

        /// <summary>
        ///  1行ずつ読み込む指定文字列を置換する
        /// </summary>
        /// <param name="strLine">ファイルの一行文字列</param>
        /// <returns>置換後文字列</returns>
        private string ReplaceLine(string strLine)
        {
            // 正規表示初期化
            string strRegex = @"(((""sentence"":"")|(""written"":""))(?<=\"").*?(?=\"")\"")|(Request.Form\[callData\[0\]\]:)";
            Regex rgx = new Regex(strRegex, RegexOptions.IgnoreCase);

            System.Text.RegularExpressions.MatchCollection mc = rgx.Matches(strLine);

            // 該当行に指定キーが存在しない場合、そのままを戻す
            if (mc.Count == 0)
            {
                return strLine;
            }

            string strRtn = string.Empty;
            string strContent = string.Empty;

            foreach (System.Text.RegularExpressions.Match m in mc)
            {
                // 指定キーによって対応内容を「*」に変換する
                if (m.Value.Contains("sentence"))
                {
                    ReplaceKey(m.Index + 12, m.Length - 12, 1, ref strLine);
                }
                else if (m.Value.Contains("written"))
                {
                    ReplaceKey(m.Index + 11, m.Length - 11, 2, ref strLine);
                }
                else if (m.Value.Contains("Request.Form[callData[0]]"))
                {
                    ReplaceKey(m.Index + 27, strLine.Length - 28, 3, ref strLine);
                }
            }

            return strLine;
        }

        /// <summary>
        /// 指定文字列から対応内容を「*」に置換する
        /// </summary>
        /// <param name="iStartIndex">指定文字列の最初位置</param>
        /// <param name="iContentLength">置換文字列の長さ</param>
        /// <param name="iKeyType">キータイプ(1:sentence 2:written)</param>
        /// <param name="strLine">指定文字列</param>
        /// <returns></returns>
        private void ReplaceKey(int iStartIndex, int iContentLength, int iKeyType, ref string strLine)
        {
            string strRtn = string.Empty;
            string strContent = string.Empty;

            // 置換前文字内容
            strContent = strLine.Substring(iStartIndex, iContentLength - 1);

            // StringBuilderを作成する
            System.Text.StringBuilder sb = new System.Text.StringBuilder(strLine);

            // 置換後文字内容
            // 指定文字列の最初位置から置換文字列の長さ文字の範囲で、文字列を置換する
            sb.Replace(strContent, new string('*', iContentLength - 1), iStartIndex, iContentLength - 1);

            strLine = sb.ToString();

        }

        /// <summary>
        /// フォルダ内のファイル名をワイルドコードで検索し､更に正規表現で検索する｡
        /// </summary>
        /// <param name="path">対象フォルダ</param>
        /// <param name="pattern">正規表現</param>
        /// <param name="wildcard">ワイルドコード</param>
        /// <param name="ignorecase">trueの時大文字小文字を区別しない｡既定値はtrue</param>
        /// <returns></returns>
        private string[] FindFiles(string path, string pattern, string wildcard, bool ignorecase)
        {
            // 開始ログ
            _logger.Debug("********************* 通過正則表達式確定文件 Start *********************");

            // ファイル名用リスト
            List<string> lstFullName = new List<string>();

            // 正規表現初期化
            RegexOptions opt = RegexOptions.None;
            if (ignorecase) opt |= RegexOptions.IgnoreCase;
            Regex reg = new Regex(pattern, opt);

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles(wildcard, SearchOption.TopDirectoryOnly);

            // 指定フォルダ配下に何ファイルがない場合、空白配列を戻す
            if (files.Length == 0)
            {
                return new string[] { };
            }

            // 正規表現で検索する
            foreach (FileInfo f in files)
            {
                if (reg.IsMatch(f.FullName))
                {
                    lstFullName.Add(f.FullName);
                }
            }

            // 指定接頭辞が存在しない場合、空白配列を戻す
            if (lstFullName.Count == 0)
            {
                return new string[] { };
            }

            // 検索したファイルの更新日時をソートする
            lstFullName.Sort(delegate(String f1, String f2)
            {
                return DateTime.Compare(Directory.GetLastWriteTime(f1), Directory.GetLastWriteTime(f2));
            });

            // 検索したファイルから最新更新日時ファイルを戻さない
            if (lstFullName.Count > 1)
            {
                lstFullName.RemoveAt(lstFullName.Count - 1);
            }

            // 終了ログ
            _logger.Debug("********************* 通過正則表達式確定文件 End *********************");

            return lstFullName.ToArray();
        }

        /// <summary>
        /// 正規表現でファイル名を作成する｡
        /// </summary>
        /// <param name="strInFilePrefix">入力ファイルの接頭語(cc,rrなど)</param>
        /// <returns>正規表現でファイル名</returns>
        private string doRegexFileNm(string strInFilePrefix)
        {
            // 開始ログ
            _logger.Debug("********************* 通過正規表達式過濾文件名 Start *********************");

            string strRegexFileNm = string.Empty;

            // 正規表現でファイルタイプ
            string REGEX_FILE_TYPE = @"{0}-default|{0}-error";

            // yyyymmddフォーマット
            string strRegexYmd = @"\d{4}\d{1,2}\d{1,2}";

            // 接頭語配列
            string[] arrPrefix = strInFilePrefix.Split(',');

            // ファイル名正規表示 
            // 「@"(cc-default|cc-error)-(({0})|({0}-(([1-9][0-9]*)))).log";」様な正規表現を作成する
            StringBuilder sb = new StringBuilder();
            foreach (string strPrefix in arrPrefix)
            {
                sb.Append(string.Format(REGEX_FILE_TYPE, strPrefix)).Append("|");
            }
            string strRegexTmp = "(" + sb.ToString().TrimEnd('|') + @")-(({0})|({0}-(([1-9][0-9]*)))).log";

            // ファイル名正規表示初期化
            strRegexFileNm = String.Format(strRegexTmp, strRegexYmd);

            // 終了ログ
            _logger.Debug("********************* 通過正規表達式過濾文件名 End *********************");

            return strRegexFileNm;
        }

        /// <summary>
        /// ファイルの内容が指定文字列に一致するか調べる
        /// 正規表現で指定文字列が存在するかどうか
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>正規表現で結果</returns>
        private bool ContainTextInFile(string filePath)
        {

            // 開始ログ
            _logger.Debug("********************* 確定文件內容是否替換 Start *********************");

            //ファイルを読み込む
            StreamReader strm = null;

            // 正規表示初期化
            string strRegex = @"(((""sentence"":"")|(""written"":""))(?<=\"").*?(?=\"")\"")|(Request.Form\[callData\[0\]\]:)";
            Regex rgx = new Regex(strRegex, RegexOptions.IgnoreCase);

            string strTextIFile = string.Empty;

            try
            {
                //strm = new StreamReader(filePath, Encoding.GetEncoding("UTF-8"), true);
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                strm = new StreamReader(fs, Encoding.GetEncoding("UTF-8"));
                strTextIFile = strm.ReadToEnd();
            }
            catch (Exception ex)
            {
                _logger.Error("讀取文件【" + Path.GetFileName(filePath) + "】時發生預期以外錯誤！");
                _logger.Error(ex.Message);

                throw;
            }
            finally
            {
                strm.Close();
            }

            // 終了ログ
            _logger.Debug("********************* 確定文件內容是否替換 End *********************");

            return rgx.IsMatch(strTextIFile);
        }
    }
}

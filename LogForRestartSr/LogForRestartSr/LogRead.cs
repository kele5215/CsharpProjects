using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


namespace LogForRestartSr
{
    /// <summary>
    /// 指定キーでログファイルから対応文字列を探して、２回目の時間間隔を計算するクラスです
    /// </summary>
    public class LogRead
    {
        /// <summary>
        /// ログ出力
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// タスク実行間隔
        /// </summary>
        public int iTaskExeInterval = 0;

        /// <summary>
        /// 指定内容を現れる間隔
        /// </summary>
        public int iSpeContentInter = 0;

        /// <summary>
        /// ログファイル最後更新時間間隔
        /// </summary>
        private int iFileWriteInterval = 0;

        /// <summary>
        /// 指定キー
        /// </summary>
        private const string REGEX = @"Ami.Communication.Web.Services.Recognition.Queue Warning: 36175";

        /// <summary>
        /// 指定検索内容情報
        /// </summary>
        public List<SearchContentInfo> lstSelContentInfo = null;

        /// <summary>
        /// 該当プログラム実行開始時間
        /// </summary>
        public DateTime statrDatetime;

        public LogRead()
        {
            // 入力パラメータからタスク実行間隔を取得する
            iTaskExeInterval = int.Parse(ConfigurationManager.AppSettings["TaskExecuteInterval"]);

            // 入力パラメータから指定内容を現れる間隔を取得する
            iSpeContentInter = int.Parse(ConfigurationManager.AppSettings["SpecifiedContentInterval"]);

            // 入力パラメータからログファイル最後更新時間間隔を取得する
            iFileWriteInterval = int.Parse(ConfigurationManager.AppSettings["FileWriteInterval"]);

            statrDatetime = DateTime.Now;
        }

        public void FindContentForKey()
        {

            // 入力ファイルのディレクトリ名
            string strInputFilePath = string.Empty;

            // 入力ファイルのファイル名
            string strInputFileNm = string.Empty;

            // 入力ファイルの接頭語
            string strInFilePrefix = string.Empty;

            try
            {

                _logger.Debug("***FindContentForKey START***");

                // 入力パラメータから入力ファイルのディレクトリ名情報を取得する
                strInputFilePath = ConfigurationManager.AppSettings["InputPath"];

                // 入力ファイルのフォルダ存在の妥当性チェック
                if (!Directory.Exists(strInputFilePath))
                {
                    _logger.Error("指定輸入路徑不存在！請確認！");
                    return;
                }

                // 入力パラメータからファイルの接頭語を取得する
                strInFilePrefix = ConfigurationManager.AppSettings["InputFilePrefix"];

                // ファイル名正規表示初期化
                string strRegexFileNm = doRegexFileNm(strInFilePrefix);

                // フォルダ内のファイル名をワイルドコードで検索し､更に正規表現で検索する｡
                string[] files = FindFiles(strInputFilePath, strRegexFileNm, "*.log", true);

                // 検索したファイルをループして、指定内容を探す
                System.Text.RegularExpressions.MatchCollection mc = null;
                string fileContent = string.Empty;

                // 指定検索内容情報リスト初期化
                lstSelContentInfo = new List<SearchContentInfo>();

                // 正規表現でファイルタイプ
                string strDateOut = @"指定文字列對應時間-{0}:{1}";

                foreach (string file in files)
                {
                    _logger.Debug("讀取文件'" + Path.GetFileName(file) + "'");
                    _logger.Debug("文件路徑: '" + file + "'");
                    mc = ContainTextInFile(file, ref fileContent);

                    if (mc.Count > 0)
                    {
                        // 指定文字列が存在する場合、対応する時間を取得する
                        GetTimeForContent(mc, fileContent);

                        _logger.Info("'" + Path.GetFileName(file) + "'中指定文字列【" + REGEX + @"】存在！");
                        for (int index = lstSelContentInfo.Count - 1; index >= 0; index--)
                        {
                            SearchContentInfo item = (SearchContentInfo)lstSelContentInfo[index];

                            // 指定文字列対応時間を出力する
                            _logger.Info(string.Format(strDateOut, Convert.ToString(index + 1), item.selConForTime.ToString("O")));
                        }
                    }
                }

                _logger.Debug("***FindContentForKey END***");
            }
            catch (Exception)
            {
                _logger.Debug("***FindContentForKey ERR***");
                throw;
            }
        }

        /// <summary>
        /// 正規表現でファイル名を作成する｡
        /// </summary>
        /// <param name="strInFilePrefix">入力ファイルの接頭語(cc,rrなど)</param>
        /// <returns>正規表現でファイル名</returns>
        private string doRegexFileNm(string strInFilePrefix)
        {
            _logger.Debug("***doRegexFileNm START***");

            string strRegexFileNm = string.Empty;

            // 正規表現でファイルタイプ
            string REGEX_FILE_TYPE = @"{0}-error";

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

            _logger.Debug("***doRegexFileNm END***");

            return strRegexFileNm;
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
            _logger.Debug("***FindFiles START***");

            // ファイル名用リスト
            List<string> lstFullName = new List<string>();

            // 正規表現初期化
            RegexOptions opt = RegexOptions.None;
            if (ignorecase) opt |= RegexOptions.IgnoreCase;
            Regex reg = new Regex(pattern, opt);

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles(wildcard, SearchOption.TopDirectoryOnly);

            // ファイル最も遅い読み込む時間
            DateTime lastDatetime = statrDatetime.AddMinutes(-(iFileWriteInterval * 60));

            // 正規表現で検索する
            foreach (FileInfo f in files)
            {
                if (reg.IsMatch(f.FullName))
                {

                    if ((f.LastWriteTime.CompareTo(lastDatetime) >= 0) &&
                        (f.LastWriteTime.CompareTo(statrDatetime) <= 0))
                    {
                        lstFullName.Add(f.FullName);
                    }
                }
            }

            // 検索したファイルの更新日時をソートする
            lstFullName.Sort(delegate(String f1, String f2)
            {
                return DateTime.Compare(Directory.GetLastWriteTime(f1), Directory.GetLastWriteTime(f2));
            });

            _logger.Debug("***FindFiles END***");

            return lstFullName.ToArray();
        }

        /// <summary>
        /// ファイルの内容が指定文字列に一致するか調べる
        /// 正規表現で指定文字列が存在するかどうか
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="fileContent">ファイル内容</param>
        /// <returns>正規表現で結果</returns>
        private System.Text.RegularExpressions.MatchCollection ContainTextInFile(string filePath, ref string fileContent)
        {

            _logger.Debug("***ContainTextInFile START***");

            //ファイルを読み込む
            StreamReader strm = null;

            // 正規表示初期化
            string strRegex = REGEX + @".*?";
            Regex rgx = new Regex(strRegex, RegexOptions.IgnoreCase);

            string strTextIFile = string.Empty;

            try
            {
                _logger.Info("'" + Path.GetFileName(filePath) + "'讀取開始");
                _logger.Debug("'讀取文件路徑 : " + filePath + "'");
                //strm = new StreamReader(filePath, Encoding.GetEncoding("UTF-8"), true);
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                strm = new StreamReader(fs, Encoding.GetEncoding("UTF-8"));
                _logger.Info("'" + Path.GetFileName(filePath) + "'讀取終了");
                strTextIFile = strm.ReadToEnd();
                fileContent = strTextIFile;
            }
            catch (Exception ex)
            {
                _logger.Debug("***ContainTextInFile ERR***");
                _logger.Error(ex.Message);
                throw;
            }
            finally
            {
                strm.Close();
            }

            _logger.Debug("***ContainTextInFile END***");

            return rgx.Matches(strTextIFile);
        }

        /// <summary>
        ///  指定文字列を読込んで、対応する時間を取得する
        /// </summary>
        /// <param name="mc">正規で検索した文字列</param>
        /// <param name="fileContent">ファイル内容</param>
        private void GetTimeForContent(System.Text.RegularExpressions.MatchCollection mc, string fileContent)
        {
            _logger.Debug("***GetTimeForContent START***");

            string strRtn = string.Empty;
            string strContent = string.Empty;

            // 指定検索内容情報初期化
            SearchContentInfo selContentInfo = null;

            string strDateTime = string.Empty;

            foreach (System.Text.RegularExpressions.Match m in mc)
            {
                // 指定キーによって対応内容をリストへ格納する
                selContentInfo = new SearchContentInfo();

                selContentInfo.selConStartIndex = m.Index;
                selContentInfo.selTimeStartIndex = fileContent.IndexOf("DateTime", m.Index + REGEX.Length + 1);
                strDateTime = fileContent.Substring(selContentInfo.selTimeStartIndex + 9, 34);
                selContentInfo.selConForTime = DateTime.Parse(strDateTime);

                lstSelContentInfo.Add(selContentInfo);
            }

            _logger.Debug("***GetTimeForContent END***");
        }
    }
}

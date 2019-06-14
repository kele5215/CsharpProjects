using NLog;
using ServerConfSection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

namespace LogForRestartSr
{
    class Program
    {
        /// <summary>
        /// ログ出力
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// エラーサービスリスト
        /// </summary>
        public static List<ServerConfigElement> lstServiceErr = null;

        static void Main(string[] args)
        {

            // ログ開始
            _logger.Info("********************* Program Start *********************");

            // 異常発生フラグ
            bool bErrFlg = false;

            try
            {
                LogRead logRead = new LogRead();

                // 指定文字列を探して、対応する時間を探す
                logRead.FindContentForKey();

                if (logRead.lstSelContentInfo != null && logRead.lstSelContentInfo.Count > 0)
                {

                    // 対応する時間を繰り返して、時間間隔が10分を超える場合、
                    // サーバをRestartして、処理終了する
                    for (int index = logRead.lstSelContentInfo.Count - 1; index >= 0; index--)
                    {
                        SearchContentInfo item = (SearchContentInfo)logRead.lstSelContentInfo[index];

                        // 取得した時間が前回タスク間隔+指定内容を現れる間隔を超える場合、
                        // ループ終了
                        TimeSpan ts1 = logRead.statrDatetime - item.selConForTime;
                        if (ts1.TotalMinutes > logRead.iTaskExeInterval + logRead.iSpeContentInter)
                        {
                            // ログ終了
                            _logger.Info("cc-error日誌文件中最新出現的36175錯誤訊息間隔，未符合【當前的時間 - 最新出現的錯誤時間 > 計劃任務執行間隔 + 日誌文件中指定內容出現間隔】，按照未發現異常處理！");
                            break;
                        }

                        // 末尾のレコードの場合、ループ終了
                        if (index == 0)
                        {
                            break;
                        }

                        SearchContentInfo itemNext = (SearchContentInfo)logRead.lstSelContentInfo[index - 1];

                        TimeSpan ts2 = item.selConForTime - itemNext.selConForTime;
                        if (ts2.TotalMinutes >= logRead.iSpeContentInter)
                        {
                            // サーバRestart batファイルを実行する
                            ExecuteBatFileSc();

                            // 異常発生
                            bErrFlg = true;
                            break;
                        }
                    }
                    if (!bErrFlg)
                    {
                        // ログ終了
                        _logger.Info("指定時間間隔內監視cc-error日誌文件未發現異常！");
                    }
                }
                else
                {
                    // ログ終了
                    _logger.Info("監視cc-error日誌文件未發現異常！");
                }

                // 指定時間後にして、失敗したサービスを再実行する
                DoErrServicerRestartSc();
            }
            catch (Exception ex)
            {
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
        ///  サーバ再起動する時、指定batファイルを呼び出す
        /// </summary>
        private static void ExecuteBatFile()
        {
            _logger.Debug("***ExecuteBatFile START***");

            Process proc = null;
            try
            {
                // 独自に定義したセクション
                ServerSection serSection = ConfigurationManager.GetSection("server.serversection") as ServerSection;
                if (serSection != null)
                {
                    // 指定検索内容情報リスト初期化
                    lstServiceErr = new List<ServerConfigElement>();

                    foreach (ServerConfigElement elem in serSection.ServerElements)
                    {
                        foreach (String serviceNm in elem.ServiceNm.Split(','))
                        {
                            //Processオブジェクトを作成
                            proc = new Process();

                            //出力とエラーをストリームに書き込むようにする
                            proc.StartInfo.UseShellExecute = false;
                            proc.StartInfo.RedirectStandardOutput = true;
                            proc.StartInfo.RedirectStandardError = true;

                            // 実行パラメータ設定する
                            string targetDir = Environment.CurrentDirectory;
                            proc.StartInfo.WorkingDirectory = targetDir;
                            proc.StartInfo.FileName = "AmiVoiceStreamingRecognizer.bat";
                            proc.StartInfo.Arguments = string.Format("{0} {1} {2} {3} {4}",
                                elem.ServerAddress,
                                elem.User,
                                elem.Password,
                                serviceNm,
                                elem.ServiceInterval);

                            // //ウィンドウを表示しないようにする
                            proc.StartInfo.CreateNoWindow = false;
                            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                            //起動
                            proc.Start();

                            //プロセス終了まで待機する
                            //WaitForExitはReadToEndの後である必要がある
                            //(親プロセス、子プロセスでブロック防止のため)
                            proc.WaitForExit();

                            // 実行結果を判定して、対応処理を行う
                            if (proc.ExitCode == 0)
                            {
                                // ログ出力
                                _logger.Info("服務器:" + elem.ServerAddress + "的" + serviceNm + @"服務已經被重啟！");
                            }
                            else
                            {
                                if (proc.ExitCode == 99)
                                {
                                    // ログ出力
                                    _logger.Error("服務器:" + elem.ServerAddress + @"連接錯誤！");

                                    break;
                                }
                                else
                                {
                                    // エラーの場合、エラーサービスをリストに設定する
                                    elem.ServiceErrNm = serviceNm;
                                    lstServiceErr.Add(elem);

                                    // ログ出力
                                    _logger.Error("服務器:" + elem.ServerAddress + "的" + serviceNm + @"服務啟動錯誤！");
                                }
                            }
                            proc.Close();
                        }
                    }
                }
                _logger.Debug("***ExecuteBatFile END***");
            }
            catch (Exception)
            {
                _logger.Debug("***ExecuteBatFile ERR***");
                throw;
            }
        }

        /// <summary>
        ///  前回失敗したサービスを再実行する
        /// </summary>
        private static void DoErrServicerRestart()
        {
            _logger.Debug("***DoErrServicerRestart START***");

            if (lstServiceErr != null && lstServiceErr.Count > 0)
            {

                Process proc = null;

                // 入力パラメータからサービス再実行時間間隔を取得する
                int iErrServiceResInterval = int.Parse(ConfigurationManager.AppSettings["ErrServiceResInterval"]);
                System.Threading.Thread.Sleep(iErrServiceResInterval * 60 * 1000);

                foreach (ServerConfigElement serviceElem in lstServiceErr)
                {
                    //Processオブジェクトを作成
                    proc = new Process();

                    //出力とエラーをストリームに書き込むようにする
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;

                    // 実行パラメータ設定する
                    string targetDir = Environment.CurrentDirectory;
                    proc.StartInfo.WorkingDirectory = targetDir;
                    proc.StartInfo.FileName = "AmiVoiceStreamingRecognizer.bat";
                    proc.StartInfo.Arguments = string.Format("{0} {1} {2} {3} {4}",
                        serviceElem.ServerAddress,
                        serviceElem.User,
                        serviceElem.Password,
                        serviceElem.ServiceErrNm,
                        serviceElem.ServiceInterval);

                    // //ウィンドウを表示しないようにする
                    proc.StartInfo.CreateNoWindow = false;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    //起動
                    proc.Start();

                    //プロセス終了まで待機する
                    //WaitForExitはReadToEndの後である必要がある
                    //(親プロセス、子プロセスでブロック防止のため)
                    proc.WaitForExit();

                    // 実行結果を判定して、対応処理を行う
                    if (proc.ExitCode == 0)
                    {
                        // ログ出力
                        _logger.Info("服務器:" + serviceElem.ServerAddress + "的" + serviceElem.ServiceErrNm + @"服務再次被重啟！");
                    }
                    else
                    {
                        if (proc.ExitCode == 99)
                        {
                            // ログ出力
                            _logger.Error("服務器:" + serviceElem.ServerAddress + @"連接錯誤！");

                            break;
                        }
                        else
                        {
                            // ログ出力
                            _logger.Error("服務器:" + serviceElem.ServerAddress + "的" + serviceElem.ServiceErrNm + @"服務再次啟動失败！請確認後台服務！");
                        }
                    }
                    proc.Close();
                }
            }

            _logger.Debug("***DoErrServicerRestart END***");
        }

        /// <summary>
        ///  サーバ再起動する時、指定batファイルを呼び出す
        /// </summary>
        private static void ExecuteBatFileSc()
        {
            _logger.Debug("***ExecuteBatFileSc START***");

            WindowsServiceHelper windowsService = null;

            try
            {
                // 独自に定義したセクション
                ServerSection serSection = ConfigurationManager.GetSection("server.serversection") as ServerSection;

                // 入力パラメータからサービス起動タイプを取得する
                string strProStartupType = ConfigurationManager.AppSettings["ProgramStartupType"];

                if (serSection != null)
                {
                    // 指定検索内容情報リスト初期化
                    lstServiceErr = new List<ServerConfigElement>();

                    foreach (ServerConfigElement elem in serSection.ServerElements)
                    {
                        foreach (String serviceNm in elem.ServiceNm.Split(','))
                        {
                            try
                            {
                                //WindowsServiceHelperオブジェクトを作成
                                windowsService = new WindowsServiceHelper(elem.ServerAddress,
                                    elem.User,
                                    elem.Password,
                                    serviceNm,
                                    elem.ServiceInterval,
                                    strProStartupType);

                                //  サーバ再起動を実行する
                                windowsService.ExeStartService();

                                // ログ出力
                                _logger.Info("服務器:" + elem.ServerAddress + "的" + serviceNm + @"服務已經被重啟！");

                            }
                            catch (RestartSrException rsException)
                            {
                                if (rsException.ExceptionCd == RestartSrException.ExceptionCode.Err_97)
                                {
                                    // ログ出力
                                    string msg = rsException.Message + " : " + rsException.InnerException.Message;
                                    _logger.Error("服務器:" + elem.ServerAddress + @"連接錯誤！" + "錯誤信息如下所示：" + Environment.NewLine + msg);
                                    break;
                                }
                                else if (rsException.ExceptionCd == RestartSrException.ExceptionCode.Err_98)
                                {
                                    // ログ出力
                                    _logger.Error(@"指定IP:" + elem.ServerAddress + "下對應服务【" + serviceNm + "】不存在！");
                                    continue;
                                }
                                else
                                {
                                    // エラーの場合、エラーサービスをリストに設定する
                                    lstServiceErr.Add(CloneSerConfigItem(elem, serviceNm));

                                    // ログ出力
                                    _logger.Error("服務器:" + elem.ServerAddress + "的" + serviceNm + @"服務啟動錯誤！");
                                }
                            }
                            catch (Exception)
                            {
                                throw;
                            }

                        }
                    }
                }
                _logger.Debug("***ExecuteBatFileSc END***");
            }
            catch (Exception)
            {
                _logger.Debug("***ExecuteBatFileSc ERR***");
                throw;
            }
        }

        /// <summary>
        ///  前回失敗したサービスを再実行する
        /// </summary>
        private static void DoErrServicerRestartSc()
        {
            if (lstServiceErr != null && lstServiceErr.Count > 0)
            {
                _logger.Debug("***DoErrServicerRestartSc START***");

                WindowsServiceHelper windowsService = null;

                int iErrServiceResFre = 0;
                // 入力パラメータからサービス再実行回数を取得する
                int iErrServiceResFreMax = int.Parse(ConfigurationManager.AppSettings["ErrServiceResFrequency"]);

                // 入力パラメータからサービス再実行時間間隔を取得する
                int iErrServiceResInterval = int.Parse(ConfigurationManager.AppSettings["ErrServiceResInterval"]);
                System.Threading.Thread.Sleep(iErrServiceResInterval * 60 * 1000);

                // 入力パラメータからサービス起動タイプを取得する
                string strProStartupType = ConfigurationManager.AppSettings["ProgramStartupType"];

                try
                {
                    foreach (ServerConfigElement serviceElem in lstServiceErr)
                    {
                        for (iErrServiceResFre = 0; iErrServiceResFre < iErrServiceResFreMax; iErrServiceResFre++)
                        {
                            try
                            {
                                //WindowsServiceHelperオブジェクトを作成
                                windowsService = new WindowsServiceHelper(serviceElem.ServerAddress,
                                    serviceElem.User,
                                    serviceElem.Password,
                                    serviceElem.ServiceErrNm,
                                    serviceElem.ServiceInterval,
                                    strProStartupType);

                                //  サーバ再起動を実行する
                                windowsService.ExeStartService();

                                // ログ出力
                                _logger.Info("服務器:" + serviceElem.ServerAddress + "的" + serviceElem.ServiceErrNm + @"服務再次被重啟！");

                                // 正常で起動される場合、対応するサービスを削除する
                                lstServiceErr.Remove(serviceElem);

                                // 次のセービスを進む
                                continue;

                            }
                            catch (RestartSrException rsException)
                            {
                                // ログ出力
                                string msg = rsException.Message + " : " + rsException.InnerException.Message;
                                _logger.Error("服務器:" + serviceElem.ServerAddress + "的" +
                                    serviceElem.ServiceErrNm + @"服務再次啟動失败！(" + Convert.ToString(iErrServiceResFre + 1) + "回) 錯誤信息如下所示：" + Environment.NewLine + msg);
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                    _logger.Debug("***DoErrServicerRestartSc END***");
                }
                catch (Exception)
                {
                    _logger.Debug("***DoErrServicerRestartSc ERR***");
                    throw;
                }
            }
        }

        private static ServerConfigElement CloneSerConfigItem(ServerConfigElement elem, String serviceNm)
        {
            ServerConfigElement serviceElemClone = new ServerConfigElement();

            serviceElemClone.ServerNm = elem.ServerNm;
            serviceElemClone.ServerAddress = elem.ServerAddress;
            serviceElemClone.User = elem.User;
            serviceElemClone.Password = elem.Password;
            serviceElemClone.ServiceNm = elem.ServiceNm;
            serviceElemClone.ServiceInterval = elem.ServiceInterval;
            serviceElemClone.ServiceErrNm = serviceNm;

            return serviceElemClone;
        }
    }
}

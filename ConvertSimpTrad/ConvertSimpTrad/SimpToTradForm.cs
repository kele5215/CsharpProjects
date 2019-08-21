using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ConvertSimpTrad
{
    public partial class ConvertSimpTrad : Form
    {
        private DataTable fileItemTable = null;

        private List<string> lstFileFormat = new List<string>();

        private List<string> lstResxFiles = new List<string>();
        private List<string> lstIssFiles = new List<string>();
        private List<string> lstJsFiles = new List<string>();
        private List<string> lstSqlFiles = new List<string>();

        private List<string> lstNewResxFiles = new List<string>();


        public ConvertSimpTrad()
        {
            InitializeComponent();
        }

        private void SimpToTrad_Load(object sender, EventArgs e)
        {
            //DataGridBing(GetDataTable());
        }

        #region イベント処理

        #region フォルダ選択処理

        private void folder_Click(object sender, EventArgs e)
        {
            // フォルダー参照ダイアログのインスタンスを生成
            var dlg = new System.Windows.Forms.FolderBrowserDialog();

            // 説明文を設定
            dlg.Description = "既存フォルダーを選択してください。";

            // ダイアログを表示
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                Button btn = sender as Button;
                if ("btnExistFolder".Equals(btn.Name))
                {
                    // 選択されたフォルダーパスをメッセージボックスに表示
                    txtExistChFolder.Text = dlg.SelectedPath;
                }
                else if ("btnOutFolderSel".Equals(btn.Name))
                {
                    // 選択されたフォルダーパスをメッセージボックスに表示
                    txtOutFolder.Text = dlg.SelectedPath;
                }
                //else
                //{
                //    // 選択されたフォルダーパスをメッセージボックスに表示
                //    txtOutFolder.Text = dlg.SelectedPath;
                //}
            }
        }

        #endregion

        #region 繁体ファイル作成処理
        private void btnTwFileMake_Click(object sender, EventArgs e)
        {
            string strRootPath = this.txtExistChFolder.Text.Trim();

            int resxFileCnt = 0;
            int sqlFileCnt = 0;
            int issFileCnt = 0;
            int jsFileCnt = 0;

            try
            {
                getSelectedFileType();

                if (lstFileFormat.Count == 0)
                {
                    //メッセージボックスを表示する
                    MessageBox.Show("少なくとも1つのファイルタイプを選択してください。",
                        "警告",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                GetFilesMostDeep(strRootPath, ref resxFileCnt, ref issFileCnt, ref jsFileCnt);

                // sqlファイル名には「zh-cn」を含めていないので、
                //「\Database\DDL\SQLServer\zh-CN」フォルダをコピーして作成する
                if (lstFileFormat.Contains(".SQL"))
                {
                    makeSqlTwFile(strRootPath, ref sqlFileCnt);
                }

                txtIssFileCnt.Text = Convert.ToString(issFileCnt);
                txtResxFileCnt.Text = Convert.ToString(resxFileCnt);
                txtSqlFileCnt.Text = Convert.ToString(sqlFileCnt);
                txtJsFileCnt.Text = Convert.ToString(jsFileCnt);

                //メッセージボックスを表示する
                MessageBox.Show("繁体ファイル作成処理実行完了。",
                    "正常完了",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                btnSimpToTw.Enabled = true;

            }
            catch (Exception ex)
            {
                //メッセージボックスを表示する
                MessageBox.Show("繁体ファイル作成処理未完成。\n" + ex.Message,
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 繁体内容変換処理

        private void btnSimpToTw_Click(object sender, EventArgs e)
        {

            int girdDataIndex = 0;
            int resxItemCnt = 0;

            if (lstFileFormat.Count == 0)
            {
                MessageBox.Show("少なくとも1つのファイルタイプを選択してください");
                return;
            }

            try
            {
                //*********************************
                // 初始化非托管资源
                //*********************************
                _Application appWord = new Microsoft.Office.Interop.Word.Application();
                object template = Missing.Value;
                object newTemplate = Missing.Value;
                object docType = Missing.Value;
                object visible = true;
                Document doc = appWord.Documents.Add(ref template, ref newTemplate, ref docType, ref visible);

                // ファイル操作用
                StreamReader reader = null;
                StreamWriter writer = null;

                // ファイルの１行
                string strReplaceLine = string.Empty;

                //BOM無しのUTF8でテキストファイルを作成する
                System.Text.Encoding enc = new System.Text.UTF8Encoding(false);


                try
                {
                    // RESXファイル変換処理
                    foreach (string resxFile in lstResxFiles)
                    {
                        ResourceLanguage resxCnFileContent = new ResourceLanguage(resxFile);
                        ResourceLanguage resxTwFileContent = new ResourceLanguage(resxFile.Replace("zh-CN", "zh-TW"));
                        foreach (string itemKey in resxCnFileContent.ResourceKeys)
                        {
                            if (string.IsNullOrEmpty(resxCnFileContent.GetValue(itemKey)))
                            {
                                continue;
                            }
                            resxTwFileContent.SetValue(itemKey, CHSToCHT(resxCnFileContent.GetValue(itemKey), appWord, doc));
                            resxTwFileContent.Save();

                            resxItemCnt += 1;
                        }
                    }

                    // ISSファイル変換処理
                    foreach (string issFile in lstIssFiles)
                    {
                        //using (FileStream fileStream = File.Open(issFile.Replace("zh-CN", "zh-TW"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        //{
                        //    using (reader = new StreamReader(fileStream, enc))
                        //    using (writer = new StreamWriter(fileStream, enc))
                        //    {
                        //        string preString = reader.ReadToEnd();
                        //        fileStream.Position = 0;
                        //        strReplaceLine = ReplaceLine(preString, appWord, doc);
                        //        writer.Write(strReplaceLine);
                        //    }
                        //}
                        UpdateContentInFile(issFile, appWord, doc, "ISS");
                    }

                    // ISSファイル変換処理
                    foreach (string jsFile in lstJsFiles)
                    {
                        //using (FileStream fileStream = File.Open(issFile.Replace("zh-CN", "zh-TW"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        //{
                        //    using (reader = new StreamReader(fileStream, enc))
                        //    using (writer = new StreamWriter(fileStream, enc))
                        //    {
                        //        string preString = reader.ReadToEnd();
                        //        fileStream.Position = 0;
                        //        strReplaceLine = ReplaceLine(preString, appWord, doc);
                        //        writer.Write(strReplaceLine);
                        //    }
                        //}
                        UpdateContentInFile(jsFile, appWord, doc, "JS");
                    }

                    // SQLファイル変換処理
                    foreach (string sqlFile in lstSqlFiles)
                    {
                        if (".SQL".Equals(Path.GetExtension(sqlFile).ToUpper()))
                        {
                            UpdateContentInFile(sqlFile, appWord, doc, "SQL");
                        }
                        else if (".CMD".Equals(Path.GetExtension(sqlFile).ToUpper()))
                        {
                            using (FileStream fileStream = File.Open(sqlFile, FileMode.Open, FileAccess.ReadWrite))
                            {
                                using (reader = new StreamReader(fileStream, System.Text.Encoding.GetEncoding("shift_jis")))
                                using (writer = new StreamWriter(fileStream, System.Text.Encoding.GetEncoding("shift_jis")))
                                {
                                    string preString = reader.ReadToEnd();
                                    fileStream.Position = 0;
                                    strReplaceLine = preString.Replace("Chinese_PRC_CI_AS", "Chinese_Taiwan_Stroke_CI_AS");
                                    writer.Write(strReplaceLine);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
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

                    //*********************************
                    // 关闭非托管资源
                    //*********************************
                    object saveChange = 0;
                    object originalFormat = Missing.Value;
                    object routeDocument = Missing.Value;
                    appWord.Quit(ref saveChange, ref originalFormat, ref routeDocument);
                    GC.Collect();//进程资源释放


                }

                //*********************************
                // Datagird表示処理
                //*********************************
                GetDataTable();

                foreach (string resxFile in lstResxFiles)
                {
                    ResourceLanguage resxCnFileContent = new ResourceLanguage(resxFile);
                    ResourceLanguage resxTwFileContent = new ResourceLanguage(resxFile.Replace("zh-CN", "zh-TW"));

                    // girdDataIndex初期化
                    girdDataIndex = 0;
                    foreach (string itemKey in resxCnFileContent.ResourceKeys)
                    {
                        // ファイル内容をGirdデータに追加
                        DataRow newRowItem;
                        newRowItem = fileItemTable.NewRow();
                        newRowItem["item_parent_id"] = doParentId(resxFile);
                        newRowItem["item_key"] = itemKey;
                        newRowItem["item_index"] = girdDataIndex + 1;
                        newRowItem["item_simp_content"] = resxCnFileContent.GetValue(itemKey);
                        newRowItem["item_trad_content"] = resxTwFileContent.GetValue(itemKey);
                        newRowItem["item_file_path"] = resxFile;

                        fileItemTable.Rows.Add(newRowItem);
                        girdDataIndex++;
                    }
                }

                DataGridBing(fileItemTable);

                txtResxItemCnt.Text = Convert.ToString(resxItemCnt);
                btnExcel.Enabled = true;
                btnOutFolder.Enabled = true;

                //メッセージボックスを表示する
                MessageBox.Show("繁体変換処理実行完了。",
                    "正常完了",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

            }
            catch (Exception SimpToTwEx)
            {
                //メッセージボックスを表示する
                MessageBox.Show("繁体変換処理未完成。\n" + SimpToTwEx.Message,
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
        }

        #endregion

        #region ファイルエクスポート
        private void btnOutFolder_Click(object sender, EventArgs e)
        {
            string strRootPath = this.txtExistChFolder.Text.Trim();

            string strDestPath = this.txtOutFolder.Text.Trim();

            if (string.IsNullOrEmpty(strDestPath) || lstFileFormat.Count == 0)
            {
                //メッセージボックスを表示する
                MessageBox.Show("少なくとも1つのファイルタイプを選択してください。\n または エクスポートフォルダを指定してください。",
                    "警告",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (lstResxFiles.Count == 0 && lstIssFiles.Count == 0 && lstJsFiles.Count == 0 && lstSqlFiles.Count == 0)
            {
                MessageBox.Show("繁体ファイル未作成！");
                return;
            }

            List<string> lstFiles = new List<string>();

            foreach (string fileFormat in lstFileFormat)
            {

                if (".ISS".Equals(fileFormat))
                {
                    lstFiles.AddRange(lstIssFiles);

                }
                else if (".RESX".Equals(fileFormat))
                {

                    lstFiles.AddRange(lstResxFiles);

                }
                else if (".JS".Equals(fileFormat))
                {

                    lstFiles.AddRange(lstJsFiles);
                }
                else if (".SQL".Equals(fileFormat))
                {

                    lstFiles.AddRange(lstSqlFiles);
                }

                CopyDirectoryFile(strRootPath, strDestPath, lstFiles);
            }

            //メッセージボックスを表示する
            MessageBox.Show("ファイルエクスポート処理実行完了。",
                "正常完了",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        #endregion

        #region 新しいResxファイル作成
        private void btnNewResxCreate_Click(object sender, EventArgs e)
        {
            string strRootPath = this.txtExistChFolder.Text.Trim();

            string strDestPath = this.txtOutFolder.Text.Trim();

            int newResxFileCnt = 0;

            string str314RootPath = @"C:\work\developer\3.1.4CN(A-Law)";
            string str314FilePath = string.Empty;
            string str314FileContent = string.Empty;

            List<string> lst314NotExistFiles = new List<string>();

            if (string.IsNullOrEmpty(strDestPath))
            {
                //メッセージボックスを表示する
                MessageBox.Show("エクスポートフォルダを指定してください。",
                    "警告",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            GetFilesMostDeepFromJa(strRootPath, ref newResxFileCnt);

            // RESXファイル変換処理
            foreach (string resxFile in lstNewResxFiles)
            {
                ResourceLanguage resxCnFileContent = new ResourceLanguage(resxFile);

                str314FilePath = str314RootPath + resxFile.Substring(strDestPath.Length).Replace(".zh-CN", "");
                if (!File.Exists(str314FilePath))
                {
                    lst314NotExistFiles.Add(resxFile);
                    continue;
                }

                ResourceLanguage resx314FileContent = new ResourceLanguage(str314FilePath);

                foreach (string itemKey in resxCnFileContent.ResourceKeys)
                {
                    if (string.IsNullOrEmpty(resxCnFileContent.GetValue(itemKey)))
                    {
                        continue;
                    }

                    str314FileContent = resx314FileContent.GetValue(itemKey);
                    if (string.IsNullOrEmpty(str314FileContent))
                    {
                        continue;
                    }

                    resxCnFileContent.SetValue(itemKey, str314FileContent);
                    resxCnFileContent.Save();

                }
            }

            foreach (string resxFile in lst314NotExistFiles)
            {
                System.Console.WriteLine(resxFile);
            }


            //メッセージボックスを表示する
            MessageBox.Show(string.Format("新しいResxファイルエクスポート処理実行完了。({0})件", newResxFileCnt),
                "正常完了",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        #endregion

        #region dataGirdChild内容表示

        //点击展开事件
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            //如果不是父节点就返回
            if (this.dataGridView1.Rows[e.RowIndex].Cells["IsEx"].Value == null)
            {
                return;
            }
            string isEx = this.dataGridView1.Rows[e.RowIndex].Cells["IsEx"].Value.ToString();
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == "EX" && isEx == "false")
            {
                if (this.dataGridView1.Rows[e.RowIndex].Cells["IsInsert"].Value.ToString() == "false")
                {
                    string parentId = this.dataGridView1.Rows[e.RowIndex].Cells["item_parent_id"].Value.ToString();
                    DataTable table = GetChildRowTable(parentId);
                    if (table.Rows.Count > 0)
                    {
                        //插入行
                        this.dataGridView1.Rows.Insert(e.RowIndex + 1, table.Rows.Count);
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex + i + 1];
                            if (i % 2 == 0)
                            {
                                row.DefaultCellStyle.BackColor = Color.Beige;
                            }
                            else
                            {
                                row.DefaultCellStyle.BackColor = Color.Bisque;
                            }
                            row.Cells["item_key"].Value = table.Rows[i]["item_key"];
                            row.Cells["item_index"].Value = table.Rows[i]["item_index"];
                            row.Cells["item_simp_content"].Value = table.Rows[i]["item_simp_content"];
                            row.Cells["item_trad_content"].Value = table.Rows[i]["item_trad_content"];
                        }
                        this.dataGridView1.Rows[e.RowIndex].Cells["IsInsert"].Value = "true";
                        this.dataGridView1.Rows[e.RowIndex].Cells["RowCount"].Value = table.Rows.Count;
                    }
                }
                else
                {
                    //显示数据
                    int RowCount = Convert.ToInt32(this.dataGridView1.Rows[e.RowIndex].Cells["RowCount"].Value);
                    for (int i = 1; i <= RowCount; i++)
                    {
                        this.dataGridView1.Rows[e.RowIndex + i].Visible = true;
                    }
                }
                //将IsEx设置为true，标明该节点已经展开
                this.dataGridView1.Rows[e.RowIndex].Cells["IsEx"].Value = "true";
                this.dataGridView1.Rows[e.RowIndex].Cells["EX"].Value = "-";
            }
            else if (this.dataGridView1.Columns[e.ColumnIndex].Name == "EX" && isEx == "true")
            {
                //string id = this.dataGridView1.Rows[e.RowIndex].Cells["ID"].Value.ToString();
                //DataTable table = GetDataTable("select * from Department where DparentId=" + id);
                //if (table.Rows.Count > 0)
                //{
                //    //利用Remove
                //    //for (int i = 0; i < table.Rows.Count; i++)
                //    //{
                //    //    foreach (DataGridViewRow row in this.dataGridView1.Rows)
                //    //    {
                //    //        if (row.Cells["ID"].Value.Equals(table.Rows[i]["ID"]))
                //    //        {
                //    //            this.dataGridView1.Rows.Remove(row);
                //    //        }
                //    //    }
                //    //}

                //    //利用RemoveAt
                //    for (int i = table.Rows.Count; i > 0; i--)
                //    {
                //        //删除行
                //        //this.dataGridView1.Rows.RemoveAt(i + e.RowIndex);
                //        //隐藏行
                //        this.dataGridView1.Rows[i + e.RowIndex].Visible = false;
                //    }
                //}
                int RowCount = Convert.ToInt32(this.dataGridView1.Rows[e.RowIndex].Cells["RowCount"].Value);
                for (int i = 1; i <= RowCount; i++)
                {
                    //隐藏行
                    this.dataGridView1.Rows[e.RowIndex + i].Visible = false;
                }
                ////将IsEx设置为false，标明该节点已经收缩
                this.dataGridView1.Rows[e.RowIndex].Cells["IsEx"].Value = "false";
                this.dataGridView1.Rows[e.RowIndex].Cells["EX"].Value = "+";
            }
        }

        #endregion

        #region EXCELファイル出力

        private void btnExcel_Click(object sender, EventArgs e)
        {
            OutputAsExcelFile(this.dataGridView1);
        }

        #endregion

        //private void btnWord_Click(object sender, EventArgs e)
        //{
        //    OutPutAsWordFile(this.dataGridView1);
        //}

        #endregion

        #region プライベート

        private DataTable GetDataTable()
        {
            fileItemTable = new DataTable();

            fileItemTable.Columns.Add(new DataColumn("ID", typeof(int)));
            fileItemTable.Columns.Add(new DataColumn("item_parent_id", typeof(string)));
            fileItemTable.Columns.Add(new DataColumn("item_key", typeof(string)));
            fileItemTable.Columns.Add(new DataColumn("item_index", typeof(int)));
            fileItemTable.Columns.Add(new DataColumn("item_simp_content", typeof(string)));
            fileItemTable.Columns.Add(new DataColumn("item_trad_content", typeof(string)));
            fileItemTable.Columns.Add(new DataColumn("item_file_path", typeof(string)));


            //DataRow dr = fileItemTable.NewRow();
            //dr["ID"] = 1;
            //dr["item_file_path"] = "item_file_path_1";
            //dr["item_parent_id"] = "item_parent_id_1";
            //dr["item_key"] = "item_key_1";
            //dr["item_index"] = 1;
            //dr["item_simp_content"] = "item_simp_content_1";
            //dr["item_trad_content"] = "item_trad_content_1";
            //fileItemTable.Rows.Add(dr);

            //dr = fileItemTable.NewRow();
            //dr["ID"] = 2;
            //dr["item_file_path"] = "item_file_path_1";
            //dr["item_parent_id"] = "item_parent_id_1";
            //dr["item_key"] = "item_key_2";
            //dr["item_index"] = 2;
            //dr["item_simp_content"] = "item_simp_content_2";
            //dr["item_trad_content"] = "item_trad_content_2";
            //fileItemTable.Rows.Add(dr);

            //dr = fileItemTable.NewRow();
            //dr["ID"] = 3;
            //dr["item_file_path"] = "item_file_path_2";
            //dr["item_parent_id"] = "item_parent_id_2";
            //dr["item_key"] = "item_key_3";
            //dr["item_index"] = 1;
            //dr["item_simp_content"] = "item_simp_content_3";
            //dr["item_trad_content"] = "item_trad_content_3";
            //fileItemTable.Rows.Add(dr);

            //dr = fileItemTable.NewRow();
            //dr["ID"] = 4;
            //dr["item_file_path"] = "item_file_path_2";
            //dr["item_parent_id"] = "item_parent_id_2";
            //dr["item_key"] = "item_key_4";
            //dr["item_index"] = 2;
            //dr["item_simp_content"] = "item_simp_content_4";
            //dr["item_trad_content"] = "item_trad_content_4";
            //fileItemTable.Rows.Add(dr);

            return fileItemTable;
        }

        private DataTable GetChildRowTable(string parentId)
        {

            var rows = fileItemTable.AsEnumerable()
                .Where(p => p.Field<string>("item_parent_id") == parentId);

            //通过CopyToDataTable()方法创建新的副本
            DataTable childRowDt = rows.CopyToDataTable<DataRow>();
            foreach (var item in childRowDt.AsEnumerable())
            {
                System.Console.WriteLine(item["item_parent_id"]);
            }

            return childRowDt;
        }

        private string doParentId(string resxFile)
        {
            string strParentId = string.Empty;

            FileInfo fileInfo = new FileInfo(resxFile);
            string fileNm = Path.GetFileNameWithoutExtension(resxFile);
            strParentId = fileInfo.Directory.FullName.Replace(txtExistChFolder.Text, "") + @"\" + fileNm;
            strParentId = strParentId.Replace(@"\", @"_").Substring(1);

            return strParentId;
        }

        //绑定GRID数据
        private void DataGridBing(DataTable table)
        {
            if (table.Rows.Count > 0)
            {
                string strFilePathSave = string.Empty;
                for (int i = 0; i < table.Rows.Count; i++)
                {

                    if (strFilePathSave.Equals(Convert.ToString(table.Rows[i]["item_file_path"])))
                    {
                        continue;
                    }
                    int k = this.dataGridView1.Rows.Add();
                    DataGridViewRow row = this.dataGridView1.Rows[k];
                    row.Cells["item_parent_id"].Value = table.Rows[i]["item_parent_id"];
                    row.Cells["item_key"].Value = table.Rows[i]["item_file_path"];
                    //row.Cells["item_file_path"].Value = table.Rows[i]["item_file_path"];
                    //row.Cells["item_simp_content"].Value = table.Rows[i]["item_simp_content"];
                    //用于显示该行是否已经展开
                    row.Cells["IsEx"].Value = "false";
                    //用于显示展开或收缩符号，为了简单我就直接用字符串了，其实用图片比较美观
                    row.Cells["EX"].Value = "+";
                    //是否插入
                    row.Cells["IsInsert"].Value = "false";

                    strFilePathSave = Convert.ToString(table.Rows[i]["item_file_path"]);
                }
            }
        }

        private void getSelectedFileType()
        {
            if (chkResx.Checked)
            {
                lstFileFormat.Add(chkResx.Text.ToUpper());
            }
            if (chkSql.Checked)
            {
                lstFileFormat.Add(chkSql.Text.ToUpper());
            }
            if (chkIss.Checked)
            {
                lstFileFormat.Add(chkIss.Text.ToUpper());
            }
            if (chkJs.Checked)
            {
                lstFileFormat.Add(chkJs.Text.ToUpper());
            }

        }

        private void GetFilesMostDeep(string path, ref int resxFileCnt, ref int issFileCnt, ref int jsFileCnt)
        {
            if (!string.IsNullOrEmpty(path))
            {
                foreach (string fp in System.IO.Directory.GetFiles(path))
                {
                    string strDot = fp.Substring(fp.LastIndexOf(".")).ToUpper();
                    if ((lstFileFormat.Contains(strDot)) && !fp.Contains("Assist") && !fp.Contains("Test"))
                    {
                        if (fp.Contains("zh-CN") && !".SQL".Equals(strDot))
                        {
                            makeZhTwFile(fp);

                            if (".ISS".Equals(strDot))
                            {
                                lstIssFiles.Add(fp);
                                issFileCnt += 1;
                            }
                            else if (".RESX".Equals(strDot))
                            {
                                lstResxFiles.Add(fp);
                                resxFileCnt += 1;
                            }
                            else if (".JS".Equals(strDot))
                            {
                                lstJsFiles.Add(fp);
                                jsFileCnt += 1;
                            }
                        }
                    }
                }

                foreach (string dp in System.IO.Directory.GetDirectories(path))
                {
                    if (dp.Contains("svn") || dp.Contains("bin") || dp.Contains("obj"))
                    {
                        continue;
                    }
                    GetFilesMostDeep(dp, ref resxFileCnt, ref issFileCnt, ref jsFileCnt);
                }
            }
        }

        private void GetFilesMostDeepFromJa(string sourceDirNm, ref int resxFileCnt)
        {
            if (!string.IsNullOrEmpty(sourceDirNm))
            {
                foreach (string fp in System.IO.Directory.GetFiles(sourceDirNm))
                {
                    string strDot = System.IO.Path.GetExtension(fp).ToUpper();
                    if (!fp.Contains("Assist") && !fp.Contains("Test"))
                    {
                        if (fp.Contains(".ja") && !".SQL".Equals(strDot))
                        {
                            makeZhFromJaFile(fp, ref resxFileCnt);
                        }
                    }
                }

                foreach (string dp in System.IO.Directory.GetDirectories(sourceDirNm))
                {
                    if (dp.Contains("svn") || dp.Contains("bin") || dp.Contains("obj"))
                    {
                        continue;
                    }
                    GetFilesMostDeepFromJa(dp, ref resxFileCnt);
                }
            }
        }

        private void makeZhTwFile(string strZhCnFilePath)
        {

            FileInfo fileInfo = new FileInfo(strZhCnFilePath);

            // 新しいファイル名
            string strNewFile = fileInfo.FullName.Replace("zh-CN", "zh-TW");

            // Use Path class to manipulate file and directory paths.
            string sourceFile = fileInfo.FullName;
            string destFile = fileInfo.FullName.Replace("zh-CN", "zh-TW");

            // To copy a file to another location and 
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(sourceFile, destFile, true);
        }

        private void makeZhFromJaFile(string strJpFilePath, ref int resxFileCnt)
        {

            string destDirSub = string.Empty;
            string sourceFileDir = string.Empty;

            string strRootPath = this.txtExistChFolder.Text.Trim();
            string strDestPath = this.txtOutFolder.Text.Trim();


            sourceFileDir = new System.IO.FileInfo(strJpFilePath).DirectoryName;
            destDirSub = strDestPath + sourceFileDir.Substring(strRootPath.Length);

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
            string strNewFilePath = destDirSub +  System.IO.Path.GetFileName(strJpFilePath).Replace(".ja", ".zh-CN");
            System.IO.File.Copy(strJpFilePath, strNewFilePath, true);

            if (".RESX".Equals(System.IO.Path.GetExtension(strNewFilePath).ToUpper()))
            {
                lstNewResxFiles.Add(strNewFilePath);
                resxFileCnt += 1;
            }

        }

        private void makeSqlTwFile(string strRootPath, ref int sqlFileCnt)
        {
            string fileName = string.Empty;
            string destFile = string.Empty;

            string sourcePath = Path.Combine(strRootPath, @"Database\DDL\SQLServer\zh-CN");
            string targetPath = sourcePath.Replace("zh-CN", "zh-TW");

            // Create a new target folder. 
            // If the directory already exists, this method does not create a new directory.
            System.IO.Directory.CreateDirectory(targetPath);

            if (System.IO.Directory.Exists(sourcePath))
            {
                string[] files = System.IO.Directory.GetFiles(sourcePath);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string sourceFile in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    fileName = System.IO.Path.GetFileName(sourceFile);
                    destFile = System.IO.Path.Combine(targetPath, fileName);
                    System.IO.File.Copy(sourceFile, destFile, true);

                    lstSqlFiles.Add(destFile);
                    if (".SQL".Equals(Path.GetExtension(destFile).ToUpper()))
                    {
                        sqlFileCnt += 1;
                    }
                }
            }
            else
            {
                Console.WriteLine("対応する中国語SQL用フォルダを存在しないので、ご確認お願い致します!");

                //メッセージボックスを表示する
                MessageBox.Show("対応する中国語SQL用フォルダを存在しないので、ご確認お願い致します!",
                    "警告",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void CopyDirectoryFile(string sourceDirNm, string destDirNm, List<string> lstFiles)
        {
            string destDirSub = string.Empty;
            string sourceFileDir = string.Empty;

            foreach (string filePath in lstFiles)
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
                System.IO.File.Copy(filePath.Replace("zh-CN", "zh-TW"), destDirSub + System.IO.Path.GetFileName(filePath).Replace("zh-CN", "zh-TW"), true);
            }
        }

        private string CHSToCHT(string src, _Application appWord, Document doc)
        {
            string des = "";
            appWord.Selection.TypeText(src);
            appWord.Selection.Range.TCSCConverter(WdTCSCConverterDirection.wdTCSCConverterDirectionSCTC, true, true);
            appWord.ActiveDocument.Select();
            des = appWord.Selection.Text.TrimEnd('\r');
            doc = null;
            appWord = null;
            return des;
        }

        ///  1行ずつ読み込んで
        /// </summary>
        /// <param name="strLine">ファイルの一行文字列</param>
        /// <returns>置換文字列かどうか</returns>
        private bool isReplaceLine(string strLine)
        {
            // 正規表示初期化
            string strRegex = @"(?:ChineseSimplified).+[\w]+=+\S.*";
            Regex rgx = new Regex(strRegex, RegexOptions.IgnoreCase);

            System.Text.RegularExpressions.MatchCollection mc = rgx.Matches(strLine);

            // 該当行に指定キーが存在しない場合、そのままを戻す
            if (mc.Count == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///  1行ずつ読み込む指定文字列を置換する
        /// </summary>
        /// <param name="strLine">ファイルの一行文字列</param>
        /// <returns>置換後文字列</returns>
        private string ReplaceLine(string strLine, _Application appWord, Document doc)
        {
            // 正規表示初期化
            string strRegex = @"(?:ChineseSimplified).+[\w]+=+\S.*";
            Regex rgx = new Regex(strRegex, RegexOptions.IgnoreCase);

            System.Text.RegularExpressions.MatchCollection mc = rgx.Matches(strLine);

            // 該当行に指定キーが存在しない場合、そのままを戻す
            if (mc.Count == 0)
            {
                return strLine;
            }

            string strContent = string.Empty;

            // StringBuilderを作成する
            System.Text.StringBuilder sb = new System.Text.StringBuilder(strLine);

            foreach (System.Text.RegularExpressions.Match m in mc)
            {
                strContent = CHSToCHT(m.Value, appWord, doc) + ('\r');
                sb.Replace(m.Value, strContent);
            }

            sb.Replace("ChineseSimplified", "ChineseTraditional");

            return sb.ToString();
        }

        private void UpdateContentInFile(string file, _Application appWord, Document doc, string fileType)
        {
            StringBuilder strB = new StringBuilder();

            // StringBuilderを作成する
            System.Text.StringBuilder strBMC = null;

            //BOM無しのUTF8でテキストファイルを作成する
            System.Text.Encoding enc = new System.Text.UTF8Encoding(true);

            // 正規表示初期化
            string strRegex = string.Empty;
            string strRegexSql = string.Empty;
            if ("ISS".Equals(fileType))
            {
                strRegex = @"(?:ChineseSimplified).+[\w]+=+\S.*";
            }
            else if ("JS".Equals(fileType))
            {
                strRegex = @"(\w+): ('[\s\S]*?')";
            }
            else if ("SQL".Equals(fileType))
            {
                strRegex = @"[\u4e00-\u9fa5]";
                strRegexSql = @"(\/\*(\s|.)*?)|(\*\/)|(\*(\s))|--.*";
            }

            Regex rgx = new Regex(strRegex, RegexOptions.IgnoreCase);
            Regex rgxSql = new Regex(strRegexSql, RegexOptions.IgnoreCase);

            string strContent = string.Empty;

            using (FileStream fin = new FileStream(file.Replace("zh-CN", "zh-TW"), FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fin, Encoding.GetEncoding("UTF-8")))
                try
                {
                    string strLine = sr.ReadLine();

                    while (strLine != null)
                    {
                        if ("SQL".Equals(fileType))
                        {
                            // 該当行にコメントじゃなくて漢字を含める場合、繫体字になる
                            if (!rgxSql.IsMatch(strLine) && rgx.IsMatch(strLine))
                            {
                                strLine = CHSToCHT(strLine, appWord, doc);
                            }
                        }
                        else
                        {
                            System.Text.RegularExpressions.MatchCollection mc = rgx.Matches(strLine);

                            // 該当行に指定キーが存在しない場合、そのままを戻す
                            if (mc.Count != 0)
                            {
                                strBMC = new System.Text.StringBuilder(strLine);
                                foreach (System.Text.RegularExpressions.Match m in mc)
                                {
                                    strContent = CHSToCHT(m.Value, appWord, doc);
                                    strBMC.Replace(m.Value, strContent);

                                    strLine = strBMC.ToString();
                                }
                            }
                        }
                        strB.Append(strLine + "\r\n");

                        strLine = sr.ReadLine();
                    }

                    if ("ISS".Equals(fileType))
                    {
                        strB.Replace("ChineseSimplified", "ChineseTraditional");
                    }
                    else if ("SQL".Equals(fileType))
                    {
                        strB.Replace("Chinese_PRC_CI_AS", "Chinese_Taiwan_Stroke_CI_AS");
                    }

                    sr.Close();
                    fin.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            using (FileStream fout = new FileStream(file.Replace("zh-CN", "zh-TW"), FileMode.Open, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fout, enc))
                try
                {
                    sw.Write(strB.ToString());
                    sw.Close();
                    fout.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
        }

        /// <summary>
        /// 导出为Excel格式
        /// </summary>
        /// <param name="dataGridView"></param>
        private void OutputAsExcelFile(DataGridView dataGridView)
        {
            //将datagridView中的数据导出到一张表中
            DataTable tempTable = this.exporeDataToTable(dataGridView);
            //导出信息到Excel表
            Microsoft.Office.Interop.Excel.ApplicationClass myExcel;
            Microsoft.Office.Interop.Excel.Workbooks myWorkBooks;
            Microsoft.Office.Interop.Excel.Workbook myWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet myWorkSheet;
            char myColumns;
            Microsoft.Office.Interop.Excel.Range myRange;
            object[,] myData = new object[500, 35];
            int i, j;//j代表行,i代表列
            myExcel = new Microsoft.Office.Interop.Excel.ApplicationClass();
            //显示EXCEL
            myExcel.Visible = true;
            if (myExcel == null)
            {
                MessageBox.Show("本地Excel程序无法启动!请检查您的Microsoft Office正确安装并能正常使用", "提示");
                return;
            }
            myWorkBooks = myExcel.Workbooks;
            myWorkBook = myWorkBooks.Add(System.Reflection.Missing.Value);
            myWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)myWorkBook.Worksheets[1];
            myColumns = (char)(tempTable.Columns.Count + 64);//设置列
            myRange = myWorkSheet.get_Range("A4", myColumns.ToString() + "5");//设置列宽
            int count = 0;
            //设置列名
            foreach (DataColumn myNewColumn in tempTable.Columns)
            {
                myData[0, count] = myNewColumn.ColumnName;
                count = count + 1;
            }
            //输出datagridview中的数据记录并放在一个二维数组中
            j = 1;
            foreach (DataRow myRow in tempTable.Rows)//循环行
            {
                for (i = 0; i < tempTable.Columns.Count; i++)//循环列
                {
                    myData[j, i] = myRow[i].ToString();
                }
                j++;
            }
            //将二维数组中的数据写到Excel中
            myRange = myRange.get_Resize(tempTable.Rows.Count + 1, tempTable.Columns.Count);//创建列和行
            myRange.Value2 = myData;
            myRange.EntireColumn.AutoFit();
        }

        /// <summary>
        /// 导出为Word格式
        /// </summary>
        /// <param name="dataGridView"></param>
        private void OutPutAsWordFile(DataGridView dataGridView)
        {
            //转换后的表
            DataTable table = exporeDataToTable(this.dataGridView1);

            Microsoft.Office.Interop.Word.ApplicationClass wordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            Microsoft.Office.Interop.Word.Document document;
            Microsoft.Office.Interop.Word.Table wordTable;
            Microsoft.Office.Interop.Word.Selection wordSelection;
            object wordObj = System.Reflection.Missing.Value;

            document = wordApp.Documents.Add(ref wordObj, ref wordObj, ref wordObj, ref wordObj);
            wordSelection = wordApp.Selection;
            //显示word文档
            wordApp.Visible = true;
            if (wordApp == null)
            {
                MessageBox.Show("本地Word程序无法启动!请检查您的Microsoft Office正确安装并能正常使用", "提示");
                return;
            }
            document.Select();
            wordTable = document.Tables.Add(wordSelection.Range, dataGridView.Rows.Count + 1, dataGridView.Columns.Count, ref wordObj, ref wordObj);
            //设置列宽
            wordTable.Columns.SetWidth(50.0F, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustSameWidth);

            //标题数据
            for (int i = 0; i < table.Columns.Count; i++)
            {
                wordTable.Cell(1, i + 1).Range.InsertAfter(table.Columns[i].ColumnName);
            }
            //输出表中数据
            for (int i = 0; i <= table.Rows.Count - 1; i++)
            {
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    if (table.Rows[i][j] != null)
                    {
                        wordTable.Cell(i + 2, j + 1).Range.InsertAfter(table.Rows[i][j].ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 转换过滤数据
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <returns></returns>
        private DataTable exporeDataToTable(DataGridView dataGridView)
        {
            //将datagridview中的数据导入到表中
            DataTable tempTable = new DataTable("tempTable");
            //定义一个模板表，专门用来获取列名
            DataTable modelTable = new DataTable("ModelTable");
            //创建列
            for (int column = 0; column < dataGridView.Columns.Count; column++)
            {
                //可见的列才显示出来
                if (dataGridView.Columns[column].Visible == true)
                {
                    DataColumn tempColumn = new DataColumn(dataGridView.Columns[column].HeaderText, typeof(string));
                    tempTable.Columns.Add(tempColumn);
                    DataColumn modelColumn = new DataColumn(dataGridView.Columns[column].Name, typeof(string));
                    modelTable.Columns.Add(modelColumn);
                }
            }
            //添加datagridview中行的数据到表
            for (int row = 0; row < dataGridView.Rows.Count; row++)
            {
                if (dataGridView.Rows[row].Visible == false)
                {
                    continue;
                }
                DataRow tempRow = tempTable.NewRow();
                for (int i = 0; i < tempTable.Columns.Count; i++)
                {
                    tempRow[i] = dataGridView.Rows[row].Cells[modelTable.Columns[i].ColumnName].Value;
                }
                tempTable.Rows.Add(tempRow);
            }
            return tempTable;
        }

        #endregion

    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ConvertSimpTrad
{
    public partial class ConvertSimpTrad : Form
    {
        private DataTable fileItemTable = null;

        private List<string> lstFileFormat = new List<string>();

        private List<string> lstFiles = new List<string>();


        public ConvertSimpTrad()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataGridBing(GetDataTable());
        }

        #region イベント処理

        #region 既存プログラムフォルダ選択処理

        private void existFolder_Click(object sender, EventArgs e)
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
                //else if ("btnExistJpFolder".Equals(btn.Name))
                //{
                //    // 選択されたフォルダーパスをメッセージボックスに表示
                //    txtExistChFolder.Text = dlg.SelectedPath;
                //}
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


            DataRow dr = fileItemTable.NewRow();
            dr["ID"] = 1;
            dr["item_file_path"] = "item_file_path_1";
            dr["item_parent_id"] = "item_parent_id_1";
            dr["item_key"] = "item_key_1";
            dr["item_index"] = 1;
            dr["item_simp_content"] = "item_simp_content_1";
            dr["item_trad_content"] = "item_trad_content_1";
            fileItemTable.Rows.Add(dr);

            dr = fileItemTable.NewRow();
            dr["ID"] = 2;
            dr["item_file_path"] = "item_file_path_1";
            dr["item_parent_id"] = "item_parent_id_1";
            dr["item_key"] = "item_key_2";
            dr["item_index"] = 2;
            dr["item_simp_content"] = "item_simp_content_2";
            dr["item_trad_content"] = "item_trad_content_2";
            fileItemTable.Rows.Add(dr);

            dr = fileItemTable.NewRow();
            dr["ID"] = 3;
            dr["item_file_path"] = "item_file_path_2";
            dr["item_parent_id"] = "item_parent_id_2";
            dr["item_key"] = "item_key_3";
            dr["item_index"] = 1;
            dr["item_simp_content"] = "item_simp_content_3";
            dr["item_trad_content"] = "item_trad_content_3";
            fileItemTable.Rows.Add(dr);

            dr = fileItemTable.NewRow();
            dr["ID"] = 4;
            dr["item_file_path"] = "item_file_path_2";
            dr["item_parent_id"] = "item_parent_id_2";
            dr["item_key"] = "item_key_4";
            dr["item_index"] = 2;
            dr["item_simp_content"] = "item_simp_content_4";
            dr["item_trad_content"] = "item_trad_content_4";
            fileItemTable.Rows.Add(dr);

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

        private void GetFilesMostDeep(string path, ref int resxFileCnt, ref int issFileCnt,ref int jsFileCnt)
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
                            lstFiles.Add(fp);

                            makeZhTwFile(fp);

                            if (".ISS".Equals(strDot))
                            {
                                issFileCnt += 1;
                            }
                            else if (".RESX".Equals(strDot))
                            {
                                resxFileCnt += 1;
                            }
                            else if (".JS".Equals(strDot))
                            {
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

                    sqlFileCnt += 1;
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


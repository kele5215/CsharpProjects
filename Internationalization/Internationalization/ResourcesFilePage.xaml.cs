using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
// フォルダー選択ダイアログの名前空間を using
using Forms = System.Windows.Forms;
using Model = Ami.UI.View.Model;

namespace Internationalization
{
    /// <summary>
    /// Page1.xaml の相互作用ロジック
    /// </summary>
    public partial class Page1 : Page
    {

        // Resourceファイル用リスト
        private IList<ResourceModel> resList = new List<ResourceModel>();

        public Page1()
        {
            InitializeComponent();

            txtExistChFolder.Text = @"C:\work\developer\3.1.4CN(A-Law)";
            txtExistJpFolder.Text = @"C:\work\developer\3.1.4JP(A-Law)";
        }

        private void existFolder_Click(object sender, RoutedEventArgs e)
        {
            // フォルダー参照ダイアログのインスタンスを生成
            var dlg = new Forms.FolderBrowserDialog();

            // 説明文を設定
            dlg.Description = "既存フォルダーを選択してください。";

            // ダイアログを表示
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                Button btn = sender as Button;
                if ("btnExistChFolder".Equals(btn.Name))
                {
                    // 選択されたフォルダーパスをメッセージボックスに表示
                    txtExistChFolder.Text = dlg.SelectedPath;
                }
                else if ("btnExistJpFolder".Equals(btn.Name))
                {
                    // 選択されたフォルダーパスをメッセージボックスに表示
                    txtExistJpFolder.Text = dlg.SelectedPath;
                }
                else
                {
                    // 選択されたフォルダーパスをメッセージボックスに表示
                    txtOutFolder.Text = dlg.SelectedPath;
                }
            }
        }

        private void ResourcesFileSel_Click(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show("照会処理開始");

            if (string.IsNullOrEmpty(txtExistChFolder.Text))
            {
                MessageBox.Show("既存中国語プロジェクトフォルダを選択してください。");
                return;
            }
            else
            {
                FileInfo[] files = FileUtility.FindFiles(txtExistChFolder.Text);

                TreeDataBind(files);
            }
        }

        private void ResourcesFileShow_Click(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show("ファイル内容表示処理開始");

            int childSelectIndex = 0;
            int girdDataIndex = 0;

            // Resourceファイル用リスト初期化
            resList.Clear();

            // テーブルの初期化
            try
            {

                if (string.IsNullOrEmpty(txtExistJpFolder.Text))
                {
                    MessageBox.Show("既存日本語プロジェクトフォルダを選択してください。");
                    return;
                }

                DataTable dtShowResxContent = InitTables();

                // 選択したファイルを判定する
                ResourceLanguage resxJpFileContent = null;
                foreach (Model.TreeModel tree in amiTreeView.ItemsSourceData)
                {
                    foreach (Model.TreeModel treeChild in tree.Children)
                    {
                        if (treeChild.IsChecked)
                        {
                            childSelectIndex++;

                            ResourceLanguage resxChFileContent = new ResourceLanguage(treeChild.PathInfo);

                            string jpResxPath = GetJpResxPath(treeChild.PathInfo);
                            if (File.Exists(jpResxPath))
                            {
                                resxJpFileContent = new ResourceLanguage(jpResxPath);
                            }

                            // Resourceファイル内容設定
                            ResourceModel multipleRes = new ResourceModel();
                            multipleRes.ResourceOld = resxChFileContent;
                            multipleRes.ResourceNew = resxJpFileContent;
                            multipleRes.MultipleResList.Add(multipleRes);

                            resList.Add(multipleRes);

                            // girdDataIndex初期化
                            girdDataIndex = 0;
                            foreach (string itemKey in resxChFileContent.ResourceKeys)
                            {
                                // ファイル内容をGirdデータに追加
                                DataRow newRowItem;
                                newRowItem = dtShowResxContent.NewRow();
                                newRowItem["item_key"] = itemKey;
                                newRowItem["item_index"] = girdDataIndex + 1;
                                newRowItem["item_ch_content"] = resxChFileContent.GetValue(itemKey);
                                if (resxJpFileContent != null)
                                {
                                    newRowItem["item_jp_content"] = resxJpFileContent.GetValue(itemKey);
                                }
                                else
                                {
                                    newRowItem["item_jp_content"] = "";
                                }

                                dtShowResxContent.Rows.Add(newRowItem);
                                girdDataIndex++;
                            }
                        }
                    }
                }

                if (childSelectIndex > 0)
                {
                    // グリッドにバインド
                    dataGrid1.DataContext = dtShowResxContent;
                }
                else
                {
                    // グリッドにバインド
                    dataGrid1.DataContext = null;
                    MessageBox.Show("Resourceファイルを選択してください。");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void ResourcesFileReplace_Click(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show("切替処理開始");

            int childSelectIndex = 0;
            int girdDataIndex = 0;

            // テーブルの初期化
            try
            {
                if (dataGrid1.DataContext == null)
                {
                    MessageBox.Show("Resourceファイルを選択してください。");
                    return;
                }

                // 対応日本語の内容を切替
                foreach (ResourceModel resxModel in resList)
                {
                    foreach (string itemKey in resxModel.ResourceOld.ResourceKeys)
                    {
                        if (String.IsNullOrEmpty(resxModel.ResourceOld.GetValue(itemKey)) ||
                            resxModel.ResourceNew == null ||
                            String.CompareOrdinal(resxModel.ResourceOld.GetValue(itemKey), resxModel.ResourceNew.GetValue(itemKey)) == 0)
                        {
                            continue;
                        }
                        else
                        {
                            resxModel.ResourceNew.SetValue(itemKey, resxModel.ResourceOld.GetValue(itemKey));
                            resxModel.ResourceNew.Save();
                        }
                    }
                }

                // Datagird再表示
                DataTable dtShowResxContent = InitTables();

                // 選択したファイルを判定する
                ResourceLanguage resxJpFileContent = null;
                foreach (Model.TreeModel tree in amiTreeView.ItemsSourceData)
                {
                    foreach (Model.TreeModel treeChild in tree.Children)
                    {
                        if (treeChild.IsChecked)
                        {
                            childSelectIndex++;

                            ResourceLanguage resxChFileContent = new ResourceLanguage(treeChild.PathInfo);
                            string jpResxPath = GetJpResxPath(treeChild.PathInfo);
                            if (File.Exists(jpResxPath))
                            {
                                resxJpFileContent = new ResourceLanguage(jpResxPath);
                            }

                            // girdDataIndex初期化
                            girdDataIndex = 0;
                            foreach (string itemKey in resxChFileContent.ResourceKeys)
                            {
                                // ファイル内容をGirdデータに追加
                                DataRow newRowItem;
                                newRowItem = dtShowResxContent.NewRow();
                                newRowItem["item_key"] = itemKey;
                                newRowItem["item_index"] = girdDataIndex + 1;
                                newRowItem["item_ch_content"] = resxChFileContent.GetValue(itemKey);
                                if (resxJpFileContent != null)
                                {
                                    newRowItem["item_jp_content"] = resxJpFileContent.GetValue(itemKey);
                                }
                                else
                                {
                                    newRowItem["item_jp_content"] = "";
                                }
                                dtShowResxContent.Rows.Add(newRowItem);
                                girdDataIndex++;
                            }
                        }
                    }
                }

                if (childSelectIndex > 0)
                {
                    // グリッドにバインド
                    dataGrid1.DataContext = dtShowResxContent;
                }
                else
                {
                    // グリッドにバインド
                    dataGrid1.DataContext = null;
                    MessageBox.Show("Resourceファイルを選択してください。");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        /// <summary>
        /// テーブルの初期化
        /// </summary>
        private DataTable InitTables()
        {
            // datagirdのDatatableを生成する
            DataTable m_table_mst_item = new DataTable("mst_item");
            m_table_mst_item.Columns.Add(new DataColumn("item_key", typeof(string)));                   // key名
            m_table_mst_item.Columns.Add(new DataColumn("item_index", typeof(int)));                    // 番号
            m_table_mst_item.Columns.Add(new DataColumn("item_ch_content", typeof(string)));            // 中国語内容
            m_table_mst_item.Columns.Add(new DataColumn("item_jp_content", typeof(string)));            // 日本語内容

            // m_table_mst_item.PrimaryKey = new DataColumn[] { m_table_mst_item.Columns["item_key"] };    // プライマリキーの設定
            //m_table_mst_item.Columns["item_index"].Unique = true;                                       // ユニークキーの設定
            //m_table_mst_item.Columns["item_index"].AutoIncrement = true;                                // 自動採番の設定
            //m_table_mst_item.Columns["item_index"].AutoIncrementSeed = 1;                               // 自動採番は1から始まり
            //m_table_mst_item.Columns["item_index"].AutoIncrementStep = 1;                               // 自動採番は1ずつ増加

            return m_table_mst_item;
        }

        /// <summary>
        /// 数据绑定
        /// </summary>
        private void TreeDataBind(FileInfo[] files)
        {
            IList<Ami.UI.View.Model.TreeModel> treeList = new List<Model.TreeModel>();

            // 入力ファイル情報を再ソートする
            Array.Sort(files, new FileInfoComparer());
            //Array.ForEach<FileInfo>(files, (s) => Console.WriteLine(string.Format("{0}", s.FullName)));

            string strTreeNm = new DirectoryInfo(((FileInfo)files[0]).FullName).Parent.Parent.Name;

            Model.TreeModel tree = new Model.TreeModel();

            int fileIndex = 0;

            foreach (FileInfo fileInfo in files)
            {
                DirectoryInfo dInfo = new DirectoryInfo(fileInfo.FullName);

                if (strTreeNm.CompareTo(dInfo.Parent.Parent.Name) != 0 ||
                    files.Length - 1 == fileIndex)
                {
                    treeList.Add(tree);

                    if (files.Length - 1 != fileIndex)
                    {
                        tree = new Model.TreeModel();

                        // 爺爺節點設定
                        tree.Id = dInfo.Parent.Parent.Name;
                        tree.Name = dInfo.Parent.Parent.Name;
                        tree.IsExpanded = true;

                        strTreeNm = dInfo.Parent.Parent.Name;
                    }
                }
                else
                {
                    if (fileIndex == 0)
                    {
                        // 爺爺節點設定
                        tree.Id = dInfo.Parent.Parent.Name;
                        tree.Name = dInfo.Parent.Parent.Name;
                        tree.IsExpanded = true;

                        strTreeNm = dInfo.Parent.Parent.Name;
                    }
                }

                // 父節點設定
                Model.TreeModel child = new Model.TreeModel();
                child.Id = dInfo.Parent.Name + @"\" + fileInfo.Name;
                child.Name = dInfo.Parent.Name + @"\" + fileInfo.Name;
                child.PathInfo = dInfo.FullName;
                child.Parent = tree;
                tree.Children.Add(child);

                fileIndex++;

            }

            amiTreeView.ItemsSourceData = treeList;

        }

        internal class FileInfoComparer : IComparer<FileInfo>
        {
            public int Compare(FileInfo x, FileInfo y)
            {
                DirectoryInfo xFile = new DirectoryInfo(x.FullName);
                DirectoryInfo yFile = new DirectoryInfo(y.FullName);

                if (xFile.Parent.Parent.ToString().CompareTo(yFile.Parent.Parent.ToString()) != 0)
                {
                    return xFile.Parent.Parent.ToString().CompareTo(yFile.Parent.Parent.ToString());
                }
                else
                {
                    if (xFile.Parent.ToString().CompareTo(yFile.Parent.ToString()) != 0)
                    {
                        return xFile.Parent.ToString().CompareTo(yFile.Parent.ToString());
                    }
                    else
                    {
                        return x.Name.CompareTo(y.Name);
                    }
                }
            }
        }

        /// <summary>
        /// 中国語版のResourceファイルによる、対応日本語Resourceファイルのパスを取得する
        /// </summary>
        private string GetJpResxPath(string chFilePath)
        {
            string jpFilePath = "";

            FileInfo chFileInfo = new FileInfo(chFilePath);

            string subFolderPath = chFileInfo.DirectoryName.Substring(txtExistChFolder.Text.Length);

            string jpFileFolderS = txtExistJpFolder.Text;

            if (!jpFileFolderS.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                jpFileFolderS += Path.DirectorySeparatorChar;
            }

            jpFilePath = jpFileFolderS +
                chFileInfo.FullName.Substring(txtExistChFolder.Text.Length + 1);

            return jpFilePath;
        }
    }

}

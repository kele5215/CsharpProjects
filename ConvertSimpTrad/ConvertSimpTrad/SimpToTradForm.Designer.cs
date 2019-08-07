namespace ConvertSimpTrad
{
    partial class ConvertSimpTrad
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.item_file_path = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.item_parent_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsInsert = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RowCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsEx = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.item_key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.item_index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.item_simp_content = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.item_trad_content = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnExcel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtExistChFolder = new System.Windows.Forms.TextBox();
            this.btnExistFolder = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkIss = new System.Windows.Forms.CheckBox();
            this.chkSql = new System.Windows.Forms.CheckBox();
            this.chkResx = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSimpToTw = new System.Windows.Forms.Button();
            this.btnTwFileMake = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtResxFileCnt = new System.Windows.Forms.TextBox();
            this.txtSqlFileCnt = new System.Windows.Forms.TextBox();
            this.txtIssFileCnt = new System.Windows.Forms.TextBox();
            this.chkJs = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtJsFileCnt = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.item_file_path,
            this.ID,
            this.item_parent_id,
            this.IsInsert,
            this.RowCount,
            this.EX,
            this.IsEx,
            this.item_key,
            this.item_index,
            this.item_simp_content,
            this.item_trad_content});
            this.dataGridView1.Location = new System.Drawing.Point(12, 197);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(817, 314);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // item_file_path
            // 
            this.item_file_path.HeaderText = "";
            this.item_file_path.Name = "item_file_path";
            this.item_file_path.Visible = false;
            // 
            // ID
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.ID.DefaultCellStyle = dataGridViewCellStyle1;
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.Visible = false;
            this.ID.Width = 30;
            // 
            // item_parent_id
            // 
            this.item_parent_id.HeaderText = "PARENT_ID";
            this.item_parent_id.Name = "item_parent_id";
            this.item_parent_id.Visible = false;
            // 
            // IsInsert
            // 
            this.IsInsert.HeaderText = "是否插入";
            this.IsInsert.Name = "IsInsert";
            this.IsInsert.Visible = false;
            // 
            // RowCount
            // 
            this.RowCount.HeaderText = "行数";
            this.RowCount.Name = "RowCount";
            this.RowCount.Visible = false;
            // 
            // EX
            // 
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.EX.DefaultCellStyle = dataGridViewCellStyle2;
            this.EX.HeaderText = "";
            this.EX.Name = "EX";
            this.EX.ReadOnly = true;
            this.EX.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EX.Width = 20;
            // 
            // IsEx
            // 
            this.IsEx.HeaderText = "是否显示";
            this.IsEx.Name = "IsEx";
            this.IsEx.Visible = false;
            // 
            // item_key
            // 
            this.item_key.HeaderText = "key名";
            this.item_key.Name = "item_key";
            // 
            // item_index
            // 
            this.item_index.HeaderText = "番号";
            this.item_index.Name = "item_index";
            // 
            // item_simp_content
            // 
            this.item_simp_content.HeaderText = "中国語简体";
            this.item_simp_content.Name = "item_simp_content";
            // 
            // item_trad_content
            // 
            this.item_trad_content.HeaderText = "中国語繁体";
            this.item_trad_content.Name = "item_trad_content";
            // 
            // btnExcel
            // 
            this.btnExcel.Location = new System.Drawing.Point(754, 517);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(75, 23);
            this.btnExcel.TabIndex = 1;
            this.btnExcel.Text = "导出EXCEL";
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "既存プログラムフォルダ";
            // 
            // txtExistChFolder
            // 
            this.txtExistChFolder.Location = new System.Drawing.Point(124, 18);
            this.txtExistChFolder.Name = "txtExistChFolder";
            this.txtExistChFolder.Size = new System.Drawing.Size(341, 19);
            this.txtExistChFolder.TabIndex = 3;
            this.txtExistChFolder.Text = "C:\\work\\developer\\3.4CN(A-Law)";
            // 
            // btnExistFolder
            // 
            this.btnExistFolder.Location = new System.Drawing.Point(471, 18);
            this.btnExistFolder.Name = "btnExistFolder";
            this.btnExistFolder.Size = new System.Drawing.Size(150, 20);
            this.btnExistFolder.TabIndex = 4;
            this.btnExistFolder.Text = "既存プログラムフォルダ選択";
            this.btnExistFolder.UseVisualStyleBackColor = true;
            this.btnExistFolder.Click += new System.EventHandler(this.existFolder_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkJs);
            this.groupBox1.Controls.Add(this.chkIss);
            this.groupBox1.Controls.Add(this.chkSql);
            this.groupBox1.Controls.Add(this.chkResx);
            this.groupBox1.Controls.Add(this.txtExistChFolder);
            this.groupBox1.Controls.Add(this.btnExistFolder);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(650, 75);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ファイル情報";
            // 
            // chkIss
            // 
            this.chkIss.AutoSize = true;
            this.chkIss.Location = new System.Drawing.Point(207, 53);
            this.chkIss.Name = "chkIss";
            this.chkIss.Size = new System.Drawing.Size(43, 16);
            this.chkIss.TabIndex = 5;
            this.chkIss.Text = ".ISS";
            this.chkIss.UseVisualStyleBackColor = true;
            // 
            // chkSql
            // 
            this.chkSql.AutoSize = true;
            this.chkSql.Location = new System.Drawing.Point(113, 53);
            this.chkSql.Name = "chkSql";
            this.chkSql.Size = new System.Drawing.Size(47, 16);
            this.chkSql.TabIndex = 5;
            this.chkSql.Text = ".SQL";
            this.chkSql.UseVisualStyleBackColor = true;
            // 
            // chkResx
            // 
            this.chkResx.AutoSize = true;
            this.chkResx.Location = new System.Drawing.Point(11, 53);
            this.chkResx.Name = "chkResx";
            this.chkResx.Size = new System.Drawing.Size(55, 16);
            this.chkResx.TabIndex = 5;
            this.chkResx.Text = ".RESX";
            this.chkResx.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSimpToTw);
            this.groupBox2.Controls.Add(this.btnTwFileMake);
            this.groupBox2.Location = new System.Drawing.Point(680, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(149, 75);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "処理実行";
            // 
            // btnSimpToTw
            // 
            this.btnSimpToTw.Location = new System.Drawing.Point(6, 44);
            this.btnSimpToTw.Name = "btnSimpToTw";
            this.btnSimpToTw.Size = new System.Drawing.Size(134, 20);
            this.btnSimpToTw.TabIndex = 7;
            this.btnSimpToTw.Text = "繁体转换";
            this.btnSimpToTw.UseVisualStyleBackColor = true;
            // 
            // btnTwFileMake
            // 
            this.btnTwFileMake.Location = new System.Drawing.Point(6, 18);
            this.btnTwFileMake.Name = "btnTwFileMake";
            this.btnTwFileMake.Size = new System.Drawing.Size(134, 20);
            this.btnTwFileMake.TabIndex = 7;
            this.btnTwFileMake.Text = "繁体ResxFile作成";
            this.btnTwFileMake.UseVisualStyleBackColor = true;
            this.btnTwFileMake.Click += new System.EventHandler(this.btnTwFileMake_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "Resxファイル作成件数：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "Sqlファイル作成件数：";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 169);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(108, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "Issファイル作成件数：";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtResxFileCnt
            // 
            this.txtResxFileCnt.Enabled = false;
            this.txtResxFileCnt.Location = new System.Drawing.Point(150, 100);
            this.txtResxFileCnt.Name = "txtResxFileCnt";
            this.txtResxFileCnt.ReadOnly = true;
            this.txtResxFileCnt.Size = new System.Drawing.Size(145, 19);
            this.txtResxFileCnt.TabIndex = 10;
            this.txtResxFileCnt.TabStop = false;
            this.txtResxFileCnt.Text = "0";
            this.txtResxFileCnt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtSqlFileCnt
            // 
            this.txtSqlFileCnt.Enabled = false;
            this.txtSqlFileCnt.Location = new System.Drawing.Point(150, 133);
            this.txtSqlFileCnt.Name = "txtSqlFileCnt";
            this.txtSqlFileCnt.ReadOnly = true;
            this.txtSqlFileCnt.Size = new System.Drawing.Size(145, 19);
            this.txtSqlFileCnt.TabIndex = 11;
            this.txtSqlFileCnt.TabStop = false;
            this.txtSqlFileCnt.Text = "0";
            this.txtSqlFileCnt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtIssFileCnt
            // 
            this.txtIssFileCnt.Enabled = false;
            this.txtIssFileCnt.Location = new System.Drawing.Point(150, 166);
            this.txtIssFileCnt.Name = "txtIssFileCnt";
            this.txtIssFileCnt.ReadOnly = true;
            this.txtIssFileCnt.Size = new System.Drawing.Size(145, 19);
            this.txtIssFileCnt.TabIndex = 12;
            this.txtIssFileCnt.TabStop = false;
            this.txtIssFileCnt.Text = "0";
            this.txtIssFileCnt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // chkJs
            // 
            this.chkJs.AutoSize = true;
            this.chkJs.Location = new System.Drawing.Point(297, 53);
            this.chkJs.Name = "chkJs";
            this.chkJs.Size = new System.Drawing.Size(40, 16);
            this.chkJs.TabIndex = 5;
            this.chkJs.Text = ".JS";
            this.chkJs.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(359, 103);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "Jsファイル作成件数：";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtJsFileCnt
            // 
            this.txtJsFileCnt.Enabled = false;
            this.txtJsFileCnt.Location = new System.Drawing.Point(488, 100);
            this.txtJsFileCnt.Name = "txtJsFileCnt";
            this.txtJsFileCnt.ReadOnly = true;
            this.txtJsFileCnt.Size = new System.Drawing.Size(145, 19);
            this.txtJsFileCnt.TabIndex = 12;
            this.txtJsFileCnt.TabStop = false;
            this.txtJsFileCnt.Text = "0";
            this.txtJsFileCnt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ConvertSimpTrad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.txtJsFileCnt);
            this.Controls.Add(this.txtIssFileCnt);
            this.Controls.Add(this.txtSqlFileCnt);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtResxFileCnt);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnExcel);
            this.Controls.Add(this.dataGridView1);
            this.Name = "ConvertSimpTrad";
            this.Text = "ConvertSimpTrad";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtExistChFolder;
        private System.Windows.Forms.Button btnExistFolder;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkIss;
        private System.Windows.Forms.CheckBox chkSql;
        private System.Windows.Forms.CheckBox chkResx;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSimpToTw;
        private System.Windows.Forms.Button btnTwFileMake;
        private System.Windows.Forms.DataGridViewTextBoxColumn item_file_path;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn item_parent_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsInsert;
        private System.Windows.Forms.DataGridViewTextBoxColumn RowCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn EX;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsEx;
        private System.Windows.Forms.DataGridViewTextBoxColumn item_key;
        private System.Windows.Forms.DataGridViewTextBoxColumn item_index;
        private System.Windows.Forms.DataGridViewTextBoxColumn item_simp_content;
        private System.Windows.Forms.DataGridViewTextBoxColumn item_trad_content;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtResxFileCnt;
        private System.Windows.Forms.TextBox txtSqlFileCnt;
        private System.Windows.Forms.TextBox txtIssFileCnt;
        private System.Windows.Forms.CheckBox chkJs;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtJsFileCnt;

    }
}


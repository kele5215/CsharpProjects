namespace ConvertSimpTrad
{
	partial class MoveGridForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.targetGrid = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Dtelphone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Daddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sourceGrid = new System.Windows.Forms.DataGridView();
            this.ToID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ToDname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ToDtelphone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ToDaddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.targetGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sourceGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // targetGrid
            // 
            this.targetGrid.AllowDrop = true;
            this.targetGrid.AllowUserToAddRows = false;
            this.targetGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.targetGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.DName,
            this.Dtelphone,
            this.Daddress});
            this.targetGrid.Location = new System.Drawing.Point(3, 12);
            this.targetGrid.Name = "targetGrid";
            this.targetGrid.ReadOnly = true;
            this.targetGrid.RowTemplate.Height = 23;
            this.targetGrid.Size = new System.Drawing.Size(337, 492);
            this.targetGrid.TabIndex = 0;
            this.targetGrid.DragOver += new System.Windows.Forms.DragEventHandler(this.targetGrid_DragOver);
            this.targetGrid.DragDrop += new System.Windows.Forms.DragEventHandler(this.targetGrid_DragDrop);
            // 
            // ID
            // 
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Width = 30;
            // 
            // DName
            // 
            this.DName.HeaderText = "科室名称";
            this.DName.Name = "DName";
            this.DName.ReadOnly = true;
            // 
            // Dtelphone
            // 
            this.Dtelphone.HeaderText = "电话";
            this.Dtelphone.Name = "Dtelphone";
            this.Dtelphone.ReadOnly = true;
            this.Dtelphone.Width = 80;
            // 
            // Daddress
            // 
            this.Daddress.HeaderText = "地址";
            this.Daddress.Name = "Daddress";
            this.Daddress.ReadOnly = true;
            this.Daddress.Width = 80;
            // 
            // sourceGrid
            // 
            this.sourceGrid.AllowUserToAddRows = false;
            this.sourceGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sourceGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ToID,
            this.ToDname,
            this.ToDtelphone,
            this.ToDaddress});
            this.sourceGrid.Location = new System.Drawing.Point(381, 12);
            this.sourceGrid.Name = "sourceGrid";
            this.sourceGrid.ReadOnly = true;
            this.sourceGrid.RowTemplate.Height = 23;
            this.sourceGrid.Size = new System.Drawing.Size(337, 492);
            this.sourceGrid.TabIndex = 0;
            this.sourceGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sourceGrid_MouseDown);
            this.sourceGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.sourceGrid_MouseMove);
            // 
            // ToID
            // 
            this.ToID.HeaderText = "ID";
            this.ToID.Name = "ToID";
            this.ToID.ReadOnly = true;
            this.ToID.Width = 30;
            // 
            // ToDname
            // 
            this.ToDname.HeaderText = "科室名称";
            this.ToDname.Name = "ToDname";
            this.ToDname.ReadOnly = true;
            // 
            // ToDtelphone
            // 
            this.ToDtelphone.HeaderText = "电话";
            this.ToDtelphone.Name = "ToDtelphone";
            this.ToDtelphone.ReadOnly = true;
            this.ToDtelphone.Width = 80;
            // 
            // ToDaddress
            // 
            this.ToDaddress.HeaderText = "地址";
            this.ToDaddress.Name = "ToDaddress";
            this.ToDaddress.ReadOnly = true;
            this.ToDaddress.Width = 80;
            // 
            // MoveGridForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 516);
            this.Controls.Add(this.sourceGrid);
            this.Controls.Add(this.targetGrid);
            this.Name = "MoveGridForm";
            this.Text = "MoveGridForm";
            this.Load += new System.EventHandler(this.MoveGridForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.targetGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sourceGrid)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView targetGrid;
		private System.Windows.Forms.DataGridView sourceGrid;
		private System.Windows.Forms.DataGridViewTextBoxColumn ID;
		private System.Windows.Forms.DataGridViewTextBoxColumn DName;
		private System.Windows.Forms.DataGridViewTextBoxColumn Dtelphone;
		private System.Windows.Forms.DataGridViewTextBoxColumn Daddress;
		private System.Windows.Forms.DataGridViewTextBoxColumn ToID;
		private System.Windows.Forms.DataGridViewTextBoxColumn ToDname;
		private System.Windows.Forms.DataGridViewTextBoxColumn ToDtelphone;
		private System.Windows.Forms.DataGridViewTextBoxColumn ToDaddress;
	}
}
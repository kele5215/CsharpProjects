namespace ConvertSimpTrad
{
    partial class Form3
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
            this.ListDragSource = new System.Windows.Forms.ListBox();
            this.ListDragTarget = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // ListDragSource
            // 
            this.ListDragSource.FormattingEnabled = true;
            this.ListDragSource.ItemHeight = 12;
            this.ListDragSource.Items.AddRange(new object[] {
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine",
            "ten"});
            this.ListDragSource.Location = new System.Drawing.Point(32, 12);
            this.ListDragSource.Name = "ListDragSource";
            this.ListDragSource.Size = new System.Drawing.Size(163, 268);
            this.ListDragSource.TabIndex = 0;
            this.ListDragSource.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListDragSource_MouseMove);
            this.ListDragSource.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListDragSource_MouseDown);
            // 
            // ListDragTarget
            // 
            this.ListDragTarget.AllowDrop = true;
            this.ListDragTarget.FormattingEnabled = true;
            this.ListDragTarget.ItemHeight = 12;
            this.ListDragTarget.Location = new System.Drawing.Point(257, 12);
            this.ListDragTarget.Name = "ListDragTarget";
            this.ListDragTarget.Size = new System.Drawing.Size(163, 268);
            this.ListDragTarget.TabIndex = 0;
            this.ListDragTarget.DragOver += new System.Windows.Forms.DragEventHandler(this.ListDragTarget_DragOver);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 316);
            this.Controls.Add(this.ListDragTarget);
            this.Controls.Add(this.ListDragSource);
            this.Name = "Form3";
            this.Text = "Form3";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox ListDragSource;
        private System.Windows.Forms.ListBox ListDragTarget;
    }
}
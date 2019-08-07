using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ConvertSimpTrad
{
	public partial class MoveGridForm : Form
	{
		public MoveGridForm()
		{
			InitializeComponent();
		}

        private DataGridViewSelectedRowCollection sourceRowCollection = null;//用来保存选中的行

		private void MoveGridForm_Load(object sender, EventArgs e)
		{
			DataTable tableTarget = GetDataTable("select * from Department_A");
			DataTable tableSource = GetDataTable("select * from Department_B");
			DataGridBing(tableTarget, targetGrid,false);
			DataGridBing(tableSource, sourceGrid,true);
            //初始化时不要选中任何行
            if (this.sourceGrid.Rows.Count > 0)
            {
                this.sourceGrid.Rows[0].Selected = false;
            }
            if (this.targetGrid.Rows.Count > 0)
            {
                this.targetGrid.Rows[0].Selected = false;
            }
		}

        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="grid"></param>
        /// <param name="flag">为true时绑定源Grid</param>
		private void DataGridBing(DataTable table,DataGridView grid,bool flag)
		{
			if (table.Rows.Count > 0)
			{
				for (int i = 0; i < table.Rows.Count; i++)
				{
					int k = grid.Rows.Add();
					DataGridViewRow row = grid.Rows[k];
					if (flag)
					{
						row.Cells["ToID"].Value = table.Rows[i]["ID"];
						row.Cells["ToDName"].Value = table.Rows[i]["DName"];
						row.Cells["ToDaddress"].Value = table.Rows[i]["Daddress"];
						row.Cells["ToDtelphone"].Value = table.Rows[i]["Dtelphone"];
					}
					else
					{
						row.Cells["ID"].Value = table.Rows[i]["ID"];
						row.Cells["DName"].Value = table.Rows[i]["DName"];
						row.Cells["Daddress"].Value = table.Rows[i]["Daddress"];
						row.Cells["Dtelphone"].Value = table.Rows[i]["Dtelphone"];
					}
				}
			}
		}

		private DataTable GetDataTable(string sql)
		{
			SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=Demo;Integrated Security=True");
			SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
			DataTable table = new DataTable();
			adapter.Fill(table);
			return table;
        }

        private void sourceGrid_MouseDown(object sender, MouseEventArgs e)
        {
            //捕获鼠标点击区域的信息
            DataGridView.HitTestInfo hitTestInfo= this.sourceGrid.HitTest(e.X, e.Y);

            if (e.X < 30 && hitTestInfo.RowIndex > -1)
            {
                if (this.sourceGrid.SelectedRows.Count > 0)
                {
                    sourceRowCollection = this.sourceGrid.SelectedRows;
                }
            }
            else
                sourceRowCollection = null;
        }

        private void targetGrid_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(typeof(DataGridViewSelectedRowCollection)))
                {
                    DataGridViewSelectedRowCollection rowCollection = e.Data.GetData(typeof(DataGridViewSelectedRowCollection)) as DataGridViewSelectedRowCollection;
                    if (rowCollection == null)
                    {
                        return;
                    }
                    //新增行
                    //注意要将鼠标的Point转换到当前工作区域，否则无法得到正确的HitTestInfo
                    Point p = this.targetGrid.PointToClient(new Point(e.X,e.Y));
                    DataGridView.HitTestInfo hitTestInfo = this.targetGrid.HitTest(p.X, p.Y);
                    //如果鼠标所在的位置的RowIndex>-1，则在当前位置接入列，否则就在最末尾新增列
                    if (hitTestInfo.RowIndex > -1)
                    {
                        this.targetGrid.Rows.Insert(hitTestInfo.RowIndex + 1, rowCollection.Count);
                        for (int i = 0; i < rowCollection.Count; i++)
                        {
                            this.targetGrid.Rows[hitTestInfo.RowIndex + i + 1].Cells["ID"].Value = rowCollection[i].Cells["ToID"].Value;
                            this.targetGrid.Rows[hitTestInfo.RowIndex + i + 1].Cells["DName"].Value = rowCollection[i].Cells["ToDName"].Value;
                            this.targetGrid.Rows[hitTestInfo.RowIndex + i + 1].Cells["Daddress"].Value = rowCollection[i].Cells["ToDaddress"].Value;
                            this.targetGrid.Rows[hitTestInfo.RowIndex + i + 1].Cells["Dtelphone"].Value = rowCollection[i].Cells["ToDtelphone"].Value;
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in rowCollection)
                        {
                            int i = this.targetGrid.Rows.Add();
                            this.targetGrid.Rows[i].Cells["ID"].Value = row.Cells["ToID"].Value;
                            this.targetGrid.Rows[i].Cells["DName"].Value = row.Cells["ToDName"].Value;
                            this.targetGrid.Rows[i].Cells["Daddress"].Value = row.Cells["ToDaddress"].Value;
                            this.targetGrid.Rows[i].Cells["Dtelphone"].Value = row.Cells["ToDtelphone"].Value;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private void targetGrid_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(DataGridViewSelectedRowCollection)))
            {

                e.Effect = DragDropEffects.None;
                return;
            }
            else
            {
                e.Effect = DragDropEffects.Move;  //这个值会返回给DoDragDrop方法
            }
        }

        private void sourceGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (sourceRowCollection != null)
                {
                    DragDropEffects effect = this.sourceGrid.DoDragDrop(sourceRowCollection, DragDropEffects.Move);
                    if (effect == DragDropEffects.Move)
                    {
                        //在sourceGrid中移除选中行
                        foreach (DataGridViewRow row in sourceRowCollection)
                        {
                            this.sourceGrid.Rows.Remove(row);
                        }
                        //将sourceRowCollection重新置空
                        sourceRowCollection = null;
                    }
                }
            }
        }
    }
}

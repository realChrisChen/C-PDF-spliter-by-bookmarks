using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Leadtools.Pdf;
using Leadtools;
using Leadtools.Codecs;
using System.Collections;
using Spire.Pdf;
using Spire.Pdf.Bookmarks;
using Spire.Pdf.Graphics;
using Spire.Pdf.General;
using Spire.Pdf.Actions;
using System.Text.RegularExpressions;
using System.IO;
//隶书,15pt
namespace PdfSpliter
{

    public partial class mainwindow : Form
    {
        
        public mainwindow()
        {
            InitializeComponent();
        }
        //分割并输出
        public void splitintopdf(PdfDocument origin,int start, int end,string destination, string name, int zoom,int count,PointF p,int len)
        {
            PdfDocument pdf = new PdfDocument();
            PdfPageBase page;
            if(end == origin.Pages.Count)
            {
                end--;
            }
            if(start > end)
            {
                name = "问题书签_" + name;
                end = start;
            }
            
            for (int s = start; s <= end; s++)
            {
                page = pdf.Pages.Add(origin.Pages[s].Size, new PdfMargins(0));
                origin.Pages[s].CreateTemplate().Draw(page,new PointF(0,0));

            }

            //改编p的y坐标
            float pointHeight = pdf.Pages[0].Size.Height;
            p.Y = pointHeight - p.Y;

            //设置书签指向的页面和位置
            PdfBookmark bookmark1 = pdf.Bookmarks.Add(name);
            bookmark1.Destination = new PdfDestination(pdf.Pages[0]);
            bookmark1.Destination.Zoom = zoom;
            bookmark1.Destination.Location = new PointF(p.X, p.Y);
            //设置打开pdf后的动作
            PdfDestination dest = new PdfDestination(pdf.Pages[0], p);
            PdfGoToAction action = new PdfGoToAction(dest);
            pdf.AfterOpenAction = action;
            //设置书签的文本格式和颜色
            bookmark1.DisplayStyle = PdfTextStyle.Bold;
            bookmark1.Color = Color.SeaGreen;

            //输出     
            pdf.SaveToFile(destination+count.ToString($"d{len}")+"_"+name+".pdf");

            GC.Collect();
            
        }

        //选择pdf文件
        private void choosefile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Title = "请选择要分割的PDF文件";
            dialog.Filter = "PDF文件(*.pdf)|*.pdf;";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)               
            {
                foreach(string file in dialog.FileNames)
                {
                    initializeData book = new initializeData(file);
                    datagrid.Rows.Add(System.IO.Path.GetFileNameWithoutExtension(file), Path.GetDirectoryName(file) + "\\", book.pcount,book.bcount);                    
                }
                GC.Collect();
            }           
        }
       private void mainwindow_Load(object sender, EventArgs e)
        {

        }
        //链接选中的数据
        private void datagrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                datagrid.ClearSelection();
                var hti = datagrid.HitTest(e.X, e.Y);
                if (hti.RowIndex != -1)
                {
                    
                    datagrid.Rows[hti.RowIndex].Selected = true;
                }
            }
        }
        //按钮删除
        private void deletebutton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in datagrid.SelectedRows)
            {
                datagrid.Rows.Remove(row);
            }

        }
        //按钮分割
        private void startbutton_Click(object sender, EventArgs e)
        {
            if (datagrid.SelectedRows.Count != 0)
            {
                foreach (DataGridViewRow row in datagrid.SelectedRows)
                {
                    int index = row.Index;
                    string item = datagrid["basepath", index].Value.ToString() + datagrid["filename", index].Value.ToString() + ".pdf";
                    initializeData book = new initializeData(item);
                    if (book.bcount > 0)
                    {
                        string destination = book.destinationpath(item);
                        int count = 1;
                        PdfDocument origin = new PdfDocument();
                        origin.LoadFromFile(item);
                        int len = book.bcount.ToString().Length;
                        
                        for (int i = 0; i < book.bcount; i++)
                        {
                            if (i == book.bcount - 1)
                            {
                                splitintopdf(origin, book.bookmarksinfo[i].pagenum - 1, origin.Pages.Count - 1, destination, book.bookmarksinfo[i].title, book.bookmarksinfo[i].zoom, count, book.bookmarksinfo[i].coor, len);
                            }
                            else
                            {
                                splitintopdf(origin, book.bookmarksinfo[i].pagenum - 1, book.bookmarksinfo[i + 1].pagenum-1, destination, book.bookmarksinfo[i].title, book.bookmarksinfo[i].zoom, count, book.bookmarksinfo[i].coor, len);
                            }
 
                            count++;
                        }
                        
                        datagrid.Rows.RemoveAt(index);
                    }
                    book = null;
                    GC.Collect();
                }
                MessageBox.Show("转换成功");
            }
        }
        //右键分割
        private void 分割ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagrid.SelectedRows.Count != 0)
            {
                foreach (DataGridViewRow row in datagrid.SelectedRows)
                {
                    int index = row.Index;
                    string item = datagrid["basepath", index].Value.ToString() + datagrid["filename", index].Value.ToString() + ".pdf";
                    initializeData book = new initializeData(item);
                    if (book.bcount > 0)
                    {
                        string destination = book.destinationpath(item);
                        int count = 1;
                        PdfDocument origin = new PdfDocument();
                        origin.LoadFromFile(item);
                        int len = book.bcount.ToString().Length;

                        for (int i = 0; i < book.bcount; i++)
                        {
                            if (i == book.bcount - 1)
                            {
                                splitintopdf(origin, book.bookmarksinfo[i].pagenum - 1, origin.Pages.Count - 1, destination, book.bookmarksinfo[i].title, book.bookmarksinfo[i].zoom, count, book.bookmarksinfo[i].coor, len);
                            }
                            else
                            {
                                splitintopdf(origin, book.bookmarksinfo[i].pagenum - 1, book.bookmarksinfo[i + 1].pagenum - 1, destination, book.bookmarksinfo[i].title, book.bookmarksinfo[i].zoom, count, book.bookmarksinfo[i].coor, len);
                            }

                            count++;
                        }

                        datagrid.Rows.RemoveAt(index);
                    }
                    book = null;
                    GC.Collect();
                }
                MessageBox.Show("转换成功");
            }
        }
        //右键删除
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in datagrid.SelectedRows)
            {
                datagrid.Rows.Remove(row);
            }

        }

        
    }
}

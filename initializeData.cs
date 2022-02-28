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
using System.Collections;
using Spire.Pdf;
using Spire.Pdf.Bookmarks;
using Spire.Pdf.Graphics;
using Spire.Pdf.General;
using System.Text.RegularExpressions;
using System.IO;

namespace PdfSpliter
{
    //类初始化传入一本书书的路径,将一本树中的所有书签信息存入数组中并初始化
    public class initializeData
    {
        //所有书签的数组
        public bookmarkinfo[] bookmarksinfo;
        //书签个数
        public int bcount;

        public int pcount;

        


        public initializeData(string filepath)
        {
            bcount = markcount(filepath);
            pcount = pagecount(filepath);

            if (bcount != 0)
            {
                this.bookmarksinfo = new bookmarkinfo[bcount];
               
                iniBigthree(filepath);
            }
        }
        //获取书签个数
        public int markcount(string filepath)
        {
            PDFDocument document = new PDFDocument(filepath);
            document.ParseDocumentStructure(PDFParseDocumentStructureOptions.Bookmarks);
            return document.Bookmarks.Count;


        }
        //获取页码数
        public int pagecount(string filepath)
        {
            PDFDocument document = new PDFDocument(filepath);
            return document.Pages.Count;


        }
        //初始化标题,页码,放大程序
        public void iniBigthree(string filepath)
        {
            int i = 0;
            PDFDocument document = new PDFDocument(filepath);
            document.ParseDocumentStructure(PDFParseDocumentStructureOptions.Bookmarks);
            string reg = @"[^a-zA-Z0-9\u4e00-\u9fa5\s\t\.]";
            string reg2 = @"[\n\r]";
            foreach (PDFBookmark bookmark in document.Bookmarks)
            {
                bookmarksinfo[i] = new bookmarkinfo();

                Regex rgx = new Regex(reg);
                Regex rgx1 = new Regex(reg2);
                bookmarksinfo[i].title =rgx1.Replace(rgx.Replace(bookmark.Title,""),"");
                bookmarksinfo[i].pagenum = bookmark.TargetPageNumber;
                bookmarksinfo[i].zoom = bookmark.TargetZoomPercent;
                var sw = new StringWriter();
                Console.SetOut(sw);
                Console.SetError(sw);
                Console.WriteLine($"{bookmark.TargetPosition}");
                string result = sw.ToString();
                string reg1 = @"{X=(.*),Y=(.*)}";
                float x=10*float.Parse(Regex.Match(result, reg1).Groups[1].Value);
                float y= 10*float.Parse(Regex.Match(result, reg1).Groups[2].Value);
                bookmarksinfo[i].coor = new PointF(x,y);
                i++;
            }
            
        }
        //创建分割后文件存放路径
        public string destinationpath(string filepath)
        {       
            string path= Path.GetDirectoryName(filepath)+"\\"+ System.IO.Path.GetFileNameWithoutExtension(filepath)+"\\";
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
            
        }
      
    }
}

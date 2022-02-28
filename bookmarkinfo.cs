using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Leadtools.Pdf;
using Leadtools;
using System.Collections;
using Spire.Pdf;
using Spire.Pdf.Bookmarks;
using Spire.Pdf.Graphics;
using Spire.Pdf.General;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
namespace PdfSpliter
{
    //书签信息类
    public class bookmarkinfo
    {
        public string title;
        public int pagenum;
        public int zoom;
        public PointF coor;
    }
}

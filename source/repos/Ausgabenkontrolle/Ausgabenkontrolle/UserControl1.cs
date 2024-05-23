using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ausgabenkontrolle
{
    public partial class UserControl1 : UserControl
    {

        private string userControlText;
        private string[] labels;
        private string[] datas;

        public UserControl1(string[] labels, string[] datas)
        {
            InitializeComponent();
            this.labels = labels;
            this.datas = datas;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Pen linePen = new Pen(Color.Black, 1);
            int columnWidth = this.Width / 2;
            for (int i = 0; i <= labels.Length; i++)
            {
                int y = 0 + i * 20;
                e.Graphics.DrawLine(linePen, 0, y, this.Width, y);
            }
            var font = new Font("Arial", 10);
            for (int i = 0; i < labels.Length; i++)
            {
                int y = 0 + i * 20;
                e.Graphics.DrawString(labels[i], font, Brushes.Black, 10, y);
                e.Graphics.DrawString("|", font, Brushes.Black, columnWidth - 5, y);
                e.Graphics.DrawString(datas[i], font, Brushes.Black, columnWidth + 10, y);
            }
        }
    }
}

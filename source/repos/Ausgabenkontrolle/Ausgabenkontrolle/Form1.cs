using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Ausgabenkontrolle
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        private string theDate;

        private void inputDate(object sender, EventArgs e)
        {
            theDate = dateTimePicker1.Value.ToString("dd.MM.yyyy");
        }

        private void SaveUser(object sender, EventArgs e)
        { 
        string[] labels = { "Vorname: ", "Nachname: ", "Geburtdatum: ", "Anschrift: ", "Handynummer: " };
        string[] datas = { textBox1.Text, textBox2.Text, theDate, textBox3.Text, textBox4.Text };
        
            UserControl1 userControl1 = new UserControl1(labels, datas);
            Form form = new Form();
            userControl1.Dock = DockStyle.Fill;
            form.Controls.Add(userControl1);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
          
        }



        private void DeleteData(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            dateTimePicker1.Value = DateTime.Now;
            textBox3.Clear();
            textBox4.Clear();
        }

      
    }
}

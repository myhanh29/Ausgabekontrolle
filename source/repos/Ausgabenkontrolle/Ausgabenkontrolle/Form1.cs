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
using System.Configuration;
using System.Diagnostics;
using System.IO;
using iTextSharp.text;
using System.Data.SqlClient;

using iTextSharp.text.pdf;
using System.Drawing.Printing;
using System.Xml.Linq;


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
            


            Button button1 = new Button();
            button1.Text = "Save";
            button1.Dock = DockStyle.Bottom;
            form.Controls.Add(button1);
            button1.Click += (s, eventArgs) => { Button1_Click(s, eventArgs, form); };

            Button button2 = new Button();
            button2.Text = "Cancel";
            button2.Dock = DockStyle.Bottom;
            form.Controls.Add(button2);
            button2.Click += Button2_Click; 
            form.ShowDialog();
        }

        private void Button1_Click(object sender, EventArgs e, Form form)
        {
            SaveFileDialog svg = new SaveFileDialog()
            {
                Filter = "PDF files (*.pdf)|*.pdf"
            };
            
               if (svg.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(svg.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    foreach (Control control in form.Controls)
                    {
                        if (control is UserControl1 userControl)
                        {
                            var (labels, datas) = userControl.GetContent();
                            PdfPTable t = new PdfPTable(2);
                            
                            t.DefaultCell.PaddingBottom = 10;
                            for (int i = 0; i < labels.Length; i++)
                            {
                                t.AddCell(new PdfPCell(new Phrase(labels[i])){ Border = PdfPCell.BOX });
                                t.AddCell(new PdfPCell(new Phrase(datas[i])){ Border = PdfPCell.BOX });

                            }
                            pdfDoc.Add(t);
                        }
                    }
                    pdfDoc.Close();
                    stream.Close();
                    Close();

                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Close();
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

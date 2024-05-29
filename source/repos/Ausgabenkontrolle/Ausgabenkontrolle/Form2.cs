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

using System.Diagnostics;
using System.IO;
using iTextSharp.text;
using System.Data.SqlClient;
using System.Configuration;

using iTextSharp.text.pdf;
using System.Drawing.Printing;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Data.Common;
using Excel = Microsoft.Office.Interop.Excel;
namespace Ausgabenkontrolle
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Verbindungszeichenfolge aus der Konfiguration abrufen
            string con_string = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            Form userlist = new Form();
            userlist.Controls.Add(dgv);
            // using-Anweisungen zur korrekten Freigabe der Ressourcen verwenden
            using (SqlConnection sqlConnection = new SqlConnection(con_string))
            {
                string CommandText = "SELECT * FROM UserData";
                using (SqlCommand cmd = new SqlCommand(CommandText, sqlConnection))
                {
                    sqlConnection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        dgv.DataSource = ds.Tables[0];
                    }
                }

            }
            Button button1 = new Button();
            button1.Text = "Save to PDF";
            button1.Dock = DockStyle.Bottom;
            userlist.Controls.Add(button1);
            button1.Click += (s, eventArgs) => { SaveToPDF(s, eventArgs, userlist, dgv); };

            Button button2 = new Button();
            button2.Text = "Save to EXCEL";
            button2.Dock = DockStyle.Bottom;
            userlist.Controls.Add(button2);
            button2.Click += (s, eventArgs) => { SaveToEXCEL(s, eventArgs, userlist, dgv); };
            userlist.ShowDialog();

        }

        private void SaveToPDF(object sender, EventArgs e, Form userlist, DataGridView dgv)
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


                    PdfPTable t = new PdfPTable(dgv.ColumnCount);

                    t.DefaultCell.PaddingBottom = 100;
                    foreach (DataGridViewColumn column in dgv.Columns)
                    {

                        t.AddCell(new PdfPCell(new Phrase(column.HeaderText)));

                    }
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            t.AddCell(new PdfPCell(new Phrase(cell.Value?.ToString() )));
                        }

                    }
                    pdfDoc.Add(t);

                    pdfDoc.Close();
                }
             
            }
            Close();
        }
        private void SaveToEXCEL(object sender, EventArgs e, Form userlist, DataGridView dgv)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {

                Microsoft.Office.Interop.Excel.Application xlexcel;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;
                xlexcel = new Excel.Application();
               
                xlWorkBook = xlexcel.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    xlWorkSheet.Cells[1, i + 1] = dgv.Columns[i].HeaderText;
                }
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    for (int j = 0; j < dgv.Columns.Count; j++)
                    {
                        xlWorkSheet.Cells[2+i, 1+j] = dgv.Rows[i].Cells[j].Value?.ToString();
                    }
                }

                xlWorkBook.SaveAs(sfd.FileName);
                xlWorkBook.Close();
                xlexcel.Quit();


                ReleaseObject(xlWorkSheet);
                ReleaseObject(xlWorkBook);
                ReleaseObject(xlexcel);
                Close();
            }
        }

            private void ReleaseObject(object obj)
            {
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                    obj = null;
                }
                catch (Exception ex)
                {
                    obj = null;
                    MessageBox.Show("Exception occurred while releasing object " + ex.ToString());
                }
                finally
                {
                    GC.Collect();
                }
            }
        }

       
        
    
}

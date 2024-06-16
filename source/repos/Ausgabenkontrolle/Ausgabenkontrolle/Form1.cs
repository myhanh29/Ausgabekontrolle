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
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics.Tracing;
using static System.Net.Mime.MediaTypeNames;

namespace Ausgabenkontrolle
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            panel1.Visible = true;
            panel2.Visible = false;
            panel3.Visible = false;

        }
        private string theDate;
        private DataGridView dgv;
        private int id;

        // Ereignishandler für die Datumsänderung
        private void inputDate(object sender, EventArgs e)
        {
            theDate = dateTimePicker1.Value.ToString("dd.MM.yyyy");
        }

        // Ereignishandler für das Speichern eines Benutzers
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
            button1.Click += (s, eventArgs) => { SaveButonnClick(s, eventArgs, form); };

            Button button2 = new Button();
            button2.Text = "Cancel";
            button2.Dock = DockStyle.Bottom;
            form.Controls.Add(button2);
            button2.Click += CancelButonnClick; 
            form.ShowDialog();
        }
        // Methode zum Speichern der Daten in der Datenbank
       private void SavetoDB(object sender, EventArgs e)
        {
            // Verbindungszeichenfolge aus der Konfiguration abrufen
            string con_string = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            // SQL-Befehl zum Einfügen von Daten in die Tabelle Userdatas
            string myinsert = "INSERT INTO UserData (Vorname, Nachname, Geburtdatum, Anschrift, Handynummer) VALUES (@Vorname, @Nachname, @Geburtdatum, @Anschrift, @Handynummer)";

            // using-Anweisungen zur korrekten Freigabe der Ressourcen verwenden
            using (SqlConnection sqlConnection = new SqlConnection(con_string))
            {
                SqlCommand mycom = new SqlCommand(myinsert, sqlConnection);
                
             
                    mycom.Parameters.AddWithValue("@Vorname", textBox1.Text);
                    mycom.Parameters.AddWithValue("@Nachname", textBox2.Text);
                if (!string.IsNullOrEmpty(theDate))
                {
                    if (DateTime.TryParseExact(theDate, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                    {
                        mycom.Parameters.AddWithValue("@Geburtdatum", parsedDate);
                    }
                    else
                    {
                        MessageBox.Show("Invalid date format. Please enter the date in dd.MM.yyyy format.");
                        return;
                    }
                }
                else
                {
                    mycom.Parameters.AddWithValue("@Geburtdatum", DBNull.Value);
                }


                mycom.Parameters.AddWithValue("@Anschrift", textBox3.Text);
                    mycom.Parameters.AddWithValue("@Handynummer", textBox4.Text);
                    // Verbindung öffnen und SQL-Befehl ausführen
                    sqlConnection.Open();
                    mycom.ExecuteNonQuery();
                
            }
        } 
        // Ereignishandler für den Klick auf den Speichern-Button
        private void SaveButonnClick(object sender, EventArgs e, Form form)
        {
            SavetoDB(sender, e);
            form.Close();
            panel1.Visible = false;
            panel2.Visible = true;
            LoadData();

        }

        // Ereignishandler für den Klick auf den Abbrechen-Button
        private void CancelButonnClick(object sender, EventArgs e)
        {
            Close();
        }
        // Ereignishandler für das Löschen der Daten
        private void DeleteData(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            dateTimePicker1.Value = DateTime.Now;
            textBox3.Clear();
            textBox4.Clear();
        }
        private void SearchinTable(object sender, EventArgs e, DataGridView dgv, TextBox searchText)
        {

            string filter = string.Empty;
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                if (column.Index == 0)
                {
                    filter += string.Format("Vorname LIKE '%{0}%'", searchText.Text);
                }
                else
                {
                    filter += string.Format(" OR Vorname LIKE '%{0}%'", searchText.Text);
                }
            }
            (dgv.DataSource as DataTable).DefaultView.RowFilter = filter;

        }
        public void LoadData()
        {
            panel2.Controls.Clear();
            // Verbindungszeichenfolge aus der Konfiguration abrufen
            string con_string = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            panel2.Controls.Add(dgv);
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
            DataGridViewButtonColumn col = new DataGridViewButtonColumn();
            col.UseColumnTextForButtonValue = true;
            col.Text = "Bearbeiten";

            col.Name = "Option";
            dgv.Columns.Add(col);
            dgv.CellClick += (sender, e) => { dgv_CellClick(sender, e);  }; 
            Panel searchpanel = new Panel();
            searchpanel.Width = dgv.Width/3;
            searchpanel.Height = 30;
            searchpanel.Dock=DockStyle.Top;
            searchpanel.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(searchpanel);

            TextBox searchText = new TextBox();
        
            // Set the BorderStyle property to FixedSingle.
            searchText.BorderStyle = BorderStyle.FixedSingle;
            searchText.Width =  150;
            searchText.Location = new Point(5, 5);
            searchText.Anchor = AnchorStyles.Left;
            searchpanel.Controls.Add(searchText);

            Button buttonsearch = new Button();
            buttonsearch.Size = new Size(60, searchText.ClientSize.Height + 3);
            buttonsearch.Location = new Point(searchText.Width+5, 3);
            buttonsearch.Cursor = Cursors.Default;
            buttonsearch.Text = "Search";
            buttonsearch.Anchor = AnchorStyles.Top;
            buttonsearch.Click += (s, eventArgs) => { SearchinTable(s, eventArgs, dgv, searchText); };
            searchpanel.Controls.Add(buttonsearch);



            Button button5 = new Button();
            button5.Text = "Save to PDF";
            button5.Dock = DockStyle.Bottom;
            panel2.Controls.Add(button5);
            button5.Click += (s, eventArgs) => { SaveToPDF(s, eventArgs, dgv); };

            Button button6 = new Button();
            button6.Text = "Save to EXCEL";
            button6.Dock = DockStyle.Bottom;
            panel2.Controls.Add(button6);
            button6.Click += (s, eventArgs) => { SaveToEXCEL(s, eventArgs, dgv); };
       

        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the click is on a valid row, not a header or an invalid index
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DataGridView dgv = (DataGridView)sender;
            // Check if the clicked cell is in the "Bearbeiten" button column
            if (dgv.Columns[e.ColumnIndex] is DataGridViewButtonColumn && dgv.Columns[e.ColumnIndex].Name == "Option")
            {
                // Retrieve the user ID from the clicked row
                id = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["Id"].Value);

                // Hide panel2 if needed
                panel2.Visible = false;
               dgv.Hide();

                panel3.Visible = true; // Ensure panel3 is made visible
                EditUser(id);
            }
        }



        private void SaveToPDF(object sender, EventArgs e, DataGridView dgv)
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
                    if (pdfDoc != null)
                    {
                        pdfDoc.Open();
                    }
                    else
                    {
                        MessageBox.Show("ne");
                    }


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
                            t.AddCell(new PdfPCell(new Phrase(cell.Value?.ToString())));
                        }

                    }
                    pdfDoc.Add(t);

                    pdfDoc.Close();
                }

            }

        }
        private void SaveToEXCEL(object sender, EventArgs e, DataGridView dgv)
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
                        xlWorkSheet.Cells[2 + i, 1 + j] = dgv.Rows[i].Cells[j].Value?.ToString();
                    }
                }

                xlWorkBook.SaveAs(sfd.FileName);
                xlWorkBook.Close();
                xlexcel.Quit();


                ReleaseObject(xlWorkSheet);
                ReleaseObject(xlWorkBook);
                ReleaseObject(xlexcel);

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
    

        private void EditUser(int id)
        {
            panel3.Controls.Clear();
            // Load user data from database using the given ID
            string con_string = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection sqlConnection = new SqlConnection(con_string))
            {
                string CommandText = "SELECT * FROM UserData WHERE Id=@Id";
                using (SqlCommand cmd = new SqlCommand(CommandText, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    sqlConnection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        panel3.Controls.Add(label6);
                        textBox5.Text = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty;
                        panel3.Controls.Add(textBox5);
                        panel3.Controls.Add(label7);
                        textBox6.Text = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
                        panel3.Controls.Add(textBox6);
                        panel3.Controls.Add(label8);
                        dateTimePicker2.Value = !reader.IsDBNull(3) ? reader.GetDateTime(3) : DateTime.Now;
                        panel3.Controls.Add(dateTimePicker2);
                        panel3.Controls.Add(label9);
                        textBox7.Text = !reader.IsDBNull(4) ? reader.GetString(4) : string.Empty;
                        panel3.Controls.Add(textBox7);
                        panel3.Controls.Add(label10);
                        textBox8.Text = !reader.IsDBNull(5) ? reader.GetString(5) : string.Empty;
                        panel3.Controls.Add(textBox8);
                    }
                }
            }
            Button button3 = new Button();
            button3.Text = "Update";
            button3.Dock = DockStyle.Bottom;
            button3.Size = new System.Drawing.Size(75, 23);
            button3.Click += (s, eventArgs) => { UpdateData(s, eventArgs); };
            panel3.Controls.Add(button3);
            Button button4 = new Button();
            button4.Text = "Cancel";
            button4.Dock = DockStyle.Bottom;
            button4.Size = new System.Drawing.Size(75, 23);
            button4.Click += (s, eventArgs) => { CancelButton(s, eventArgs); };
            panel3.Controls.Add(button4);
            // Ensure the correct panels are visible
            panel3.Visible = true;
            panel2.Visible = false;
        }

        private void UpdateData(object sender, EventArgs e)
        {

            // Verbindungszeichenfolge aus der Konfiguration abrufen
            string con_string = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            // SQL-Befehl zum Einfügen von Daten in die Tabelle Userdatas
            string myinsert = "UPDATE UserData SET Vorname=@Vorname, Nachname=@Nachname, Geburtdatum=@Geburtdatum, Anschrift=@Anschrift, Handynummer=@Handynummer WHERE Id=@Id";

            // using-Anweisungen zur korrekten Freigabe der Ressourcen verwenden
            using (SqlConnection sqlConnection = new SqlConnection(con_string))
            {
                SqlCommand mycom = new SqlCommand(myinsert, sqlConnection);

                mycom.Parameters.AddWithValue("@Id", id);
                mycom.Parameters.AddWithValue("@Vorname", textBox5.Text);
                mycom.Parameters.AddWithValue("@Nachname", textBox6.Text);
                mycom.Parameters.AddWithValue("@Geburtdatum", dateTimePicker2.Value);

                mycom.Parameters.AddWithValue("@Anschrift", textBox7.Text);
                mycom.Parameters.AddWithValue("@Handynummer", textBox8.Text);
                // Verbindung öffnen und SQL-Befehl ausführen
                sqlConnection.Open();
                mycom.ExecuteNonQuery();
                panel3.Visible=false;
                panel2.Visible=true ;
                LoadData(); // Refresh Form2 data
              
          
            }
        }

        private void CancelButton(object sender, EventArgs e)
        {
            Close();
        }

    }

}

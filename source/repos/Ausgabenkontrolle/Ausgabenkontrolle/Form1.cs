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


namespace Ausgabenkontrolle
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        private string theDate;

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
   
            form.Hide();
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();
            this.Close();
            /*  SaveFileDialog svg = new SaveFileDialog()
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
                       stream.Close();*/




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

 
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Ausgabenkontrolle
{
    public partial class Form3 : Form
    {
        private int id;
        private Form2 form2;
        public Form3(int value, Form2 form2Instance)
        {
            InitializeComponent();
            id = value ;
            form2 = form2Instance;
        }
        private void Form3_Load(object sender, EventArgs e)
        {
           
            string con_string = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection sqlConnection = new SqlConnection(con_string))
            {
               string CommandText = "SELECT * FROM UserData WHERE Id=@Id";
                using (SqlCommand cmd = new SqlCommand(CommandText, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    sqlConnection.Open();

                   SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        
                        textBox1.Text = !reader.IsDBNull(1) ? reader.GetString(1) : string.Empty;
                        textBox2.Text = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
                        dateTimePicker1.Value = !reader.IsDBNull(3) ? reader.GetDateTime(3) : DateTime.Now;
                        textBox3.Text = !reader.IsDBNull(2) ? reader.GetString(4) : string.Empty;
                        textBox4.Text = !reader.IsDBNull(2) ? reader.GetString(5) : string.Empty;



                    }
                }
            }
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
                mycom.Parameters.AddWithValue("@Vorname", textBox1.Text);
                mycom.Parameters.AddWithValue("@Nachname", textBox2.Text);
                mycom.Parameters.AddWithValue("@Geburtdatum", dateTimePicker1.Value);

                mycom.Parameters.AddWithValue("@Anschrift", textBox3.Text);
                mycom.Parameters.AddWithValue("@Handynummer", textBox4.Text);
                // Verbindung öffnen und SQL-Befehl ausführen
                sqlConnection.Open();
                mycom.ExecuteNonQuery();
                  form2.LoadData(); // Refresh Form2 data
            form2.Show();
            this.Close();
            }
        }

        private void CancelButton(object sender, EventArgs e)
        {
            Close();
        }
    }
}

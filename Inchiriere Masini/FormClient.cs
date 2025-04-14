using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices; // Required for drag functionality
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Inchiriere_Masini
{
    public partial class FormClient : Form
    {
        int total = 0;
        string produseTotal = string.Empty;

        // Import necessary WinAPI functions for dragging
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public FormClient()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None; // Set the form to borderless
        }

        private void FormClient_MouseDown(object sender, MouseEventArgs e)
        {
            // Enable dragging of the borderless form
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void IncarcaComboBox()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\BD_CarRental.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string comanda = "SELECT DISTINCT Categorie FROM Masini";
            string comanda2 = "SELECT DISTINCT Marca FROM Masini";

            SqlCommand sqlCommand = new SqlCommand(comanda, sqlConnection); //comanda pentru filtrare categorie

            SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                comboBox1.Items.Add(reader[0].ToString());
            }
            comboBox1.SelectedIndex = 0;

            reader.Close(); //inchidem prima conexiune pentru functionarea programului

            SqlCommand sqlCommand1 = new SqlCommand(comanda2, sqlConnection); //comanda pentru filtrare marca

            SqlDataReader reader2 = sqlCommand1.ExecuteReader();

            while (reader2.Read())
            {
                comboBox2.Items.Add(reader2[0].ToString());
            }
            comboBox2.SelectedIndex = 0;

            sqlConnection.Close();
        }

        private void FormClient_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'bD_CarRentalDataSet.Masini' table. You can move, or remove it, as needed.
            this.masiniTableAdapter.Fill(this.bD_CarRentalDataSet.Masini);
            IncarcaComboBox();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int coloana = e.ColumnIndex;
            int rand = e.RowIndex;

            DataGridViewRow randSelectat = new DataGridViewRow();
            randSelectat = dataGridView1.Rows[rand];

            if (dataGridView1.Columns[coloana].Index == 5)
            {
                string prestSting = randSelectat.Cells[3].Value.ToString();
                int pret = Convert.ToInt32(prestSting);
                total = total + pret;
                label4.Text = total.ToString() + "  " + "Euro/saptamana";

                produseTotal += "\n" + randSelectat.Cells[0].Value.ToString() + " " + randSelectat.Cells[1].Value.ToString();
                label6.Text = produseTotal.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\BD_CarRental.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string comanda = "SELECT * FROM Masini WHERE Categorie='" + comboBox1.Text + "'";

            SqlCommand sqlCommand = new SqlCommand(comanda, sqlConnection); //comanda pentru filtrare marca
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);

            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;

            sqlConnection.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\BD_CarRental.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string comanda = "SELECT * FROM Masini WHERE Marca='" + comboBox2.Text + "' AND Categorie ='" + comboBox1.Text + "'";

            SqlCommand sqlCommand = new SqlCommand(comanda, sqlConnection); //comanda pentru filtrare categorie
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);

            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            sqlConnection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\BD_CarRental.mdf;Integrated Security=True;Connect Timeout=30";
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string comanda = String.Format("INSERT INTO Comenzi (Produse) VALUES ('{0}')", label6.Text);

                    using (SqlCommand sqlCommand = new SqlCommand(comanda, sqlConnection))
                    {
                        int result = sqlCommand.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show("Comanda efectuata cu succes!");

                            label6.Text = string.Empty;
                            label4.Text = "0";
                            total = 0;
                            produseTotal = string.Empty;
                        }
                        else
                        {
                            MessageBox.Show("Inserarea comenzii a esuat.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("A aparut o eroare: " + ex.Message);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
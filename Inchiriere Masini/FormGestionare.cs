using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices; // Required for drag functionality
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inchiriere_Masini
{
    public partial class FormGestionare : Form
    {
        // Import necessary WinAPI functions for dragging
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public FormGestionare()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None; // Set the form to borderless
        }

        private void FormGestionare_Load(object sender, EventArgs e)
        {

        }

        private void FormGestionare_Load_1(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'bD_CarRentalDataSet.Masini' table. You can move, or remove it, as needed.
            this.masiniTableAdapter.Fill(this.bD_CarRentalDataSet.Masini);
            comboBox1.Items.Clear();
            incarcaCoduri();
            incarcaComenzi();
        }

        // Enable dragging of the borderless form
        private void FormGestionare_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void incarcaCoduri()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\BD_CarRental.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string comanda = "SELECT DISTINCT idMasina FROM Masini";

            SqlCommand sqlCommand = new SqlCommand(comanda, sqlConnection); //comanda pentru filtrare categorie

            SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                comboBox1.Items.Add(reader[0].ToString());
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }

            reader.Close(); //inchidem prima conexiune pentru functionarea programului

            sqlConnection.Close();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            incarcaCoduri();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\BD_CarRental.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string comanda = "SELECT * FROM Masini WHERE idMasina='" + comboBox1.Text + "'";


            SqlCommand sqlCommand = new SqlCommand(comanda, sqlConnection);

            SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                textBox1.Text = reader[1].ToString();
                textBox2.Text = reader[2].ToString();
                textBox3.Text = reader[3].ToString();
                textBox4.Text = reader[4].ToString();
                MemoryStream ms = new MemoryStream(reader[5] as byte[]);
                pictureBox2.Image = Image.FromStream(ms, true);
            }

            reader.Close();

            sqlConnection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\BD_CarRental.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string comanda = "UPDATE Masini SET Marca=@Marca, Model=@Model, Categorie=@Categorie, Pret=@Pret, Imagine=@Imagine WHERE idMasina=@idMasina";
            SqlCommand sqlCommand = new SqlCommand(comanda, sqlConnection);

            sqlCommand.Parameters.Add("@Marca", SqlDbType.NVarChar, 50);
            sqlCommand.Parameters.Add("@idMasina", SqlDbType.NVarChar, 50);
            sqlCommand.Parameters.Add("@Model", SqlDbType.NVarChar, 50);
            sqlCommand.Parameters.Add("@Categorie", SqlDbType.NVarChar, 50);
            sqlCommand.Parameters.Add("@Pret", SqlDbType.Int);
            sqlCommand.Parameters.Add("@Imagine", SqlDbType.Image);


            sqlCommand.Parameters["@Marca"].Value = textBox1.Text.ToString();
            sqlCommand.Parameters["@Model"].Value = textBox2.Text.ToString();
            sqlCommand.Parameters["@Categorie"].Value = textBox3.Text.ToString();
            sqlCommand.Parameters["@Pret"].Value = Convert.ToInt32(textBox4.Text.ToString());
            sqlCommand.Parameters["@idMasina"].Value = comboBox1.Text.ToString();


            MemoryStream memoryStream = new MemoryStream();
            pictureBox2.Image.Save(memoryStream, pictureBox2.Image.RawFormat);

            Byte[] img = memoryStream.ToArray();
            sqlCommand.Parameters["@Imagine"].Value = img;

            sqlCommand.ExecuteNonQuery();
            MessageBox.Show("Informatii actualizate cu succes");

            sqlConnection.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\BD_CarRental.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string comanda = "INSERT INTO Masini VALUES (@Marca, @Model, @Categorie, @Pret, @Imagine)";

            SqlCommand sqlCommand = new SqlCommand(comanda, sqlConnection); //comanda pentru filtrare marca 


            sqlCommand.Parameters.Add("@Marca", SqlDbType.NVarChar, 50);
            sqlCommand.Parameters.Add("@Model", SqlDbType.NVarChar, 50);
            sqlCommand.Parameters.Add("@Categorie", SqlDbType.NVarChar, 50);
            sqlCommand.Parameters.Add("@Pret", SqlDbType.Int);
            sqlCommand.Parameters.Add("@Imagine", SqlDbType.Image);


            sqlCommand.Parameters["@Marca"].Value = textBox8.Text.ToString();
            sqlCommand.Parameters["@Model"].Value = textBox7.Text.ToString();
            sqlCommand.Parameters["@Categorie"].Value = textBox6.Text.ToString();
            sqlCommand.Parameters["@Pret"].Value = Convert.ToInt32(textBox5.Text.ToString());


            MemoryStream memoryStream = new MemoryStream();
            pictureBox1.Image.Save(memoryStream, pictureBox1.Image.RawFormat);

            Byte[] img = memoryStream.ToArray();
            sqlCommand.Parameters["@Imagine"].Value = img;

            sqlCommand.ExecuteNonQuery();

            MessageBox.Show("Produse adaugate cu succes!");

            textBox8.Text = string.Empty;
            textBox7.Text = string.Empty;
            textBox6.Text = string.Empty;
            textBox5.Text = string.Empty;

            comboBox1.Items.Clear();
            incarcaCoduri();


            sqlConnection.Close();
        }

        private void incarcaComenzi()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\BD_CarRental.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string comanda = "SELECT nrComanda FROM Comenzi";

            SqlCommand sqlCommand = new SqlCommand(comanda, sqlConnection); //comanda pentru filtrare categorie

            SqlDataReader reader = sqlCommand.ExecuteReader();

            comboBox2.Items.Clear();

            while (reader.Read())
            {
                comboBox2.Items.Add(reader[0].ToString());
            }

            if (comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0;
            }

            sqlConnection.Close();
        }


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\BD_CarRental.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string comanda = "SELECT * FROM Comenzi WHERE nrComanda='" + comboBox2.Text + "'";


            SqlCommand sqlCommand = new SqlCommand(comanda, sqlConnection); //comanda pentru filtrare categorie

            SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                label13.Text = reader[1].ToString();
            }

            reader.Close(); //inchidem prima conexiune pentru functionarea programului

            sqlConnection.Close();

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {
            incarcaComenzi();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox2.Items.Count > 0)
            {
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\BD_CarRental.mdf;Integrated Security=True;Connect Timeout=30";
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                string comanda = "DELETE FROM Comenzi WHERE nrComanda='" + comboBox2.Text + "'";

                SqlCommand sqlCommand = new SqlCommand(comanda, sqlConnection); //comanda pentru filtrare categorie

                sqlCommand.ExecuteNonQuery();
                MessageBox.Show("Comanda onorată cu succes!");

                incarcaComenzi();

                sqlConnection.Close();
            }
            else
            {
                MessageBox.Show("Nu există comenzi disponibile pentru a fi completate.");
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image imagineAleasa = Image.FromFile(openFileDialog1.FileName);
                pictureBox1.Image = imagineAleasa;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                Image imagineAleasa = Image.FromFile(openFileDialog2.FileName);
                pictureBox2.Image = imagineAleasa;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.masiniTableAdapter.Fill(this.bD_CarRentalDataSet.Masini);
            comboBox1.Items.Clear();
            incarcaCoduri();
            incarcaComenzi();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.masiniTableAdapter.Fill(this.bD_CarRentalDataSet.Masini);
            comboBox1.Items.Clear();
            incarcaCoduri();
            incarcaComenzi();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
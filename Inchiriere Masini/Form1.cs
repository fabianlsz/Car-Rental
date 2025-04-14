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

namespace Inchiriere_Masini
{
    public partial class Form1 : Form
    {
        // Import necessary WinAPI functions for dragging
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None; // Set the form to borderless
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            // Enable dragging of the borderless form
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Check if the textboxes are empty
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Introduceti numele de utilizator si parola!");
                return; // Exit the method to prevent further execution
            }

            int ok = 0;
            int tip = 0;
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\BD_CarRental.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string comanda = "SELECT * FROM Utilizatori";
            SqlCommand sqlCommand = new SqlCommand(comanda, sqlConnection);

            SqlDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                if (reader[1].ToString() == textBox1.Text && reader[2].ToString() == textBox2.Text)
                {
                    ok = 1;
                    tip = Convert.ToInt32(reader[3].ToString());
                }
            }

            if (ok == 1)
            {
                this.Hide();
                if (tip == 1)
                {
                    FormGestionare formGestionare = new FormGestionare();
                    formGestionare.ShowDialog();
                }
                else if (tip == 2)
                {
                    FormClient formClient = new FormClient();
                    formClient.ShowDialog();
                }
                this.Show();
            }
            else
            {
                MessageBox.Show("Date incorecte!");
            }

            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            sqlConnection.Close();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
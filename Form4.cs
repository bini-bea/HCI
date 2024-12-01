using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace login
{
    public partial class Form4 : Form
    {
        private string connectionString = "server=localhost;database=user_db;username=root;password=;";
        private decimal initialBalance = 0;

        private MyAppData userData = new MyAppData(); // Create an instance

        public Form4()
        {
            InitializeComponent();
            LoadDashboardData(); // Load the data when form initializes
        }

        private void btntransact_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
            this.Hide();
        }

        private void LoadDashboardData()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT SUM(CASE WHEN transaction_type = 'Income' THEN amount ELSE 0 END) AS TotalIncome, " +
                                         "SUM(CASE WHEN transaction_type = 'Expense' THEN amount ELSE 0 END) AS TotalExpenses " +
                                         "FROM transactions WHERE user_id = @userId AND transaction_type IN ('Income', 'Expense')";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userData.UserID);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userData.TotalIncome = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                                userData.TotalExpenses = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
                                userData.TotalWallet = userData.TotalIncome - userData.TotalExpenses;

                                // Set the text of the labels directly
                                lblincome.Text = "₱" + userData.TotalIncome.ToString("N2");
                                lblexpense.Text = "₱" + userData.TotalExpenses.ToString("N2");
                                lblwallet.Text = "₱" + userData.TotalWallet.ToString("N2");
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Database error: " + ex.Message);
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            // ...
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            // ...
        }

        public class MyAppData
        {
            public string UserName { get; set; }
            public string UserEmail { get; set; }
            public int UserID { get; set; }
            public decimal TotalIncome { get; set; }
            public decimal TotalExpenses { get; set; }
            public decimal TotalWallet { get; set; }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // Clear user data on logout
            userData.UserName = "";
            userData.UserEmail = "";
            userData.UserID = 0;

            // Navigate to login screen
            this.Hide();
            Form2 loginForm = new Form2();
            loginForm.Show();
        }
    }
}
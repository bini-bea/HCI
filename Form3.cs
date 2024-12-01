using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows.Forms;
using System;
using static login.Form4;

namespace login
{
    public partial class Form3 : Form
    {
        private string connectionString = "server=localhost;database=user_db;username=root;password=;";
        private decimal initialBalance = 0;
        private MyAppData userData = new MyAppData(); // Define userData as a class-level variable

        // Categories for Income and Expense
        private Dictionary<string, List<string>> categories = new Dictionary<string, List<string>>()
        {
            { "Income", new List<string> { "Salary", "Freelance", "Investments" } },
            { "Expense", new List<string> { "Rent", "Groceries", "Utilities" } }
        };

        public Form3()
        {
            InitializeComponent();
            LoadInitialBalance(); // Load the initial balance when the form initializes  
            InitializeTransactionComboBoxes(); // Initialize ComboBox behavior
        }

        private void InitializeTransactionComboBoxes()
        {
            // Populate cmbtransaction with Income and Expense options
            cmbtransaction.Items.Add("Income");
            cmbtransaction.Items.Add("Expense");

            // Set default selection
            cmbtransaction.SelectedIndex = 0;

            // Handle the SelectedIndexChanged event for cmbtransaction
            cmbtransaction.SelectedIndexChanged += cmbtransaction_SelectedIndexChanged;

            // Initialize cmbcategory with Income categories by default
            UpdateCategoryComboBox("Income");
        }

        private void cmbtransaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected transaction type
            string selectedType = cmbtransaction.SelectedItem.ToString();

            // Update categories based on the selected transaction type
            UpdateCategoryComboBox(selectedType);
        }

        private void UpdateCategoryComboBox(string transactionType)
        {
            // Clear existing items in cmbcategory
            cmbcategory.Items.Clear();

            // Add the appropriate categories to cmbcategory
            if (categories.ContainsKey(transactionType))
            {
                cmbcategory.Items.AddRange(categories[transactionType].ToArray());
            }

            // Optionally set the first category as the default selection
            if (cmbcategory.Items.Count > 0)
            {
                cmbcategory.SelectedIndex = 0;
            }
        }

        private void LoadInitialBalance()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT initial_balance FROM Wallet LIMIT 1"; // Adjust as needed  
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();

                        // Check if result is DBNull or null  
                        if (result != null && result != DBNull.Value)
                        {
                            initialBalance = Convert.ToDecimal(result);
                        }
                        else
                        {
                            // Set a default initial balance if none exists  
                            initialBalance = 0; // or any other default amount  
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    
                }
            }

            // Display the initial balance in a TextBox  
            txtbalance.Text = "₱" + initialBalance.ToString("N2");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtamount.Text) || cmbtransaction.SelectedItem == null || cmbcategory.SelectedItem == null)
            {
                MessageBox.Show("Please enter a valid amount, select a transaction type, and choose a category.");
                return;
            }

            decimal amount;
            if (!decimal.TryParse(txtamount.Text, out amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid amount greater than 0.");
                return;
            }

            string category = cmbcategory.SelectedItem.ToString().Trim();
            string transactionType = cmbtransaction.SelectedItem.ToString().Trim();

            if (transactionType.Equals("Expense", StringComparison.OrdinalIgnoreCase))
            {
                initialBalance -= amount; // Subtract for expenses
            }
            else if (transactionType.Equals("Income", StringComparison.OrdinalIgnoreCase))
            {
                initialBalance += amount; // Add for income
            }
            else
            {
                MessageBox.Show("Invalid transaction type or category selected.");
                return;
            }

            // Update the displayed balance  
            txtbalance.Text = "₱" + initialBalance.ToString("N2");

            // Insert the transaction into the database  
            InsertTransaction(amount, category, transactionType, dateTimePicker1.Value, userData);
        }

        private void InsertTransaction(decimal amount, string category, string transactionType, DateTime transactionDate, MyAppData userData)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string insertQuery = "INSERT INTO transactions (amount, category, transaction_type, transaction_date) VALUES (@amount, @category, @transactionType, @transactionDate)";
                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                    {
                        // Add parameters
                        cmd.Parameters.AddWithValue("@amount", amount);
                        cmd.Parameters.AddWithValue("@category", category);
                        cmd.Parameters.AddWithValue("@transactionType", transactionType);
                        cmd.Parameters.AddWithValue("@transactionDate", transactionDate);

                        // Execute the command
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Transaction added successfully!");

                            // Update the userData object with the new transaction data
                            userData.TotalIncome += (transactionType == "Income") ? amount : 0;
                            userData.TotalExpenses += (transactionType == "Expense") ? amount : 0;
                            userData.TotalWallet = userData.TotalIncome - userData.TotalExpenses;

                            Form4 form4 = new Form4();
                            form4.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Error adding transaction.");
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Database error: " + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}

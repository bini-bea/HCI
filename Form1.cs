﻿using System;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace login
{
    public partial class Form1 : Form
    {
        private string connectionString = "server=localhost;database=user_db;username=root;password=;";
        public Form1()
        { 
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

       
        

        private void ckbshow_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbshow.Checked)
            {
                txtpass.PasswordChar = '\0';
            }
            else
            {
                txtpass.PasswordChar = '*';
            }
        }

        private void btnback_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();
        }

        private void txtname_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtname.Text))
            {   
                txtname.Focus();
                errorProvider1.SetError(txtname, "Name is required");
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtname, "");
            }
        }

        private void txtemail_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtemail.Text))
            {
                txtname.Focus();
                errorProvider1.SetError(txtemail, "email is required");
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtemail, "");
            }
        }

        private void txtusername_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtusername.Text))
            {
                txtname.Focus();
                errorProvider1.SetError(txtusername, "password is required");
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtusername, "");
            }
        }

        private void txtpass_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtpass.Text))
            {
                txtname.Focus();
                errorProvider1.SetError(txtpass, "username is required");
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtpass, "");
            }
        }
        private void btnreg_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren(ValidationConstraints.Enabled))
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }


            // Proceed with registration
            string username = txtusername.Text;
            string email = txtemail.Text;
            string password = txtpass.Text;
            string name = txtname.Text;

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM users WHERE username = @username", connection);
            command.Parameters.AddWithValue("@username", username);

            int count = Convert.ToInt32(command.ExecuteScalar());

            if (count > 0)
            {
                MessageBox.Show("Username already exists. Please choose a different username.");
                return;
            }

            command = new MySqlCommand("INSERT INTO users (username, email, password, name) VALUES (@username, @email, @password, @name)", connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@name", name);

            command.ExecuteNonQuery();

            MessageBox.Show("Signup successful!");
            // Open Form1 or other form
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();


            connection.Close();
        }

    }

}


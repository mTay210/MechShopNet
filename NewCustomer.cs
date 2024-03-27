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

namespace MechShop
{
    public partial class NewCustomer : Form
    {
        // Connect to the server and database
        private const string connectionString = "Server=localhost\\SQLEXPRESS;Database=MechanicShop;Integrated Security=SSPI;";

        public NewCustomer()
        {
            InitializeComponent();
        }

        // Method to add a new customer to the database
        private void button1_Click(object sender, EventArgs e)
        {
            string firstName = textBox1.Text;
            string lastName = textBox2.Text;
            string phoneNumber = textBox3.Text;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(phoneNumber))
            {
                MessageBox.Show("Please enter all fields.");
                return;
            }

            // SQL query to insert data into the Customer table
            string query = "INSERT INTO Customer (Cust_FN, Cust_LN, Phone) VALUES (@FirstName, @LastName, @PhoneNumber)";

            // Create a SqlConnection object using the connection string
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create a SqlCommand object with the query and connection
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                    try
                    {
                        // Open the connection
                        connection.Open();
                        // Execute the command
                        int rowsAffected = command.ExecuteNonQuery();
                        // Check if any rows were affected
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Customer added successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Failed to add customer.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }

            // Clear the form boxes
            this.Close();

            // Open the Home Page form
            Form1 form1 = new Form1();
            form1.Show();
        }
    }
}

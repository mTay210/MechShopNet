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

namespace MechShop
{
    public partial class AddTech : Form
    {
        // Connect to the server and database
        private const string connectionString = "Server=localhost\\SQLEXPRESS;Database=MechanicShop;Integrated Security=SSPI;";

        public AddTech()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string firstName = textBox1.Text.Trim();
            string lastName = textBox2.Text.Trim();

            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string insertQuery = "INSERT INTO Technician (Tech_FN, Tech_LN) VALUES (@FirstName, @LastName)";

                        using (SqlCommand command = new SqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@FirstName", firstName);
                            command.Parameters.AddWithValue("@LastName", lastName);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Technician added successfully!");
                                // Optionally, clear the text boxes after successful addition
                                textBox1.Text = string.Empty;
                                textBox2.Text = string.Empty;
                            }
                            else
                            {
                                MessageBox.Show("Error adding technician.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding technician: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please enter both first name and last name.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TechServices techServices = new TechServices();
            techServices.Show();

            // Hide the current form
            this.Hide();
        }
    }
}

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
    public partial class AddService : Form
    {
        // Connect to the server and database
        private const string connectionString = "Server=localhost\\SQLEXPRESS;Database=MechanicShop;Integrated Security=SSPI;";

        public AddService()
        {
            InitializeComponent();
        }

        // Method to add a new service to the database
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the service name and cost from the textboxes
                string serviceName = textBox1.Text.Trim();
                decimal serviceCost;

                if (decimal.TryParse(textBox2.Text, out serviceCost))
                {
                    // Check if the service name and cost are valid
                    if (!string.IsNullOrEmpty(serviceName) && serviceCost > 0)
                    {
                        // SQL query to insert a new service into the Services table
                        string insertQuery = "INSERT INTO Services (Service, Cost) VALUES (@ServiceName, @ServiceCost)";

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            using (SqlCommand command = new SqlCommand(insertQuery, connection))
                            {
                                // Add parameters for the service name and cost
                                command.Parameters.AddWithValue("@ServiceName", serviceName);
                                command.Parameters.AddWithValue("@ServiceCost", serviceCost);

                                // Execute the query
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Service added successfully!");
                                }
                                else
                                {
                                    MessageBox.Show("Error adding service.");
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid service name and cost.");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid numeric value for service cost.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

    }
}

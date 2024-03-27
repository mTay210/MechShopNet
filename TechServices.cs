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
    public partial class TechServices : Form
    {
        private const string connectionString = "Server=localhost\\SQLEXPRESS;Database=MechanicShop;Integrated Security=SSPI;";

        public TechServices()
        {
            InitializeComponent();
            InitializeComboBoxes();
        }

        private void InitializeComboBoxes()
        {
            // No need to reinitialize comboBox3 and comboBox4 as they are already initialized in TechServices.designer.cs
            PopulateTechniciansComboBox();
            PopulateServicesComboBox();
        }

        private void PopulateTechniciansComboBox()
        {
            comboBox2.Items.Clear();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Tech_ID, CONCAT(Tech_FN, ' ', Tech_LN) AS FullName FROM Technician";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int techID = reader.GetInt32(0);
                        string fullName = reader.GetString(1);
                        ComboBoxItem item = new ComboBoxItem(fullName, techID);
                        comboBox2.Items.Add(item);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching technicians: " + ex.Message);
            }
        }

        private void PopulateServicesComboBox()
        {
            comboBox1.Items.Clear();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Service_ID, Service FROM Services";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int serviceID = reader.GetInt32(0);
                        string serviceName = reader.GetString(1);
                        ComboBoxItem item = new ComboBoxItem(serviceName, serviceID);
                        comboBox1.Items.Add(item);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching services: " + ex.Message);
            }
        }

        // Custom class to hold ComboBox item data
        public class ComboBoxItem
        {
            public string Text { get; set; }
            public int Value { get; set; }

            public ComboBoxItem(string text, int value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null && comboBox1.SelectedItem != null)
            {
                ComboBoxItem selectedTechnician = (ComboBoxItem)comboBox2.SelectedItem;
                ComboBoxItem selectedService = (ComboBoxItem)comboBox1.SelectedItem;

                int technicianID = selectedTechnician.Value;
                int serviceID = selectedService.Value;

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string insertQuery = "INSERT INTO Tech_to_Services (Tech_ID, Service_ID) VALUES (@TechID, @ServiceID)";

                        SqlCommand command = new SqlCommand(insertQuery, connection);
                        command.Parameters.AddWithValue("@TechID", technicianID);
                        command.Parameters.AddWithValue("@ServiceID", serviceID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Assignment successful.");
                        }
                        else
                        {
                            MessageBox.Show("Assignment failed.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error assigning technician to service: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select both a technician and a service.");
            }
        }
    }
}

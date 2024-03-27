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
    public partial class Form3 : Form
    {
        // Connections to the server and database
        private const string connectionString = "Server=localhost\\SQLEXPRESS;Database=MechanicShop;Integrated Security=SSPI;";

        // Constructor
        public Form3()
        {
            InitializeComponent();
            PopulateModelComboBox();
            PopulateMakeComboBox(); ;

            // Set the DateTimePicker format to Custom to display both date and time
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            // Set the CustomFormat to a format that includes both date and time
            dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm";
        }

        // Populates the Model combobox based on the selected Make
        private void PopulateModelComboBox()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ModelID, Model FROM Model";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    comboBox1.DisplayMember = "Model";
                    comboBox1.ValueMember = "ModelID";
                    comboBox1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Method to populate the Make combobox
        private void PopulateMakeComboBox()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT MakeID, Make FROM Make";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    comboBox2.DisplayMember = "Make";
                    comboBox2.ValueMember = "MakeID";
                    comboBox2.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        // Method to populate the cars in comboBox3 displays license plate number
        private void PopulateCarsComboBox(int custID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to retrieve customer's cars with make, model, and license plate details
                    string query = @"
                            SELECT Car.Car_ID, Make.Make, Model.Model, Car.LicensePlate 
                            FROM Car 
                            INNER JOIN Make ON Car.MakeID = Make.MakeID 
                            INNER JOIN Model ON Car.ModelID = Model.ModelID 
                            INNER JOIN CarOwner ON Car.Car_ID = CarOwner.Car_ID 
                            WHERE CarOwner.Cust_ID = @CustID";

                    // Create a SqlCommand object for the query
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameter for customer ID
                        command.Parameters.AddWithValue("@CustID", custID);

                        // Execute the query and populate a DataTable
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Set the DisplayMember and ValueMember for comboBox3
                        comboBox3.DisplayMember = "DisplayText";
                        comboBox3.ValueMember = "Car_ID";

                        // Create a new DataColumn for the display text (Make, Model, LicensePlate)
                        dt.Columns.Add("DisplayText", typeof(string), "Make + ' ' + Model + ' - ' + LicensePlate");

                        // Bind the DataTable to comboBox3
                        comboBox3.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while populating the cars in comboBox3: " + ex.Message);
            }
        }


        // Method to get Cust_ID from phone number
        private int GetCustomerID(string phoneNumber)
        {
            string query = "SELECT Cust_ID FROM Customer WHERE Phone = @PhoneNumber";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    connection.Close();

                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        // Method to get the Cust_Car_ID based on the selected car in comboBox3
        private int GetCustCarID(int carID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to get the Cust_Car_ID based on the selected car
                    string query = "SELECT Cust_Car_ID FROM CarOwner WHERE Car_ID = @CarID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CarID", carID);

                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Cust_Car_ID: " + ex.Message);
            }

            return 0;
        }

        // Adds a new car to the database and links it to the customer
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedMakeID = Convert.ToInt32(comboBox2.SelectedValue);
                int selectedModelID = Convert.ToInt32(comboBox1.SelectedValue);
                string licensePlate = textBox1.Text;

                // SQL query to insert data into the Car table and retrieve the Car_ID
                string carQuery = "INSERT INTO Car (MakeID, ModelID, LicensePlate) VALUES (@MakeID, @ModelID, @LicensePlate); SELECT SCOPE_IDENTITY();";

                int carID;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create a SqlCommand object with the query and connection
                    using (SqlCommand command = new SqlCommand(carQuery, connection))
                    {
                        command.Parameters.AddWithValue("@MakeID", selectedMakeID);
                        command.Parameters.AddWithValue("@ModelID", selectedModelID);
                        command.Parameters.AddWithValue("@LicensePlate", licensePlate);

                        // ExecuteScalar to get the newly inserted Car_ID
                        carID = Convert.ToInt32(command.ExecuteScalar());
                    }
                }

                // Check if Car_ID was retrieved successfully
                if (carID > 0)
                {
                    // Get the Cust_ID from the searched customer
                    int custID = GetCustomerID(textBox4.Text);

                    if (custID > 0)
                    {
                        // Insert into CarOwner table
                        string ownerQuery = "INSERT INTO CarOwner (Cust_ID, Car_ID) VALUES (@CustID, @CarID)";

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            using (SqlCommand command = new SqlCommand(ownerQuery, connection))
                            {
                                command.Parameters.AddWithValue("@CustID", custID);
                                command.Parameters.AddWithValue("@CarID", carID);

                                // ExecuteNonQuery to insert into CarOwner table
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Car added successfully and linked to the customer!");
                                }
                                else
                                {
                                    MessageBox.Show("Error linking car to the customer.");
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Customer not found.");
                    }
                }
                else
                {
                    MessageBox.Show("Error adding the car.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Method to submit the appointment and link it to both Cust_Car_Service_Date_Time and Car_Service_Date_Services
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the customer information and selected car are available
                if (!string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrEmpty(textBox3.Text) && comboBox3.SelectedValue != null)
                {
                    // Get the selected car ID from comboBox3
                    int selectedCarID = Convert.ToInt32(comboBox3.SelectedValue);

                    // Create an instance of the Appointment form
                    Appointment appointmentForm = new Appointment();

                    // Pass the necessary data to the appointmentForm instance
                    appointmentForm.CustomerFirstName = textBox2.Text;
                    appointmentForm.CustomerLastName = textBox3.Text;
                    appointmentForm.CustCarID = selectedCarID; // Pass the selected car ID

                    // Show the appointment form
                    appointmentForm.Show();
                }
                else
                {
                    MessageBox.Show("Error: Customer information or selected car is missing.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        // Method to search for a customer based on phone number and display their service history and populate comboBox3 with their cars
        private void button3_Click(object sender, EventArgs e)
        {
            string phoneNumber = textBox4.Text;

            // Check if phone number is empty
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                MessageBox.Show("Phone number is required to search for the customer.");
                return; // Exit the method without further execution
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to select customer information based on phone number
                    string query = "SELECT Cust_ID, Cust_FN, Cust_LN FROM Customer WHERE Phone = @PhoneNumber";

                    // Create a SqlCommand object with the query and connection
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameter to prevent SQL injection
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                        // Execute the command and get the SqlDataReader
                        SqlDataReader reader = command.ExecuteReader();

                        // Check if the reader has rows
                        if (reader.HasRows)
                        {
                            // Read the first row
                            reader.Read();

                            // Set the values in Form3 textboxes
                            textBox2.Text = reader["Cust_FN"].ToString();
                            textBox3.Text = reader["Cust_LN"].ToString();

                            // Get the customer ID for further queries
                            int custID = Convert.ToInt32(reader["Cust_ID"]);

                            // Close the reader before executing the second query
                            reader.Close();

                            // SQL query to retrieve service information for the customer with time formatted without seconds
                            string serviceQuery = @"
                                    SELECT
                                        M.Make AS Car_Make,
                                        MD.Model AS Car_Model,
                                        Ca.LicensePlate AS Car_License_Plate,
                                        FORMAT(CCSDT.ServiceDate, 'MMMM dd, yyyy') AS Service_Date, -- Format as Month Day, Year
                                        CONVERT(VARCHAR(5), CCSDT.ServiceTime, 108) AS Service_Time, -- Format as HH:MI
                                        S.Service AS Service_Name,
                                        S.Cost AS Service_Cost,
                                        CONCAT(T.Tech_FN, ' ', T.Tech_LN) AS Technician_Name
                                    FROM
                                        CarOwner CO
                                        INNER JOIN Car Ca ON CO.Car_ID = Ca.Car_ID
                                        INNER JOIN Cust_Car_Service_Date_Time CCSDT ON CO.Cust_Car_ID = CCSDT.Cust_Car_ID
                                        INNER JOIN Car_Service_Date_Services CSDS ON CCSDT.Cust_Car_Date_ID = CSDS.Cust_Car_Date_ID
                                        INNER JOIN Tech_to_Services TS ON CSDS.Tech_Service_ID = TS.Tech_Service_ID
                                        INNER JOIN Services S ON TS.Service_ID = S.Service_ID
                                        INNER JOIN Technician T ON TS.Tech_ID = T.Tech_ID
                                        INNER JOIN Make M ON Ca.MakeID = M.MakeID
                                        INNER JOIN Model MD ON Ca.ModelID = MD.ModelID
                                    WHERE
                                        CO.Cust_ID = @CustID";

                            // Create a new SqlCommand object for the service query
                            using (SqlCommand serviceCommand = new SqlCommand(serviceQuery, connection))
                            {
                                // Add parameter for customer ID
                                serviceCommand.Parameters.AddWithValue("@CustID", custID);

                                // Execute the service query and populate a DataTable
                                SqlDataAdapter adapter = new SqlDataAdapter(serviceCommand);
                                DataTable dt = new DataTable();
                                adapter.Fill(dt);

                                // Bind the DataTable to the DataGridView
                                dataGridView1.DataSource = dt;

                                // Optional: Display a message if no service records are found
                                if (dt.Rows.Count == 0)
                                {
                                    MessageBox.Show("No service records found for the customer.");
                                }
                            }

                            // Populate comboBox3 with the customer's cars
                            PopulateCarsComboBox(custID);
                        }
                        else
                        {
                            MessageBox.Show("Customer not found.");
                        }

                        // Close the reader
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        // Method to go back to the home page
        private void button4_Click(object sender, EventArgs e)
        {
            // Create an instance of Form2 
            Form1 form1 = new Form1();

            // Show Form2
            form1.Show();

            // Optionally, hide or close Form1
            this.Hide();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

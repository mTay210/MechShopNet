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
    public partial class Appointment : Form
    {
        private const string connectionString = "Server=localhost\\SQLEXPRESS;Database=MechanicShop;Integrated Security=SSPI;";

        public int CustCarID { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }

        private List<int> selectedTechServiceIDs = new List<int>();
        private List<int> selectedTechnicianIDs = new List<int>();

        private DateTime selectedDateTime;
        private List<AppointmentItem> appointmentsList = new List<AppointmentItem>();

        public Appointment()
        {
            InitializeComponent();
            PopulateServicesComboBox();
            selectedDateTime = DateTime.Now;
        }

        public DateTime DateAndTime
        {
            get => selectedDateTime;
            set => selectedDateTime = value;
        }

        public class AppointmentItem
        {
            public string Service { get; set; }
            public string Technician { get; set; }
            public decimal Cost { get; set; }
        }

        private void PopulateServicesComboBox()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Service_ID, Service FROM Services";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    DataRow blankRow = dt.NewRow();
                    blankRow["Service_ID"] = DBNull.Value;
                    blankRow["Service"] = string.Empty;
                    dt.Rows.InsertAt(blankRow, 0);

                    comboBox1.DisplayMember = "Service";
                    comboBox1.ValueMember = "Service_ID";
                    comboBox1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void PopulateTechniciansComboBox(int serviceID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT DISTINCT T.Tech_ID, T.Tech_FN, T.Tech_LN 
                            FROM Technician T 
                            INNER JOIN Tech_to_Services TS ON T.Tech_ID = TS.Tech_ID 
                            WHERE TS.Service_ID = @ServiceID";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@ServiceID", serviceID);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dt.Columns.Add("FullName", typeof(string), "Tech_FN + ' ' + Tech_LN");

                    comboBox2.DisplayMember = "FullName";
                    comboBox2.ValueMember = "Tech_ID";
                    comboBox2.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private int GetTechServiceID(System.Windows.Forms.ComboBox techComboBox, System.Windows.Forms.ComboBox serviceComboBox)
        {
            int techServiceID = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Tech_Service_ID FROM Tech_to_Services WHERE Tech_ID = @TechID AND Service_ID = @ServiceID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TechID", techComboBox.SelectedValue);
                        command.Parameters.AddWithValue("@ServiceID", serviceComboBox.SelectedValue);
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            techServiceID = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Tech_Service_ID: " + ex.Message);
            }

            return techServiceID;
        }


        private int GetTechnicianID(string technicianFullName)
        {
            int techID = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT Tech_ID FROM Technician WHERE CONCAT(Tech_FN, ' ', Tech_LN) = @FullName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FullName", technicianFullName);
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            techID = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting Technician ID: " + ex.Message);
            }

            return techID;
        }

        private decimal CalculateCost(object serviceID)
        {
            decimal cost = 0.00m;

            try
            {
                // Check if the serviceID is not null and is of the expected type
                if (serviceID != null && serviceID is int)
                {
                    int id = (int)serviceID;

                    // Fetch the service cost from the database based on the service ID
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "SELECT Cost FROM Services WHERE Service_ID = @ServiceID";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@ServiceID", id);
                            object result = command.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                cost = Convert.ToDecimal(result);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating cost: " + ex.Message);
            }

            return cost;
        }


        private void UpdateDataGridView()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear(); // Clear existing columns if needed

            // Add columns to the DataGridView
            dataGridView1.Columns.Add("Service", "Service");
            dataGridView1.Columns.Add("Technician", "Technician");
            dataGridView1.Columns.Add("Cost", "Cost");

            foreach (var appointment in appointmentsList)
            {
                dataGridView1.Rows.Add(appointment.Service, appointment.Technician, appointment.Cost);
            }
        }



        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null && comboBox1.SelectedValue != DBNull.Value)
            {
                int selectedServiceID = Convert.ToInt32(comboBox1.SelectedValue);
                PopulateTechniciansComboBox(selectedServiceID);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int techServiceID = GetTechServiceID(comboBox2, comboBox1);
            int technicianID = GetTechnicianID(comboBox2.Text);

            if (techServiceID > 0 && technicianID > 0)
            {
                decimal cost = CalculateCost(comboBox1.SelectedValue);

                AppointmentItem appointment = new AppointmentItem
                {
                    Service = comboBox1.Text,
                    Technician = comboBox2.Text,
                    Cost = cost
                };

                appointmentsList.Add(appointment);

                UpdateDataGridView();
            }
            else
            {
                MessageBox.Show("Error: Please select a valid service and technician.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (CustCarID > 0 && !string.IsNullOrEmpty(CustomerFirstName) && !string.IsNullOrEmpty(CustomerLastName))
                {
                    DateTime appointmentDateTime = selectedDateTime;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        foreach (var appointment in appointmentsList)
                        {
                            string insertQuery = @"
                                    INSERT INTO Cust_Car_Service_Date_Time (Cust_Car_ID, ServiceDate, ServiceTime, Service, Technician, Cost) 
                                    VALUES (@CustCarID, @AppointmentDate, @AppointmentTime, @Service, @Technician, @Cost)";

                            using (SqlCommand command = new SqlCommand(insertQuery, connection))
                            {
                                command.Parameters.AddWithValue("@CustCarID", CustCarID);
                                command.Parameters.AddWithValue("@AppointmentDate", appointmentDateTime.Date);
                                command.Parameters.AddWithValue("@AppointmentTime", appointmentDateTime.TimeOfDay);
                                command.Parameters.AddWithValue("@Service", appointment.Service);
                                command.Parameters.AddWithValue("@Technician", appointment.Technician);
                                command.Parameters.AddWithValue("@Cost", appointment.Cost);

                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    // Success message or further processing
                                }
                                else
                                {
                                    MessageBox.Show("Error adding service: " + appointment.Service);
                                }
                            }
                        }

                        appointmentsList.Clear();
                    }

                    MessageBox.Show("All services scheduled successfully!");
                }
                else
                {
                    MessageBox.Show("Error: Selected car information is missing.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

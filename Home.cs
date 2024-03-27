using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MechShop
{
    public partial class Form1 : Form
    {
        // Connect to the server and database
        private const string connectionString = "Server=localhost\\SQLEXPRESS;Database=MechanicShop;Integrated Security=SSPI;";

        public Form1()
        {
            InitializeComponent();
        }

        // This method is used to open the NewCustomer form
        private void button1_Click(object sender, EventArgs e)
        {
            NewCustomer newCustomer = new NewCustomer();
            newCustomer.Show();
        }

        // This method is used to open the Customer form
        private void button2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();

            // Hide the current form
            this.Hide();
        }

        // This method is used to open the AddTech form
        private void button3_Click(object sender, EventArgs e)
        {
            AddTech addTech = new AddTech();
            addTech.Show();
        }

        // This method is used to open the AddService form
        private void button4_Click(object sender, EventArgs e)
        {
            AddService addService = new AddService();
            addService.Show();
        }

        // This method is used to open the TechServices form
        private void button5_Click(object sender, EventArgs e)
        {
            TechServices techServices = new TechServices();
            techServices.Show();
        }
    }
}
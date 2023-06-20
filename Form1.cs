using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;
using System.Reflection;
using System.Xml.Linq;
using System.Security.Policy;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.IO;

namespace Test
{
    public partial class Form1 : Form
    {
        string connectionString = @"Data Source=.;Initial Catalog=Ordering;Trusted_Connection=True;";
        string curItem = "";
        string itemId = "";
        private bool button1WasClicked = false;
        public Form1()
        {
            InitializeComponent();
            insert_to_listCategories();
            insert_notes_to_noteslist_and_combobox();
        }

        private void insert_notes_to_noteslist_and_combobox()
        {
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString("https://noteapi.popssolutions.net/notes/getall");
                dynamic result = JsonConvert.DeserializeObject<dynamic>(json);

                foreach (var item in result)
                {
                    ListViewItem item1 = new ListViewItem(item["text"].ToString());
                    item1.SubItems.Add(item["userId"].ToString());
                    listViewNotes.Items.AddRange(new ListViewItem[] { item1 });

                }
            }

            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString("https://noteapi.popssolutions.net/users/getall");
                dynamic result = JsonConvert.DeserializeObject<dynamic>(json);

                foreach (var item in result)
                {
                    cboAssignToUser.Items.Add(item["id"].ToString());
                }
            }

            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString("https://noteapi.popssolutions.net/intrests/getall");
                dynamic result = JsonConvert.DeserializeObject<dynamic>(json);

                foreach (var item in result)
                {
                    cboIntresets.Items.Add(item["id"].ToString());
                }
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand sqlComm = new SqlCommand();
                String query = "insert into ItemPrinters (ItemId,Printer) values(@itemId,@printer);";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@itemId", itemId);
                cmd.Parameters.AddWithValue("@printer", listPrinters.SelectedItem.ToString());
                cmd.ExecuteNonQuery();
                SqlDataReader r = cmd.ExecuteReader();
                con.Close();
                MessageBox.Show("item and printer added Successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void insert_to_listCategories()
        {
            String query = "select * from categories";
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            DataSet ds = new DataSet();
            SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                listCateogries.Items.Add(r["Name"]);
            }
            con.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string name = textBox2.Text;
            string price = textBox3.Text;
            listItems.Items.Add(name);
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand sqlComm = new SqlCommand();
            String query = "insert into Items (Name,Price) values (@name,@price);";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.ExecuteNonQuery();
            SqlDataReader r = cmd.ExecuteReader();
            con.Close();
            MessageBox.Show("item added Successfully");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {                
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand sqlComm = new SqlCommand();
                String query = "insert into Orders (TotalPrice,TaxPrice) values(@total,@tax);";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@total", label7.Text);
                cmd.Parameters.AddWithValue("@tax", label6.Text);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Prices added Successfully");
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.ToString());
            }
            label7.Text = "0";
            label6.Text = "0";

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string text1 = textBox1.Text;
            listCateogries.Items.Add(text1);
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand sqlComm = new SqlCommand();
            String query = "insert into categories (Name) values(@text1);";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@text1", text1);
            cmd.ExecuteNonQuery();
            con.Close();
            MessageBox.Show("Category added successfully!!");
        }

        private void listCateogries_SelectedIndexChanged(object sender, EventArgs e)
        {
            listItems.Items.Clear();
            curItem = listCateogries.SelectedItem.ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string query = "select i.Name,i.Id from Items i where i.CategoryId=(select Id from categories where name=@curItem)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@curItem", curItem);
            SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                itemId = r["Id"].ToString();
                listItems.Items.Add(r["Name"]);
            }
            //MessageBox.Show(itemId);
            con.Close();
        }

        private void listItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            curItem = listItems.SelectedItem.ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            String query = "select Name,Price from Items where Name=@curItem";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@curItem", curItem);
            SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                var index = this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[index].Cells[0].Value = r["Name"];
                this.dataGridView1.Rows[index].Cells[1].Value = r["Price"];
            }
            con.Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            label7.Text = "0";
            label6.Text = "0";
            if (dataGridView1.Rows.Count.ToString() == "0")
                MessageBox.Show("Select items to order");
            else
            {
                double s = 0;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    s += double.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString());
                }
                label7.Text = s.ToString();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            label7.Text = "0";
            label6.Text = "0";
            if (dataGridView1.Rows.Count.ToString() == "0")
                MessageBox.Show("Select items to order");
            else
            {
                double s = 0;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    s += double.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString());
                }
                label7.Text = s.ToString();
                s = s * 0.05;
                label6.Text = s.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 


        private async void  btnClearAll_Click(object sender, EventArgs e)
        {            
            HttpClient client = new HttpClient();
            var response = await client.PostAsync("https://noteapi.popssolutions.net/notes/clear", null);
            listViewNotes.Items.Clear();
        }

        private async void btnSaveNote_Click(object sender, EventArgs e)
        {
            if (button1WasClicked)
            {
                listViewNotes.Items.Clear();
                string payload = JsonConvert.SerializeObject(new
                {
                    Text = "Test Note44",
                    UserId = "1",
                    PlaceDateTime = "2021-11-18T09:39:44"
                });
                var client = new HttpClient();
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("https://noteapi.popssolutions.net/notes/insert", content);
                insert_notes_to_noteslist_and_combobox();
            }

            else
            {
                listViewNotes.Items.Clear();
                string payload = JsonConvert.SerializeObject(new
                {
                    Id = "0",
                    Text = "dasd...",
                    UserId = "1",
                    PlaceDateTime= "2021-11-18T09:39:44"
                });
                var client = new HttpClient();
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("https://noteapi.popssolutions.net/notes/update", content);
                insert_notes_to_noteslist_and_combobox();
            }
            button1WasClicked = false;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            button1WasClicked = true;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString("https://noteapi.popssolutions.net/notes/getall");
                dynamic result = JsonConvert.DeserializeObject<dynamic>(json);

                foreach (var item in result)
                {
                    ListViewItem item1 = new ListViewItem(item["text"].ToString());
                    item1.SubItems.Add(item["userId"].ToString());
                    listViewNotes.Items.AddRange(new ListViewItem[] { item1 });

                }
            }
        }

        private void listViewNotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                curItem = listViewNotes.SelectedItems[0].ToString();
                tbNotes.AppendText(curItem.Split('{')[1].Split('}')[0]+Environment.NewLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cboAssignToUser_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void btnAddNewUser_Click(object sender, EventArgs e)
        {
            try
            {
                listViewNotes.Items.Clear();
                string image = null;
                if (tbImage.Text != "") image = tbImage.Text;
                string payload = JsonConvert.SerializeObject(new
                {
                    Username = tbUsername.Text,
                    Password = tbPassword.Text,
                    UserId = tbEmail.Text,
                    intrestId = cboIntresets.GetItemText(cboIntresets.SelectedItem).ToString(),
                    imageAsBase64 = image,
                });
                var client = new HttpClient();
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("https://noteapi.popssolutions.net/users/insert", content);
                insert_notes_to_noteslist_and_combobox();
                MessageBox.Show("User added successfully!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void button7_Click(object sender, EventArgs e)
        {
            int size = -1;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    string text = File.ReadAllText(file);
                    size = text.Length;                    
                    tbImage.Text = text;
                }
                catch (IOException)
                {
                }
            }
        }
    }
}

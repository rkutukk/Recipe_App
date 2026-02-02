using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RECİPE_APP
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        string Connection_Str = "Data Source=RABIAAA\\SQLEXPRESS;Initial Catalog=DB_TarifApp;Integrated Security=true; TrustServerCertificate=True;";
        SqlCommand Command;
        SqlDataReader D_Reader;
        SqlDataAdapter D_Adapter;
        DataSet D_Set;
        private bool Control_RecipeName(string recipe_name)
        {
            using (SqlConnection con = new SqlConnection(Connection_Str))
            {
                con.Open();
                string query = "select COUNT(*) FROM Tarifler WHERE TarifAdi = @TarifAdi";
                using (Command = new SqlCommand(query, con))
                {
                    Command.Parameters.AddWithValue("@TarifAdi", recipe_name);
                    int count = (int)Command.ExecuteScalar();
                    return count > 0; 
                }
            }
        }
        void Get_Data()
        {
            using (SqlConnection connection = new SqlConnection(Connection_Str))
            {
                connection.Open();
                string Query = "select TarifID,TarifAdi,Kategori,HazirlamaSuresi,Talimatlar from tarifler";
                D_Adapter = new SqlDataAdapter(Query, connection);
                D_Set = new DataSet();
                D_Adapter.Fill(D_Set, "tarifler");
                dataGridView1.DataSource = D_Set.Tables["tarifler"];

            }
        }
        void Get_Category()
        {
            using (SqlConnection con = new SqlConnection(Connection_Str))
            {
                con.Open();
                string Query = "select DISTINCT Kategori from Tarifler";
                using (Command = new SqlCommand(Query, con))
                {
                    using (SqlDataReader D_Reader = Command.ExecuteReader())
                    {
                        while (D_Reader.Read())
                        {
                            comboBox1.Items.Add(D_Reader["Kategori"].ToString());
                        }
                    }
                }

            }
        }
        void Add_Recipe(string recipe_name, string category, decimal time, string instructıons)
        {

            using (SqlConnection con = new SqlConnection(Connection_Str))
            {

                con.Open();
                string query = @"
                INSERT INTO Tarifler (TarifAdi, HazirlamaSuresi, Kategori, Talimatlar) 
                VALUES (@TarifAdi, @HazirlamaSuresi, @Kategori, @Talimatlar)";
                using (Command = new SqlCommand(query, con))
                {
                    Command.Parameters.AddWithValue("@TarifAdi", recipe_name);
                    Command.Parameters.AddWithValue("@HazirlamaSuresi", time);
                    Command.Parameters.AddWithValue("@Kategori", category);
                    Command.Parameters.AddWithValue("@Talimatlar", instructıons);
                    int result = Command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Tarif başarıyla eklendi!");
                    }
                    else
                    {
                        MessageBox.Show("Tarif eklenemedi!");
                    }

                }
            }
        }
        void Update_Recipe(string recipe_ID, string recipe_name, string category, decimal time, string instructıons)
        {
            using (SqlConnection con = new SqlConnection(Connection_Str))
            {

                con.Open();
                string query = @"
                UPDATE Tarifler 
                SET  TarifAdi=@TarifAdi  , HazirlamaSuresi=@HazirlamaSuresi , Kategori=@Kategori, Talimatlar=@Talimatlar
                Where TarifID= @TarifID ";

                using (Command = new SqlCommand(query, con))
                {
                    Command.Parameters.AddWithValue("@TarifAdi", recipe_name);
                    Command.Parameters.AddWithValue("@HazirlamaSuresi", time);
                    Command.Parameters.AddWithValue("@Kategori", category);
                    Command.Parameters.AddWithValue("@Talimatlar", instructıons);
                    Command.Parameters.AddWithValue("@TarifID", recipe_ID);
                    int result = Command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Tarif başarıyla GÜNCELLENDİ!");
                    }
                    else
                    {
                        MessageBox.Show("Tarif güncellenemedi!");
                    }
                }
            }
        }
        void Delete_recipe(string recipe_ID)
        {
            using (SqlConnection con = new SqlConnection(Connection_Str))
            {
                con.Open();
                string query = @"delete from tarifler where TarifID = @TarifID";
                using (Command = new SqlCommand(query, con))
                {
                    Command.Parameters.AddWithValue("@TarifID", recipe_ID);
                }
                int result = Command.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Tarif başarıyla silindi!");
                }
                else
                {
                    MessageBox.Show("Tarif silinemedi!");
                }
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Get_Category();
            Get_Data();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string recipe_name = textBox1.Text;
            string category = comboBox1.Text;
            decimal time = numericUpDown1.Value;
            string instructıons = richTextBox1.Text;
            if (string.IsNullOrEmpty(recipe_name))
            {
                MessageBox.Show("Tarif ismi giriniz!");
            }
            else if (string.IsNullOrEmpty(category))
            {
                MessageBox.Show("Kategori seçiniz!");
            }

            else if (time <= 0)
            {
                MessageBox.Show("Hazırlama süresini giriniz!");

            }
            else if (string.IsNullOrEmpty(instructıons))
            {
                MessageBox.Show("Talimatları girinz!");
            }
            else if (Control_RecipeName(recipe_name)) 
            {
                MessageBox.Show("aynı isimde tarif mevcut başka bir isim giriniz!");
            }
            else
            {
                Add_Recipe(recipe_name, category, time, instructıons);
                Get_Data();
            }

        }

        public void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selected_Row = dataGridView1.Rows[e.RowIndex];
                textBox2.Text = selected_Row.Cells[0].Value.ToString();
                textBox1.Text = selected_Row.Cells[1].Value.ToString();
                comboBox1.Text = selected_Row.Cells[2].Value.ToString();
                numericUpDown1.Value = (decimal)selected_Row.Cells[3].Value;
                richTextBox1.Text = selected_Row.Cells[4].Value.ToString();
            }
        }

        public void button2_Click(object sender, EventArgs e)
        {

            string recipeID = textBox2.Text;
            string recipe_name = textBox1.Text;
            string category = comboBox1.Text;
            decimal time = numericUpDown1.Value;
            string instructıons = richTextBox1.Text;
          
            Update_Recipe(recipeID, recipe_name, category, time, instructıons);
            Get_Data() ;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string recipeID = textBox2.Text;
            Delete_recipe(recipeID);
            Get_Data();
        }
    }
}
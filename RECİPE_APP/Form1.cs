using Microsoft.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;
using Quickenshtein;

namespace RECİPE_APP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string Connection_Str = "Data Source=RABIAAA\\SQLEXPRESS;Initial Catalog=DB_TarifApp;Integrated Security=true; TrustServerCertificate=True;";
        SqlCommand Command;
        SqlDataReader D_Reader;
        SqlDataAdapter D_Adapter;
        DataSet D_Set;

        void List_Category(string category)
        {
            using (SqlConnection con = new SqlConnection(Connection_Str))
            {
                con.Open();
                string query = @"select * from tarifler where Kategori = @Kategori";
                using (Command = new SqlCommand(query, con))
                {
                    Command.Parameters.AddWithValue("@Kategori", category);
                    DataTable D_Table = new DataTable();
                    using (SqlDataReader reader = Command.ExecuteReader())
                    {
                        D_Table.Load(reader);
                    }
                    dataGridView1.DataSource = D_Table;
                }

            }
        }

        void Search_Data()
        {
            string keyword = textBox1.Text;
            var similar_recipes = new DataTable();

            similar_recipes.Columns.Add("TarifID", typeof(int));
            similar_recipes.Columns.Add("TarifAdi", typeof(string));
            similar_recipes.Columns.Add("Kategori", typeof(string));
            similar_recipes.Columns.Add("HazirlamaSuresi", typeof(int));
            similar_recipes.Columns.Add("Talimatlar", typeof(string));

            using (var con = new SqlConnection(Connection_Str))
            {
                con.Open();
                string Query = "select * from Tarifler";
                using (SqlCommand command = new SqlCommand(Query, con))
                using (SqlDataReader Reader = command.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        int tarifId = (int)Reader["TarifID"];
                        string tarifAdi = Reader["TarifAdi"].ToString();
                        string kategori = Reader["Kategori"].ToString();
                        decimal hazirlamaSuresi = (decimal)Reader["HazirlamaSuresi"];
                        string talimatlar = Reader["Talimatlar"].ToString();

                        int similarity = Levenshtein.GetDistance(keyword, tarifAdi);
                        if (similarity < 5)
                        {
                            {
                                similar_recipes.Rows.Add(tarifId, tarifAdi, kategori, hazirlamaSuresi, talimatlar);
                            }
                        }
                    }
                }

                dataGridView1.DataSource = similar_recipes;
            }
        }


        void Get_Ingredient()
        {
            using (SqlConnection connection = new SqlConnection(Connection_Str))
            {
                connection.Open();
                string Query = "select MalzemeAdi from Malzemeler";
                using (Command = new SqlCommand(Query, connection))
                {
                    using (D_Reader = Command.ExecuteReader())
                    {
                        while (D_Reader.Read())
                        {
                            comboBox1.Items.Add(D_Reader["MalzemeAdi"].ToString());
                        }
                    }
                }
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
                            comboBox2.Items.Add(D_Reader["Kategori"].ToString());
                        }
                    }
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
        private void Form1_Load(object sender, EventArgs e)
        {
            Get_Data();
            Get_Category();
            Get_Ingredient();

            comboBox3.Items.Add("0-50");
            comboBox3.Items.Add("50-100");
            comboBox3.Items.Add("100-150");
            comboBox3.Items.Add("150-200");
            comboBox3.Items.Add("200-250");
            comboBox3.Items.Add("250-300");
            comboBox3.Items.Add("300-350");

            comboBox4.Items.Add("0-20");
            comboBox4.Items.Add("20-40");
            comboBox4.Items.Add("40-60");
            comboBox4.Items.Add("60-80");
            comboBox4.Items.Add("80-100");
            comboBox4.Items.Add("100-120");
           

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.ShowDialog();
        }



        private void button3_Click(object sender, EventArgs e)
        {
            if ((comboBox1.SelectedItem == null))
            {
                MessageBox.Show("malzeme seçiniz");
            }
            string ingredient = comboBox1.Text;
            if (listBox1.Items.Contains(ingredient))
            {
                MessageBox.Show("bu malzeme zaten ekli");
            }
            else
            {

                listBox1.Items.Add(ingredient);
            }

        }


        private void button4_Click(object sender, EventArgs e)
        {
           string selected_ingredients = comboBox2.SelectedItem.ToString();
           List_Category(selected_ingredients);
            
        }
    }
}


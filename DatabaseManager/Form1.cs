using Microsoft.VisualBasic.Devices;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DatabaseManager
{
    public partial class Form1 : Form
    {

        private MySqlConnection connection;
        private MySqlCommand command;
        Button backButton = new()
        {
            Text = "<",
            Location = new(12, 408),
            Size = new(30, 30)
        };

        public Form1()
        {
            InitializeComponent();
            button2.Visible = false;
            button3.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button4.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            backButton.Visible = false;

            TableLayoutPanel text = new()
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                RowCount = 3,
                ColumnCount = 2
            };

            text.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));
            text.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));
            text.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));
            text.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            TextBox tbox = new()
            {
                PlaceholderText = "Host/Server: ",
                Dock = DockStyle.Fill,
                ForeColor = Color.Black
            };
            text.Controls.Add(tbox, 0, 0);

            TextBox tbox2 = new()
            {
                PlaceholderText = "Username: ",
                Dock = DockStyle.Fill,
                ForeColor = Color.Black
            };
            text.Controls.Add(tbox2, 0, 1);

            TextBox tbox3 = new()
            {
                PlaceholderText = "Password: ",
                Dock = DockStyle.Fill,
                ForeColor = Color.Black,
                PasswordChar = '*'
            };
            text.Controls.Add(tbox3, 0, 2);

            Button connectButton = new()
            {
                Text = "Connect!",
                Dock = DockStyle.Fill
            };
            connectButton.Click += (sender, e) =>
            {
                string server = tbox.Text;
                string username = tbox2.Text;
                string pw = tbox3.Text;

                string connectionquery = $"Server={server}; User Id={username}; Password={pw}";

                try
                {
                    connection = new(connectionquery);
                    connection.Open();
                    MessageBox.Show("Connection Successful!");
                    button1.Visible = true;
                    button2.Visible = true;
                    button4.Visible = true;
                    text.Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error Connecting! {ex.Message}");
                    Clear(tbox, tbox2, tbox3);
                    return;
                }
            };

            text.Controls.Add(connectButton, 0, 3);

            this.Controls.Add(text);
            button1.Visible = false;
        }

        private static void Clear(TextBox tbox, TextBox tbox2, TextBox tbox3)
        {
            tbox.Clear();
            tbox2.Clear();
            tbox3.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button4.Visible = false;
            HideButtons();

            TableLayoutPanel text = new()
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                RowCount = 1,
                ColumnCount = 1
            };

            text.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));

            TextBox tbox = new TextBox
            {
                Dock = DockStyle.Fill,
                PlaceholderText = "Database Name: ",
                ForeColor = Color.Black
            };

            text.Controls.Add(tbox, 0, 0);

            Button createButton = new()
            {
                Text = "Create!",
                Dock = DockStyle.Fill
            };

            createButton.Click += (sender, e) =>
            {
                string dbName = tbox.Text;
                string createDatabaseQuery = $"CREATE DATABASE {dbName}";

                try
                {
                    command = new MySqlCommand(createDatabaseQuery, connection);
                    command.ExecuteNonQuery();

                    MessageBox.Show($"{dbName} database created!");
                    text.Visible = false;

                    button1.Visible = true;
                    button2.Visible = true;
                    button3.Visible = true;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Error creating database with name {dbName}! {ex.Message}");
                }
            };

            this.Controls.Add(text);
            text.Controls.Add(createButton, 0, 1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button4.Visible = false;
            HideButtons();

            TableLayoutPanel text = new()
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                RowCount = 1,
                ColumnCount = 1
            };

            text.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));

            TextBox tbox = new()
            {
                Dock = DockStyle.Fill,
                PlaceholderText = "Database to Delete: ",
                ForeColor = Color.Black
            };

            text.Controls.Add(tbox, 0, 0);

            Button deleteButton = new()
            {
                Text = "Delete!",
                Dock = DockStyle.Fill

            };

            deleteButton.Click += (sender, e) =>
            {
                string dbNameDelete = tbox.Text;
                string createDatabaseQuery = $"DROP DATABASE {dbNameDelete}";
                string checkDatabaseCountQuery = "SELECT COUNT(*) FROM information_schema.SCHEMATA WHERE SCHEMA_NAME NOT IN ('information_schema', 'mysql', 'performance_schema', 'sys', 'sakilla', 'world')"; // If you use this this depends on your config, some might not have these same databases by default
                int count = 0;
                try
                {
                    command = new(createDatabaseQuery, connection);
                    command.ExecuteNonQuery();

                    MySqlCommand command2 = new(checkDatabaseCountQuery, connection);

                    count = Convert.ToInt32(command2.ExecuteScalar());

                    MessageBox.Show($"{dbNameDelete} database deleted!");
                    text.Visible = true;
                    HideButtons();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Error deleting database with name {dbNameDelete}! {ex.Message}");
                }

                if (count == 0)
                {
                    MessageBox.Show("No databases to delete!");
                    ShowButtons();
                    text.Visible = false;
                    button3.Visible = false;
                    return;
                }
            };

            Button backButton = new()
            {
                Text = "Back",
                Dock = DockStyle.Fill
            };

            backButton.Click += (sender, e) =>
            {
                try
                {
                    ShowButtons();
                    text.Visible = false;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Error going back {ex.Message}");
                }
            };

            this.Controls.Add(text);
            text.Controls.Add(deleteButton, 0, 1);
            text.Controls.Add(backButton, 0, 2);
        }

        private void HideButtons()
        {
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
        }

        private void ShowButtons()
        {
            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            HideButtons();
            int click = 0;

            click++;

            if(click >= 1)
            {
                backButton.Visible = true;
            }
            else
            {
                backButton.Visible = false;
            }


                backButton.Click += (sender, e) =>
                        {
                            if (click >= 1)
                            {
                                backButton.Visible = false;
                                click--;
                            }

                            try
                            {

                                if (connection == null)
                                    button1.Visible = true;
                                else
                                {
                                    ShowButtons();
                                    button4.Visible = true;
                                    backButton.Visible = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        };

            this.Controls.Add(backButton);
        }
    }
}

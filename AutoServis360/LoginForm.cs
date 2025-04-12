using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoServis360
{
    public class LoginForm : Form
    {
        private Panel container; // Hlavní kontejner
        private Label titleLabel;
        private ComboBox roleComboBox;
        private TextBox usernameBox;
        private TextBox passwordBox;
        private Button loginButton;

        public User LoggedInUser { get; private set; }

        public LoginForm()
        {
            this.Text = "Přihlášení";
            this.Size = new Size(800, 600);
            this.MinimumSize = new Size(400, 300);

            InitializeComponents();
            this.Resize += new EventHandler(ResponsiveLayout);
            CenterContainer();
        }

        private void InitializeComponents()
        {
            // Hlavní kontejner
            container = new Panel
            {
                BackColor = Color.White,
                Padding = new Padding(20),
                MaximumSize = new Size(1000, 0), // Max šířka 1000px
                AutoSize = true
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                AutoSize = true,
                Padding = new Padding(10)
            };

            // Nadpis
            titleLabel = new Label
            {
                Text = "Přihlášení",
                Font = new Font("Arial", 28, FontStyle.Bold),
                ForeColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 70
            };

            // ComboBox pro výběr role
            roleComboBox = new ComboBox
            {
                Font = new Font("Arial", 14),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 10, 0, 10),
                Dock = DockStyle.Top,
                Height = 40
            };
            roleComboBox.Items.AddRange(new string[] { "Recepční", "Mechanik" });
            roleComboBox.SelectedIndex = 0;

            // TextBox pro uživatelské jméno
            usernameBox = new TextBox
            {
                PlaceholderText = "Uživatelské jméno",
                Font = new Font("Arial", 14),
                Margin = new Padding(0, 10, 0, 10),
                Dock = DockStyle.Top,
                Height = 40
            };

            // TextBox pro heslo
            passwordBox = new TextBox
            {
                PlaceholderText = "Heslo",
                UseSystemPasswordChar = true,
                Font = new Font("Arial", 14),
                Margin = new Padding(0, 10, 0, 10),
                Dock = DockStyle.Top,
                Height = 40
            };

            // Přihlašovací tlačítko
            loginButton = new Button
            {
                Text = "Přihlásit se",
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                Font = new Font("Arial", 14),
                FlatStyle = FlatStyle.Flat,
                Height = 50,
                Margin = new Padding(0, 20, 0, 0),
                Dock = DockStyle.Top
            };
            loginButton.Click += LoginButton_Click;

            // Přidání prvků do layoutu
            layout.Controls.Add(titleLabel, 0, 0);
            layout.Controls.Add(roleComboBox, 0, 1);
            layout.Controls.Add(usernameBox, 0, 2);
            layout.Controls.Add(passwordBox, 0, 3);
            layout.Controls.Add(loginButton, 0, 4);

            // Přidání layoutu do kontejneru
            container.Controls.Add(layout);

            // Přidání kontejneru do formuláře
            this.Controls.Add(container);
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string username = usernameBox.Text.Trim();
            string password = passwordBox.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Zadejte uživatelské jméno a heslo.", "Upozornění", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserDAO userDAO = new UserDAO();
            User user = userDAO.AuthenticateUser(username, password);

            if (user != null)
            {
                LoggedInUser = user;
                this.DialogResult = DialogResult.OK;
                
            }
            else
            {
                MessageBox.Show("Neplatné přihlašovací údaje!", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
  
        private void ResponsiveLayout(object sender, EventArgs e)
        {
            CenterContainer();
        }

        private void CenterContainer()
        {
            container.Left = (this.ClientSize.Width - container.Width) / 2;
            container.Top = (this.ClientSize.Height - container.Height) / 2;

            if (this.ClientSize.Width < 1000)
            {
                container.Width = this.ClientSize.Width - 40;
            }
            else
            {
                container.Width = 1000;
            }
        }
    }
}

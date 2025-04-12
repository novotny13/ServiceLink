using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace AutoServis360
{
    public class ReceptionForm : Form
    {
        private Panel customerPanel;
        private Panel vehiclePanel;
        private Button submitButton;

        public ReceptionForm()
        {
            this.Text = "Reception Form";
            this.Size = new Size(800, 600);
            this.MinimumSize = new Size(400, 300);

            InitializeComponents();
            this.Resize += new EventHandler(ResponsiveLayout);
            CenterPanels();
        }

        private void InitializeComponents()
        {
            // Panel for Customer Information
            customerPanel = CreatePanel("Customer", new string[] { "Jmeno:", "Prijmeni:", "Email:", "Telefon:", "Adresa:" });

            // Panel for Vehicle Information
            vehiclePanel = CreatePanel("Vozidlo", new string[] { "Znacka:", "Model:", "SPZ:", "Rok vyroby:", "Text area pro popis problemu:" });

            // Submit Button
            submitButton = new Button
            {
                Text = "Odeslat",
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                Font = new Font("Arial", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Height = 50,
                Dock = DockStyle.Bottom,
                Margin = new Padding(20)
            };

            // Add controls to the form
            this.Controls.Add(submitButton);
            this.Controls.Add(customerPanel);
            this.Controls.Add(vehiclePanel);
            submitButton.Click += new EventHandler(SubmitButton_Click);
        }

        private Panel CreatePanel(string headerText, string[] labels)
        {
            Panel panel = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = true
            };

            // Header
            Label header = new Label
            {
                Text = headerText,
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 40
            };

            // Add input fields
            TableLayoutPanel layout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = labels.Length,
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(10),
                BackColor = Color.White
            };

            foreach (string label in labels)
            {
                Label lbl = new Label
                {
                    Text = label,
                    Font = new Font("Arial", 12),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Dock = DockStyle.Fill
                };

                Control input;
                if (label.Contains("Text area"))
                {
                    input = new TextBox
                    {
                        Multiline = true,
                        Height = 60,
                        Font = new Font("Arial", 12),
                        Dock = DockStyle.Fill
                    };
                }
                else
                {
                    input = new TextBox
                    {
                        Font = new Font("Arial", 12),
                        Dock = DockStyle.Fill
                    };
                }

                layout.Controls.Add(lbl);
                layout.Controls.Add(input);
            }

            panel.Controls.Add(layout);
            panel.Controls.Add(header);
            return panel;
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Najdeme TableLayoutPanel v customerPanel a vehiclePanel
                TableLayoutPanel customerTable = customerPanel.Controls.OfType<TableLayoutPanel>().FirstOrDefault();
                TableLayoutPanel vehicleTable = vehiclePanel.Controls.OfType<TableLayoutPanel>().FirstOrDefault();

                if (customerTable == null || vehicleTable == null)
                {
                    MessageBox.Show("Chyba: Nelze najít vstupní pole.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Získání hodnot z formuláøe
                string jmeno = ((TextBox)customerTable.Controls[1]).Text;
                string prijmeni = ((TextBox)customerTable.Controls[3]).Text;
                string email = ((TextBox)customerTable.Controls[5]).Text;
                string telefon = ((TextBox)customerTable.Controls[7]).Text;
                string adresa = ((TextBox)customerTable.Controls[9]).Text;

                string znacka = ((TextBox)vehicleTable.Controls[1]).Text;
                string model = ((TextBox)vehicleTable.Controls[3]).Text;
                string spz = ((TextBox)vehicleTable.Controls[5]).Text;
                int rokVyroby = int.TryParse(((TextBox)vehicleTable.Controls[7]).Text, out int rok) ? rok : 0;
                string problem = ((TextBox)vehicleTable.Controls[9]).Text;

                // Vytvoøení objektù
                Zakaznik zakaznik = new Zakaznik(0, jmeno, prijmeni, email, telefon, adresa);
                Vozidlo auto = new Vozidlo(0, 0, znacka, model, spz, rokVyroby);

                // DAO instance
                ZakaznikDAO zakaznikDAO = new ZakaznikDAO();
                VozidloDAO carDAO = new VozidloDAO();
                RepairDAO repairDAO = new RepairDAO();

                // Zákazník - existuje?
                if (zakaznikDAO.CustomerExists(zakaznik))
                {
                    int customerId = zakaznikDAO.GetCustomerId(zakaznik);
                    if (customerId == -1)
                    {
                        MessageBox.Show("Chyba: Zákazník nebyl nalezen.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    auto.MajitelId = customerId;
                }
                else
                {
                    if (!zakaznikDAO.InsertCustomer(zakaznik))
                    {
                        MessageBox.Show("Chyba: Nepodaøilo se pøidat zákazníka.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int customerId = zakaznikDAO.GetCustomerId(zakaznik);
                    if (customerId == -1)
                    {
                        MessageBox.Show("Chyba: Nepodaøilo se získat ID zákazníka.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    auto.MajitelId = customerId;
                }

                // Auto - existuje?
                if (carDAO.CarExists(auto.SPZ))
                {
                    // Auto existuje, získáme jeho ID
                    int carId = carDAO.GetCarIdBySPZ(auto.SPZ);
                    if (carId == -1)
                    {
                        MessageBox.Show("Chyba: Nepodaøilo se získat ID auta.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    auto.Id = carId;
                }
                else
                {
                    // Vložit auto do DB
                    if (!carDAO.InsertCar(auto))
                    {
                        MessageBox.Show("Chyba: Nepodaøilo se pøidat auto.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    auto.Id = carDAO.GetCarIdBySPZ(auto.SPZ);
                }

                // Vytvoøit opravu
                if (repairDAO.CreateRepair(auto.Id, auto.MajitelId, problem))
                {
                    MessageBox.Show("Záznam úspìšnì vytvoøen. Oprava byla založena.", "Úspìch", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Chyba: Nepodaøilo se vytvoøit opravu.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Neoèekávaná chyba: {ex.Message}", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResponsiveLayout(object sender, EventArgs e)
        {
            CenterPanels();
        }

        private void CenterPanels()
        {
            int panelSpacing = 20;

            // Dynamically size and position panels
            int totalHeight = customerPanel.Height + vehiclePanel.Height + submitButton.Height + panelSpacing;
            int startY = (this.ClientSize.Height - totalHeight) / 2;

            customerPanel.Width = Math.Min(this.ClientSize.Width - 40, 600);
            customerPanel.Left = (this.ClientSize.Width - customerPanel.Width) / 2;
            customerPanel.Top = startY;

            vehiclePanel.Width = customerPanel.Width;
            vehiclePanel.Left = customerPanel.Left;
            vehiclePanel.Top = customerPanel.Bottom + panelSpacing;

            submitButton.Width = customerPanel.Width / 2;
            submitButton.Left = (this.ClientSize.Width - submitButton.Width) / 2;
            submitButton.Top = vehiclePanel.Bottom + panelSpacing;
        }
    }
}
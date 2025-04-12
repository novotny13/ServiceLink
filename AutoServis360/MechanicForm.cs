using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace AutoServis360
{
    public class MechanicForm : Form
    {
        private DataGridView dgvCars;
        private Panel panelCarList;
        private Panel panelInventory;
        private Panel detailPanel;
        private Label lblProblemTitle;
        private TextBox txtProblemDescription;
        private Mechanic currentMechanic;
        private MechanicDAO mechanicDAO;
        private RepairDAO repairDAO;
        private VozidloDAO vozidloDAO;
        private bool showingInventory = false;

        public MechanicForm(Mechanic mechanic)
        {
            currentMechanic = mechanic;
            mechanicDAO = new MechanicDAO();
            repairDAO = new RepairDAO();
            vozidloDAO = new VozidloDAO();

            InitializeForm();
            InitializeCarListPanel();
            InitializeDetailPanel();
            InitializeInventoryPanel();
            InitializeToggleButton();

            LoadCarList();
        }

        private void InitializeForm()
        {
            this.Text = "Mechanik - AutoServis360";
            this.Size = new Size(900, 700);
            this.BackColor = Color.WhiteSmoke;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
        }

        private void InitializeCarListPanel()
        {
            panelCarList = new Panel
            {
                Size = new Size(860, 350),
                Location = new Point(20, 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            dgvCars = new DataGridView
            {
                Size = new Size(860, 350),
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.DodgerBlue,
                    ForeColor = Color.White,
                    Font = new Font("Arial", 10, FontStyle.Bold)
                },
                EnableHeadersVisualStyles = false,
                RowTemplate = { Height = 40 },
                AllowUserToResizeRows = false
            };

            dgvCars.Columns.Add("SPZ", "SPZ");
            dgvCars.Columns.Add("Brand", "Značka");
            dgvCars.Columns.Add("Model", "Model");

            var assignColumn = new DataGridViewButtonColumn
            {
                HeaderText = "Přiřadit",
                Text = "Přiřadit",
                UseColumnTextForButtonValue = true,
                Width = 80
            };

            var detailColumn = new DataGridViewButtonColumn
            {
                HeaderText = "Detail",
                Text = "Detail",
                UseColumnTextForButtonValue = true,
                Width = 80
            };

            dgvCars.Columns.Add(assignColumn);
            dgvCars.Columns.Add(detailColumn);
            dgvCars.CellContentClick += DgvCars_CellContentClick;

            panelCarList.Controls.Add(dgvCars);
            this.Controls.Add(panelCarList);
        }

        private void InitializeDetailPanel()
        {
            detailPanel = new Panel
            {
                Size = new Size(860, 150),
                Location = new Point(20, 420),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray
            };

            lblProblemTitle = new Label
            {
                Text = "Popis problému:",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            txtProblemDescription = new TextBox
            {
                Location = new Point(10, 40),
                Size = new Size(830, 80),
                Multiline = true,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Arial", 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            detailPanel.Controls.Add(lblProblemTitle);
            detailPanel.Controls.Add(txtProblemDescription);
            this.Controls.Add(detailPanel);
        }

        private void InitializeInventoryPanel()
        {
            panelInventory = new Panel
            {
                Size = new Size(860, 500),
                Location = new Point(20, 50),
                Visible = false,
                AutoScroll = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(panelInventory);
        }

        private void InitializeToggleButton()
        {
            Button btnToggleView = new Button
            {
                Text = "Přepnout pohled",
                Location = new Point(20, 10),
                Size = new Size(150, 30)
            };
            btnToggleView.Click += BtnToggleView_Click;
            this.Controls.Add(btnToggleView);
        }

        private void LoadCarList()
        {
            dgvCars.Rows.Clear();
            List<Vozidlo> cars = mechanicDAO.GetAvailableCars();

            foreach (var car in cars)
            {
                dgvCars.Rows.Add(car.SPZ, car.Znacka, car.Model, "Přiřadit", "Detail");
            }
        }

        private void LoadMechanicInventory()
        {
            panelInventory.Controls.Clear();
            List<Oprava> repairs = repairDAO.GetRepairs()
                .Where(r => r.MechanikId == currentMechanic.MechanicID && r.Status != "completed")
                .ToList();

            int yOffset = 10;
            foreach (var repair in repairs)
            {
                Vozidlo car = vozidloDAO.GetCarByID(repair.VozidloId);
                if (car == null) continue;

                Panel repairPanel = new Panel
                {
                    Size = new Size(820, 220),
                    Location = new Point(10, yOffset),
                    BorderStyle = BorderStyle.FixedSingle
                };

                Label lblInfo = new Label
                {
                    Text = $"{car.Znacka} {car.Model} ({car.SPZ})",
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    Location = new Point(10, 10),
                    Size = new Size(400, 25)
                };

                Label lblProblem = new Label
                {
                    Text = $"Problém: {repair.PopisProblemu}",
                    Location = new Point(10, 40),
                    Size = new Size(400, 20)
                };

                TextBox txtRepairDescription = new TextBox
                {
                    Text = repair.PopisOpravy,
                    Location = new Point(10, 70),
                    Size = new Size(400, 25)
                };

                TextBox txtCost = new TextBox
                {
                    Text = repair.Cena.ToString(),
                    Location = new Point(420, 70),
                    Size = new Size(100, 25)
                };

                Button btnSave = new Button
                {
                    Text = "Uložit",
                    Location = new Point(530, 70),
                    Size = new Size(80, 30)
                };
                btnSave.Click += (sender, e) =>
                {
                    int cost;
                    if (int.TryParse(txtCost.Text, out cost))
                    {
                        repairDAO.SaveRepair(repair.Id, txtRepairDescription.Text, cost);
                        MessageBox.Show("Oprava uložena.", "Uloženo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Cena musí být číslo.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                Button btnComplete = new Button
                {
                    Text = "Dokončit",
                    Location = new Point(620, 70),
                    Size = new Size(80, 30)
                };
                btnComplete.Click += (sender, e) =>

                {

                    int cost;
                    if (int.TryParse(txtCost.Text, out cost))
                    {
                        repairDAO.SaveRepair(repair.Id, txtRepairDescription.Text, cost);
                        
                    }
                    
                    repairDAO.UpdateRepairStatus(repair.Id, "completed");
                    MessageBox.Show("Oprava dokončena.", "Dokončeno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMechanicInventory();
                };

                repairPanel.Controls.Add(lblInfo);
                repairPanel.Controls.Add(lblProblem);
                repairPanel.Controls.Add(txtRepairDescription);
                repairPanel.Controls.Add(txtCost);
                repairPanel.Controls.Add(btnSave);
                repairPanel.Controls.Add(btnComplete);

                panelInventory.Controls.Add(repairPanel);
                yOffset += 230;
            }
        }

        private void BtnToggleView_Click(object sender, EventArgs e)
        {
            showingInventory = !showingInventory;
            panelCarList.Visible = !showingInventory;
            panelInventory.Visible = showingInventory;

            if (showingInventory)
            {
                LoadMechanicInventory();
                detailPanel.Visible = false; // Skryjeme detail panel při zobrazení inventáře
            }
            else
            {
                LoadCarList();
                detailPanel.Visible = true; // Zobrazíme detail panel při zobrazení seznamu aut
            }
        }

        private void DgvCars_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string spz = dgvCars.Rows[e.RowIndex].Cells[0].Value.ToString();
                var car = vozidloDAO.GetCarByLicensePlate(spz);

                if (e.ColumnIndex == 3) // Přiřadit
                {
                    if (mechanicDAO.AssignCarToMechanic(currentMechanic.MechanicID, spz))
                    {
                        LoadCarList();
                    }
                }
                else if (e.ColumnIndex == 4) // Detail
                {
                    txtProblemDescription.Text = repairDAO.GetProblemDescription(car.Id);
                }
            }
        }
    }
}
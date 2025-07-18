using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Freelancer_Task.Models;
using Freelancer_Task.Services;

namespace Freelancer_Task.Forms
{
    [System.ComponentModel.DesignerCategory("")]
    public partial class ClientForm : Form
    {
        // UI Controls
        private TextBox txtName, txtEmail, txtPhone, txtCompany, txtSearch;
        private DataGridView dgvClients;
        private Button btnAdd, btnUpdate, btnDelete, btnSearch;

        // Services
        private readonly ClientRepository _clientRepo;
        private Client _currentClient;

        public ClientForm(ClientRepository clientRepo)
        {
            _clientRepo = clientRepo;
            BuildForm();
            SetupEventHandlers();
            LoadClients();
        }

        private void BuildForm()
        {
            // Form Configuration
            this.Text = "Client Management";
            this.ClientSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);

            // Input Fields
            CreateLabel("Name:", 20, 20);
            txtName = CreateTextBox(100, 20, 200);

            CreateLabel("Email:", 20, 60);
            txtEmail = CreateTextBox(100, 60, 200);

            CreateLabel("Phone:", 20, 100);
            txtPhone = CreateTextBox(100, 100, 200);

            CreateLabel("Company:", 20, 140);
            txtCompany = CreateTextBox(100, 140, 200);

            // Action Buttons
            btnAdd = CreateButton("Add", 320, 20, 80, 30);
            btnUpdate = CreateButton("Update", 320, 60, 80, 30);
            btnDelete = CreateButton("Delete", 320, 100, 80, 30);

            // Search Section
            CreateLabel("Search:", 420, 20);
            txtSearch = CreateTextBox(480, 20, 200);
            btnSearch = CreateButton("Search", 690, 20, 80, 25);

            // Data Grid
            dgvClients = new DataGridView
            {
                Location = new Point(20, 200),
                Size = new Size(750, 350),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.Fixed3D,
                BackgroundColor = SystemColors.Window
            };

            // Add all controls to form
            this.Controls.Add(dgvClients);
        }

        private void SetupEventHandlers()
        {
            btnAdd.Click += (s, e) => AddClient();
            btnUpdate.Click += (s, e) => UpdateClient();
            btnDelete.Click += (s, e) => DeleteClient();
            btnSearch.Click += (s, e) => SearchClients();
            dgvClients.SelectionChanged += (s, e) => LoadSelectedClient();
        }

        #region Helper Methods
        private Label CreateLabel(string text, int x, int y)
        {
            var lbl = new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true
            };
            this.Controls.Add(lbl);
            return lbl;
        }

        private TextBox CreateTextBox(int x, int y, int width)
        {
            var txt = new TextBox
            {
                Location = new Point(x, y),
                Width = width
            };
            this.Controls.Add(txt);
            return txt;
        }

        private Button CreateButton(string text, int x, int y, int width, int height)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                FlatStyle = FlatStyle.Standard
            };
            this.Controls.Add(btn);
            return btn;
        }
        #endregion

        #region Business Logic
        private void LoadClients()
        {
            try
            {
                dgvClients.DataSource = _clientRepo.GetAllClients();
                dgvClients.ClearSelection();
                _currentClient = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading clients: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSelectedClient()
        {
            if (dgvClients.SelectedRows.Count > 0)
            {
                _currentClient = (Client)dgvClients.SelectedRows[0].DataBoundItem;
                txtName.Text = _currentClient.Name;
                txtEmail.Text = _currentClient.Email;
                txtPhone.Text = _currentClient.Phone;
                txtCompany.Text = _currentClient.Company;
            }
        }

        private bool ValidateClientInputs()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name is required!", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Please enter a valid email address!", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ClearForm()
        {
            _currentClient = null;
            txtName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtCompany.Clear();
        }

        private void AddClient()
        {
            if (!ValidateClientInputs()) return;

            try
            {
                var client = new Client
                {
                    Name = txtName.Text,
                    Email = txtEmail.Text,
                    Phone = txtPhone.Text,
                    Company = txtCompany.Text,
                    CreatedDate = DateTime.Now
                };

                _clientRepo.AddClient(client);
                LoadClients();
                ClearForm();
                MessageBox.Show("Client added successfully!", "Success",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding client: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateClient()
        {
            if (_currentClient == null || !ValidateClientInputs()) return;

            try
            {
                _currentClient.Name = txtName.Text;
                _currentClient.Email = txtEmail.Text;
                _currentClient.Phone = txtPhone.Text;
                _currentClient.Company = txtCompany.Text;
                _currentClient.ModifiedDate = DateTime.Now;

                _clientRepo.UpdateClient(_currentClient);
                LoadClients();
                MessageBox.Show("Client updated successfully!", "Success",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating client: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteClient()
        {
            if (_currentClient == null) return;

            try
            {
                if (MessageBox.Show("Delete this client?", "Confirm",
                                  MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    _clientRepo.DeleteClient(_currentClient.Id);
                    LoadClients();
                    ClearForm();
                    MessageBox.Show("Client deleted successfully!", "Success",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting client: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchClients()
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();
                var results = _clientRepo.SearchClients(searchTerm);

                dgvClients.DataSource = results;

                if (results.Count == 0)
                {
                    MessageBox.Show("No clients found matching your search criteria.",
                                  "Information",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching clients: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
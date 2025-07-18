using Freelancer_Task.Models;
using Freelancer_Task.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using WorkLog = Freelancer_Task.Models.WorkLog;

namespace Freelancer_Task.Forms
{
    public partial class ProjectForm : Form
    {
        private readonly ProjectRepository _projectRepo;
        private readonly ClientRepository _clientRepo;
        private readonly TaskRepository _taskRepo;
        private Project _currentProject;

        // UI Controls
        private TextBox txtName, txtDescription, txtSearch;
        private ComboBox comboClient, comboStatus;
        private DateTimePicker dateStart, dateDeadline, dateEnd;
        private CheckBox chkEndDate;
        private DataGridView dataGridView;
        private Button btnAdd, btnUpdate, btnDelete, btnClear, btnSearch;

        public ProjectForm(ProjectRepository projectRepo, ClientRepository clientRepo, TaskRepository taskRepo)
        {
            _projectRepo = projectRepo;
            _clientRepo = clientRepo;
            _taskRepo = taskRepo;
            InitializeForm();
            LoadData();
            SetupEventHandlers();
            _taskRepo = taskRepo;
        }

        private void InitializeForm()
        {

            var actionPanel = new Panel
            {
                Location = new Point(750, 60),  // Right side of form
                Size = new Size(220, 180),
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle
            };

            var btnExport = new Button
            {
                Text = "Export Report",
                Location = new Point(10, 10),
                Size = new Size(200, 35),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat
            };
            btnExport.Click += (s, e) => ExportReport();

            var btnInvoice = new Button
            {
                Text = "Generate Invoice",
                Location = new Point(10, 55),
                Size = new Size(200, 35),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat
            };
            btnInvoice.Click += (s, e) => GenerateInvoice();


            // Form Configuration
            this.Text = "Project Management";
            this.ClientSize = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);

            // Labels
            var lblName = new Label { Text = "Name:", Location = new Point(20, 20), AutoSize = true };
            var lblDescription = new Label { Text = "Description:", Location = new Point(20, 60), AutoSize = true };
            var lblClient = new Label { Text = "Client:", Location = new Point(20, 100), AutoSize = true };
            var lblStartDate = new Label { Text = "Start Date:", Location = new Point(20, 140), AutoSize = true };
            var lblDeadline = new Label { Text = "Deadline:", Location = new Point(20, 180), AutoSize = true };
            var lblStatus = new Label { Text = "Status:", Location = new Point(20, 220), AutoSize = true };
            var lblEndDate = new Label { Text = "End Date:", Location = new Point(20, 260), AutoSize = true };
            var lblSearch = new Label { Text = "Search:", Location = new Point(500, 20), AutoSize = true };

            // Input Controls
            txtName = new TextBox { Location = new Point(150, 20), Width = 300 };
            txtDescription = new TextBox { Location = new Point(150, 60), Width = 300, Multiline = true, Height = 60 };
            comboClient = new ComboBox
            {
                Location = new Point(150, 100),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            dateStart = new DateTimePicker { Location = new Point(150, 140), Width = 150 };
            dateDeadline = new DateTimePicker { Location = new Point(150, 180), Width = 150 };
            comboStatus = new ComboBox { Location = new Point(150, 220), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            chkEndDate = new CheckBox { Text = "Set End Date", Location = new Point(150, 260), AutoSize = true };
            dateEnd = new DateTimePicker { Location = new Point(280, 260), Width = 150, Enabled = false };
            txtSearch = new TextBox { Location = new Point(560, 20), Width = 300 };

            // DataGridView
            dataGridView = new DataGridView
            {
                Location = new Point(20, 300),
                Size = new Size(960, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Buttons
            btnAdd = new Button { Text = "Add", Location = new Point(500, 60), Size = new Size(100, 35) };
            btnUpdate = new Button { Text = "Update", Location = new Point(500, 100), Size = new Size(100, 35), Enabled = false };
            btnDelete = new Button { Text = "Delete", Location = new Point(500, 140), Size = new Size(100, 35), Enabled = false };
            btnClear = new Button { Text = "Clear", Location = new Point(500, 180), Size = new Size(100, 35) };
            btnSearch = new Button { Text = "Search", Location = new Point(870, 20), Size = new Size(100, 25) };


            var btnSummary = new Button
            {
                Text = "View Summary",
                Location = new Point(10, 100), // Positioned below the other buttons in action panel
                Size = new Size(200, 35),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            btnSummary.Click += BtnSummary_Click;
     
            // Add Controls to Form
            this.Controls.AddRange(new Control[]
            {
                lblName, lblDescription, lblClient, lblStartDate, lblDeadline, lblStatus, lblEndDate, lblSearch,
                txtName, txtDescription, comboClient, dateStart, dateDeadline, comboStatus, chkEndDate, dateEnd, txtSearch,
                dataGridView, btnAdd, btnUpdate, btnDelete, btnClear, btnSearch
            });

            actionPanel.Controls.AddRange(new Control[] { btnExport, btnInvoice, btnSummary });

            this.Controls.Add(actionPanel);
            actionPanel.BringToFront();
        }

        private void LoadData()
        {
            // Load Clients
            comboClient.DataSource = _clientRepo.GetAllClients();
            comboClient.DisplayMember = "Name";
            comboClient.ValueMember = "Id";

            // Load Statuses
            comboStatus.DataSource = Enum.GetValues(typeof(ProjectStatus));

            // Load Projects
            dataGridView.DataSource = _projectRepo.GetAllProjects();
            dataGridView.Columns["Client"].Visible = false; // Hide navigation property
        }
        public class ProjectSummary
        {
            public int TotalTasks { get; set; }
            public int CompletedTasks { get; set; }
            public TimeSpan TotalTimeSpent { get; set; }
            public decimal CompletionPercentage =>
                TotalTasks > 0 ? (CompletedTasks * 100m / TotalTasks) : 0;
        }

        private void ShowProjectSummary(int projectId)
        {
            var tasks = _taskRepo.GetTasksByProject(projectId);
            var summary = new ProjectSummary
            {
                TotalTasks = tasks.Count,
                CompletedTasks = tasks.Count(t => t.Status == PTaskStatus.Done),
                TotalTimeSpent = new TimeSpan(tasks.Sum(t => t.TotalTimeSpent?.Ticks ?? 0))
            };

            MessageBox.Show(
                $@"Project Summary:
        Completed: {summary.CompletionPercentage:0}%
        Total Time: {summary.TotalTimeSpent:hh\:mm}
        Tasks: {summary.CompletedTasks}/{summary.TotalTasks}",
                "Project Summary");
        }
        private void SetupEventHandlers()
        {
            // CheckBox Toggle
            chkEndDate.CheckedChanged += (s, e) => dateEnd.Enabled = chkEndDate.Checked;

            // Buttons
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += BtnClear_Click;
            btnSearch.Click += BtnSearch_Click;

            // DataGridView Selection
            dataGridView.SelectionChanged += (s, e) =>
            {
                if (dataGridView.SelectedRows.Count > 0)
                {
                    _currentProject = (Project)dataGridView.SelectedRows[0].DataBoundItem;
                    txtName.Text = _currentProject.Name;
                    txtDescription.Text = _currentProject.Description;
                    comboClient.SelectedValue = _currentProject.ClientId;
                    dateStart.Value = _currentProject.StartDate;
                    dateDeadline.Value = _currentProject.Deadline;
                    comboStatus.SelectedItem = _currentProject.Status;
                    chkEndDate.Checked = _currentProject.EndDate.HasValue;
                    if (_currentProject.EndDate.HasValue) dateEnd.Value = _currentProject.EndDate.Value;

                    btnAdd.Enabled = false;
                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
                }
            };
        }

        // CRUD Operations
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            var project = new Project
            {
                Name = txtName.Text?.Trim() ?? throw new Exception("Project name is required"),
                Description = txtDescription.Text?.Trim(),
                StartDate = dateStart.Value,
                Deadline = dateDeadline.Value,
                Status = comboStatus.SelectedItem is ProjectStatus status
             ? status
             : throw new Exception("Invalid status selection"),
                ClientId = comboClient.SelectedValue is int id
               ? id
               : throw new Exception("No client selected"),
                EndDate = chkEndDate.Checked ? dateEnd.Value : (DateTime?)null
            };

            _projectRepo.AddProject(project);
            _projectRepo.SaveChanges();
            LoadData();
            MessageBox.Show("Project added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_currentProject == null || !ValidateInputs()) return;

            _currentProject.Name = txtName.Text;
            _currentProject.Description = txtDescription.Text;
            _currentProject.StartDate = dateStart.Value;
            _currentProject.Deadline = dateDeadline.Value;
            _currentProject.Status = (ProjectStatus)comboStatus.SelectedItem;
            _currentProject.ClientId = (int)comboClient.SelectedValue;
            _currentProject.EndDate = chkEndDate.Checked ? dateEnd.Value : null;

            _projectRepo.UpdateProject(_currentProject);
            _projectRepo.SaveChanges();
            LoadData();
            MessageBox.Show("Project updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_currentProject == null) return;

            var confirm = MessageBox.Show("Delete this project?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                _projectRepo.DeleteProject(_currentProject.Id);
                _projectRepo.SaveChanges();
                LoadData();
                MessageBox.Show("Project deleted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtName.Clear();
            txtDescription.Clear();
            comboClient.SelectedIndex = -1;
            dateStart.Value = DateTime.Now;
            dateDeadline.Value = DateTime.Now.AddDays(7);
            comboStatus.SelectedIndex = 0;
            chkEndDate.Checked = false;
            dataGridView.ClearSelection();
            _currentProject = null;
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            var results = _projectRepo.GetAllProjects()
                .Where(p => p.Name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase))
                .ToList();

            dataGridView.DataSource = results;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name is required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (dateDeadline.Value < dateStart.Value)
            {
                MessageBox.Show("Deadline must be after start date!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private ProjectSummary GetProjectSummary(int projectId)
        {
            var tasks = _taskRepo.GetTasksByProject(projectId).ToList();

            return new ProjectSummary
            {
                TotalTasks = tasks.Count,
                CompletedTasks = tasks.Count(t => t.Status == PTaskStatus.Done),
                TotalTimeSpent = TimeSpan.FromTicks(tasks
                    .Where(t => t.TotalTimeSpent.HasValue)
                    .Sum(t => t.TotalTimeSpent.Value.Ticks))
            };
        }
        private void BtnSummary_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a project first!");
                return;
            }

            var selectedProject = (Project)dataGridView.SelectedRows[0].DataBoundItem;
            var summary = GetProjectSummary(selectedProject.Id);

            // Create aligned summary text
            string[] summaryLines = {
        $"📊 Project: {selectedProject.Name}",
        $"✅ Completion: {summary.CompletionPercentage:0}%",
        $"⏱️ Time Spent: {summary.TotalTimeSpent:hh\\:mm}",
        $"📝 Tasks: {summary.CompletedTasks}/{summary.TotalTasks}"
    };

            // Calculate maximum line length for centering
            int maxLength = summaryLines.Max(line => line.Length);
            string alignedSummary = string.Join(Environment.NewLine,
                summaryLines.Select(line => line.PadRight(maxLength)));

            MessageBox.Show(alignedSummary,
                          "Project Summary",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Information);
        }

        private List<WorkLog> GetWorkLogs()
        {
            if (dataGridView.SelectedRows.Count == 0)
                return new List<WorkLog>();

            var projectId = ((Project)dataGridView.SelectedRows[0].DataBoundItem).Id;
            var tasks = _taskRepo.GetTasksByProject(projectId);

            return tasks.Select(t => new WorkLog
            {
                Date = t.TimerStart ?? DateTime.Now, // Handle null TimerStart
                ClientName = t.Project?.Client?.Name ?? "N/A",
                ProjectName = t.Project?.Name ?? "N/A",
                TaskTitle = t.Title,
                TimeSpent = t.TotalTimeSpent ?? TimeSpan.Zero,
                Status = (PTaskStatus)t.Status
            }).ToList();
        }


        private void ExportReport()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|PDF Files (*.pdf)|*.pdf",
                Title = "Export Report"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                var workLogs = GetWorkLogs();
                var exporter = new ReportExport();

                if (saveDialog.FilterIndex == 1)
                    exporter.ExportToCsv(workLogs, saveDialog.FileName);
                else
                    exporter.ExportToPdf(workLogs, saveDialog.FileName);

                MessageBox.Show("Report exported successfully!");
            }
        }

        private void GenerateInvoice()
        {
            if (dataGridView.SelectedRows.Count == 0) return;

            var project = (Project)dataGridView.SelectedRows[0].DataBoundItem;
            var inputDialog = new InputDialog("Enter Hourly Rate", "Invoice Generation");

            if (inputDialog.ShowDialog() == DialogResult.OK &&
                decimal.TryParse(inputDialog.InputValue, out decimal rate))
            {
                var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        new InvoiceService().GenerateInvoice(project, rate, folderDialog.SelectedPath);
                        MessageBox.Show($"Invoice generated in:\n{folderDialog.SelectedPath}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }

    }


}
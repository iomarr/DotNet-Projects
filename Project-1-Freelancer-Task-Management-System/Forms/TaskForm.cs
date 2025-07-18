using Freelancer_Task.Models;
using Freelancer_Task.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Freelancer_Task.Forms
{
    public partial class TaskForm : Form
    {
        private readonly TaskRepository _taskRepo;
        private readonly ProjectRepository _projectRepo;
        private PTask _currentTask;
        private DateTime? _timerStart;

        // UI Controls
        private TextBox txtTitle, txtDescription, txtTimeSpent, txtSearch;
        private ComboBox comboProject, comboStatus;
        private DateTimePicker dateDue;
        private DataGridView dataGridView;
        private Button btnAdd, btnUpdate, btnDelete, btnClear, btnSearch;
        private Button btnStartTimer, btnStopTimer, btnLogTime;
        private TextBox txtHours, txtMinutes;
        private Label lblTotalTime;



        public TaskForm(TaskRepository taskRepo, ProjectRepository projectRepo)
        {
            _taskRepo = taskRepo;
            _projectRepo = projectRepo;
            InitializeForm();
            LoadData();
            SetupEventHandlers();
        }

        private void InitializeForm()
        {
            // Form Configuration
            this.Text = "Task Management";
            this.ClientSize = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);

            // Labels
            var lblTitle = new Label { Text = "Title:", Location = new Point(20, 20), AutoSize = true };
            var lblDescription = new Label { Text = "Description:", Location = new Point(20, 60), AutoSize = true };
            var lblProject = new Label { Text = "Project:", Location = new Point(20, 100), AutoSize = true };
            var lblDueDate = new Label { Text = "Due Date:", Location = new Point(20, 140), AutoSize = true };
            var lblStatus = new Label { Text = "Status:", Location = new Point(20, 180), AutoSize = true };
            var lblTimeSpent = new Label { Text = "Time Spent:", Location = new Point(20, 220), AutoSize = true };
            var lblSearch = new Label { Text = "Search:", Location = new Point(500, 20), AutoSize = true };

            // Input Controls
            txtTitle = new TextBox { Location = new Point(150, 20), Width = 300 };
            txtDescription = new TextBox { Location = new Point(150, 60), Width = 300, Multiline = true, Height = 60 };
            comboProject = new ComboBox { Location = new Point(150, 100), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            dateDue = new DateTimePicker { Location = new Point(150, 140), Width = 150 };
            comboStatus = new ComboBox { Location = new Point(150, 180), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            txtTimeSpent = new TextBox { Location = new Point(150, 220), Width = 150, ReadOnly = true };
            txtSearch = new TextBox { Location = new Point(560, 20), Width = 300 };

            // Timer Buttons
            btnStartTimer = new Button { Text = "Start Timer", Location = new Point(320, 220), Size = new Size(100, 25) };
            btnStopTimer = new Button { Text = "Stop Timer", Location = new Point(430, 220), Size = new Size(100, 25), Enabled = false };

            // DataGridView
            dataGridView = new DataGridView
            {
                Location = new Point(20, 260),
                Size = new Size(960, 350),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Action Buttons
            btnAdd = new Button { Text = "Add", Location = new Point(500, 60), Size = new Size(100, 35) };
            btnUpdate = new Button { Text = "Update", Location = new Point(500, 100), Size = new Size(100, 35), Enabled = false };
            btnDelete = new Button { Text = "Delete", Location = new Point(500, 140), Size = new Size(100, 35), Enabled = false };
            btnClear = new Button { Text = "Clear", Location = new Point(500, 180), Size = new Size(100, 35) };
            btnSearch = new Button { Text = "Search", Location = new Point(870, 20), Size = new Size(100, 25) };

            // Add Controls to Form
            this.Controls.AddRange(new Control[]
            {
                lblTitle, lblDescription, lblProject, lblDueDate, lblStatus, lblTimeSpent, lblSearch,
                txtTitle, txtDescription, comboProject, dateDue, comboStatus, txtTimeSpent, txtSearch,
                btnStartTimer, btnStopTimer, dataGridView,
                btnAdd, btnUpdate, btnDelete, btnClear, btnSearch
            });
        }

        private void LoadData()
        {
            // Load Projects
            comboProject.DataSource = _projectRepo.GetAllProjects();
            comboProject.DisplayMember = "Name";
            comboProject.ValueMember = "Id";

            // Load Statuses
            comboStatus.DataSource = Enum.GetValues(typeof(PTaskStatus));

            // Load Tasks
            RefreshTaskGrid();
        }

        private void RefreshTaskGrid()
        {
            if (comboProject.SelectedValue == null)
            {
                dataGridView.DataSource = null;
                return;
            }

            dataGridView.DataSource = _taskRepo.GetTasksByProject((int)comboProject.SelectedValue);
            dataGridView.Columns["Project"].Visible = false;
        }

        private void SetupEventHandlers()
        {
            // Project Selection Changed
            comboProject.SelectedIndexChanged += (s, e) => RefreshTaskGrid();

            // Timer Buttons
            btnStartTimer.Click += (s, e) =>
            {
                _timerStart = DateTime.Now;
                btnStartTimer.Enabled = false;
                btnStopTimer.Enabled = true;
            };

            btnStopTimer.Click += (s, e) =>
            {
                if (_timerStart.HasValue)
                {
                    var timeSpent = DateTime.Now - _timerStart.Value;
                    _taskRepo.LogTime(_currentTask.Id, timeSpent);
                    _timerStart = null;
                    btnStartTimer.Enabled = true;
                    btnStopTimer.Enabled = false;
                    UpdateTimeDisplay();
                }
            };

            // CRUD Buttons
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += BtnClear_Click;
            btnSearch.Click += BtnSearch_Click;

            // Grid Selection
            dataGridView.SelectionChanged += (s, e) =>
            {
                if (dataGridView.SelectedRows.Count > 0)
                {
                    _currentTask = (PTask)dataGridView.SelectedRows[0].DataBoundItem;
                    txtTitle.Text = _currentTask.Title;
                    txtDescription.Text = _currentTask.Description;
                    comboProject.SelectedValue = _currentTask.ProjectId;
                    dateDue.Value = _currentTask.DueDate;
                    comboStatus.SelectedItem = _currentTask.Status;
                    UpdateTimeDisplay();

                    btnAdd.Enabled = false;
                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
                }
            };
        }

        private void UpdateTimeDisplay()
        {
            txtTimeSpent.Text = _currentTask?.TotalTimeSpent?.ToString(@"hh\:mm\:ss") ?? "00:00:00";
        }

        // CRUD Operations
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            var task = new PTask
            {
                Title = txtTitle.Text,
                Description = txtDescription.Text,
                DueDate = dateDue.Value,
                Status = (PTaskStatus)comboStatus.SelectedItem,
                ProjectId = (int)comboProject.SelectedValue
            };

            _taskRepo.AddTask(task);
            RefreshTaskGrid();
            MessageBox.Show("Task added successfully!", "Success");
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_currentTask == null || !ValidateInputs()) return;

            _currentTask.Title = txtTitle.Text;
            _currentTask.Description = txtDescription.Text;
            _currentTask.DueDate = dateDue.Value;
            _currentTask.Status = (PTaskStatus)comboStatus.SelectedItem;
            _currentTask.ProjectId = (int)comboProject.SelectedValue;

            _taskRepo.UpdateTask(_currentTask);
            RefreshTaskGrid();
            MessageBox.Show("Task updated successfully!", "Success");
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_currentTask == null) return;

            var confirm = MessageBox.Show("Delete this task?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                _taskRepo.DeleteTask(_currentTask.Id);
                RefreshTaskGrid();
                MessageBox.Show("Task deleted!", "Success");
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtTitle.Clear();
            txtDescription.Clear();
            comboProject.SelectedIndex = 0;
            dateDue.Value = DateTime.Now.AddDays(1);
            comboStatus.SelectedIndex = 0;
            txtTimeSpent.Text = "00:00:00";
            dataGridView.ClearSelection();
            _currentTask = null;
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            var tasks = _taskRepo.GetTasksByProject((int)comboProject.SelectedValue)
                .Where(t => t.Title.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase))
                .ToList();

            dataGridView.DataSource = tasks;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Title is required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }


    }
}
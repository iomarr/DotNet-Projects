using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Freelancer_Task.Forms
{
    public partial class MainForm : Form
    {
        private readonly IServiceProvider _services;

        public MainForm(IServiceProvider services)
        {
            _services = services;
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Freelancer System";
            this.ClientSize = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            var btnClients = new Button { Text = "Clients", Size = new Size(150, 50), Location = new Point(125, 50) };
            var btnProjects = new Button { Text = "Projects", Size = new Size(150, 50), Location = new Point(125, 120) };
            var btnTasks = new Button { Text = "Tasks", Size = new Size(150, 50), Location = new Point(125, 190) };

            btnClients.Click += (s, e) => OpenForm<ClientForm>();
            btnProjects.Click += (s, e) => OpenForm<ProjectForm>();
            btnTasks.Click += (s, e) => OpenForm<TaskForm>();

            this.Controls.AddRange(new Control[] { btnClients, btnProjects, btnTasks });
        }

        private void OpenForm<T>() where T : Form
        {
            var form = _services.GetRequiredService<T>();
            form.Show();
            this.Hide();
            form.FormClosed += (s, e) => this.Show();
        }
    }
}
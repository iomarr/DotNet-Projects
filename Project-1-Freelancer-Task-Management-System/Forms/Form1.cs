using Freelancer_Task.Data;
using Freelancer_Task.Service;
using Freelancer_Task.Services;
using Microsoft.CodeAnalysis;
using Freelancer_Task.Forms;


namespace Freelancer_Task
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var btnOpenClientForm = new Button
            {
                Text = "Manage Clients",
                Location = new Point(50, 50),
                Size = new Size(150, 40),
                BackColor = Color.LightBlue
            };

            btnOpenClientForm.Click += (s, e) =>
            {
                var context = new FreelancerContext();
                var clientRepo = new ClientRepository(context);

                var clientForm = new ClientForm(clientRepo);
                clientForm.ShowDialog();
            };

            this.Controls.Add(btnOpenClientForm);
        }

        private void btnOpenClientForm_Click(object sender, EventArgs e)
        {
            var clientForm = new ClientForm(new ClientRepository(new FreelancerContext())
            );
            clientForm.ShowDialog();
        }
    }


}

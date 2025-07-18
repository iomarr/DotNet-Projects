using System.Windows.Forms;

namespace Freelancer_Task.Forms
{
    public partial class InputDialog : Form
    {
        public string InputValue { get; private set; }

        public InputDialog(string prompt, string title)
        {
            InitializeComponents(prompt, title);
        }

        private void InitializeComponents(string prompt, string title)
        {
            this.Text = title;
            this.ClientSize = new Size(300, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            var lblPrompt = new Label
            {
                Text = prompt,
                Location = new Point(20, 20),
                AutoSize = true
            };

            var txtInput = new TextBox
            {
                Location = new Point(20, 50),
                Width = 260
            };

            var btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(120, 80)
            };

            btnOk.Click += (s, e) =>
            {
                InputValue = txtInput.Text;
                this.Close();
            };

            this.Controls.AddRange(new Control[] { lblPrompt, txtInput, btnOk });
            this.AcceptButton = btnOk;
        }
    }
}
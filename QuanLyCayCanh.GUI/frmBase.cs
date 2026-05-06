using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace QuanLyCayCanh.GUI
{
    public partial class frmBase : Form
    {
        protected Panel pnlHeader;
        protected Label lblHeaderTitle;
        protected Button btnClose;

        public frmBase()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 243, 247);
            this.Size = new Size(1000, 650);
            this.Font = new Font("Segoe UI", 10);
            this.StartPosition = FormStartPosition.CenterScreen;

            // build header UI
            CreateHeader();
        }

        private void CreateHeader()
        {
            if (pnlHeader != null) return;

            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = Color.FromArgb(33, 47, 61)
            };

            lblHeaderTitle = new Label
            {
                ForeColor = Color.White,
                Location = new Point(15, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Text = "FORM TITLE"
            };

            btnClose = new Button
            {
                Text = "X",
                Dock = DockStyle.Right,
                Width = 50,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.Red;
            btnClose.Click += (s, e) => this.Close();

            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Controls.Add(btnClose);

            this.Controls.Add(pnlHeader);

            pnlHeader.MouseDown += Header_MouseDown;
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void Header_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void frmBase_Load(object sender, EventArgs e)
        {
            // no-op default. Derived forms can subscribe to Load or override OnLoad.
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // additional runtime init if needed
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceInvadersIteration1
{
    public partial class FormStart : Form
    {
        public string PlayerName { get; private set; } = "";
        public FormStart()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
            this.WindowState = FormWindowState.Maximized;
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            g.DrawImage(Properties.Resources.image,
                0, 0, ClientSize.Width, ClientSize.Height);

            g.DrawString("ROOTIN' TOOTIN' COWBOY SHOOTIN'", new Font("MS Gothic", 60F, FontStyle.Bold),
                         Brushes.Yellow,
                         new PointF(ClientSize.Width / 2 - 650, ClientSize.Height / 2 - 200));

            g.DrawString("ENTER YOUR NAME:", new Font("MS Gothic", 25F),
                         Brushes.White,
                         new PointF(ClientSize.Width / 2 - 150, ClientSize.Height / 2 - 60));
        }

        private void FormStart_Load(object sender, EventArgs e)
        {

            TextBox nameBox = new TextBox
            {
                Location = new Point(ClientSize.Width / 2 - 150, ClientSize.Height / 2),
                Size = new Size(300, 40),
                Font = new Font("MS Gothic", 16F),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                MaxLength = 15,
            };

            Button startButton = new Button();
            startButton.Location = new Point(ClientSize.Width / 2 - 100, ClientSize.Height / 2 + 60);
            startButton.Size = new Size(200, 50);
            startButton.Font = new Font("MS Gothic", 16F);
            startButton.Text = "START";
            startButton.BackColor = Color.FromArgb(92, 70, 154);
            startButton.ForeColor = Color.White;
            this.Controls.Add(startButton);


            Button highscoreButton = new Button();
            highscoreButton.Location = new Point(ClientSize.Width / 2 - 100, ClientSize.Height / 2 + 120);
            highscoreButton.Size = new Size(200, 50);
            highscoreButton.Font = new Font("MS Gothic", 16F);
            highscoreButton.Text = "HIGH SCORES";
            highscoreButton.BackColor = Color.FromArgb(92, 70, 154);
            highscoreButton.ForeColor = Color.White;
            this.Controls.Add(highscoreButton);

            startButton.Click += (s, ev) =>
            {
                if (nameBox.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter a name!", "Error");
                    return;
                }
                PlayerName = nameBox.Text.Trim();
                FileManager.FileCreate();
                FileManager.PlayerName = PlayerName;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            highscoreButton.Click += (s, ev) =>
            {
                formLeaderboard leaderboardForm = new formLeaderboard();
                leaderboardForm.ShowDialog();
            };

            this.Load += (s, ev) =>
            {
                nameBox.Location = new Point(ClientSize.Width / 2 - 150, ClientSize.Height / 2);
                startButton.Location = new Point(ClientSize.Width / 2 - 100, ClientSize.Height / 2 + 60);
                highscoreButton.Location = new Point(ClientSize.Width / 2 - 100, ClientSize.Height / 2 + 120);
            };

            this.Controls.Add(nameBox);

        }
    }
}

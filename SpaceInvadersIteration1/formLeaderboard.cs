using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceInvadersIteration1
{
    public partial class formLeaderboard : Form
    {
        public formLeaderboard()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
            this.WindowState = FormWindowState.Maximized;
            this.Text = "High Scores";
        }

        private void formLeaderboard_Load_1(object sender, EventArgs e)
        {
            Label title = new Label();
            title.Text = "HIGH SCORES";
            title.Font = new Font("MS Gothic", 40F, FontStyle.Bold);
            title.ForeColor = Color.Yellow;
            title.AutoSize = true;
            title.Location = new Point(ClientSize.Width / 2 - 200, 80);

            this.Controls.Add(title);

            var scores = FileManager.LoadScores();
            for (int i = 0; i < scores.Count; i++)
            {
                Label entry = new Label
                {
                    Text = $"{i + 1}.   {scores[i].name,-15} {scores[i].score}",
                    Font = new Font("MS Gothic", 24F),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(ClientSize.Width / 2 - 200, 180 + (i * 50))
                };
                this.Controls.Add(entry);
            }

            Button backButton = new Button();
            backButton.Text = "BACK";
            backButton.Font = new Font("MS Gothic", 16F);
            backButton.BackColor = Color.FromArgb(92, 70, 154);
            backButton.ForeColor = Color.White;
            backButton.Size = new Size(200, 50);
            backButton.Location = new Point(ClientSize.Width / 2 - 100, 180 + (11 * 50));

            backButton.Click += (s, ev) => this.Close();
            this.Controls.Add(backButton);
        }
    }
}

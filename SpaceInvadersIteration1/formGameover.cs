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
    public partial class formGameover : Form
    {
        private int intCurrentScore;
        public formGameover(int score)
        {
            InitializeComponent();
            this.BackColor = Color.Black;
            this.WindowState = FormWindowState.Maximized;
            this.intCurrentScore = score;

            Label lblgameOver = new Label();
            lblgameOver.Text = "GAME OVER";
            lblgameOver.Font = new Font("MS Gothic", 60F, FontStyle.Bold);
            lblgameOver.ForeColor = Color.Red;
            lblgameOver.Location = new Point(ClientSize.Width / 2 - 230, ClientSize.Height / 2 - 150);
            lblgameOver.AutoSize = true;
            this.Controls.Add(lblgameOver);

            Label lblScore = new Label();
            lblScore.Text = $"FINAL SCORE: {intCurrentScore}";
            lblScore.Font = new Font("MS Gothic", 30F);
            lblScore.ForeColor = Color.White;
            lblScore.Location = new Point(ClientSize.Width / 2 - 180, ClientSize.Height / 2 - 40);
            lblScore.AutoSize = true;
            this.Controls.Add(lblScore);

            Label lblRestart = new Label();
            lblRestart.Text = "PRESS R TO RESTART";
            lblRestart.Font = new Font("MS Gothic", 30F);
            lblRestart.ForeColor = Color.White;
            lblRestart.Location = new Point(ClientSize.Width / 2 - 210, ClientSize.Height / 2 - 200);
            lblRestart.AutoSize = true;
            this.Controls.Add(lblRestart);

            Button btnHighscore = new Button();
            btnHighscore.Text = "HIGH SCORES";
            btnHighscore.Font = new Font("MS Gothic", 16F);
            btnHighscore.BackColor = Color.FromArgb(92, 70, 154);
            btnHighscore.ForeColor = Color.White;
            btnHighscore.Location = new Point(ClientSize.Width / 2 - 120, ClientSize.Height / 2 + 60);
            btnHighscore.Size = new Size(200, 50);
            this.Controls.Add(btnHighscore);

            btnHighscore.Click += (s, ev) =>
            {
                formLeaderboard leaderboardForm = new formLeaderboard();
                leaderboardForm.ShowDialog();
            };

            this.Shown += (s, ev) =>
            {
                lblgameOver.Location = new Point(ClientSize.Width / 2 - 230, ClientSize.Height / 2 - 150);
                lblScore.Location = new Point(ClientSize.Width / 2 - 180, ClientSize.Height / 2 - 40);
                btnHighscore.Location = new Point(ClientSize.Width / 2 - 120, ClientSize.Height / 2 + 60);
                lblRestart.Location = new Point(ClientSize.Width / 2 - 210, ClientSize.Height / 2 - 200);
            };

            this.KeyPreview = true;
            this.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.R)
                    this.Close();
            };
        }

        private void formGameover_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;
                }
            }
        }    
    }


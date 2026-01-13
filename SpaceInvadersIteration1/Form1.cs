using System.Numerics;

namespace SpaceInvadersIteration1
{
    public partial class Form1 : Form
    {
        Label
            lblCurrentScore;

        int
            intHighScore = 0,
            intCurrentScore = 0,
            playerSpeed = 8,
            enemySpeed = 40,
            enemySpacing = 30,
            enemyWidth = 70,
            enemyHeight = 70,
            gcDisposeCount = 0,
            shootCooldown = 0;

        enum Direction { Left, Right }

        bool
            Up,
            Down,
            Left,
            Right,
            canShoot,
            gameEnd;

        Direction direction = Direction.Left;

        Rectangle
            rectPlayer = new Rectangle(),
            leftPanel = new Rectangle(),
            rightPanel = new Rectangle(),
            powerBarMax = new Rectangle(),
            powerBarCurrent = new Rectangle(),
            defence1 = new Rectangle(),
            defence2 = new Rectangle(),
            defence3 = new Rectangle();

        List<Rectangle> Enemies = new List<Rectangle>();
        List<Projectile> Projectiles = new List<Projectile>();
        List<Projectile> disposeProjectiles = new List<Projectile>();
        List<Rectangle> disposeEnemy = new List<Rectangle>();
        public int[] enemyHP = new int[0];
        private readonly object enemyLock = new();


        Thread moveenemyis;

        public Form1()
        {
            InitializeComponent();

            rectPlayer.Width = 60;
            rectPlayer.Height = 60;
            rectPlayer.Location = new Point(ClientSize.Width / 2, 965);

            leftPanel.Width = ClientSize.Width / 6;
            leftPanel.Height = ClientSize.Height;
            leftPanel.Location = new Point(0, 0);

            rightPanel.Width = ClientSize.Width / 6;
            rightPanel.Height = ClientSize.Height;
            rightPanel.Location = new Point(ClientSize.Width - rightPanel.Width, ClientSize.Height - rightPanel.Height);

            powerBarMax.Width = ClientSize.Width / 12;
            powerBarMax.Height = 900;
            powerBarMax.Location = new Point(ClientSize.Width - 230, 30);

            powerBarCurrent.Width = powerBarMax.Width - 10;
            powerBarCurrent.Height = powerBarMax.Height - 10;
            powerBarCurrent.Location = new Point(powerBarMax.X + (powerBarMax.Width - powerBarCurrent.Width) / 2,
                powerBarMax.Y + (powerBarMax.Height - powerBarCurrent.Height) / 2);

            defence3.Width = 180;
            defence3.Height = 100;
            defence3.Location = new Point(ClientSize.Width - (rightPanel.Width * 2), ClientSize.Height - (ClientSize.Height / 3));

            defence2.Width = 180;
            defence2.Height = 100;
            defence2.Location = new Point(ClientSize.Width / 2 - (defence2.Width / 2), ClientSize.Height - (ClientSize.Height / 3));

            defence1.Width = 180;
            defence1.Height = 100;
            defence1.Location = new Point(leftPanel.Width + (leftPanel.Width / 2), ClientSize.Height - (ClientSize.Height / 3));

            lblCurrentScore.Location = new Point(25, 150);
            lblCurrentScore.Name = "currentScore";
            lblCurrentScore.Size = new Size(280, 200);
            lblCurrentScore.TabIndex = 0;
            lblCurrentScore.ForeColor = Color.White;
            lblCurrentScore.BackColor = Color.Transparent;
            lblCurrentScore.Font = new Font("MS Gothic", 25F);
            lblCurrentScore.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblCurrentScore);

            lblHighScore.BackColor = Color.Transparent;
            lblHighScore.Font = new Font("MS Gothic", 25F);
            lblHighScore.ForeColor = Color.White;
            lblHighScore.Location = new Point(70, 30);
            lblHighScore.Name = "lblHighScore";
            lblHighScore.Size = new Size(190, 100);
            lblHighScore.TabIndex = 0;
            lblHighScore.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblHighScore);

            PictureBox playerHeart1 = new PictureBox();

            moveenemyis = new Thread(enemyMovement);
            moveenemyis.Start();

            timer1.Enabled = true;
            timer1.Start();

            FormWindowState formWindowState = FormWindowState.Maximized;

            this.BackColor = Color.Black;
            this.DoubleBuffered = true;

            int
                startX = leftPanel.Width + 10,
                startY = 10,
                numRows = 3,
                numCols = 8;

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    int x = startX + col * (enemyWidth + enemySpacing);
                    int y = startY + row * (enemyHeight + enemySpacing);
                    Enemies.Add(new Rectangle(x, y, enemyWidth, enemyHeight));
                }
            }

            Array.Resize(ref enemyHP, Enemies.Count);
            Array.Fill(enemyHP, 2);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Brush b = new SolidBrush(Color.FromArgb(92, 70, 154));
            Brush b2 = new SolidBrush(Color.FromArgb(136, 76, 167));
            Brush b3 = new SolidBrush(Color.Black);
            Brush b4 = new SolidBrush(Color.Red);
            Brush b5 = new SolidBrush(Color.FromArgb(255, 253, 187));
            Brush b6 = new SolidBrush(Color.FromArgb(0, 150, 255));
            g.FillRectangle(b, leftPanel);
            g.FillRectangle(b, rightPanel);
            g.FillRectangle(b2, rectPlayer);
            g.FillRectangle(b3, powerBarMax);
            g.FillRectangle(b5, powerBarCurrent);
            g.FillRectangle(b6, defence1);
            g.FillRectangle(b6, defence2);
            g.FillRectangle(b6, defence3);

            lock (enemyLock)
            {
                foreach (Rectangle Enemy in Enemies)
                {
                    g.FillRectangle(b4, Enemy);
                }
            }

            lock (enemyLock)
            {
                foreach (Projectile p in Projectiles)
                {
                    g.FillRectangle(Brushes.Yellow, p.Rect);
                }
            }
        }

        private void GcDispose()
        {
            if (++gcDisposeCount >= 500)
            {
                GC.Collect();
                gcDisposeCount = 0;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblHighScore.Text = ($"HIGHSCORE: {intHighScore}");
            lblCurrentScore.Text = ($"CURRENT SCORE: {intCurrentScore}");
            GcDispose();

            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                Projectiles[i].Update();

                if (Projectiles[i].IsOffScreen())
                {
                    disposeProjectiles.Add(Projectiles[i]);
                }

                for (int t = Enemies.Count - 1; t >= 0; t--)
                {
                    if (Projectiles[i].Rect.IntersectsWith(Enemies[t]))
                    {
                        disposeProjectiles.Add(Projectiles[i]);
                        if (--enemyHP[t] <= 0)
                        {
                            disposeEnemy.Add(Enemies[t]);
                            intCurrentScore += 50;
                            break;
                        }
                    }
                }
            }

            foreach (Rectangle x in disposeEnemy) Enemies.Remove(x);
            disposeEnemy.Clear();
            foreach (var x in disposeProjectiles) Projectiles.Remove(x);
            disposeProjectiles.Clear();

            Invalidate();

            gameOver();

            playerMovement();

            this.Invalidate();

            this.Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    Left = true;
                    break;
                case Keys.S:
                    Down = true;
                    break;
                case Keys.D:
                    Right = true;
                    break;
                case Keys.W:
                    Up = true;
                    break;
            }

            if (e.KeyCode == Keys.Space)
            {
                ShootBullet();
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    Left = false;
                    break;
                case Keys.S:
                    Down = false;
                    break;
                case Keys.D:
                    Right = false;
                    break;
                case Keys.W:
                    Up = false;
                    break;
            }
        }

        private void playerMovement()
        {
            if (Right)
                rectPlayer.X += playerSpeed;

            if (Left)
                rectPlayer.X -= playerSpeed;

            if (Up)
                rectPlayer.Y -= playerSpeed;

            if (Down)
                rectPlayer.Y += playerSpeed;

            if (rectPlayer.Left < 0)
                playerSpeed = 0;

            if (rectPlayer.Top < 0)
                playerSpeed = 0;

            if (rectPlayer.Left > this.ClientSize.Width - rightPanel.Width)
                rectPlayer.X = this.ClientSize.Width - rightPanel.Width;

            if (rectPlayer.Top < this.ClientSize.Height / 2 + rectPlayer.Height)
                rectPlayer.Y = this.ClientSize.Height / 2 + rectPlayer.Height;

            if (rectPlayer.Bottom > this.ClientSize.Height)
                rectPlayer.Y = this.ClientSize.Height - rectPlayer.Height;

            if (rectPlayer.IntersectsWith(rightPanel))
                rectPlayer.X = rightPanel.Left - rectPlayer.Width;

            if (rectPlayer.IntersectsWith(leftPanel))
                rectPlayer.X = leftPanel.Right;

            if (rectPlayer.Top < defence1.Height + rectPlayer.Height)
            {
                
            }
        }

        private async void enemyMovement()
        {
            while (true)
            {
                Thread.Sleep(300);
                int dx;
                if (direction == Direction.Left) dx = -enemySpeed;
                else dx = enemySpeed;

                for (int i = 0; i < Enemies.Count; i++)
                {
                    Rectangle Enemy = Enemies[i];
                    Enemy.X += dx;
                    Enemies[i] = Enemy;
                }

                bool hitEdge = false;
                lock (enemyLock)
                {
                    //if any enemy is touching panel, hitEdge true
                    foreach (Rectangle Enemy in Enemies)
                    {
                        if (Enemy.Left < leftPanel.Width || Enemy.Right > ClientSize.Width - rightPanel.Width)
                        {
                            hitEdge = true;
                            break;
                        }
                    }
                }

                if (hitEdge)
                {
                    //changes Y of every enemy, moves down
                    for (int i = 0; i < Enemies.Count; i++)
                    {
                        Rectangle Enemy = Enemies[i];
                        Enemy.Y += enemyHeight;
                        Enemies[i] = Enemy;
                    }

                    //changes horizontal direction
                    if (direction == Direction.Left) direction = Direction.Right;
                    else direction = Direction.Left;
                }
            }
        }

        private void gameOver()
        {
            gameEnd = false;
            lock (enemyLock)
            {
                foreach (Rectangle enemy in Enemies)
                {
                    if (Enemies.Count < 0)
                    {
                        gameEnd = true;
                    }

                    if (enemy.Bottom > this.ClientSize.Height - this.ClientSize.Height / 2)
                    {
                        gameEnd = true;
                    }
                }
            }
            if (gameEnd)
            {
                Application.Exit();
            }
        }

        private void ShootBullet()
        {
            int bulletX = rectPlayer.X + (rectPlayer.Width / 2);
            int bulletY = rectPlayer.Y;

            Projectile p = new Projectile(bulletX, bulletY);
            Projectiles.Add(p);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MessageBox.Show($"you lose maybe or win idk here is youe score: {intCurrentScore}");
        }
    }
}

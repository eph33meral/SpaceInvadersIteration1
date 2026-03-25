using System.Numerics;
using System.Security.Cryptography;
using Microsoft.VisualBasic;

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
            enemySpeed = 15,
            enemySpacing = 70,
            enemyHeight = 30,
            enemyWidth = 30,
            gcDisposeCount = 0,
            intOpacity1 = 255,
            intOpacity2 = 255,
            intOpacity3 = 255,
            currentWave = 1,
            playerHP = 3,
            enemyShootInterval = 1000,
            enemyMoveInterval = 500,
            waveSpeedBonus;

        string playerName = "";
        string leaderboardPath = "leaderboard.txt";

        enum Direction { Left, Right }

        bool
            Up,
            Down,
            Left,
            Right,
            gameEnd,
            waveActive = false,
            scoreSaved = false,
            gameOverShown = false;

        Direction direction = Direction.Left;

        Rectangle
            rectPlayer = new Rectangle(),
            leftPanel = new Rectangle(),
            rightPanel = new Rectangle(),
            defence1 = new Rectangle(),
            defence2 = new Rectangle(),
            defence3 = new Rectangle();

        List<Enemy> Enemies = new List<Enemy>();
        List<Projectile> Projectiles = new List<Projectile>();
        List<Projectile> disposeProjectiles = new List<Projectile>();
        List<Rectangle> disposeDefences = new List<Rectangle>();

        List<Projectile> enemyProjectiles = new List<Projectile>();

        List<Enemy> disposeEnemy = new List<Enemy>();
        private readonly object enemyLock = new();

        Random rng = new Random();

        public Form1(string playerName)
        {
            this.playerName = playerName;
            InitializeComponent();

            rectPlayer.Width = 60;
            rectPlayer.Height = 60;
            rectPlayer.Location = new Point(ClientSize.Width / 2, 965);

            leftPanel = new Rectangle(0, 0, ClientSize.Width / 10, ClientSize.Height);
            rightPanel = new Rectangle(ClientSize.Width - ClientSize.Width / 10, 0,
                                       ClientSize.Width / 10, ClientSize.Height);

            defence3.Width = 180;
            defence3.Height = 100;
            defence3.Location = new Point(ClientSize.Width - (rightPanel.Width * 2), ClientSize.Height - (ClientSize.Height / 3));

            defence2.Width = 180;
            defence2.Height = 100;
            defence2.Location = new Point(ClientSize.Width / 2 - (defence2.Width / 2), ClientSize.Height - (ClientSize.Height / 3));

            defence1.Width = 180;
            defence1.Height = 100;
            defence1.Location = new Point(leftPanel.Width + (leftPanel.Width / 2), ClientSize.Height - (ClientSize.Height / 3));

            lblCurrentScore.Location = new Point(0, leftPanel.Width + 20);
            lblCurrentScore.Name = "currentScore";
            lblCurrentScore.Size = new Size(160, 50);
            lblCurrentScore.TabIndex = 0;
            lblCurrentScore.ForeColor = Color.White;
            lblCurrentScore.BackColor = Color.Transparent;
            lblCurrentScore.Font = new Font("MS Gothic", 15F);
            lblCurrentScore.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblCurrentScore);

            lblHighScore.BackColor = Color.Transparent;
            lblHighScore.Font = new Font("MS Gothic", 15F);
            lblHighScore.ForeColor = Color.White;
            lblHighScore.Location = new Point(0, 10);
            lblHighScore.Name = "lblHighScore";
            lblHighScore.Size = new Size(leftPanel.Width, 120);
            lblHighScore.TabIndex = 0;
            lblHighScore.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblHighScore);

            timer1.Enabled = true;
            timer1.Start();

            FormWindowState formWindowState = FormWindowState.Maximized;

            

            this.BackColor = Color.Black;
            this.DoubleBuffered = true;
        }

        

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            g.DrawImage(Properties.Resources.image,
                0, 0, ClientSize.Width, ClientSize.Height);

            using (var b = new SolidBrush(Color.FromArgb(92, 70, 154)))
            {
                g.FillRectangle(b, leftPanel);
                g.FillRectangle(b, rightPanel);
            }

            if (!gameEnd)
            {
                g.DrawImage(Properties.Resources.Main_character1,
                           new Rectangle(rectPlayer.Location, rectPlayer.Size));
                if (intOpacity1 > 0)
                    using (var db = new SolidBrush(Color.FromArgb(intOpacity1, Color.DeepSkyBlue)))
                        g.FillRectangle(db, defence1);

                if (intOpacity2 > 0)
                    using (var db = new SolidBrush(Color.FromArgb(intOpacity2, Color.DeepSkyBlue)))
                        g.FillRectangle(db, defence2);

                if (intOpacity3 > 0)
                    using (var db = new SolidBrush(Color.FromArgb(intOpacity3, Color.DeepSkyBlue)))
                        g.FillRectangle(db, defence3);

                lock (enemyLock)
                {
                    foreach (Enemy enemy in Enemies)
                        if (enemy.isAlive) enemy.Draw(g);
                }

                foreach (Projectile p in Projectiles)
                    g.FillRectangle(Brushes.Yellow, p.Rect);

                foreach (Projectile p in enemyProjectiles)
                    g.FillRectangle(Brushes.OrangeRed, p.Rect);

                g.DrawString("LIVES", new Font("MS Gothic", 16F),
                 Brushes.White, new PointF(ClientSize.Width - rightPanel.Width + 10, 20));

                for (int i = 0; i < playerHP; i++)
                {
                    g.DrawString("♥", new Font("MS Gothic", 28F),
                                 Brushes.Red, new PointF(ClientSize.Width - rightPanel.Width + 20, 50 + (i * 38)));
                }


                g.DrawString($"WAVE: {currentWave}", new Font("MS Gothic", 16F),
                             Brushes.White, new PointF(ClientSize.Width / 2 - 60, 10));

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

        private void SpawnWave(int waveNumber)
        {
            lock (enemyLock)
            {
                Enemies.Clear();
                Projectiles.Clear();
                enemyProjectiles.Clear();

                int actualWave = ((waveNumber - 1) % 4) + 1;
                waveSpeedBonus = (waveNumber - 1) / 4;

                enemySpeed = 2 + waveSpeedBonus;
                enemyMoveInterval = Math.Max(20, 200 - (waveSpeedBonus * 100));

                int startX = leftPanel.Width + 20;
                int startY = 20;
                int numCols = 8;

                switch (actualWave)
                {

                    case 1:
                        SpawnRows(startX, startY, 3, numCols, EnemyType.Large);
                        break;
                    case 2:
                        SpawnRows(startX, startY, 2, numCols, EnemyType.Large);
                        SpawnRows(startX, startY + 2 * (enemyHeight + enemySpacing), 1, numCols, EnemyType.Medium);
                        break;
                    case 3:
                        SpawnRows(startX, startY, 2, numCols, EnemyType.Medium);
                        SpawnRows(startX, startY + 2 * (enemyHeight + enemySpacing), 1, numCols, EnemyType.Small);
                        break;
                    case 4:
                        SpawnRows(startX, startY, 1, 1 , EnemyType.Boss);
                        break;
                }
            }
        }

        private void SpawnRows(int startX, int startY, int numRows, int numCols, EnemyType type)
        {
            for (int row = 0; row < numRows; row++)
                for (int col = 0; col < numCols; col++)
                {
                    int x = startX + col * (enemyWidth + enemySpacing);
                    int y = startY + row * (enemyHeight + enemySpacing);
                    Enemies.Add(new Enemy(x, y, type));
                }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            lblHighScore.Text = ($"HIGHSCORE: {intHighScore}");
            lblCurrentScore.Text = ($"SCORE: {intCurrentScore}");
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
                    if (Projectiles[i].Rect.IntersectsWith(Enemies[t].enemyRect))
                    {
                        disposeProjectiles.Add(Projectiles[i]);
                        Enemies[t].TakeDamage(1);

                        if (!Enemies[t].isAlive)
                        {
                            disposeEnemy.Add(Enemies[t]);
                            intCurrentScore += Enemies[t].scoreValue;
                            break;
                        }
                    }
                }

                if (Projectiles[i].Rect.IntersectsWith(defence1))
                {
                    disposeProjectiles.Add(Projectiles[i]);
                    intOpacity1 -= 10;
                    if (intOpacity1 <= 0)
                    {
                        intOpacity1 = 0;
                        defence1 = Rectangle.Empty;
                    }
                }

                if (Projectiles[i].Rect.IntersectsWith(defence2))
                {
                    disposeProjectiles.Add(Projectiles[i]);
                    intOpacity2 -= 10;
                    if (intOpacity2 <= 0)
                    {
                        intOpacity2 = 0;
                        defence2 = Rectangle.Empty;
                    }
                }

                if (Projectiles[i].Rect.IntersectsWith(defence3))
                {
                    disposeProjectiles.Add(Projectiles[i]);
                    intOpacity3 -= 10;
                    if (intOpacity3 <= 0)
                    {
                        intOpacity3 = 0;
                        defence3 = Rectangle.Empty;
                    }
                }
            }

            for (int i = enemyProjectiles.Count - 1; i >= 0; i--)
            {
                enemyProjectiles[i].Update();

                if (enemyProjectiles[i].IsOffScreen())
                {
                    enemyProjectiles.RemoveAt(i);
                    continue;
                }

                if (enemyProjectiles[i].Rect.IntersectsWith(rectPlayer))
                {
                    enemyProjectiles.RemoveAt(i);
                    playerHP--;
                    if (playerHP <= 0)
                    {
                        gameEnd = true;
                        timer1.Stop();
                    }
                    continue;
                }

                if (intOpacity1 > 0 && enemyProjectiles[i].Rect.IntersectsWith(defence1))
                {
                    enemyProjectiles.RemoveAt(i);
                    intOpacity1 -= 25;
                    if (intOpacity1 <= 0)
                    {
                        intOpacity1 = 0;
                        defence1 = Rectangle.Empty;
                    }
                    continue;
                }

                if (intOpacity2 > 0 && enemyProjectiles[i].Rect.IntersectsWith(defence2))
                {
                    enemyProjectiles.RemoveAt(i);
                    intOpacity2 -= 25;
                    if (intOpacity2 <= 0)
                    {
                        intOpacity2 = 0;
                        defence2 = Rectangle.Empty;
                    }
                    continue;
                }

                if (intOpacity3 > 0 && enemyProjectiles[i].Rect.IntersectsWith(defence3))
                {
                    enemyProjectiles.RemoveAt(i);
                    intOpacity3 -= 25;
                    if (intOpacity3 <= 0)
                    {
                        intOpacity3 = 0;
                        defence3 = Rectangle.Empty;
                    }
                    continue;
                }
            }

            foreach (Enemy x in disposeEnemy) Enemies.Remove(x);
            disposeEnemy.Clear();
            foreach (var x in disposeProjectiles) Projectiles.Remove(x);
            disposeProjectiles.Clear();

            Invalidate();

            if (gameEnd)
            {
                GameOver();
            }

            if (!waveActive)
            {
                SpawnWave(currentWave);
                waveActive = true;
            }
            else if (Enemies.Count == 0)
            {
                currentWave++;
                SpawnWave(currentWave);
            }

            playerMovement();

            enemyMovement();

            UpdatePlayerProjectiles();

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
                case Keys.Escape:
                    Application.Exit();
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
        }

        public void enemyMovement()
        {
            bool hitEdge = false;

            lock (enemyLock)
            {
                foreach (Enemy enemy in Enemies)
                {
                    if (!enemy.isAlive) continue;
                    Rectangle r = enemy.enemyRect;
                    int move = (direction == Direction.Right) ? enemySpeed : -enemySpeed;
                    enemy.enemyRect = new Rectangle(r.X + move, r.Y, r.Width, r.Height);
                }

                foreach (Enemy enemy in Enemies)
                {
                    if (!enemy.isAlive) continue;

                    if (enemy.enemyRect.Right >= ClientSize.Width - rightPanel.Width ||
                        enemy.enemyRect.Left <= leftPanel.Right)
                    {
                        hitEdge = true;
                        break;
                    }
                }

                if (hitEdge)
                {
                    direction = (direction == Direction.Right) ? Direction.Left : Direction.Right;

                    foreach (Enemy enemy in Enemies)
                    {
                        if (!enemy.isAlive) continue;
                        Rectangle r = enemy.enemyRect;
                        enemy.enemyRect = new Rectangle(r.X, r.Y + 20, r.Width, r.Height);
                    }
                }
            }

            foreach (Enemy enemy in Enemies)
            {
                if (enemy.isAlive && enemy.enemyRect.Bottom >= defence1.Top)
                {
                    gameEnd = true;
                    timer1.Stop();
                    return;
                }
            }
        }

        private void UpdatePlayerProjectiles()
        {
            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                Projectiles[i].Update();

                if (Projectiles[i].IsOffScreen())
                {
                    disposeProjectiles.Add(Projectiles[i]);
                    continue;
                }

                bool hitSomething = false;

                lock (enemyLock)
                {
                    for (int t = Enemies.Count - 1; t >= 0; t--)
                    {
                        if (!Enemies[t].isAlive) continue;
                        if (Projectiles[i].Rect.IntersectsWith(Enemies[t].enemyRect))
                        {
                            Enemies[t].TakeDamage(1);
                            if (!Enemies[t].isAlive)
                            {
                                disposeEnemy.Add(Enemies[t]);
                                intCurrentScore += Enemies[t].scoreValue;
                                if (intCurrentScore > intHighScore)
                                    intHighScore = intCurrentScore;
                            }
                            disposeProjectiles.Add(Projectiles[i]);
                            hitSomething = true;
                            break;
                        }
                    }
                }

                if (hitSomething) continue;

                if (intOpacity1 > 0 && Projectiles[i].Rect.IntersectsWith(defence1))
                {
                    DamageDefence(ref intOpacity1, ref defence1);
                    disposeProjectiles.Add(Projectiles[i]);
                }
                else if (intOpacity2 > 0 && Projectiles[i].Rect.IntersectsWith(defence2))
                {
                    DamageDefence(ref intOpacity2, ref defence2);
                    disposeProjectiles.Add(Projectiles[i]);
                }
                else if (intOpacity3 > 0 && Projectiles[i].Rect.IntersectsWith(defence3))
                {
                    DamageDefence(ref intOpacity3, ref defence3);
                    disposeProjectiles.Add(Projectiles[i]);
                }
            }

            foreach (Enemy x in disposeEnemy) Enemies.Remove(x);
            disposeEnemy.Clear();
            foreach (var x in disposeProjectiles) Projectiles.Remove(x);
            disposeProjectiles.Clear();
        }

        private void DamageDefence(ref int opacity, ref Rectangle defence)
        {
            opacity -= 25;
            if (opacity <= 0)
            {
                opacity = 0;
                defence = Rectangle.Empty;
            }
        }

        private void GameOver()
        {
            if (gameOverShown) return;

            if (playerHP <= 0)
            {
                gameEnd = true;
                gameOverShown = true;
                timer1.Stop();
                if (!scoreSaved)
                {
                    FileManager.addScore(intCurrentScore);
                    scoreSaved = true;
                }
                formGameover formGameover = new formGameover(intCurrentScore);
                formGameover.ShowDialog();
                Invalidate();
                return;
            }
            foreach (Enemy enemy in Enemies)
            {
                if (enemy.isAlive && enemy.enemyRect.Bottom >= defence1.Top)
                {
                    gameEnd = true;
                    timer1.Stop();
                    if (!scoreSaved)
                    {
                        FileManager.addScore(intCurrentScore);
                        scoreSaved = true;
                    }
                    formGameover formGameover = new formGameover(intCurrentScore);
                    formGameover.ShowDialog();
                    Invalidate();
                    return;
                }
            }

            formGameover gameOverForm = new formGameover(intCurrentScore);
            gameOverForm.ShowDialog();

            scoreSaved = false;
            SpawnWave(currentWave);
            waveActive = true;
        }

        private void ShootBullet()
        {
            int bulletX = rectPlayer.X + (rectPlayer.Width / 2) - 2;
            int bulletY = rectPlayer.Y;
            Projectile p = new Projectile(bulletX, bulletY, 20);
            Projectiles.Add(p);
        }

        private void EnemyShoot()
        {
            lock (enemyLock)
            {
                List<Enemy> aliveEnemies = Enemies.Where(e => e.isAlive).ToList();
                if (aliveEnemies.Count == 0) return;

                Enemy shooter = aliveEnemies[rng.Next(aliveEnemies.Count)];
                Point shootPos = shooter.GetShootPosition();

                enemyProjectiles.Add(new Projectile(shootPos.X, shootPos.Y, -15));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread enemyShootThread = new Thread(() =>
            {
                while (!gameEnd)
                {
                    Thread.Sleep(enemyShootInterval);
                    this.Invoke(() => EnemyShoot());
                }
            });
            enemyShootThread.IsBackground = true;
            enemyShootThread.Start();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            int screenW = Screen.PrimaryScreen.Bounds.Width;
            int screenH = Screen.PrimaryScreen.Bounds.Height;

            leftPanel = new Rectangle(0, 0, screenW / 10, screenH);
            rightPanel = new Rectangle(screenW - screenW / 10, 0, screenW / 10, screenH);

            rectPlayer = new Rectangle(screenW / 2 - 30, screenH - 100, 60, 60);

            int defY = screenH - (screenH / 3);
            defence1 = new Rectangle(leftPanel.Right + 50, defY, 180, 100);
            defence2 = new Rectangle(screenW / 2 - 90, defY, 180, 100);
            defence3 = new Rectangle(rightPanel.Left - 230, defY, 180, 100);

            int panelW = leftPanel.Width;
            lblHighScore.Size = new Size(panelW, 60);
            lblHighScore.Location = new Point(0, 10);
            lblCurrentScore.Size = new Size(panelW, 60);
            lblCurrentScore.Location = new Point(0, 80);

            SpawnWave(currentWave);
            waveActive = true;
        }
    }
}


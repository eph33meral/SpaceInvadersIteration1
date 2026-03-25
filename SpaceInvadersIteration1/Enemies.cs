using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceInvadersIteration1;

namespace SpaceInvadersIteration1
{
    public enum EnemyType
    {
        Small,
        Medium,
        Large,
        Boss
    }
    public class Enemy
    {
        public Rectangle enemyRect { get; set; }
        public EnemyType Type { get; private set; }
        public int HP { get; set; }
        public int maxHP { get; private set; }
        public int scoreValue { get; private set; }
        public Color color { get; private set; }
        public float shootChance { get; private set; }
        public bool isAlive => HP > 0;

        public Enemy(int x, int y, EnemyType type)
        {
            Type = type;
            switch (type)
            {
                case EnemyType.Small:
                    enemyRect = new Rectangle(x, y, 40, 40);
                    HP = 3;
                    maxHP = 3;
                    scoreValue = 100;
                    color = Color.FromArgb(255, 100, 100);
                    shootChance = 1.5f;
                    break;

                case EnemyType.Medium:
                    enemyRect = new Rectangle(x, y, 55, 55);
                    HP = 2;
                    maxHP = 2;
                    scoreValue = 75;
                    color = Color.FromArgb(255, 165, 0);
                    shootChance = 1.0f;
                    break;

                case EnemyType.Large:
                    enemyRect = new Rectangle(x, y, 70, 70);
                    HP = 1;
                    maxHP = 1;
                    scoreValue = 50;
                    color = Color.FromArgb(255, 50, 50);
                    shootChance = 0.5f;
                    break;

                case EnemyType.Boss:
                    enemyRect = new Rectangle(x, y, 150, 150);
                    HP = 50;
                    maxHP = 50;
                    scoreValue = 500;
                    color = Color.FromArgb(139, 0, 139);
                    shootChance = 5.0f;
                    break;
            }
        }

        public void TakeDamage(int damage)
        {
            HP -= damage;
            if (HP < 0) HP = 0;
        }

        public void Draw(Graphics g)
        {
            using (SolidBrush brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, enemyRect);
            }
        }

        public bool ShouldShoot(Random rng)
        {
            return rng.Next(0, 100000) < (shootChance * 100);
        }

        public Point GetShootPosition()
        {
            int bulletX = enemyRect.X + (enemyRect.Width / 2);
            int bulletY = enemyRect.Bottom;
            return new Point(bulletX, bulletY);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersIteration1
{
    internal class Enemy
    {

        public Rectangle Rect;
        public int Health = 3;
        public int enemyHeight = 0;
        public int enemyWidth = 0;
   
        public Enemy(Rectangle rect, int pHealth, int penemyHeight, int penemyWidth)
        {
            Rect = new Rectangle();
            Health = pHealth;
            enemyHeight = penemyHeight;
            enemyWidth = penemyWidth;
        }
    }
}

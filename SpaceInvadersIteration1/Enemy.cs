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
   
        public Enemy(Rectangle rect)
        {
            Rect = new Rectangle();
        }
    }
}

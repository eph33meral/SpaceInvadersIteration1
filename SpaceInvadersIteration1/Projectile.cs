using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersIteration1
{
    internal class Projectile
    {
        public Rectangle Rect;
        public int projectileSpeed = 30;

        public Projectile(int x, int y)
        {
            Rect = new Rectangle(x, y, 4, 12);
        }

        public void Update()
        {
            Rect = new Rectangle(
                Rect.X,
                Rect.Y - projectileSpeed,
                Rect.Width,
                Rect.Height
            );
        }

        public bool IsOffScreen()
        {
            return Rect.Y < -20;
        }

    }
}

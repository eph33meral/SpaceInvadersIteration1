using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersIteration1
{
    internal class Powerups
    {
        int maxValue = 500;
        int currentValue = 0;
        Rectangle powerBarBG = new Rectangle();
        Rectangle powerBar = new Rectangle();
        bool isActive;
        enum powerup
        {
            shield,
            bulletSpeed,
            movementSpeed,
            invisibility
        }

        public Powerups(Size ClientSize)
        {
            powerBarBG.Width = ClientSize.Width / 12;
            powerBarBG.Height = 900;
            powerBarBG.Location = new Point(ClientSize.Width - 230, 30);

            powerBar.Width = powerBarBG.Width - 10;
            powerBar.Height = powerBarBG.Height - 10;
            powerBar.Location = new Point(powerBarBG.X + (powerBarBG.Width - powerBar.Width) / 2,
            powerBarBG.Y + (powerBarBG.Height - powerBar.Height) / 2);
        }

        public void draw(Graphics g)
        {
            Brush b3 = new SolidBrush(Color.Black);
            Brush b5 = new SolidBrush(Color.DeepSkyBlue);
            g.FillRectangle(b3, powerBarBG);
            g.FillRectangle(b5, powerBar);
        }
    }
}

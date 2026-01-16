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
        Rectangle powerBarCurrent = new Rectangle();

        public Powerups(Size ClientSize)
        {
            powerBarBG.Width = ClientSize.Width / 12;
            powerBarBG.Height = 900;
            powerBarBG.Location = new Point(ClientSize.Width - 230, 30);

            powerBarCurrent.Width = powerBarBG.Width - 10;
            powerBarCurrent.Height = powerBarBG.Height - 10;
            powerBarCurrent.Location = new Point(powerBarBG.X + (powerBarBG.Width - powerBarCurrent.Width) / 2,
            powerBarBG.Y + (powerBarBG.Height - powerBarCurrent.Height) / 2);
        }

        public void draw(Graphics g)
        {
            Brush b3 = new SolidBrush(Color.Black);
            Brush b5 = new SolidBrush(Color.FromArgb(255, 253, 187));
            g.FillRectangle(b3, powerBarBG);
            g.FillRectangle(b5, powerBarCurrent);
        }
    }
}

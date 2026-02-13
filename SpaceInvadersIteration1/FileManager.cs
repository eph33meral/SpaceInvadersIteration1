using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.Devices;
using Microsoft.VisualBasic;

namespace SpaceInvadersIteration1
{
    internal class FileManager
    {
        
        private void Form1_Load(object sender, EventArgs e)
        {
            string userName = Interaction.InputBox("Enter your username", "", "");
        }
        public static void main()
        {
            string path = @"C:\Users\ms.2402725\OneDrive - Hereford Sixth Form College\Computer science\Project\SpaceInvadersIteration1\Leaderboard.txt";
            using (FileStream fs = File.Create(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(path);
                }
            }
        }       
    }
}

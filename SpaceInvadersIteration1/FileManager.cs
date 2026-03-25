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
        public static string PlayerName { get; set; } = "";

        private static readonly string Leaderboard = Path.Combine(
    AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Leaderboard.txt"
);

        private void Form1_Load(object sender, EventArgs e)
        {
            string userName = Interaction.InputBox("Enter your username", "", "");
        }
        public static void FileCreate()
        {
            if (!File.Exists(Leaderboard))
            {
                using (StreamWriter sw = File.CreateText(Leaderboard))
                {
                    sw.WriteLine("Name, Score");
                }
            }
        }

        public static void addScore(int score)
        {
            var scores = new List<(string name, int score)>();
            foreach (string line in File.ReadAllLines(Leaderboard).Skip(1))
            {
                string[] parts = line.Split(',');
                if (parts.Length == 2 && int.TryParse(parts[1], out int s))
                    scores.Add((parts[0], s));
            }

            scores.Add((PlayerName, score));

            for (int i = 0; i < scores.Count - 1; i++)
            {
                for (int j = 0; j < scores.Count - 1 - i; j++)
                {
                    if (scores[j].score < scores[j + 1].score)
                    {
                        var temp = scores[j];
                        scores[j] = scores[j + 1];
                        scores[j + 1] = temp;
                    }
                }
            }

            if (scores.Count > 10)
                scores = scores.GetRange(0, 10);

            using (StreamWriter sw = new StreamWriter(Leaderboard, append: false))
            {
                sw.WriteLine("Name, Score");
                foreach (var entry in scores)
                    sw.WriteLine(entry.name + "," + entry.score);
            }
        }

        public static List<(string name, int score)> LoadScores()
        {
            var scores = new List<(string name, int score)>();

            if (!File.Exists(Leaderboard))
                return scores;

            foreach (string line in File.ReadAllLines(Leaderboard).Skip(1))
            {
                string[] parts = line.Split(',');
                if (parts.Length == 2 && int.TryParse(parts[1], out int s))
                    scores.Add((parts[0], s));
            }

            return scores;
        }
    }
}

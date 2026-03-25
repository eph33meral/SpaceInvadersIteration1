namespace SpaceInvadersIteration1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FormStart startForm = new FormStart();
            if (startForm.ShowDialog() == DialogResult.OK)
            {
                Form1 gameForm = new Form1(startForm.PlayerName);
                Application.Run(gameForm);
            }
        }
    }
}
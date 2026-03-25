namespace SpaceInvadersIteration1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            timer1 = new System.Windows.Forms.Timer(components);
            lblHighScore = new Label();
            lblCurrentScore = new Label();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 16;
            timer1.Tick += timer1_Tick;
            // 
            // lblHighScore
            // 
            lblHighScore.Location = new Point(0, 0);
            lblHighScore.Name = "lblHighScore";
            lblHighScore.Size = new Size(100, 23);
            lblHighScore.TabIndex = 0;
            // 
            // lblCurrentScore
            // 
            lblCurrentScore.Location = new Point(0, 0);
            lblCurrentScore.Name = "lblCurrentScore";
            lblCurrentScore.Size = new Size(100, 23);
            lblCurrentScore.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(1924, 1055);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private Label lblHighScore;
    }
}

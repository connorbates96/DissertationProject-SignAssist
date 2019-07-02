using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace SignAssist
{
    public struct resultEntry
    {
        public int wordIndex;
        public int handshapeScore;
        public int movementScore;
        public int overallScore;
        public bool helpNeeded;
        public int timePenalty;

        public resultEntry(int wordindex, int overallscore, int handshapescore, int movementscore, bool helpneeded, int timepenalty)
        {
            wordIndex = wordindex;
            overallScore = overallscore;
            handshapeScore = handshapescore;
            movementScore = movementscore;
            helpNeeded = helpneeded;
            timePenalty = timepenalty;
        }
    }


    public partial class Form2 : Form
    {
        List<int> signOrder = new List<int>() { 0, 7, 2, 3, 6, 1, 4, 9, 11, 8, 10, 5 };
        //int randomiseCounter = 11;
        enum wordEnum { AEROPLANE = 0, ALWAYS, CLEAN, EVERYONE, EQUAL, IMPROVE, LAPTOP, MIDDLE, OKAY, OVER, READ, WHERE }
        int currentWord;
        int currentIndex = 0;
        Label currentWordLabel;
        Label currentScoreLabel; 
        List<Label> wordLabels;
        List<Label> scoreLabels;
        private string currentVideo = "";
        private bool requiredHelp = false;
        public bool testStart = false;
        private int tickerCount = 0;
        private bool scoreCalculated = false;
        private bool signFinished = false;
        private Process animationProcess = null;
        List<resultEntry> testResults;


        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        //Feedback
        string[] HSFB = { "Your handshape was excellent!",
                          "Your handshape was almost there, try to continue to keep your hands in the correct position during the sign.",
                          "Your hands seemed to move a little during the sign. Perhaps watch the video above now and see how the actor performs the sign.",
                          "There was a lot of differences between your attempt and the 'perfect' sign, please attempt this sign in the practice section again."};
        string[] MFB = { "Your movement was almost perfect!",
                         "The movement in your sign was very good, make sure you aren't moving too quickly",
                         "Your movement was slightly off, perhaps you didnt start the sign in the correct location.",
                         "Your movement wasn't great, make sure that you are performing the entire sign within the sensing region of the Leap Motion sensor."};


        public Form2()
        {
            InitializeComponent();
            currentIndex = 0;
            wordLabels = new List<Label> { label3, label5, label7, label9, label11, label13, label15, label17, label19, label21, label23, label25 };
            scoreLabels = new List<Label> { label4, label6, label8, label10, label12, label14, label16, label18, label20, label22, label24, label26 };
            testResults = new List<resultEntry>();
            button1.Text = "Begin Test!";

        }

        //Close on esc press
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void beginNextSection()
        {
            if (label26.BackColor != Color.LightSteelBlue) //There are still more words to do
            {
                updateTest();
                startCountdown();
                currentIndex++;
                scoreCalculated = false;
            }
            else
            {
                button1.Click += new EventHandler(finishTest);
                button1.Text = "Finish Test";
                popup resultWindow = new popup(testResults);
                resultWindow.TopMost = true;
                resultWindow.Show();
            }

        }

        private void endSection()
        {
            timer1.Stop();
            if (scoreCalculated == false) calcResult();
            button4.Visible = true;
        }


        //Takes the current sign from the currentIn dexForSign then updates the table on the left hand side, the current video and the title text.
        private void updateTest()
        {
            label30.Text = "";
            label31.Text = "";
            progressBar1.Value = 100;
            ModifyProgressBarColor.SetState(progressBar1, 1);
            requiredHelp = false;
            signFinished = false;

            currentWord = signOrder[currentIndex];
            currentWordLabel = wordLabels[currentIndex];
            currentScoreLabel = scoreLabels[currentIndex];

            switch (currentWord) {

                case 0:
                    currentWordLabel.Text = "Aeroplane";
                    currentWordLabel.BackColor = Color.LightSteelBlue;
                    currentScoreLabel.Text = "";
                    currentScoreLabel.BackColor = Color.LightSteelBlue;
                    currentVideo = "aeroplane.mp4";
                    label32.Text = "Current Word - Aeroplane";
                    break;
                case 1:
                    currentWordLabel.Text = "Always";
                    currentWordLabel.BackColor = Color.LightSteelBlue;
                    currentScoreLabel.Text = "";
                    currentScoreLabel.BackColor = Color.LightSteelBlue;
                    currentVideo = "always.mp4";
                    label32.Text = "Current Word - Always";
                    break;
                case 2:
                    currentWordLabel.Text = "Clean";
                    currentWordLabel.BackColor = Color.LightSteelBlue;
                    currentScoreLabel.Text = "";
                    currentScoreLabel.BackColor = Color.LightSteelBlue;
                    currentVideo = "clean.mp4";
                    label32.Text = "Current Word - Clean";
                    break;
                case 3:
                    currentWordLabel.Text = "Everyone";
                    currentWordLabel.BackColor = Color.LightSteelBlue;
                    currentScoreLabel.Text = "";
                    currentScoreLabel.BackColor = Color.LightSteelBlue;
                    currentVideo = "everyone.mp4";
                    label32.Text = "Current Word - Everyone";
                    break;
                case 4:
                    currentWordLabel.Text = "Equal";
                    currentWordLabel.BackColor = Color.LightSteelBlue;
                    currentScoreLabel.Text = "";
                    currentScoreLabel.BackColor = Color.LightSteelBlue;
                    currentVideo = "equal.mp4";
                    label32.Text = "Current Word - Equal";
                    break;
                case 5:
                    currentWordLabel.Text = "Improve";
                    currentWordLabel.BackColor = Color.LightSteelBlue;
                    currentScoreLabel.Text = "";
                    currentScoreLabel.BackColor = Color.LightSteelBlue;
                    currentVideo = "improve.mp4";
                    label32.Text = "Current Word - Improve";
                    break;
                case 6:
                    currentWordLabel.Text = "Laptop";
                    currentWordLabel.BackColor = Color.LightSteelBlue;
                    currentScoreLabel.Text = "";
                    currentScoreLabel.BackColor = Color.LightSteelBlue;
                    currentVideo = "laptop.mp4";
                    label32.Text = "Current Word - Laptop";
                    break;
                case 7:
                    currentWordLabel.Text = "Middle";
                    currentWordLabel.BackColor = Color.LightSteelBlue;
                    currentScoreLabel.Text = "";
                    currentScoreLabel.BackColor = Color.LightSteelBlue;
                    currentVideo = "middle.mp4";
                    label32.Text = "Current Word - Middle";
                    break;
                case 8:
                    currentWordLabel.Text = "Okay";
                    currentWordLabel.BackColor = Color.LightSteelBlue;
                    currentScoreLabel.Text = "";
                    currentScoreLabel.BackColor = Color.LightSteelBlue;
                    currentVideo = "okay.mp4";
                    label32.Text = "Current Word - Okay";
                    break;
                case 9:
                    currentWordLabel.Text = "Over";
                    currentWordLabel.BackColor = Color.LightSteelBlue;
                    currentScoreLabel.Text = "";
                    currentScoreLabel.BackColor = Color.LightSteelBlue;
                    currentVideo = "over.mp4";
                    label32.Text = "Current Word - Over";
                    break;
                case 10:
                    currentWordLabel.Text = "Read";
                    currentWordLabel.BackColor = Color.LightSteelBlue;
                    currentScoreLabel.Text = "";
                    currentScoreLabel.BackColor = Color.LightSteelBlue;
                    currentVideo = "read.mp4";
                    label32.Text = "Current Word - Read";
                    break;
                case 11:
                    currentWordLabel.Text = "Where";
                    currentWordLabel.BackColor = Color.LightSteelBlue;
                    currentScoreLabel.Text = "";
                    currentScoreLabel.BackColor = Color.LightSteelBlue;
                    currentVideo = "where.mp4";
                    label32.Text = "Current Word - Where";
                    break;
            }



        }

        private void startCountdown()
        {
            timer1.Start();
            timer1.Interval = 1000;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"C:\Users\Connor Bates\Documents\Visual Studio 2015\Projects\SignAssist\Videos\SignBank\" + currentVideo))
            {
                axWindowsMediaPlayer1.URL = @"C:\Users\Connor Bates\Documents\Visual Studio 2015\Projects\SignAssist\Videos\SignBank\" + currentVideo;
            }

            requiredHelp = true;
            if (animationProcess != null) SetForegroundWindow(animationProcess.MainWindowHandle);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (signFinished == false)
            {
                signFinished = true;
                endSection();
            }
            if (animationProcess != null) SetForegroundWindow(animationProcess.MainWindowHandle);
        }

        private void timer_Tick(Object sender, EventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;

            if (progressBar1.Value > progressBar1.Minimum && tickerCount == 2)
            {
                progressBar1.Value--;
                tickerCount = 0;
            }
            else if (progressBar1.Value > progressBar1.Minimum)
            {
                progressBar1.Value = progressBar1.Value - 2;
                tickerCount++;
            }

            if (progressBar1.Value < 17) ModifyProgressBarColor.SetState(progressBar1, 2);      //   1=Green    2=Red   3=Yellow
            else if (progressBar1.Value < 50) ModifyProgressBarColor.SetState(progressBar1, 3);

            if (progressBar1.Value == 0) endSection();

        }

        private void calcResult()
        {
            //Step1
            Random rnd = new Random();
            int bracketScore = rnd.Next(1, 21);
            int bracket; //Excellent:1, Good:2, Okay:3, Poor:4

            if (bracketScore > 16) bracket = 1;
            else if (bracketScore > 4) bracket = 2;
            else if (bracketScore > 1) bracket = 3;
            else bracket = 4;

            //Step2
            int offset;
            if (bracket == 1 || bracket == 4) offset = rnd.Next(1, 6);
            else offset = rnd.Next(1, 10);

            int posCheck = rnd.Next(1, 3);
            bool positive;
            if (posCheck == 1) positive = false;
            else positive = true;

            int score;
            int handshapeScore;
            int movementScore;
            int whichScorePos = rnd.Next(1, 3);

            if (bracket == 1)
            {
                score = 95;
            }
            else if (bracket == 2)
            {
                score = 80;
            }
            else if (bracket == 3)
            {
                score = 60;
            }
            else //equals 4
            {
                score = 45;
            }

            if (positive == true)
            {
                score += offset;
                if (whichScorePos == 1)
                {
                    handshapeScore = score + offset;
                    movementScore = score - offset;
                }
                else
                {
                    handshapeScore = score - offset;
                    movementScore = score + offset;
                }
            }
            else
            {
                score -= offset;
                if (whichScorePos == 1)
                {
                    handshapeScore = score + offset;
                    movementScore = score - offset;
                }
                else
                {
                    handshapeScore = score - offset;
                    movementScore = score + offset;

                }
            }

            //Step3

            string movementFeedback = MFB[bracket - 1];
            string handshapeFeedback = HSFB[bracket - 1];

            string helpString;
            if (requiredHelp == true)
            {
                score -= 10;
                helpString = "\nHelp required?: Yes (Score = -10%)";
            }
            else
            {
                helpString = "\nHelp required?: No";
            }

            string timeString;
            int timePenalty;
            if (progressBar1.Value > 50)
            {
                timeString = "\nTime used: Less than 30s";
                timePenalty = 0;
            }
            else if (progressBar1.Value > 25)
            {
                timeString = "\nTime used: Less than 45s (Score = -5%)";
                score -= 5;
                timePenalty = 5;
            }
            else if (progressBar1.Value > 0)
            {
                timeString = "\nTime used: Less than 60s (Score = -10%)";
                score -= 10;
                timePenalty = 10;
            }
            else
            {
                timeString = "\nTime used: Run out of time! (Score = -15%)";
                score -= 15;
                timePenalty = 15;
            }

            string outputScore = "Overall Score: " + score
                        + "\nHand Shape: " + handshapeScore
                        + "\nMovement: " + movementScore
                        + helpString + timeString;
            string outputFeedback = handshapeFeedback + "\n" + movementFeedback;

            label30.Text = outputFeedback;
            label31.Text = outputScore;
            if (score > 89)
            {
                label31.ForeColor = Color.LightGreen;
                scoreLabels[currentIndex - 1].ForeColor = Color.LightGreen;
            }
            else if (score > 69)
            {
                label31.ForeColor = Color.DarkGreen;
                scoreLabels[currentIndex - 1].ForeColor = Color.DarkGreen;
            }
            else if (score > 49)
            {
                label31.ForeColor = Color.Yellow;
                scoreLabels[currentIndex - 1].ForeColor = Color.Yellow;
            }
            else
            {
                label31.ForeColor = Color.Red;
                scoreLabels[currentIndex - 1].ForeColor = Color.Red;
            }

            scoreLabels[currentIndex - 1].Text = "" + score;
            scoreCalculated = true;

            resultEntry temp = new resultEntry(currentWord, score, handshapeScore, movementScore, requiredHelp, timePenalty);
            testResults.Add(temp);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            beginNextSection();
            button1.Text = "";
            button1.Click -= button1_Click;
            button2.Click += new EventHandler(button2_Click);
            button3.Click += new EventHandler(button3_Click);
            if (animationProcess != null) SetForegroundWindow(animationProcess.MainWindowHandle);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            beginNextSection();
            button4.Visible = false;
            if (animationProcess != null) SetForegroundWindow(animationProcess.MainWindowHandle);
        }

        private void finishTest(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Users\Connor Bates\Documents\Visual Studio 2015\Projects\SignAssist\AllHandModels\AllHandModels.exe");
            animationProcess = Process.Start(startInfo);
            animationProcess.WaitForInputIdle();
            SendKeys.Send("{Enter}");
            //Thread.Sleep(1000);
            ShowWindow(this.Handle, 6);
            //Thread.Sleep(1000);
            ShowWindow(this.Handle, 3);

            //Set window into panel
            SetParent(animationProcess.MainWindowHandle, this.panel1.Handle);
            MoveWindow(animationProcess.MainWindowHandle, 0, 0, this.panel1.Width, this.panel1.Height, true);

            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
        }
    }

    public static class ModifyProgressBarColor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }

}

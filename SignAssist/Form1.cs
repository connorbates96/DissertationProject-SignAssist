using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Media;
using Microsoft.Expression.Encoder;
using Microsoft.Expression.Encoder.Devices;
using Microsoft.Expression.Encoder.ScreenCapture;
using Microsoft.Expression.Encoder.Live;
using System.Collections.ObjectModel;
using Leap;

namespace SignAssist
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //On load up of the form, initialise controller, start the AllHandModels exe as a process and set its parent to panel1
        private void Form1_Load(object sender, EventArgs e)
        {
            //Initialise the leap motion
            controller = new Controller();

            //button1.PerformClick();
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
            MoveWindow(animationProcess.MainWindowHandle, 0, 70, this.panel1.Width, this.panel1.Height - 70, true);

            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            testResults = new List<resultEntry>();


        }

        //On closing the form, stop the screen cap and dispose of it. If the animation process is still running, kill the process
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (screenCap.Status == RecordStatus.Running)
            {
                screenCap.Stop();
            }
            screenCap.Dispose();

            if (animationProcess != null) animationProcess.Kill();
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


        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        const int WM_KEYDOWN = 0x0100;
        const int WM_KEYUP = 0x101;
        const int VK_RETURN = 0x0D;
        private Process animationProcess = null;
        private ScreenCaptureJob screenCap;
        const int GWL_STYLE = (-16);
        const UInt32 WS_VISIBLE = 0x10000000;
        enum wordEnum { AEROPLANE = 0, ALWAYS, CLEAN, EVERYONE, EQUAL, IMPROVE, LAPTOP, MIDDLE, OKAY, OVER, READ, WHERE }//Create an enumerator which keeps track of the index for each item
        public int currentWord = -1;
        public int previousWord = -1;
        //Feedback
        string[] aeroplaneHSFB = { "Make sure that your thumb and pinky finger are fully extended.", "Make sure that your index, middle and ring finger are bent as if making a fist.", "Make sure that your palm is facing away from you" };
        string[] aeroplaneMVFB = { "Make sure that your hand is moving upwards and slightly away from you", "Make sure that the movement begins near the bottom of your torso and moves towards the top of your torso."};
        string[] alwaysHSFB = { "Make sure that your hands are closed into a fist with only the index finger extended out straight." };
        string[] alwaysMVFB = { "Make sure both index fingers are pointing towards each other.", "Make sure that your hands are moving in a forward circular motion with the index fingers 'rolling' over each other." };
        string[] cleanHSFB = { "Ensure that your hands are in 2 closed fists with both palms facing away from your body." };
        string[] cleanMVFB = { "Make sure that both hands are moving in circular motions (clockwise for the left hand and anti-clockwise for the right hand)." };
        string[] everyoneHSFB = { "Make sure that your hand is flat.", "Make sure that all figners are fully extended and kept together." };
        string[] everyoneMVFB = { "Make sure that you're moving your hand from right to left in a sweeping motion." };
        string[] equalHSFB = { "Make sure that your fingers are at a right angle with your palm so that only the knuckles connecting your fingers and hand are bent.", "Make sure that all other knuckles are fully extended.", "Make sure that both your palms are facing each other (towards your centre)." };
        string[] equalMVFB = { "Make sure that both of your hands have been brought up to the centre of your signing area.", "Make sure that you move your fingertips towards each other so that they are touching, then move them further away and repeat a few times." };
        string[] improveHSFB = { "Ensure that both of your hands are in a closed fist with only the thumb extended (like a thumbs up)", "Ensure that your left hand has its palm facing to the right with the thumb extended up and forwards slightly.", "Ensure that your right hand is rotated so that your thumb is extended out to the left." };
        string[] improveMVFB = { "Ensure that you are moving your right thumb across the left thumb in a brushing motion from base to tip.", "Make sure that you are repeating the movement a few times." };
        string[] laptopHSFB = { "Make sure that both of your hands are completely open and flat.", "Ensure you are keeping all of your fingers together.", "Ensure that your left hand has its palm facing straight up with the thumb away from the body.", "Ensure that your right hand has its palm facing down with the thumb towards your body." };
        string[] laptopMVFB = { "Ensure that the sign starts with your right hand resting on top of your left hand.", "Ensure that you lift the right hand up while keeping the far sides of your hands together (like opening a book)." };
        string[] middleHSFB = { "Ensure that your left hand is open and completely flat, with its palm facing upwards.", "Ensure that your middle finger of your right hand is angled at the first knuckle and extended down from the rest of the fingers" };
        string[] middleMVFB = { "Ensure that your right hand middle finger is touching the centre of your left palm.", "Ensure that you are bringing your right hand up and away from the left hand before touching it again." };
        string[] okayHSFB = { "Ensure that your right hand is in a closed fist with only the thumb extended out." };
        string[] okayMVFB = { "Ensure that your palm is facing towards yourself and slightly up.", "Ensure that you are moving your hand in circular, anti-clockwise motions."};
        string[] overHSFB = { "Ensure both hands are completely flat with their palms facing towards the floor." };
        string[] overMVFB = { "Ensure that your left hand remains stationary in front of your torso during the sign.", "Make sure that your right hand starts directly below your left hand.", "Make sure that your right hand moves forward from under the left hand, then upwards and back to sit above the left hand."};
        string[] readHSFB = { "Ensure that your left hand is open with all figners fully extended and together.", "Make sure that your left hand has its palm facing towards you.", "Make sure that your right hand is in a closed fist with both your index and middle fingers extended out straight."};
        string[] readMVFB = { "Ensure that your right hand points with the extended fingers towards the palm of your left hand.", "Ensure that your fingers are moving over the plam as if reading across lines from a book."};
        string[] whereHSFB = { "Ensure that both hands are open flat, with their palms facing upwards."};
        string[] whereMVFB = { "Ensure that both of your hands are moving towards each other and then away, repeating this a few times.", "Ensure that your hands are in the middle of your signing space and that you have raised them high enough." };
        

        private string currSignBankVid = "";
        private string currDualVid = "";

        List<resultEntry> testResults;
        List<int> signOrder = new List<int>() { 0, 7, 2, 3, 6, 1, 4, 9, 11, 8, 10, 5 };
        int currentIndex = 0;
        Random rnd = new Random();

        Controller controller;
        private List<Frame> currentSign = new List<Frame>();

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);


         private void button1_Click(object sender, EventArgs e)
        {
            previousWord = currentWord;
            currentWord = (int)wordEnum.AEROPLANE;
            update();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            previousWord = currentWord;
            currentWord = (int)wordEnum.ALWAYS;
            update();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            previousWord = currentWord;
            currentWord = (int)wordEnum.CLEAN;
            update();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            previousWord = currentWord;
            currentWord = (int)wordEnum.EVERYONE;
            update();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            previousWord = currentWord;
            currentWord = (int)wordEnum.EQUAL;
            update();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            previousWord = currentWord;
            currentWord = (int)wordEnum.IMPROVE;
            update();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            previousWord = currentWord;
            currentWord = (int)wordEnum.LAPTOP;
            update();
        }
        private void button8_Click(object sender, EventArgs e)
        {
            previousWord = currentWord;
            currentWord = (int)wordEnum.MIDDLE;
            update();
        }
        private void button9_Click(object sender, EventArgs e)
        {
            previousWord = currentWord;
            currentWord = (int)wordEnum.OKAY;
            update();
        }
        private void button10_Click(object sender, EventArgs e)
        {
            previousWord = currentWord;
            currentWord = (int)wordEnum.OVER;
            update();
        }
        private void button11_Click(object sender, EventArgs e)
        {
            previousWord = currentWord;
            currentWord = (int)wordEnum.READ;
            update();
        }
        private void button12_Click(object sender, EventArgs e)
        {
            previousWord = currentWord;
            currentWord = (int)wordEnum.WHERE;
            update();
        }

        //Set the animation process (AllHandModels.exe) as the active window so that the program runs then start the recording
        private void button16_Click(object sender, EventArgs e)
        {
            if (animationProcess != null) SetForegroundWindow(animationProcess.MainWindowHandle);
            startRecord();
            Thread.Sleep(100);
            if (animationProcess != null) SetForegroundWindow(animationProcess.MainWindowHandle);

            Thread captureSignThread = new Thread(new ThreadStart(captureSign));
            captureSignThread.Start();

            //captureSign();

            //while (continueCapturing)
            //{
            //    currFrame = controller.Frame();

            //    List<Hand> hands = currFrame.Hands;
            //    //sign.Add(currFrame);
            //    //currentSign.Add(currFrame);

            //    if (hands.Count > 0) label1.Text = ("Hand Direction X = " + hands[0].Direction.x + ",\n Hand Direction Y = " + hands[0].Direction.y + ",\n Hand Direction Z = " + hands[0].Direction.z + " .....\n\n\n");
            //    else label1.Text = "Nope..";
            //}


        }

        //Function for stopping the recording once the button has been pressed
        private void button17_Click(object sender, EventArgs e)
        {
            if (screenCap.Status == RecordStatus.Running)
            {
                screenCap.Stop();
            }
            provideFeedback();

            //Frame temp = currentSign.First();
            //List<Hand> hands = temp.Hands;


            //label3.Text = "" + currentSign.Count();
            //Console.WriteLine("currentSign Count = " + currentSign.Count);

        }

        //Functions for playing back the videos through the windows media player that is embedded

        private void button18_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"C:\Users\Connor Bates\Documents\Visual Studio 2015\Projects\SignAssist\Videos\SignBank\" + currSignBankVid))
            {
                axWindowsMediaPlayer1.URL = @"C:\Users\Connor Bates\Documents\Visual Studio 2015\Projects\SignAssist\Videos\SignBank\" + currSignBankVid;
            }

        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"C:\Users\Connor Bates\Documents\Visual Studio 2015\Projects\SignAssist\Videos\Perfect\" + currDualVid))
            {
                axWindowsMediaPlayer1.URL = @"C:\Users\Connor Bates\Documents\Visual Studio 2015\Projects\SignAssist\Videos\Perfect\" + currDualVid;
            }


        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"C:\Users\Connor Bates\Documents\Visual Studio 2015\Projects\SignAssist\Videos\Attempts\" + currDualVid))
            {
                axWindowsMediaPlayer1.URL = @"C:\Users\Connor Bates\Documents\Visual Studio 2015\Projects\SignAssist\Videos\Attempts\" + currDualVid;
            }
        }

        //Function which sets the recording are for the screen capture and begins the recording 
        void startRecord()
        {
            screenCap = new ScreenCaptureJob();

            System.Drawing.Size WorkingArea = SystemInformation.WorkingArea.Size;
            Rectangle captureRect = new Rectangle(this.panel1.Location.X + 780, this.panel1.Location.Y + 330, this.panel1.Size.Width - 50, this.panel1.Size.Height - 170);
            screenCap.CaptureRectangle = captureRect;
            screenCap.ShowFlashingBoundary = true;
            screenCap.ShowCountdown = true;
            //screenCap.CaptureMouseCursor = true;
            //screenCap.AddAudioDeviceSource(AudioDevices());
            //screenCap.OutputPath = @"C:\Users\Connor Bates\Documents\Visual Studio 2015\Projects\SignAssist";

            if (File.Exists(@"C:\Users\Connor Bates\Documents\Visual Studio 2015\Projects\SignAssist\Videos\Attempts\" + currDualVid))
            {
                 File.Delete(@"C:\Users\Connor Bates\Documents\Visual Studio 2015\Projects\SignAssist\Videos\Attempts\" + currDualVid);
                axWindowsMediaPlayer1.URL = "";
                screenCap.OutputScreenCaptureFileName = @"C:\Users\Connor Bates\Documents\Visual Studio 2015\Projects\SignAssist\Videos\Attempts\" + currDualVid;
                screenCap.Start();
            }
            else
            {
                screenCap.OutputScreenCaptureFileName = @"C:\Users\Connor Bates\Documents\Visual Studio 2015\Projects\SignAssist\Videos\Attempts\" + currDualVid;
                screenCap.Start();
            }

        }

        void captureSign()
        {
           //// Thread.Sleep(0);
           // Console.WriteLine("Started");
           // //List<Frame> sign = new List<Frame>();

           // while (continueCapturing == true)
           // {
           //     currFrame = controller.Frame();
           //     //sign.Add(currFrame);
           //     //currentSign.Add(currFrame);

           //     //Console.WriteLine("No. of Hands = %d", currFrame.Hands.Count());
           //     //button20.Text = currFrame.Hands.Count.ToString();

           // }

            //Console.WriteLine("No. of frames in sign = " + sign.Count);
            //currentSign = sign;
            
        }

        void update()
        {
            switch (currentWord)
            {
                case 0:
                    currSignBankVid = "aeroplane.mp4";
                    currDualVid = "aeroplane.xesc";
                    button1.BackColor = System.Drawing.Color.DarkSlateGray;
                    break;
                case 1:
                    currSignBankVid = "always.mp4";
                    currDualVid = "always.xesc";
                    button2.BackColor = System.Drawing.Color.DarkSlateGray;
                    break;
                case 2:
                    currSignBankVid = "clean.mp4";
                    currDualVid = "clean.xesc";
                    button3.BackColor = System.Drawing.Color.DarkSlateGray;
                    break;
                case 3:
                    currSignBankVid = "everyone.mp4";
                    currDualVid = "everyone.xesc";
                    button4.BackColor = System.Drawing.Color.DarkSlateGray;
                    break;
                case 4:
                    currSignBankVid = "equal.mp4";
                    currDualVid = "equal.xesc";
                    button5.BackColor = System.Drawing.Color.DarkSlateGray;
                    break;
                case 5:
                    currSignBankVid = "improve.mp4";
                    currDualVid = "improve.xesc";
                    button6.BackColor = System.Drawing.Color.DarkSlateGray;
                    break;
                case 6:
                    currSignBankVid = "laptop.mp4";
                    currDualVid = "laptop.xesc";
                    button7.BackColor = System.Drawing.Color.DarkSlateGray;
                    break;
                case 7:
                    currSignBankVid = "middle.mp4";
                    currDualVid = "middle.xesc";
                    button8.BackColor = System.Drawing.Color.DarkSlateGray;
                    break;
                case 8:
                    currSignBankVid = "okay.mp4";
                    currDualVid = "okay.xesc";
                    button9.BackColor = System.Drawing.Color.DarkSlateGray;
                    break;
                case 9:
                    currSignBankVid = "over.mp4";
                    currDualVid = "over.xesc";
                    button10.BackColor = System.Drawing.Color.DarkSlateGray;
                    break;
                case 10:
                    currSignBankVid = "read.mp4";
                    currDualVid = "read.xesc";
                    button11.BackColor = System.Drawing.Color.DarkSlateGray;
                    break;
                case 11:
                    currSignBankVid = "where.mp4";
                    currDualVid = "where.xesc";
                    button12.BackColor = System.Drawing.Color.DarkSlateGray;
                    break;
            }

            if (previousWord != -1 && previousWord != currentWord)
            {
                switch (previousWord)
                {
                    case 0:
                        button1.BackColor = System.Drawing.Color.SteelBlue;
                        break;
                    case 1:
                        button2.BackColor = System.Drawing.Color.SteelBlue;
                        break;
                    case 2:
                        button3.BackColor = System.Drawing.Color.SteelBlue;
                        break;
                    case 3:
                        button4.BackColor = System.Drawing.Color.SteelBlue;
                        break;
                    case 4:
                        button5.BackColor = System.Drawing.Color.SteelBlue;
                        break;
                    case 5:
                        button6.BackColor = System.Drawing.Color.SteelBlue;
                        break;
                    case 6:
                        button7.BackColor = System.Drawing.Color.SteelBlue;
                        break;
                    case 7:
                        button8.BackColor = System.Drawing.Color.SteelBlue;
                        break;
                    case 8:
                        button9.BackColor = System.Drawing.Color.SteelBlue;
                        break;
                    case 9:
                        button10.BackColor = System.Drawing.Color.SteelBlue;
                        break;
                    case 10:
                        button11.BackColor = System.Drawing.Color.SteelBlue;
                        break;
                    case 11:
                        button12.BackColor = System.Drawing.Color.SteelBlue;
                        break;
                }
            }
        }

        private void provideFeedback()
        {
            Random rnd = new Random();
            int HScount = 0;
            int MVcount = 0;
            string HSfeedback = "\n";
            string MVfeedback = "\n";
            int currentIndex = 0;
            int previousIndex = -1;

            switch (currentWord)
            {
                case 0:
                    HScount = rnd.Next(1, 3);
                    for(int i = 0; i < HScount; i++)
                    {
                        currentIndex = rnd.Next(0, 3);
                        if (currentIndex != previousIndex)
                        {
                            HSfeedback += aeroplaneHSFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }

                    }

                    currentIndex = 0;
                    previousIndex = -1;

                    MVcount = rnd.Next(1, 2);
                    for(int i = 0; i < MVcount; i++)
                    {
                        currentIndex = rnd.Next(0, 2);
                        if(currentIndex != previousIndex)
                        {
                            MVfeedback += aeroplaneMVFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }
                    }


                    label5.Text = HSfeedback;
                    label6.Text = MVfeedback;
                    
                    break;


                case 1:
                    HScount = rnd.Next(1, 1);
                    for (int i = 0; i < HScount; i++)
                    {
                        currentIndex = rnd.Next(0, 1);
                        if (currentIndex != previousIndex)
                        {
                            HSfeedback += alwaysHSFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }

                    }

                    currentIndex = 0;
                    previousIndex = -1;

                    MVcount = rnd.Next(1, 2);
                    for (int i = 0; i < MVcount; i++)
                    {
                        currentIndex = rnd.Next(0, 2);
                        if (currentIndex != previousIndex)
                        {
                            MVfeedback += alwaysMVFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }
                    }


                    label5.Text = HSfeedback;
                    label6.Text = MVfeedback;

                    break;


                case 2:
                    HScount = rnd.Next(1, 1);
                    for (int i = 0; i < HScount; i++)
                    {
                        currentIndex = rnd.Next(0, 1);
                        if (currentIndex != previousIndex)
                        {
                            HSfeedback += cleanHSFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }

                    }

                    currentIndex = 0;
                    previousIndex = -1;

                    MVcount = rnd.Next(1, 1);
                    for (int i = 0; i < MVcount; i++)
                    {
                        currentIndex = rnd.Next(0, 1);
                        if (currentIndex != previousIndex)
                        {
                            MVfeedback += cleanMVFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }
                    }


                    label5.Text = HSfeedback;
                    label6.Text = MVfeedback;

                    break;


                case 3:
                    HScount = rnd.Next(1, 2);
                    for (int i = 0; i < HScount; i++)
                    {
                        currentIndex = rnd.Next(0, 2);
                        if (currentIndex != previousIndex)
                        {
                            HSfeedback += everyoneHSFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }

                    }

                    currentIndex = 0;
                    previousIndex = -1;

                    MVcount = rnd.Next(1, 1);
                    for (int i = 0; i < MVcount; i++)
                    {
                        currentIndex = rnd.Next(0, 1);
                        if (currentIndex != previousIndex)
                        {
                            MVfeedback += everyoneMVFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }
                    }


                    label5.Text = HSfeedback;
                    label6.Text = MVfeedback;

                    break;


                case 4:
                    HScount = rnd.Next(1, 3);
                    for (int i = 0; i < HScount; i++)
                    {
                        currentIndex = rnd.Next(0, 3);
                        if (currentIndex != previousIndex)
                        {
                            HSfeedback += equalHSFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }

                    }

                    currentIndex = 0;
                    previousIndex = -1;

                    MVcount = rnd.Next(1, 2);
                    for (int i = 0; i < MVcount; i++)
                    {
                        currentIndex = rnd.Next(0, 2);
                        if (currentIndex != previousIndex)
                        {
                            MVfeedback += equalMVFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }
                    }


                    label5.Text = HSfeedback;
                    label6.Text = MVfeedback;

                    break;


                case 5:
                    HScount = rnd.Next(1, 3);
                    for (int i = 0; i < HScount; i++)
                    {
                        currentIndex = rnd.Next(0, 3);
                        if (currentIndex != previousIndex)
                        {
                            HSfeedback += improveHSFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }

                    }

                    currentIndex = 0;
                    previousIndex = -1;

                    MVcount = rnd.Next(1, 2);
                    for (int i = 0; i < MVcount; i++)
                    {
                        currentIndex = rnd.Next(0, 2);
                        if (currentIndex != previousIndex)
                        {
                            MVfeedback += improveMVFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }
                    }


                    label5.Text = HSfeedback;
                    label6.Text = MVfeedback;

                    break;


                case 6:
                    HScount = rnd.Next(1, 4);
                    for (int i = 0; i < HScount; i++)
                    {
                        currentIndex = rnd.Next(0, 4);
                        if (currentIndex != previousIndex)
                        {
                            HSfeedback += laptopHSFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }

                    }

                    currentIndex = 0;
                    previousIndex = -1;

                    MVcount = rnd.Next(1, 2);
                    for (int i = 0; i < MVcount; i++)
                    {
                        currentIndex = rnd.Next(0, 2);
                        if (currentIndex != previousIndex)
                        {
                            MVfeedback += laptopMVFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }
                    }


                    label5.Text = HSfeedback;
                    label6.Text = MVfeedback;

                    break;


                case 7:
                    HScount = rnd.Next(1, 2);
                    for (int i = 0; i < HScount; i++)
                    {
                        currentIndex = rnd.Next(0, 2);
                        if (currentIndex != previousIndex)
                        {
                            HSfeedback += middleHSFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }

                    }

                    currentIndex = 0;
                    previousIndex = -1;

                    MVcount = rnd.Next(1, 2);
                    for (int i = 0; i < MVcount; i++)
                    {
                        currentIndex = rnd.Next(0, 2);
                        if (currentIndex != previousIndex)
                        {
                            MVfeedback += middleMVFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }
                    }


                    label5.Text = HSfeedback;
                    label6.Text = MVfeedback;

                    break;


                case 8:
                    HScount = rnd.Next(1, 1);
                    for (int i = 0; i < HScount; i++)
                    {
                        currentIndex = rnd.Next(0, 1);
                        if (currentIndex != previousIndex)
                        {
                            HSfeedback += okayHSFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }

                    }

                    currentIndex = 0;
                    previousIndex = -1;

                    MVcount = rnd.Next(1, 2);
                    for (int i = 0; i < MVcount; i++)
                    {
                        currentIndex = rnd.Next(0, 2);
                        if (currentIndex != previousIndex)
                        {
                            MVfeedback += okayMVFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }
                    }


                    label5.Text = HSfeedback;
                    label6.Text = MVfeedback;

                    break;


                case 9:
                    HScount = rnd.Next(1, 1);
                    for (int i = 0; i < HScount; i++)
                    {
                        currentIndex = rnd.Next(0, 1);
                        if (currentIndex != previousIndex)
                        {
                            HSfeedback += overHSFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }

                    }

                    currentIndex = 0;
                    previousIndex = -1;

                    MVcount = rnd.Next(1, 3);
                    for (int i = 0; i < MVcount; i++)
                    {
                        currentIndex = rnd.Next(0, 3);
                        if (currentIndex != previousIndex)
                        {
                            MVfeedback += overMVFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }
                    }


                    label5.Text = HSfeedback;
                    label6.Text = MVfeedback;

                    break;


                case 10:
                    HScount = rnd.Next(1, 3);
                    for (int i = 0; i < HScount; i++)
                    {
                        currentIndex = rnd.Next(0, 3);
                        if (currentIndex != previousIndex)
                        {
                            HSfeedback += readHSFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }

                    }

                    currentIndex = 0;
                    previousIndex = -1;

                    MVcount = rnd.Next(1, 2);
                    for (int i = 0; i < MVcount; i++)
                    {
                        currentIndex = rnd.Next(0, 2);
                        if (currentIndex != previousIndex)
                        {
                            MVfeedback += readMVFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }
                    }


                    label5.Text = HSfeedback;
                    label6.Text = MVfeedback;

                    break;


                case 11:
                    HScount = rnd.Next(1, 1);
                    for (int i = 0; i < HScount; i++)
                    {
                        currentIndex = rnd.Next(0, 1);
                        if (currentIndex != previousIndex)
                        {
                            HSfeedback += whereHSFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }

                    }

                    currentIndex = 0;
                    previousIndex = -1;

                    MVcount = rnd.Next(1, 2);
                    for (int i = 0; i < MVcount; i++)
                    {
                        currentIndex = rnd.Next(0, 2);
                        if (currentIndex != previousIndex)
                        {
                            MVfeedback += whereMVFB[currentIndex] + "\n\n";
                            previousIndex = currentIndex;
                        }
                        else
                        {
                            i--;
                        }
                    }


                    label5.Text = HSfeedback;
                    label6.Text = MVfeedback;

                    break;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Form2 testForm = new Form2();
            testForm.FormBorderStyle = FormBorderStyle.None;
            testForm.WindowState = FormWindowState.Maximized;
            testForm.TopMost = true;
            testForm.Show();
        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            if (animationProcess != null) animationProcess.Kill();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            testResults = new List<resultEntry>();
            for(int i = 0; i < 12; i++)
            {
                calcResult();
            }
            popup resultWindow = new popup(testResults);
            resultWindow.TopMost = true;
            resultWindow.Show();
        }

        private void calcResult()
        {
            //Step1
            int bracketScore = rnd.Next(1, 21);
            int bracket; //Excellent:1, Good:2, Okay:3, Poor:4

            if (bracketScore > 16) bracket = 1;
            else if (bracketScore > 4) bracket = 2;
            else if (bracketScore > 1) bracket = 3;
            else bracket = 4;

            currentWord = signOrder[currentIndex];
            currentIndex++;

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
            int requiredHelp = rnd.Next(1, 3);

            string helpString;
            if (requiredHelp == 1)
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

            int progressBarValue = rnd.Next(1, 4);

            if (progressBarValue == 1)
            {
                timeString = "\nTime used: Less than 30s";
                timePenalty = 0;
            }
            else if (progressBarValue == 2)
            {
                timeString = "\nTime used: Less than 45s (Score = -5%)";
                score -= 5;
                timePenalty = 5;
            }
            else if (progressBarValue == 3)
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


            bool tempval;
            if (requiredHelp == 1) tempval = true;
            else tempval = false;

            resultEntry temp = new resultEntry(currentWord, score, handshapeScore, movementScore, tempval, timePenalty);
            testResults.Add(temp);
        }

    }
}

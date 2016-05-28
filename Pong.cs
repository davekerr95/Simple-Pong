using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Runtime.InteropServices;

namespace Pong
{
    public partial class Form1 : Form
    {
        //SoundPlayer plick = new SoundPlayer(@"plick.wav");
        //SoundPlayer plunk = new SoundPlayer(@"plunk.wav");

        [DllImport("user32.dll")]
        static extern ushort GetAsyncKeyState(int vKey);
        public static bool IsKeyPushedDown(Keys keyData)  // For fast keyboard input
        {
            return 0 != (GetAsyncKeyState((int)keyData) & 0x8000);
        }

        Point Ball;
        Point Ball_Direction;
        int Paddle_Left_Y = 128;
        int Paddle_Right_Y = 128;
        bool Game_paused = true;
        int left_score = 0, right_score = 0;


        public Form1()
        {
            InitializeComponent();
            Ball = new Point(25, 13);
            Ball_Direction = new Point(5, 5);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (IsKeyPushedDown(Keys.W)) Paddle_Left_Y -= 10;
            if (IsKeyPushedDown(Keys.X)) Paddle_Left_Y += 10;
            if (IsKeyPushedDown(Keys.P)) Paddle_Right_Y -= 10;
            if (IsKeyPushedDown(Keys.M)) Paddle_Right_Y += 10;

            if (Paddle_Left_Y < 5) Paddle_Left_Y = 0;  // Clamp paddle positions
            if (Paddle_Left_Y > 251) Paddle_Left_Y = 251;
            if (Paddle_Right_Y < 5) Paddle_Right_Y = 0;
            if (Paddle_Right_Y > 251) Paddle_Right_Y = 251;

            if (IsKeyPushedDown(Keys.Space)) Game_paused = false;

            if (IsKeyPushedDown(Keys.Escape)) Application.Exit();

            // Ball/Paddle collision detection on left paddle
            if ((Ball.X < 30) && (Math.Abs(Ball.Y - Paddle_Left_Y) < 50) && (Ball_Direction.X < 0))
            {
                Ball_Direction.X = -Ball_Direction.X;
            }

            if ((Ball.X > 482) && (Math.Abs(Ball.Y - Paddle_Right_Y) < 50) && (Ball_Direction.X > 0))
            {
                Ball_Direction.X = -Ball_Direction.X;
            }
            if (!Game_paused)
            {
                Ball.X += Ball_Direction.X;
                if ((Ball.X > 511) && (Ball_Direction.X > 0))   // Ball hit back wall travelling left
                {
                    //Ball_Direction.X = -Ball_Direction.X;    // Reverse horizontal motion
                    Ball.X = 30;                             // Restart ball in front of paddle
                    Ball.Y = Paddle_Left_Y;
                    Game_paused = true;
                    left_score++;
                    //plunk.Play();
                }

                if ((Ball.X < 1) && (Ball_Direction.X < 0)) // Ball hit back wall travelling right
                {
                    //Ball_Direction.X = -Ball_Direction.X;    // Reverse horizontal motion
                    Ball.X = 480;                            // Restart ball in front of paddle
                    Ball.Y = Paddle_Right_Y;
                    Game_paused = true;
                    right_score++;
                    //plunk.Play();
                }

                Ball.Y += Ball_Direction.Y;
                if ((Ball.Y > 255) && (Ball_Direction.Y > 0)) // Ball hit bottom wall travelling down
                {
                    Ball_Direction.Y = -Ball_Direction.Y;      // Reverse horizontal motion
                    //plunk.Play();
                }

                if ((Ball.Y < 1) && (Ball_Direction.Y < 0))   // Ball hit top wall travelling down
                {
                    Ball_Direction.Y = -Ball_Direction.Y;	 // Reverse horizontal motion
                    //plunk.Play();
                }
            }


            Bitmap image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(image);
            g.FillEllipse(new SolidBrush(Color.Red), Ball.X - 10, Ball.Y - 10, 20, 20);
            g.FillRectangle(new SolidBrush(Color.Blue), 10, Paddle_Left_Y - 50, 10, 100);
            g.FillRectangle(new SolidBrush(Color.Blue), 490, Paddle_Right_Y - 50, 10, 100);
            g.Dispose();
            label1.Text = left_score.ToString("0.#");
            label2.Text = right_score.ToString("0.#");
            if ((left_score > 2) || (right_score > 2))  // Identify a winner
            {
                timer1.Stop();  // Stop game loop
                String text = "Right";
                if (left_score > right_score) text = "Left";
                DialogResult reply = MessageBox.Show(text + " player wins\rDo you wish to play again?", "Winner", MessageBoxButtons.YesNo);
                if (reply == DialogResult.Yes)
                {
                    left_score = 0;     // Reset game values
                    right_score = 0;
                    Paddle_Left_Y = 128;
                    Paddle_Right_Y = 128;
                    Game_paused = true;
                    Ball.X = 25; Ball.Y = 13;
                    Ball_Direction.X = 3; Ball_Direction.Y = 3;
                    timer1.Start();  // Restart game loop
                }
                else
                {
                    Application.Exit(); // Quit game
                }

            }

            pictureBox1.Image = image;

        }
    }
}

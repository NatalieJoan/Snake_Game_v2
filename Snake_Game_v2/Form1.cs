using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics.Tracing;

namespace Snake_Game_v2
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();

        int maxWidth;
        int maxHeight;

        int scoreGame;
        int highScore;

        private int currentInterval = 100;

        Random rand = new Random();
        bool goLeft, goRight, goUp, goDown;
        public Form1()
        {
            InitializeComponent();
            new Settings();
        }

        private void StartGame(object sender, EventArgs e)
        {
            gameTimer.Interval = 100;
            currentInterval = 100;
            RestartGame();
        }

        private void TakeSnapShot(object sender, EventArgs e)
        {
            Label caption = new Label();
            caption.Text = "Score: " + scoreGame + "  Record: " + highScore;
            caption.Font = new Font("Ariel", 12, FontStyle.Bold);
            caption.ForeColor = Color.DarkBlue;
            caption.AutoSize = false;
            caption.Width = box.Width;
            caption.Height = 40;
            caption.TextAlign = ContentAlignment.MiddleCenter;
            box.Controls.Add(caption);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "Snake Game SnapShot";
            saveFileDialog.DefaultExt = "jpg";
            saveFileDialog.Filter = "JPG Image File | *.jpg";
            saveFileDialog.ValidateNames = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                int width = Convert.ToInt32(box.Width);
                int height = Convert.ToInt32(box.Height);
                Bitmap bmp = new Bitmap(width, height);
                box.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
                bmp.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                box.Controls.Remove(caption);
            }
        }
        private void UpdatePictureBox(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            Brush snakeColor;
            for (int i = 0; i < Snake.Count; i++)
            {
                if (i == 0)
                {
                    snakeColor = Brushes.DarkGreen;
                }
                else
                {
                    snakeColor = Brushes.ForestGreen;
                }
                canvas.FillRectangle(snakeColor, new Rectangle
                (
                    Snake[i].X * Settings.Width,
                    Snake[i].Y * Settings.Height,
                    Settings.Width,
                    Settings.Height
                ));
            }
            canvas.FillEllipse(Brushes.DarkRed, new Rectangle
                (
                    food.X * Settings.Width,
                    food.Y * Settings.Height,
                    Settings.Width,
                    Settings.Height
                ));
  
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && Settings.directions != "right")
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right && Settings.directions != "left")
            {
                goRight = true;
            }
            if (e.KeyCode == Keys.Up && Settings.directions != "up")
            {
                goUp = true;
            }
            if (e.KeyCode == Keys.Down && Settings.directions != "down")
            {
                goDown = true;
            }
        }
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }
        }

        private void GameTimer(object sender, EventArgs e)
        {
            // ustawienie kierunkow
            if (goLeft)
            {
                Settings.directions = "left";
            }
            if (goRight)
            {
                Settings.directions = "right";
            }
            if (goUp)
            {
                Settings.directions = "up";
            }
            if (goDown)
            {
                Settings.directions = "down";
            }

            // ustawienia reszty ciala, w ktora strone ma sie poruszac
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (Settings.directions)
                    {
                        case "left":
                            Snake[i].X--;
                            break;
                        case "right":
                            Snake[i].X++;
                            break;
                        case "up":
                            Snake[i].Y--;
                            break;
                        case "down":
                            Snake[i].Y++;
                            break;
                    }
                    // nastepuje koniec gry po zderzeniu sie weza ze sciana
                    if (Snake[0].X < 0 || Snake[0].X > maxWidth || Snake[0].Y < 0 || Snake[0].Y > maxHeight)
                    {
                        GameOver();
                        return;
                    }

                    if (Snake[i].X < 0)
                    {
                        Snake[i].X = maxWidth;
                    }
                    if (Snake[i].X > maxWidth)
                    {
                        Snake[i].X = 0;
                    }
                    if (Snake[i].Y < 0)
                    {
                        Snake[i].Y = maxHeight;
                    }
                    if (Snake[i].Y > maxHeight)
                    {
                        Snake[i].Y = 0;
                    }

                    if (Snake[i].X == food.X && Snake[i].Y == food.Y)
                    {
                        EatFood();
                    }
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X && Snake[i].Y == Snake[j].Y)
                        {
                            GameOver();
                        }
                    }
                }
                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
            box.Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void gameOverLabel_Click(object sender, EventArgs e)
        {

        }

        private void RestartGame()
        {
            maxWidth = box.Width / Settings.Width - 1;
            maxHeight = box.Height / Settings.Height - 1;

            Snake.Clear();
            gameOverLabel.Text = "";

            startButton.Enabled = false;
            snapButton.Enabled = false;

            scoreGame = 0;
            scoreLabel.Text = "Score: " + scoreGame;

            Circle head = new Circle { X = 10, Y = 5 };     // umiejscowanie glowy snake na pozycji (10,5)
            Snake.Add(head);

            for (int i = 0; i < 10; i++)
            {
                Circle body = new Circle();
                Snake.Add(body);
            }
            food = new Circle { X = rand.Next(2, maxWidth), Y = rand.Next(2, maxHeight) };
            gameTimer.Start();
        }
        private void EatFood()
        {
            scoreGame += 1;
            scoreLabel.Text = "Score: " + scoreGame;

            Circle body = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };

            Snake.Add(body);
            food = new Circle { X = rand.Next(2, maxWidth), Y = rand.Next(2, maxHeight) };

            // proces przyspieszenia weza po kazdym kolejnym jedzeniu
            currentInterval -= 5;

            if (currentInterval < 40)
            {
                currentInterval = 40;
            }
            gameTimer.Interval = currentInterval;
        }
        private void GameOver()
        {
            gameTimer.Stop();
            startButton.Enabled = true;
            snapButton.Enabled = true;
            gameOverLabel.Text = "Game Over";

            if (scoreGame > highScore)
            {
                highScore = scoreGame;
                highScoreLabel.Text = "Record: " + highScore;
                highScoreLabel.ForeColor = Color.Maroon;
                highScoreLabel.Font = new Font(highScoreLabel.Font, FontStyle.Bold);
                highScoreLabel.TextAlign = ContentAlignment.MiddleCenter;
            }
        }
    }
}

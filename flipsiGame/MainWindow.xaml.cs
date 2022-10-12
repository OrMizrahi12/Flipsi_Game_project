using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace flipsiGame
{


    public partial class MainWindow : Window
    {
        DispatcherTimer GameTimer = new DispatcherTimer();
        Random random = new Random();
        ImageBrush playerSprit = new ImageBrush(), backgroundSprit = new ImageBrush(),opbstacleSprit = new ImageBrush(), opbstacleUpSprit = new ImageBrush(), florSprit = new ImageBrush();

        public Rect playerHitBox, groundHitBox, groundHitBox2, opbstacleHitBox, opbstacleUpHitBox;
        bool playerJump, gameOver, canClickForStart = true, playerSlide = false;
        int playerForce = 20, playerSpeed = 5, score = 0,jumpCounter = 0, distabce =0, speedGame = 8;
        int[] opbstaclePosition = { 320, 310, 300, 305, 315 };
        double spriteIndex =0;
        
        public MainWindow()
        {
            InitializeComponent();
            InitialGame();
            InitialGameDesign();
        }

        private void InitialGame()
        {
            myCanvas.Focus();
            GameTimer.Tick += GameEngine;
            GameTimer.Interval = TimeSpan.FromMilliseconds(20);
        }

        public void GameEngine(object sender, EventArgs e)
        {
            RunAnimation();
            SetRects();
            PosesObstacles();
            EnforcesGameRules();
            EnforcesPlayerRules();
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && gameOver)        
                StartGame();        
            else if(e.Key == Key.Down)
                playerSlide = true;
            else if (e.Key == Key.Enter && !gameOver && canClickForStart)
            {
                canClickForStart = false;
                StartGame();
            }
            if (e.Key == Key.Space && playerJump == false && Canvas.GetTop(player) > 250 )
            {
                jumpCounter = 1;
                playerJump = true;
                playerForce = 15;
                playerSpeed = -12;
                playerSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_02.gif"));
            }
            else if(jumpCounter == 1 && e.Key == Key.Space)
            {
                jumpCounter = 0;
                playerJump = true;
                playerForce = 10;
                playerSpeed = -12;
                playerSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_03.gif"));
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
                playerSlide = false;
        }

        private void RunAnimation()
        {
            Canvas.SetLeft(background1, Canvas.GetLeft(background1) - speedGame);
            Canvas.SetLeft(background2, Canvas.GetLeft(background2) - speedGame);
            Canvas.SetLeft(ground, Canvas.GetLeft(background1) - speedGame);
            Canvas.SetLeft(ground2, Canvas.GetLeft(background2) - speedGame);
            Canvas.SetTop(player, Canvas.GetTop(player) + playerSpeed);
            Canvas.SetLeft(obstacle, Canvas.GetLeft(obstacle) - speedGame);
            Canvas.SetLeft(obstacleUp, Canvas.GetLeft(obstacleUp) - speedGame);

            distabce++;
            scoreText.Content = distabce;

            if(distabce % 500 == 0)
            {
                ++speedGame;

            }

            if (Canvas.GetLeft(background1) < -1261)
            {
                if (Canvas.GetLeft(obstacleUp) > 0 && Canvas.GetLeft(obstacle) > 0)
                    ground.Width = random.Next(1050, 1100);

                Canvas.SetLeft(background1, Canvas.GetLeft(background2) + background2.Width);
            }
            if (Canvas.GetLeft(background2) < -1262) Canvas.SetLeft(background2, Canvas.GetLeft(background1) + background1.Width);
            if (Canvas.GetLeft(ground) < -1262) Canvas.SetLeft(ground, Canvas.GetLeft(ground2) + ground2.Width); 
            if (Canvas.GetLeft(ground2) < -1262) Canvas.SetLeft(ground2, Canvas.GetLeft(ground) + ground.Width);
        }

        private void EnforcesGameRules()
        {
            if (playerHitBox.IntersectsWith(opbstacleHitBox) && Canvas.GetLeft(player) < Canvas.GetLeft(obstacle))
                GameOver();
            else if (playerHitBox.IntersectsWith(opbstacleUpHitBox) && !playerSlide && Canvas.GetLeft(player) < Canvas.GetLeft(obstacleUp) && Canvas.GetTop(player) > 150)
                GameOver();
            if (Canvas.GetTop(player) > Canvas.GetTop(ground) || Canvas.GetTop(player) > Canvas.GetTop(ground2))
                GameOver();
        }

        private void EnforcesPlayerRules()
        {
            if (playerHitBox.IntersectsWith(groundHitBox) || playerHitBox.IntersectsWith(groundHitBox2))
            {
                Canvas.SetTop(player, Canvas.GetTop(ground) - player.Height);
                playerSpeed = 0; playerJump = false;
                spriteIndex += .5;

                if (spriteIndex > 8) spriteIndex = 1;
                if (playerSlide == false)  RunSprite(spriteIndex);
                else RunSprite(9);
            }
            if (playerJump)
            {
                playerSpeed = -9;
                playerForce -= 1;
            }
            else playerSpeed = 12;
            if (playerForce < 0) playerJump = false;
        }

        private void SetRects()
        {
            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width - 15, player.Height);
            groundHitBox = new Rect(Canvas.GetLeft(ground), Canvas.GetTop(ground), ground.Width, ground.Height);
            groundHitBox2 = new Rect(Canvas.GetLeft(ground2), Canvas.GetTop(ground2), ground2.Width, ground2.Height);
            opbstacleHitBox = new Rect(Canvas.GetLeft(obstacle), Canvas.GetTop(obstacle), obstacle.Width - 20, obstacle.Height);
            opbstacleUpHitBox = new Rect(Canvas.GetLeft(obstacleUp), Canvas.GetTop(obstacleUp), obstacleUp.Width, obstacleUp.Height);
        }

        private void StartGame()
        {
            Canvas.SetLeft(background1, 0); Canvas.SetLeft(background2, 1260);
            Canvas.SetLeft(player, 100); Canvas.SetTop(player, 140);
            Canvas.SetLeft(obstacle, 1000); Canvas.SetTop(obstacle, 300);
            Canvas.SetLeft(obstacleUp, 1500); Canvas.SetTop(obstacleUp, 0);

            gameControllText.Visibility = Visibility.Collapsed;
            opbstacleSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/obstacle.png"));
            obstacle.Fill = opbstacleSprit;

            playerJump = false;
            gameOver = false;
            score = 0;

            RunSprite(1);
            GameTimer.Start();
        }

        private void PosesObstacles()
        {
            if (Canvas.GetLeft(obstacleUp) < -50 && Canvas.GetLeft(obstacle) < -50)
            {
                Canvas.SetLeft(obstacle, -60);
                Canvas.SetLeft(obstacleUp, -60);

                if (random.Next(0, 3) % 2 == 0)
                {
                    Canvas.SetLeft(obstacle, random.Next(1000, 1250)); Canvas.SetTop(obstacle, opbstaclePosition[random.Next(0, opbstaclePosition.Length)]);
                    Canvas.SetLeft(obstacleUp, random.Next(1400, 1500)); Canvas.SetTop(obstacleUp, 0);
                }
                else
                {
                    Canvas.SetLeft(obstacle, random.Next(1400, 1500)); Canvas.SetTop(obstacle, opbstaclePosition[random.Next(0, opbstaclePosition.Length)]);
                    Canvas.SetLeft(obstacleUp, random.Next(1000, 1250)); Canvas.SetTop(obstacleUp, 0);
                }
            }
        }
        private void RunSprite(double i)
        {
            switch (i)
            {
                case 1:
                    playerSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_01.gif"));
                    break;
                case 2:
                    playerSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_02.gif"));
                    break;
                case 3:
                    playerSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_03.gif"));
                    break;
                case 4:
                    playerSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_04.gif"));
                    break;
                case 5:
                    playerSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_05.gif"));
                    break;
                case 6:
                    playerSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_06.gif"));
                    break;
                case 7:
                    playerSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_07.gif"));
                    break;
                case 8:
                    playerSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_08.gif"));
                    break;
                case 9:
                    playerSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunnerSlide.png"));
                    playerHitBox.Height = playerHitBox.Width;
                    break;
            }
            player.Fill = playerSprit;
        }

        private void InitialGameDesign()
        {
            backgroundSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/J.jpg"));
            background1.Fill = backgroundSprit;
            background2.Fill = backgroundSprit;
            florSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/florF.png"));
            ground.Fill = florSprit;
            ground2.Fill = florSprit;
            opbstacleUpSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/obstacleUpAImg.png"));
            obstacleUp.Fill = opbstacleUpSprit;
            opbstacleSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/obstacle.png"));
            obstacle.Fill = opbstacleSprit;
        }

        private void GameOver()
        {
            GameTimer.Stop();
            gameOver = true;
            gameControllText.Visibility = Visibility.Visible;
            gameControllText.Content = "Enter For Play Again >>";
            score = 0;
            speedGame = 8;
            distabce = 0;
            scoreText.Content = $"Score: {score}";
        }
    }
}

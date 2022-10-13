using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
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
        ImageBrush playerSprit = new ImageBrush(), lifeSpirit = new ImageBrush(), backgroundSprit = new ImageBrush(),opbstacleSprit = new ImageBrush(), opbstacleUpSprit = new ImageBrush(), florSprit = new ImageBrush();

        public Rect playerHitBox, groundHitBox, groundHitBox2, opbstacleHitBox, opbstacleUpHitBox;
        bool playerJump, gameOver, canClickForStart = true, playerSlide = false, stopGame = true, firstPressSpace = false, firstPressDown = false ;
        int playerForce = 20, playerSpeed = 5, jumpCounter = 0, distabce = 0, speedGame = 12, level = 1, lifePlayer = 100;
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
            InfomationTxtController();
            LevelSwitcher();
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && gameOver)        
                StartGame();        
            else if(e.Key == Key.Down && playerHitBox.IntersectsWith(groundHitBox) == true || e.Key == Key.Down && playerHitBox.IntersectsWith(groundHitBox2) == true)
            {
                playerSlide = true;
                firstPressDown = true;
            }
            else if (e.Key == Key.Enter && !gameOver && canClickForStart)
            {
                canClickForStart = false;
                StartGame();
            }
            if (e.Key == Key.Space && playerJump == false && Canvas.GetTop(player) > 250 )
            {
                firstPressSpace = true;
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
            if(e.Key == Key.S && stopGame)
            {
                stopGame = false;
                GameTimer.Stop();
            } 
            else if (e.Key == Key.S && !stopGame)
            {
                stopGame = true;
                GameTimer.Start();
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

                if (Canvas.GetLeft(background1) < -1262)
                {
                    if (Canvas.GetLeft(obstacleUp) > 0 && Canvas.GetLeft(obstacle) > 0)
                        ground.Width = random.Next(950, 1050);
                    Canvas.SetLeft(background1, Canvas.GetLeft(background2) + background2.Width);
                }
                if (Canvas.GetLeft(background2) < -1262)
                {
                    Canvas.SetLeft(background2, Canvas.GetLeft(background1) + background1.Width);
                    ground2.Width = random.Next(950, 1000);
                }             
                if (Canvas.GetLeft(ground) < -1262) Canvas.SetLeft(ground, Canvas.GetLeft(ground2) + ground2.Width);
                if (Canvas.GetLeft(ground2) < -1262) Canvas.SetLeft(ground2, Canvas.GetLeft(ground) + ground.Width);          
        }

        private void EnforcesGameRules()
        {
            if (playerHitBox.IntersectsWith(opbstacleHitBox) && Canvas.GetLeft(player) < Canvas.GetLeft(obstacle))
            {
                lifePlayer -= 5;
                if (lifePlayer <= 0)
                    GameOver();
            }
            else if (playerHitBox.IntersectsWith(opbstacleUpHitBox) && !playerSlide && Canvas.GetLeft(player) < Canvas.GetLeft(obstacleUp) && Canvas.GetTop(player) > 150)
            {
                lifePlayer-=5;
                if (lifePlayer <= 0)
                    GameOver();
            }
            else if (Canvas.GetTop(player) > Canvas.GetTop(ground) || Canvas.GetTop(player) > Canvas.GetTop(ground2))
            {
                lifePlayer -= 5;
                if (lifePlayer <= 0)
                    GameOver();
            }
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

            if (firstPressSpace == false && distabce > 60)           
                gameControllText.Content = "Press SPACE!";          
            else if (firstPressSpace && !firstPressDown && distabce > 110)           
                gameControllText.Content = "Now Press DOWN!";         
            else if (firstPressDown && firstPressSpace)
                gameControllText.Visibility = Visibility.Collapsed;
        }

        private void InfomationTxtController()
        {
            distabce++;
            scoreText.Content = $"Distance: {distabce}";
            if (distabce % 500 == 0)
                ++speedGame;
            levelText.Content = $"Level: {level}";
            lifePrograsser.Value = lifePlayer;
        }
        private void SetRects()
        {
            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width - 15, player.Height);
            groundHitBox = new Rect(Canvas.GetLeft(ground), Canvas.GetTop(ground), ground.Width, ground.Height);
            groundHitBox2 = new Rect(Canvas.GetLeft(ground2), Canvas.GetTop(ground2), ground2.Width, ground2.Height);
            opbstacleHitBox = new Rect(Canvas.GetLeft(obstacle), Canvas.GetTop(obstacle), obstacle.Width - 20, obstacle.Height);
            opbstacleUpHitBox = new Rect(Canvas.GetLeft(obstacleUp), Canvas.GetTop(obstacleUp), obstacleUp.Width, obstacleUp.Height);

            RenderOptions.SetBitmapScalingMode(player, BitmapScalingMode.LowQuality);
            RenderOptions.SetBitmapScalingMode(ground, BitmapScalingMode.LowQuality);
            RenderOptions.SetBitmapScalingMode(ground2, BitmapScalingMode.LowQuality);
            RenderOptions.SetBitmapScalingMode(obstacle, BitmapScalingMode.LowQuality);
            RenderOptions.SetBitmapScalingMode(obstacleUp, BitmapScalingMode.LowQuality);
        }

        private void StartGame()
        {
            Canvas.SetLeft(background1, 0); Canvas.SetLeft(background2, 1260);
            Canvas.SetLeft(player, 100); Canvas.SetTop(player, 140);
            Canvas.SetLeft(obstacle, 1000); Canvas.SetTop(obstacle, 300);
            Canvas.SetLeft(obstacleUp, 1500); Canvas.SetTop(obstacleUp, 0);

            gameOverTxt.Visibility = Visibility.Collapsed;
            gameControllText.Content = "";
            opbstacleSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/obstacle.png"));
            obstacle.Fill = opbstacleSprit;

            playerJump = false; gameOver = false;
            lifePlayer = 100; speedGame = 12; distabce = 0;
          
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
            for (int x = 1; x <= 8; x++)           
                if(x == i)
                    playerSprit.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/images/newRunner_0{i}.gif"));
            if (i == 9)
                playerSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunnerSlide.png"));

            player.Fill = playerSprit;
        }

        private void InitialGameDesign()
        {
            backgroundSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/P.jpg"));
            background1.Fill = backgroundSprit;
            background2.Fill = backgroundSprit;
            florSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/florF.png"));
            ground.Fill = florSprit;
            ground2.Fill = florSprit;
            opbstacleUpSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/obstacleUpAImg.png"));
            obstacleUp.Fill = opbstacleUpSprit;
            opbstacleSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/obstacle.png"));
            obstacle.Fill = opbstacleSprit;
            lifeSpirit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/lifePlayer.png"));
            lifePlayerImg.Fill = lifeSpirit;

            RenderOptions.SetBitmapScalingMode(player, BitmapScalingMode.LowQuality);
            RenderOptions.SetBitmapScalingMode(ground, BitmapScalingMode.LowQuality);
            RenderOptions.SetBitmapScalingMode(ground2, BitmapScalingMode.LowQuality);
            RenderOptions.SetBitmapScalingMode(obstacle, BitmapScalingMode.LowQuality);
            RenderOptions.SetBitmapScalingMode(obstacleUp, BitmapScalingMode.LowQuality);
        }

        private void LevelSwitcher()
        {
            if(distabce < 1000)
            {
                backgroundSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/P.jpg"));
                level = 1;
            }
            else if(distabce >= 1000 && distabce <= 2000)
            {
                backgroundSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/C.jpg"));
                level = 2;
            }
            else if (distabce >= 2000 && distabce <= 3000)
            {
                backgroundSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/E.jpg"));
                level = 3;
            }
            else if (distabce >= 3000 && distabce <= 4000)
            {
                backgroundSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/G.jpg"));
                level = 4;
            }
            else if (distabce >= 4000 && distabce <= 5000 )
            {
                backgroundSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/H.jpg"));
                level = 5;
            }
            else if (distabce >= 5000 )
            {
                backgroundSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/N.jpg"));
                level = 6;
            }

            background1.Fill = backgroundSprit;
            background2.Fill = backgroundSprit;
        }


        private void GameOver()
        {
            GameTimer.Stop();
            gameOver = true;
            gameOverTxt.Visibility = Visibility.Visible;
            gameOverTxt.Content = "GAME OVER | Press Enter";


        }
    }
}

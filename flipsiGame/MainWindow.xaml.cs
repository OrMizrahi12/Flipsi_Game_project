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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        DispatcherTimer GameTimer = new DispatcherTimer();

        public Rect playerHitBox;
        public Rect groundHitBox;
        public Rect groundHitBox2;
        public Rect opbstacleHitBox;
        public Rect opbstacleUpHitBox;


        bool jumping;
        bool gameOver;
        bool canClickForStart = true;
        bool playerSlide = false;

        int force = 20;
        int speed = 5;
        int score = 0;

        double spriteIndex =0;
        
        Random random = new Random();
        Random randomGap = new Random();

        ImageBrush playerSprit = new ImageBrush();
        ImageBrush backgroundSprit = new ImageBrush();
        ImageBrush opbstacleSprit = new ImageBrush();
        ImageBrush opbstacleUpSprit = new ImageBrush();

        ImageBrush florSprit = new ImageBrush();

        int[] opbstaclePosition = { 320, 310, 300, 305, 315 };


        public MainWindow()
        {
            InitializeComponent();
            myCanvas.Focus();
            GameTimer.Tick += GameEngine;
            GameTimer.Interval = TimeSpan.FromMilliseconds(20);

            backgroundSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/J.jpg"));
            background1.Fill = backgroundSprit;
            background2.Fill = backgroundSprit;
            florSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/florF.png"));
            ground.Fill = florSprit;
            ground2.Fill = florSprit;
            opbstacleUpSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/obstacleUpAImg.png"));
            obstacleUp.Fill = opbstacleUpSprit;


        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && gameOver)
            {
                StartGame();
                gameControllText.Visibility = Visibility.Collapsed;
            }
            if (e.Key == Key.Enter && !gameOver && canClickForStart)
            {
                canClickForStart = false;
                gameControllText.Visibility = Visibility.Collapsed;
                StartGame();
            }
            if(e.Key == Key.Down)
            {
                playerHitBox.Width = playerHitBox.Height;
                playerHitBox.Height = playerHitBox.Width;
                playerSlide = true;
            }



        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space && jumping == false && Canvas.GetTop(player) > 250)
            {
                jumping = true;
                force = 15;
                speed = -12;
                playerSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_02.gif"));
            }
            if (e.Key == Key.Down)
            {

                playerSlide = false;
            }

        }

        public void GameEngine(object sender, EventArgs e)
        {

           


            Canvas.SetLeft(background1, Canvas.GetLeft(background1) - 7);
            Canvas.SetLeft(background2, Canvas.GetLeft(background2) - 7);

            Canvas.SetLeft(ground, Canvas.GetLeft(background1) - 7);
            Canvas.SetLeft(ground2, Canvas.GetLeft(background2) - 7);

            Canvas.SetTop(player, Canvas.GetTop(player) + speed);
            Canvas.SetLeft(obstacle, Canvas.GetLeft(obstacle) - 7);
            Canvas.SetLeft(obstacleUp, Canvas.GetLeft(obstacleUp) - 7);

            if (Canvas.GetLeft(background1) < -1262)
            {
                //ground.width = randomgap.next(1000, 1200);
                if(Canvas.GetLeft(obstacleUp) > 0 && Canvas.GetLeft(obstacle) > 0)
                ground.Width = random.Next(1000, 1150);

                Canvas.SetLeft(background1, Canvas.GetLeft(background2) + background2.Width);
            }
            if (Canvas.GetLeft(background2) < -1262)
            {
                Canvas.SetLeft(background2, Canvas.GetLeft(background1) + background1.Width);
            }

            if (Canvas.GetLeft(ground) < -1262)
            {
                Canvas.SetLeft(ground, Canvas.GetLeft(ground2) + ground2.Width );
            }
            if (Canvas.GetLeft(ground2) < -1262)
            {
                Canvas.SetLeft(ground2, Canvas.GetLeft(ground) + ground.Width);
            }




            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width - 15, player.Height);
            opbstacleHitBox = new Rect(Canvas.GetLeft(obstacle), Canvas.GetTop(obstacle), obstacle.Width, obstacle.Height);
            groundHitBox = new Rect(Canvas.GetLeft(ground), Canvas.GetTop(ground), ground.Width , ground.Height);
            groundHitBox2 = new Rect(Canvas.GetLeft(ground2), Canvas.GetTop(ground2), ground2.Width, ground2.Height);
            opbstacleUpHitBox = new Rect(Canvas.GetLeft(obstacleUp), Canvas.GetTop(obstacleUp), obstacleUp.Width, obstacleUp.Height);

            if (playerHitBox.IntersectsWith(groundHitBox) || playerHitBox.IntersectsWith(groundHitBox2))
            {
                speed = 0;
                Canvas.SetTop(player, Canvas.GetTop(ground) - player.Height);

                jumping = false;
                spriteIndex += .5;

                if (spriteIndex > 8) spriteIndex = 1;

                if(playerSlide == false)
                    RunSprite(spriteIndex);

                 if(playerSlide == true)
                    RunSprite(9);
            }      
            if (Canvas.GetTop(player) > Canvas.GetTop(ground) || Canvas.GetTop(player) > Canvas.GetTop(ground2)) 
            {
                GameTimer.Stop();
                gameOver = true;
                gameControllText.Visibility = Visibility.Visible;
                gameControllText.Content = "Enter For Play Again >>";
            } 

            if (jumping)
            {
                speed = -9;
                force -= 1;
            }
            else speed = 12;
            
            if(force < 0) jumping = false;

            double gapBetweenObsacla = Canvas.GetLeft(obstacleUp) - Canvas.GetLeft(obstacle);


            //    score += 1;

            //        scoreText.Content = $"Score: {score}";

            if (Canvas.GetLeft(obstacleUp) < -50 && Canvas.GetLeft(obstacle) < -50)
            {

                Canvas.SetLeft(obstacle, -60);
                Canvas.SetLeft(obstacleUp, -60);
              
                if(random.Next(0,3) % 2 == 0)
                {
                    Canvas.SetLeft(obstacle, random.Next(1000, 1250));
                    Canvas.SetTop(obstacle, opbstaclePosition[random.Next(0, opbstaclePosition.Length)]);

                    Canvas.SetLeft(obstacleUp, random.Next(1400, 1500));
                    Canvas.SetTop(obstacleUp, 0);
                }
                else
                {
                    Canvas.SetLeft(obstacle, random.Next(1400, 1500));
                    Canvas.SetTop(obstacle, opbstaclePosition[random.Next(0, opbstaclePosition.Length)]);

                    Canvas.SetLeft(obstacleUp, random.Next(1000, 1250));
                    Canvas.SetTop(obstacleUp, 0);
                }
             
                
            }

            if (playerHitBox.IntersectsWith(opbstacleHitBox) && Canvas.GetLeft(player) < Canvas.GetLeft(obstacle))
            {
                GameTimer.Stop();
                gameOver = true;
                gameControllText.Visibility = Visibility.Visible;
                gameControllText.Content = "Enter For Play Again >>";
                score = 0;
                scoreText.Content = $"Score: {score}";
            }
            else if (playerHitBox.IntersectsWith(opbstacleUpHitBox) && !playerSlide && Canvas.GetLeft(player) < Canvas.GetLeft(obstacleUp))
            {
                GameTimer.Stop();
                gameOver = true;
                gameControllText.Visibility = Visibility.Visible;
                gameControllText.Content = "Enter For Play Again >>";
                score = 0;
                scoreText.Content = $"Score: {score}";
            }

        }

        private void StartGame()
        {
            Canvas.SetLeft(background1, 0);
            Canvas.SetLeft(background2, 1260);

            Canvas.SetLeft(player, 100);
            Canvas.SetTop(player, 140);

            Canvas.SetLeft(obstacle, 1000);
            Canvas.SetTop(obstacle, 300);

            Canvas.SetLeft(obstacleUp, 1500);
            Canvas.SetTop(obstacleUp, 0);

            RunSprite(1);

            opbstacleSprit.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/obstacleB.png"));
            obstacle.Fill = opbstacleSprit;

            jumping = false;
            gameOver = false;
            score = 0;

            GameTimer.Start();
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

        
    }
}

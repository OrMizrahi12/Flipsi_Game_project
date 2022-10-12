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

        bool jumping;
        bool gameOver;
        bool canClickForStart = true;

        int force = 20;
        int speed = 5;
        int score = 0;

        double spriteIndex =0;
        
        Random random = new Random();
        Random randomGap = new Random();
        ImageBrush playerSprit = new ImageBrush();
        ImageBrush backgroundSprit = new ImageBrush();
        ImageBrush opbstacleSprit = new ImageBrush();
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
        }

        public void GameEngine(object sender, EventArgs e)
        {
           
            Canvas.SetLeft(background1, Canvas.GetLeft(background1) - 7);
            Canvas.SetLeft(background2, Canvas.GetLeft(background2) - 7);

            Canvas.SetLeft(ground, Canvas.GetLeft(background1) - 7);
            Canvas.SetLeft(ground2, Canvas.GetLeft(background2) - 7);

            Canvas.SetLeft(obstacle, Canvas.GetLeft(obstacle));
          

            if (Canvas.GetLeft(background1) < -1262)
            {
                ground.Width = randomGap.Next(1000, 1200);
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


            Canvas.SetTop(player, Canvas.GetTop(player) + speed);
            Canvas.SetLeft(obstacle, Canvas.GetLeft(obstacle) - 12);

            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width - 15, player.Height);
            opbstacleHitBox = new Rect(Canvas.GetLeft(obstacle), Canvas.GetTop(obstacle), obstacle.Width, obstacle.Height);
            groundHitBox = new Rect(Canvas.GetLeft(ground), Canvas.GetTop(ground), ground.Width , ground.Height);
            groundHitBox2 = new Rect(Canvas.GetLeft(ground2), Canvas.GetTop(ground2), ground2.Width, ground2.Height);


            if (playerHitBox.IntersectsWith(groundHitBox) || playerHitBox.IntersectsWith(groundHitBox2))
            {
                speed = 0;
                Canvas.SetTop(player, Canvas.GetTop(ground) - player.Height);

                jumping = false;
                spriteIndex += .5;

                if (spriteIndex > 8) spriteIndex = 1;

                RunSprite(spriteIndex);
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


            if (Canvas.GetLeft(obstacle) < -100)
            {
                score += 1;
                scoreText.Content = $"Score: {score}";
                Canvas.SetLeft(obstacle, 950);
                Canvas.SetTop(obstacle, opbstaclePosition[random.Next(0, opbstaclePosition.Length)]);
            }

            if (playerHitBox.IntersectsWith(opbstacleHitBox))
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

            Canvas.SetLeft(obstacle, 950);
            Canvas.SetTop(obstacle, 300);

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
            }

            player.Fill = playerSprit;
        }

        
    }
}

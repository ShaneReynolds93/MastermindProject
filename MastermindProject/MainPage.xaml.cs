using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MastermindProject
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        int turn = 0;
        public MainPage()
        {
            this.InitializeComponent();

            PlaneProjection pp = new PlaneProjection(); // this is for tilting the gameBoard back for realism/perspective
            pp.RotationX = -0;

            StackPanel gameBoard = new StackPanel();
            gameBoard.Width = 500;
            gameBoard.HorizontalAlignment = HorizontalAlignment.Center;
            gameBoard.Orientation = Orientation.Horizontal;

            StackPanel userInput = new StackPanel();
            userInput.Name = "userInputPanel";

            // setting the colors of the ellipses/pegs that will be made available in the game
            List<SolidColorBrush> colorList = new List<SolidColorBrush>();
            colorList.Add(new SolidColorBrush(Colors.Red));
            colorList.Add(new SolidColorBrush(Colors.Orange));
            colorList.Add(new SolidColorBrush(Colors.Yellow));
            colorList.Add(new SolidColorBrush(Colors.Green));
            colorList.Add(new SolidColorBrush(Colors.Blue));
            colorList.Add(new SolidColorBrush(Colors.Violet));
            colorList.Add(new SolidColorBrush(Colors.PaleVioletRed));

            // this will allow the user to choose their desired peg/ellipse colour for a turn from the list defined above
            foreach (var c in colorList)
            {
                Ellipse colorChoice;
                colorChoice = new Ellipse();
                colorChoice.Fill = c;
                colorChoice.Height = 50;
                colorChoice.Width = 50;
                colorChoice.Tapped += colorChoice_Tapped;

                Debug.WriteLine("here" + colorChoice.Name);
                userInput.Children.Add(colorChoice);
            }

            userInput.Visibility = Visibility.Collapsed;

            StackPanel vertical = new StackPanel();
            gameBoard.Children.Add(userInput);
            gameBoard.Children.Add(vertical);


            // START OF STACKPANEL AND ELLIPSES FOR THE GAME SOLUTION/PEG ANSWER
            StackPanel solution = new StackPanel();
            solution.Name = "RevealSolution";
            solution.Orientation = Orientation.Horizontal;
            solution.Visibility = Visibility.Collapsed;
            Random random = new Random();

            for (int s = 0; s < 4; s++)
            {
                Ellipse boardSolution;
                boardSolution = new Ellipse();

                boardSolution.Fill = colorList[random.Next(0, 7)];
                boardSolution.Height = 50;
                boardSolution.Width = 50;
                solution.Children.Add(boardSolution);
                boardSolution.Name = "Solution" + s;
            }

            vertical.Children.Add(solution);
            vertical.Projection = pp;

            // END OF STACKPANEL AND ELLIPSES FOR THE GAME SOLUTION


            // START OF STACKPANEL AND ELLIPSES FOR THE USERS TURNS
            for (int t = 9; t >= 0; t--) // the amount of turns that the user has to guess
            {
                StackPanel turnPanel = new StackPanel();
                turnPanel.Orientation = Orientation.Horizontal;
                turnPanel.VerticalAlignment = VerticalAlignment.Center;
                turnPanel.HorizontalAlignment = HorizontalAlignment.Center;
                turnPanel.Background = new SolidColorBrush(Colors.Beige);

                StackPanel panelEllipse = new StackPanel();
                panelEllipse.Orientation = Orientation.Horizontal;

                Grid resultsGrid = new Grid();

                for (int i = 0; i < 4; i++) // the pegs that the user is choosing for the turn
                {
                    Ellipse turnChoice = new Ellipse();
                    turnChoice.Fill = new SolidColorBrush(Colors.White);
                    turnChoice.Stroke = new SolidColorBrush(Colors.Black);
                    turnChoice.Height = 50;
                    turnChoice.Width = 50;
                    turnChoice.Margin = new Thickness(5);
                    panelEllipse.Children.Add(turnChoice);
                    turnChoice.Name = "turn" + t + "_Peg" + i; // to check turn and peg placements
                    turnChoice.Tapped += TurnChoice_Tapped;
                }
                turnPanel.Children.Add(panelEllipse);
                // END OF PANELS AND ELLIPSES FOR THE USERS TURNS


                // START OF PANELS AND ELLIPSES FOR TURN FEEDBACK 
                for (int i = 0; i < 2; i++)
                {
                    resultsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    resultsGrid.RowDefinitions.Add(new RowDefinition());
                }

                int counter = 0;
                for (int row = 0; row < 2; row++)
                {
                    for (int column = 0; column < 2; column++)
                    {
                        Ellipse turnFeedback = new Ellipse()
                        {
                        };

                        turnFeedback.Fill = new SolidColorBrush(Colors.Gray);
                        turnFeedback.Height = 25;
                        turnFeedback.Width = 25;
                        turnFeedback.Margin = new Thickness(2);
                        turnFeedback.SetValue(Grid.RowProperty, row);
                        turnFeedback.SetValue(Grid.ColumnProperty, column);
                        resultsGrid.Children.Add(turnFeedback);
                        turnFeedback.Name = "result" + t + "_" + counter++;
                    }
                }
                // END OF PANELS AND ELLIPSES FOR TURN FEEDBACK

                turnPanel.Children.Add(resultsGrid);
                vertical.Children.Add(turnPanel);
            }

            // Creates a button that allows the user to finalize their guess for the current turn and moves them onto the next
            board.Children.Add(gameBoard);
            Button button = new Button();
            button.Content = "Next Turn";
            board.Children.Add(button);
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            List<Ellipse> senders = new List<Ellipse>();

            for (int i = 0; i < 4; i++)
            {
                senders.Add(new Ellipse
                {
                    Fill = ((Ellipse)FindName("turn" + turn + "_" + i)).Fill
                } );
            }

            List<Ellipse> results = new List<Ellipse>();
            for (int i = 0; i < 4; i++)
            {
                results.Add(new Ellipse
                {
                    Fill = ((Ellipse)FindName("solution" + i)).Fill
                } );
            }
        }


        private void colorChoice_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Ellipse toChange = (Ellipse)FindName(senderToChange);
            Ellipse ori = (Ellipse)sender;

            toChange.Fill = ori.Fill;
            StackPanel inputPanel = (StackPanel)FindName("inputPanel");
            inputPanel.Visibility = Visibility.Collapsed;
        }


        String senderToChange = "";
        private void TurnChoice_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Ellipse clicked = (Ellipse)sender;
            Debug.WriteLine(clicked.Name);
            int row, col;
            row = Convert.ToInt32(clicked.Name.Substring(4, 1));
            col = Convert.ToInt32(clicked.Name.Substring(6, 1));
            if (row == turn)
            {
                senderToChange = clicked.Name;
                StackPanel userInputPanel = (StackPanel)FindName("userInputPanel");
                userInputPanel.Visibility = Visibility.Visible;
            }
        }

    }
}

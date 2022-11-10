using LeagueHandler;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPFAllsvenskan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> teamsToGuess = new();
        Serie allsvenskan = new();
        public MainWindow()
        {
            InitializeComponent();

            ButtonSubmit.IsEnabled = false;
            ButtonSelectTeam.IsEnabled = false;
            ButtonRemoveTeam.IsEnabled = false;
            ButtonHomeWin.Visibility = Visibility.Hidden;
            ButtonDraw.Visibility = Visibility.Hidden;
            ButtonAwayWin.Visibility = Visibility.Hidden;
            ButtonSubmit.Visibility = Visibility.Collapsed;
            ComboBoxResult.Visibility = Visibility.Collapsed;
            allsvenskan = new();
            PopulateComboBoxes();
            PopulateListView();

            lblTable.Content = allsvenskan.PrintTable();
        }

        private void PopulateComboBoxes()
        {
            ComboBoxUpcomingDifficulty.Items.Add(1);
            ComboBoxUpcomingDifficulty.Items.Add(3);
            ComboBoxUpcomingDifficulty.Items.Add(5);
            ComboBoxUpcomingDifficulty.Items.Add(7);
            ComboBoxUpcomingDifficulty.Items.Add(10);
            ComboBoxUpcomingDifficulty.Items.Add(15);

            foreach (Team t in allsvenskan.teams)
            {
                ComboBoxFixtures.Items.Add(t.Name);
            }
        }

        private void PopulateListView()
        {
            foreach (Team team in allsvenskan.teams)
            {
                lvListOfTeams.Items.Add(team.Name);
            }
        }

        private void ButtonGuessGames_Click(object sender, RoutedEventArgs e)
        {
            allsvenskan.GuessTheFinish(teamsToGuess);
            //allsvenskan.GuessTheFinish((int)ComboBoxNumberOfTeams.SelectedIndex + 1);
            ButtonSubmit.Content = $"Nästa match ({allsvenskan.gamesToGuess.Count}";
            if (allsvenskan.gamesToGuess.Count > 0)
            {
                lblGame.Content = allsvenskan.gamesToGuess[0].PrintGame();
                ButtonDraw.Visibility = Visibility.Visible;
                ButtonAwayWin.Visibility = Visibility.Visible;
                ButtonHomeWin.Visibility = Visibility.Visible;
                updateResultBox();
                updateGuessButtons();
                ButtonSubmit.IsEnabled = true;
                allsvenskan.SortTable();
                lblTable.Content = allsvenskan.PrintTable();

            }
            else
            {
                MessageBox.Show("Please choose a some teams", "Errormessage");
            }
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (allsvenskan.gamesToGuess.Count > 0)
            {
                allsvenskan.gamesToGuess[0].GuessTheGame(ComboBoxResult.Text);
                if (allsvenskan.gamesToGuess.Count > 1)
                {
                    allsvenskan.gamesToGuess.Remove(allsvenskan.gamesToGuess[0]);
                    lblGame.Content = $"({allsvenskan.gamesToGuess.Count}) {allsvenskan.gamesToGuess[0].PrintGame()}";
                    updateResultBox();
                }
                else if (allsvenskan.gamesToGuess.Count == 1)
                {
                    allsvenskan.gamesToGuess.Remove(allsvenskan.gamesToGuess[0]);
                    allsvenskan.SortByEndPoints();
                    ComboBoxResult.Items.Clear();
                    lblGame.Content = "";
                    ButtonSubmit.IsEnabled = false;
                }
                ButtonSubmit.Content = $"Nästa match ({allsvenskan.gamesToGuess.Count}";
                lblTable.Content = allsvenskan.PrintTable();
            }
        }
        private void updateResultBox()
        {
            ComboBoxResult.Items.Clear();
            ComboBoxResult.Items.Add(allsvenskan.gamesToGuess[0].homeTeam.Name);
            ComboBoxResult.Items.Add(allsvenskan.gamesToGuess[0].awayTeam.Name);
            ComboBoxResult.Items.Add("Oavgjort");
        }
        private void updateGuessButtons()
        {
            if (allsvenskan.gamesToGuess.Count > 0)
            {
                ButtonHomeWin.Content = allsvenskan.gamesToGuess[0].homeTeam.Name;
                ButtonAwayWin.Content = allsvenskan.gamesToGuess[0].awayTeam.Name;
                lblGame.Content = $"({allsvenskan.gamesToGuess.Count}) {allsvenskan.gamesToGuess[0].PrintGame()}";
            }
            else
            {
                ButtonHomeWin.Visibility = Visibility.Hidden;
                ButtonAwayWin.Visibility = Visibility.Hidden;
                ButtonDraw.Visibility = Visibility.Hidden;
            }
        }

        private void ButtonFixtures_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxFixtures.SelectedItem != null)
            {
                lblFixtures.Content = allsvenskan.FindTeam(ComboBoxFixtures.Text).GetGamesLeftAsString();
            }
            else
            {
                MessageBox.Show("Please Choose a team too show fixtures for");
            }
        }

        private void UpdateTable_Click(object sender, RoutedEventArgs e)
        {
            allsvenskan.AverageOpponentInUpcomingGames((int)ComboBoxUpcomingDifficulty.SelectedValue);
            lblTable.Content = allsvenskan.PrintTable();
        }

        private void ButtonSelectTeam_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            if (b == ButtonSelectTeam)
            {
                string s = (string)lvListOfTeams.SelectedItem;
                lvSelectedTeams.Items.Add(s);
                teamsToGuess.Add(s);
                lvListOfTeams.Items.Remove(s);
            }
            else if (b == ButtonRemoveTeam)
            {
                string s = (string)lvSelectedTeams.SelectedItem;
                lvListOfTeams.Items.Add(s);
                teamsToGuess.Remove(s);
                lvSelectedTeams.Items.Remove(s);
            }
        }

        private void lvListOfTeams_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lvListOfTeams.SelectedItem != null)
            {
                ButtonSelectTeam.IsEnabled = true;
            }
            else
            {
                ButtonSelectTeam.IsEnabled = false;
            }
        }

        private void lvSelectedTeams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvSelectedTeams.SelectedItem != null)
            {
                ButtonRemoveTeam.IsEnabled = true;
            }
            else
            {
                ButtonRemoveTeam.IsEnabled = false;
            }
        }

        private void ButtonGuess_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (allsvenskan.gamesToGuess.Count > 1)
            {
                allsvenskan.gamesToGuess[0].GuessTheGame(button.Content.ToString());
                allsvenskan.gamesToGuess.RemoveAt(0);
            }
            else if (allsvenskan.gamesToGuess.Count == 1)
            {
                allsvenskan.gamesToGuess[0].GuessTheGame(button.Content.ToString());
                allsvenskan.gamesToGuess.RemoveAt(0);
                lblGame.Content = "";
                ButtonSubmit.IsEnabled = false;
            }
            updateGuessButtons();
            allsvenskan.SortByEndPoints();
            lblTable.Content = allsvenskan.PrintTable();
        }
    }
}

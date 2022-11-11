using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WPFAllsvenskan.Models;

namespace WPFAllsvenskan
{
    /// <summary>
    /// Interaction logic for SerieWindow.xaml
    /// </summary>
    public partial class SerieWindow : Window
    {
        private List<string> teamsToGuess = new();
        private Serie serie;
        public SerieWindow(string league)
        {
            try
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
                serie = new(league);
                PopulateComboBoxes();
                PopulateListView();

                lblTable.Content = serie.PrintTable();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("File doesn´t exist");
                Close();
            }
        }

        private void PopulateComboBoxes()
        {
            ComboBoxUpcomingDifficulty.Items.Add(1);
            ComboBoxUpcomingDifficulty.Items.Add(3);
            ComboBoxUpcomingDifficulty.Items.Add(5);
            ComboBoxUpcomingDifficulty.Items.Add(7);
            ComboBoxUpcomingDifficulty.Items.Add(10);
            ComboBoxUpcomingDifficulty.Items.Add(15);

            foreach (Team t in serie.teams)
            {
                ComboBoxFixtures.Items.Add(t.Name);
            }
        }

        private void PopulateListView()
        {
            foreach (Team team in serie.teams)
            {
                lvListOfTeams.Items.Add(team.Name);
            }
        }

        private void ButtonGuessGames_Click(object sender, RoutedEventArgs e)
        {
            serie.GuessTheFinish(teamsToGuess);
            //allsvenskan.GuessTheFinish((int)ComboBoxNumberOfTeams.SelectedIndex + 1);
            ButtonSubmit.Content = $"Nästa match ({serie.gamesToGuess.Count}";
            if (serie.gamesToGuess.Count > 0)
            {
                lblGame.Content = serie.gamesToGuess[0].PrintGame();
                ButtonDraw.Visibility = Visibility.Visible;
                ButtonAwayWin.Visibility = Visibility.Visible;
                ButtonHomeWin.Visibility = Visibility.Visible;
                updateResultBox();
                updateGuessButtons();
                ButtonSubmit.IsEnabled = true;
                serie.SortTable();
                lblTable.Content = serie.PrintTable();

            }
            else
            {
                MessageBox.Show("Please choose a some teams", "Errormessage");
            }
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (serie.gamesToGuess.Count > 0)
            {
                serie.gamesToGuess[0].GuessTheGame(ComboBoxResult.Text);
                if (serie.gamesToGuess.Count > 1)
                {
                    serie.gamesToGuess.Remove(serie.gamesToGuess[0]);
                    lblGame.Content = $"({serie.gamesToGuess.Count}) {serie.gamesToGuess[0].PrintGame()}";
                    updateResultBox();
                }
                else if (serie.gamesToGuess.Count == 1)
                {
                    serie.gamesToGuess.Remove(serie.gamesToGuess[0]);
                    serie.SortByEndPoints();
                    ComboBoxResult.Items.Clear();
                    lblGame.Content = "";
                    ButtonSubmit.IsEnabled = false;
                }
                ButtonSubmit.Content = $"Nästa match ({serie.gamesToGuess.Count}";
                lblTable.Content = serie.PrintTable();
            }
        }
        private void updateResultBox()
        {
            ComboBoxResult.Items.Clear();
            ComboBoxResult.Items.Add(serie.gamesToGuess[0].homeTeam.Name);
            ComboBoxResult.Items.Add(serie.gamesToGuess[0].awayTeam.Name);
            ComboBoxResult.Items.Add("Oavgjort");
        }
        private void updateGuessButtons()
        {
            if (serie.gamesToGuess.Count > 0)
            {
                ButtonHomeWin.Content = serie.gamesToGuess[0].homeTeam.Name;
                ButtonAwayWin.Content = serie.gamesToGuess[0].awayTeam.Name;
                lblGame.Content = $"({serie.gamesToGuess.Count}) {serie.gamesToGuess[0].PrintGame()}";
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
                lblFixtures.Content = serie.FindTeam(ComboBoxFixtures.Text).GetGamesLeftAsString();
            }
            else
            {
                MessageBox.Show("Please Choose a team too show fixtures for");
            }
        }

        private void UpdateTable_Click(object sender, RoutedEventArgs e)
        {
            serie.AverageOpponentInUpcomingGames((int)ComboBoxUpcomingDifficulty.SelectedValue);
            lblTable.Content = serie.PrintTable();
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
            if (serie.gamesToGuess.Count > 1)
            {
                serie.gamesToGuess[0].GuessTheGame(button.Content.ToString());
                serie.gamesToGuess.RemoveAt(0);
            }
            else if (serie.gamesToGuess.Count == 1)
            {
                serie.gamesToGuess[0].GuessTheGame(button.Content.ToString());
                serie.gamesToGuess.RemoveAt(0);
                lblGame.Content = "";
                ButtonSubmit.IsEnabled = false;
            }
            updateGuessButtons();
            serie.SortByEndPoints();
            lblTable.Content = serie.PrintTable();
        }
    }
}

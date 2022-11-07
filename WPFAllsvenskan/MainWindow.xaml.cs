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
            ButtonHomeWin.IsEnabled = false;
            ButtonDraw.IsEnabled = false;
            ButtonAwayWin.IsEnabled = false;

            allsvenskan = new();
            PopulateComboBoxes();
            PopulateListView();

            allsvenskan.AverageOpponent(1);
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

            for (int i = 1; i <= allsvenskan.teams.Length; i++)
            {
                ComboBoxFixtures.Items.Add(allsvenskan.teams[i - 1].name);
            }
        }

        private void PopulateListView()
        {
            foreach (Team team in allsvenskan.teams)
            {
                lvListOfTeams.Items.Add(team.name);
            }
        }

        private void ButtonTopTeams_Click(object sender, RoutedEventArgs e)
        {
            allsvenskan.GuessTheFinish(teamsToGuess);
            //allsvenskan.GuessTheFinish((int)ComboBoxNumberOfTeams.SelectedIndex + 1);
            ButtonSubmit.Content = $"Nästa match ({allsvenskan.gamesToGuess.Count}";
            if (allsvenskan.gamesToGuess.Count > 0)
            {
                lblGame.Content = allsvenskan.gamesToGuess[0].printGame();
                ButtonDraw.IsEnabled = true;
                ButtonAwayWin.IsEnabled = true;
                ButtonHomeWin.IsEnabled = true;
                ButtonDraw.Content = "Oavgjort";
                updateResultBox();
                updateGuessButtons();
                ButtonSubmit.IsEnabled = true;
                allsvenskan.sortTable();
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
                    lblGame.Content = allsvenskan.gamesToGuess[0].printGame();
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
            ComboBoxResult.Items.Add(allsvenskan.gamesToGuess[0].homeTeam.name);
            ComboBoxResult.Items.Add(allsvenskan.gamesToGuess[0].awayTeam.name);
            ComboBoxResult.Items.Add("Oavgjort");
        }
        private void updateGuessButtons()
        {
            ButtonHomeWin.Content = allsvenskan.gamesToGuess[0].homeTeam.name;
            ButtonAwayWin.Content = allsvenskan.gamesToGuess[0].awayTeam.name;
        }

        private void ButtonFixtures_Click(object sender, RoutedEventArgs e)
        {
            lblFixtures.Content = allsvenskan.findTeam(ComboBoxFixtures.Text).GetGamesLeft();
        }

        private void UpdateTable_Click(object sender, RoutedEventArgs e)
        {
            allsvenskan.AverageOpponent((int)ComboBoxUpcomingDifficulty.SelectedValue);
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
            ButtonSelectTeam.IsEnabled = true;
        }

        private void lvSelectedTeams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonRemoveTeam.IsEnabled = true;
        }
    }
}

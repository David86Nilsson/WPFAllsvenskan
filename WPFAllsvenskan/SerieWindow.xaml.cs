using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        private AppManager appManager;
        public SerieWindow(AppManager appManager, Serie serie)
        {
            try
            {
                InitializeComponent();
                HideButtons();

                this.serie = serie;
                this.appManager = appManager;
                //manager.SetStatsBasedOnSerie(serie);
                PopulateComboBoxes();
                PopulateListView();
                ShowTable();
                //lblTable.Content = serie.PrintTable();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("File doesn´t exist");
                Close();
            }
        }

        private void HideButtons()
        {
            ButtonSubmit.IsEnabled = false;
            ButtonSelectTeam.IsEnabled = false;
            ButtonRemoveTeam.IsEnabled = false;
            ButtonHomeWin.Visibility = Visibility.Hidden;
            ButtonDraw.Visibility = Visibility.Hidden;
            ButtonAwayWin.Visibility = Visibility.Hidden;
            ButtonSubmit.Visibility = Visibility.Collapsed;
            ComboBoxResult.Visibility = Visibility.Collapsed;
        }

        private void ShowTable()
        {
            lvTable.Items.Clear();
            lvTable.Items.Add(appManager.GetTableHeadLine());
            foreach (SerieMember serieMember in serie.SerieMembers)
            {
                ListViewItem item = new();
                item.Content = appManager.GetTeamInSerieAsString(serie, serieMember.Team);
                item.Tag = serieMember.Team;
                item.VerticalContentAlignment = VerticalAlignment.Top;
                item.VerticalAlignment = VerticalAlignment.Top;
                lvTable.Items.Add(item);
            }
        }

        private void PopulateComboBoxes()
        {
            ComboBoxUpcomingGames.Items.Add(1);
            ComboBoxUpcomingGames.Items.Add(3);
            ComboBoxUpcomingGames.Items.Add(5);
            ComboBoxUpcomingGames.Items.Add(7);
            ComboBoxUpcomingGames.Items.Add(10);
            ComboBoxUpcomingGames.Items.Add(15);

            ComboboxPointsPerGame.Items.Add("All");
            ComboboxPointsPerGame.Items.Add("Plastic");
            ComboboxPointsPerGame.Items.Add("Grass");

            foreach (SerieMember serieMember in serie.SerieMembers)
            {
                ComboBoxItem comboBoxItem = new();
                comboBoxItem.Content = serieMember.Team.Name;
                comboBoxItem.Tag = serieMember;
                ComboboxPlace.Items.Add(serieMember.Rank);
            }

            int nbr = serie.SerieMembers[0].GamesPlayed;
            for (int i = 1; i <= nbr; i++)
            {
                ComboboxGames.Items.Add(i);
            }

        }

        private void PopulateListView()
        {
            foreach (SerieMember serieMember in serie.SerieMembers)
            {
                lvListOfTeams.Items.Add(serieMember.Team.Name);
            }
        }

        private void ButtonGuessGames_Click(object sender, RoutedEventArgs e)
        {
            //serie.GuessTheFinish(teamsToGuess);
            ////allsvenskan.GuessTheFinish((int)ComboBoxNumberOfTeams.SelectedIndex + 1);
            //ButtonSubmit.Content = $"Nästa match ({serie.GamesToGuess.Count}";
            //if (serie.GamesToGuess.Count > 0)
            //{
            //    lblGame.Content = serie.GamesToGuess[0].PrintGame();
            //    ButtonDraw.Visibility = Visibility.Visible;
            //    ButtonAwayWin.Visibility = Visibility.Visible;
            //    ButtonHomeWin.Visibility = Visibility.Visible;
            //    updateResultBox();
            //    updateGuessButtons();
            //    ButtonSubmit.IsEnabled = true;

            //}
            //else
            //{
            //    MessageBox.Show("Please choose a some teams", "Errormessage");
            //}
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            //if (serie.GamesToGuess.Count > 0)
            //{
            //    serie.GamesToGuess[0].GuessTheGame(ComboBoxResult.Text);
            //    if (serie.GamesToGuess.Count > 1)
            //    {
            //        serie.GamesToGuess.Remove(serie.GamesToGuess[0]);
            //        lblGame.Content = $"({serie.GamesToGuess.Count}) {serie.GamesToGuess[0].PrintGame()}";
            //        updateResultBox();
            //    }
            //    else if (serie.GamesToGuess.Count == 1)
            //    {
            //        serie.GamesToGuess.Remove(serie.GamesToGuess[0]);
            //        serie.SortByEndPoints();
            //        ComboBoxResult.Items.Clear();
            //        lblGame.Content = "";
            //        ButtonSubmit.IsEnabled = false;
            //    }
            //    ButtonSubmit.Content = $"Nästa match ({serie.GamesToGuess.Count}";
            //}
        }
        private void updateResultBox()
        {
            ComboBoxResult.Items.Clear();
            ComboBoxResult.Items.Add(serie.GamesToGuess[0].HomeTeam.Name);
            ComboBoxResult.Items.Add(serie.GamesToGuess[0].AwayTeam.Name);
            ComboBoxResult.Items.Add("Oavgjort");
        }
        private void updateGuessButtons()
        {
            if (serie.GamesToGuess.Count > 0)
            {
                ButtonHomeWin.Content = serie.GamesToGuess[0].HomeTeam.Name;
                ButtonAwayWin.Content = serie.GamesToGuess[0].AwayTeam.Name;
                lblGame.Content = $"({serie.GamesToGuess.Count}) {serie.GamesToGuess[0].PrintGame()}";
            }
            else
            {
                ButtonHomeWin.Visibility = Visibility.Hidden;
                ButtonAwayWin.Visibility = Visibility.Hidden;
                ButtonDraw.Visibility = Visibility.Hidden;
            }
        }

        //private void ButtonFixtures_Click(object sender, RoutedEventArgs e)
        //{
        //    lvListOfGames.Items.Clear();
        //    ComboBoxItem item = (ComboBoxItem)ComboBoxFixtures.SelectedItem;
        //    if (item != null)
        //    {
        //        Team t = (Team)item.Tag;
        //        if (t.Schedule.Count == t.PlayedGames.Count)
        //        {
        //            foreach (Game game in t.Schedule)
        //            {
        //                ListViewItem listViewItem = new();
        //                listViewItem.Content = game.PrintGame();
        //                listViewItem.Tag = game;
        //                lvListOfGames.Items.Add(listViewItem);
        //                if (manager.GetOpponentsRank(t, game) >= 11)
        //                {
        //                    listViewItem.Background = Brushes.LightGreen;
        //                }
        //                else if (manager.GetOpponentsRank(t, game) >= 6)
        //                {
        //                    listViewItem.Background = Brushes.LightYellow;
        //                }
        //                else
        //                {
        //                    listViewItem.Background = Brushes.LightPink;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (Game game in t.Schedule)
        //            {
        //                if (game.IsPlayed == false)
        //                {
        //                    ListViewItem listViewItem = new();
        //                    listViewItem.Content = game.PrintGame();
        //                    listViewItem.Tag = game;
        //                    lvListOfGames.Items.Add(listViewItem);
        //                    if (manager.GetOpponentsRank(t, game) >= 11)
        //                    {
        //                        listViewItem.Background = Brushes.LightGreen;
        //                    }
        //                    else if (manager.GetOpponentsRank(t, game) >= 6)
        //                    {
        //                        listViewItem.Background = Brushes.LightYellow;
        //                    }
        //                    else
        //                    {
        //                        listViewItem.Background = Brushes.LightPink;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please select a team");
        //    }
        //}

        private void UpdateTable_Click(object sender, RoutedEventArgs e)
        {
            //string pPerGame = "All";
            //if (ComboboxGames.SelectedValue != null)
            //{
            //    int nbrOfGamesToPrint = int.Parse(ComboboxGames.SelectedValue.ToString());
            //    serie.CalculateTableBetweenRounds(1, nbrOfGamesToPrint);
            //}
            //if (ComboBoxUpcomingGames.SelectedValue != null)
            //{
            //    serie.AverageOpponentInUpcomingGames((int)ComboBoxUpcomingGames.SelectedValue);
            //}
            //if (ComboboxPointsPerGame.SelectedValue != null)
            //{
            //    pPerGame = ComboboxPointsPerGame.SelectedValue.ToString();
            //}
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
            //Button button = (Button)sender;
            //if (serie.GamesToGuess.Count > 1)
            //{
            //    serie.GamesToGuess[0].GuessTheGame(button.Content.ToString());
            //    serie.GamesToGuess.RemoveAt(0);
            //}
            //else if (serie.GamesToGuess.Count == 1)
            //{
            //    serie.GamesToGuess[0].GuessTheGame(button.Content.ToString());
            //    serie.GamesToGuess.RemoveAt(0);
            //    lblGame.Content = "";
            //    ButtonSubmit.IsEnabled = false;
            //}
            //updateGuessButtons();
            //serie.SortByEndPoints();
        }
        private void lvTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvTable.SelectedItem is ListViewItem item)
            {
                if (item != null)
                {
                    Team team = (Team)item.Tag;
                    UpdateLvGames(team);
                }
            }

        }
        private void UpdateLvGames(Team team)
        {
            lvListOfGames.Items.Clear();
            foreach (Game game in team.Games)
            {
                if (game.IsPlayed == false)
                {
                    ListViewItem listViewItem = new();
                    listViewItem.Content = $"{appManager.GetGameInfo(game)}";
                    listViewItem.Tag = game;
                    lvListOfGames.Items.Add(listViewItem);
                    if (serie.Games[24].IsPlayed)
                    {
                        int rank = appManager.GetOpponentsRank(serie, team, game, false);
                        if (rank == 0 || rank >= 10)
                        {
                            listViewItem.Background = Brushes.LightGreen;
                        }
                        else if (rank >= 6)
                        {
                            listViewItem.Background = Brushes.LightYellow;
                        }
                        else
                        {
                            listViewItem.Background = Brushes.LightPink;
                        }
                    }
                    else
                    {
                        //Get Opponents rank from last year
                        int rank = appManager.GetOpponentsRank(serie, team, game, true);

                        if (rank == 0 || rank >= 10) listViewItem.Background = Brushes.LightGreen;
                        else if (rank >= 6) listViewItem.Background = Brushes.LightYellow;
                        else listViewItem.Background = Brushes.LightPink;
                    }
                }
            }
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new(appManager);
            mainWindow.Show();
            Close();
        }
    }
}

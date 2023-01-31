using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WPFAllsvenskan.Models;

namespace WPFAllsvenskan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppManager _appManager;
        public MainWindow()
        {
            InitializeComponent();
            CreateAppManager();
            PopulateLeagueComboBox();
            PopulateListView();
            ButtonGo.Visibility = Visibility.Hidden;
        }
        public MainWindow(AppManager appManager)
        {
            InitializeComponent();
            this._appManager = appManager;
            PopulateLeagueComboBox();
            ButtonGo.Visibility = Visibility.Hidden;
        }
        private async void CreateAppManager()
        {
            _appManager = await AppManager.CreateAppManagerAsync();
        }

        private void PopulateLeagueComboBox()
        {
            ComboBoxLeagues.Items.Clear();
            List<Competion> competions = _appManager.Competions;
            foreach (Competion competion in competions)
            {
                ComboBoxItem item = new();
                item.Content = competion.Name;
                item.Tag = competion;
                ComboBoxLeagues.Items.Add(item);
            }
        }

        private void ButtonGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboBoxItem item = (ComboBoxItem)ComboBoxSeasons.SelectedItem;
                Serie? serie = (Serie)item.Tag;
                if (serie != null)
                {
                    SerieWindow serieWindow = new(_appManager, serie);
                    serieWindow.Show();
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void ComboBoxLeagues_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            PopulateComboBoxSeasons();
        }
        private void PopulateComboBoxSeasons()
        {
            ComboBoxSeasons.Items.Clear();
            ComboBoxItem item = (ComboBoxItem)ComboBoxLeagues.SelectedItem;
            if (item != null)
            {
                Competion competion = (Competion)item.Tag;
                if (competion is League)
                {
                    League league = (League)competion;
                    foreach (Serie serie in league.Series)
                    {
                        ComboBoxItem newItem = new();
                        newItem.Content = serie.Year;
                        newItem.Tag = serie;
                        ComboBoxSeasons.Items.Add(newItem);
                    }
                }
            }
        }

        private void ComboBoxSeasons_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ButtonGo.Visibility = Visibility.Visible;
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            _appManager.ReadAllSchedules();
            _appManager.ReadAllOdds();
            _appManager.ReadAllChances();
            PopulateLeagueComboBox();
            PopulateListView();
        }

        private void PopulateListView()
        {
            lvGamesWithValue.Items.Clear();
            List<Game> games = _appManager.GetValueGames(5);
            MessageBox.Show(games.Count.ToString());
            foreach (Game game in games)
            {
                ListViewItem item = new();
                item.Content = _appManager.GetGameInfo(game);
                item.Tag = game;
                lvGamesWithValue.Items.Add(item);
            }

        }

        private async void btDownload_ClickAsync(object sender, RoutedEventArgs e)
        {
            LockButtons();
            btDownload.Content = "Downloading...";
            await _appManager.DownloadFromWebAsync();
            btDownload.Content = "Download info";
            UnlockButtons();
        }

        private void UnlockButtons()
        {
            btReloadCombobox.IsEnabled = true;
            ComboBoxLeagues.IsEnabled = true;
            ComboBoxSeasons.IsEnabled = true;
            lvGamesWithValue.IsEnabled = true;
        }

        private void LockButtons()
        {
            btReloadCombobox.IsEnabled = false;
            ComboBoxLeagues.IsEnabled = false;
            ComboBoxSeasons.IsEnabled = false;
            lvGamesWithValue.IsEnabled = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace WPFAllsvenskan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PopulateComboBox();
            ButtonGo.Visibility = Visibility.Hidden;
        }

        private void PopulateComboBox()
        {
            string[] files = Directory.GetFiles(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\");
            List<string> leagues = new();
            foreach (string file in files)
            {
                if (file.EndsWith(".txt"))
                {
                    string s = file.Remove(file.Length - 4);
                    string[] league = s.Split(@"\");
                    leagues.Add(league[league.Length - 1]);
                }
            }
            ComboBoxLeagues.ItemsSource = leagues;
        }

        private void ButtonGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SerieWindow serieWindow = new(ComboBoxLeagues.Text);
                serieWindow.Show();
                Close();
            }
            catch (Exception ex)
            {

            }
        }

        private void ComboBoxLeagues_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ButtonGo.Visibility = Visibility.Visible;
        }
    }
}

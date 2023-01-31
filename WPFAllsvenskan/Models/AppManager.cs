using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UglyToad.PdfPig;

namespace WPFAllsvenskan.Models
{
    public class AppManager
    {
        private string _textFilesAdress = @"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\";
        public List<Team> Teams { get; set; } = new();
        public List<Competion> Competions { get; set; } = new();
        public Dictionary<string, string> TeamNameDictionary { get; set; } = new();
        public AppManager()
        {
            //LoadAllSchedules();

            PopulateDictionary();

            ReadAllSchedules();
            ReadAllOdds();
            ReadAllChances();

            //UrlToTxtFile(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\PLchance.txt", "https://projects.fivethirtyeight.com/soccer-predictions/premier-league/", "Matches");
        }

        private void PopulateDictionary()
        {
            TeamNameDictionary.Add("Man. City", "Manchester City");
            TeamNameDictionary.Add("Man. United", "Manchester United");
            TeamNameDictionary.Add("Nottm Forest", "Nottingham");
            TeamNameDictionary.Add("Öster", "Östers IF");
            TeamNameDictionary.Add("Almería", "Almeria");
            TeamNameDictionary.Add("AIK", "AIK Solna");
        }

        public static async Task<AppManager> CreateAppManagerAsync()
        {
            AppManager appmanager = new();
            return appmanager;
        }
        // Return all games with value above value in serie if serie exist othervise all series
        public List<Game> GetValueGames(double value, Serie? serie = null)
        {
            List<Game> games = new();
            if (serie == null)
            {
                foreach (Competion competion in Competions)
                {
                    if (competion is League league)
                    {
                        foreach (Serie s in league.Series)
                        {
                            foreach (Game game in s.Games)
                            {
                                if (game.IsPlayed == false && GetGameValue(game) > value)
                                {
                                    games.Add(game);
                                }

                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Game game in serie.Games)
                {
                    if (GetGameValue(game) > value)
                    {
                        games.Add(game);
                    }
                }
            }
            return games.OrderBy(g => g.Date).ToList();
        }
        private double GetGameValue(Game game)
        {
            if (game.OddsChances1 > 0 && game.Chances1 > 0)
            {
                double value1 = game.Chances1 - game.OddsChances1;
                double value2 = game.Chances2 - game.OddsChances2;
                return Math.Round(Math.Max(value1, value2), 0);
            }
            return 0;
        }

        public async Task LoadAllSchedulesAsync()
        {
            await DownloadFromWebAsync();
            string[] files = Directory.GetFiles(_textFilesAdress);
            foreach (string file in files)
            {
                if (!(file.Contains("chance") || file.Contains("Odds")))
                {
                    string[] leagues = file.Remove(file.Length - 4).Split(@"\");
                    string name = leagues[leagues.Length - 1];
                    if (file.EndsWith(".txt")) //Adds a Serie
                    {
                        Competion? competion = GetCompetion(name);
                        Serie serie = new();
                        if (competion != null)
                        {
                            if (competion is League league)
                            {
                                LoadSchedule(serie, file);
                                serie.League = league;
                                league.Series.Add(serie);
                            }
                        }
                        else
                        {
                            League newLeague = new(name);
                            serie.League = newLeague;
                            LoadSchedule(serie, file);
                            newLeague.Series.Add(serie);
                            Competions.Add(newLeague);
                        }
                    }

                    if (!file.Contains("2") && file.Contains("Cup") && file.EndsWith(".txt")) // Adds a tournament
                    {
                        // Todo: Add Code for Tournaments
                    }
                }
            }
        }
        public void ReadAllSchedules()
        {
            string[] files = Directory.GetFiles(_textFilesAdress);
            foreach (string file in files)
            {
                if (!(file.Contains("Chance") || file.Contains("Odds")))
                {
                    string[] leagues = file.Remove(file.Length - 4).Split(@"\");
                    string name = leagues[leagues.Length - 1];
                    if (file.EndsWith(".txt")) //Adds a Serie
                    {
                        Competion? competion = GetCompetion(name);
                        Serie serie = new();
                        if (competion != null)
                        {
                            if (competion is League league)
                            {
                                LoadSchedule(serie, file);
                                serie.League = league;
                                league.Series.Add(serie);
                            }
                        }
                        else
                        {
                            League newLeague = new(name)
                            {
                                UrlSchedule = file,
                                UrlOdds = file.Remove(file.Length - 4) + "Odds.txt",
                                UrlChances = file.Remove(file.Length - 4) + "Chances.txt"
                            };
                            serie.League = newLeague;
                            LoadSchedule(serie, file);
                            newLeague.Series.Add(serie);
                            Competions.Add(newLeague);
                        }
                    }

                    if (!file.Contains("2") && file.Contains("Cup") && file.EndsWith(".txt")) // Adds a tournament
                    {
                        // Todo: Add Code for Tournaments
                    }
                }
            }
        }

        public async Task DownloadFromWebAsync()
        {
            await UrlToTxtFileAsync(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\Premier League.txt", "https://www.svenskafans.com/england/premier-league/spelschema.sf");
            await UrlToTxtFileAsync(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\Allsvenskan2023.txt", "https://www.svenskafans.com/fotboll/allsvenskan/spelschema.sf");
            await UrlToTxtFileAsync(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\Championship.txt", "https://www.svenskafans.com/england/the-championship/spelschema.sf");
            await UrlToTxtFileAsync(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\Superettan2023.txt", "https://www.svenskafans.com/fotboll/superettan/spelschema.sf");
            await UrlToTxtFileAsync(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\La liga.txt", "https://www.svenskafans.com/spanien/la-liga/spelschema.sf");

            await UrlToTxtFileAsync(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\Premier LeagueOdds.txt", "https://www.unibet.se/betting/sports/filter/football/england/premier_league/all/matches");
            await UrlToTxtFileAsync(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\AllsvenskanOdds.txt", "https://www.unibet.se/betting/sports/filter/football/sweden/allsvenskan/all/matches");
            await UrlToTxtFileAsync(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\ChampionshipOdds.txt", "https://www.unibet.com/betting/sports/filter/football/england/the_championship/all/matches");
            await UrlToTxtFileAsync(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\SuperettanOdds.txt", "https://www.unibet.se/betting/sports/filter/football/sweden/superettan/all/matches");
            await UrlToTxtFileAsync(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\La ligaOdds.txt", "https://www.unibet.com/betting/sports/filter/football/spain/la_liga/all/matches");

            await UrlToTxtFileAsync(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\Premier LeagueChances.txt", "https://projects.fivethirtyeight.com/soccer-predictions/premier-league/", "Matches");
            await UrlToTxtFileAsync(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\AllsvenskanChances.txt", "https://projects.fivethirtyeight.com/soccer-predictions/allsvenskan/", "Matches");
            await UrlToTxtFileAsync(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\ChampionshipChances.txt", "https://projects.fivethirtyeight.com/soccer-predictions/championship/", "Matches");
            await UrlToTxtFileAsync(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\La ligaChances.txt", "https://projects.fivethirtyeight.com/soccer-predictions/la-liga/", "Matches");
        }

        private League? GetLeagueWithName(string name)
        {
            foreach (Competion competion in Competions)
            {
                if (competion is League)
                {
                    League league = (League)competion;
                    if (league.Name == name)
                    {
                        return league;
                    }
                }
            }
            return null;
        }

        public void ReadAllOdds()
        {
            LoadOdds("Premier League", "https://www.unibet.se/betting/sports/filter/football/england/premier_league/all/matches");
            LoadOdds("Allsvenskan", "https://www.unibet.se/betting/sports/filter/football/sweden/allsvenskan/all/matches");
            LoadOdds("Championship", "https://www.unibet.com/betting/sports/filter/football/england/the_championship/all/matches");
            LoadOdds("La Liga", "https://www.unibet.com/betting/sports/filter/football/spain/la_liga/all/matches");
            LoadOdds("Superettan", "https://www.unibet.se/betting/sports/filter/football/sweden/superettan/all/matches");
        }
        public void ReadAllChances()
        {
            foreach (Competion competion in Competions)
            {
                if (competion is League league)
                {
                    LoadChances(competion.Name);
                }
            }

        }
        public string GetGameInfo(Game game)
        {
            if (game.IsPlayed)
            {
                return $"\n{game.Date.ToShortDateString()} Omg:{game.Round} : {game.HomeGoals} - {game.AwayGoals} \t{game.HomeTeam.Name} - {game.AwayTeam.Name}";
            }
            else if (game.OddsChances1 > 0)
            {
                return $"{game.Date.ToShortDateString()} Omg:{game.Round} | {game.HomeTeam.Name.Trim()} - {game.AwayTeam.Name.Trim()} | {game.Odds1}({game.OddsChances1}%) - {game.OddsX}({game.OddsChancesX}%) - {game.Odds2}({game.OddsChances2}%) | {game.Chances1}% - {game.ChancesX}% - {game.Chances2}%";
            }
            else
            {
                return $"{game.Date.ToShortDateString()} Omg:{game.Round} | {game.HomeTeam.Name.Trim()} - {game.AwayTeam.Name.Trim()}";
            }

        }
        private void LoadChances(string leagueName)
        {

            string adress = _textFilesAdress + GetLeagueWithName(leagueName).TxtChances;
            if (File.Exists(adress))
            {
                string[] lines = File.ReadAllLines(adress);
                {
                    Team? homeTeam = null;
                    Team? awayTeam = null;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i] == "Completed matches") break;

                        if (lines[i].Length >= 2 && lines[i].Length <= 5 && lines[i].Contains("/"))
                        {
                            homeTeam = GetTeam(lines[i + 2]);
                            //if (homeTeam != null && lines[i + 2].Contains("Nottm"))
                            //{
                            //    MessageBox.Show($"{lines[i + 2]} - {homeTeam.Name}");
                            //}
                            awayTeam = GetTeam(lines[i + 7]);
                            if (homeTeam != null && awayTeam != null)
                            {
                                Game? game = GetGame(homeTeam, awayTeam, DateTime.Now.Year.ToString());
                                if (game != null)
                                {
                                    game.Chances1 = double.Parse(lines[i + 3].Replace("%", "").Trim());
                                    game.ChancesX = double.Parse(lines[i + 4].Replace("%", "").Trim());
                                    game.Chances2 = double.Parse(lines[i + 8].Replace("%", "").Trim());
                                }
                                else
                                {
                                    //MessageBox.Show($"did not find {homeTeam.Name} - {awayTeam.Name}");
                                }
                            }
                            homeTeam = null;
                            awayTeam = null;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show($"{leagueName} is missing chances file. {adress} doesnt exist");
            }
        }
        private void LoadOdds(string leagueName, string fullUrl)
        {
            string adress = @"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\" + leagueName + "Odds.txt";
            if (System.IO.File.Exists(adress))
            {
                string[] lines = System.IO.File.ReadAllLines(adress);

                Team? homeTeam = null;
                Team? awayTeam = null;

                for (int j = 0; j < lines.Length; j++) // For every line of text
                {
                    if (lines[j].Contains("+"))
                    {
                        homeTeam = GetTeam(lines[j - 5]);
                        awayTeam = GetTeam(lines[j - 4]);
                        if (homeTeam != null && awayTeam != null)
                        {
                            Game? game = GetGame(homeTeam, awayTeam, DateTime.Now.Year.ToString());
                            if (game != null)
                            {
                                game.Odds1 = lines[j - 3];
                                game.OddsX = lines[j - 2];
                                game.Odds2 = lines[j - 1];
                                SetGameOutcomeChancesFromOdds(game);
                            }

                            else
                            {
                                MessageBox.Show($"Couldnt find game between {homeTeam} - {awayTeam}");
                            }
                        }
                        homeTeam = null;
                        awayTeam = null;
                    }
                }
            }
            else
            {
                MessageBox.Show($"{leagueName} is missing odds file. {adress} doesnt exist");
            }
        }

        private void LoadAllsvenskanOdds()
        {
            CallAsync("https://www.unibet.se/betting/sports/filter/football/sweden/allsvenskan/all/matches");
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\html.txt");

            Team? homeTeam = null;
            Team? awayTeam = null;
            string odds1 = "";
            string oddsX = "";
            string odds2 = "";
            char[] dividers = { '<', '>' };

            for (int j = 0; j < lines.Length; j++) // For every line of text
            {
                string[] splitString = lines[j].Split(dividers);
                for (int i = 0; i < splitString.Length - 1; i++) // for every part of line
                {
                    string s = splitString[i];
                    if (s.Contains("teamName"))
                    {
                        string teamName = splitString[i + 1].Trim();
                        if (homeTeam == null) homeTeam = GetTeam(teamName);
                        else if (awayTeam == null) awayTeam = GetTeam(teamName);
                    }
                    if (s.Contains("data-test-name=\"odds\""))
                    {
                        if (string.IsNullOrEmpty(odds1)) odds1 = splitString[i + 1].Trim();
                        else if (string.IsNullOrEmpty(oddsX)) oddsX = splitString[i + 1].Trim();
                        else odds2 = splitString[i + 1].Trim();
                    }

                    if (homeTeam != null && awayTeam != null && !string.IsNullOrEmpty(odds1) && !string.IsNullOrEmpty(oddsX) && !string.IsNullOrEmpty(odds2))
                    {
                        Game? game = GetGame(homeTeam, awayTeam, "2023");
                        if (game != null)
                        {
                            game.Odds1 = odds1;
                            game.OddsX = oddsX;
                            game.Odds2 = odds2;
                        }
                        homeTeam = null;
                        awayTeam = null;
                        odds1 = "";
                        oddsX = "";
                        odds2 = "";
                    }
                }
            }
        }
        private async Task UrlToPdf(string adress, string fullUrl)
        {
            var options = new LaunchOptions()
            {
                Headless = true,
                ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            };
            var browser = await Puppeteer.LaunchAsync(options, null);
            var page = await browser.NewPageAsync();
            await page.GoToAsync(fullUrl);
            await page.PdfAsync(adress);
        }
        private List<string> PdfToStrings(string adress)
        {
            using (var document = PdfDocument.Open(adress))
            {
                List<string> pages = new();
                foreach (var page in document.GetPages())
                {
                    pages.Add(page.Text);
                }
                return pages;
            }
        }
        private async Task UrlToTxtFileAsync(string adress, string fullUrl, string? pushButton = null)
        {
            string text = await UrlToStringAsync(fullUrl, pushButton);
            await WriteToTxtAsync(adress, text);
        }
        private async void CallAsync(string fullUrl)
        {
            //List<string> links = await CallUrl(fullUrl);
            //WriteToCsv(links);
        }
        private void LoadAllsvenskan2023(Serie serie, string adress)
        {
            string[] lines = System.IO.File.ReadAllLines(adress);
            int omg = 0;
            int maxTeamNameLength = 0;
            DateTime dateTime = new(2023, 1, 1);
            serie.Year = "2023";
            foreach (string line in lines)
            {
                if (line.Contains("Omg"))
                {
                    omg++;
                }
                else if (line.EndsWith(':'))
                {
                    string[] dateStrings = line.Split(' ');
                    int year = 2023;
                    int day = int.Parse(dateStrings[1]);
                    int month = 1;
                    if (dateStrings[2].Trim() == "april:") month = 4;
                    else if (dateStrings[2].Trim() == "maj:") month = 5;
                    else if (dateStrings[2].Trim() == "juni:") month = 6;
                    else if (dateStrings[2].Trim() == "juli:") month = 7;
                    dateTime = new(year, month, day);
                }
                else if (line.StartsWith('1'))
                {

                    string[] gameStrings = line.Remove(0, 6).Trim().Split('–');
                    Team homeTeam = GetTeam(gameStrings[0].Trim());
                    Team awayTeam = GetTeam(gameStrings[1].Trim());

                    if (homeTeam == null)
                    {
                        homeTeam = new(gameStrings[0].Trim());
                        Teams.Add(homeTeam);
                        if (homeTeam.Name.Length > maxTeamNameLength)
                        {
                            maxTeamNameLength = homeTeam.Name.Length;
                        }
                    }
                    if (awayTeam == null)
                    {
                        awayTeam = new(gameStrings[1].Trim());
                        Teams.Add(awayTeam);
                        if (awayTeam.Name.Length > maxTeamNameLength)
                        {
                            maxTeamNameLength = awayTeam.Name.Length;
                        }
                    }
                    if (GetTeamFromSerie(serie, homeTeam) == null)
                    {
                        serie.SerieMembers.Add(new() { Team = homeTeam });
                    }
                    if (GetTeamFromSerie(serie, awayTeam) == null)
                    {
                        serie.SerieMembers.Add(new() { Team = awayTeam });
                    }
                    serie.Games.Add(new(homeTeam, awayTeam, omg, dateTime, serie));
                }
            }
            SortTable(serie);
            serie.League = (League)GetCompetion("Allsvenskan");
        }
        private async Task<string> UrlToStringAsync(string fullUrl, string? pushButton = null)
        {
            string textString = "";

            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            }, null))
            {
                using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(fullUrl); // Go to web page

                    var buttons = await page.QuerySelectorAllAsync("button"); // All buttons
                    foreach (var button in buttons) // Checks if theres a accept-cookies button and clicks it
                    {
                        var text = await button.EvaluateFunctionAsync<string>("(element) => element.innerText");
                        if (text == "GODKÄNN")
                        {
                            // Click the button
                            await button.ClickAsync();
                            //await page.WaitForNavigationAsync();
                            await page.WaitForTimeoutAsync(1000);
                            break;
                        }
                    }
                    if (pushButton != null)
                    {
                        var labels = await page.QuerySelectorAllAsync("label"); // All Labels
                        foreach (var label in labels) // Clicks label with "pushButton" text
                        {
                            var text = await label.EvaluateFunctionAsync<string>("(element) => element.innerText");
                            if (text == pushButton)
                            {
                                // Click the button
                                await label.ClickAsync();
                                await page.WaitForTimeoutAsync(1000);
                                var element = await page.QuerySelectorAsync(".more-upcoming.btn-cta.btn-cta-oneline");
                                if (element != null)
                                {
                                    await element.ClickAsync();
                                    await page.WaitForTimeoutAsync(1000);
                                    break;
                                }
                                break;
                            }
                        }
                    }
                    await page.WaitForTimeoutAsync(1000);
                    //await page.WaitForNavigationAsync();
                    textString = await page.EvaluateExpressionAsync<string>("document.body.innerText"); // Save all innertext in web page
                }
            }
            return textString;
        }

        private void WriteToCsv(List<string> links)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var link in links)
            {
                sb.AppendLine(link);
            }

            System.IO.File.WriteAllText("links2.csv", sb.ToString());
        }
        private async Task WriteToTxtAsync(string adress, string text)
        {
            await System.IO.File.WriteAllTextAsync(adress, text);
            //MessageBox.Show("Writing " + adress + " done");
            //System.IO.File.WriteAllText(@"C:\Users\david\source\repos\WPFAllsvenskan\WPFAllsvenskan\TextFiles\html.txt", html);
        }
        public Competion? GetCompetion(string name)
        {
            foreach (Competion competion in Competions)
            {
                if (name.Contains(competion.Name)) return competion;
            }
            return null;
        }
        public Serie? GetSerie(League league, string year)
        {
            foreach (Serie serie in league.Series)
            {
                if (serie.Year == year) return serie;
            }
            return null;
        }
        public League? GetLeagueWithSerie(Serie serie)
        {
            foreach (Competion competion in Competions)
            {
                if (competion is League)
                {
                    League league = (League)competion;
                    foreach (Serie serieInLeague in league.Series)
                    {
                        if (serieInLeague == serie) return league;
                    }
                }
            }
            return null;
        }

        //Method that return rank, last years rank if CheckLastYear = true and topLeague, return 0 if wasn´t in the top league
        public int GetOpponentsRank(Serie serie, Team team, Game game, bool CheckLastYearsRank)
        {
            int rank;
            Team? opponent = GetOpponent(game, team);

            if (CheckLastYearsRank)
            {
                int lastYear = DateTime.Now.Year - 1;
                League? league = (League)GetLeagueWithSerie(serie);
                Serie? lastYearSerie = GetSerie(league, lastYear.ToString());
                rank = GetPositionInSerie(lastYearSerie, opponent);
                //MessageBox.Show("Before: " + league.Name + " " + lastYearSerie.Year + " " + rank);

            }
            else
            {
                rank = GetPositionInSerie(serie, opponent);
            }
            //MessageBox.Show("After: " + rank.ToString());
            return rank;
        }
        public Team? GetOpponent(Game game, Team team)
        {
            if (team == game.HomeTeam)
            {
                return game.AwayTeam;
            }
            else if (team == game.AwayTeam)
            {
                return game.HomeTeam;
            }
            return null;
        }
        public Serie? GetLastYearsSerieWithTeam(Team team)
        {
            int lastYear = DateTime.Now.Year - 1;
            foreach (Competion competion in Competions)
            {
                League league;
                if (competion is League)
                {
                    league = (League)competion;
                    Serie? serie = GetSerie(league, lastYear.ToString());
                    if (serie != null)
                    {
                        Team? TeamInSerie = GetTeamFromSerie(serie, team);
                        if (TeamInSerie != null)
                        {
                            return serie;
                        }
                    }

                }
            }
            return null;
        }
        public League? GetLastYearsLeagueWithTeam(Team team)
        {
            int lastYear = DateTime.Now.Year - 1;
            foreach (Competion competion in Competions)
            {
                League league;
                if (competion is League)
                {
                    league = (League)competion;
                    Serie? serie = GetSerie(league, lastYear.ToString());
                    if (serie != null)
                    {
                        Team? TeamInSerie = GetTeamFromSerie(serie, team);
                        if (TeamInSerie != null)
                        {
                            return league;
                        }
                    }

                }
            }
            return null;
        }


        private void LoadSchedule(Serie serie, string adress)
        {
            string[] lines = System.IO.File.ReadAllLines(adress);
            int omg = 0;
            int maxTeamNameLength = 0;
            DateTime date;
            if (!adress.Contains("Html") && !adress.Contains("chance"))
            {
                for (int i = 0; i < lines.Length - 2; i++)
                {
                    if (lines[i].Contains("MEST LÄSTA")) break;
                    if (lines[i].Contains("Omg")) omg++;
                    else if (omg > 0)
                    {
                        if (lines[i].Contains("2022") || lines[i].Contains("2023"))
                        { // ADDS NEW TEAM
                            char[] dividers = { '-', ' ' };
                            string[] DateStrings = lines[i].Split(dividers);
                            int year = int.Parse(DateStrings[0]);
                            int month = int.Parse(DateStrings[1]);
                            int day = int.Parse(DateStrings[2]);
                            date = new DateTime(year, month, day);
                            Team? homeTeam = GetTeam(lines[i + 1]);
                            Team? awayTeam = GetTeam(lines[i + 2]);
                            if (homeTeam == null)
                            {
                                homeTeam = new(lines[i + 1]);
                                Teams.Add(homeTeam);
                                if (homeTeam.Name.Length > maxTeamNameLength)
                                {
                                    maxTeamNameLength = homeTeam.Name.Length;
                                }
                            }
                            if (awayTeam == null)
                            {
                                awayTeam = new(lines[i + 2]);
                                Teams.Add(awayTeam);
                                if (awayTeam.Name.Length > maxTeamNameLength)
                                {
                                    maxTeamNameLength = awayTeam.Name.Length;
                                }
                            }

                            if (GetTeamFromSerie(serie, homeTeam) == null) serie.SerieMembers.Add(new() { Team = homeTeam });
                            if (GetTeamFromSerie(serie, awayTeam) == null) serie.SerieMembers.Add(new() { Team = awayTeam });

                            if (lines.Length > i + 3 && lines[i + 3].Length == 5)
                            {
                                // Adds new Played Game
                                string[] result = lines[i + 3].Split(' ');
                                int hGoals = int.Parse(result[0]);
                                int aGoals = int.Parse(result[2]);
                                AddGame(homeTeam, awayTeam, hGoals, aGoals, omg, date, serie);
                            }
                            else
                            { // adds new Unplayed game
                                serie.Games.Add(new(homeTeam, awayTeam, omg, date, serie));
                            }
                        }
                    }
                    else if (lines[i].Contains("-"))
                    {
                    }
                }
                if (serie.Games.Count > 0)
                {
                    serie.Year = serie.Games[0].Date.Year.ToString();
                }
                else
                {
                    serie.Year = "Default";
                }

                serie.maxTeamNameLength = maxTeamNameLength;
                serie.nbrOfGames = serie.Games.Count;
                serie.NbrOfTeams = serie.SerieMembers.Count;
                SortTable(serie);
            }
        }
        private void AddGame(Team homeTeam, Team awayTeam, int hGoals, int aGoals, int omg, DateTime date, Serie serie)
        {
            Game newGame = new(homeTeam, awayTeam, hGoals, aGoals, omg, date, serie);
            homeTeam.Games.Add(newGame);
            awayTeam.Games.Add(newGame);
            SerieMember? homeMember = GetSerieMember(serie, homeTeam);
            SerieMember? awayMember = GetSerieMember(serie, awayTeam);

            int homePoints = GetTeamPointsInGame(newGame, homeTeam);
            int awayPoints = GetTeamPointsInGame(newGame, awayTeam);
            homeMember.Points += homePoints;
            awayMember.Points += awayPoints;
            homeMember.GoalsFor += hGoals;
            awayMember.GoalsFor += aGoals;
            homeMember.GoalsAgainst += aGoals;
            awayMember.GoalsAgainst += hGoals;
            homeMember.GamesPlayed++;
            awayMember.GamesPlayed++;

            if (homeTeam.Pitch == "Plastic")
            {
                homeMember.GamesPlayedOnPlastic++;
                awayMember.GamesPlayedOnPlastic++;
                homeMember.PointsOnPlastic += homePoints;
                awayMember.PointsOnPlastic += awayPoints;
            }
            else
            {
                homeMember.GamesPlayedOnGrass++;
                awayMember.GamesPlayedOnGrass++;
                homeMember.PointsOnGrass += homePoints;
                awayMember.PointsOnGrass += awayPoints;
            }


            serie.Games.Add(newGame);

        }

        private Team? GetTeam(string name)
        {
            string value;
            foreach (Team team in Teams)
            {
                TeamNameDictionary.TryGetValue(name, out value);

                if (value == null)
                {
                    value = name;
                }

                if (team.Name.Contains(value))
                {
                    return team;
                }
                else if (value.Contains(team.Name))
                {
                    SetNewTeamName(team, value);
                    return team;
                }
                else if (name.Contains(' '))
                {
                    string tempName = GetUpperCharsFromStringAsString(name);
                    if (team.Name.Contains(tempName))
                    {
                        return team;
                    }
                    else if (tempName.Contains(team.Name))
                    {
                        return team;
                    }
                }
            }
            return null;
        }

        private string GetUpperCharsFromStringAsString(string name)
        {
            char[] chars = name.ToCharArray();
            string s = "";
            foreach (char c in chars)
            {
                if (char.IsUpper(c))
                {
                    s += c;
                }
            }
            return s;
        }

        private void SetNewTeamName(Team team, string name)
        {
            team.Name = name.Trim();
        }

        private Team? GetTeamFromSerie(Serie serie, Team team)
        {
            List<SerieMember> serieMembers = serie.SerieMembers;
            foreach (SerieMember serieMember in serieMembers)
            {
                if (serieMember.Team == team) return serieMember.Team;
            }
            return null;
        }
        public void SortTable(Serie serie)
        {
            int NbrOfTeams = serie.NbrOfTeams;
            for (int i = 0; i < NbrOfTeams; i++)
            {
                for (int x = 0; x < NbrOfTeams; x++)
                {

                    if (serie.SerieMembers[x].Points < serie.SerieMembers[i].Points)
                    {
                        SwitchPositionInSerie(serie, i, x);
                    }
                    else if (serie.SerieMembers[x].Points == serie.SerieMembers[i].Points && serie.SerieMembers[x].GoalDiff < serie.SerieMembers[i].GoalDiff)
                    {
                        SwitchPositionInSerie(serie, i, x);
                    }
                    else if (serie.SerieMembers[x].Points == serie.SerieMembers[i].Points && serie.SerieMembers[x].GoalDiff == serie.SerieMembers[i].GoalDiff && serie.SerieMembers[x].GoalsFor < serie.SerieMembers[i].GoalsFor)
                    {
                        SwitchPositionInSerie(serie, i, x);
                    }
                    else if (serie.SerieMembers[x].Points == serie.SerieMembers[i].Points && serie.SerieMembers[x].GoalDiff == serie.SerieMembers[i].GoalDiff && serie.SerieMembers[x].GoalsFor == serie.SerieMembers[i].GoalsFor && string.Compare(serie.SerieMembers[x].Team.Name, serie.SerieMembers[i].Team.Name) > 0)
                    {
                        SwitchPositionInSerie(serie, i, x);
                    }
                }
            }
            int place = 1;
            foreach (SerieMember s in serie.SerieMembers)
            {
                s.Rank = place;
                place++;
            }
        }
        public int GetPositionInSerie(Serie serie, Team team)
        {
            SerieMember? serieMember = GetSerieMember(serie, team);
            if (serieMember != null)
            {
                return serieMember.Rank;
            }
            return 0;
        }
        private SerieMember? GetSerieMember(Serie serie, Team team)
        {
            foreach (SerieMember serieMember in serie.SerieMembers)
            {
                if (serieMember.Team == team) return serieMember;
            }
            return null;
        }
        private void SwitchPositionInSerie(Serie serie, int a, int b)
        {

            //MessageBox.Show($"Before: {serie.Teams[a].Points} < {serie.Teams[b].Points}");
            SerieMember temp = serie.SerieMembers[a];
            serie.SerieMembers[a] = serie.SerieMembers[b];
            serie.SerieMembers[b] = temp;
            //MessageBox.Show($"After: {serie.Teams[a].Points} < {serie.Teams[b].Points}");
        }

        public int GetNbrOfPlayedGamesByTeamInSerie(Serie serie, Team team)
        {
            int playedGames = 0;
            List<Game> games = serie.Games;
            foreach (Game game in games)
            {
                if (game.IsPlayed == true && (team == game.HomeTeam || team == game.AwayTeam))
                {
                    playedGames++;
                }
            }
            return playedGames;
        }
        public int GetTeamPointsInSerie(Serie serie, Team team, string? pitch)
        {
            int points = 0;
            List<Game> games = serie.Games;
            return GetPointsTakenInGames(games, team);
        }
        private int GetTeamPointsInGame(Game game, Team team)
        {
            if (game.HomeTeam == team || game.AwayTeam == team)
            {
                if (game.Winner == team) return 3;
                else if (game.HomeGoals == game.AwayGoals) return 1;
            }
            return 0;
        }
        private int GetPointsTakenInGames(List<Game> games, Team team)
        {
            int points = 0;
            foreach (Game game in games)
            {
                points += GetTeamPointsInGame(game, team);
            }
            return points;
        }
        public string GetTeamInSerieAsString(Serie serie, Team team)
        {
            SerieMember serieMember = GetSerieMember(serie, team);
            string emptySpaces = "";
            while (team.Name.Length + emptySpaces.Length < serie.maxTeamNameLength + 10)
            {
                emptySpaces += " ";
            }
            //if (pPerGame == "Grass")
            //{
            //    return $"{Name}{emptySpaces}\t{Games}\t{Points}\t{PointsPerGrassGame}\t{GoalDiff}\t{Difficulty}\t\t{SpecialAverage}\t\t{EndPoint}";
            //}
            //else if (pPerGame == "Plastic")
            //{
            //    return $"\n{Rank}:\t{Name}{emptySpaces}\t{Games}\t{Points}\t{PointsPerPlasticGame}\t{GoalDiff}\t{Difficulty}\t\t{SpecialAverage}\t\t{EndPoint}";
            //}
            return $"\n{serieMember.Rank}:\t{team.Name}{emptySpaces}\t{serieMember.GamesPlayed}\t{serieMember.Points}\t{PointsPerGame(serieMember)}\t{serieMember.GoalDiff}\t{serieMember.Difficulty}\t\t{serieMember.SpecialAverage}\t\t{serieMember.EndPoint}";
        }

        private double PointsPerGame(SerieMember serieMember, string? pitch = null)
        {
            if (pitch == null) return Math.Round(Convert.ToDouble(serieMember.Points) / Convert.ToDouble(serieMember.GamesPlayed), 2);
            else if (pitch == "Grass") return Math.Round(Convert.ToDouble(serieMember.PointsOnGrass) / Convert.ToDouble(serieMember.GamesPlayedOnGrass), 2);
            else if (pitch == "Plastic") return Math.Round(Convert.ToDouble(serieMember.PointsOnPlastic) / Convert.ToDouble(serieMember.GamesPlayedOnPlastic), 2);
            return 0;
        }

        public string GetTableHeadLine()
        {
            return "\tLag \t\t\tM\tPoäng \tp/m \tms \tMotstånd \tKommande\t Slutpoäng";
        }
        public Game? GetGame(Team homeTeam, Team awayTeam, string year)
        {
            List<Game> games = homeTeam.Games;
            //MessageBox.Show(homeTeam.Name + " - " + awayTeam.Name);
            foreach (Game game in games)
            {
                if (game.HomeTeam == homeTeam && game.AwayTeam == awayTeam && game.Date.Year == int.Parse(year))
                {
                    return game;
                }
            }
            return null;
        }
        public void SetGameOutcomeChancesFromOdds(Game game)
        {
            if (!(string.IsNullOrEmpty(game.Odds1) || string.IsNullOrEmpty(game.OddsX) || string.IsNullOrEmpty(game.Odds2)))
            {
                game.OddsChances1 = Math.Round(100 / double.Parse(game.Odds1.Replace(".", ",")), 2);
                game.OddsChancesX = Math.Round(100 / double.Parse(game.OddsX.Replace(".", ",")), 2);
                game.OddsChances2 = Math.Round(100 / double.Parse(game.Odds2.Replace(".", ",")), 2);
            }
        }

    }
}

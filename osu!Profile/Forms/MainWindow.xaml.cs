using HtmlAgilityPack;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using osu_Profile.IO;
using osu_Profile.OsuAPIObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace osu_Profile.Forms
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region Attributes
        public static IniFile config = new IniFile(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config.ini", "=");
        public Loop loopupdate = new Loop();

        Thread versioncheck;
        Thread loopthread;
        Thread loopfilethread;

        public static List<OutputFile> files = new List<OutputFile>();
        public static List<Event> lastplayedbeatmaps = new List<Event>();

        public static int mode = 0;
        public static int scoremode = 0;
        static String Username, APIKey;
        int rankingcomponents = 0;
        #endregion Variables

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            MWindow = this;
            config.Load();
            settingsPanel.ConfigFile = config;

            loopthread = new Thread(new ThreadStart(loopupdate.loop));
            loopthread.IsBackground = true;

            loopfilethread = new Thread(new ThreadStart(loopupdate.fileLoop));
            loopfilethread.IsBackground = true;
            loopfilethread.Start();

            versioncheck = new Thread(checkversion);
            versioncheck.IsBackground = true;
            versioncheck.Start();
            
            beatmapscheck.IsChecked = config.GetValue("User", "beatmaps", "false") == "true";
            playedbox.IsEnabled = config.GetValue("User", "beatmaps", "false") == "true";

            ThemeManager.AddAppTheme("FullLight", new Uri("pack://application:,,,/osu!Profile;component/Resources/MyStyle.xaml"));
        }
        #endregion

        #region Properties
        public String Ranked
        {
            get
            {
                return rankedbox.Text;
            }
            set
            {
                if (value == "0")
                    rankedbox.Text = "";
                else
                    rankedbox.Text = value;
            }
        }
        public String Level
        {
            get
            {
                return levelbox.Text;
            }
            set
            {
                if (value == "0")
                    levelbox.Text = "";
                else
                    levelbox.Text = value;
            }
        }
        public String Total
        {
            get
            {
                return totalbox.Text;
            }
            set
            {
                if (value == "0")
                    totalbox.Text = "";
                else
                    totalbox.Text = value;
            }
        }
        public String Rank
        {
            get
            {
                return rankbox.Text;
            }
            set
            {
                if (value == "0")
                    rankbox.Text = "";
                else
                    rankbox.Text = value;
            }
        }
        public String CountryRank
        {
            get
            {
                return countryrankbox.Text;
            }
            set
            {
                if (value == "0")
                    countryrankbox.Text = "";
                else
                    countryrankbox.Text = value;
            }
        }
        public String PP
        {
            get
            {
                return ppbox.Text;
            }
            set
            {
                if (value == "0")
                    ppbox.Text = "";
                else
                    ppbox.Text = value;
            }
        }
        public String Accuracy
        {
            get
            {
                return accuracybox.Text;
            }
            set
            {
                if (value == "0")
                    accuracybox.Text = "";
                else
                    accuracybox.Text = value;
            }
        }

        public String PlayTime
        {
            get
            {
                return playtimebox.Text;
            }
            set
            {
                if (value == "0")
                    playtimebox.Text = "";
                else
                    playtimebox.Text = value;
            }
        }
        public String PlayCount
        {
            get
            {
                return playcountbox.Text;
            }
            set
            {
                if (value == "0")
                    playcountbox.Text = "";
                else
                    playcountbox.Text = value;
            }
        }

        public String TotalHits
        {
            get
            {
                return totalhitsbox.Text;
            }
            set
            {
                if (value == "0")
                    totalhitsbox.Text = "";
                else
                    totalhitsbox.Text = value;
            }
        }
        public String HitsPerPlay
        {
            get
            {
                return hitsperplaybox.Text;
            }
            set
            {
                if (value == "0")
                    hitsperplaybox.Text = "";
                else
                    hitsperplaybox.Text = value;
            }
        }
        public String TopPP
        {
            get
            {
                return topPPbox.Text;
            }
            set
            {
                if (value == "0")
                    topPPbox.Text = "";
                else
                    topPPbox.Text = value;
            }
        }
        public String RankA
        {
            get
            {
                return rankAbox.Text;
            }
            set
            {
                if (value == "0")
                    rankAbox.Text = "";
                else
                    rankAbox.Text = value;
            }
        }
        public String RankS
        {
            get
            {
                return rankSbox.Text;
            }
            set
            {
                if (value == "0")
                    rankSbox.Text = "";
                else
                    rankSbox.Text = value;
            }
        }
        public String RankSH
        {
            get
            {
                return rankSHbox.Text;
            }
            set
            {
                if (value == "0")
                    rankSHbox.Text = "";
                else
                    rankSHbox.Text = value;
            }
        }
        public String RankSS
        {
            get
            {
                return rankSSbox.Text;
            }
            set
            {
                if (value == "0")
                    rankSSbox.Text = "";
                else
                    rankSSbox.Text = value;
            }
        }
        public String RankSSH
        {
            get
            {
                return rankSSHbox.Text;
            }
            set
            {
                if (value == "0")
                    rankSSHbox.Text = "";
                else
                    rankSSHbox.Text = value;
            }
        }

        public String TotalS
        {
            get
            {
                return totalSbox.Text;
            }
            set
            {
                if (value == "0")
                    totalSbox.Text = "";
                else
                    totalSbox.Text = value;
            }
        }

        public String TotalSS
        {
            get
            {
                return totalSSbox.Text;
            }
            set
            {
                if (value == "0")
                    totalSSbox.Text = "";
                else
                    totalSSbox.Text = value;
            }
        }


        public String RankedScoreChange
        {
            get
            {
                return rankedscorechangebox.Text;
            }
            set
            {
                if (value == "0")
                    rankedscorechangebox.Text = "";
                else
                    rankedscorechangebox.Text = value;
            }
        }
        public String LevelChange
        {
            get
            {
                return levelchangebox.Text;
            }
            set
            {
                if (value == "0")
                    levelchangebox.Text = "";
                else
                    levelchangebox.Text = value;
            }
        }
        public String TotalScoreChange
        {
            get
            {
                return totalscorechangebox.Text;
            }
            set
            {
                if (value == "0")
                    totalscorechangebox.Text = "";
                else
                    totalscorechangebox.Text = value;
            }
        }
        public String RankChange
        {
            get
            {
                return rankchangebox.Text;
            }
            set
            {
                if (value == "0")
                    rankchangebox.Text = "";
                else
                    rankchangebox.Text = value;
            }
        }
        public String CountryRankChange
        {
            get
            {
                return countryrankchangebox.Text;
            }
            set
            {
                if (value == "0")
                    countryrankchangebox.Text = "";
                else
                    countryrankchangebox.Text = value;
            }
        }
        public String PPChange
        {
            get
            {
                return ppchangebox.Text;
            }
            set
            {
                if (value == "0")
                    ppchangebox.Text = "";
                else
                    ppchangebox.Text = value;
            }
        }
        public String AccuracyChange
        {
            get
            {
                return accuracychangebox.Text;
            }
            set
            {
                if (value == "0")
                    accuracychangebox.Text = "";
                else
                    accuracychangebox.Text = value;
            }
        }
        public String PlayTimeChange
        {
            get
            {
                return playtimechangebox.Text;
            }
            set
            {
                if (value == "0")
                    playtimechangebox.Text = "";
                else
                    playtimechangebox.Text = value;
            }
        }
        public String PlayCountChange
        {
            get
            {
                return playcountchangebox.Text;
            }
            set
            {
                if (value == "0")
                    playcountchangebox.Text = "";
                else
                    playcountchangebox.Text = value;
            }
        }
        public String TotalHitsChange
        {
            get
            {
                return totalhitschangebox.Text;
            }
            set
            {
                if (value == "0")
                    totalhitschangebox.Text = "";
                else
                    totalhitschangebox.Text = value;
            }
        }
        public String HitsPerPlayChange
        {
            get
            {
                return hitsperplaychangebox.Text;
            }
            set
            {
                if (value == "0")
                    hitsperplaychangebox.Text = "";
                else
                    hitsperplaychangebox.Text = value;
            }
        }
        public String TopPPChange
        {
            get
            {
                return topPPchangebox.Text;
            }
            set
            {
                if (value == "0")
                    topPPchangebox.Text = "";
                else
                    topPPchangebox.Text = value;
            }
        }
        public String RankAChange
        {
            get
            {
                return rankAchangebox.Text;
            }
            set
            {
                if (value == "0")
                    rankAchangebox.Text = "";
                else
                    rankAchangebox.Text = value;
            }
        }
        public String RankSChange
        {
            get
            {
                return rankSchangebox.Text;
            }
            set
            {
                if (value == "0")
                    rankSchangebox.Text = "";
                else
                    rankSchangebox.Text = value;
            }
        }
        public String RankSHChange
        {
            get
            {
                return rankSHchangebox.Text;
            }
            set
            {
                if (value == "0")
                    rankSHchangebox.Text = "";
                else
                    rankSHchangebox.Text = value;
            }
        }
        public String RankSSChange
        {
            get
            {
                return rankSSchangebox.Text;
            }
            set
            {
                if (value == "0")
                    rankSSchangebox.Text = "";
                else
                    rankSSchangebox.Text = value;
            }
        }
        public String RankSSHChange
        {
            get
            {
                return rankSSHchangebox.Text;
            }
            set
            {
                if (value == "0")
                    rankSSHchangebox.Text = "";
                else
                    rankSSHchangebox.Text = value;
            }
        }
        public String TotalSChange
        {
            get
            {
                return totalSchangebox.Text;
            }
            set
            {
                if (value == "0")
                    totalSchangebox.Text = "";
                else
                    totalSchangebox.Text = value;
            }
        }

        public String TotalSSChange
        {
            get
            {
                return totalSSchangebox.Text;
            }
            set
            {
                if (value == "0")
                    totalSSchangebox.Text = "";
                else
                    totalSSchangebox.Text = value;
            }
        }

        public TextBox RankedBox
        {
            get
            {
                return rankedbox;
            }
        }
        public TextBox LevelBox
        {
            get
            {
                return levelbox;
            }
        }
        public TextBox TotalBox
        {
            get
            {
                return totalbox;
            }
        }
        public TextBox RankBox
        {
            get
            {
                return rankbox;
            }
        }
        public TextBox CountryRankBox
        {
            get
            {
                return countryrankbox;
            }
        }
        public TextBox PPBox
        {
            get
            {
                return ppbox;
            }
        }
        public TextBox AccuracyBox
        {
            get
            {
                return accuracybox;
            }
        }
        public TextBox PlayCountBox
        {
            get
            {
                return playcountbox;
            }
        }
        public TextBox TopPPBox
        {
            get
            {
                return topPPbox;
            }
        }
        public TextBox RankABox
        {
            get
            {
                return rankAbox;
            }
        }
        public TextBox RankSBox
        {
            get
            {
                return rankSbox;
            }
        }
        public TextBox RankSHBox
        {
            get
            {
                return rankSHbox;
            }
        }
        public TextBox RankSSBox
        {
            get
            {
                return rankSSbox;
            }
        }
        public TextBox RankSSHBox
        {
            get
            {
                return rankSSHbox;
            }
        }

        public TextBox RankedScoreChangeBox
        {
            get
            {
                return rankedscorechangebox;
            }
        }
        public TextBox LevelChangeBox
        {
            get
            {
                return levelchangebox;
            }
        }
        public TextBox TotalScoreChangeBox
        {
            get
            {
                return totalscorechangebox;
            }
        }
        public TextBox RankChangeBox
        {
            get
            {
                return rankchangebox;
            }
        }
        public TextBox CountryRankChangeBox
        {
            get
            {
                return countryrankchangebox;
            }
        }
        public TextBox PPChangeBox
        {
            get
            {
                return ppchangebox;
            }
        }
        public TextBox AccuracyChangeBox
        {
            get
            {
                return accuracychangebox;
            }
        }
        public TextBox PlayTimeChangeBox
        {
            get
            {
                return playtimechangebox;
            }
        }
        public TextBox PlayCountChangeBox
        {
            get
            {
                return playcountchangebox;
            }
        }
        public TextBox TotalHitsChangeBox
        {
            get
            {
                return totalhitschangebox;
            }
        }
        public TextBox HitsPerPlayChangeBox
        {
            get
            {
                return hitsperplaychangebox;
            }
        }
        public TextBox TopPPChangeBox
        {
            get
            {
                return topPPchangebox;
            }
        }
        public TextBox RankAChangeBox
        {
            get
            {
                return rankAchangebox;
            }
        }
        public TextBox RankSChangeBox
        {
            get
            {
                return rankSchangebox;
            }
        }
        public TextBox RankSHChangeBox
        {
            get
            {
                return rankAchangebox;
            }
        }
        public TextBox RankSSChangeBox
        {
            get
            {
                return rankSSchangebox;
            }
        }
        public TextBox RankSSHChangeBox
        {
            get
            {
                return rankSSHchangebox;
            }
        }

        public TextBox TotalSChangeBox
        {
            get
            {
                return totalSchangebox;
            }
        }

        public TextBox TotalSSChangeBox
        {
            get
            {
                return totalSSchangebox;
            }
        }
        public TextBox PlayBox
        {
            get
            {
                return playedbox;
            }
        }        

        public Player PlayerFirstState { get; set; }
        public Player PlayerPreviousState { get; set; }
        public Player PlayerActualState { get; set; }

        public static MainWindow MWindow { get; set; }
        #endregion

        #region Methods
        public static void checkversion()
        {

            using (WebClient client = new WebClient())
            {
                try
                {
                    String version = Regex.Split(client.DownloadString("http://entrivax.fr/osu!p/changelog.txt"), @"\r?\n|\r")[0].Trim().Substring(1);
                    String[] remoteversionnumbers = version.Split('.');
                    String[] actualversionnumbers = Assembly.GetEntryAssembly().GetName().Version.ToString().Split('.');
                    if (int.Parse(remoteversionnumbers[0]) > int.Parse(actualversionnumbers[0]))
                    {
                        ShowNewVersion();
                        return;
                    }
                    else if (int.Parse(remoteversionnumbers[0]) < int.Parse(actualversionnumbers[0]))
                    {
                        return;
                    }

                    if (int.Parse(remoteversionnumbers[1]) > int.Parse(actualversionnumbers[1]))
                    {
                        ShowNewVersion();
                        return;
                    }
                    else if (int.Parse(remoteversionnumbers[1]) < int.Parse(actualversionnumbers[1]))
                    {
                        return;
                    }

                    if (int.Parse(remoteversionnumbers[2]) > int.Parse(actualversionnumbers[2]))
                    {
                        ShowNewVersion();
                        return;
                    }
                }
                catch (Exception) { }
            }
        }

        private static void ShowNewVersion()
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                MessageBoxResult result = MessageBox.Show(Application.Current.MainWindow, "New version available, go download it?", "New version available", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Process.Start("http://entrivax.fr/osu!p");
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    MessageBoxResult result = MessageBox.Show(Application.Current.MainWindow, "New version available, go download it?", "New version available", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        Process.Start("http://entrivax.fr/osu!p");
                    }
                }));
            }
            return;
        }

        public bool Start(string user, string apikey)
        {
            bool downloaded = false;
            short retry = 0;
            while (!downloaded && retry < 3)
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        string apiReturn = client.DownloadString("https://osu.ppy.sh/api/get_user?k=" + apikey + "&u=" + user + "&m=" + MainWindow.mode);
                        apiReturn = apiReturn.Substring(1, apiReturn.Length - 2);
                        PlayerActualState = JsonConvert.DeserializeObject<Player>(apiReturn);
                        PlayerActualState.TopRanks = JsonConvert.DeserializeObject<Score[]>(client.DownloadString("https://osu.ppy.sh/api/get_user_best?k=" + apikey + "&u=" + user + "&m=" + MainWindow.mode));
                        PlayerActualState.Mode = MainWindow.mode;
                        PlayerFirstState = PlayerPreviousState = PlayerActualState;
                        downloaded = true;
                        config.SetValue("User", "APIkey", apikey);
                        config.SetValue("User", "LastUsername", user);
                        config.Export();
                        Username = user;
                        APIKey = apikey;
                    }
                }
                catch (Exception e) { downloaded = false; retry++; Console.WriteLine(e.StackTrace); }
            }
            if (!downloaded)
                return false;
            if (PlayerActualState != null && PlayerActualState.ID != 0)
            {
                this.Title = $"osu!Profile - {PlayerActualState.Username}";
                SetValue(rankedbox, PlayerActualState.RankedScore, "#,#");
                SetValue(levelbox, PlayerActualState.Level, "#,#.####");
                SetValue(totalbox, PlayerActualState.Score, "#,#");
                SetValue(rankbox, PlayerActualState.PPRank, "#,#");
                SetValue(countryrankbox, PlayerActualState.PPCountryRank, "#,#");
                SetValue(ppbox, PlayerActualState.PP, "#,#.##");
                SetValue(accuracybox, PlayerActualState.Accuracy, "#,#.#####");
                SetValue(playtimebox, PlayerActualState.PlayTime, "#,#");
                SetValue(playcountbox, PlayerActualState.PlayCount, "#,#");
                SetValue(totalhitsbox, PlayerActualState.Count300 + PlayerActualState.Count100 + PlayerActualState.Count50, "#,#");
                SetValue(hitsperplaybox, (PlayerActualState.Count300 + PlayerActualState.Count100 + PlayerActualState.Count50) / PlayerActualState.PlayCount, "#,#.#####");
                SetValue(rankAbox, PlayerActualState.RankA, "#,#");
                SetValue(rankSbox, PlayerActualState.RankS, "#,#");
                SetValue(rankSHbox, PlayerActualState.RankSH, "#,#");
                SetValue(rankSSbox, PlayerActualState.RankSS, "#,#");
                SetValue(rankSSHbox, PlayerActualState.RankSSH, "#,#");
                SetValue(totalSbox, PlayerActualState.RankS + PlayerActualState.RankSH, "#,#");
                SetValue(totalSSbox, PlayerActualState.RankSS + PlayerActualState.RankSSH, "#,#");
                if (PlayerActualState.TopRanks != null && PlayerActualState.TopRanks.Length > 0)
                    SetValue(topPPbox, PlayerActualState.TopRanks[0].PP, "#,#.#####");
                else
                    SetValue(topPPbox, 0, "");

                SetValue(levelchangebox, 0, "");
                SetValue(rankedscorechangebox, 0, "");
                SetValue(totalscorechangebox, 0, "");
                SetValue(rankchangebox, 0, "");
                SetValue(countryrankchangebox, 0, "");
                SetValue(ppchangebox, 0, "");
                SetValue(accuracychangebox, 0, "");
                SetValue(playtimechangebox, 0, "");
                SetValue(playcountchangebox, 0, "");
                SetValue(totalhitschangebox, 0, "");
                SetValue(hitsperplaybox, 0, "");
                SetValue(topPPbox, 0, "");
                SetValue(rankAbox, 0, "");
                SetValue(rankSbox, 0, "");
                SetValue(rankSHbox, 0, "");
                SetValue(rankSSbox, 0, "");
                SetValue(rankSSHbox, 0, "");
                SetValue(totalSbox, 0, "");
                SetValue(totalSSbox, 0, "");

                if (!loopthread.IsAlive)
                    loopthread.Start();

                new Thread(new ThreadStart((Action)(() =>
                {
                    HtmlWeb web = new HtmlWeb();
                    HtmlDocument doc = web.Load("http://osu.ppy.sh/u/" + PlayerActualState.ID);
                    var imgs = doc.DocumentNode.Descendants("img");
                    foreach (var img in imgs)
                    {
                        var alt = img.Attributes["alt"];
                        if (alt != null && alt.Value == "User avatar")
                        {
                            WebClient webClient = new WebClient();
                            byte[] data = webClient.DownloadData("http:" + img.Attributes["src"].Value);
                            MemoryStream stream = new MemoryStream(data);

                            BitmapImage image = new BitmapImage();
                            image.BeginInit();
                            image.StreamSource = stream;
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.EndInit();
                            image.Freeze();

                            window.Dispatcher.Invoke((Action)(() => { ((MainWindow)window).avatar.Source = image; }));
                        }
                    }
                }))).Start();
                return true;
            }
            return false;
        }

        public static void SetValue(TextBox textbox, int obj, String format) {
            if (obj != 0)
            {
                textbox.Text = obj.ToString(format, CultureInfo.InvariantCulture);
            }
            else
            {
                textbox.Text = "";
            }
        }
        public static void SetValue(TextBox textbox, float obj, String format)
        {
            if (obj != 0)
            {
                textbox.Text = obj.ToString(format, CultureInfo.InvariantCulture);
            }
            else
            {
                textbox.Text = "";
            }
        }
        public static void SetValue(TextBox textbox, long obj, String format)
        {
            if (obj != 0)
            {
                textbox.Text = obj.ToString(format, CultureInfo.InvariantCulture);
            }
            else
            {
                textbox.Text = "";
            }
        }

        public void UpdateRankingDisplay()
        {
            List<Control> controls = new List<Control>();

            if (MainWindow.config.GetValue("User", "levelbox", "true") == "true")
            {
                levelLab.Visibility = Visibility.Visible;
                levelbox.Visibility = Visibility.Visible;
                levelchangebox.Visibility = Visibility.Visible;
                controls.Add(levelLab);
                controls.Add(levelbox);
                controls.Add(levelchangebox);
            }
            else
            {
                levelLab.Visibility = Visibility.Hidden;
                levelbox.Visibility = Visibility.Hidden;
                levelchangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "rankscorebox", "true") == "true")
            {
                rscoreLab.Visibility = Visibility.Visible;
                rankedbox.Visibility = Visibility.Visible;
                rankedscorechangebox.Visibility = Visibility.Visible;
                controls.Add(rscoreLab);
                controls.Add(rankedbox);
                controls.Add(rankedscorechangebox);
            }
            else
            {
                rscoreLab.Visibility = Visibility.Hidden;
                rankedbox.Visibility = Visibility.Hidden;
                rankedscorechangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "totalscorebox", "true") == "true")
            {
                tscoreLab.Visibility = Visibility.Visible;
                totalbox.Visibility = Visibility.Visible;
                totalscorechangebox.Visibility = Visibility.Visible;
                controls.Add(tscoreLab);
                controls.Add(totalbox);
                controls.Add(totalscorechangebox);
            }
            else
            {
                tscoreLab.Visibility = Visibility.Hidden;
                totalbox.Visibility = Visibility.Hidden;
                totalscorechangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "rankbox", "true") == "true")
            {
                rankLab.Visibility = Visibility.Visible;
                rankbox.Visibility = Visibility.Visible;
                rankchangebox.Visibility = Visibility.Visible;
                controls.Add(rankLab);
                controls.Add(rankbox);
                controls.Add(rankchangebox);
            }
            else
            {
                rankLab.Visibility = Visibility.Hidden;
                rankbox.Visibility = Visibility.Hidden;
                rankchangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "countryrankbox", "true") == "true")
            {
                countryrankLab.Visibility = Visibility.Visible;
                countryrankbox.Visibility = Visibility.Visible;
                countryrankchangebox.Visibility = Visibility.Visible;
                controls.Add(countryrankLab);
                controls.Add(countryrankbox);
                controls.Add(countryrankchangebox);
            }
            else
            {
                countryrankLab.Visibility = Visibility.Hidden;
                countryrankbox.Visibility = Visibility.Hidden;
                countryrankchangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "ppbox", "true") == "true")
            {
                ppLab.Visibility = Visibility.Visible;
                ppbox.Visibility = Visibility.Visible;
                ppchangebox.Visibility = Visibility.Visible;
                controls.Add(ppLab);
                controls.Add(ppbox);
                controls.Add(ppchangebox);
            }
            else
            {
                ppLab.Visibility = Visibility.Hidden;
                ppbox.Visibility = Visibility.Hidden;
                ppchangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "accubox", "true") == "true")
            {
                accuLab.Visibility = Visibility.Visible;
                accuracybox.Visibility = Visibility.Visible;
                accuracychangebox.Visibility = Visibility.Visible;
                controls.Add(accuLab);
                controls.Add(accuracybox);
                controls.Add(accuracychangebox);
            }
            else
            {
                accuLab.Visibility = Visibility.Hidden;
                accuracybox.Visibility = Visibility.Hidden;
                accuracychangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "playtimebox", "true") == "true")
            {
                playtimeLab.Visibility = Visibility.Visible;
                playtimebox.Visibility = Visibility.Visible;
                playtimechangebox.Visibility = Visibility.Visible;
                controls.Add(playtimeLab);
                controls.Add(playtimebox);
                controls.Add(playtimechangebox);
            }
            else
            {
                playtimeLab.Visibility = Visibility.Hidden;
                playtimebox.Visibility = Visibility.Hidden;
                playtimechangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "playcountbox", "true") == "true")
            {
                playcountLab.Visibility = Visibility.Visible;
                playcountbox.Visibility = Visibility.Visible;
                playcountchangebox.Visibility = Visibility.Visible;
                controls.Add(playcountLab);
                controls.Add(playcountbox);
                controls.Add(playcountchangebox);
            }
            else
            {
                playcountLab.Visibility = Visibility.Hidden;
                playcountbox.Visibility = Visibility.Hidden;
                playcountchangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "totalhitsbox", "true") == "true")
            {
                totalhitsLab.Visibility = Visibility.Visible;
                totalhitsbox.Visibility = Visibility.Visible;
                totalhitschangebox.Visibility = Visibility.Visible;
                controls.Add(totalhitsLab);
                controls.Add(totalhitsbox);
                controls.Add(totalhitschangebox);
            }
            else
            {
                totalhitsLab.Visibility = Visibility.Hidden;
                totalhitsbox.Visibility = Visibility.Hidden;
                totalhitschangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "hitsperplaybox", "true") == "true")
            {
                hitsperplayLab.Visibility = Visibility.Visible;
                hitsperplaybox.Visibility = Visibility.Visible;
                hitsperplaychangebox.Visibility = Visibility.Visible;
                controls.Add(hitsperplayLab);
                controls.Add(hitsperplaybox);
                controls.Add(hitsperplaychangebox);
            }
            else
            {
                hitsperplayLab.Visibility = Visibility.Hidden;
                hitsperplaybox.Visibility = Visibility.Hidden;
                hitsperplaychangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "topPPbox", "true") == "true")
            {
                topPPLab.Visibility = Visibility.Visible;
                topPPbox.Visibility = Visibility.Visible;
                topPPchangebox.Visibility = Visibility.Visible;
                controls.Add(topPPLab);
                controls.Add(topPPbox);
                controls.Add(topPPchangebox);
            }
            else
            {
                topPPLab.Visibility = Visibility.Hidden;
                topPPbox.Visibility = Visibility.Hidden;
                topPPchangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "rankAbox", "true") == "true")
            {
                rankALab.Visibility = Visibility.Visible;
                rankAbox.Visibility = Visibility.Visible;
                rankAchangebox.Visibility = Visibility.Visible;
                controls.Add(rankALab);
                controls.Add(rankAbox);
                controls.Add(rankAchangebox);
            }
            else
            {
                rankALab.Visibility = Visibility.Hidden;
                rankAbox.Visibility = Visibility.Hidden;
                rankAchangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "rankSbox", "true") == "true")
            {
                rankSLab.Visibility = Visibility.Visible;
                rankSbox.Visibility = Visibility.Visible;
                rankSchangebox.Visibility = Visibility.Visible;
                controls.Add(rankSLab);
                controls.Add(rankSbox);
                controls.Add(rankSchangebox);
            }
            else
            {
                rankSLab.Visibility = Visibility.Hidden;
                rankSbox.Visibility = Visibility.Hidden;
                rankSchangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "rankSHbox", "true") == "true")
            {
                rankSHLab.Visibility = Visibility.Visible;
                rankSHbox.Visibility = Visibility.Visible;
                rankSHchangebox.Visibility = Visibility.Visible;
                controls.Add(rankSHLab);
                controls.Add(rankSHbox);
                controls.Add(rankSHchangebox);
            }
            else
            {
                rankSHLab.Visibility = Visibility.Hidden;
                rankSHbox.Visibility = Visibility.Hidden;
                rankSHchangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "rankSSbox", "true") == "true")
            {
                rankSSLab.Visibility = Visibility.Visible;
                rankSSbox.Visibility = Visibility.Visible;
                rankSSchangebox.Visibility = Visibility.Visible;
                controls.Add(rankSSLab);
                controls.Add(rankSSbox);
                controls.Add(rankSSchangebox);
            }
            else
            {
                rankSSLab.Visibility = Visibility.Hidden;
                rankSSbox.Visibility = Visibility.Hidden;
                rankSSchangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "rankSSHbox", "true") == "true")
            {
                rankSSHLab.Visibility = Visibility.Visible;
                rankSSHbox.Visibility = Visibility.Visible;
                rankSSHchangebox.Visibility = Visibility.Visible;
                controls.Add(rankSSHLab);
                controls.Add(rankSSHbox);
                controls.Add(rankSSHchangebox);
            }
            else
            {
                rankSSHLab.Visibility = Visibility.Hidden;
                rankSSHbox.Visibility = Visibility.Hidden;
                rankSSHchangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "totalSbox", "true") == "true")
            {
                totalSLab.Visibility = Visibility.Visible;
                totalSbox.Visibility = Visibility.Visible;
                totalSchangebox.Visibility = Visibility.Visible;
                controls.Add(totalSLab);
                controls.Add(totalSbox);
                controls.Add(totalSchangebox);
            }
            else
            {
                totalSLab.Visibility = Visibility.Hidden;
                totalSbox.Visibility = Visibility.Hidden;
                totalSchangebox.Visibility = Visibility.Hidden;
            }

            if (MainWindow.config.GetValue("User", "totalSSbox", "true") == "true")
            {
                totalSSLab.Visibility = Visibility.Visible;
                totalSSbox.Visibility = Visibility.Visible;
                totalSSchangebox.Visibility = Visibility.Visible;
                controls.Add(totalSSLab);
                controls.Add(totalSSbox);
                controls.Add(totalSSchangebox);
            }
            else
            {
                totalSSLab.Visibility = Visibility.Hidden;
                totalSSbox.Visibility = Visibility.Hidden;
                totalSSchangebox.Visibility = Visibility.Hidden;
            }

            rankingcomponents = controls.Count / 3;
            int count = 0;
            foreach (Control control in controls)
            {
                if (count % 3 == 0)
                {
                    Canvas.SetTop(control, 31 * (count / 3) + 13);
                }
                else
                {
                    Canvas.SetTop(control, 31 * (count / 3) + 14);
                }
                count++;
            }

            TabControl_SelectionChanged(null, null);
        }
        public void UpdateRankingControls()
        {
            if (MainWindow.MWindow.PlayerActualState != null)
            {
                MainWindow.MWindow.RankedScoreChangeBox.Dispatcher.Invoke(new Action(() =>
                {
                    MainWindow.MWindow.Ranked = MainWindow.MWindow.PlayerActualState.RankedScore.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.Level = MainWindow.MWindow.PlayerActualState.Level.ToString("#,#.####", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.Total = MainWindow.MWindow.PlayerActualState.Score.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.Rank = MainWindow.MWindow.PlayerActualState.PPRank.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.CountryRank = MainWindow.MWindow.PlayerActualState.PPCountryRank.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.PP = MainWindow.MWindow.PlayerActualState.PP.ToString("#,#.##", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.Accuracy = MainWindow.MWindow.PlayerActualState.Accuracy.ToString("#,#.#####", CultureInfo.InvariantCulture);
                    TimeSpan pt = TimeSpan.FromSeconds(MainWindow.MWindow.PlayerActualState.PlayTime);
                    MainWindow.MWindow.PlayTime = string.Format("{0}h {1}m {2}s", pt.Days * 24 + pt.Hours, pt.Minutes, pt.Seconds);
                    MainWindow.MWindow.PlayCount = MainWindow.MWindow.PlayerActualState.PlayCount.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.TotalHits = (MainWindow.MWindow.PlayerActualState.Count300 + MainWindow.MWindow.PlayerActualState.Count100 + MainWindow.MWindow.PlayerActualState.Count50).ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.HitsPerPlay = ((MainWindow.MWindow.PlayerActualState.Count300 + MainWindow.MWindow.PlayerActualState.Count100 + MainWindow.MWindow.PlayerActualState.Count50)/MainWindow.MWindow.PlayerActualState.PlayCount).ToString("#,#.##", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.RankA = MainWindow.MWindow.PlayerActualState.RankA.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.RankS = MainWindow.MWindow.PlayerActualState.RankS.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.RankSH = MainWindow.MWindow.PlayerActualState.RankSH.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.RankSS = MainWindow.MWindow.PlayerActualState.RankSS.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.RankSSH = MainWindow.MWindow.PlayerActualState.RankSSH.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.TotalS = (MainWindow.MWindow.PlayerActualState.RankS + MainWindow.MWindow.PlayerActualState.RankSH).ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.TotalSS = (MainWindow.MWindow.PlayerActualState.RankSS + MainWindow.MWindow.PlayerActualState.RankSSH).ToString("#,#", CultureInfo.InvariantCulture);

                    if (MainWindow.MWindow.PlayerActualState.TopRanks != null && MainWindow.MWindow.PlayerActualState.TopRanks.Length > 0)
                        MainWindow.MWindow.TopPP = MainWindow.MWindow.PlayerActualState.TopRanks[0].PP.ToString("#,#.#####", CultureInfo.InvariantCulture);

                    int ppRankDif = 0, ppCountryRankDif = 0, aCountDif = 0, sCountDif = 0, shCountDif = 0, ssCountDif = 0, sshCountDif = 0, totalsCountDif = 0, totalssCountDif = 0, playTimeDif = 0;
                    float levelDif = 0, ppDif = 0, accuracyDif = 0, topPPDif = 0, hitsperplayDif = 0, playCountDif = 0, totalHitsDif = 0;
                    long rankedScoreDif = 0, scoreDif = 0;
                    if (MainWindow.scoremode == 0) // Each game mode
                    {
                        rankedScoreDif = MainWindow.MWindow.PlayerActualState.RankedScore - MainWindow.MWindow.PlayerPreviousState.RankedScore;
                        levelDif = MainWindow.MWindow.PlayerActualState.Level - MainWindow.MWindow.PlayerPreviousState.Level;
                        scoreDif = MainWindow.MWindow.PlayerActualState.Score - MainWindow.MWindow.PlayerPreviousState.Score;
                        ppRankDif = MainWindow.MWindow.PlayerActualState.PPRank - MainWindow.MWindow.PlayerPreviousState.PPRank;
                        ppCountryRankDif = MainWindow.MWindow.PlayerActualState.PPCountryRank - MainWindow.MWindow.PlayerPreviousState.PPCountryRank;
                        ppDif = MainWindow.MWindow.PlayerActualState.PP - MainWindow.MWindow.PlayerPreviousState.PP;
                        accuracyDif = MainWindow.MWindow.PlayerActualState.Accuracy - MainWindow.MWindow.PlayerPreviousState.Accuracy;
                        playTimeDif = MainWindow.MWindow.PlayerActualState.PlayTime - MainWindow.MWindow.PlayerPreviousState.PlayTime;
                        playCountDif = MainWindow.MWindow.PlayerActualState.PlayCount - MainWindow.MWindow.PlayerPreviousState.PlayCount;
                        totalHitsDif = MainWindow.MWindow.PlayerActualState.Count300 + MainWindow.MWindow.PlayerActualState.Count100 + MainWindow.MWindow.PlayerActualState.Count50 - (MainWindow.MWindow.PlayerPreviousState.Count300 + MainWindow.MWindow.PlayerPreviousState.Count100 + MainWindow.MWindow.PlayerPreviousState.Count50);
                        hitsperplayDif = ((MainWindow.MWindow.PlayerActualState.Count300 + MainWindow.MWindow.PlayerActualState.Count100 + MainWindow.MWindow.PlayerActualState.Count50) / MainWindow.MWindow.PlayerActualState.PlayCount) - ((MainWindow.MWindow.PlayerPreviousState.Count300 + MainWindow.MWindow.PlayerPreviousState.Count100 + MainWindow.MWindow.PlayerPreviousState.Count50) / MainWindow.MWindow.PlayerPreviousState.PlayCount);
                        aCountDif = MainWindow.MWindow.PlayerActualState.RankA - MainWindow.MWindow.PlayerPreviousState.RankA;
                        sCountDif = MainWindow.MWindow.PlayerActualState.RankS - MainWindow.MWindow.PlayerPreviousState.RankS;
                        shCountDif = MainWindow.MWindow.PlayerActualState.RankSH - MainWindow.MWindow.PlayerPreviousState.RankSH;
                        ssCountDif = MainWindow.MWindow.PlayerActualState.RankSS - MainWindow.MWindow.PlayerPreviousState.RankSS;
                        sshCountDif = MainWindow.MWindow.PlayerActualState.RankSSH - MainWindow.MWindow.PlayerPreviousState.RankSSH;
                        totalsCountDif = MainWindow.MWindow.PlayerActualState.RankS + MainWindow.MWindow.PlayerActualState.RankSH - (MainWindow.MWindow.PlayerPreviousState.RankS + MainWindow.MWindow.PlayerPreviousState.RankSH);
                        totalssCountDif = MainWindow.MWindow.PlayerActualState.RankSS + MainWindow.MWindow.PlayerActualState.RankSSH - (MainWindow.MWindow.PlayerPreviousState.RankSS + MainWindow.MWindow.PlayerPreviousState.RankSSH);
                        if (MainWindow.MWindow.PlayerActualState.TopRanks != null && MainWindow.MWindow.PlayerActualState.TopRanks.Length > 0)
                            if (MainWindow.MWindow.PlayerPreviousState.TopRanks != null && MainWindow.MWindow.PlayerPreviousState.TopRanks.Length > 0)
                                topPPDif = MainWindow.MWindow.PlayerActualState.TopRanks[0].PP - MainWindow.MWindow.PlayerPreviousState.TopRanks[0].PP;
                            else
                                topPPDif = MainWindow.MWindow.PlayerActualState.TopRanks[0].PP;
                    }
                    else if (MainWindow.scoremode == 1) // This session mode
                    {
                        rankedScoreDif = MainWindow.MWindow.PlayerActualState.RankedScore - MainWindow.MWindow.PlayerFirstState.RankedScore;
                        levelDif = MainWindow.MWindow.PlayerActualState.Level - MainWindow.MWindow.PlayerFirstState.Level;
                        scoreDif = MainWindow.MWindow.PlayerActualState.Score - MainWindow.MWindow.PlayerFirstState.Score;
                        ppRankDif = MainWindow.MWindow.PlayerActualState.PPRank - MainWindow.MWindow.PlayerFirstState.PPRank;
                        ppCountryRankDif = MainWindow.MWindow.PlayerActualState.PPCountryRank - MainWindow.MWindow.PlayerFirstState.PPCountryRank;
                        ppDif = MainWindow.MWindow.PlayerActualState.PP - MainWindow.MWindow.PlayerFirstState.PP;
                        accuracyDif = MainWindow.MWindow.PlayerActualState.Accuracy - MainWindow.MWindow.PlayerFirstState.Accuracy;
                        playTimeDif = MainWindow.MWindow.PlayerActualState.PlayTime - MainWindow.MWindow.PlayerFirstState.PlayTime;
                        playCountDif = MainWindow.MWindow.PlayerActualState.PlayCount - MainWindow.MWindow.PlayerFirstState.PlayCount;
                        totalHitsDif = MainWindow.MWindow.PlayerActualState.Count300 + MainWindow.MWindow.PlayerActualState.Count100 + MainWindow.MWindow.PlayerActualState.Count50 - (MainWindow.MWindow.PlayerFirstState.Count300 + MainWindow.MWindow.PlayerFirstState.Count100 + MainWindow.MWindow.PlayerFirstState.Count50);
                        hitsperplayDif = ((MainWindow.MWindow.PlayerActualState.Count300 + MainWindow.MWindow.PlayerActualState.Count100 + MainWindow.MWindow.PlayerActualState.Count50) / MainWindow.MWindow.PlayerActualState.PlayCount) - ((MainWindow.MWindow.PlayerFirstState.Count300 + MainWindow.MWindow.PlayerFirstState.Count100 + MainWindow.MWindow.PlayerFirstState.Count50) / MainWindow.MWindow.PlayerFirstState.PlayCount);
                        aCountDif = MainWindow.MWindow.PlayerActualState.RankA - MainWindow.MWindow.PlayerFirstState.RankA;
                        sCountDif = MainWindow.MWindow.PlayerActualState.RankS - MainWindow.MWindow.PlayerFirstState.RankS;
                        shCountDif = MainWindow.MWindow.PlayerActualState.RankSH - MainWindow.MWindow.PlayerFirstState.RankSH;
                        ssCountDif = MainWindow.MWindow.PlayerActualState.RankSS - MainWindow.MWindow.PlayerFirstState.RankSS;
                        sshCountDif = MainWindow.MWindow.PlayerActualState.RankSSH - MainWindow.MWindow.PlayerFirstState.RankSSH;
                        totalsCountDif = MainWindow.MWindow.PlayerActualState.RankS + MainWindow.MWindow.PlayerActualState.RankSH - (MainWindow.MWindow.PlayerFirstState.RankS + MainWindow.MWindow.PlayerFirstState.RankSH);
                        totalssCountDif = MainWindow.MWindow.PlayerActualState.RankSS + MainWindow.MWindow.PlayerActualState.RankSSH - (MainWindow.MWindow.PlayerFirstState.RankSS + MainWindow.MWindow.PlayerFirstState.RankSSH);
                        if (MainWindow.MWindow.PlayerActualState.TopRanks != null && MainWindow.MWindow.PlayerActualState.TopRanks.Length > 0)
                            if (MainWindow.MWindow.PlayerFirstState.TopRanks != null && MainWindow.MWindow.PlayerFirstState.TopRanks.Length > 0)
                                topPPDif = MainWindow.MWindow.PlayerActualState.TopRanks[0].PP - MainWindow.MWindow.PlayerFirstState.TopRanks[0].PP;
                            else
                                topPPDif = MainWindow.MWindow.PlayerActualState.TopRanks[0].PP;
                    }
                    MainWindow.MWindow.RankedScoreChange = rankedScoreDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.LevelChange = levelDif.ToString("#,#0.####", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.TotalScoreChange = scoreDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.RankChange = ppRankDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.CountryRankChange = ppCountryRankDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.PPChange = ppDif.ToString("#,#0.##", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.AccuracyChange = accuracyDif.ToString("#,#0.#####", CultureInfo.InvariantCulture);
                    TimeSpan ptc = TimeSpan.FromSeconds(playTimeDif);
                    MainWindow.MWindow.PlayTimeChange = string.Format("{0}h {1}m {2}s", ptc.Days * 24 + ptc.Hours, ptc.Minutes, ptc.Seconds); ;
                    MainWindow.MWindow.PlayCountChange = playCountDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.TotalHitsChange = totalHitsDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.HitsPerPlayChange = hitsperplayDif.ToString("#,#0.##", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.TopPPChange = topPPDif.ToString("#,#0.#####", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.RankAChange = aCountDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.RankSChange = sCountDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.RankSHChange = shCountDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.RankSSChange = ssCountDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.RankSSHChange = sshCountDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.TotalSChange = totalsCountDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MainWindow.MWindow.TotalSSChange = totalssCountDif.ToString("#,#", CultureInfo.InvariantCulture);


                    if (ppDif > 0)
                    {
                        MainWindow.MWindow.PPChange = "+" + MainWindow.MWindow.PPChange;
                        MainWindow.MWindow.PPChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.PPChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (scoreDif > 0)
                    {
                        MainWindow.MWindow.TotalScoreChange = "+" + MainWindow.MWindow.TotalScoreChange;
                        MainWindow.MWindow.TotalScoreChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.TotalScoreChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (rankedScoreDif > 0)
                    {
                        MainWindow.MWindow.RankedScoreChange = "+" + MainWindow.MWindow.RankedScoreChange;
                        MainWindow.MWindow.RankedScoreChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.RankedScoreChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (levelDif > 0)
                    {
                        MainWindow.MWindow.LevelChange = "+" + MainWindow.MWindow.LevelChange;
                        MainWindow.MWindow.LevelChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.LevelChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (ppRankDif > 0)
                    {
                        MainWindow.MWindow.RankChange = (-ppRankDif).ToString("#,#", CultureInfo.InvariantCulture);
                        MainWindow.MWindow.RankChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else if (ppRankDif != 0)
                    {
                        MainWindow.MWindow.RankChange = "+" + (-ppRankDif).ToString("#,#", CultureInfo.InvariantCulture);
                        MainWindow.MWindow.RankChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }

                    if (ppCountryRankDif > 0)
                    {
                        MainWindow.MWindow.CountryRankChange = (-ppCountryRankDif).ToString("#,#", CultureInfo.InvariantCulture);
                        MainWindow.MWindow.CountryRankChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else if (ppCountryRankDif != 0)
                    {
                        MainWindow.MWindow.CountryRankChange = "+" + (-ppCountryRankDif).ToString("#,#", CultureInfo.InvariantCulture);
                        MainWindow.MWindow.CountryRankChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }

                    if (accuracyDif > 0)
                    {
                        MainWindow.MWindow.AccuracyChange = "+" + MainWindow.MWindow.AccuracyChange;
                        MainWindow.MWindow.AccuracyChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.AccuracyChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (playTimeDif > 0)
                    {
                        MainWindow.MWindow.PlayTimeChange = "+" + MainWindow.MWindow.PlayTimeChange;
                        MainWindow.MWindow.PlayTimeChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.PlayTimeChange = "";
                        MainWindow.MWindow.PlayTimeChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (playCountDif > 0)
                    {
                        MainWindow.MWindow.PlayCountChange = "+" + MainWindow.MWindow.PlayCountChange;
                        MainWindow.MWindow.PlayCountChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.PlayCountChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (totalHitsDif > 0)
                    {
                        MainWindow.MWindow.TotalHitsChange = "+" + MainWindow.MWindow.TotalHitsChange;
                        MainWindow.MWindow.TotalHitsChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.TotalHitsChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (hitsperplayDif > 0)
                    {
                        MainWindow.MWindow.HitsPerPlayChange = "+" + MainWindow.MWindow.HitsPerPlayChange;
                        MainWindow.MWindow.HitsPerPlayChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.HitsPerPlayChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (topPPDif > 0)
                    {
                        MainWindow.MWindow.TopPPChange = "+" + MainWindow.MWindow.TopPPChange;
                        MainWindow.MWindow.TopPPChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.TopPPChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (aCountDif > 0)
                    {
                        MainWindow.MWindow.RankAChange = "+" + MainWindow.MWindow.RankAChange;
                        MainWindow.MWindow.RankAChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.RankAChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (sCountDif > 0)
                    {
                        MainWindow.MWindow.RankSChange = "+" + MainWindow.MWindow.RankSChange;
                        MainWindow.MWindow.RankSChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.RankSChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (shCountDif > 0)
                    {
                        MainWindow.MWindow.RankSHChange = "+" + MainWindow.MWindow.RankSHChange;
                        MainWindow.MWindow.RankSHChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.RankSHChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (ssCountDif > 0)
                    {
                        MainWindow.MWindow.RankSSChange = "+" + MainWindow.MWindow.RankSSChange;
                        MainWindow.MWindow.RankSSChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.RankSSChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (sshCountDif > 0)
                    {
                        MainWindow.MWindow.RankSSHChange = "+" + MainWindow.MWindow.RankSSHChange;
                        MainWindow.MWindow.RankSSHChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.RankSSHChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (totalsCountDif > 0)
                    {
                        MainWindow.MWindow.TotalSChange = "+" + MainWindow.MWindow.TotalSChange;
                        MainWindow.MWindow.TotalSChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.TotalSChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (totalssCountDif > 0)
                    {
                        MainWindow.MWindow.TotalSSChange = "+" + MainWindow.MWindow.TotalSSChange;
                        MainWindow.MWindow.TotalSSChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MainWindow.MWindow.TotalSSChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                }));
            }
            else
            {
                MainWindow.MWindow.Ranked = "";
                MainWindow.MWindow.Level = "";
                MainWindow.MWindow.Total = "";
                MainWindow.MWindow.Rank = "";
                MainWindow.MWindow.CountryRank = "";
                MainWindow.MWindow.PP = "";
                MainWindow.MWindow.Accuracy = "";
                MainWindow.MWindow.PlayTime = "";
                MainWindow.MWindow.PlayCount = "";
                MainWindow.MWindow.TotalHits = "";
                MainWindow.MWindow.HitsPerPlay = "";
                MainWindow.MWindow.TopPP = "";
                MainWindow.MWindow.RankA = "";
                MainWindow.MWindow.RankS = "";
                MainWindow.MWindow.RankSH = "";
                MainWindow.MWindow.RankSS = "";
                MainWindow.MWindow.RankSSH = "";
                MainWindow.MWindow.TotalS = "";
                MainWindow.MWindow.TotalSS = "";

                MainWindow.MWindow.RankedScoreChange = "";
                MainWindow.MWindow.LevelChange = "";
                MainWindow.MWindow.TotalScoreChange = "";
                MainWindow.MWindow.RankChange = "";
                MainWindow.MWindow.CountryRankChange = "";
                MainWindow.MWindow.PPChange = "";
                MainWindow.MWindow.AccuracyChange = "";
                MainWindow.MWindow.PlayTimeChange = "";
                MainWindow.MWindow.PlayCountChange = "";
                MainWindow.MWindow.TotalHitsChange = "";
                MainWindow.MWindow.HitsPerPlayChange = "";
                MainWindow.MWindow.TopPPChange = "";
                MainWindow.MWindow.RankAChange = "";
                MainWindow.MWindow.RankSChange = "";
                MainWindow.MWindow.RankSHChange = "";
                MainWindow.MWindow.RankSSChange = "";
                MainWindow.MWindow.RankSSHChange = "";
                MainWindow.MWindow.TotalSChange = "";
                MainWindow.MWindow.TotalSSChange = "";
            }
        }

        public static void ChangeThemeColor(byte r, byte g, byte b)
        {
            ResourceDictionary dict = new ResourceDictionary();

            dict.Source = new Uri("pack://application:,,,/osu!Profile;component/Resources/Color.xaml");

            dict["HighlightColor"] = Color.FromArgb(0xFF, (byte)(r * 0.725f), (byte)(g * 0.725f), (byte)(b * 0.725f));
            dict["AccentColor"] = Color.FromArgb(0xCC, r, g, b);
            dict["AccentColor2"] = Color.FromArgb(0x99, r, g, b);
            dict["AccentColor3"] = Color.FromArgb(0x66, r, g, b);
            dict["AccentColor4"] = Color.FromArgb(0x33, r, g, b);

            ResourceDictionary dict2 = new ResourceDictionary();

            dict2.BeginInit();

            foreach (DictionaryEntry de in dict)
            {
                dict2.Add(de.Key, de.Value);
            }

            dict2.EndInit();

            Accent accent = ThemeManager.GetAccent(dict2);
            AppTheme theme = ThemeManager.GetAppTheme("FullLight");

            ThemeManager.ChangeAppStyle(Application.Current, accent, theme);
        }

        public static bool ContainsFilename(string filename)
        {
            foreach(OutputFile outputfile in files)
            {
                if (outputfile.Name.ToLower() == filename.ToLower())
                    return true;
            }
            return false;
        }

        public static int IndexOfFilename(string filename)
        {
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Name.ToLower() == filename.ToLower())
                    return i;
            }
            return -1;
        }
        #endregion

        #region Handlers
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            int nfiles = 0;
            int.TryParse(config.GetValue("User", "files", "0"), out nfiles);

            this.Topmost = config.GetValue("User", "topmost", "false") == "true";

            for (int i = 0; i < nfiles; i++)
            {
                int time = 0;
                int.TryParse(config.GetValue("Files", "filetime" + i, "0"), out time);
                OutputFile outputFile = new OutputFile(config.GetValue("Files", "filename" + i, ""), config.GetValue("Files", "filecontent" + i, "").Replace("\\n", Environment.NewLine), time);
                files.Add(outputFile);
                settingsPanel.FileList.Add(outputFile.Name);
            }

            string startupShotcut = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "osu!profile.lnk");

            if (File.Exists(startupShotcut))
            {
                if (config.GetValue("User", "startWithWindows", "false") == "false")
                    File.Delete(startupShotcut);
            }
            else
            {
                if (config.GetValue("User", "startWithWindows", "false") == "true")
                {
                    Shortcut.IShellLink link = (Shortcut.IShellLink)new Shortcut.ShellLink();

                    link.SetDescription("osu!profile");
                    link.SetPath(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

                    IPersistFile file = (IPersistFile)link;
                    file.Save(startupShotcut, false);
                }
            }

            UpdateRankingDisplay();
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (tab.SelectedIndex == 0)
            {
                if (rankingcomponents == 0)
                    tab.Height = 200;
                else
                    tab.Height = 68 + rankingcomponents*31;
            }
            else if (tab.SelectedIndex == 1)
            {
                tab.Height = 238;
            }
            this.Height = tab.Height + 40;
        }

        private void beatmapscheck_Checked(object sender, RoutedEventArgs e)
        {
            config.SetValue("User", "beatmaps", "true");
            config.Export();
            playedbox.IsEnabled = true;
        }

        private void beatmapscheck_Unchecked(object sender, RoutedEventArgs e)
        {
            config.SetValue("User", "beatmaps", "false");
            config.Export();
            playedbox.IsEnabled = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Options.IsOpen = true;
        }
        
        private void Options_ClosingFinished(object sender, RoutedEventArgs e)
        {
            config.Export();
        }
        #endregion

        public class Loop
        {
            #region Attribute
            private int timer = 5;
            #endregion

            #region Methods
            public void loop()
            {
                while (Thread.CurrentThread.IsAlive)
                {
                    UpdateRankingPanel();
                    ExportToFile();

                    if (MainWindow.config.GetValue("User", "beatmaps", "false") == "true")
                        UpdatePlayPanel();
                    ExportToFile();

                    TimeSpan interval = new TimeSpan(0, 0, timer);
                    Thread.Sleep(interval);
                }
            }

            public void fileLoop()
            {
                while (Thread.CurrentThread.IsAlive)
                {
                    for (int i = 0; i < MainWindow.files.Count; i++)
                    {
                        if (MainWindow.files[i].TimeLeft > 0)
                            MainWindow.files[i].TimeLeft--;
                    }
                    TimeSpan interval = new TimeSpan(0, 0, 1);
                    Thread.Sleep(interval);
                }
            }

            public void setTimer(int time)
            {
                this.timer = time;
            }

            private void UpdateRankingPanel()
            {
                bool downloaded = false;
                while (!downloaded)
                {
                    try
                    {
                        WebClient client = new WebClient();
                        string apiReturn = client.DownloadString("https://osu.ppy.sh/api/get_user?k=" + APIKey + "&u=" + Username + "&m=" + MainWindow.mode);
                        apiReturn = apiReturn.Substring(1, apiReturn.Length - 2);
                        long score = MainWindow.MWindow.PlayerActualState.Score;
                        Player tempState = JsonConvert.DeserializeObject<Player>(apiReturn);

                        tempState.Mode = MainWindow.mode;
                        if (tempState.Mode != MainWindow.MWindow.PlayerFirstState.Mode || tempState.ID != MainWindow.MWindow.PlayerFirstState.ID)
                        {
                            MainWindow.MWindow.PlayerPreviousState = MainWindow.MWindow.PlayerFirstState = MainWindow.MWindow.PlayerActualState = tempState;
                        }

                        if (tempState.Score != score)
                        {
                            MainWindow.MWindow.PlayerPreviousState = MainWindow.MWindow.PlayerActualState;
                            MainWindow.MWindow.PlayerActualState = tempState;
                            //TODO : Add
                            if (MainWindow.MWindow.PlayerPreviousState.PP < MainWindow.MWindow.PlayerActualState.PP)
                            {
                                MainWindow.MWindow.PlayerActualState.TopRanks = JsonConvert.DeserializeObject<Score[]>(client.DownloadString("https://osu.ppy.sh/api/get_user_best?k=" + APIKey + "&u=" + Username + "&m=" + MainWindow.mode));
                            }

                            for (int i = 0; i < MainWindow.files.Count; i++)
                            {
                                MainWindow.files[i].TimeLeft = MainWindow.files[i].Time;
                            }

                            if (config.GetValue("User", "popupEachMap", "false") == "true" && MainWindow.MWindow.PlayerPreviousState.RankedScore != MainWindow.MWindow.PlayerActualState.RankedScore)
                            {
                                MainWindow.MWindow.RankedScoreChangeBox.Dispatcher.Invoke(new Action(() =>
                                {
                                    MainWindow.MWindow.Activate();
                                    MainWindow.MWindow.Focus();
                                }));
                            }
                            else if (config.GetValue("User", "popupPP", "false") == "true" && MainWindow.MWindow.PlayerPreviousState.PP < MainWindow.MWindow.PlayerActualState.PP)
                            {
                                MainWindow.MWindow.RankedScoreChangeBox.Dispatcher.Invoke(new Action(() =>
                                {
                                    MainWindow.MWindow.Activate();
                                    MainWindow.MWindow.Focus();
                                }));
                            }
                        }
                        downloaded = true;
                    }
                    catch (Exception) { downloaded = false; Thread.Sleep(new TimeSpan(0, 0, 1)); }
                }

               MainWindow.MWindow.UpdateRankingControls();
            }

            private void UpdatePlayPanel()
            {
                Event[] events = null;
                bool downloaded = false;
                while (!downloaded)
                {
                    try
                    {
                        WebClient client = new WebClient();
                        string apiReturn = client.DownloadString("https://osu.ppy.sh/api/get_user_recent?k=" + APIKey + "&u=" + Username + "&m=" + MainWindow.mode);
                        events = JsonConvert.DeserializeObject<Event[]>(apiReturn);
                        downloaded = true;
                    }
                    catch (Exception) { downloaded = false; Thread.Sleep(new TimeSpan(0, 0, 1)); }
                }

                lastplayedbeatmaps.Clear();
                                
                foreach(Event ev in events){
                    ev.Initialize(APIKey);
                    lastplayedbeatmaps.Add(ev);
                }

                MainWindow.MWindow.PlayBox.Dispatcher.Invoke(new Action(() =>
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (Event ev in events)
                    {
                        sb.AppendLine(ev.Beatmap.Artist + " - " + ev.Beatmap.Title + " [" + ev.Beatmap.Difficulty + "]");
                        sb.AppendLine("BPM : " + ev.Beatmap.BPM + " - Stars : " + ev.Beatmap.Stars);
                        sb.AppendLine("CS:" + ev.Beatmap.CircleSize + " AR: " + ev.Beatmap.ApproachRate + " OD:" + ev.Beatmap.OverallDifficulty + " HP:" + ev.Beatmap.HealthDrain);
                        sb.AppendLine("Score : " + ev.Score.ToString("#,#", CultureInfo.InvariantCulture) + " " + ev.ModsString);
                        sb.Append("Rank : " + ev.Grade);
                        if (MainWindow.MWindow.PlayerActualState.PP != MainWindow.MWindow.PlayerPreviousState.PP)
                        {
                            sb.AppendLine("PP : " + (MainWindow.MWindow.PlayerActualState.PP - MainWindow.MWindow.PlayerPreviousState.PP));
                        }
                        sb.AppendLine("\n");
                    }

                    MainWindow.MWindow.PlayBox.Text = sb.ToString();
                }));
            }

            private void ExportToFile()
            {
                for (int i = 0; i < MainWindow.files.Count; i++)
                {
                    if (MainWindow.files[i].TimeLeft == 0 && MainWindow.files[i].Time > 0)
                    {
                        if (File.ReadAllText(MainWindow.files[i].Name) != "")
                            File.WriteAllText(MainWindow.files[i].Name, "");
                    }
                    else
                    {
                        String output = MainWindow.files[i].Content;
                        if (output != "")
                        {
                            MainWindow.MWindow.RankedScoreChangeBox.Dispatcher.Invoke(new Action(() =>
                            {
                                output = output.Replace("[/rankedscore]", MainWindow.MWindow.Ranked);
                                output = output.Replace("[/totalscore]", MainWindow.MWindow.Total);
                                output = output.Replace("[/lvl]", MainWindow.MWindow.Level);
                                output = output.Replace("[/rank]", MainWindow.MWindow.Rank);
                                output = output.Replace("[/countryrank]", MainWindow.MWindow.CountryRank);
                                output = output.Replace("[/pp]", MainWindow.MWindow.PP);
                                output = output.Replace("[/acc]", MainWindow.MWindow.Accuracy);
                                output = output.Replace("[/playtime]", MainWindow.MWindow.PlayTime);
                                output = output.Replace("[/playcount]", MainWindow.MWindow.PlayCount);
                                output = output.Replace("[/totalhits]", MainWindow.MWindow.TotalHits);
                                output = output.Replace("[/hitsperplay]", MainWindow.MWindow.HitsPerPlay);
                                output = output.Replace("[/toppp]", MainWindow.MWindow.TopPP);
                                output = output.Replace("[/arank]", MainWindow.MWindow.RankA);
                                output = output.Replace("[/srank]", MainWindow.MWindow.RankS);
                                output = output.Replace("[/shrank]", MainWindow.MWindow.RankSH);
                                output = output.Replace("[/ssrank]", MainWindow.MWindow.RankSS);
                                output = output.Replace("[/sshrank]", MainWindow.MWindow.RankSSH);
                                output = output.Replace("[/totals]", MainWindow.MWindow.TotalS);
                                output = output.Replace("[/totalss]", MainWindow.MWindow.TotalSS);

                                output = output.Replace("[/rankedscorechange]", MainWindow.MWindow.RankedScoreChange);
                                output = output.Replace("[/totalscorechange]", MainWindow.MWindow.TotalScoreChange);
                                output = output.Replace("[/lvlchange]", MainWindow.MWindow.LevelChange);
                                output = output.Replace("[/rankchange]", MainWindow.MWindow.RankChange);
                                output = output.Replace("[/countryrankchange]", MainWindow.MWindow.CountryRankChange);
                                output = output.Replace("[/ppchange]", MainWindow.MWindow.PPChange);
                                output = output.Replace("[/accchange]", MainWindow.MWindow.AccuracyChange);
                                output = output.Replace("[/playtimechange]", MainWindow.MWindow.PlayTimeChange);
                                output = output.Replace("[/playcountchange]", MainWindow.MWindow.PlayCountChange);
                                output = output.Replace("[/totalhitschange]", MainWindow.MWindow.TotalHitsChange);
                                output = output.Replace("[/hitsperplaychange]", MainWindow.MWindow.HitsPerPlayChange);
                                output = output.Replace("[/topppchange]", MainWindow.MWindow.TopPPChange);
                                output = output.Replace("[/arankchange]", MainWindow.MWindow.RankAChange);
                                output = output.Replace("[/srankchange]", MainWindow.MWindow.RankSChange);
                                output = output.Replace("[/shrankchange]", MainWindow.MWindow.RankSHChange);
                                output = output.Replace("[/ssrankchange]", MainWindow.MWindow.RankSSChange);
                                output = output.Replace("[/sshrankchange]", MainWindow.MWindow.RankSSHChange);
                                output = output.Replace("[/totalschange]", MainWindow.MWindow.TotalSChange);
                                output = output.Replace("[/totalsschange]", MainWindow.MWindow.TotalSSChange);

                                if (lastplayedbeatmaps.Count > 0)
                                {
                                    output = output.Replace("[/lpbArtist]", lastplayedbeatmaps[0].Beatmap.Artist);
                                    output = output.Replace("[/lpbAR]", lastplayedbeatmaps[0].Beatmap.ApproachRate.ToString());
                                    output = output.Replace("[/lpbBPM]", lastplayedbeatmaps[0].Beatmap.BPM.ToString());
                                    output = output.Replace("[/lpbCS]", lastplayedbeatmaps[0].Beatmap.CircleSize.ToString());
                                    output = output.Replace("[/lpbCreator]", lastplayedbeatmaps[0].Beatmap.Creator.ToString());
                                    output = output.Replace("[/lpbDifficulty]", lastplayedbeatmaps[0].Beatmap.Difficulty.ToString());
                                    output = output.Replace("[/lpbHP]", lastplayedbeatmaps[0].Beatmap.HealthDrain.ToString());
                                    output = output.Replace("[/lpbID]", lastplayedbeatmaps[0].Beatmap.ID.ToString());
                                    output = output.Replace("[/lpbOD]", lastplayedbeatmaps[0].Beatmap.OverallDifficulty.ToString());
                                    output = output.Replace("[/lpbSetID]", lastplayedbeatmaps[0].Beatmap.SetID.ToString());
                                    output = output.Replace("[/lpbStars]", lastplayedbeatmaps[0].Beatmap.Stars.ToString());
                                    output = output.Replace("[/lpbTitle]", lastplayedbeatmaps[0].Beatmap.Title.ToString());
                                    output = output.Replace("[/lpbGrade]", lastplayedbeatmaps[0].Grade);
                                    output = output.Replace("[/lpbMods]", lastplayedbeatmaps[0].ModsString);
                                    output = output.Replace("[/lpbScore]", lastplayedbeatmaps[0].Score.ToString());
                                }
                            }));
                        }
                        try
                        {
                            if (!File.Exists(MainWindow.files[i].Name) || File.ReadAllText(MainWindow.files[i].Name) != output)
                                File.WriteAllText(MainWindow.files[i].Name, output);
                        }
                        catch (Exception) { }
                    }
                }
            }
            #endregion
        }
    }
}

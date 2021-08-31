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
        #region Attributes and variables
        public static IniFile config = new IniFile(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config.ini", "=");
        public Loop loopupdate = new Loop();

        //Thread versioncheck;
        Thread loopthread;
        Thread loopfilethread;

        public static List<OutputFile> files = new List<OutputFile>();
        public static List<Event> lastplayedbeatmaps = new List<Event>();

        public static int mode = 0;
        public static int scoremode = 1;
        public static int scoremodeOld = 0;
        public static int userID = 0;
        static String Username, APIKey;
        Player PrevStatState = null;
        int rankingcomponents = 0;
        #endregion

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

            ///Version check was through website of Entrivax - a person who made original version of the program
            ///Now the site throws 404 errors trying to access the link to check version, causing a WebException in this program.
            ///For this reason, version check was commented out.
            //versioncheck = new Thread(checkversion);
            //versioncheck.IsBackground = true;
            //versioncheck.Start();
            
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

        public String ScoreRank
        {
            get
            {
                return scorerankbox.Text;
            }
            set
            {
                if (value == "0")
                    scorerankbox.Text = "";
                else
                    scorerankbox.Text = value;
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

        public String TSPerPlay
        {
            get
            {
                return tsperplaybox.Text;
            }
            set
            {
                if (value == "0")
                    tsperplaybox.Text = "";
                else
                    tsperplaybox.Text = value;
            }
        }

        public String RSPerPlay
        {
            get
            {
                return rsperplaybox.Text;
            }
            set
            {
                if (value == "0")
                    rsperplaybox.Text = "";
                else
                    rsperplaybox.Text = value;
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

        public String Clears
        {
            get
            {
                return clearsbox.Text;
            }
            set
            {
                if (value == "0")
                    clearsbox.Text = "";
                else
                    clearsbox.Text = value;
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
        public String ScoreRankChange
        {
            get
            {
                return scorerankchangebox.Text;
            }
            set
            {
                if (value == "0")
                    scorerankchangebox.Text = "";
                else
                    scorerankchangebox.Text = value;
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

        public String TSPerPlayChange
        {
            get
            {
                return tsperplaychangebox.Text;
            }
            set
            {
                if (value == "0")
                    tsperplaychangebox.Text = "";
                else
                    tsperplaychangebox.Text = value;
            }
        }

        public String RSPerPlayChange
        {
            get
            {
                return rsperplaychangebox.Text;
            }
            set
            {
                if (value == "0")
                    rsperplaychangebox.Text = "";
                else
                    rsperplaychangebox.Text = value;
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

        public String ClearsChange
        {
            get
            {
                return clearschangebox.Text;
            }
            set
            {
                if (value == "0")
                    clearschangebox.Text = "";
                else
                    clearschangebox.Text = value;
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
        public TextBox ScoreRankBox
        {
            get
            {
                return scorerankbox;
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

        public TextBox ClearsBox
        {
            get
            {
                return clearsbox;
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
        public TextBox ScoreRankChangeBox
        {
            get
            {
                return scorerankchangebox;
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

        public TextBox TSPerPlayChangeBox
        {
            get
            {
                return tsperplaychangebox;
            }
        }

        public TextBox RSPerPlayChangeBox
        {
            get
            {
                return rsperplaychangebox;
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
                return rankSHchangebox;
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

        public TextBox ClearsChangeBox
        {
            get
            {
                return clearschangebox;
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
        /* This Function is outdated because it would access a page that doesn't exist.
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
        */
        public bool Start(string user, string apikey)
        {
            bool downloaded = false;
            short retry = 0;
            while (!downloaded && retry < 5)
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        string apiReturn = client.DownloadString("https://osu.ppy.sh/api/get_user?k=" + apikey + "&u=" + user + "&m=" + mode);
                        apiReturn = apiReturn.Substring(1, apiReturn.Length - 2);
                        PlayerActualState = JsonConvert.DeserializeObject<Player>(apiReturn);
                        userID = PlayerActualState.ID;
                        PlayerActualState.TopRanks = JsonConvert.DeserializeObject<Score[]>(client.DownloadString("https://osu.ppy.sh/api/get_user_best?k=" + apikey + "&u=" + user + "&m=" + mode + "&limit=" + 2));
                        PlayerActualState.Mode = mode;
                        PlayerActualState.scoerinfo = new Scoerapi { ScoreRank = 0, ID = 0, SCOER = 0, Scoer_username = "None" };
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
            downloaded = false;
            retry = 0;
            while (!downloaded && retry <= 3)
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        string scoerapiReturn = client.DownloadString("https://score.respektive.pw/u/" + userID + "?m=" + mode);
                        scoerapiReturn = scoerapiReturn.Substring(1, scoerapiReturn.Length - 2);
                        PlayerActualState.scoerinfo = JsonConvert.DeserializeObject<Scoerapi>(scoerapiReturn);
                        downloaded = true;
                        PlayerFirstState.scoerinfo = PlayerPreviousState.scoerinfo = PlayerActualState.scoerinfo;
                    }

                }
                catch (Exception e) { downloaded = false; retry++; Console.WriteLine(e.StackTrace); }
            if (!downloaded)
            {
                PlayerActualState.scoerinfo = new Scoerapi { ScoreRank = 0, ID = 0, SCOER = 0, Scoer_username = "None" };
                PlayerFirstState.scoerinfo = PlayerPreviousState.scoerinfo = PlayerActualState.scoerinfo;
            }
            if (PlayerActualState != null && PlayerActualState.ID != 0)
            {
                this.Title = $"osu!Profile - {PlayerActualState.Username}";
                SetValue(rankedbox, PlayerActualState.RankedScore, "#,#");
                SetValue(levelbox, PlayerActualState.Level, "#,#.####");
                SetValue(totalbox, PlayerActualState.Score, "#,#");
                SetValue(rankbox, PlayerActualState.PPRank, "#,#");
                SetValue(countryrankbox, PlayerActualState.PPCountryRank, "#,#");
                SetValue(ppbox, PlayerActualState.PP, "#,#.##");
                SetValue(accuracybox, (PlayerActualState.Accuracy / 100), "#,#.#####%");
                SetValue(playtimebox, PlayerActualState.PlayTime, "#,#");
                SetValue(playcountbox, PlayerActualState.PlayCount, "#,#");
                SetValue(totalhitsbox, PlayerActualState.Count300 + PlayerActualState.Count100 + PlayerActualState.Count50, "#,#");
                SetValue(hitsperplaybox, (PlayerActualState.Count300 + PlayerActualState.Count100 + PlayerActualState.Count50) / PlayerActualState.PlayCount, "#,#.##");
                SetValue(tsperplaybox, PlayerActualState.Score / PlayerActualState.PlayCount, "#,#.##");
                SetValue(rsperplaybox, PlayerActualState.RankedScore / PlayerActualState.PlayCount, "#,#.##");
                SetValue(rankAbox, PlayerActualState.RankA, "#,#");
                SetValue(rankSbox, PlayerActualState.RankS, "#,#");
                SetValue(rankSHbox, PlayerActualState.RankSH, "#,#");
                SetValue(rankSSbox, PlayerActualState.RankSS, "#,#");
                SetValue(rankSSHbox, PlayerActualState.RankSSH, "#,#");
                SetValue(totalSbox, PlayerActualState.RankS + PlayerActualState.RankSH, "#,#");
                SetValue(totalSSbox, PlayerActualState.RankSS + PlayerActualState.RankSSH, "#,#");
                if (PlayerActualState.scoerinfo != null)
                {
                    if (PlayerActualState.scoerinfo.ID != 0)
                    {
                        SetValue(scorerankbox, PlayerActualState.scoerinfo.ScoreRank, "#,#");
                    }
                }
                else
                    MWindow.ScoreRankBox.Text = "No Score Rank";
                int clearcount = 0;
                clearcount = PlayerActualState.RankA + PlayerActualState.RankS + PlayerActualState.RankSH + PlayerActualState.RankSS + PlayerActualState.RankSSH;
                SetValue(clearsbox, clearcount, "#,#");
                if (PlayerActualState.TopRanks != null && PlayerActualState.TopRanks.Length > 0)
                    SetValue(topPPbox, PlayerActualState.TopRanks[0].PP, "#,#.#####");
                else
                    SetValue(topPPbox, 0, "");
                SetValue(levelchangebox, 0, "");
                SetValue(rankedscorechangebox, 0, "");
                SetValue(totalscorechangebox, 0, "");
                SetValue(rankchangebox, 0, "");
                SetValue(scorerankchangebox, 0, "");
                SetValue(countryrankchangebox, 0, "");
                SetValue(ppchangebox, 0, "");
                SetValue(accuracychangebox, 0, "");
                SetValue(playtimechangebox, 0, "");
                SetValue(playcountchangebox, 0, "");
                SetValue(totalhitschangebox, 0, "");
                SetValue(hitsperplaychangebox, 0, "");
                SetValue(tsperplaychangebox, 0, "");
                SetValue(rsperplaychangebox, 0, "");
                SetValue(topPPchangebox, 0, "");
                SetValue(rankAchangebox, 0, "");
                SetValue(rankSchangebox, 0, "");
                SetValue(rankSHchangebox, 0, "");
                SetValue(rankSSchangebox, 0, "");
                SetValue(rankSSHchangebox, 0, "");
                SetValue(totalSchangebox, 0, "");
                SetValue(totalSSchangebox, 0, "");
                SetValue(clearschangebox, 0, "");

                if (!loopthread.IsAlive)
                    loopthread.Start();
                ///Old code for user image, used old osu site. The new code below does not. :)
                /*
                new Thread(new ThreadStart((Action)(() =>
                {
                    HtmlWeb web = new HtmlWeb();
                    HtmlDocument doc = web.Load("http://old.ppy.sh/u/" + PlayerActualState.ID);
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
                */
                ///New code for getting user image.
                new Thread(new ThreadStart((Action)(() =>
                {
                    string url = "http://s.ppy.sh/a/" + userID;
                    WebClient client = new WebClient();
                    byte[] data = client.DownloadData(url);
                    MemoryStream stream = new MemoryStream(data);
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();
                    window.Dispatcher.Invoke((Action)(() => { ((MainWindow)window).avatar.Source = image; }));
                            }))).Start();
                return true;
            }
            return false;
        }

        public static void SetValue(TextBox textbox, int obj, String format) {
            if (obj != 0)
            {
                textbox.Text = obj.ToString(format, CultureInfo.InvariantCulture);
                ///Prepend "#" if the value is a rank.
                if ((obj == MWindow.PlayerActualState.PPRank) || (obj == MWindow.PlayerActualState.PPCountryRank))
                {
                    textbox.Text = "#" + textbox.Text;
                }
                if ((MWindow.PlayerActualState.scoerinfo.ScoreRank != 0)) {
                    if (obj == MWindow.PlayerActualState.scoerinfo.ScoreRank)
                    {
                        textbox.Text = "#" + textbox.Text;
                    }
                }
                ///If the value is playtime
                if ((obj == MWindow.PlayerActualState.PlayTime)) {
                    TimeSpan pt = TimeSpan.FromSeconds(obj);
                    MWindow.PlayTime = string.Format("{0}h {1}m {2}s", pt.Days * 24 + pt.Hours, pt.Minutes, pt.Seconds);
                }
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

            if (config.GetValue("User", "scorerankbox", "true") == "true")
            {
                scorerankLab.Visibility = Visibility.Visible;
                scorerankbox.Visibility = Visibility.Visible;
                scorerankchangebox.Visibility = Visibility.Visible;
                controls.Add(scorerankLab);
                controls.Add(scorerankbox);
                controls.Add(scorerankchangebox);
            }
            else
            {
                scorerankLab.Visibility = Visibility.Hidden;
                scorerankbox.Visibility = Visibility.Hidden;
                scorerankchangebox.Visibility = Visibility.Hidden;
            }

            if (config.GetValue("User", "levelbox", "true") == "true")
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

            if (config.GetValue("User", "rankscorebox", "true") == "true")
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

            if (config.GetValue("User", "totalscorebox", "true") == "true")
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

            if (config.GetValue("User", "rankbox", "true") == "true")
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

            if (config.GetValue("User", "countryrankbox", "true") == "true")
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

            if (config.GetValue("User", "ppbox", "true") == "true")
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

            if (config.GetValue("User", "accubox", "true") == "true")
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

            if (config.GetValue("User", "playtimebox", "true") == "true")
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

            if (config.GetValue("User", "playcountbox", "true") == "true")
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

            if (config.GetValue("User", "totalhitsbox", "true") == "true")
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

            if (config.GetValue("User", "hitsperplaybox", "true") == "true")
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

            if (config.GetValue("User", "tsperplaybox", "true") == "true")
            {
                tsperplayLab.Visibility = Visibility.Visible;
                tsperplaybox.Visibility = Visibility.Visible;
                tsperplaychangebox.Visibility = Visibility.Visible;
                controls.Add(tsperplayLab);
                controls.Add(tsperplaybox);
                controls.Add(tsperplaychangebox);
            }
            else
            {
                tsperplayLab.Visibility = Visibility.Hidden;
                tsperplaybox.Visibility = Visibility.Hidden;
                tsperplaychangebox.Visibility = Visibility.Hidden;
            }

            if (config.GetValue("User", "rsperplaybox", "true") == "true")
            {
                rsperplayLab.Visibility = Visibility.Visible;
                rsperplaybox.Visibility = Visibility.Visible;
                rsperplaychangebox.Visibility = Visibility.Visible;
                controls.Add(rsperplayLab);
                controls.Add(rsperplaybox);
                controls.Add(rsperplaychangebox);
            }
            else
            {
                rsperplayLab.Visibility = Visibility.Hidden;
                rsperplaybox.Visibility = Visibility.Hidden;
                rsperplaychangebox.Visibility = Visibility.Hidden;
            }

            if (config.GetValue("User", "topPPbox", "true") == "true")
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

            if (config.GetValue("User", "rankAbox", "true") == "true")
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

            if (config.GetValue("User", "rankSbox", "true") == "true")
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

            if (config.GetValue("User", "rankSHbox", "true") == "true")
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

            if (config.GetValue("User", "rankSSbox", "true") == "true")
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

            if (config.GetValue("User", "rankSSHbox", "true") == "true")
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

            if (config.GetValue("User", "totalSbox", "true") == "true")
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

            if (config.GetValue("User", "totalSSbox", "true") == "true")
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

            if (config.GetValue("User", "clearsbox", "true") == "true")
            {
                clearsLab.Visibility = Visibility.Visible;
                clearsbox.Visibility = Visibility.Visible;
                clearschangebox.Visibility = Visibility.Visible;
                controls.Add(clearsLab);
                controls.Add(clearsbox);
                controls.Add(clearschangebox);
            }
            else
            {
                clearsLab.Visibility = Visibility.Hidden;
                clearsbox.Visibility = Visibility.Hidden;
                clearschangebox.Visibility = Visibility.Hidden;
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
            if (MWindow.PlayerActualState != null)
            {
                MWindow.RankedScoreChangeBox.Dispatcher.Invoke(new Action(() =>
                {
                MWindow.Ranked = MWindow.PlayerActualState.RankedScore.ToString("#,#", CultureInfo.InvariantCulture);
                if (MWindow.PlayerActualState.scoerinfo.ID != 0)
                {
                    MWindow.ScoreRank = MWindow.PlayerActualState.scoerinfo.ScoreRank.ToString("#,#", CultureInfo.InvariantCulture);
                }
                MWindow.Level = MWindow.PlayerActualState.Level.ToString("#,#.####", CultureInfo.InvariantCulture);
                MWindow.Total = MWindow.PlayerActualState.Score.ToString("#,#", CultureInfo.InvariantCulture);
                MWindow.Rank = MWindow.PlayerActualState.PPRank.ToString("#,#", CultureInfo.InvariantCulture);
                MWindow.CountryRank = MWindow.PlayerActualState.PPCountryRank.ToString("#,#", CultureInfo.InvariantCulture);
                MWindow.PP = MWindow.PlayerActualState.PP.ToString("#,#.##", CultureInfo.InvariantCulture);
                MWindow.Accuracy = (MWindow.PlayerActualState.Accuracy / 100).ToString("#,#.#####%", CultureInfo.InvariantCulture);
                TimeSpan pt = TimeSpan.FromSeconds(MWindow.PlayerActualState.PlayTime);
                MWindow.PlayTime = string.Format("{0}h {1}m {2}s", pt.Days * 24 + pt.Hours, pt.Minutes, pt.Seconds);
                MWindow.PlayCount = MWindow.PlayerActualState.PlayCount.ToString("#,#", CultureInfo.InvariantCulture);
                MWindow.TotalHits = (MWindow.PlayerActualState.Count300 + MWindow.PlayerActualState.Count100 + MWindow.PlayerActualState.Count50).ToString("#,#", CultureInfo.InvariantCulture);
                MWindow.HitsPerPlay = ((MWindow.PlayerActualState.Count300 + MWindow.PlayerActualState.Count100 + MWindow.PlayerActualState.Count50) / MWindow.PlayerActualState.PlayCount).ToString("#,#.##", CultureInfo.InvariantCulture);
                MWindow.TSPerPlay = (MWindow.PlayerActualState.Score / MWindow.PlayerActualState.PlayCount).ToString("#,#.##", CultureInfo.InvariantCulture);
                MWindow.RSPerPlay = (MWindow.PlayerActualState.RankedScore / MWindow.PlayerActualState.PlayCount).ToString("#,#.##", CultureInfo.InvariantCulture);
                MWindow.RankA = MWindow.PlayerActualState.RankA.ToString("#,#", CultureInfo.InvariantCulture);
                MWindow.RankS = MWindow.PlayerActualState.RankS.ToString("#,#", CultureInfo.InvariantCulture);
                MWindow.RankSH = MWindow.PlayerActualState.RankSH.ToString("#,#", CultureInfo.InvariantCulture);
                MWindow.RankSS = MWindow.PlayerActualState.RankSS.ToString("#,#", CultureInfo.InvariantCulture);
                MWindow.RankSSH = MWindow.PlayerActualState.RankSSH.ToString("#,#", CultureInfo.InvariantCulture);
                MWindow.TotalS = (MWindow.PlayerActualState.RankS + MWindow.PlayerActualState.RankSH).ToString("#,#", CultureInfo.InvariantCulture);
                MWindow.TotalSS = (MWindow.PlayerActualState.RankSS + MWindow.PlayerActualState.RankSSH).ToString("#,#", CultureInfo.InvariantCulture);
                int clearcount = MWindow.PlayerActualState.RankA + MWindow.PlayerActualState.RankS + MWindow.PlayerActualState.RankSH
                    + MWindow.PlayerActualState.RankSS + MWindow.PlayerActualState.RankSSH;
                MWindow.Clears = (clearcount).ToString("#,#", CultureInfo.InvariantCulture);
                if (MWindow.PlayerActualState.TopRanks != null && MWindow.PlayerActualState.TopRanks.Length > 0)
                    MWindow.TopPP = MWindow.PlayerActualState.TopRanks[0].PP.ToString("#,#.#####", CultureInfo.InvariantCulture);

                int ppRankDif = 0, ppCountryRankDif = 0, aCountDif = 0, sCountDif = 0, shCountDif = 0, ssCountDif = 0, sshCountDif = 0, totalsCountDif = 0, totalssCountDif = 0, playTimeDif = 0, clearsDif = 0;
                float levelDif = 0, ppDif = 0, accuracyDif = 0, topPPDif = 0, hitsperplayDif = 0, tsperplayDif = 0, rsperplayDif = 0, playCountDif = 0, totalHitsDif = 0;
                long rankedScoreDif = 0, scoreDif = 0, scoreRankDif = 0;
                    if (scoremode == 0) // Each game mode
                    {
                        rankedScoreDif = MWindow.PlayerActualState.RankedScore - MWindow.PlayerPreviousState.RankedScore;
                        if ((MWindow.PlayerActualState.scoerinfo.ID != 0) && (MWindow.PlayerPreviousState.scoerinfo.ID != 0))
                        {
                            scoreRankDif = MWindow.PlayerActualState.scoerinfo.ScoreRank - MWindow.PlayerPreviousState.scoerinfo.ScoreRank;
                        }
                        levelDif = MWindow.PlayerActualState.Level - MWindow.PlayerPreviousState.Level;
                        scoreDif = MWindow.PlayerActualState.Score - MWindow.PlayerPreviousState.Score;
                        ppRankDif = MWindow.PlayerActualState.PPRank - MWindow.PlayerPreviousState.PPRank;
                        ppCountryRankDif = MWindow.PlayerActualState.PPCountryRank - MWindow.PlayerPreviousState.PPCountryRank;
                        ppDif = MWindow.PlayerActualState.PP - MWindow.PlayerPreviousState.PP;
                        accuracyDif = MWindow.PlayerActualState.Accuracy - MWindow.PlayerPreviousState.Accuracy;
                        playTimeDif = MWindow.PlayerActualState.PlayTime - MWindow.PlayerPreviousState.PlayTime;
                        playCountDif = MWindow.PlayerActualState.PlayCount - MWindow.PlayerPreviousState.PlayCount;
                        totalHitsDif = MWindow.PlayerActualState.Count300 + MWindow.PlayerActualState.Count100 + MWindow.PlayerActualState.Count50 - (MWindow.PlayerPreviousState.Count300 + MWindow.PlayerPreviousState.Count100 + MWindow.PlayerPreviousState.Count50);
                        hitsperplayDif = ((MWindow.PlayerActualState.Count300 + MWindow.PlayerActualState.Count100 + MWindow.PlayerActualState.Count50) / MWindow.PlayerActualState.PlayCount) - ((MWindow.PlayerPreviousState.Count300 + MWindow.PlayerPreviousState.Count100 + MWindow.PlayerPreviousState.Count50) / MWindow.PlayerPreviousState.PlayCount);
                        tsperplayDif = (MWindow.PlayerActualState.Score / MWindow.PlayerActualState.PlayCount) - (MWindow.PlayerPreviousState.Score / MWindow.PlayerPreviousState.PlayCount);
                        rsperplayDif = (MWindow.PlayerActualState.RankedScore / MWindow.PlayerActualState.PlayCount) - (MWindow.PlayerPreviousState.RankedScore / MWindow.PlayerPreviousState.PlayCount);
                        aCountDif = MWindow.PlayerActualState.RankA - MWindow.PlayerPreviousState.RankA;
                        sCountDif = MWindow.PlayerActualState.RankS - MWindow.PlayerPreviousState.RankS;
                        shCountDif = MWindow.PlayerActualState.RankSH - MWindow.PlayerPreviousState.RankSH;
                        ssCountDif = MWindow.PlayerActualState.RankSS - MWindow.PlayerPreviousState.RankSS;
                        sshCountDif = MWindow.PlayerActualState.RankSSH - MWindow.PlayerPreviousState.RankSSH;
                        totalsCountDif = MWindow.PlayerActualState.RankS + MWindow.PlayerActualState.RankSH - (MWindow.PlayerPreviousState.RankS + MWindow.PlayerPreviousState.RankSH);
                        totalssCountDif = MWindow.PlayerActualState.RankSS + MWindow.PlayerActualState.RankSSH - (MWindow.PlayerPreviousState.RankSS + MWindow.PlayerPreviousState.RankSSH);
                        if (MWindow.PlayerActualState.TopRanks != null && MWindow.PlayerActualState.TopRanks.Length > 0)
                            if (MWindow.PlayerPreviousState.TopRanks != null && MWindow.PlayerPreviousState.TopRanks.Length > 0)
                                topPPDif = MWindow.PlayerActualState.TopRanks[0].PP - MWindow.PlayerPreviousState.TopRanks[0].PP;
                            else
                                topPPDif = MWindow.PlayerActualState.TopRanks[0].PP;
                        int clears = MWindow.PlayerActualState.RankSS + MWindow.PlayerActualState.RankSSH + MWindow.PlayerActualState.RankS + MWindow.PlayerActualState.RankSH + MWindow.PlayerActualState.RankA;
                        int oldclears = MWindow.PlayerPreviousState.RankSS + MWindow.PlayerPreviousState.RankSSH + MWindow.PlayerPreviousState.RankS + MWindow.PlayerPreviousState.RankSH + MWindow.PlayerPreviousState.RankA;
                        clearsDif = clears - oldclears;
                    }
                    else if (scoremode == 1) // This session mode
                    {
                        rankedScoreDif = MWindow.PlayerActualState.RankedScore - MWindow.PlayerFirstState.RankedScore;
                        if ((MWindow.PlayerActualState.scoerinfo != null) && (MWindow.PlayerFirstState.scoerinfo != null)){
                            if ((MWindow.PlayerActualState.scoerinfo.ID != 0) && (MWindow.PlayerFirstState.scoerinfo.ID != 0))
                            {
                                scoreRankDif = MWindow.PlayerActualState.scoerinfo.ScoreRank - MWindow.PlayerFirstState.scoerinfo.ScoreRank;
                            }
                        }
                        levelDif = MWindow.PlayerActualState.Level - MWindow.PlayerFirstState.Level;
                        scoreDif = MWindow.PlayerActualState.Score - MWindow.PlayerFirstState.Score;
                        ppRankDif = MWindow.PlayerActualState.PPRank - MWindow.PlayerFirstState.PPRank;
                        ppCountryRankDif = MWindow.PlayerActualState.PPCountryRank - MWindow.PlayerFirstState.PPCountryRank;
                        ppDif = MWindow.PlayerActualState.PP - MWindow.PlayerFirstState.PP;
                        accuracyDif = MWindow.PlayerActualState.Accuracy - MWindow.PlayerFirstState.Accuracy;
                        playTimeDif = MWindow.PlayerActualState.PlayTime - MWindow.PlayerFirstState.PlayTime;
                        playCountDif = MWindow.PlayerActualState.PlayCount - MWindow.PlayerFirstState.PlayCount;
                        totalHitsDif = MWindow.PlayerActualState.Count300 + MWindow.PlayerActualState.Count100 + MWindow.PlayerActualState.Count50
                                       - (MWindow.PlayerFirstState.Count300 + MWindow.PlayerFirstState.Count100 + MWindow.PlayerFirstState.Count50);
                        hitsperplayDif = ((MWindow.PlayerActualState.Count300 + MWindow.PlayerActualState.Count100 + MWindow.PlayerActualState.Count50) / MWindow.PlayerActualState.PlayCount)
                                       - ((MWindow.PlayerFirstState.Count300 + MWindow.PlayerFirstState.Count100 + MWindow.PlayerFirstState.Count50) / MWindow.PlayerFirstState.PlayCount);
                        tsperplayDif = (MWindow.PlayerActualState.Score / MWindow.PlayerActualState.PlayCount) - (MWindow.PlayerFirstState.Score / MWindow.PlayerFirstState.PlayCount);
                        rsperplayDif = (MWindow.PlayerActualState.RankedScore / MWindow.PlayerActualState.PlayCount) - (MWindow.PlayerFirstState.RankedScore / MWindow.PlayerFirstState.PlayCount);
                        aCountDif = MWindow.PlayerActualState.RankA - MWindow.PlayerFirstState.RankA;
                        sCountDif = MWindow.PlayerActualState.RankS - MWindow.PlayerFirstState.RankS;
                        shCountDif = MWindow.PlayerActualState.RankSH - MWindow.PlayerFirstState.RankSH;
                        ssCountDif = MWindow.PlayerActualState.RankSS - MWindow.PlayerFirstState.RankSS;
                        sshCountDif = MWindow.PlayerActualState.RankSSH - MWindow.PlayerFirstState.RankSSH;
                        totalsCountDif = MWindow.PlayerActualState.RankS + MWindow.PlayerActualState.RankSH - (MWindow.PlayerFirstState.RankS + MWindow.PlayerFirstState.RankSH);
                        totalssCountDif = MWindow.PlayerActualState.RankSS + MWindow.PlayerActualState.RankSSH - (MWindow.PlayerFirstState.RankSS + MWindow.PlayerFirstState.RankSSH);
                        if (MWindow.PlayerActualState.TopRanks != null && MWindow.PlayerActualState.TopRanks.Length > 0)
                            if (MWindow.PlayerFirstState.TopRanks != null && MWindow.PlayerFirstState.TopRanks.Length > 0)
                                topPPDif = MWindow.PlayerActualState.TopRanks[0].PP - MWindow.PlayerFirstState.TopRanks[0].PP;
                            else
                                topPPDif = MWindow.PlayerActualState.TopRanks[0].PP;
                        int clears = MWindow.PlayerActualState.RankSS + MWindow.PlayerActualState.RankSSH + MWindow.PlayerActualState.RankS + MWindow.PlayerActualState.RankSH + MWindow.PlayerActualState.RankA;
                        int oldclears = MWindow.PlayerFirstState.RankSS + MWindow.PlayerFirstState.RankSSH + MWindow.PlayerFirstState.RankS + MWindow.PlayerFirstState.RankSH + MWindow.PlayerFirstState.RankA;
                        clearsDif = clears - oldclears;
                    }
                    MWindow.RankedScoreChange = rankedScoreDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MWindow.ScoreRankChange = scoreRankDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MWindow.LevelChange = levelDif.ToString("#,#0.####", CultureInfo.InvariantCulture);
                    MWindow.TotalScoreChange = scoreDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MWindow.RankChange = ppRankDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MWindow.CountryRankChange = ppCountryRankDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MWindow.PPChange = ppDif.ToString("#,#0.##", CultureInfo.InvariantCulture);
                    MWindow.AccuracyChange = accuracyDif.ToString("#,#0.#####", CultureInfo.InvariantCulture);
                    TimeSpan ptc = TimeSpan.FromSeconds(playTimeDif);
                    if (playTimeDif < 3600)
                    {
                        MWindow.PlayTimeChange = string.Format("{0}m {1}s", ptc.Minutes, ptc.Seconds);
                    }
                    else
                    {
                        MWindow.PlayTimeChange = string.Format("{0}h {1}m {2}s", ptc.Days * 24 + ptc.Hours, ptc.Minutes, ptc.Seconds);
                    }
                    MWindow.PlayCountChange = playCountDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MWindow.TotalHitsChange = totalHitsDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MWindow.HitsPerPlayChange = hitsperplayDif.ToString("#,#0.##", CultureInfo.InvariantCulture);
                    MWindow.TSPerPlayChange = tsperplayDif.ToString("#,#0.##", CultureInfo.InvariantCulture);
                    MWindow.RSPerPlayChange = rsperplayDif.ToString("#,#0.##", CultureInfo.InvariantCulture);
                    MWindow.TopPPChange = topPPDif.ToString("#,#0.#####", CultureInfo.InvariantCulture);
                    MWindow.RankAChange = aCountDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MWindow.RankSChange = sCountDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MWindow.RankSHChange = shCountDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MWindow.RankSSChange = ssCountDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MWindow.RankSSHChange = sshCountDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MWindow.TotalSChange = totalsCountDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MWindow.TotalSSChange = totalssCountDif.ToString("#,#", CultureInfo.InvariantCulture);
                    MWindow.ClearsChange = clearsDif.ToString("#,#", CultureInfo.InvariantCulture);


                    if (ppDif > 0)
                    {
                        MWindow.PPChange = "+" + MWindow.PPChange;
                        MWindow.PPChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.PPChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (scoreDif > 0)
                    {
                        MWindow.TotalScoreChange = "+" + MWindow.TotalScoreChange;
                        MWindow.TotalScoreChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.TotalScoreChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    ///overall ranked score milestone aesthetics.
                    ///Comment EVERYTHING in this section if you don't want to have colored *Current* Ranked Score display.
                    /*
                    if (MWindow.PlayerActualState.RankedScore > 1000000000000)
                    {
                        MWindow.rankedbox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8000ff"));
                    }
                    else if (MWindow.PlayerActualState.RankedScore > 700000000000)
                    {
                        MWindow.rankedbox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0026ff"));
                    }
                    else if (MWindow.PlayerActualState.RankedScore > 500000000000)
                    {
                        MWindow.rankedbox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00c8ff"));
                    }
                    else if (MWindow.PlayerActualState.RankedScore > 333333333333)
                    {
                        MWindow.rankedbox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00ff91"));
                    }
                    else if (MWindow.PlayerActualState.RankedScore > 200000000000)
                    {
                        MWindow.rankedbox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#11ff00"));
                    }
                    else if (MWindow.PlayerActualState.RankedScore > 100000000000)
                    {
                        MWindow.rankedbox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffff00"));
                    }
                    else if (MWindow.PlayerActualState.RankedScore > 50000000000)
                    {
                        MWindow.rankedbox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffa600"));
                    }
                    else if (MWindow.PlayerActualState.RankedScore > 25000000000)
                    {
                        MWindow.rankedbox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff0000"));
                    }
                    else if (MWindow.PlayerActualState.RankedScore > 0)
                    {
                        MWindow.rankedbox.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        MWindow.RankedScoreChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    */


                    ///PP rank "#" symbol prepended to the rank value.
                    ///Comment this out if you don't want this.
                    ///Don't forget to also comment this out in the SetValue function.

                    if (MWindow.PlayerActualState.PPRank != 0) {
                        MWindow.Rank = "#" + MWindow.Rank;
                    }

                    if (MWindow.PlayerActualState.PPCountryRank != 0)
                    {
                        MWindow.CountryRank = "#" + MWindow.CountryRank;
                    }



                    ///Score Rank with hardcoded color aesthetics. 
                    ///If you want to see "#" prepended to your score rank. Uncomment all the first lines within the "if" statements.
                    ///If you don't want colors, keep this section commented out.
                    /*
                    if (mode != 0)
                    {
                        MWindow.ScoreRank = "Score Rank Unavailable";
                    }
                    else if (MWindow.PlayerActualState.scoerinfo == null) {
                        MWindow.ScoreRank = "No Score Rank";
                        MWindow.ScoreRankBox.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else if (MWindow.PlayerActualState.scoerinfo.ScoreRank <= 0) {
                        MWindow.ScoreRankBox.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else if (MWindow.PlayerActualState.scoerinfo.ScoreRank == 1)
                    {
                        MWindow.ScoreRank = "#" + MWindow.ScoreRank;
                        MWindow.ScoreRankBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2e5579"));
                    }
                    else if (MWindow.PlayerActualState.scoerinfo.ScoreRank <= 10)
                    {
                        MWindow.ScoreRank = "#" + MWindow.ScoreRank;
                        MWindow.ScoreRankBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#776b27"));
                    }
                    else if (MWindow.PlayerActualState.scoerinfo.ScoreRank <= 100)
                    {
                        MWindow.ScoreRank = "#" + MWindow.ScoreRank;
                        MWindow.ScoreRankBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#839abf"));
                    }
                    else if (MWindow.PlayerActualState.scoerinfo.ScoreRank <= 1000)
                    {
                        MWindow.ScoreRank = "#" + MWindow.ScoreRank;
                        MWindow.ScoreRankBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7c4b1a"));
                    }
                    else 
                    {
                        MWindow.ScoreRank = "#" + MWindow.ScoreRank;
                        MWindow.ScoreRankBox.Foreground = new SolidColorBrush(Colors.White);
                    }
                    */


                    ///Non-Aesthetic version. 
                    ///If you are going to use the version with colors, comment this section out.
                    ///Otherwise keep the previous section commented out. :)
                    ///If you don't want "#" prepended to your rank don't forget to also comment it out in SetValue() function along with the line in the "else" statement below!
                    if (MWindow.PlayerActualState.scoerinfo != null) {
                        if (MainWindow.MWindow.PlayerActualState.scoerinfo.ID == 0)
                        {
                            MWindow.ScoreRank = "No Score Rank";
                        }
                        else
                        {
                            MWindow.ScoreRank = "#" + MainWindow.MWindow.ScoreRank;
                        }
                    }
                    



                    ///Session ranked score color aesthetics.
                    ///Uncomment everything above the ">0" condition for colors.
                    /*
                    if (rankedScoreDif >= 500000000)
                    {
                        MWindow.RankedScoreChange = "+" + MWindow.RankedScoreChange;
                        MWindow.RankedScoreChangeBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2e5579"));
                    }
                    else if (rankedScoreDif >= 300000000)
                    {
                        MWindow.RankedScoreChange = "+" + MWindow.RankedScoreChange;
                        MWindow.RankedScoreChangeBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#776b27"));
                    }
                    else if (rankedScoreDif >= 200000000)
                    {
                        MWindow.RankedScoreChange = "+" + MWindow.RankedScoreChange;
                        MWindow.RankedScoreChangeBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#839abf"));
                    }
                    else if (rankedScoreDif >= 100000000)
                    {
                        MWindow.RankedScoreChange = "+" + MWindow.RankedScoreChange;
                        MWindow.RankedScoreChangeBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7c4b1a"));
                    }
                    else
                    */
                    if (rankedScoreDif > 0)
                    {
                        MWindow.RankedScoreChange = "+" + MWindow.RankedScoreChange;
                        MWindow.RankedScoreChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.RankedScoreChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }


                    if (levelDif > 0)
                    {
                        MWindow.LevelChange = "+" + MWindow.LevelChange;
                        MWindow.LevelChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.LevelChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (ppRankDif > 0)
                    {
                        MWindow.RankChange = (-ppRankDif).ToString("#,#", CultureInfo.InvariantCulture);
                        MWindow.RankChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else if (ppRankDif != 0)
                    {
                        MWindow.RankChange = "+" + (-ppRankDif).ToString("#,#", CultureInfo.InvariantCulture);
                        MWindow.RankChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }

                    if (scoreRankDif > 0)
                    {
                        MWindow.ScoreRankChange = (-scoreRankDif).ToString("#,#", CultureInfo.InvariantCulture);
                        MWindow.ScoreRankChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else if (scoreRankDif != 0)
                    {
                        MWindow.ScoreRankChange = "+" + (-scoreRankDif).ToString("#,#", CultureInfo.InvariantCulture);
                        MWindow.ScoreRankChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }

                    if (ppCountryRankDif > 0)
                    {
                        MWindow.CountryRankChange = (-ppCountryRankDif).ToString("#,#", CultureInfo.InvariantCulture);
                        MWindow.CountryRankChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else if (ppCountryRankDif != 0)
                    {
                        MWindow.CountryRankChange = "+" + (-ppCountryRankDif).ToString("#,#", CultureInfo.InvariantCulture);
                        MWindow.CountryRankChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }

                    if (accuracyDif > 0)
                    {
                        MWindow.AccuracyChange = "+" + MWindow.AccuracyChange;
                        MWindow.AccuracyChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.AccuracyChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (playTimeDif > 0)
                    {
                        MWindow.PlayTimeChange = "+" + MWindow.PlayTimeChange;
                        MWindow.PlayTimeChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.PlayTimeChange = "";
                        MWindow.PlayTimeChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (playCountDif > 0)
                    {
                        MWindow.PlayCountChange = "+" + MWindow.PlayCountChange;
                        MWindow.PlayCountChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.PlayCountChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (totalHitsDif > 0)
                    {
                        MWindow.TotalHitsChange = "+" + MWindow.TotalHitsChange;
                        MWindow.TotalHitsChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.TotalHitsChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if ((hitsperplayDif >= 0.001) && (MWindow.HitsPerPlayChange != ""))
                    {
                        MWindow.HitsPerPlayChange = "+" + MWindow.HitsPerPlayChange;
                        MWindow.HitsPerPlayChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.HitsPerPlayChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if ((tsperplayDif >= 0.001) && (MWindow.TSPerPlayChange != ""))
                    {
                        MWindow.TSPerPlayChange = "+" + MWindow.TSPerPlayChange;
                        MWindow.TSPerPlayChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.TSPerPlayChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if ((rsperplayDif >= 0.001) && (MWindow.RSPerPlayChange != ""))
                    {
                        MWindow.RSPerPlayChange = "+" + MWindow.RSPerPlayChange;
                        MWindow.RSPerPlayChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.RSPerPlayChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (topPPDif > 0)
                    {
                        MWindow.TopPPChange = "+" + MWindow.TopPPChange;
                        MWindow.TopPPChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.TopPPChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (aCountDif > 0)
                    {
                        MWindow.RankAChange = "+" + MWindow.RankAChange;
                        MWindow.RankAChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.RankAChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (sCountDif > 0)
                    {
                        MWindow.RankSChange = "+" + MWindow.RankSChange;
                        MWindow.RankSChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.RankSChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (shCountDif > 0)
                    {
                        MWindow.RankSHChange = "+" + MWindow.RankSHChange;
                        MWindow.RankSHChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.RankSHChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (ssCountDif > 0)
                    {
                        MWindow.RankSSChange = "+" + MWindow.RankSSChange;
                        MWindow.RankSSChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.RankSSChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (sshCountDif > 0)
                    {
                        MWindow.RankSSHChange = "+" + MWindow.RankSSHChange;
                        MWindow.RankSSHChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.RankSSHChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (totalsCountDif > 0)
                    {
                        MWindow.TotalSChange = "+" + MWindow.TotalSChange;
                        MWindow.TotalSChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.TotalSChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    if (totalssCountDif > 0)
                    {
                        MWindow.TotalSSChange = "+" + MWindow.TotalSSChange;
                        MWindow.TotalSSChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.TotalSSChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                    ///Session clear aesthetics.
                    ///Uncomment everything here above the "if (clearsDif >0)" line to enable colors.
                    /*
                    if (clearsDif > 149)
                    {
                        MWindow.ClearsChange = "+" + MWindow.ClearsChange;
                        MWindow.ClearsChangeBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2e5579"));
                    }
                    else if (clearsDif > 99)
                    {
                        MWindow.ClearsChange = "+" + MWindow.ClearsChange;
                        MWindow.ClearsChangeBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#776b27"));
                    }
                    else if (clearsDif > 49)
                    {
                        MWindow.ClearsChange = "+" + MWindow.ClearsChange;
                        MWindow.ClearsChangeBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#839abf"));
                    }
                    else if (clearsDif > 32)
                    {
                        MWindow.ClearsChange = "+" + MWindow.ClearsChange;
                        MWindow.ClearsChangeBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7c4b1a"));
                    }
                    else
                    */
                    if (clearsDif > 0)
                    {
                        MWindow.ClearsChange = "+" + MWindow.ClearsChange;
                        MWindow.ClearsChangeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MWindow.ClearsChangeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }

                }));
            }
            else
            {
                MWindow.Ranked = "";
                MWindow.Level = "";
                MWindow.Total = "";
                MWindow.Rank = "";
                MWindow.ScoreRank = "";
                MWindow.CountryRank = "";
                MWindow.PP = "";
                MWindow.Accuracy = "";
                MWindow.PlayTime = "";
                MWindow.PlayCount = "";
                MWindow.TotalHits = "";
                MWindow.HitsPerPlay = "";
                MWindow.TSPerPlay = "";
                MWindow.RSPerPlay = "";
                MWindow.TopPP = "";
                MWindow.RankA = "";
                MWindow.RankS = "";
                MWindow.RankSH = "";
                MWindow.RankSS = "";
                MWindow.RankSSH = "";
                MWindow.TotalS = "";
                MWindow.TotalSS = "";
                MWindow.Clears = "";
                MWindow.RankedScoreChange = "";
                MWindow.LevelChange = "";
                MWindow.TotalScoreChange = "";
                MWindow.RankChange = "";
                MWindow.ScoreRankChange = "";
                MWindow.CountryRankChange = "";
                MWindow.PPChange = "";
                MWindow.AccuracyChange = "";
                MWindow.PlayTimeChange = "";
                MWindow.PlayCountChange = "";
                MWindow.TotalHitsChange = "";
                MWindow.HitsPerPlayChange = "";
                MWindow.TSPerPlayChange = "";
                MWindow.RSPerPlayChange = "";
                MWindow.TopPPChange = "";
                MWindow.RankAChange = "";
                MWindow.RankSChange = "";
                MWindow.RankSHChange = "";
                MWindow.RankSSChange = "";
                MWindow.RankSSHChange = "";
                MWindow.TotalSChange = "";
                MWindow.TotalSSChange = "";
                MWindow.ClearsChange = "";
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

                    if (config.GetValue("User", "beatmaps", "false") == "true")
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
                    for (int i = 0; i < files.Count; i++)
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
                short retry = 0;
                Player tempState = null;
                bool downloaded = false;
                while (!downloaded)
                {
                    try
                    {
                        WebClient client = new WebClient();
                        string apiReturn = client.DownloadString("https://osu.ppy.sh/api/get_user?k=" + APIKey + "&u=" + Username + "&m=" + mode);
                        apiReturn = apiReturn.Substring(1, apiReturn.Length - 2);
                        //long score = MainWindow.MWindow.PlayerActualState.Score;
                        tempState = JsonConvert.DeserializeObject<Player>(apiReturn);
                        downloaded = true;
                    }
                    catch (Exception) { downloaded = false; Thread.Sleep(new TimeSpan(0, 0, 1)); }
                }
                Scoerapi tempScoerState = new Scoerapi { ScoreRank = 0, ID = 0, SCOER = 0, Scoer_username = "None" };
                Scoerapi PrevScoerState = new Scoerapi { ScoreRank = 0, ID = 0, SCOER = 0, Scoer_username = "None" };
                //ScoerChange variable shows if a change in Score Rank has happened or not.
                bool ScoerChange = false;
                if (MWindow.PlayerActualState.scoerinfo != null)
                {
                    if (MWindow.PlayerActualState.scoerinfo.ID != 0)
                    {
                        PrevScoerState = MWindow.PlayerActualState.scoerinfo;
                    }
                }
                if ((MWindow.PlayerFirstState.Mode == tempState.Mode) && (tempState.ID == MWindow.PlayerFirstState.ID) && (scoremode == 1))
                {
                    WebClient client = new WebClient();
                    userID = tempState.ID;
                    retry = 0;
                    downloaded = false;
                    while (!downloaded && retry < 5)
                        try
                        {
                            string scoerapiReturn = client.DownloadString("https://score.respektive.pw/u/" + userID + "?m=" + tempState.Mode);
                            scoerapiReturn = scoerapiReturn.Substring(1, scoerapiReturn.Length - 2);
                            tempScoerState = JsonConvert.DeserializeObject<Scoerapi>(scoerapiReturn);
                            if (tempScoerState.ID != 0)
                            {
                                //if your enter top 10000, make starting score rank 10001.
                                if ((MWindow.PlayerFirstState.scoerinfo.ID == 0) && (tempScoerState.ID != 0))
                                {
                                    MWindow.PlayerFirstState.scoerinfo = MWindow.PlayerActualState.scoerinfo = MWindow.PlayerPreviousState.scoerinfo = tempScoerState;
                                    MWindow.PlayerFirstState.scoerinfo.ScoreRank = 10001;
                                    PrevScoerState = tempScoerState;
                                    PrevScoerState.ScoreRank = 10001;
                                    MWindow.PlayerActualState.scoerinfo.ScoreRank = tempScoerState.ScoreRank;
                                    ScoerChange = true;
                                }
                                else
                                {
                                    MWindow.PlayerActualState.scoerinfo = tempScoerState;
                                    ScoerChange = true;
                                }
                            }
                            downloaded = true;
                        }
                        catch (Exception) { retry++; downloaded = false; Thread.Sleep(new TimeSpan(0, 0, 1)); }
                    if (!downloaded) { tempScoerState = MWindow.PlayerActualState.scoerinfo; }
                }
                tempState.Mode = mode;
                if ((tempState.Mode != MWindow.PlayerFirstState.Mode) || (tempState.ID != MWindow.PlayerFirstState.ID))
                {
                    WebClient client = new WebClient();
                    MWindow.PrevStatState = MWindow.PlayerPreviousState = MWindow.PlayerFirstState = MWindow.PlayerActualState = tempState;
                    if (tempState.PP > 0)
                    {
                        downloaded = false;
                        while (!downloaded)
                        {
                            try
                            {
                                MWindow.PlayerActualState.TopRanks = JsonConvert.DeserializeObject<Score[]>(client.DownloadString("https://osu.ppy.sh/api/get_user_best?k=" + APIKey + "&u=" + Username + "&m=" + mode + "&limit=" + 1));
                                downloaded = true;
                            }
                            catch (Exception) { downloaded = false; Thread.Sleep(new TimeSpan(0, 0, 1)); }
                        }
                        MWindow.PlayerFirstState.TopRanks = MWindow.PlayerPreviousState.TopRanks = MWindow.PlayerActualState.TopRanks;
                    }
                    userID = tempState.ID;
                    retry = 0;
                    downloaded = false;
                    while (!downloaded && retry <= 5)
                    {
                        try
                        {
                            string scoerapiReturn = client.DownloadString("https://score.respektive.pw/u/" + userID + "?m=" + tempState.Mode);
                            scoerapiReturn = scoerapiReturn.Substring(1, scoerapiReturn.Length - 2);
                            tempScoerState = JsonConvert.DeserializeObject<Scoerapi>(scoerapiReturn);
                            PrevScoerState = tempScoerState;
                            MWindow.PlayerPreviousState.scoerinfo = MWindow.PrevStatState.scoerinfo = MWindow.PlayerFirstState.scoerinfo = MWindow.PlayerActualState.scoerinfo = tempScoerState;
                            downloaded = true;
                        }
                        catch (Exception) { downloaded = false; retry++; Thread.Sleep(new TimeSpan(0, 0, 1)); }
                    }
                    if (!downloaded) {
                        MWindow.PlayerActualState.scoerinfo = new Scoerapi { ID = 0, SCOER = 0, Scoer_username = "None", ScoreRank = 0 };
                        MWindow.PlayerPreviousState.scoerinfo = MWindow.PrevStatState.scoerinfo = MWindow.PlayerFirstState.scoerinfo = tempScoerState = MWindow.PlayerActualState.scoerinfo;
                    }
                }
                if (((tempState.Score != MWindow.PlayerActualState.Score) && (tempState.Mode == MWindow.PlayerActualState.Mode)) || ((ScoerChange == true) && (scoremode == 1)) || (scoremodeOld != scoremode))
                {
                    if (MWindow.PrevStatState == null)
                    {
                        MWindow.PrevStatState = MWindow.PlayerFirstState;
                        if (MWindow.PlayerFirstState.scoerinfo != null)
                        {
                            if (MWindow.PlayerFirstState.scoerinfo.ID != 0)
                            {
                                MWindow.PrevStatState.scoerinfo = MWindow.PlayerFirstState.scoerinfo;
                            }
                        }
                        MWindow.PrevStatState.TopRanks = MWindow.PlayerFirstState.TopRanks;
                    }
                    if ((tempState.Score == MWindow.PlayerActualState.Score))
                    {
                        if (tempState.Score != MWindow.PlayerPreviousState.Score)
                        {
                            MWindow.PrevStatState = MWindow.PlayerPreviousState;
                            MWindow.PrevStatState.TopRanks = MWindow.PlayerPreviousState.TopRanks;
                            if (MWindow.PlayerPreviousState.scoerinfo != null)
                            {
                                if (MWindow.PlayerPreviousState.scoerinfo.ID != 0)
                                {
                                    MWindow.PrevStatState.scoerinfo = MWindow.PlayerPreviousState.scoerinfo;
                                }
                            }
                        }
                    }
                    else
                    {
                        MWindow.PrevStatState = MWindow.PlayerActualState;
                        MWindow.PrevStatState.TopRanks = MWindow.PlayerActualState.TopRanks;
                        if (MWindow.PlayerActualState.scoerinfo != null)
                        {
                            if (MWindow.PlayerActualState.scoerinfo.ID != 0)
                            {
                                MWindow.PrevStatState.scoerinfo = MWindow.PlayerActualState.scoerinfo;
                            }
                        }
                    }
                    MWindow.PlayerPreviousState = MWindow.PlayerActualState;
                    MWindow.PlayerActualState = tempState;
                    if ((tempScoerState.ID != 0) || (scoremode == 0))
                    {
                        if (scoremode == 0)
                        {
                            WebClient client = new WebClient();
                            userID = tempState.ID;
                            retry = 0;
                            downloaded = false;
                            while (!downloaded && retry <= 3)
                            {
                                try
                                {
                                    string scoerapiReturn = client.DownloadString("https://score.respektive.pw/u/" + userID + "?m=" + tempState.Mode);
                                    scoerapiReturn = scoerapiReturn.Substring(1, scoerapiReturn.Length - 2);
                                    tempScoerState = JsonConvert.DeserializeObject<Scoerapi>(scoerapiReturn);
                                    downloaded = true;
                                }
                                catch (Exception) { downloaded = false; retry++; Thread.Sleep(new TimeSpan(0, 0, 1)); }
                            }
                            if (!downloaded) { tempScoerState = MWindow.PlayerActualState.scoerinfo; }
                            if ((tempScoerState.ID != 0) && (tempScoerState != PrevScoerState))
                            {
                                MWindow.PlayerPreviousState = MWindow.PrevStatState;
                                //if your enter top 10000, make starting score rank 10001.
                                if ((MWindow.PlayerFirstState.scoerinfo.ID == 0) && (tempScoerState.ID != 0))
                                {
                                    MWindow.PlayerFirstState.scoerinfo = MWindow.PlayerActualState.scoerinfo = MWindow.PlayerPreviousState.scoerinfo = tempScoerState;
                                    MWindow.PlayerFirstState.scoerinfo.ScoreRank = 10001;
                                    PrevScoerState = tempScoerState;
                                    PrevScoerState.ScoreRank = 10001;
                                    MWindow.PlayerActualState.scoerinfo.ScoreRank = tempScoerState.ScoreRank;
                                }
                                else
                                {
                                    MWindow.PlayerPreviousState.scoerinfo = MWindow.PrevStatState.scoerinfo;
                                    MWindow.PlayerActualState.scoerinfo = tempScoerState;
                                }
                            }
                        }
                        else
                        {
                            MWindow.PlayerPreviousState.scoerinfo = PrevScoerState;
                            MWindow.PlayerActualState.scoerinfo = tempScoerState;
                        }
                    }
                    if ((scoremodeOld != scoremode) && (MWindow.PrevStatState != null))
                    {
                        scoremodeOld = scoremode;
                        MWindow.PlayerPreviousState = MWindow.PrevStatState;
                        if (MWindow.PrevStatState.TopRanks != null)
                        {
                            MWindow.PlayerPreviousState.TopRanks = MWindow.PrevStatState.TopRanks;
                        }
                        if (MWindow.PrevStatState.scoerinfo.ID != 0)
                        {
                            MWindow.PlayerPreviousState.scoerinfo = MWindow.PrevStatState.scoerinfo;
                        }
                    }
                    if (MWindow.PlayerPreviousState.PP < MWindow.PlayerActualState.PP)
                    {
                        downloaded = false;
                        while (!downloaded)
                        {
                            try
                            {
                                WebClient client = new WebClient();
                                MWindow.PrevStatState.TopRanks = MWindow.PlayerActualState.TopRanks;
                                MWindow.PlayerActualState.TopRanks = JsonConvert.DeserializeObject<Score[]>(client.DownloadString("https://osu.ppy.sh/api/get_user_best?k=" + APIKey + "&u=" + Username + "&m=" + mode + "&limit=" + 1));
                                downloaded = true;
                            }
                            catch (Exception) { downloaded = false; Thread.Sleep(new TimeSpan(0, 0, 1)); }
                        }
                    }

                    for (int i = 0; i < files.Count; i++)
                    {
                        MainWindow.files[i].TimeLeft = MainWindow.files[i].Time;
                    }

                    if (config.GetValue("User", "popupEachMap", "false") == "true" && MWindow.PlayerPreviousState.RankedScore != MWindow.PlayerActualState.RankedScore)
                    {
                        MWindow.RankedScoreChangeBox.Dispatcher.Invoke(new Action(() =>
                        {
                            MWindow.Activate();
                            MWindow.Focus();
                        }));
                    }
                    else if (config.GetValue("User", "popupPP", "false") == "true" && MWindow.PlayerPreviousState.PP < MWindow.PlayerActualState.PP)
                    {
                        MWindow.RankedScoreChangeBox.Dispatcher.Invoke(new Action(() =>
                        {
                            MWindow.Activate();
                            MWindow.Focus();
                        }));
                    }
                }
                MWindow.UpdateRankingControls();
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
                        string apiReturn = client.DownloadString("https://osu.ppy.sh/api/get_user_recent?k=" + APIKey + "&u=" + Username + "&m=" + mode);
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

                MWindow.PlayBox.Dispatcher.Invoke(new Action(() =>
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (Event ev in events)
                    {
                        sb.AppendLine(ev.Beatmap.Artist + " - " + ev.Beatmap.Title + " [" + ev.Beatmap.Difficulty + "]");
                        sb.AppendLine("BPM : " + ev.Beatmap.BPM + " - Stars : " + ev.Beatmap.Stars);
                        sb.AppendLine("CS:" + ev.Beatmap.CircleSize + " AR: " + ev.Beatmap.ApproachRate + " OD:" + ev.Beatmap.OverallDifficulty + " HP:" + ev.Beatmap.HealthDrain);
                        sb.AppendLine("Score : " + ev.Score.ToString("#,#", CultureInfo.InvariantCulture) + " " + ev.ModsString);
                        sb.Append("Rank : " + ev.Grade);
                        if (MWindow.PlayerActualState.PP != MWindow.PlayerPreviousState.PP)
                        {
                            sb.AppendLine("PP : " + (MWindow.PlayerActualState.PP - MWindow.PlayerPreviousState.PP));
                        }
                        sb.AppendLine("\n");
                    }

                    MWindow.PlayBox.Text = sb.ToString();
                }));
            }

            private void ExportToFile()
            {
                for (int i = 0; i < files.Count; i++)
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
                            MWindow.RankedScoreChangeBox.Dispatcher.Invoke(new Action(() =>
                            {
                                output = output.Replace("[/rankedscore]", MWindow.Ranked);
                                output = output.Replace("[/totalscore]", MWindow.Total);
                                output = output.Replace("[/lvl]", MWindow.Level);
                                output = output.Replace("[/rank]", MWindow.Rank);
                                output = output.Replace("[/scorerank]", MWindow.ScoreRank);
                                output = output.Replace("[/countryrank]", MWindow.CountryRank);
                                output = output.Replace("[/pp]", MWindow.PP);
                                output = output.Replace("[/acc]", MWindow.Accuracy);
                                output = output.Replace("[/playtime]", MWindow.PlayTime);
                                output = output.Replace("[/playcount]", MWindow.PlayCount);
                                output = output.Replace("[/totalhits]", MWindow.TotalHits);
                                output = output.Replace("[/hitsperplay]", MWindow.HitsPerPlay);
                                output = output.Replace("[/tsperplay]", MWindow.TSPerPlay);
                                output = output.Replace("[/rsperplay]", MWindow.RSPerPlay);
                                output = output.Replace("[/toppp]", MWindow.TopPP);
                                output = output.Replace("[/arank]", MWindow.RankA);
                                output = output.Replace("[/srank]", MWindow.RankS);
                                output = output.Replace("[/shrank]", MWindow.RankSH);
                                output = output.Replace("[/ssrank]", MWindow.RankSS);
                                output = output.Replace("[/sshrank]", MWindow.RankSSH);
                                output = output.Replace("[/totals]", MWindow.TotalS);
                                output = output.Replace("[/totalss]", MWindow.TotalSS);
                                output = output.Replace("[/clears]", MWindow.Clears);

                                output = output.Replace("[/rankedscorechange]", MWindow.RankedScoreChange);
                                output = output.Replace("[/totalscorechange]", MWindow.TotalScoreChange);
                                output = output.Replace("[/lvlchange]", MWindow.LevelChange);
                                output = output.Replace("[/rankchange]", MWindow.RankChange);
                                output = output.Replace("[/scorerankchange]", MWindow.ScoreRankChange);
                                output = output.Replace("[/countryrankchange]", MWindow.CountryRankChange);
                                output = output.Replace("[/ppchange]", MWindow.PPChange);
                                output = output.Replace("[/accchange]", MWindow.AccuracyChange);
                                output = output.Replace("[/playtimechange]", MWindow.PlayTimeChange);
                                output = output.Replace("[/playcountchange]", MWindow.PlayCountChange);
                                output = output.Replace("[/totalhitschange]", MWindow.TotalHitsChange);
                                output = output.Replace("[/hitsperplaychange]", MWindow.HitsPerPlayChange);
                                output = output.Replace("[/tsperplaychange]", MWindow.TSPerPlayChange);
                                output = output.Replace("[/rsperplaychange]", MWindow.RSPerPlayChange);
                                output = output.Replace("[/topppchange]", MWindow.TopPPChange);
                                output = output.Replace("[/arankchange]", MWindow.RankAChange);
                                output = output.Replace("[/srankchange]", MWindow.RankSChange);
                                output = output.Replace("[/shrankchange]", MWindow.RankSHChange);
                                output = output.Replace("[/ssrankchange]", MWindow.RankSSChange);
                                output = output.Replace("[/sshrankchange]", MWindow.RankSSHChange);
                                output = output.Replace("[/totalschange]", MWindow.TotalSChange);
                                output = output.Replace("[/totalsschange]", MWindow.TotalSSChange);
                                output = output.Replace("[/clearschange]", MWindow.ClearsChange);

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

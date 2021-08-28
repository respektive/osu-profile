using MahApps.Metro.Controls;
using osu_Profile.IO;
using System;
using System.Windows;
using System.Windows.Controls;

namespace osu_Profile.Forms
{
    /// <summary>
    /// Logique d'interaction pour FilesWindow.xaml
    /// </summary>
    public partial class FilesWindow : MetroWindow
    {
        #region Attributes
        public string file = "";
        public string content = "";
        public int number = -1;
        public int time = 0;
        public ListBox list;
        #endregion

        #region Constructor
        public FilesWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Property
        public string TimeToWait
        {
            set
            {
                if (!int.TryParse(value, out time))
                {
                    time = 0;
                }
                if (time < 0)
                    time = 0;
                txtNum.Text = time.ToString();
                if (MainWindow.MWindow.loopupdate != null)
                    MainWindow.MWindow.loopupdate.setTimer(time);
            }
            get
            {
                return txtNum.Text;
            }
        }
        #endregion

        #region Method
        public void setlist(ref ListBox box)
        {
            list = box;
        }
        #endregion

        #region Handlers
        private void window1_Loaded(object sender, RoutedEventArgs e)
        {
            this.Owner.IsEnabled = false;
            filebox.Text = file;
            contentbox.Text = content;
            TimeToWait = time.ToString();

            contentbox.ToolTip = "[/rankedscore] for ranked score" + Environment.NewLine
                + "[/totalscore] for total score" + Environment.NewLine
                + "[/lvl] for level" + Environment.NewLine
                + "[/rank] for performance rank" + Environment.NewLine
                + "[/countryrank] for country rank" + Environment.NewLine
                + "[/pp] for PP" + Environment.NewLine
                + "[/acc] for accuracy" + Environment.NewLine
                + "[/playtime] for play time" + Environment.NewLine
                + "[/playcount] for play count" + Environment.NewLine
                + "[/totalhits] for total hits" + Environment.NewLine
                + "[/hitsperplay] for hits per play" + Environment.NewLine
                + "[/toppp] for the top PP" + Environment.NewLine 
                + "[/arank] for A ranks" + Environment.NewLine
                + "[/srank] for S ranks" + Environment.NewLine
                + "[/shrank] for SH ranks" + Environment.NewLine
                + "[/ssrank] for SS ranks" + Environment.NewLine
                + "[/sshrank] for SSH ranks" + Environment.NewLine
                + "[/totals] for total S ranks" + Environment.NewLine
                + "[/totalss] for total SSranks" + Environment.NewLine
                + "[/clears] for total clears (With loved)" + Environment.NewLine
                + "[/scorerank] for Score Rank (STD only)"
                + Environment.NewLine + Environment.NewLine

                + "[/rankedscorechange] for ranked score difference" + Environment.NewLine
                + "[/totalscorechange] for total score difference" + Environment.NewLine
                + "[/lvlchange] for level difference" + Environment.NewLine
                + "[/rankchange] for performance rank difference" + Environment.NewLine
                + "[/countryrankchange] for country rank difference" + Environment.NewLine
                + "[/ppchange] for PP difference" + Environment.NewLine
                + "[/accchange] for accuracy difference" + Environment.NewLine
                + "[/playtimechange] for play time difference" + Environment.NewLine
                + "[/playcountchange] for play count difference" + Environment.NewLine
                + "[/totalhitschange] for total hits difference" + Environment.NewLine
                + "[/hitsperplaychange] for hits per play difference" + Environment.NewLine
                + "[/topppchange] for the top PP difference" + Environment.NewLine 
                + "[/arankchange] for A rank difference" + Environment.NewLine
                + "[/srankchange] for S rank difference" + Environment.NewLine
                + "[/shrankchange] for SH rank difference" + Environment.NewLine
                + "[/ssrankchange] for SS rank difference" + Environment.NewLine
                + "[/sshrankchange] for SSH rank difference" + Environment.NewLine
                + "[/totalschange] for total S rank difference" + Environment.NewLine
                + "[/totalsschange] for total SS rank difference" + Environment.NewLine
                + "[/clearschange] for total clears difference" + Environment.NewLine
                + "[/scorerankchange] for Score Rank Change (STD only)"
                + Environment.NewLine + Environment.NewLine

                + "[/lpbArtist] for the last played beatmap's artist" + Environment.NewLine
                + "[/lpbTitle] for the last played beatmap's title" + Environment.NewLine
                + "[/lpbBPM] for the last played beatmap's BPM" + Environment.NewLine
                + "[/lpbCreator] for the last played beatmap's creator" + Environment.NewLine
                + "[/lpbDifficulty] for the last played beatmap's difficulty name" + Environment.NewLine
                + "[/lpbID] for the last played beatmap's ID" + Environment.NewLine
                + "[/lpbSetID] for the last played beatmap's set ID" + Environment.NewLine

                + "[/lpbAR] for the last played beatmap's approach rate" + Environment.NewLine
                + "[/lpbCS] for the last played beatmap's circle size rate" + Environment.NewLine
                + "[/lpbHP] for the last played beatmap's health drain rate" + Environment.NewLine
                + "[/lpbOD] for the last played beatmap's overrall difficulty rate" + Environment.NewLine
                + "[/lpbStars] for the last played beatmap's stars number" + Environment.NewLine

                + "[/lpbGrade] for the last played beatmap's grade" + Environment.NewLine
                + "[/lpbMods] for the last played beatmap's mods enabled" + Environment.NewLine
                + "[/lpbScore] for the last played beatmap's mods score";

            txtNum.ToolTip = "The time in seconds to show something on the output after a change. (0 = unlimited)";
        }

        private void cmdUp_Click(object sender, RoutedEventArgs e) => TimeToWait = (time + 1).ToString();

        private void cmdDown_Click(object sender, RoutedEventArgs e) => TimeToWait = (time - 1).ToString();

        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow.MWindow != null)
                TimeToWait = txtNum.Text;
        }
        
        private void valid_Click(object sender, RoutedEventArgs e)
        {
            if (number == -1)
            {
                if (MainWindow.ContainsFilename(filebox.Text))
                {
                    MessageBox.Show("File already exists!");
                    return;
                }
                OutputFile outputFile = new OutputFile(filebox.Text, contentbox.Text, time);
                MainWindow.files.Add(outputFile);
                number = MainWindow.IndexOfFilename(filebox.Text);
                MainWindow.config.SetValue("Files", "filename" + number, filebox.Text.ToLower());
                MainWindow.config.SetValue("Files", "filecontent" + number, contentbox.Text.Replace(Environment.NewLine, "\\n"));
                MainWindow.config.SetValue("Files", "filetime" + number, time.ToString());
                MainWindow.config.SetValue("User", "files", MainWindow.files.Count.ToString());
            }
            else
            {
                if (MainWindow.ContainsFilename(filebox.Text) && file.ToLower() != filebox.Text.ToLower())
                {
                    MessageBox.Show("File already exists!");
                    return;
                }
                MainWindow.files[number].Name = filebox.Text.ToLower();
                MainWindow.files[number].Content = contentbox.Text;
                MainWindow.files[number].Time = time;
                MainWindow.config.SetValue("Files", "filename" + number, filebox.Text.ToLower());
                MainWindow.config.SetValue("Files", "filecontent" + number, contentbox.Text.Replace(Environment.NewLine, "\\n"));
                MainWindow.config.SetValue("Files", "filetime" + number, time.ToString());
                MainWindow.config.SetValue("User", "files", MainWindow.files.Count.ToString());
            }
            this.Close();
        }

        private void cancel_Click(object sender, RoutedEventArgs e) => this.Close();

        private void window1_Closed(object sender, EventArgs e)
        {
            this.Owner.IsEnabled = true;
            list.Items.Clear();
            for (int i = 0; i < MainWindow.files.Count; i++)
            {
                list.Items.Add(MainWindow.files[i].Name);
            }
        }
        #endregion
    }
}

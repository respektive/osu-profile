using osu_Profile.IO;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace osu_Profile.Forms
{
    /// <summary>
    /// Logique d'interaction pour Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        #region Attributes
        protected IniFile config;
        protected int _numValue = 0;
        #endregion

        #region Constructor
        public Settings()
        {
            InitializeComponent();
            config = null;
        }
        #endregion

        #region Properties
        public IniFile ConfigFile
        {
            set
            {
                config = value;
            }
        }

        public string Username
        {
            set
            {
                userbox.Text = value;
            }
            get
            {
                return userbox.Text;
            }
        }

        public string APIKey
        {
            set
            {
                apibox.Password = value;
            }
            get
            {
                return apibox.Password;
            }
        }

        public string TimeToWait
        {
            set
            {
                if (!int.TryParse(value, out _numValue))
                {
                    _numValue = 5;
                }
                if (_numValue < 5)
                    _numValue = 5;
                txtNum.Text = _numValue.ToString();
                if (MainWindow.MWindow.loopupdate != null)
                    MainWindow.MWindow.loopupdate.setTimer(_numValue);
                config.SetValue("User", "looptime", _numValue.ToString());
            }
            get
            {
                return txtNum.Text;
            }
        }

        public ItemCollection FileList
        {
            get
            {
                return filelist.Items;
            }
        }
        #endregion

        #region Handlers
        private void userbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Button_Click(sender, e);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.MWindow.Start(userbox.Text, apibox.Password))
                apibox.IsEnabled = false;
            else
                apibox.IsEnabled = true;
        }

        private void cmdUp_Click(object sender, RoutedEventArgs e) => TimeToWait = (_numValue + 1).ToString();

        private void cmdDown_Click(object sender, RoutedEventArgs e) => TimeToWait = (_numValue - 1).ToString();

        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindow.MWindow != null)
                TimeToWait = txtNum.Text;
        }

        private void modelist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainWindow.mode != modelist.SelectedIndex)
            {
                MainWindow.mode = modelist.SelectedIndex;
                config.SetValue("User", "mode", MainWindow.mode.ToString());
                MainWindow.MWindow.Start(userbox.Text, apibox.Password);
            }
        }

        private void modelist_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.mode = int.Parse(config.GetValue("User", "mode", "0"));
            }
            catch (Exception) { }
            if (MainWindow.mode < 0) MainWindow.mode = 0; if (MainWindow.mode > 3) MainWindow.mode = 3;
            modelist.SelectedIndex = MainWindow.mode;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Username = config.GetValue("User", "LastUsername", "");
            APIKey = config.GetValue("User", "APIkey", "");
            TimeToWait = config.GetValue("User", "looptime", "5");
            alwaysontopcheck.IsChecked = config.GetValue("User", "topmost", "false") == "true";
            popupEachMap.IsChecked = config.GetValue("User", "popupEachMap", "false") == "true";
            popupPPUp.IsChecked = config.GetValue("User", "popupPP", "false") == "true";
            checkOnStart.IsChecked = config.GetValue("User", "checkOnStart", "false") == "true";
            startWithWindows.IsChecked = config.GetValue("User", "startWithWindows", "false") == "true";
            versiontext.Dispatcher.BeginInvoke(new Action(() =>
            {
                versiontext.Content = "Version " + Assembly.GetEntryAssembly().GetName().Version.ToString();
            }), DispatcherPriority.Background);

            if (config.GetValue("User", "checkOnStart", "false") == "true")
                if (MainWindow.MWindow.Start(userbox.Text, apibox.Password))
                    apibox.IsEnabled = false;
                else
                    apibox.IsEnabled = true;
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            FilesWindow fwindow = new FilesWindow();
            fwindow.Owner = MainWindow.MWindow;
            fwindow.number = -1;
            fwindow.setlist(ref filelist);
            fwindow.Show();
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            if (filelist.SelectedIndex >= 0)
            {
                FilesWindow fwindow = new FilesWindow();
                fwindow.Owner = MainWindow.MWindow;
                fwindow.number = filelist.SelectedIndex;
                fwindow.setlist(ref filelist);
                fwindow.file = MainWindow.files[filelist.SelectedIndex].Name;
                fwindow.content = MainWindow.files[filelist.SelectedIndex].Content;
                fwindow.time = MainWindow.files[filelist.SelectedIndex].Time;
                fwindow.Show();
            }
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            if (filelist.SelectedIndex >= 0)
            {
                MainWindow.files.RemoveAt(filelist.SelectedIndex);
                config.SetValue("User", "files", MainWindow.files.Count.ToString());

                filelist.Items.Clear();
                for (int i = 0; i < MainWindow.files.Count; i++)
                {
                    filelist.Items.Add(MainWindow.files[i].Name);
                }
            }
        }

        private void scoremodelist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainWindow.scoremodeOld = MainWindow.scoremode;
            MainWindow.scoremode = scoremodelist.SelectedIndex;
            config.SetValue("User", "scoremode", MainWindow.scoremode.ToString());
        }

        private void scoremodelist_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.scoremode = int.Parse(config.GetValue("User", "scoremode", "1"));
            }
            catch (Exception) { }
            if (MainWindow.scoremode < 0) MainWindow.scoremode = 0; if (MainWindow.scoremode > 1) MainWindow.scoremode = 1;
            scoremodelist.SelectedIndex = MainWindow.scoremode;
        }

        private void rankingButton_Click(object sender, RoutedEventArgs e)
        {
            RankingSelector fwindow = new RankingSelector();
            fwindow.Owner = MainWindow.MWindow;
            fwindow.ShowDialog();
            MainWindow.MWindow.RankedScoreChangeBox.Dispatcher.Invoke(new Action(() =>
            {
                MainWindow.MWindow.UpdateRankingDisplay();
            }));
        }

        private void alwaysontopcheck_Unchecked(object sender, RoutedEventArgs e)
        {
            config.SetValue("User", "topmost", "false");
            MainWindow.MWindow.Topmost = false;
        }

        private void alwaysontopcheck_Checked(object sender, RoutedEventArgs e)
        {
            config.SetValue("User", "topmost", "true");
            MainWindow.MWindow.Topmost = true;
        }

        private void popupEachMap_Checked(object sender, RoutedEventArgs e)
        {
            config.SetValue("User", "popupEachMap", "true");
        }

        private void popupEachMap_Unchecked(object sender, RoutedEventArgs e)
        {
            config.SetValue("User", "popupEachMap", "false");
        }

        private void popupPP_Checked(object sender, RoutedEventArgs e)
        {
            config.SetValue("User", "popupPP", "true");
        }

        private void popupPP_Unchecked(object sender, RoutedEventArgs e)
        {
            config.SetValue("User", "popupPP", "false");
        }

        private void checkOnStart_Checked(object sender, RoutedEventArgs e)
        {
            config.SetValue("User", "checkOnStart", "true");
        }

        private void checkOnStart_Unchecked(object sender, RoutedEventArgs e)
        {
            config.SetValue("User", "checkOnStart", "false");
        }

        private void startWithWindows_Checked(object sender, RoutedEventArgs e)
        {
            config.SetValue("User", "startWithWindows", "true");

            string startupShotcut = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "osu!profile.lnk");
            Console.WriteLine(startupShotcut);
            if (!File.Exists(startupShotcut))
            {
                Shortcut.IShellLink link = (Shortcut.IShellLink)new Shortcut.ShellLink();

                link.SetDescription("osu!profile");
                link.SetPath(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                link.SetWorkingDirectory(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

                IPersistFile file = (IPersistFile)link;
                file.Save(startupShotcut, false);
            }
        }

        private void startWithWindows_Unchecked(object sender, RoutedEventArgs e)
        {
            config.SetValue("User", "startWithWindows", "false");

            string startupShotcut = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "osu!profile.lnk");
            if (File.Exists(startupShotcut))
            {
                File.Delete(startupShotcut);
            }
        }

        private void backgroundColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Color SelectedColor = (Color)backgroundColor.SelectedColor;
            MainWindow.ChangeThemeColor(SelectedColor.R, SelectedColor.G, SelectedColor.B);
            config.SetValue("User", "UI_red", SelectedColor.R.ToString());
            config.SetValue("User", "UI_green", SelectedColor.G.ToString());
            config.SetValue("User", "UI_blue", SelectedColor.B.ToString());
        }

        private void backgroundColor_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                byte r = byte.Parse(config.GetValue("User", "UI_red", "17"));
                byte g = byte.Parse(config.GetValue("User", "UI_green", "158"));
                byte b = byte.Parse(config.GetValue("User", "UI_blue", "218"));
                backgroundColor.SelectedColor = Color.FromRgb(r, g, b);
                MainWindow.ChangeThemeColor(r, g, b);
            }
            catch (Exception)
            {
                backgroundColor.SelectedColor = Color.FromRgb(0x11, 0x9E, 0xDA);
            }
        }
        #endregion

    }
}

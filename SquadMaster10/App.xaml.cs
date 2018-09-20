using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.Serialization.Json;
using ApplicationSettings;
using Windows.UI.ViewManagement;

namespace SquadMaster10
{
    public enum GameModeType { None = 0, Beginner = 1, Advanced = 2 };

    public class GameSettings
    {
        private bool _EnableGameScreen;
        private GameModeType _GameMode;
        private bool _ShowRounds;
        private int _TotalRounds;
        private int _totalPoints;

        public int TotalPoints
        {
            get { return _totalPoints; }
            set
            {
                _totalPoints = value;
            }
        }

        private int _CurrentPoints;

        public int CurrentPoints
        {
            get { return _CurrentPoints; }
            set { _CurrentPoints = value; }
        }

        public bool EnableGameScreen
        {
            get { return _EnableGameScreen; }
            set { _EnableGameScreen = value; }
        }

        public GameModeType GameMode
        {
            get { return _GameMode; }
            set { _GameMode = value; }
        }

        public bool ShowRounds
        {
            get { return _ShowRounds; }
            set { _ShowRounds = value; }
        }

        public int TotalRounds
        {
            get { return _TotalRounds; }
            set { _TotalRounds = value; }
        }

        public bool DataLoaded { get; set; }

        public GameSettings()
        {

        }
    }

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private static ObservableCollection<CardMini> carddataset = new ObservableCollection<CardMini>();
        public static ObservableCollection<CardMini> CardDataset
        {
            get { return carddataset; }
            set { carddataset = value; }
        }


        public static GameSettings GameSetting { get; set; }

        public static SettingsFlyout1 settingsFly { get; set; }

        public static ObservableCollection<CardMini> carddatasetselected = new ObservableCollection<CardMini>();
        public static ObservableCollection<GlyphControl> glyphdataset = new ObservableCollection<GlyphControl>();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            GameSetting = new GameSettings();
            GameSetting.GameMode = GameModeType.Advanced;
            GameSetting.EnableGameScreen = true;
            GameSetting.ShowRounds = true;
            GameSetting.TotalRounds = 10;   //default to 10
            GameSetting.DataLoaded = false;
            GameSetting.TotalPoints = 0;
            GameSetting.CurrentPoints = 0;
            
            settingsFly = new SettingsFlyout1();

            //ApplicationData.Current.RoamingSettings.Values.Clear();  //run this to forget settings
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("rounds"))
            {
                GameSetting.TotalRounds = Convert.ToInt16(ApplicationData.Current.RoamingSettings.Values["rounds"]);
            }
        }

        public static FrameworkElement GetParentByName(FrameworkElement currentPage, string parentName)
        {
            if (currentPage == null) return null;
            FrameworkElement fe = (FrameworkElement)currentPage.Parent;

            // Walk your way up the chain of Parents until we get a match
            while (fe.GetType().Name != parentName)
                fe = (FrameworkElement)fe.Parent;

            return fe;
        }

        public static BitmapImage ImageFromRelativePath(FrameworkElement parent, string path)
        {
            if (parent == null) return null;
            var uri = new Uri(parent.BaseUri, path);
            BitmapImage result = new BitmapImage();
            result.UriSource = uri;
            return result;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
                bool dataRetrievalComplete = false;
                dataRetrievalComplete = await GetPackagedFile(@"Assets", "HeroScapeDB.xml", Window.Current.Content);
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }


        private async Task<bool> GetPackagedFile(string folderName, string fileName, UIElement rootFrame)
        {
            StorageFolder installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = null;

            if (folderName != null)
            {
                StorageFolder subFolder = await installFolder.GetFolderAsync(folderName);
                file = await subFolder.GetFileAsync(fileName);
            }
            else
            {
                file = await installFolder.GetFileAsync(fileName);
            }

            //string text = await Windows.Storage.FileIO.ReadTextAsync(file);
            //datafile df = new datafile();
            //df = Newtonsoft.Json.JsonConvert.DeserializeObject<datafile>(text);



            XmlLoadSettings settings = new XmlLoadSettings() { ValidateOnParse = false };
            XmlDocument xmlDoc = await XmlDocument.LoadFromFileAsync(file, settings);

            foreach (IXmlNode xmlNode in xmlDoc.GetElementsByTagName("card"))
            {
                CardMini c = new CardMini();
                foreach (XmlAttribute xa in xmlNode.Attributes)
                {
                    if (xa.Value == null) continue; //skip bad/blank entries
                    if (xa.Value == "") continue;
                    switch (xa.Name.ToLower())
                    {
                        case "id":
                            c.Hero.EntryId = xa.Value;
                            break;

                        case "life":
                            c.Hero.Life = Convert.ToInt32(xa.Value);
                            break;

                        case "world":
                            c.Hero.World = xa.Value;
                            break;

                        case "set":
                            c.Hero.Set = xa.Value;
                            break;

                        case "collectornumber":
                            c.Hero.CollectorNumber = xa.Value;
                            break;

                        case "name":
                            c.Hero.Name = xa.Value;
                            break;

                        case "valkyrie":
                            c.Hero.Valkyrie = xa.Value;
                            break;

                        case "species":
                            c.Hero.Species = xa.Value;
                            break;

                        case "herotype":
                            c.Hero.HeroType = xa.Value;
                            break;

                        case "class":
                            c.Hero.ClassType = xa.Value;
                            break;

                        case "personality":
                            c.Hero.Personality = xa.Value;
                            break;

                        case "size":
                            c.Hero.Size = xa.Value;
                            break;

                        case "move":
                            c.Hero.Move = Convert.ToInt32(xa.Value);
                            break;

                        case "range":
                            c.Hero.Range = Convert.ToInt32(xa.Value);
                            break;

                        case "attack":
                            c.Hero.Attack = Convert.ToInt32(xa.Value);
                            break;

                        case "defense":
                            c.Hero.Defense = Convert.ToInt32(xa.Value);
                            break;

                        case "points":
                            c.Hero.Points = Convert.ToInt32(xa.Value);
                            break;

                        case "bmove":
                            c.Hero.BasicMove = Convert.ToInt32(xa.Value);
                            break;

                        case "brange":
                            c.Hero.BasicRange = Convert.ToInt32(xa.Value);
                            break;

                        case "battack":
                            c.Hero.BasicAttack = Convert.ToInt32(xa.Value);
                            break;

                        case "bdefense":
                            c.Hero.BasicDefense = Convert.ToInt32(xa.Value);
                            break;

                        case "armysize":
                            c.Hero.ArmySize = Convert.ToInt32(xa.Value);
                            break;
                    }
                }
                c.Hero.Image = string.Format("/Assets/Cards/{0}.png", c.Hero.EntryId);
                c.Hero.Thumb = string.Format("/Assets/Cards/_t/{0}.png", c.Hero.EntryId);
                //c.CardSource = ImageFromRelativePath(rootFrame, c.Hero.Thumb);
                c.CardName = c.Hero.Name;
                c.CardPoints = string.Format("{0} PTS", c.Hero.Points);
                c.UpdateData();

                CardDataset.Add(c);
            }

            foreach (IXmlNode xmlNode in xmlDoc.GetElementsByTagName("glyph"))
            {
                GlyphControl g = new GlyphControl();
                foreach (XmlAttribute xa in xmlNode.Attributes)
                {
                    switch (xa.Name.ToLower())
                    {
                        case "id":
                            g.EntryId = xa.Value;
                            break;

                        case "description":
                            g.Description = xa.Value;
                            break;
                    }
                }
                g.ImageFileName = string.Format("/Assets/Glyphs/{0}.png", g.EntryId);
                //                g.GlyphSource = ImageFromRelativePath(rootFrame, g.ImageFileName);
                glyphdataset.Add(g);
            }
            GameSetting.DataLoaded = true;
            return true;
        }

    }
}

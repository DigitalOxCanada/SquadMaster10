using System;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SquadMaster10
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public GameControl GameFlipView { get; set; }
        private static MainPage _current;
        private string PointsString;
        private string CardMiniPath;

        public static MainPage Current
        {
            get { return MainPage._current; }
            set { MainPage._current = value; }
        }

        public FlipView Flipper
        {
            get { return flipper; }
        }

        public MapControl Mapper
        {
            get { return mapper; }
        }

        public MainPage()
        {
            this.InitializeComponent();
            //SettingsPane.GetForCurrentView().CommandsRequested += StartPage_CommandsRequested;
            ResourceLoader rl = new ResourceLoader();
            PointsString = rl.GetString("PointsLabel");
            CardMiniPath = rl.GetString("CardMiniPath");
            GameFlipView = new GameControl();

            DisableTopAppBar();
            Flipper.SelectionChanged += Flipper_SelectionChanged;
            Current = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.ToString().Equals("BuildNow"))
            {
                //we came from the build page
                int cnt = 0;
                foreach (CardMini i in App.carddatasetselected)
                {
                    Card c = new Card();
                    c.Hero = i.Hero;
                    c.CardSource = App.ImageFromRelativePath(this, i.Hero.Image);
                    c.CardName = c.Hero.Name;
                    c.GenerateSquad();
                    Mapper.AddItem(cnt++, string.Format(CultureInfo.InvariantCulture, CardMiniPath, c.Hero.EntryId));
                    Flipper.Items.Add(c); //insert before this page
                }
                Mapper.AddItem(cnt++, "/Assets/mapicon.png");  //add game page icon as last item.
                Flipper.Items.Add(GameFlipView);

                BuildPaging();  //rebuild page numbering
                EnableTopAppBar(); //enable top bar now that we have a squad
                CheckForAppBarOptions();
                BuildPopup.IsOpen = false;  //close this popup squad builder
            }
            else
            {
                BuildPopup.IsOpen = true;   //we must not have an army so lets show the popup
            }
        }


        private async void DeleteSquad_Click_1(object sender, RoutedEventArgs e)
        {
            if (Flipper.Items.Count < 3)
            {
                var messageDialog = new MessageDialog("We can't remove your only squad.", "Delete Squad");

                messageDialog.Commands.Add(new UICommand("Ok"));

                // Show the message dialog
                await messageDialog.ShowAsync();
                return;
            }
            int currentpageindex = flipper.SelectedIndex;
            if (currentpageindex >= flipper.Items.Count() - 1) return;

            var thiscard = (flipper.Items[currentpageindex] as Card);

            //find this card in the selectedlist
            foreach (var it in App.carddatasetselected)
            {
                if (it.Hero.EntryId == thiscard.Hero.EntryId)
                {
                    App.carddatasetselected.Remove(it);
                    break;  //only remove 1 instance in case the same card was added multiple times
                }
            }

            flipper.Items.RemoveAt(currentpageindex);
            mapper.DeleteSquad(currentpageindex);

            BuildPaging();
        }

        void Flipper_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Flipper.SelectedItem == null)   //this happens when you start a new game and the flipper gets cleared
            {
                DisableTopAppBar();
                return;
            }
            if (Flipper.SelectedItem.GetType().Equals(typeof(Card)))
            {
                EnableTopAppBar();
                CheckForAppBarOptions();
            }
            else
            {
                DisableTopAppBar();
            }
        }
        public void DisableTopAppBar()
        {
            //topAppBar.IsOpen = false;
            //topAppBar.IsEnabled = false;
            //topAppBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        public void EnableTopAppBar()
        {
            //topAppBar.IsOpen = false;
            //topAppBar.IsEnabled = true;
            //topAppBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
        public void NotifyUser(string message, string command, NotifyType type)
        {
            //switch (type)
            //{
            //    case NotifyType.StatusMessage:
            //        break;
            //    case NotifyType.ErrorMessage:
            //        break;
            //}

            switch (command)
            {
                case "0":
                    //user cancelled
                    break;

                case "1":
                    //user started new game
                    mapper.Reset();
                    flipper.Items.Clear();

                    this.Frame.Navigate(typeof(BuilderPage));

                    break;
            }
        }

        public async void StartNewGameClick(object sender, RoutedEventArgs routedEvent)
        {
            var messageDialog = new MessageDialog("Starting a new game with lose all changes?", "Start New Game");

            // Add commands and set their callbacks
            messageDialog.Commands.Add(new UICommand("Cancel", (command) =>
            {
                Current.NotifyUser("The user cancelled.", "0", NotifyType.StatusMessage);
            }));

            messageDialog.Commands.Add(new UICommand("Start New Game", (command) =>
            {
                Current.NotifyUser("The user is starting a new game", "1", NotifyType.StatusMessage);
            }));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();

        }
        public int CalculateScore()
        {
            int pts = 0;
            foreach (var c in flipper.Items)
            {
                if (c.GetType() == typeof(Card))
                {
                    Card crd = (Card)c;
                    if (crd.IsAlive())
                    {
                        pts += crd.Hero.Points;
                    }
                }
            }
            Mapper.UpdateScore(pts, App.GameSetting.TotalPoints);
            GameFlipView.UpdateScore(pts, App.GameSetting.TotalPoints);
            return pts;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Help_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://digitalox.ca"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BuilderPage));
        }

        private void Button_Click(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BuilderPage));
        }


        private void CheckForAppBarOptions()
        {
            if (Flipper.Items.Count() < 3)  //1 card + GameControl
            {
                btnMoveLeft.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                btnMoveRight.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                btnMoveLeft.Visibility = Windows.UI.Xaml.Visibility.Visible;
                btnMoveRight.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            if (Flipper.SelectedIndex < Flipper.Items.Count() - 2 && Flipper.Items.Count() > 2)    //we have 2+ cards and 
            {
                btnMoveRight.IsEnabled = true;
                //btnMoveRight.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                btnMoveRight.IsEnabled = false;
                //btnMoveRight.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            if (Flipper.SelectedIndex > 0 && Flipper.Items.Count() > 2)    //we have 2+ cards and 
            {
                //btnMoveLeft.Visibility = Windows.UI.Xaml.Visibility.Visible;
                btnMoveLeft.IsEnabled = true;
            }
            else
            {
                btnMoveLeft.IsEnabled = false;
                //btnMoveLeft.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveLeft_Click_1(object sender, RoutedEventArgs e)
        {
            //reorder flipview
            //rebuild paging
            //reorder mapctl
            if (Flipper.Items.Count < 3) return;
            int currentpageindex = flipper.SelectedIndex;
            if (currentpageindex < 1) return;
            if (flipper.Items[currentpageindex].GetType().Equals(typeof(Card)))
            {
                Card c = (Card)flipper.Items[currentpageindex];
                flipper.Items.RemoveAt(currentpageindex);
                flipper.Items.Insert(currentpageindex - 1, c);
                BuildPaging();
                mapper.ReorderLeft(currentpageindex);
                flipper.SelectedIndex = currentpageindex - 1;
            }
        }

        private void MoveRight_Click_1(object sender, RoutedEventArgs e)
        {
            if (Flipper.Items.Count < 3) return;
            int currentpageindex = flipper.SelectedIndex;
            if (currentpageindex >= flipper.Items.Count() - 1) return;
            if (flipper.Items[currentpageindex].GetType().Equals(typeof(Card)))
            {
                Card c = (Card)flipper.Items[currentpageindex];
                flipper.Items.RemoveAt(currentpageindex);
                flipper.Items.Insert(currentpageindex + 1, c);
                BuildPaging();
                mapper.ReorderRight(currentpageindex);
                flipper.SelectedIndex = currentpageindex + 1;
            }
        }

        private void UpdatePoints()
        {
            //calculate TOTAL points from dataset
            App.GameSetting.TotalPoints = App.carddatasetselected.Sum(x => x.Hero.Points);

            App.GameSetting.CurrentPoints = 0;  //reset to calculate
            foreach (var it in flipper.Items)
            {
                if (it.GetType().Equals(typeof(Card)))
                {
                    Card c = it as Card;
                    if (c.IsAlive())
                    {
                        App.GameSetting.CurrentPoints += c.Hero.Points;
                    }
                }
            }
            GameFlipView.UpdateScore(App.GameSetting.CurrentPoints, App.GameSetting.TotalPoints);
            Mapper.UpdateScore(App.GameSetting.CurrentPoints, App.GameSetting.TotalPoints);
        }

        private void BuildPaging()
        {
            int cnt = 1;
            int total = Flipper.Items.Count() - 1; //-1 because of the gamectl being the last item
            for (int i = 0; i < total; i++)
            {
                if (Flipper.Items[i].GetType().Equals(typeof(Card)))    //make sure its a card and not GameControl
                {
                    ((Card)Flipper.Items[i]).CardPage = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", cnt++, total);
                }
            }
            UpdatePoints();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //settingsFlyout.Closed += delegate (object sn, object e)
            //{
            //    App.GameSetting.TotalRounds = Convert.ToInt16(cb.SelectionBoxItem, CultureInfo.InvariantCulture);

            //    //store in roaming settings
            //    ApplicationData.Current.RoamingSettings.Values["rounds"] = App.GameSetting.TotalRounds;
            //};
            //App.settingsFly.Content = stackPanel;
            
            App.settingsFly.UpdateFromValues();
            App.settingsFly.ShowIndependent();
        }
    }
}

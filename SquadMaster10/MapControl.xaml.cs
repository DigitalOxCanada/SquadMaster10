using System.Globalization;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace SquadMaster10
{
    public sealed partial class MapControl : UserControl
    {
        private string _currentscorelabel;

        public MapControl()
        {
            this.InitializeComponent();
            ResourceLoader rl = new ResourceLoader();
            _currentscorelabel = rl.GetString("CurrentScoreLabel");
        }

        public TextBlock ScoreLabel
        {
            get { return scorelabel; }
        }

        public void ClearItems()
        {
            stacker.Children.Clear();
        }

        public void ReorderLeft(int page)
        {
            MapItemControl m = (MapItemControl)stacker.Children[page];
            stacker.Children.RemoveAt(page);
            stacker.Children.Insert(page - 1, m);
            RebuildNumbering();
        }

        public void ReorderRight(int page)
        {
            MapItemControl m = (MapItemControl)stacker.Children[page];
            stacker.Children.RemoveAt(page);
            stacker.Children.Insert(page + 1, m);
            RebuildNumbering();
        }

        public void RebuildNumbering()
        {
            int cnt = 0;
            foreach (MapItemControl item in stacker.Children)
            {
                item.PageIndex = cnt++;
            }
        }

        public void AddItem(int pageIndex, string imageFileName)
        {
            MapItemControl mctl = new MapItemControl();
            mctl.PageIndex = pageIndex;
            mctl.SetSource(App.ImageFromRelativePath(this, imageFileName));
            mctl.Tapped += mctl_Tapped;
            stacker.Children.Add(mctl);
        }

        private void mctl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //MainPage mainpage = (MainPage)App.GetParentByName(this, "MainPage");
            //navigate to the proper page index on the flipview
            MapItemControl mi = (MapItemControl)sender;
            MainPage.Current.Flipper.SelectedIndex = mi.PageIndex;
        }

        internal void DeleteSquad(int page)
        {
            stacker.Children[page].Tapped -= mctl_Tapped;
            stacker.Children.RemoveAt(page);
            RebuildNumbering();
        }

        internal void UpdateScore(int curpts, int totalpts)
        {
            ScoreLabel.Text = string.Format(CultureInfo.InvariantCulture, _currentscorelabel, curpts, totalpts);
        }

        internal void Reset()
        {
            stacker.Children.Clear();
            RebuildNumbering();
        }
    }
}
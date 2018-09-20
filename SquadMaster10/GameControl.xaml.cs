using System;
using System.Globalization;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace SquadMaster10
{
    public sealed partial class GameControl : UserControl
    {
        //int _currentround = 1;
        //int _maxround = 1;
        private string _currentscorelabel;

        public GameControl()
        {
            this.InitializeComponent();
            ResourceLoader rl = new ResourceLoader();
            _currentscorelabel = rl.GetString("CurrentScoreLabel");

            lister.DataContext = App.glyphdataset;
            Reset();
        }

        public TextBlock ArmyScore
        {
            get { return armyscore; }
        }

        public GridView Lister
        {
            get { return lister; }
        }

        public void Reset()
        {
            if (App.GameSetting.ShowRounds)
            {
                RoundsStack.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                RoundsStack.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                return;
            }
            int cnt = 0;
            foreach (var rnd in RoundsStack.Children)
            {
                if (rnd.GetType().Equals(typeof(RoundMarker)))
                {
                    RoundMarker rm = rnd as RoundMarker;
                    rm.Select(false);
                    cnt++;
                    if (cnt > App.GameSetting.TotalRounds)
                    {
                        rm.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        rm.ShowArrow();
                    }
                    else
                    {
                        rm.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        rm.ShowArrow();
                    }

                    if (cnt == App.GameSetting.TotalRounds)
                    {
                        rm.ShowArrow(false);
                    }
                    if (cnt == 1)
                    {
                        rm.Select(true);
                    }
                }
            }
        }

        internal void UpdateScore(int curpts, int totalpts)
        {
            ArmyScore.Text = string.Format(CultureInfo.InvariantCulture, _currentscorelabel, curpts, totalpts);
        }

        private void RoundMarker_Tapped(object sender, EventArgs e)
        {
            RoundMarker it = (RoundMarker)App.GetParentByName((Image)sender, "RoundMarker");
            foreach (var rnd in RoundsStack.Children)
            {
                if (rnd.GetType().Equals(typeof(RoundMarker)))
                {
                    ((RoundMarker)rnd).Select(false);
                }
            }
            it.Select();
        }
    }
}
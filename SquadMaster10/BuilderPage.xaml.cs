using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SquadMaster10
{
    /// <summary>
    /// 
    /// </summary>
    public class CaseInsensitiveComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class BuilderPage : Page //SquadMaster10.Common.LayoutAwarePage
    {
        public bool DataLoaded { get; set; }
        public string PointsString { get; set; }
        public string CardMiniPath { get; set; }

        public BuilderPage()
        {
            this.InitializeComponent();
            DataLoaded = false;

            ResourceLoader rl = new ResourceLoader();
            PointsString = rl.GetString("PointsLabel");
            CardMiniPath = rl.GetString("CardMiniPath");

            cbSpecies.SelectionChanged += cbSpecies_SelectionChanged;
            cbPoints.SelectionChanged += cbPoints_SelectionChanged;
            
            App.carddatasetselected.Clear();
            gridResults.DataContext = App.CardDataset;
            listSquad.DataContext = App.carddatasetselected;
            BuildFilters();
        }

        /// <summary>
        /// 
        /// </summary>
        private void BuildFilters()
        {
            if (DataLoaded) return;
            if (App.GameSetting.DataLoaded)
            {
                foreach (var it in App.CardDataset)
                {
                    it.CardSource = App.ImageFromRelativePath(this, it.Hero.Thumb);
                }
                foreach (var it in App.glyphdataset)
                {
                    it.GlyphSource = App.ImageFromRelativePath(this, it.ImageFileName);
                }
                DataLoaded = true;
                cbSpecies.Items.Clear();
                cbSpecies.Items.Add(new ComboBoxItem() { Content = "All", Tag = "" });
                cbSpecies.SelectedIndex = 0;    //default to All

                var uniquespecies = App.CardDataset.Select(x => x.Hero.Species).Distinct().OrderBy(a => a, new CaseInsensitiveComparer());
                foreach (var it in uniquespecies)
                {
                    int uniqueit = App.CardDataset.Count(y => y.Hero.Species.Equals(it));
                    cbSpecies.Items.Add(new ComboBoxItem() { Content = string.Format("{0} ({1})", it, uniqueit), Tag = it.ToString() });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            gridResults.SelectedIndex = -1;
            App.carddatasetselected.Clear();
            UpdatePoints();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBuildSquad_Click_1(object sender, RoutedEventArgs e)
        {
            if (App.carddatasetselected.Count < 1) return;

            this.Frame.Navigate(typeof(MainPage), "BuildNow");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click_1(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdatePoints()
        {
            App.GameSetting.TotalPoints = App.carddatasetselected.Sum(x => x.Hero.Points);

            pointsLabel.Text = string.Format(CultureInfo.InvariantCulture, PointsString, App.GameSetting.TotalPoints);
            if (App.carddatasetselected.Count > 0)
            {
                btnBuildSquad.IsEnabled = true;
            }
            else
            {
                btnBuildSquad.IsEnabled = false;
            }

            if (App.carddatasetselected.Count < 20)
            {
                gridResults.IsEnabled = true;
                gridResults.Opacity = 1.0;
            }
            else
            {
                gridResults.IsEnabled = false;
                gridResults.Opacity = 0.5;
            }
            BuildFilters();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSpecies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1) return;
            FilterChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbPoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1) return;
            FilterChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        private void FilterChanged()
        {
            //if we picked ALL on both, then default to show entire dataset
            if ((cbSpecies.SelectedItem as ComboBoxItem).Content.ToString().Equals("All") && ((cbPoints.SelectedItem as ComboBoxItem).Content.ToString().Equals("All")))
            {
                gridResults.DataContext = App.CardDataset;
            }
            else
            {
                ComboBoxItem cbPointsSel = (cbPoints.SelectedItem as ComboBoxItem);
                var pointsplits = cbPointsSel.Tag.ToString().Split('-');

                if ((cbSpecies.SelectedItem as ComboBoxItem).Content.ToString().Equals("All"))
                {
                    var result = App.CardDataset.Where(w => w.Hero.Points >= Convert.ToInt16(pointsplits[0]) & w.Hero.Points <= Convert.ToInt16(pointsplits[1]));
                    gridResults.DataContext = result;
                }
                else
                {
                    var result = App.CardDataset.Where(w => w.Hero.Species.Equals((cbSpecies.SelectedItem as ComboBoxItem).Tag.ToString()) & w.Hero.Points >= Convert.ToInt16(pointsplits[0]) & w.Hero.Points <= Convert.ToInt16(pointsplits[1]));
                    gridResults.DataContext = result;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listSquad_ItemClick(object sender, ItemClickEventArgs e)
        {
            App.carddatasetselected.Remove(e.ClickedItem as CardMini);
            UpdatePoints();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridResults_ItemClick(object sender, ItemClickEventArgs e)
        {
            CardMini cardSel = e.ClickedItem as CardMini;
            CardMini selcard = new CardMini();
            selcard.Hero = cardSel.Hero;
            selcard.CardName = cardSel.CardName;
            selcard.CardPoints = cardSel.CardPoints;
            selcard.CardSource = cardSel.CardSource;
            selcard.UpdateData();

            App.carddatasetselected.Add(selcard);
            listSquad.DataContext = App.carddatasetselected;

            UpdatePoints();
        }       

        ///// <summary>
        ///// Populates the page with content passed during navigation.  Any saved state is also
        ///// provided when recreating a page from a prior session.
        ///// </summary>
        ///// <param name="navigationParameter">The parameter value passed to
        ///// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        ///// </param>
        ///// <param name="pageState">A dictionary of state preserved by this page during an earlier
        ///// session.  This will be null the first time a page is visited.</param>
        //protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        //{
        //}

        ///// <summary>
        ///// Preserves state associated with this page in case the application is suspended or the
        ///// page is discarded from the navigation cache.  Values must conform to the serialization
        ///// requirements of <see cref="SuspensionManager.SessionState"/>.
        ///// </summary>
        ///// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        //protected override void SaveState(Dictionary<String, Object> pageState)
        //{
        //}
    }
}

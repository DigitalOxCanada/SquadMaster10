using Platoon;
using System;
using System.Globalization;
using System.Resources;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Platoon10;

namespace SquadMaster10
{
    public sealed partial class CardMini : UserControl
    {
        private HeroUnit _Hero;// = new HeroUnit();

        public HeroUnit Hero
        {
            get { return _Hero; }
            set { _Hero = value; }
        }

        private string _statline;

        public string StatLine
        {
            get { return _statline; }
            set { _statline = value; }
        }

        public CardMini()
        {
            this.InitializeComponent();
            Hero = new HeroUnit();
            ResourceLoader rl = new ResourceLoader();
            StatLine = rl.GetString("CardMiniStatsLine");
        }

        public void UpdateData()
        {
            CardName = Hero.Name;

            //txtStats.Text = String.Format(CultureInfo.InvariantCulture, "M:{0} R:{1} A:{2} D:{3}", Hero.Move, Hero.Range, Hero.Attack, Hero.Defense);
            CardStats = String.Format(CultureInfo.InvariantCulture, StatLine, Hero.Move, Hero.Range, Hero.Attack, Hero.Defense);
            CardSize = String.Format(CultureInfo.InvariantCulture, "{0}", Hero.Size);
        }

        public ImageSource CardSource
        {
            get { return (ImageSource)GetValue(CardSourceProperty); }
            set { SetValue(CardSourceProperty, value); }
        }

        internal static  DependencyProperty CardSourceProperty = DependencyProperty.Register("CardSource", typeof(ImageSource), typeof(CardMini), new PropertyMetadata(null));

        public string CardSize
        {
            get { return (string)GetValue(CardSizeProperty); }
            set { SetValue(CardSizeProperty, value); }
        }
        internal static DependencyProperty CardSizeProperty = DependencyProperty.Register("CardSize", typeof(string), typeof(CardMini), new PropertyMetadata(null));

        public string CardStats
        {
            get { return (string)GetValue(CardStatsProperty); }
            set { SetValue(CardStatsProperty, value); }
        }
        internal static DependencyProperty CardStatsProperty = DependencyProperty.Register("CardStats", typeof(string), typeof(CardMini), new PropertyMetadata(null));

        public string CardName
        {
            get { return (string)GetValue(CardNameProperty); }
            set { SetValue(CardNameProperty, value); }
        }

        internal static DependencyProperty CardNameProperty = DependencyProperty.Register("CardName", typeof(string), typeof(CardMini), new PropertyMetadata(null));

        public string CardPoints
        {
            get { return (string)GetValue(CardPointsProperty); }
            set { SetValue(CardPointsProperty, value); }
        }

        internal static DependencyProperty CardPointsProperty = DependencyProperty.Register("CardPoints", typeof(string), typeof(CardMini), new PropertyMetadata(null));
    }
}
using Platoon;
using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Platoon10;

namespace SquadMaster10
{
    public sealed partial class Card : UserControl
    {
        private HeroUnit _herounit; // = new Hero();

        public HeroUnit Hero
        {
            get { return _herounit; }
            set { _herounit = value; }
        }

        //public List<SquadSingle> squad = new List<SquadSingle>();

        public Card()
        {
            this.InitializeComponent();
            Hero = new HeroUnit();
        }

        public void DoWobble()
        {
            Wobble.Stop();
            Wobble.Begin();
        }

        public ImageSource CardSource
        {
            get { return (ImageSource)GetValue(CardSourceProperty); }
            set { SetValue(CardSourceProperty, value); }
        }

        internal static DependencyProperty CardSourceProperty = DependencyProperty.Register("CardSource", typeof(ImageSource), typeof(Card), new PropertyMetadata(null));

        public string CardName
        {
            get { return (string)GetValue(CardNameProperty); }
            set { SetValue(CardNameProperty, value); }
        }

        internal static DependencyProperty CardNameProperty = DependencyProperty.Register("CardName", typeof(string), typeof(Card), new PropertyMetadata(null));

        public string CardPage
        {
            get { return (string)GetValue(CardPageProperty); }
            set { SetValue(CardPageProperty, value); }
        }

        internal static DependencyProperty CardPageProperty = DependencyProperty.Register("CardPage", typeof(string), typeof(Card), new PropertyMetadata(null));

        internal bool IsAlive()
        {
            //if any squad member has life, then the army is alive and should award full points
            foreach (SquadSingle ss in SquadStacker.Children)
            {
                if (ss.CurLife < ss.MaxLife) return true;
            }
            return false;
        }

        internal void GenerateSquad()
        {
            for (int i = 0; i < Hero.ArmySize; i++)
            {
                SquadSingle ar = new SquadSingle();
                ar.MaxLife = Hero.Life;
                ar.CurLife = 0;
                ar.ImageSoldier = App.ImageFromRelativePath(this, string.Format(CultureInfo.InvariantCulture, "/Assets/Cards/_s/{0}-{1}.png", Hero.EntryId, i + 1));
                int h = 100;
                string[] str = Hero.Size.Split(' ');
                switch (Convert.ToInt32(str[1], CultureInfo.InvariantCulture))
                {
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        h += 50;
                        break;

                    case 10:
                    case 11:
                    case 12:
                    case 13:
                        h += 100;
                        break;
                }
                ar.Height = h;

                //squad.Add(ar);
                this.SquadStacker.Children.Add(ar);
            }
        }
    }
}
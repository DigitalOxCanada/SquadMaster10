using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SquadMaster10
{
    public sealed partial class SquadSingle : UserControl
    {
        internal static  DependencyProperty ImageSoldierProperty = DependencyProperty.Register("ImageSoldier", typeof(ImageSource), typeof(SquadSingle), new PropertyMetadata(null));
        private int _curLife = 0;

        public int CurLife
        {
            get { return _curLife; }
            set { _curLife = value; }
        }

        //        public List<DamageChit> damChits = new List<DamageChit>();
        private int _maxLife = 0;

        public int MaxLife
        {
            get { return _maxLife; }
            set { _maxLife = value; }
        }

        internal SolidColorBrush alivebrush = new SolidColorBrush(Colors.DarkSlateGray);
        internal SolidColorBrush deadbrush = new SolidColorBrush(Colors.Red);

        public SquadSingle()
        {
            this.InitializeComponent();
        }

        public ImageSource ImageSoldier
        {
            get { return (ImageSource)GetValue(ImageSoldierProperty); }
            set { SetValue(ImageSoldierProperty, value); }
        }

        private void btnDamageMinus_Click(object sender, RoutedEventArgs e)
        {
            if (damageCanvas.Children.Count() > 0)
            {
                for (int i = this.damageCanvas.Children.Count() - 1; i > -1; i--)
                {
                    if (this.damageCanvas.Children[i].GetType().Equals(typeof(DamageChit)))
                    {
                        //damChits.Remove((DamageChit)this.damageCanvas.Children[i]);
                        this.damageCanvas.Children.RemoveAt(i);

                        break;
                    }
                }
            }
            CurLife = damageCanvas.Children.Count();
            if (damageCanvas.Children.Count() < MaxLife)
            {
                theblock.Background = alivebrush;
            }
            MainPage.Current.CalculateScore();
        }

        private void btnDamagePlus_Click(object sender, RoutedEventArgs e)
        {
            if (damageCanvas.Children.Count() < MaxLife)
            {
                DamageChit dc = new DamageChit();
                dc.SetValue(Canvas.LeftProperty, 50 + damageCanvas.Children.Count() * 36);
                dc.SetValue(Canvas.TopProperty, 50);

                //damChits.Add(dc);
                this.damageCanvas.Children.Add(dc);
                dc.Added();
            }
            CurLife = damageCanvas.Children.Count();
            if (damageCanvas.Children.Count() >= MaxLife)
            {
                theblock.Background = deadbrush;
            }
            MainPage.Current.CalculateScore();
        }
    }
}
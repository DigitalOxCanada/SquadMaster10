using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SquadMaster10
{
    public sealed partial class RoundMarker : UserControl
    {
        internal static  DependencyProperty RoundValueProperty = DependencyProperty.Register("RoundValue", typeof(string), typeof(RoundMarker), new PropertyMetadata(null));

        internal SolidColorBrush White = new SolidColorBrush(Windows.UI.Colors.White);

        internal SolidColorBrush Yellow = new SolidColorBrush(Windows.UI.Colors.Yellow);

        public RoundMarker()
        {
            this.InitializeComponent();

            //thebutton.Tapped += thebutton_Tapped;
        }

        public event EventHandler OnButtonHit;

        public string RoundValue
        {
            get { return (string)GetValue(RoundValueProperty); }
            set { SetValue(RoundValueProperty, value); if (value != null) { thenumber.Text = value.ToString(); } }
        }

        public Image TheButton
        {
            get { return TheButtonControl; }
        }

        //public bool ShowArrow
        //{
        //    get { return (bool)GetValue(ShowArrowProperty); }
        //    set { SetValue(ShowArrowProperty, value); }
        //    //get { return (bool)(thearrow.Visibility == Windows.UI.Xaml.Visibility.Visible ? true : false); } // GetValue(ShowArrowProperty); }
        //    //set { if (value) thearrow.Visibility = Windows.UI.Xaml.Visibility.Visible; else thearrow.Visibility = Windows.UI.Xaml.Visibility.Collapsed; }  //SetValue(ShowArrowProperty, value);
        //}
        //public static readonly DependencyProperty ShowArrowProperty = DependencyProperty.Register("ShowArrow", typeof(bool), typeof(bool), new PropertyMetadata(null));
        public void SetNumber(int number)
        {
            thenumber.Text = number.ToString(CultureInfo.InvariantCulture);
        }

        public void ShowArrow()
        {
            ShowArrow(true);
        }
        public void ShowArrow(bool show)
        {
            if (show) thearrow.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else thearrow.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        public void TheButtonTapped(object sender, TappedRoutedEventArgs tappedEvent)
        {
            OnButtonHit(sender, null);
        }

        internal void Select(bool p = true)
        {
            if (p)
            {
                thenumber.Foreground = Yellow;
                thenumber.FontWeight = Windows.UI.Text.FontWeights.Bold;
            }
            else
            {
                thenumber.Foreground = White;
                thenumber.FontWeight = Windows.UI.Text.FontWeights.Normal;
            }
        }
    }
}
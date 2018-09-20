using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SquadMaster10
{
    public sealed partial class MapItemControl : UserControl
    {
        private int _pageIndex = 0;

        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }

        public MapItemControl()
        {
            this.InitializeComponent();
        }

        public void SetSource(ImageSource imageSource)
        {
            image.Source = imageSource;
        }
    }
}
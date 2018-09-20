using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SquadMaster10
{
    public sealed partial class GlyphControl : UserControl
    {
        internal static DependencyProperty GlyphSourceProperty = DependencyProperty.Register("GlyphSource", typeof(ImageSource), typeof(GlyphControl), new PropertyMetadata(null));
        private string _description;
        private string _EntryId;

        private string _imageFileName;

        public GlyphControl()
        {
            this.InitializeComponent();
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string EntryId
        {
            get { return _EntryId; }
            set { _EntryId = value; }
        }
        public ImageSource GlyphSource
        {
            get { return (ImageSource)GetValue(GlyphSourceProperty); }
            set { SetValue(GlyphSourceProperty, value); }
        }

        public string ImageFileName
        {
            get { return _imageFileName; }
            set { _imageFileName = value; }
        }
    }
}
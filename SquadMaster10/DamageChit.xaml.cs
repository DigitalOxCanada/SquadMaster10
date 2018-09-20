using Windows.UI.Xaml.Controls;

namespace SquadMaster10
{
    public sealed partial class DamageChit : UserControl
    {
        public DamageChit()
        {
            this.InitializeComponent();
        }

        public void Added()
        {
            HitAnim.Stop();
            HitAnim.Begin();
        }
    }
}
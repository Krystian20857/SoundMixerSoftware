using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace SoundMixerSoftware.Controls
{
    public class WebHyperLink : Hyperlink
    {
        public WebHyperLink()
        {
            RequestNavigate += OnRequestNavigate;
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }
    }
}
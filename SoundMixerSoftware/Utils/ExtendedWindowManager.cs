using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Caliburn.Micro;

namespace SoundMixerSoftware.Utils
{
    public class ExtendedWindowManager : WindowManager
    {
        public override async Task ShowWindowAsync(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            NavigationWindow navWindow = null;
            var showWindow = true;
            if (settings != null)
            {
                if (settings.TryGetValue("showWindow", out var value))
                    showWindow = value is bool ? (bool) value : true;
            }

            var application = Application.Current;
            if (application != null && application.MainWindow != null)
            {
                navWindow = application.MainWindow as NavigationWindow;
            }

            if (navWindow != null)
            {
                var window = await CreatePageAsync(rootModel, context, settings);
                if(showWindow)
                    navWindow.Navigate(window);
            }
            else
            {
                var window = await CreateWindowAsync(rootModel, false, context, settings);
                if (showWindow)
                    window.Show();
            }
        }
    }
}
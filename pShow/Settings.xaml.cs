using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Text.RegularExpressions;
using pShow.Resources;

namespace pShow
{
    /// <summary>
    /// The page resonsible for application settings.
    /// </summary>
    public partial class Settings : PhoneApplicationPage
    {
        /// <summary>
        /// The page constructor.
        /// </summary>
        public Settings()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Opacity = 0.5;
            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton saveButton = new ApplicationBarIconButton(new Uri("/Resources/save.png", UriKind.Relative));
            saveButton.Text = AppResources.SaveTitle;
            saveButton.Click += saveSettings;
            ApplicationBar.Buttons.Add(saveButton);
        }

        /// <summary>
        /// Set the slide duration from settings.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            durationSetting.Text = App.slideDuration.ToString();
        }

        private void saveSettings(object sender, EventArgs e)
        {
            var durationLine = durationSetting.Text;
            if (Regex.IsMatch(durationLine, @"^[0-9]+$"))
            {
                System.IO.TextWriter tr = new System.IO.StreamWriter("Resources/settings.txt");
                tr.WriteLineAsync("duration=" + durationLine);
                tr.Close();
                App.slideDuration = Int32.Parse(durationLine);
                MessageBox.Show(AppResources.SaveSuccess);
            }
            else
            {
                MessageBox.Show(AppResources.SaveFailedNoNumber);
            }
            
        }
    }
}
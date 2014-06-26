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
        static private Dictionary<int, string> sortorderList = new Dictionary<int, string>() 
        { 
            {0, AppResources.SettingsDateAsc},
            {1, AppResources.SettingsDateDsc},
            {2, AppResources.SettingsNameAsc},
            {3, AppResources.SettingsNameDsc},
            {4, AppResources.SettingsRandom}
        };

        static private Dictionary<int, string> blendmodeList = new Dictionary<int, string>() 
        { 
            {0, AppResources.SettingsBlendModeNone},
            {1, AppResources.SettingsBlendModeFading},
        };

        /// <summary>
        /// The page constructor.
        /// </summary>
        public Settings()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
            sortorderPicker.ItemsSource = sortorderList.Values.ToList();
            blendmodePicker.ItemsSource = blendmodeList.Values.ToList();
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
            durationSetting.Text = App.userSettings["slideDuration"].ToString();
            sortorderPicker.SelectedIndex = (int)App.userSettings["sortOrder"];
            blendmodePicker.SelectedIndex = (int)App.userSettings["blendMode"];
        }

        /// <summary>
        /// Save the parameters to the settings file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveSettings(object sender, EventArgs e)
        {
            var durationLine = durationSetting.Text;
            if (Regex.IsMatch(durationLine, @"^[0-9]+$"))
            {
                App.userSettings["slideDuration"] = Int32.Parse(durationLine);
                App.userSettings["sortOrder"] = sortorderPicker.SelectedIndex;
                App.userSettings["blendMode"] = blendmodePicker.SelectedIndex;
                MessageBox.Show(AppResources.SaveSuccess);
            }
            else
            {
                MessageBox.Show(AppResources.SaveFailedNoNumber);
            }
            
        }
    }
}
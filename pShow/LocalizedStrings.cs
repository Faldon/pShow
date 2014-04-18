using pShow.Resources;

namespace pShow
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static AppResources _localizedResources = new AppResources();

        /// <summary>
        /// Provide localized resources.
        /// </summary>
        public AppResources LocalizedResources { get { return _localizedResources; } }
    }
}
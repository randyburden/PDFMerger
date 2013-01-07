using System;
using System.Configuration;
using System.Windows;

namespace PDFMerger.Helpers
{
    /// <summary>
    /// Used as a type-safe interface for App.Config settings
    /// </summary>
    public static class ConfigurationHelper
    {
        #region Public Properties

        /// <summary>
        /// The default environment to load on application startup
        /// </summary>
        public static string ThemeName { get; set; }

        #endregion Public Properties

        #region Constructor

        static ConfigurationHelper()
        {
            SetDefaults();

            TryToLoadKeys();
        }

        #endregion Constructor

        #region Private Methods

        /// <summary>
        /// Sets the default values for appSetting keys
        /// </summary>
        private static void SetDefaults()
        {
            ThemeName = "Inc";
        }

        /// <summary>
        /// Attempts to load appSetting values and uses default values if the appSetting key doesn't exist
        /// </summary>
        private static void TryToLoadKeys()
        {
            ThemeName = GetAppSettingsValueFromKey( AppSettingKeys.ThemeName ) ?? ThemeName;
        }

        /// <summary>
        /// Helper method to get values from appSettings
        /// </summary>
        /// <param name="key">AppSetting key</param>
        /// <returns>AppSetting value</returns>
        private static string GetAppSettingsValueFromKey( string key )
        {
            string value = null;

            try
            {
                value = ConfigurationManager.AppSettings[ key ];
            }
            catch ( Exception ex )
            {
                // Swallow error and use defaults
                System.Diagnostics.Debug.WriteLine( ex.Message );
            }

            return value;
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Saves all current appSetting values
        /// </summary>
        /// <returns>Boolean value whether or not the save was successful</returns>
        public static bool SaveAll()
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.None );
                
                config.AppSettings.Settings.Remove( AppSettingKeys.ThemeName );
                config.AppSettings.Settings.Add( AppSettingKeys.ThemeName, ThemeName );

                config.Save();

                return true;
            }
            catch ( Exception ex )
            {
                MessageBox.Show(
                    string.Format( "An error occurred while attempting to save application settings: {0}", ex.Message ),
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error );
            }

            return false;
        }

        #endregion Public Methods

        #region Nested type: AppSettingsKeys

        /// <summary>
        /// Used as a type-safe interface for storing appSetting keys
        /// </summary>
        private static class AppSettingKeys
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            
            public const string ThemeName = "ThemeName";

            // ReSharper restore MemberHidesStaticFromOuterClass
        }

        #endregion Nested type: AppSettingsKeys
    }
}

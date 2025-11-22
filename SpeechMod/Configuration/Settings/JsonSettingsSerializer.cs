using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace SpeechMod.Configuration.Settings
{
    /// <summary>
    /// Handles serialization and deserialization of JsonSettings to and from JSON files.
    /// </summary>
    public static class JsonSettingsSerializer
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Include,
        };

        
        /// <summary>
        /// Saves JsonSettings object to the specified JSON file.
        /// Isn't used right now, but might be in the future
        /// </summary>
        /// <param name="settings">The settings object to serialize</param>
        /// <param name="filePath">Path to save the JSON file</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool SaveSettings(JsonSettings settings, string filePath)
        {
            if (!File.Exists(filePath)) {
                return false;
            }
            
            try
            {
                string json = JsonConvert.SerializeObject(settings, SerializerSettings);
                File.WriteAllText(filePath, json, Encoding.UTF8);
                Main.Logger?.Log($"Settings saved to {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Main.Logger?.Error($"Failed to save settings: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loads JsonSettings from the specified JSON file
        /// </summary>
        /// <param name="filePath">Path to the JSON file (optional, uses default if not specified)</param>
        /// <returns>Loaded JsonSettings object or default if file doesn't exist or is invalid</returns>
        public static JsonSettings LoadSettings(string filePath = null)
        {
            Main.Logger?.Log("Loading JSON settings...");

            try
            {
                if (!File.Exists(filePath))
                {
                    Main.Logger?.Log($"Settings file not found at {filePath}, creating with defaults");
                    JsonSettings defaultSettings = new JsonSettings();
                    //SaveSettings(defaultSettings, filePath);
                    return defaultSettings;
                }

                string json = File.ReadAllText(filePath, Encoding.UTF8);
                JsonSettings settings = JsonConvert.DeserializeObject<JsonSettings>(json, SerializerSettings);
                Main.Logger?.Log($"Settings loaded from {filePath}");
                return settings;
            }
            catch (Exception ex)
            {
                Main.Logger?.Error($"Failed to load settings: {ex.Message}");
                return new JsonSettings(); // Return default settings on error
            }
        }

    }
}

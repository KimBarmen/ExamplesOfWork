using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElectronBoilerplate.Data
{
    /*
     * Saves and reads SettingsDto to and from .json-files
     */
    public class SettingsService
    {
        public Task<SettingsDto> GetSettings(string path)
        {
            if ((path == "") || path is null){
                path = "./ApplicationPrefrences.json";
            }

            var jsonStr = File.ReadAllText(path);
            var settingsContainer = JsonSerializer.Deserialize<SettingsDto>(jsonStr);
            return Task.FromResult<SettingsDto>(settingsContainer);
        }

        public void PostSettings(SettingsDto dtoToSave, string path)
        {
            var jsonStr = JsonSerializer.Serialize(dtoToSave,
                new JsonSerializerOptions() { WriteIndented = true }
            );
            File.WriteAllText(path, jsonStr, Encoding.UTF8);
        }
    }
}

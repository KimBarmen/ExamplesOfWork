using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ElectronBoilerplate.Data
{
    [Serializable]
    public class SettingsDto
    {
        public SettingsValueDto[] SettingList { get; set; }
        public DateTime LastChangedDate { get; set; }
        public string PathToSettingsJson { get; set; }
    }

    [Serializable]
    public class SettingsValueDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }

    }
}

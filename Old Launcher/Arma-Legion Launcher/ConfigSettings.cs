// Type: PhoenixLoader.ConfigSettings
// Assembly: PhoenixLoader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6715B8DE-A526-4B73-BB93-5407B4838AE0
// Assembly location: D:\Desktop\Mod-Installer\Arma-Legion Launcher.exe

using System.Collections.Generic;

namespace PhoenixLoader
{
    public class ConfigSettings
    {
        public bool loadEmptyWorld = true;
        public bool stopOnDesktop = false;
        public bool ignoreIntro = true;
        public bool autoLoadAddons = false;
        public bool skipIntro = true;
        public bool useNewMaxMem = false;
        public bool useNewMaxGraphicMem = false;
        public bool useCPUCount = false;
        public bool useShowScriptErrors = false;

        public int cpuCount = 4;
        public int newMaxGraphicMem = 1024;
        public int newMaxMem = 1024;

        public List<string> activatedAddons = new List<string>();
        public List<DownloadableAddon> downloadedAddons = new List<DownloadableAddon>();
        public List<Profil> profiles = new List<Profil>();
        public List<Server> servers = new List<Server>();
        public string startParameter;
        public string path = @"C:\Program Files (x86)\Steam\steamapps\common\Arma 3";
        public string downloadPath = "http://www.legion-arma3.de/legionlauncher/addons.json";
    }
}
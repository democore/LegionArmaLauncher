// Type: PhoenixLoader.ConfigManager
// Assembly: PhoenixLoader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6715B8DE-A526-4B73-BB93-5407B4838AE0
// Assembly location: D:\Desktop\Mod-Installer\Arma-Legion Launcher.exe

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;

namespace PhoenixLoader
{
  public class ConfigManager
  {
    private static string configName = "PhoenixLauncherConfig.json";

    static ConfigManager()
    {
    }

    public static bool doesConfigExist()
    {
      return File.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + ConfigManager.configName);
    }

    public static void writeConfig(ConfigSettings settings)
    {
      try
      {
        string str1 = JsonConvert.SerializeObject((object) settings);
        string str2 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + ConfigManager.configName;
        StreamWriter streamWriter = new StreamWriter(ConfigManager.configName);
        streamWriter.Write(str1);
        streamWriter.Close();
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("Konnte Config Datei aufgrund eines Fehlers nicht schreiben.");
      }
    }

    public static ConfigSettings readConfig()
    {
      try
      {
        string str1 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + ConfigManager.configName;
        StreamReader streamReader = new StreamReader(ConfigManager.configName);
        string str2 = streamReader.ReadToEnd();
        streamReader.Close();
        return JsonConvert.DeserializeObject<ConfigSettings>(str2);
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("Konnte Config Datei aufgrund eines Fehlers nicht lesen.");
      }
      return (ConfigSettings) null;
    }

    public static void writeVersionFileForAddon(DownloadableAddon addon, string path)
    {
      string str = JsonConvert.SerializeObject((object) addon);
      if (!File.Exists(path + addon.name + "\\"))
        return;
      StreamWriter streamWriter = new StreamWriter(path + "\\" + addon.name + "\\version.json");
      streamWriter.Write(str);
      streamWriter.Close();
    }

    public static DownloadableAddon readVersionFileForAddon(string path, string name)
    {
      string path1 = path + "\\" + name + "\\version.json";
      if (!File.Exists(path1))
        return (DownloadableAddon) null;
      StreamReader streamReader = new StreamReader(path1);
      string str = streamReader.ReadToEnd();
      streamReader.Close();
      return JsonConvert.DeserializeObject<DownloadableAddon>(str);
    }

    public static Dictionary<string, int> getKeys()
    {
      StreamReader streamReader = new StreamReader(Application.GetResourceStream(new Uri("pack://application:,,,/PhoenixLoader;component/keys.txt")).Stream);
      Dictionary<string, int> dictionary = new Dictionary<string, int>();
      while (!streamReader.EndOfStream)
      {
        try
        {
          Match match = new Regex("(.+?) = (\\d+)").Match(streamReader.ReadLine());
          if (match.Groups.Count == 3)
          {
            string key = match.Groups[1].Value;
            int result = 0;
            if (int.TryParse(match.Groups[2].Value, out result))
              dictionary.Add(key, result);
          }
        }
        catch (Exception ex)
        {
        }
      }
      return dictionary;
    }

    public static int getCurrentKey(string pathToArma)
    {
      if (ConfigManager.isAcreUserConfigInstalled(pathToArma))
      {
        pathToArma = pathToArma + "\\userconfig";
        pathToArma = pathToArma + "\\acre";
        pathToArma = pathToArma + "\\acre_keys.hpp";
        Regex regex = new Regex("class PTTRadio.+?key = (\\d+)", RegexOptions.Singleline);
        string input = File.ReadAllText(pathToArma);
        if (regex.IsMatch(input))
        {
          foreach (Match match in regex.Matches(input))
          {
            int result = 0;
            if (int.TryParse(match.Groups[1].Value, out result))
              return result;
          }
        }
        else
        {
          int num = (int) MessageBox.Show("Konnte den aktuellen ACRE Key nicht finden.");
        }
      }
      return -1;
    }

    public static void setCurrentKey(string pathToArma, int key)
    {
      if (!ConfigManager.isAcreUserConfigInstalled(pathToArma))
        return;
      pathToArma = pathToArma + "\\userconfig";
      pathToArma = pathToArma + "\\acre";
      pathToArma = pathToArma + "\\acre_keys.hpp";
      Regex regex = new Regex("class PTTRadio.+?key = (?<id>\\d+)", RegexOptions.Singleline);
      string contents = RegexHelper.Replace(File.ReadAllText(pathToArma), regex, "id", string.Concat((object) key));
      File.WriteAllText(pathToArma, contents);
    }

    public static bool isAcreUserConfigInstalled(string pathToArma)
    {
      pathToArma = pathToArma + "\\userconfig";
      if (Directory.Exists(pathToArma))
      {
        pathToArma = pathToArma + "\\acre";
        if (Directory.Exists(pathToArma))
        {
          pathToArma = pathToArma + "\\acre_keys.hpp";
          if (File.Exists(pathToArma))
            return true;
        }
      }
      int num = (int) MessageBox.Show("Konnte " + pathToArma + "\\userconfig\\acre\\acre_keys.hpp nicht finden.");
      return false;
    }
  }
}

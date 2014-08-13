// Type: PhoenixLoader.RegexHelper
// Assembly: PhoenixLoader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6715B8DE-A526-4B73-BB93-5407B4838AE0
// Assembly location: D:\Desktop\Mod-Installer\Arma-Legion Launcher.exe

using System.Text.RegularExpressions;

namespace PhoenixLoader
{
  public static class RegexHelper
  {
    public static string Replace(this string input, Regex regex, string groupName, string replacement)
    {
      return regex.Replace(input, (MatchEvaluator) (m => RegexHelper.ReplaceNamedGroup(input, groupName, replacement, m)));
    }

    private static string ReplaceNamedGroup(string input, string groupName, string replacement, Match m)
    {
      return m.Value.Remove(m.Groups[groupName].Index - m.Index, m.Groups[groupName].Length).Insert(m.Groups[groupName].Index - m.Index, replacement);
    }
  }
}

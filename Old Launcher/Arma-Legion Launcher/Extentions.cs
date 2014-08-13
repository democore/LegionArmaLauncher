// Type: PhoenixLoader.Extentions
// Assembly: PhoenixLoader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6715B8DE-A526-4B73-BB93-5407B4838AE0
// Assembly location: D:\Desktop\Mod-Installer\Arma-Legion Launcher.exe

using mshtml;
using System.Windows.Controls;

namespace PhoenixLoader
{
  public static class Extentions
  {
    public static void SetZoom(this WebBrowser WebBrowser1, double Zoom)
    {
      // ISSUE: variable of a compiler-generated type
      IHTMLDocument2 htmlDocument2 = WebBrowser1.Document as IHTMLDocument2;
      // ISSUE: reference to a compiler-generated method
      htmlDocument2.parentWindow.execScript("document.body.style.zoom=" + Zoom.ToString().Replace(",", ".") + ";", "JScript");
    }
  }
}

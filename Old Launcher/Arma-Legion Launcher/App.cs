// Type: PhoenixLoader.App
// Assembly: PhoenixLoader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6715B8DE-A526-4B73-BB93-5407B4838AE0
// Assembly location: D:\Desktop\Mod-Installer\Arma-Legion Launcher.exe

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;

namespace PhoenixLoader
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public class App : Application
  {
    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
    }

    [DebuggerNonUserCode]
    [STAThread]
    public static void Main()
    {
        try
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            MessageBox.Show(ex.StackTrace);
            MessageBox.Show("INNER EXCEPTION: " + ex.InnerException.Message);
            MessageBox.Show("INNER EXCEPTION: " + ex.InnerException.StackTrace);
        }
    }
  }
}

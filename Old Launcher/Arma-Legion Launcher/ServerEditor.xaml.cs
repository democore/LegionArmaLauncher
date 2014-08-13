// Type: PhoenixLoader.ServerEditor
// Assembly: PhoenixLoader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6715B8DE-A526-4B73-BB93-5407B4838AE0
// Assembly location: D:\Desktop\Mod-Installer\Arma-Legion Launcher.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PhoenixLoader
{
  public partial class ServerEditor : Window, IComponentConnector
  {
    public bool wasAccepted = false;
    public Server server;

    public ServerEditor()
    {
      this.InitializeComponent();
      this.server = new Server();
    }

    public ServerEditor(Server server)
    {
      this.InitializeComponent();
      this.server = server;
      this.textIp.Text = server.ip;
      this.textPassword.Text = server.passwort;
      this.textServerName.Text = server.name;
      this.textPort.Text = server.port;
    }

    private void btnAccept_Click(object sender, RoutedEventArgs e)
    {
      this.wasAccepted = true;
      this.server.ip = this.textIp.Text;
      this.server.name = this.textServerName.Text;
      this.server.passwort = this.textPassword.Text;
      int result;
      if (int.TryParse(this.textPort.Text, out result))
      {
        this.server.port = this.textPort.Text;
        this.Close();
      }
      else
      {
        int num = (int) MessageBox.Show("Der Port darf nur Zahlen enthalten.");
      }
    }

    private void btnAbort_Click(object sender, RoutedEventArgs e)
    {
      this.wasAccepted = false;
      this.Close();
    }
  }
}

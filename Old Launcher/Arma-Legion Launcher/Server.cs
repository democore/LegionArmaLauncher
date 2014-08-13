// Type: PhoenixLoader.Server
// Assembly: PhoenixLoader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6715B8DE-A526-4B73-BB93-5407B4838AE0
// Assembly location: D:\Desktop\Mod-Installer\Arma-Legion Launcher.exe

namespace PhoenixLoader
{
  public class Server
  {
    public string name;
    public string ip;
    public string port;
    public string passwort;

    public Server(string name, string ip, string port, string passwort)
    {
      this.name = name;
      this.ip = ip;
      this.port = port;
      this.passwort = passwort;
    }

    public Server()
    {
    }
  }
}

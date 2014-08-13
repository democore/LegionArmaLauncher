// Type: PhoenixLoader.FtpHandler
// Assembly: PhoenixLoader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6715B8DE-A526-4B73-BB93-5407B4838AE0
// Assembly location: D:\Desktop\Mod-Installer\Arma-Legion Launcher.exe

using System;
using System.Net;

namespace PhoenixLoader
{
  public class FtpHandler : WebClient
  {
    protected override WebRequest GetWebRequest(Uri address)
    {
      FtpWebRequest ftpWebRequest = (FtpWebRequest) base.GetWebRequest(address);
      ftpWebRequest.UsePassive = true;
      ftpWebRequest.Proxy = (IWebProxy) null;
      ftpWebRequest.UseBinary = true;
      return (WebRequest) ftpWebRequest;
    }
  }
}

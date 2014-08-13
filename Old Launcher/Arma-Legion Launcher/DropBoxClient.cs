// Type: PhoenixLoader.DropBoxClient
// Assembly: PhoenixLoader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6715B8DE-A526-4B73-BB93-5407B4838AE0
// Assembly location: D:\Desktop\Mod-Installer\Arma-Legion Launcher.exe

using DropNet;
using DropNet.Models;
using System.Windows;

namespace PhoenixLoader
{
  public class DropBoxClient
  {
    private DropNetClient client;
    private UserLogin token;

    public DropBoxClient()
    {
      this.client = new DropNetClient("rr0n067pp4t69un", "cw5i32y7lf9if2z", "lquz5zyvq5qiyvnz", "1x0dqljtnambjr2");
    }

    public void downloadFiles(string fileName)
    {
      MetaData metaData = this.client.GetMetaData("/Phoenix/Acre.rar");
      byte[] numArray = new byte[metaData.Bytes];
      int num1 = 100;
      int num2 = 0;
      while (num2 < num1)
        ++num2;
      long num3 = metaData.Bytes / (long) num1;
      long startByte = 0L;
      while (startByte < metaData.Bytes)
      {
        if (startByte + num3 * 2L > metaData.Bytes)
          num3 = metaData.Bytes - startByte;
        byte[] file = this.client.GetFile("/Phoenix/Acre.rar", startByte, startByte + num3, "");
        for (int index = 0; (long) index < num3; ++index)
          numArray[startByte + (long) index] = file[index];
        startByte += num3;
        int num4 = (int) MessageBox.Show(string.Concat(new object[4]
        {
          (object) startByte,
          (object) " von ",
          (object) metaData.Bytes,
          (object) " done"
        }));
      }
    }
  }
}

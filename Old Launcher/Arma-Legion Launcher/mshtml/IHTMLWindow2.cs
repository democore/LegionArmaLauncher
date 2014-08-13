// Type: mshtml.IHTMLWindow2
// Assembly: PhoenixLoader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6715B8DE-A526-4B73-BB93-5407B4838AE0
// Assembly location: D:\Desktop\Mod-Installer\Arma-Legion Launcher.exe

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace mshtml
{
  [TypeIdentifier]
  [CompilerGenerated]
  [Guid("332C4427-26CB-11D0-B483-00C04FD90119")]
  [ComImport]
  public interface IHTMLWindow2 : IHTMLFramesCollection2
  {
    [SpecialName]
    void _VtblGap1_61();

    [DispId(1165)]
    [return: MarshalAs(UnmanagedType.Struct)]
    object execScript([MarshalAs(UnmanagedType.BStr), In] string code, [MarshalAs(UnmanagedType.BStr), In] string language = "JScript");
  }
}

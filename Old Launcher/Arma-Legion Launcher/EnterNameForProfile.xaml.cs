// Type: PhoenixLoader.EnterNameForProfile
// Assembly: PhoenixLoader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6715B8DE-A526-4B73-BB93-5407B4838AE0
// Assembly location: D:\Desktop\Mod-Installer\Arma-Legion Launcher.exe

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;

namespace PhoenixLoader
{
  public partial class EnterNameForProfile : Window, IComponentConnector
  {
    public string name;
    public DialogResult result;
    private List<string> alreadyTakenNames;

    public EnterNameForProfile(List<string> alreadyTakenNames, List<string> addons)
    {
      this.InitializeComponent();
      this.alreadyTakenNames = alreadyTakenNames;
      foreach (object newItem in addons)
        this.listAddons.Items.Add(newItem);
    }

    private void btnAccept_Click(object sender, RoutedEventArgs e)
    {
      if (this.textProfileName.Text.Length > 0)
      {
        if (!this.alreadyTakenNames.Contains(this.textProfileName.Text))
        {
            this.result = System.Windows.Forms.DialogResult.OK;
          this.name = this.textProfileName.Text;
          this.Close();
        }
        else
        {
          int num1 = (int) System.Windows.MessageBox.Show("Der gewählte Name ist bereits vergeben.");
        }
      }
      else
      {
        int num2 = (int) System.Windows.MessageBox.Show("Bitte geben sie einen Namen ein.");
      }
    }

    private void btnAbort_Click(object sender, RoutedEventArgs e)
    {
        this.result = System.Windows.Forms.DialogResult.Abort;
      this.Close();
    }
  }
}

// Type: PhoenixLoader.ProfilChanger
// Assembly: PhoenixLoader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6715B8DE-A526-4B73-BB93-5407B4838AE0
// Assembly location: D:\Desktop\Mod-Installer\Arma-Legion Launcher.exe

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;

namespace PhoenixLoader
{
  public partial class ProfilChanger : Window, IComponentConnector
  {
    public Profil profil;
    public DialogResult result;
    private string oldName;

    public ProfilChanger(Profil p, List<string> allAddons)
    {
      this.InitializeComponent();
      this.profil = p;
      this.textName.Text = p.name;
      foreach (string s in p.addons)
      {
        this.addListItem(s);
        allAddons.Remove(s);
      }
      foreach (object newItem in allAddons)
        this.cmbAddons.Items.Add(newItem);
      this.cmbAddons.SelectedIndex = 0;
      this.oldName = p.name;
    }

    private void addListItem(string s)
    {
      System.Windows.Controls.ListViewItem listViewItem = new System.Windows.Controls.ListViewItem();
      listViewItem.Content = (object) s;
      System.Windows.Controls.ContextMenu contextMenu = new System.Windows.Controls.ContextMenu();
      System.Windows.Controls.MenuItem menuItem = new System.Windows.Controls.MenuItem();
      menuItem.Header = (object) "Entfernen";
      menuItem.Click += new RoutedEventHandler(this.m_Click);
      menuItem.Tag = (object) listViewItem;
      contextMenu.Items.Add((object) menuItem);
      listViewItem.ContextMenu = contextMenu;
      this.listAddons.Items.Add((object) listViewItem);
    }

    private void m_Click(object sender, RoutedEventArgs e)
    {
      System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
      this.listAddons.Items.Remove((object) (menuItem.Tag as System.Windows.Controls.ListViewItem));
      this.cmbAddons.Items.Add((menuItem.Tag as System.Windows.Controls.ListViewItem).Content);
      this.cmbAddons.SelectedIndex = 0;
    }

    private void btnAccept_Click(object sender, RoutedEventArgs e)
    {
      if (this.textName.Text != this.oldName)
        this.profil.name = this.textName.Text;
      this.profil.addons = new List<string>();
      foreach (ContentControl contentControl in (IEnumerable) this.listAddons.Items)
        this.profil.addons.Add(contentControl.Content as string);
      this.result = System.Windows.Forms.DialogResult.OK;
      this.Close();
    }

    private void btnAbort_Click(object sender, RoutedEventArgs e)
    {
        this.result = System.Windows.Forms.DialogResult.Abort;
      this.Close();
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
      string s = this.cmbAddons.SelectedItem as string;
      if (s == null)
        return;
      this.addListItem(s);
      this.cmbAddons.Items.Remove((object) s);
      this.cmbAddons.SelectedIndex = 0;
    }
  }
}

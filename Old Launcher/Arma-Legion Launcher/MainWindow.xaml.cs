// Type: PhoenixLoader.MainWindow
// Assembly: PhoenixLoader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6715B8DE-A526-4B73-BB93-5407B4838AE0
// Assembly location: D:\Desktop\Mod-Installer\Arma-Legion Launcher.exe

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace PhoenixLoader
{
    public partial class MainWindow : Window, IComponentConnector
    {
        public string path = "";
        private string pathWithExe = "";
        private bool didLoadAddons = false;
        private List<DownloadableAddon> downloadableAddons = new List<DownloadableAddon>();
        private bool isCurrentlyRightDirectory = false;
        private int currentDownloadCount = 0;
        private bool stopDownloadingModsXml = false;
        private ConfigSettings settings;
        private Downloader downloader;

        private static String version = "1.2.6";

        public MainWindow()
        {
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.InitializeComponent();
                try
                {
                    this.settings = new ConfigSettings();
                    if (!ConfigManager.doesConfigExist())
                    {
                        ConfigManager.writeConfig(this.settings);
                        this.settings = ConfigManager.readConfig();
                    }
                    else
                        this.settings = ConfigManager.readConfig();

                    if (settings.downloadPath.Equals("http://jpjd.jp.ohost.de/legionlauncher/addons.json"))
                    {
                        settings.downloadPath = "http://www.legion-arma3.de/legionlauncher/addons.json";
                    }
                    textDownloadPath.Text = settings.downloadPath;
                    try
                    {
                        this.getDownloadableAddons();
                    }
                    catch (Exception ex)
                    {
                    }
                    this.textPath.Text = settings.path;
                    this.path = this.textPath.Text;
                    this.checkIfExeIsThere();

                    DispatcherTimer dispatcherTimer = new DispatcherTimer();
                    dispatcherTimer.Tick += new EventHandler(this.dispatcherTimer_Tick);
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
                    dispatcherTimer.Start();
                    DownloadedSettings downloadedSettings = this.downloader.downloadXML();
                    if (downloadedSettings != null)
                    {
                        //this.listEvents.SelectedIndex = 0;
                    }
                    else
                    {
                        int num = (int)System.Windows.MessageBox.Show("Konnte keine Verbindung zum Server herstellen. Bitte stellen sie sicher das kein Antivirenprogramm das Programm blockiert.");
                    }
                    this.getFieldOfView();
                    this.comboServer.SelectedIndex = 0;
                    this.setValuesFromConfig(this.settings);
                }
                catch (Exception ex)
                {
                    int num = (int)System.Windows.MessageBox.Show("Unbekannter Fehler beim starten. Bitte kontaktieren sie www.arama3-legion.de            " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                ErrorWriter.writeError("Error during loadup: " + ex.Message + " # " + ex.Source + " ### " + ex.InnerException.Message + " # " + ex.InnerException.Source);
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            /*this.checkForAddons(true);
            if (this.stopDownloadingModsXml)
                return;
            this.getDownloadableAddons();*/
        }

        private void getFieldOfView()
        {
        }

        private void setFieldOfView(int toFoV)
        {
            /*string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            if (!Directory.Exists(folderPath + "\\Arma 3\\"))
              return;
            foreach (string path in Directory.GetFiles(folderPath + "\\Arma 3\\"))
            {
              if (path.Contains(".Arma3Profile") && !path.Contains(".vars.Arma3Profile"))
              {
                string input = File.ReadAllText(path);
                Match match = new Regex("fovTop=(.+?);(\n|\r|\r\n)fovLeft=(.+?);", RegexOptions.Singleline).Match(input);
                double num1 = Convert.ToDouble(match.Groups[1].Value);
                double num2 = Convert.ToDouble(match.Groups[3].Value);
                double[] ratio = this.getRatio();
                Convert.ToInt32((this.comboFOV.SelectedItem as ComboBoxItem).Content as string);
                double num3 = this.ConvertToRadians(2.0 * Math.Atan(Math.Tan((double) (toFoV / 2)) * 1.0));
                double num4 = num1 / ratio[1] * ratio[0];
                input.Replace("fovTop=" + (object) num1, "fovTop=" + (object) num3).Replace("fovLeft=" + (object) num2, "fovLeft=" + (object) num4);
              }
            }*/
        }

        public double ConvertToRadians(double angle)
        {
            return Math.PI / 180.0 * angle;
        }

        private double[] getRatio()
        {
            int greatestCommonDivisor = this.GetGreatestCommonDivisor(Convert.ToInt32(SystemParameters.PrimaryScreenHeight), Convert.ToInt32(SystemParameters.PrimaryScreenWidth));
            return new double[2]
      {
        (double) (Convert.ToInt32(SystemParameters.PrimaryScreenHeight) / greatestCommonDivisor),
        (double) (Convert.ToInt32(SystemParameters.PrimaryScreenWidth) / greatestCommonDivisor)
      };
        }

        private int GetGreatestCommonDivisor(int a, int b)
        {
            return b == 0 ? a : this.GetGreatestCommonDivisor(b, a % b);
        }

        private bool isNewerVersionOfAddonUploaded(DownloadableAddon addon)
        {
            foreach (System.Windows.Controls.CheckBox checkBox in (IEnumerable)this.listAddons.Items)
            {
                if ((checkBox.Content as string).Equals(addon.name) && checkBox.Tag != null && checkBox.Tag is DownloadableAddon)
                    return (checkBox.Tag as DownloadableAddon).version < addon.version;
            }
            return true;
        }

        public static void SetSilent(System.Windows.Controls.WebBrowser browser, bool silent)
        {
            if (browser == null)
                throw new ArgumentNullException("browser");

            // get an IWebBrowser2 from the document
            IOleServiceProvider sp = browser.Document as IOleServiceProvider;
            if (sp != null)
            {
                Guid IID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
                Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

                object webBrowser;
                sp.QueryService(ref IID_IWebBrowserApp, ref IID_IWebBrowser2, out webBrowser);
                if (webBrowser != null)
                {
                    webBrowser.GetType().InvokeMember("Silent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty, null, webBrowser, new object[] { silent });
                }
            }
        }


        [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IOleServiceProvider
        {
            [PreserveSig]
            int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
        }

        private void getDownloadableAddons()
        {
            try
            {
                if (this.currentDownloadCount != 0)
                    return;
                List<System.Windows.Controls.CheckBox> list = new List<System.Windows.Controls.CheckBox>();
                foreach (System.Windows.Controls.CheckBox checkBox in (IEnumerable)this.listAllPheonixAddons.Items)
                    list.Add(checkBox);
                this.listAllPheonixAddons.Items.Clear();
                this.downloader = new Downloader(this);
                downloader.downloadAddonsJson(textDownloadPath.Text);
                DownloadedSettings downloadedSettings = this.downloader.downloadXML();

                if (version != downloadedSettings.version)
                {
                    System.Windows.MessageBox.Show("Es ist die neue Version " + downloadedSettings.version + " erschienen. Diese kann auf der Homepage geladen werden.");
                }
                
                if (downloadableAddons != null)
                {
                    this.downloadableAddons = downloadedSettings.addons;
                    foreach (DownloadableAddon addon in this.downloadableAddons)
                    {
                        if (true)//!this.getAllAddons().Contains(addon.fileName.Substring(0, addon.fileName.LastIndexOf("."))) && this.isNewerVersionOfAddonUploaded(addon))
                        {
                            System.Windows.Controls.CheckBox checkBox1 = new System.Windows.Controls.CheckBox();
                            foreach (System.Windows.Controls.CheckBox checkBox2 in list)
                            {
                                if ((checkBox2.Content as string).Equals(addon.name))
                                {
                                    checkBox1.IsChecked = checkBox2.IsChecked;
                                    checkBox1.IsEnabled = checkBox2.IsEnabled;
                                }
                            }
                            /*bool? isChecked = this.checkAutoUpdateAddons.IsChecked;
                            if ((!isChecked.GetValueOrDefault() ? 0 : (isChecked.HasValue ? 1 : 0)) != 0)
                                checkBox1.IsEnabled = false;*/
                            checkBox1.Content = (object)addon.name;
                            if (addon.size != 0L)
                            {
                                System.Windows.Controls.CheckBox checkBox2 = checkBox1;
                                string str = string.Concat(new object[4]
                {
                  checkBox2.Content,
                  (object) " - ",
                  (object) this.downloader.doubleToShortenedString((double) addon.size / 1000000.0),
                  (object) "MB"
                });
                                checkBox2.Content = (object)str;
                            }
                            checkBox1.Tag = (object)addon;
                            checkBox1.Foreground = (Brush)Brushes.White;
                            this.listAllPheonixAddons.Items.Add((object)checkBox1);
                            //isChecked = this.checkAutoUpdateAddons.IsChecked;
                            /*if ((!isChecked.GetValueOrDefault() ? 0 : (isChecked.HasValue ? 1 : 0)) != 0 && this.isCurrentlyRightDirectory)
                            {
                                this.downloader.downloadAddon(addon);
                                ++this.currentDownloadCount;
                            }*/
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.stopDownloadingModsXml = true;
            }
        }

        private void setValuesFromConfig(ConfigSettings settings)
        {
            this.checkEmptyWorld.IsChecked = new bool?(settings.loadEmptyWorld);
            this.checkIgnoreIntro.IsChecked = new bool?(settings.ignoreIntro);
            this.checkStopOnDesktop.IsChecked = new bool?(settings.stopOnDesktop);
            //this.checkAutoUpdateAddons.IsChecked = new bool?(settings.autoLoadAddons);
            if (settings.activatedAddons == null)
                settings.activatedAddons = new List<string>();

            if (this.didLoadAddons)
                this.activateAddons(settings.activatedAddons);

            foreach (Profil p in settings.profiles)
                this.addProfil(p);
            this.textStartParas.Text = settings.startParameter;
            if (settings.path != "" && settings.path != null)
                this.textPath.Text = settings.path;
            foreach (Server s in settings.servers)
                this.addServer(s);


            this.checkNewMaxMbArbeitsspeicher.IsChecked = new bool?(settings.useNewMaxMem);
            this.textMaxMb.Text = settings.newMaxMem.ToString();

            this.checkGrafikMb.IsChecked = new bool?(settings.useNewMaxGraphicMem);
            this.textGrafikMb.Text = settings.newMaxGraphicMem.ToString();

            this.checkCPUCount.IsChecked = new bool?(settings.useCPUCount);
            this.textCPUCount.Text = settings.cpuCount.ToString();

            this.checkShowScriptErrors.IsChecked = new bool?(settings.useShowScriptErrors);

            this.textDownloadPath.Text = settings.downloadPath;
        }

        public void addServer(Server s)
        {
            System.Windows.Controls.ListViewItem listViewItem = new System.Windows.Controls.ListViewItem();
            listViewItem.Foreground = (Brush)Brushes.White;
            listViewItem.Content = (object)s.name;
            listViewItem.Tag = (object)s;
            listViewItem.ContextMenu = new System.Windows.Controls.ContextMenu();
            System.Windows.Controls.MenuItem menuItem1 = new System.Windows.Controls.MenuItem();
            menuItem1.Header = (object)"Bearbeiten";
            menuItem1.Click += new RoutedEventHandler(this.differServer_Click);
            menuItem1.Tag = (object)s;
            System.Windows.Controls.MenuItem menuItem2 = new System.Windows.Controls.MenuItem();
            menuItem2.Header = (object)"Löschen";
            menuItem2.Click += new RoutedEventHandler(this.mi_Click);
            menuItem2.Tag = (object)s;
            listViewItem.ContextMenu.Items.Add((object)menuItem1);
            listViewItem.ContextMenu.Items.Add((object)menuItem2);
            ComboBoxItem comboBoxItem = new ComboBoxItem();
            comboBoxItem.Content = (object)s.name;
            comboBoxItem.Tag = (object)s;
            this.comboServer.Items.Add((object)comboBoxItem);
            this.listServer.Items.Add((object)listViewItem);
        }

        private void differServer_Click(object sender, RoutedEventArgs e)
        {
            Server server1 = (sender as System.Windows.Controls.MenuItem).Tag as Server;
            ServerEditor serverEditor = new ServerEditor(server1);
            serverEditor.ShowDialog();
            if (!serverEditor.wasAccepted)
                return;
            this.settings.servers.Remove(server1);
            this.settings.servers.Add(serverEditor.server);
            System.Windows.Controls.ListViewItem serverItem = this.getServerItem(server1);
            foreach (ComboBoxItem comboBoxItem in (IEnumerable)this.comboServer.Items)
            {
                if (comboBoxItem.Tag == server1)
                {
                    comboBoxItem.Tag = (object)serverEditor.server;
                    comboBoxItem.Content = (object)serverEditor.server.name;
                    break;
                }
            }
            Server server2 = serverEditor.server;
            serverItem.Content = (object)server2.name;
            foreach (FrameworkElement frameworkElement in (IEnumerable)serverItem.ContextMenu.Items)
                frameworkElement.Tag = (object)server2;
        }

        private System.Windows.Controls.ListViewItem getServerItem(Server s)
        {
            System.Windows.Controls.ListViewItem listViewItem1 = (System.Windows.Controls.ListViewItem)null;
            foreach (System.Windows.Controls.ListViewItem listViewItem2 in (IEnumerable)this.listServer.Items)
            {
                if (listViewItem2.Tag == s)
                {
                    listViewItem1 = listViewItem2;
                    break;
                }
            }
            return listViewItem1;
        }

        private void mi_Click(object sender, RoutedEventArgs e)
        {
            Server s = (sender as System.Windows.Controls.MenuItem).Tag as Server;
            this.settings.servers.Remove(s);
            System.Windows.Controls.ListViewItem serverItem = this.getServerItem(s);
            if (serverItem != null)
                this.listServer.Items.Remove((object)serverItem);
            ComboBoxItem comboBoxItem1 = (ComboBoxItem)null;
            foreach (ComboBoxItem comboBoxItem2 in (IEnumerable)this.comboServer.Items)
            {
                Server server = comboBoxItem2.Tag as Server;
                if (server != null && (server.name == s.name && server.ip == s.ip))
                {
                    comboBoxItem1 = comboBoxItem2;
                    break;
                }
            }
            this.comboServer.Items.Remove((object)comboBoxItem1);
        }

        private bool doesProfileExist(string name)
        {
            return this.getAllProfiles().Contains(name);
        }

        private void addProfil(Profil p)
        {
            if (!this.doesProfileExist(p.name))
            {
                System.Windows.Controls.ListViewItem listViewItem = new System.Windows.Controls.ListViewItem();
                listViewItem.Content = (object)p.name;
                listViewItem.Tag = (object)p;
                System.Windows.Controls.ContextMenu contextMenu = new System.Windows.Controls.ContextMenu();
                System.Windows.Controls.MenuItem menuItem = new System.Windows.Controls.MenuItem();
                menuItem.Header = (object)"Löschen";
                menuItem.Click += new RoutedEventHandler(this.item_Click);
                contextMenu.Items.Add((object)menuItem);
                listViewItem.ContextMenu = contextMenu;
                this.listProfiles.Items.Add((object)listViewItem);
            }
            else
            {
                foreach (System.Windows.Controls.ListViewItem listViewItem in (IEnumerable)this.listProfiles.Items)
                {
                    if ((listViewItem.Tag as Profil).name == p.name)
                    {
                        listViewItem.Tag = (object)p;
                        break;
                    }
                }
            }
        }

        private void item_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.ListViewItem listViewItem = this.listProfiles.SelectedItem as System.Windows.Controls.ListViewItem;
            this.listAddonsInProfile.Items.Clear();
            this.listProfiles.Items.Remove((object)listViewItem);
            this.settings.profiles.Remove(listViewItem.Tag as Profil);
            ConfigManager.writeConfig(this.settings);
            this.checkForAddons(true);
            this.getDownloadableAddons();
        }

        private void activateAddons(List<string> names)
        {
            foreach (ToggleButton toggleButton in (IEnumerable)this.listAddons.Items)
                toggleButton.IsChecked = new bool?(false);
            foreach (string str in names)
            {
                foreach (System.Windows.Controls.CheckBox checkBox in (IEnumerable)this.listAddons.Items)
                {
                    if (checkBox.Content.Equals((object)str))
                        checkBox.IsChecked = new bool?(true);
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            if (folderBrowserDialog.SelectedPath.Length <= 0)
                return;
            this.path = folderBrowserDialog.SelectedPath;
            this.textPath.Text = this.path;
        }

        private void textPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.path = this.textPath.Text;
            this.checkIfExeIsThere();
        }

        private void checkIfExeIsThere()
        {
            this.pathWithExe = this.path + "\\Arma3.exe";
            if (File.Exists(this.pathWithExe))
            {
                this.isCurrentlyRightDirectory = true;

                if (this.imageKreuz != null)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri("pack://application:,,,/PhoenixLoader;component/Images/icon_found.png");
                    bitmapImage.EndInit();
                    this.imageKreuz.Source = (ImageSource)bitmapImage;
                }
                this.checkForAddons(false);
            }
            else
            {
                this.isCurrentlyRightDirectory = false;
                if (this.imageKreuz != null)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri("pack://application:,,,/PhoenixLoader;component/Images/icon_delete.png");
                    bitmapImage.EndInit();
                    this.imageKreuz.Source = (ImageSource)bitmapImage;
                }
            }
        }

        private void checkForAddons(bool force)
        {
            if (listAddons == null)
            {
                //System.Windows.MessageBox.Show("Error could not load installed mods");
                //listAddons = new System.Windows.Controls.ListBox();
            }
            if (this.didLoadAddons && !force || !this.isCurrentlyRightDirectory)
                return;

            List<DirectoryInfo> list1;
            DirectoryInfo directoryInfo1;
            List<System.Windows.Controls.CheckBox> list2;
            List<String> allActiveMods;
            try
            {
                list1 = new List<DirectoryInfo>();
                directoryInfo1 = new DirectoryInfo(this.path);
                list2 = new List<System.Windows.Controls.CheckBox>();
                allActiveMods = new List<string>();
                try
                {
                    foreach (System.Windows.Controls.CheckBox checkBox in (IEnumerable)this.listAddons.Items)
                    {
                        bool flag = false;
                        foreach (FileSystemInfo fileSystemInfo in directoryInfo1.GetDirectories())
                        {
                            if (((object)fileSystemInfo.Name).Equals(checkBox.Content))
                            {
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            list2.Add(checkBox);
                        }
                        if (checkBox.IsChecked == true)
                        {
                            allActiveMods.Add(checkBox.Content as String);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorWriter.writeError( "first inner exception: " + ex.Message + " " + ex.StackTrace + " " + ex.Source);
                }
                try
                {
                    foreach (object removeItem in list2)
                        this.listAddons.Items.Remove(removeItem);
                    foreach (DirectoryInfo directoryInfo2 in directoryInfo1.GetDirectories())
                    {
                        if (directoryInfo2.Name.Substring(0, 1) == "@" && !this.getAllAddons().Contains(directoryInfo2.Name))
                        {
                            System.Windows.Controls.CheckBox checkBox = new System.Windows.Controls.CheckBox();
                            System.Windows.Controls.ContextMenu contextMenu = new System.Windows.Controls.ContextMenu();
                            System.Windows.Controls.MenuItem menuItem = new System.Windows.Controls.MenuItem();
                            menuItem.Header = (object)"Löschen";
                            menuItem.Click += new RoutedEventHandler(this.addonDeleteClick);
                            menuItem.Tag = (object)checkBox;
                            contextMenu.Items.Add((object)menuItem);
                            checkBox.ContextMenu = contextMenu;
                            DownloadableAddon downloadableAddon = ConfigManager.readVersionFileForAddon(this.path, directoryInfo2.Name);
                            checkBox.Tag = (object)downloadableAddon;
                            checkBox.Foreground = (Brush)Brushes.White;
                            checkBox.Content = (object)directoryInfo2.Name;
                            list1.Add(directoryInfo2);
                            this.listAddons.Items.Add((object)checkBox);
                            checkBox.Checked += new RoutedEventHandler(this.c_Checked);
                            checkBox.Unchecked += new RoutedEventHandler(this.c_Unchecked);
                        }
                    }
                    this.activateAddons(allActiveMods);
                    this.didLoadAddons = true;
                }
                catch (Exception ex)
                {
                    ErrorWriter.writeError("second inner exception: " + ex.Message + " " + ex.StackTrace + " " + ex.Source);
                }
            }
            catch (Exception ex)
            {
                ErrorWriter.writeError("Error in CheckForAddons() (first)" +  ex.Message + " " + ex.StackTrace);
            }

           
        }

        private void addonDeleteClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is System.Windows.Controls.MenuItem))
                return;
            System.Windows.Controls.CheckBox checkBox = (sender as System.Windows.Controls.MenuItem).Tag as System.Windows.Controls.CheckBox;
            if (this.downloader != null)
            {
                this.downloader.removeAddon(checkBox.Content as string);
            }
            else
            {
                int num = (int)System.Windows.MessageBox.Show("Konnte Addon nicht löschen, da keine verbindung zum server vorhanden ist.");
            }
        }

        private void c_Unchecked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox checkBox = sender as System.Windows.Controls.CheckBox;
            if (!Enumerable.Contains<object>((IEnumerable<object>)this.settings.activatedAddons, checkBox.Content))
                return;
            this.settings.activatedAddons.Remove(checkBox.Content as string);
        }

        private void c_Checked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox checkBox = sender as System.Windows.Controls.CheckBox;
            if (Enumerable.Contains<object>((IEnumerable<object>)this.settings.activatedAddons, checkBox.Content))
                return;
            this.settings.activatedAddons.Add(checkBox.Content as string);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!this.isCurrentlyRightDirectory || !File.Exists(this.pathWithExe))
                    return;
                Process process = new Process();
                process.StartInfo.FileName = this.pathWithExe;
                bool? nullable = this.checkStopOnDesktop.IsChecked;
                nullable = nullable.HasValue ? new bool?(!nullable.GetValueOrDefault()) : new bool?();
                if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
                {
                    ProcessStartInfo startInfo = process.StartInfo;
                    string str = startInfo.Arguments + "-noPause ";
                    startInfo.Arguments = str;
                }
                nullable = this.checkIgnoreIntro.IsChecked;
                if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
                {
                    ProcessStartInfo startInfo = process.StartInfo;
                    string str = startInfo.Arguments + "-noSplash ";
                    startInfo.Arguments = str;
                }
                nullable = this.checkEmptyWorld.IsChecked;
                if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
                {
                    ProcessStartInfo startInfo = process.StartInfo;
                    string str = startInfo.Arguments + "-world=empty ";
                    startInfo.Arguments = str;
                }
                if (checkNewMaxMbArbeitsspeicher.IsChecked == true)
                {
                    ProcessStartInfo startInfo = process.StartInfo;
                    string str = startInfo.Arguments + "-maxMem=" + textMaxMb.Text + " ";
                    startInfo.Arguments = str;
                }
                if (this.checkGrafikMb.IsChecked == true)
                {
                    ProcessStartInfo startInfo = process.StartInfo;
                    string str = startInfo.Arguments + "-maxVRAM=" + textGrafikMb.Text + " ";
                    startInfo.Arguments = str;
                }
                if (this.checkCPUCount.IsChecked == true)
                {
                    ProcessStartInfo startInfo = process.StartInfo;
                    string str = startInfo.Arguments + "-cpuCount=" + textCPUCount.Text + " ";
                    startInfo.Arguments = str;
                }
                if (this.checkShowScriptErrors.IsChecked == true)
                {
                    ProcessStartInfo startInfo = process.StartInfo;
                    string str = startInfo.Arguments + "-showScriptErrors ";
                    startInfo.Arguments = str;
                }

                ProcessStartInfo startInfo1 = process.StartInfo;
                string str1 = startInfo1.Arguments + this.textStartParas.Text + " ";
                startInfo1.Arguments = str1;
                bool flag = false;
                foreach (System.Windows.Controls.CheckBox checkBox in (IEnumerable)this.listAddons.Items)
                {
                    nullable = checkBox.IsChecked;
                    if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
                    {
                        if (!flag)
                        {
                            ProcessStartInfo startInfo2 = process.StartInfo;
                            string str2 = startInfo2.Arguments + "\"-mod=";
                            startInfo2.Arguments = str2;
                        }
                        else
                        {
                            ProcessStartInfo startInfo2 = process.StartInfo;
                            string str2 = startInfo2.Arguments + ";";
                            startInfo2.Arguments = str2;
                        }
                        ProcessStartInfo startInfo3 = process.StartInfo;
                        string str3 = startInfo3.Arguments + checkBox.Content;
                        startInfo3.Arguments = str3;
                        flag = true;
                    }
                }
                if (flag)
                {
                    ProcessStartInfo startInfo2 = process.StartInfo;
                    string str2 = startInfo2.Arguments + "\" ";
                    startInfo2.Arguments = str2;
                }
                if (this.comboServer.SelectedIndex != 0)
                {
                    Server server = (this.comboServer.SelectedItem as ComboBoxItem).Tag as Server;
                    ProcessStartInfo startInfo2 = process.StartInfo;
                    string str2 = startInfo2.Arguments + "-connect=" + server.ip + " -port=" + server.port + " -password=" + server.passwort;
                    startInfo2.Arguments = str2;
                }
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Verb = "runas";
                process.Start();
            }
            catch (Exception ex)
            {
                int num = (int)System.Windows.MessageBox.Show("Konnte Arma3 nicht starten. (" + ex.Message + ")");
            }
        }

        private void Phoenix_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://www.legion-arma3.de");
        }

        private void tabItem1_Loaded(object sender, RoutedEventArgs e)
        {
            newsWebBrowser.LoadCompleted += new LoadCompletedEventHandler(newsWebBrowser_LoadCompleted);
            newsWebBrowser.Navigate("http://www.legion-arma3.de");
        }

        private void newsWebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            //Extentions.SetZoom(this.newsWebBrowser, 1);
        }

        private void checkAutoUpdateAddons_Checked(object sender, RoutedEventArgs e)
        {
            this.settings.autoLoadAddons = true;
            ConfigManager.writeConfig(this.settings);
            foreach (UIElement uiElement in (IEnumerable)this.listAllPheonixAddons.Items)
                uiElement.IsEnabled = false;
        }

        private void checkAutoUpdateAddons_Unchecked(object sender, RoutedEventArgs e)
        {
            this.settings.autoLoadAddons = false;
            ConfigManager.writeConfig(this.settings);
            foreach (UIElement uiElement in (IEnumerable)this.listAllPheonixAddons.Items)
                uiElement.IsEnabled = true;
        }

        private void btnAcceptSettingChanges_Click(object sender, RoutedEventArgs e)
        {
            ConfigSettings configSettings1 = this.settings;
            /*bool? isChecked = this.checkAutoUpdateAddons.IsChecked;
            int num1 = !isChecked.GetValueOrDefault() ? 0 : (isChecked.HasValue ? 1 : 0);
            configSettings1.autoLoadAddons = num1 != 0;*/
            ConfigSettings configSettings2 = this.settings;
            bool? isChecked = this.checkIgnoreIntro.IsChecked;
            int num2 = !isChecked.GetValueOrDefault() ? 0 : (isChecked.HasValue ? 1 : 0);
            configSettings2.ignoreIntro = num2 != 0;
            ConfigSettings configSettings3 = this.settings;
            isChecked = this.checkEmptyWorld.IsChecked;
            int num3 = !isChecked.GetValueOrDefault() ? 0 : (isChecked.HasValue ? 1 : 0);
            configSettings3.loadEmptyWorld = num3 != 0;
            ConfigSettings configSettings4 = this.settings;
            isChecked = this.checkStopOnDesktop.IsChecked;
            int num4 = !isChecked.GetValueOrDefault() ? 0 : (isChecked.HasValue ? 1 : 0);
            configSettings4.stopOnDesktop = num4 != 0;
            this.settings.startParameter = this.textStartParas.Text;

            this.settings.useNewMaxMem = (checkNewMaxMbArbeitsspeicher.IsChecked == true);
            this.settings.newMaxMem = int.Parse(textMaxMb.Text);

            this.settings.useNewMaxGraphicMem = (checkGrafikMb.IsChecked == true);
            this.settings.newMaxGraphicMem = int.Parse(textGrafikMb.Text);


            this.settings.useCPUCount = (checkCPUCount.IsChecked == true);
            this.settings.cpuCount = int.Parse(textCPUCount.Text);

            this.settings.useShowScriptErrors = (checkShowScriptErrors.IsChecked == true);

            this.settings.downloadPath = textDownloadPath.Text;

            ConfigManager.writeConfig(this.settings);
            //ConfigManager.setCurrentKey(this.path, (int) (this.comboAcrePtt.SelectedItem as ComboBoxItem).Tag);
        }

        private void btnAddProfile_Click(object sender, RoutedEventArgs e_)
        {
            Profil p = new Profil();
            p.addons = this.getSelectedAddons();
            EnterNameForProfile enterNameForProfile = new EnterNameForProfile(this.getAllProfiles(), p.addons);
            enterNameForProfile.ShowDialog();
            if (enterNameForProfile.result != System.Windows.Forms.DialogResult.OK)
                return;
            p.name = enterNameForProfile.name;
            this.settings.profiles.Add(p);
            this.addProfil(p);
        }

        private List<string> getAllProfiles()
        {
            List<string> list = new List<string>();
            foreach (System.Windows.Controls.ListViewItem listViewItem in (IEnumerable)this.listProfiles.Items)
                list.Add(listViewItem.Content as string);
            return list;
        }

        private List<string> getSelectedAddons()
        {
            List<string> list = new List<string>();
            foreach (System.Windows.Controls.CheckBox checkBox in (IEnumerable)this.listAddons.Items)
            {
                bool? isChecked = checkBox.IsChecked;
                if ((!isChecked.GetValueOrDefault() ? 0 : (isChecked.HasValue ? 1 : 0)) != 0)
                    list.Add(checkBox.Content as string);
            }
            return list;
        }

        private List<string> getAllAddons()
        {
            List<string> list = new List<string>();
            foreach (System.Windows.Controls.CheckBox checkBox in (IEnumerable)this.listAddons.Items)
                list.Add(checkBox.Content as string);
            return list;
        }

        private void listProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.listAddonsInProfile.Items.Clear();
            System.Windows.Controls.ListViewItem listViewItem = this.listProfiles.SelectedItem as System.Windows.Controls.ListViewItem;
            if (listViewItem == null || !(listViewItem.Tag is Profil))
                return;
            foreach (object newItem in (listViewItem.Tag as Profil).addons)
                this.listAddonsInProfile.Items.Add(newItem);
        }

        private void btnActivateProfile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.ListViewItem listViewItem = this.listProfiles.SelectedItem as System.Windows.Controls.ListViewItem;
            if (listViewItem == null || !(listViewItem.Tag is Profil))
                return;
            this.activateAddons((listViewItem.Tag as Profil).addons);
        }

        private void btnEditProfile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.ListViewItem listViewItem = this.listProfiles.SelectedItem as System.Windows.Controls.ListViewItem;
            if (listViewItem == null || !(listViewItem.Tag is Profil))
                return;
            ProfilChanger profilChanger = new ProfilChanger(listViewItem.Tag as Profil, this.getAllAddons());
            profilChanger.ShowDialog();
            if (profilChanger.result == System.Windows.Forms.DialogResult.OK)
            {
                Profil profil = profilChanger.profil;
                listViewItem.Content = (object)profil.name;
                this.listAddonsInProfile.Items.Clear();
                foreach (object newItem in profil.addons)
                    this.listAddonsInProfile.Items.Add(newItem);
                ConfigManager.writeConfig(this.settings);
            }
        }

        private void checkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (ToggleButton toggleButton in (IEnumerable)this.listAddons.Items)
                toggleButton.IsChecked = new bool?(true);
        }

        private void checkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (ToggleButton toggleButton in (IEnumerable)this.listAddons.Items)
                toggleButton.IsChecked = new bool?(false);
        }

        private void btnDownloadSelectedAddonsNow_Click(object sender, RoutedEventArgs e)
        {
            bool? isChecked;
            int num;
            if (this.isCurrentlyRightDirectory)
            {
                //isChecked = this.checkAutoUpdateAddons.IsChecked;
                //num = (isChecked.GetValueOrDefault() ? 0 : (isChecked.HasValue ? 1 : 0)) == 0 ? 1 : 0;
            }
            else
                num = 1;
            //if (num != 0)
                //return;
            foreach (System.Windows.Controls.CheckBox checkBox in (IEnumerable)this.listAllPheonixAddons.Items)
            {
                isChecked = checkBox.IsChecked;
                if ((!isChecked.GetValueOrDefault() ? 0 : (isChecked.HasValue ? 1 : 0)) != 0)
                {
                    this.downloader.downloadAddon(checkBox.Tag as DownloadableAddon);
                    ++this.currentDownloadCount;
                }
            }
        }

        public void reportProgress(DownloadableAddon addon, string percentage, bool done)
        {
            foreach (System.Windows.Controls.CheckBox checkBox in (IEnumerable)this.listAllPheonixAddons.Items)
            {
                if ((checkBox.Content as string).Equals(addon.name) || (checkBox.Tag as DownloadableAddon).name.Equals(addon.name))
                {
                    if (percentage != " Fertig.")
                    {
                        checkBox.IsChecked = false;
                        checkBox.IsEnabled = false;
                        checkBox.Content = (object)(addon.name + " - " + percentage);
                        if (done)
                            checkBox.Content = (object)(addon.name + " - Entpacke...");
                    }
                    else
                    {
                        checkBox.IsChecked = false;
                        checkBox.IsEnabled = false;
                        checkBox.Content = addon.name + " Fertig.";
                    }
                }
            }
        }

        public void addonWasSuccesfullyExtracted(DownloadableAddon addon)
        {
            --this.currentDownloadCount;
            System.Windows.Controls.CheckBox checkBox1 = (System.Windows.Controls.CheckBox)null;
            foreach (System.Windows.Controls.CheckBox checkBox2 in (IEnumerable)this.listAllPheonixAddons.Items)
            {
                if ((checkBox2.Tag as DownloadableAddon).name.Equals(addon.fileName))
                {
                    checkBox1 = checkBox2;
                    break;
                }
            }
            if (checkBox1 == null)
                return;
            this.checkForAddons(true);
            this.getDownloadableAddons();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.settings.path = this.path;
            ConfigManager.writeConfig(this.settings);
        }

        private bool areAddonsExisting(List<DownloadableAddon> addons, bool downloadIfNotExisting)
        {
            bool flag = true;
            List<DownloadableAddon> list = new List<DownloadableAddon>();
            List<string> allAddons = this.getAllAddons();
            foreach (DownloadableAddon downloadableAddon in addons)
            {
                if (!allAddons.Contains(downloadableAddon.name))
                {
                    if (!downloadIfNotExisting)
                        return false;
                    list.Add(downloadableAddon);
                    flag = false;
                }
            }
            foreach (DownloadableAddon addon in list)
            {
                this.downloader.downloadAddon(addon);
                ++this.currentDownloadCount;
                this.tabMain.SelectedIndex = 1;
            }
            return flag;
        }

        private void loadAcreKeysIntoGroupBox(Dictionary<string, int> keys)
        {
            /*int num1 = 0;
            int num2 = 0;
            int currentKey = ConfigManager.getCurrentKey(this.path);
            foreach (KeyValuePair<string, int> keyValuePair in keys)
            {
              ComboBoxItem comboBoxItem = new ComboBoxItem();
              comboBoxItem.Content = (object) keyValuePair.Key;
              comboBoxItem.Tag = (object) keyValuePair.Value;
              this.comboAcrePtt.Items.Add((object) comboBoxItem);
              if (keyValuePair.Value == currentKey)
                num2 = num1;
              ++num1;
            }
            this.comboAcrePtt.SelectedIndex = num2;
            if (this.isCurrentlyRightDirectory && currentKey != -1)
              return;
            this.comboAcrePtt.IsEnabled = false;*/
        }

        private void tabMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.tabMain.SelectedIndex != 3)
                return;
            this.loadAcreKeysIntoGroupBox(ConfigManager.getKeys());
        }

        private void btnAddServer_Click(object sender, RoutedEventArgs e)
        {
            ServerEditor serverEditor = new ServerEditor();
            serverEditor.ShowDialog();
            if (!serverEditor.wasAccepted)
                return;
            this.settings.servers.Add(serverEditor.server);
            this.addServer(serverEditor.server);
        }

        private void newsWebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            SetSilent(newsWebBrowser, true); // make it silent
        }

        public void AllowUIToUpdate()
        {

            DispatcherFrame frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate(object parameter)
            {

                frame.Continue = false;

                return null;

            }), null);

            Dispatcher.PushFrame(frame);

        }

        private void textMaxMb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            
                Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
                return !regex.IsMatch(text);
        }

        private void textMaxMb_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void textMaxMb_LostFocus(object sender, RoutedEventArgs e)
        {
            int bla = 0;
            if (int.TryParse(textMaxMb.Text, out bla))
            {
                if (bla > 2047)
                {
                    textMaxMb.Text = "2047";
                }
                if (bla < 256)
                {
                    textMaxMb.Text = "256";
                }
            }
        }

        private void btnReloadMods_Click(object sender, RoutedEventArgs e)
        {
            this.checkForAddons(true); 
            if (this.stopDownloadingModsXml)
                return;
            this.getDownloadableAddons();
        }
    }
}

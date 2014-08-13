// Type: PhoenixLoader.Downloader
// Assembly: PhoenixLoader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6715B8DE-A526-4B73-BB93-5407B4838AE0
// Assembly location: D:\Desktop\Mod-Installer\Arma-Legion Launcher.exe

using Ionic.Zip;
using SharpCompress.Common;
using SharpCompress.Reader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Xml;
using Newtonsoft.Json;
using SharpCompress.Archive.SevenZip;
using SharpCompress.Archive;
using Microsoft.Win32;
using System.Windows.Threading;

namespace PhoenixLoader
{
    public class Downloader
    {
        private int i = 0;
        private string repo = "ftp://178.199.210.228/";
        private string xml = "LegionGuest/Launcher/Mods.xml";
        private string user = "LegionGuest";
        private string password = "LegionGuest";
        private string downloadPath = "";
        private bool useFtp = true;
        private int downloadCount = 0;
        private string lasDownloadedXml = "";
        private MainWindow main;
        private WebClient webClient;

        Dictionary<WebClient, NetSpeedCounter> speedCounter = new Dictionary<WebClient, NetSpeedCounter>();
        long bytesInLastSecond = 0;
        long currentSecond = 0;
        long[] bytesInLastFifeSeconds;

        public Downloader(MainWindow main)
        {
            this.main = main;

            bytesInLastFifeSeconds = new long[5];

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            /*DispatcherTimer timer = (DispatcherTimer)sender;
            bytesInLastSecond = bytesInLastSecond / (1000 * 1000);
            main.labelKbs.Content = "Downloadgeschwindigkeit: " + Average(bytesInLastSecond) + " kb/s";
            bytesInLastFifeSeconds[currentSecond] = bytesInLastSecond;
            bytesInLastSecond = 0;

            currentSecond++;
            if (currentSecond > 4)
            {
                currentSecond = 0;
            }*/


            double d = 0;
            foreach (KeyValuePair<WebClient, NetSpeedCounter> counter in speedCounter)
            {
                d += counter.Value.Speed;
            }
            main.labelKbs.Content = "Downloadgeschwindigkeit: " + doubleToShortenedString(d) + " kb/s";
        }

        public long Sum(params long[] customerssalary)
        {
            long result = 0;

            for (int i = 0; i < customerssalary.Length; i++)
            {
                result += customerssalary[i];
            }

            return result;
        }

        public decimal Average(params long[] customerssalary)
        {
            long sum = Sum(customerssalary);
            decimal result = (decimal)sum / customerssalary.Length;
            return result;
        }

        public bool isConnectedToInternet()
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    using (webClient.OpenRead("http://www.google.com"))
                        return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool downloadAddonsJson(String downloadPath)
        {
            this.webClient = new WebClient();
            this.webClient.Proxy = (IWebProxy)null;

            string downloadLocation = System.AppDomain.CurrentDomain.FriendlyName;
            downloadLocation = downloadLocation.Replace("vshost.", "");
            String path = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(downloadLocation, "addons.json");

            webClient.DownloadFile(downloadPath, path);

            return File.Exists(path);
        }

        public DownloadedSettings downloadXML()
        {
            Exception exception;
            DownloadedSettings m = null;
            try
            {
                bool flag = false;
                ++this.downloadCount;
                flag = true;
                this.webClient = new WebClient();
                this.webClient.Proxy = (IWebProxy)null;
                try
                {
                    String path = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("PhoenixLoader.exe", "addons.json");
                    String json = File.ReadAllText(path);
                    m = JsonConvert.DeserializeObject<DownloadedSettings>(json);
                }
                catch (Exception ex)
                {

                    String path = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(System.AppDomain.CurrentDomain.FriendlyName, "addons.json");
                    String json = File.ReadAllText(path);
                    m = JsonConvert.DeserializeObject<DownloadedSettings>(json);
                }

                
            }
            catch (Exception ex)
            {
                exception = ex;
                int num = (int)MessageBox.Show("Fehler beim lesen der Mods.xml.");
            }
            return m;
        }

        public void downloadAddon(DownloadableAddon addon)
        {
            bool flag = false;
            string str = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\";
            flag = false;
            WebClient webClient = (WebClient)new WebClient();
            webClient.Proxy = (IWebProxy)null;
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.webClient_DownloadFileCompleted);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.webClient_DownloadProgressChanged);
            webClient.DownloadFileAsync(new Uri(addon.Link), str + System.IO.Path.GetFileName(addon.Link), (object)addon);

            NetSpeedCounter counter = new NetSpeedCounter(webClient);
            speedCounter.Add(webClient, counter);
            counter.Start();
        }

        public string doubleToShortenedString(double d)
        {
            string str = d.ToString();
            int num = 0;
            if (str.Contains(","))
                num = str.IndexOf(",");
            if (str.Length > num + 3)
                str = d.ToString().Substring(0, num + 3);
            return str;
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            /*if (e.TotalBytesToReceive - e.BytesReceived >= 0)
                bytesInLastSecond += Convert.ToInt64(e.TotalBytesToReceive - e.BytesReceived);*/


            double d1 = (double)(e.BytesReceived / 1000L) / 1000.0;
            string str1 = this.doubleToShortenedString(d1);
            double d2 = (double)(e.TotalBytesToReceive / 1000L) / 1000.0;
            string str2 = this.doubleToShortenedString(d2);
            if (d2 != 0.0)
            {
                double d3 = 100.0 / d2 * d1;
                this.main.reportProgress(e.UserState as DownloadableAddon, str1 + " MB / " + str2 + " MB (" + this.doubleToShortenedString(d3) + "%)", 0 != 0);
            }
            else
                this.main.reportProgress(e.UserState as DownloadableAddon, str1 + " MB / " + str2 + " MB", false);
        }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            WebClient w = (WebClient)sender;
            speedCounter.Remove(w);

            this.main.reportProgress(e.UserState as DownloadableAddon, "Entpacke...", true);
            this.main.AllowUIToUpdate();
            DownloadableAddon addon = e.UserState as DownloadableAddon;
            addon.fileName = System.IO.Path.GetFileName(addon.Link);
            string extension = Path.GetExtension(addon.fileName);
            if (extension == ".zip" && false)
            {
                this.extractZip(addon);
                this.removeDownloadedAddon(addon);
            }
            else 
            {
                this.extractRar(addon, extension);
            }
            /*else
            {
                int num = (int)MessageBox.Show("Das addon " + addon.fileName + " hat ein unbekanntes dateiformat. (Es werden nur .zip und .rar erkannt");
            }*/
            ConfigManager.writeVersionFileForAddon(addon, this.main.path);
        }

        public void extractZip(DownloadableAddon addon)
        {
            try
            {
                string fileName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + addon.fileName;
                string baseDirectory = this.main.path;
                using (ZipFile zipFile = ZipFile.Read(fileName))
                {
                    foreach (ZipEntry zipEntry in zipFile)
                        zipEntry.Extract(baseDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
                this.main.addonWasSuccesfullyExtracted(addon);
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show("Fehler beim entpacken: " + ex.Message);
                this.main.reportProgress(addon, " - Fehler beim entpacken.", true);
            }
        }







        Dictionary<IArchive, DownloadableAddon> archiveToAddon = new Dictionary<IArchive, DownloadableAddon>();
        Dictionary<BackgroundWorker, DownloadableAddon> backgroundWorkerToAddon = new Dictionary<BackgroundWorker, DownloadableAddon>();
        Dictionary<DownloadableAddon, BackgroundWorker> addonToBackgroundDownloader = new Dictionary<DownloadableAddon, BackgroundWorker>();
        Dictionary<BackgroundWorker, IArchive> backGroundWorkerToArchive = new Dictionary<BackgroundWorker, IArchive>();
        Dictionary<DownloadableAddon, long> bytesRecievedOfAddon = new Dictionary<DownloadableAddon, long>();
        Dictionary<DownloadableAddon, String> addonToRootPath = new Dictionary<DownloadableAddon, string>();

        public void extractRar(DownloadableAddon addon, String extention)
        {
            try
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);

                IArchive compressed = ArchiveFactory.Open(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + addon.fileName);
                compressed.CompressedBytesRead += new EventHandler<CompressedBytesReadEventArgs>(compressed_CompressedBytesRead);
                archiveToAddon.Add(compressed, addon);

                backgroundWorkerToAddon.Add(worker, addon);
                addonToBackgroundDownloader.Add(addon, worker);
                backGroundWorkerToArchive.Add(worker, compressed);
                bytesRecievedOfAddon.Add(addon, 0);

                worker.RunWorkerAsync(compressed);
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show("Fehler beim entpacken: " + ex.Message);
                this.main.reportProgress(addon, " - Fehler beim entpacken.", true);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            List<Object> objects = e.UserState as List<Object>;
            long BytesRecieved = (long)objects[0];
            DownloadableAddon addon = objects[1] as DownloadableAddon;
            BytesRecieved += bytesRecievedOfAddon[addon];
            long TotalBytesToRecieve = backGroundWorkerToArchive[ addonToBackgroundDownloader[addon]].TotalSize;

            double d1 = (double)(BytesRecieved / 1000L) / 1000.0;
            string str1 = this.doubleToShortenedString(d1);
            double d2 = (double)(TotalBytesToRecieve / 1000L) / 1000.0;
            string str2 = this.doubleToShortenedString(d2);

            double d3 = 100.0 / d2 * d1;
            this.main.reportProgress(addon, " Entpacke: " +  str1 + " MB / " + str2 + " MB (" + this.doubleToShortenedString(d3) + "%)", false);
        }

        void compressed_CompressedBytesRead(object sender, CompressedBytesReadEventArgs e)
        {
            DownloadableAddon addon = archiveToAddon[sender as IArchive];

            BackgroundWorker worker = addonToBackgroundDownloader[addon];

            List<object> objectsToGive = new List<object>();
            objectsToGive.Add(e.CompressedBytesRead);
            objectsToGive.Add(addon);

            worker.ReportProgress(1, objectsToGive);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DownloadableAddon addon = backgroundWorkerToAddon[sender as BackgroundWorker];
            IArchive archive = backGroundWorkerToArchive[sender as BackgroundWorker];
            String pathName = addonToRootPath[addon];

            archiveToAddon.Remove(archive);
            backgroundWorkerToAddon.Remove(sender as BackgroundWorker);
            addonToBackgroundDownloader.Remove(addon);
            backGroundWorkerToArchive.Remove(sender as BackgroundWorker);
            addonToRootPath.Remove(addon);

            this.main.reportProgress(addon, " durchsuche Ordner", false);

            List<String> pathes = new List<string>();

            getRealModFolders(main.path + "\\" + pathName, ref pathes);
            copyFoldersToRightPosition(pathes);

            this.main.reportProgress(addon, " Fertig.", false);
        }

        void copyFoldersToRightPosition(List<String> folders)
        {
            foreach (String s in folders)
            {
                DirectoryInfo info = new DirectoryInfo(s);
                if (info.Name.Substring(0, 1) == "@" || info.Name.Equals("userconfig") || info.Name.Equals("keys"))
                {
                    copyFolder(info.FullName, main.path + "\\" + info.Name);
                    /*File.Copy(info.FullName, main.path + "\\" + info.Name, true);
                    File.Delete(info.FullName);*/
                }
                else if (info.Name.Equals("TeamSpeak 3 Client"))
                {
                    if (info.GetDirectories().Length > 0 && info.GetDirectories()[0].Name.Equals("plugins"))
                    {
                        object keys = Registry.ClassesRoot.OpenSubKey("ts3file").OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command", RegistryKeyPermissionCheck.ReadSubTree).GetValue("");
                        if (keys != null && keys is String)
                        {
                            String tsPath = keys as String;
                            if (tsPath.Contains("\""))
                            {
                                tsPath = tsPath.Substring(1);
                                if (tsPath.Contains("\""))
                                {
                                    tsPath = tsPath.Substring(0, tsPath.IndexOf("\""));
                                    tsPath = Path.GetDirectoryName(tsPath);
                                    copyFolder(info.GetDirectories()[0].FullName, tsPath + "\\" + info.GetDirectories()[0].Name);
                                }
                            }
                        }
                    }
                }
            }
        }

        void copyFolder(String SourcePath, String DestinationPath)
        {
            try
            {
                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                    SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
            }
            catch (Exception ex)
            {
                ErrorWriter.writeError(ex.Message + " " + ex.Source);
            }
        }

        void getRealModFolders(String modRootPath, ref List<String> output)
        {
            try
            {
                if (Path.GetDirectoryName(modRootPath).Substring(0, 1) != "@" && Path.GetFileName(modRootPath).Substring(0, 1) != "@")
                {
                    List<String> modsAndStuff = new List<string>();
                    DirectoryInfo di = new DirectoryInfo(modRootPath);
                    foreach (DirectoryInfo info in di.GetDirectories())
                    {
                        if (info.Name.Substring(0, 1) == "@")
                        {
                            output.Add(info.FullName);
                        }
                        else if (info.Name.Equals("userconfig"))
                        {
                            output.Add(info.FullName);
                        }
                        else if (info.Name.Equals("keys") || info.Name.Equals("TeamSpeak 3 Client"))
                        {
                            output.Add(info.FullName);
                        }
                        else
                        {
                            getRealModFolders(info.FullName, ref output);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorWriter.writeError(ex.Message + " " + ex.StackTrace);
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            IArchive compressed = e.Argument as IArchive;
            DownloadableAddon addon = archiveToAddon[compressed];

            string destinationDirectory = this.main.path;
            int i = 0;
            
            foreach (var entry in compressed.Entries)
            {
                if (i == 0)
                {
                    addonToRootPath[addon] = entry.FilePath.Replace("/", "");
                }
                if (!entry.IsDirectory)
                {
                    try
                    {   
                        entry.WriteToDirectory(destinationDirectory, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                        List<object> objectsToGive = new List<object>();
                        objectsToGive.Add(i);
                        objectsToGive.Add(addon);
                        bytesRecievedOfAddon[addon] += entry.Size;
                    }
                    catch (Exception ex)
                    {
                        ErrorWriter.writeError(ex.Message + " " + ex.StackTrace);
                    }
                    //worker.ReportProgress(1, objectsToGive);
                }
                i++;
            }
        }







        public static void Load(string streamname, Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            char[] buffer = new char[1024];
            while (!reader.EndOfStream)
            {
                reader.Read(buffer, 0, buffer.Length);
            }
        }

        public void removeAddon(string name)
        {
            string path = this.main.path + "\\" + name;
            if (!Directory.Exists(path))
                return;
            try
            {
                Directory.Delete(path, true);
                if (Directory.Exists(this.main.path + "\\userconfig") && Directory.Exists((this.main.path + "\\userconfig\\" + name.Substring(1)).ToLower()))
                    Directory.Delete((this.main.path + "\\userconfig\\" + name.Substring(1)).ToLower(), true);
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message);
            }
        }

        public void removeDownloadedAddon(DownloadableAddon addon)
        {
            try
            {
                System.IO.File.Delete(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + addon.fileName);
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show("Konnte Addon " + addon.name + " nicht löschen. " + ex.Message);
            }
        }
    }
}

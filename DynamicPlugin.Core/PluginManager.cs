using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DynamicPlugin.Core
{
    public class PluginManager : IDisposable
    {
        List<string> _tempPathFiles = new List<string>();

        Dictionary<string, IPlugin> _map = new Dictionary<string, IPlugin>();

        public void RegisterFileChange()
        {
            var combine = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "domain");
            if (Directory.Exists(combine) == false)
            {
                Directory.CreateDirectory(combine);
            }

            FileSystemWatcher fileWatcher = new FileSystemWatcher(combine);
            fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileWatcher.Filter = "*.*";
            fileWatcher.EnableRaisingEvents = true;

            fileWatcher.Changed += FileWatcher_Changed;
         
        }

        public IPlugin GetCurrentPlugin(string pathFile)
        {
            var originalFile = new FileInfo(pathFile);
            IPlugin plugin;
            var originalFileName = originalFile.Name;
            if (_map.TryGetValue(originalFileName, out plugin))
            {
                return plugin;
            }
            return null;
        }

        private void FileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            var originalFile = new FileInfo(e.FullPath);
            IPlugin plugin;
            var originalFileName = originalFile.Name;
            if (_map.TryGetValue(originalFileName, out plugin))
            {
                plugin.Dispose();
                _map.Remove(originalFileName);
            }
        }

        private void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            var xxx = Load(e.FullPath);
            Console.WriteLine("new plugin file");
            xxx.GetInfo();
        }

        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            var xxx = Load(e.FullPath);
            Console.WriteLine("change plugin file");
            xxx.GetInfo();
        }

        public IPlugin Load(string pathFile)
        {
            var tempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
            if (Directory.Exists(tempDir) == false)
            {
                Directory.CreateDirectory(tempDir);
            }

            var originalFile = new FileInfo(pathFile);

           
            var originalFileName = originalFile.Name;

            var tempFile = Path.Combine(tempDir, originalFileName);
            var fileVersion = 0;

            while (true)
            {
                if (File.Exists(tempFile))
                {
                    fileVersion++;
                    tempFile = Path.Combine(tempDir, fileVersion + "_" + originalFileName);
                    continue;
                }

                try
                {
                    File.Copy(pathFile, tempFile, true);
                }
                catch
                {
                    continue;
                }
                break;
            }

            var assembly = Assembly.LoadFile(tempFile);
            var allTypes = assembly.GetTypes();
            var listHandler = allTypes.Where(t => typeof(IPlugin).IsAssignableFrom(t)
                                                  && t.IsClass && !t.IsAbstract).ToList();

            if (listHandler.Count > 0)
            {
                var newPluginVersion = (IPlugin)Activator.CreateInstance(listHandler[0]);

                IPlugin oldPlugin;
                if (_map.TryGetValue(originalFileName, out oldPlugin))
                {
                    oldPlugin.Dispose();
                }

                _map[originalFileName] = null;
                _map[originalFileName] = newPluginVersion;

                _tempPathFiles.Add(tempFile);

                return newPluginVersion;
            }

            return null;
        }




        public void Dispose()
        {
            try
            {
                foreach (var plugin in _map)
                {
                    plugin.Value.Dispose();
                }

                foreach (var file in _tempPathFiles)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
            }
            catch
            {

            }
        }
    }
}
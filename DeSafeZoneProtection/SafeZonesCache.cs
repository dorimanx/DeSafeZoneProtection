using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeSafeZoneProtection
{
    public class SafeZonesCache : IDisposable
    {
        public string path;
        public Dictionary<long, string> Cache = new Dictionary<long, string>();
        private bool loading;

        public SafeZonesCache(string fileName) => path = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Instance"), fileName);

        public void Load()
        {
            loading = true;

            try
            {
                if (!File.Exists(path))
                    using (StreamWriter text = File.CreateText(path)) text.WriteLine("");

                string str = File.ReadAllText(path);
                if (!(string.IsNullOrEmpty(str) || str.Length < 10))
                {
                    Enumerable.ForEach(delegate (KeyValuePair<long, string> b)
                     {
                         _ = Cache.Append(b);
                     });
                    DeSafeZoneProtectionPlugin.Log.Info("Cache Reloading...");
                }
            }
            catch (Exception ex)
            {
                DeSafeZoneProtectionPlugin.Log.Error(ex);
                DeSafeZoneProtectionPlugin.Log.Warn("Error in cache loading!");
            }
            finally
            {
                loading = false;
            }
        }

        private IEnumerable<KeyValuePair<long, string>> Enumerable => from b in File.ReadAllText(path).Split(new string[]
                                                                                                 {
                    "\n"
                                                                                                 }, StringSplitOptions.RemoveEmptyEntries)
                                                                      select new KeyValuePair<long, string>(long.Parse(b.Split(new char[]
                                               {
                    '='
                                               })[0]), b.Split(new char[]
                                               {
                    '='
                                               })[1]);

        public void Save()
        {
            if (loading)
                return;

            try
            {
                if (!File.Exists(path))
                {
                    using (StreamWriter text = File.CreateText(path))
                        text.WriteLine("");
                }

                lock (this)
                {
                    var ValuesToWrite = Values().Count();

                    if (ValuesToWrite > 0)
                        File.WriteAllText(path, string.Join("\n", Values()));
                }
                DeSafeZoneProtectionPlugin.Log.Info("Cache Saving...");
            }
            catch (Exception ex)
            {
                DeSafeZoneProtectionPlugin.Log.Error(ex);
                DeSafeZoneProtectionPlugin.Log.Warn("Error in cache saving!");
            }
            return;
        }

        private IEnumerable<string> Values()
        {
            foreach (var b in Cache)
            {
                yield return string.Join("=", new string[] { b.Key.ToString(), b.Value });
            }
        }

        public void Dispose()
        {
            try
            {
                DeSafeZoneProtectionPlugin.Log.Info("Dispose Cache Saving...");
                Save();
            }
            catch (Exception ex)
            {
                DeSafeZoneProtectionPlugin.Log.Error(ex);
                DeSafeZoneProtectionPlugin.Log.Warn("Error in cache saving!");
                return;
            }
        }
    }
}

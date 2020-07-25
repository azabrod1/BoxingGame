using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Main
{
    public static class DataPersistance
    {
        static readonly ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string GetSaveDataDirectory()
        {
            string platform = Utility.GetOSPlatform().ToString();

            return platform switch
            {
                "OSX" => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Application Support/BoxingGame/"),
                _ => throw new Exception($"OS Platform {platform} currently unsupported"),
            };
        }

        private static JsonSerializer Serializer = new JsonSerializer
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static string GetSaveDataPath()
        {
            var directory = GetSaveDataDirectory();
            Directory.CreateDirectory(directory);
            if (!Directory.Exists(directory))
                throw new Exception($"Could not create save data directory {directory}");

            string path = GetSaveDataDirectory() + ConfigurationManager.AppSettings["SaveFile"];
            return path;
        }

        public static void SaveGame(this Application app)
        {
            
            var directory = GetSaveDataDirectory();
            Directory.CreateDirectory(directory);
            if (!Directory.Exists(directory))
                throw new Exception($"Could not create save data directory {directory}");

            string path = GetSaveDataDirectory() + ConfigurationManager.AppSettings["SaveFile"];

            using (FileStream fs = File.Open(path, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;

                
                //serializer.ContractResolver = new AllMembersContractResolver();
                Serializer.Serialize(jw, app);
            }
        }

        public static Application LoadGame()
        {
            string path = GetSaveDataDirectory() + ConfigurationManager.AppSettings["SaveFile"];

            if (!File.Exists(path))
            {
                LOGGER.Info($"{path} not found. Game save not created yet?");
                return null;
            }

            using StreamReader file = File.OpenText(path);
           
            Application app = (Application) Serializer.Deserialize(file, typeof(Application));
            return app;
        }

        public class AllMembersContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
        {
            protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, Newtonsoft.Json.MemberSerialization memberSerialization)
            {
                var privates = type
                    .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance) //All properties
                    .Where(prop => !prop.PropertyType.IsDefined(typeof(JsonProperty), false))
                    .Select(p => base.CreateProperty(p, memberSerialization))
                    .ToList();

                privates.ForEach(p => { p.Writable = true; p.Readable = true; });

                privates.ForEach(p => Console.WriteLine(p));

                var BaseProps = base.CreateProperties(type, memberSerialization);
                return BaseProps.Union(privates).ToList();
          
            }
        }
    }
}
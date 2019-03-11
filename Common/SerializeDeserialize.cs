using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace Common
{
    public static class SerializeDeserialize
    {

        public static void SerializeObject<T>(List<T> list, string fileName)
        {
            using (var stream = File.OpenWrite(fileName))
            {
                Serializer.Serialize(stream, list);
            }
        }

        public static string SerializeObject<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, obj);
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }


        public static void SerializeQueue(ConcurrentDictionary<string, ItineraryQueue> list, string fileName)
        {
            using (var stream = File.OpenWrite(fileName))
            {
                Serializer.Serialize(stream, list);
            }
        }

        public static void DeserializeObject<T>(List<T> list, string fileName)
        {
            if (!File.Exists(fileName))
                return;
            using (var stream = File.OpenRead(fileName))
            {
                var other = Serializer.Deserialize<List<T>>(stream);
                list.Clear();
                list.AddRange(other);
            }
            File.Delete(fileName);
        }

        public static ConcurrentDictionary<string, ItineraryQueue> DeserializeQueue(ConcurrentDictionary<string, ItineraryQueue> list, string fileName)
        {
            if (!File.Exists(fileName))
                return new ConcurrentDictionary<string, ItineraryQueue>();
            using (var stream = File.OpenRead(fileName))
            {
                foreach (var item in Serializer.Deserialize<ConcurrentDictionary<string, ItineraryQueue>>(stream))
                {
                    list.TryAdd(item.Key, item.Value);
                }
            }
            File.Delete(fileName);
            return list;
        }
    }
}
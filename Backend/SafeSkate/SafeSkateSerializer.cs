using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SafeSkate
{
    public class Serializer<T> where T : class
    {
        public static string Serialize(T obj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
            using (var sww = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(sww) { Formatting = Formatting.Indented })
                {
                    xsSubmit.Serialize(writer, obj);
                    return sww.ToString();
                }
            }
        }
    }

    public static class SafeSkateSerializer
    {
        public static string SerializeMapMarkerUpdateMessage(MapMarkerUpdateMessage message)
        {
            var serializer = new XmlSerializer(typeof(MapMarkerUpdateMessage));
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, message);
                return stringWriter.ToString();
            }
        }

        public static string SerializeMapMarkerInfo(MapMarkerInfo info)
        {
            var serializer = new XmlSerializer(typeof(MapMarkerInfo));
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, info);
                return stringWriter.ToString();
            }
        }

        public static MapMarkerUpdateMessage DeserializeMapMarkerUpdateMessage(string xml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(MapMarkerUpdateMessage));
                using (var stringReader = new StringReader(xml))
                {
                    return (MapMarkerUpdateMessage)serializer.Deserialize(stringReader);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static MapMarkerInfo DeserializeMapMarkerInfo(string xml)
        {
            var serializer = new XmlSerializer(typeof(MapMarkerInfo));
            using (var stringReader = new StringReader(xml))
            {
                return (MapMarkerInfo)serializer.Deserialize(stringReader);
            }
        }

        public static string SerializeMapMarkerInfoList(List<MapMarkerInfo> infoList)
        {
            var serializer = new XmlSerializer(typeof(List<MapMarkerInfo>));
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, infoList);
                return stringWriter.ToString();
            }
        }

        public static List<MapMarkerInfo> DeserializeMapMarkerInfoList(string xml)
        {
            var serializer = new XmlSerializer(typeof(List<MapMarkerInfo>));
            using (var stringReader = new StringReader(xml))
            {
                return (List<MapMarkerInfo>)serializer.Deserialize(stringReader);
            }
        }
    }
}

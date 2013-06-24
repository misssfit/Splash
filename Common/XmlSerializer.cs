using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Splash.Common
{
    public static class Serializer
    {
        /// <summary>
        ///     Method serializes given object to xml
        /// </summary>
        /// <typeparam name="T">any type that can be serialized</typeparam>
        /// <param name="source">type to serialize</param>
        /// <param name="types"> </param>
        /// <returns>Xml reprezentation od T source as XDocument</returns>
        public static XDocument SerializeToXDocument<T>(this T source, Type[] types) where T : class
        {
            var target = new XDocument();
            var xmlSerializer = new XmlSerializer(typeof (T), types);
            using (XmlWriter writer = target.CreateWriter())
            {
                xmlSerializer.Serialize(writer, source);
                writer.Close();
            }
            return target;
        }

        public static XDocument SerializeToXDocument<T>(this T source) where T : class
        {
            return SerializeToXDocument(source, new[] {source.GetType()});
        }

        public static T Deserialize<T>(this XDocument xmlDocument)
        {
            var xmlSerializer = new XmlSerializer(typeof (T));
            using (XmlReader reader = xmlDocument.CreateReader())
            {
                return (T) xmlSerializer.Deserialize(reader);
            }
        }
    }
}
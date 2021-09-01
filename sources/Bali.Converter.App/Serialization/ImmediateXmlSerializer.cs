namespace Bali.Converter.App.Serialization
{
    using System.IO;
    using System.Xml.Serialization;
    using Prism.Regions.Behaviors;

    public static class ImmediateXmlSerializer
    {
        public static void Serialize<T>(string path, T obj)
        {
            using var writer = new StreamWriter(path);
            var serializer = new XmlSerializer(typeof(T));

            serializer.Serialize(writer, obj);
        }

        public static T Deserialize<T>(string path)
        {
            using var reader = new StreamReader(path);
            var serializer = new XmlSerializer(typeof(T));

            return (T)serializer.Deserialize(reader);
        }
    }
}

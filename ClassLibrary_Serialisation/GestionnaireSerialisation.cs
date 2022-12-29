using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace ClassLibrary_Serialisation
{
    /// <summary>
    /// <c>GestionnaireFichierFact</c>: Abstract class to describe serializer manager
    /// </summary>
    /// <typeparam name="T"> The class to serialize</typeparam>
    public abstract class SerializationManagerFact<T>
    {
        // Can't put an IFormatter formatter here because XMLSerializer is not one.

        /// <summary>
        /// Serialize an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns> our serialized object</returns>
        public abstract string Serialization(T obj);
        /// <summary>
        /// Deserialize an object
        /// </summary>
        /// <param name="str"> the string which represent our object </param>
        /// <returns> Our object</returns>
        public abstract T Deserialization(string str);

    }

    /// <summary>
    /// Manager for binary Serialization
    /// </summary>
    ///  <inheritdoc/>
    public class BinarySerializationManager<T> : SerializationManagerFact<T>
    {
        private readonly IFormatter formatter;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BinarySerializationManager()
        {
            formatter = new BinaryFormatter();
        }

        /// <inheritdoc/>
        public override string Serialization(T obj)
        {
            MemoryStream memoryStream = new MemoryStream();
            formatter.Serialize(memoryStream, obj);
            memoryStream.Flush();
            memoryStream.Position = 0;

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        /// <inheritdoc/>
        public override T Deserialization(string s)
        {
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(s));
            return (T)formatter.Deserialize(memoryStream);
        }
    }

    /// <summary>
    /// Manager for XML Serialization
    /// </summary>
    ///  <inheritdoc/>
    public class XMLSerializationManager<T> : SerializationManagerFact<T>
    {
        readonly XmlSerializer formatter;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public XMLSerializationManager()
        {
            formatter = new XmlSerializer(typeof(T));
        }

        /// <inheritdoc/>
        public override string Serialization(T obj)
        {
            using StringWriter writer = new StringWriter();
            formatter.Serialize(writer, obj);
            Deserialization(writer.ToString());
            return writer.ToString();
        }

        /// <inheritdoc/>
        public override T Deserialization(string s)
        {
            using StringReader reader = new StringReader(s);
            return (T)formatter.Deserialize(reader);
        }
    }
}

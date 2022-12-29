using ClassLibrary_Contact;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace ClassLibrary_Serialisation
{
    public abstract class GestionnaireFichierFact<T>
    {
        public abstract string Serialization(T obj);
        public abstract T Deserialization(string fileName);

    }


    public class GestionnaireBinaire<T> : GestionnaireFichierFact<T>
    {
        private readonly IFormatter formatter;
        public GestionnaireBinaire()
        {
            formatter = new BinaryFormatter();
        }

        public override string Serialization(T obj)
        {
            MemoryStream memoryStream = new MemoryStream();
            formatter.Serialize(memoryStream, obj);
            memoryStream.Flush();
            memoryStream.Position = 0;
            
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public override T Deserialization(string s)
        {
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(s));
            return (T)formatter.Deserialize(memoryStream);
        }
    }
    public class GestionnaireXML<T> : GestionnaireFichierFact<T>
    {
        readonly XmlSerializer formatter;
        public GestionnaireXML()
        {
            formatter = new XmlSerializer(typeof(T));
        }
        public override string Serialization(T obj)
        {
            using StringWriter writer = new StringWriter();
            formatter.Serialize(writer, obj);
            Deserialization(writer.ToString());
            return writer.ToString();
        }
        public override T Deserialization(string s)
        {
            using StringReader reader = new StringReader(s);
            return (T)formatter.Deserialize(reader);
        }
    }
}

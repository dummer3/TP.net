using ClassLibrary_Contact;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace ClassLibrary_Serialisation
{
    public abstract class GestionnaireFichierFact<T>
    {
        public abstract void Serialization(string fileName, T obj);
        public abstract T Deserialization(string fileName);
    }


    public class GestionnaireBinaire<T> : GestionnaireFichierFact<T>
    {
        private readonly IFormatter formatter;
        public GestionnaireBinaire()
        {
            formatter = new BinaryFormatter();
        }

        public override void Serialization(string fileName, T obj)
        {
            Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, obj);
            stream.Close();
        }

        public override T Deserialization(string fileName)
        {
            Stream readStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            T d = (T)formatter.Deserialize(readStream);
            readStream.Close();
            return d;
        }
    }
    public class GestionnaireXML<T> : GestionnaireFichierFact<T>
    {
        readonly XmlSerializer formatter;
        public GestionnaireXML()
        {
            formatter = new XmlSerializer(typeof(T));
        }
        public override void Serialization(string fileName, T obj)
        {
            TextWriter writer = new StreamWriter(fileName);
            formatter.Serialize(writer, obj);
            writer.Close();
        }
        public override T Deserialization(string fileName)
        {
            TextReader reader = new StringReader(fileName);
            T d = (T)formatter.Deserialize(reader);
            reader.Close();
            return d;
        }
    }
}

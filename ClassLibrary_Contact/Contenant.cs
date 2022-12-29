using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ClassLibrary_Contact
{
    [XmlInclude(typeof(Dossier))]
    [XmlInclude(typeof(Contact))]
    [Serializable]
    public class Contenant
    {
        [XmlElement(DataType = "dateTime")]
        public DateTime CreationTime { get; set; }
        [XmlElement(DataType = "dateTime")]
        public DateTime ModificationTime { get; set; }

        public Contenant(DateTime CreationTime, DateTime ModificationTime)
        {
            this.CreationTime = CreationTime;
            this.ModificationTime = ModificationTime;
        }

        public Contenant() {
            CreationTime = ModificationTime = DateTime.Now;
        }

        public static DateTime DateFromString(string value)
        {
            try
            {
                int[] i = value.Split(':', '/').Select(int.Parse).ToArray();
                DateTime d = new DateTime(i[2], i[1], i[0]);
                return d;
            }
            catch(Exception e) { throw e; }
        }
    }
}

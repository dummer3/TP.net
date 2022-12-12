using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ClassLibrary_Contact
{
    [Serializable]
    public class Dossier : IStockable
    {
        public string Nom { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime ModificationTime { get; set; }
        [XmlIgnore]
        public List<IStockable> contents;


        public Dossier() : this("root", DateTime.Now, DateTime.Now) { }
        public Dossier(string nom): this(nom, DateTime.Now, DateTime.Now){}
        public Dossier(string nom, DateTime creationTime, DateTime modificationTime)
        {
            Nom = nom;
            CreationTime = creationTime;
            ModificationTime = modificationTime;
            contents = new List<IStockable>();
        }

        public void AddStockable(IStockable st)
        {
            contents.Add(st);
        }

        public override string ToString()
        {
            return $"Dossier {Nom} -- Modif {ModificationTime} -- Creation {CreationTime} {System.Environment.NewLine}";
        }

    }
}

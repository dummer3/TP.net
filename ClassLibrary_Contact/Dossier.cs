using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ClassLibrary_Contact
{
    [Serializable]
    public class Dossier : Contenant
    {
        public string Nom { get; set; }

        public List<Contenant> contents;
        
        [XmlIgnore]
        public Dossier Parent { get; set; }

        public Dossier() : this("root", DateTime.Now, DateTime.Now) { Parent = this;}
        public Dossier(string nom): this(nom, DateTime.Now, DateTime.Now){}
        public Dossier(string nom, DateTime creationTime, DateTime modificationTime) : base(creationTime, modificationTime)
        {
            Nom = nom;
            contents = new List<Contenant>();
        }

        public void AddStockable(Contenant contenant)
        {
            contents.Add(contenant);
        }

        public override string ToString()
        {
            return $"Dossier {Nom} -- Modif {ModificationTime} -- Creation {CreationTime} {System.Environment.NewLine}";
        }

    }
}

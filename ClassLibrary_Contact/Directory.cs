using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ClassLibrary_Contact
{

    /// <summary>
    /// <c> Dossier </c> represent a folder, it can contains folder or contact
    /// </summary>
    [Serializable]
    public class Directory : Content
    {
        /// <value> <c>Name</c>, the name of this folder </value>
        public string Name { get; set; }

        /// <value> <c>contents</c>, Contents store inside this folder </value>
        public List<Content> contents;

        /// <value> <c>Parent</c>, the super diretory</value>
        [XmlIgnore]
        public Directory Parent { get; set; }


        /// <summary>
        /// Default constructor
        /// By default the name is "root" and all DateTime define to Now
        /// </summary>
        public Directory() : this("root", DateTime.Now, DateTime.Now) { Parent = this; }
        /// <summary>
        /// Constructor to modify the directory name
        /// </summary>
        /// <param name="name"> name of the directory</param>
        public Directory(string name) : this(name, DateTime.Now, DateTime.Now) { }

        /// <summary>
        /// Constructor to modify the directory name and is date
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="creationTime"></param>
        /// <param name="modificationTime"></param>
        public Directory(string nom, DateTime creationTime, DateTime modificationTime) : base(creationTime, modificationTime)
        {
            Name = nom;
            contents = new List<Content>();
        }

        /// <summary>
        /// Add a content to this directory
        /// </summary>
        /// <param name="contenant"> Content to add</param>
        public void AddStockable(Content contenant)
        {
            contents.Add(contenant);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns> a string which define our directory </returns>
        public override string ToString()
        {
            return $"Dossier [{CreationTime}|{ModificationTime}] {Name}{System.Environment.NewLine}";
        }

    }
}

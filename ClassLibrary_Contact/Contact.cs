using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;


namespace ClassLibrary_Contact
{
    public enum Lien { AMI, COLLEGUE, RELATION, RESEAU, NONDEFINI };

    [Serializable]
    public class Contact : IStockable
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }

        public MailAddress courriel; 
        public string SetCourrielStr { 
            set
            {
                try
                {
                    MailAddress m = new MailAddress(value);
                    courriel = m;
                }
                catch (FormatException exception)
                {
                    Console.WriteLine($"Erreur, Email non valide: {exception.Message}");
                }
            } 
        }
        public MailAddress GetCourriel{ get; }

        public string Societe { get; set; }
        public Lien Lien { get; set;}
        public DateTime CreationTime { get; set; }
        public DateTime ModificationTime { get; set; }

        public Contact(string nom, string prenom, MailAddress courriel, string societe, Lien lien, DateTime creationTime, DateTime modificationTime)
        {
            Nom = nom;
            Prenom = prenom;
            this.courriel = courriel;
            Societe = societe;
            Lien = lien;
            CreationTime = creationTime;
            ModificationTime = modificationTime;
        }

        public Contact()
        {
            Nom = Prenom = Societe = "___";
            SetCourrielStr = "____@__.___";
            Lien = Lien.NONDEFINI;
            CreationTime = ModificationTime = DateTime.Now;
        }

        public override string ToString()
        {
            return $" Contact {Nom} {Prenom} -- Email {courriel} -- Lien {Lien} -- Societe {Societe} {System.Environment.NewLine}";
        }
    }
}

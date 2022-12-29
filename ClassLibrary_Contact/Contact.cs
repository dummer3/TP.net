using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;


namespace ClassLibrary_Contact
{
    public enum Lien { AMI, COLLEGUE, RELATION, RESEAU, NONDEFINI };

    [Serializable]
    public class Contact : Contenant
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }

        private string courriel; 
        public static string courrielStr(string s){
            string r = "unknown";    
            try
                {
                MailAddress m = new MailAddress(s);
                r = s;
                }
                catch (FormatException exception)
                {
                    Console.WriteLine($"Erreur, Email non valide: {exception.Message}");
                } 
            return r;
        }


        public string Societe { get; set; }
        public Lien Lien { get; set;}

        public Contact(string nom, string prenom, string courriel, string societe, Lien lien, DateTime creationTime, DateTime modificationTime) : base(creationTime, modificationTime)
        {
            Nom = nom;
            Prenom = prenom;
            this.courriel = courrielStr(courriel);
            Societe = societe;
            Lien = lien;
        }

        public Contact() : this("___", "___", "___@___.___", "___", Lien.NONDEFINI, DateTime.Now, DateTime.Now){}
        public Contact(string nom, string prenom, string courriel, string societe, Lien lien) : this(nom,prenom,courriel,societe,lien, DateTime.Now, DateTime.Now) { }
        public override string ToString()
        {
            return $" Contact {Nom} {Prenom} -- Email {courriel} -- Lien {Lien} -- Societe {Societe} {System.Environment.NewLine}";
        }
    }
}

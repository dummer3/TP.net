using System;
using System.Collections.Generic;
using System.Text;
using ClassLibrary_Contact;
using ClassLibrary_Serialisation;

namespace ClassLibrary_Contact
{
    class GestionnaireContact
    {
        private Dossier root;
        private Dossier current;

        private readonly GestionnaireFichierFact<Dossier> xmlSerial = new GestionnaireXML<Dossier>();
        private readonly GestionnaireFichierFact<Dossier> binSerial = new GestionnaireBinaire<Dossier>();

        public GestionnaireContact()
        {
            root = new Dossier("root");
            current = root;
        }

        public string Aff()
        {
            int posLevel = 0;
            List<List<Dossier>> dossierperLevel = new List<List<Dossier>>(10);
            List<List<Contact>> contactperLevel = new List<List<Contact>>(10);

            for (int i = 0; i < 10; i++)
            {
                dossierperLevel.Add(new List<Dossier>());
                contactperLevel.Add(new List<Contact>());
            }
            string result = "";
            dossierperLevel[0].Add(root);

            while (dossierperLevel[0].Count != 0 || posLevel > 0)
            {
                while (dossierperLevel[posLevel].Count != 0)
                {
                    result += string.Concat(System.Linq.Enumerable.Repeat("   | ", posLevel)) + $"--> {dossierperLevel[posLevel][0]}";
                    posLevel++;
                    foreach (IStockable st in dossierperLevel[posLevel - 1][0].contents)
                    {
                        if (st is Dossier dossier)
                            dossierperLevel[posLevel].Add(dossier);
                        else
                            contactperLevel[posLevel].Add((Contact)st);
                    }
                    dossierperLevel[posLevel - 1].RemoveAt(0);
                }
                while (posLevel >= 0 && dossierperLevel[posLevel].Count == 0)
                {
                    while (contactperLevel[posLevel].Count != 0)
                    {
                        result += string.Concat(System.Linq.Enumerable.Repeat("   | ", posLevel)) + $"+-- {contactperLevel[posLevel][0]}";
                        contactperLevel[posLevel].RemoveAt(0);
                    }
                    posLevel--;
                }


            }

            return result;
        }
        public interface INamaeble
        {
            string Name { get; set; }
        }


        private void Question(ref string file, ref string choice)
        {
            do
            {
                Console.WriteLine("format binaire (b) ou XML (x)");
                choice = Console.ReadLine();
            } while (choice != "b" && choice != "x");

            Console.WriteLine("Entrer le nom de votre fichier");
            file = Console.ReadLine();
        }
        public void Charger()
        {
            string choice = "non";
            string file = "";
            Question(ref file, ref choice);

            if (choice == "b")
            {
                root = binSerial.Deserialization(file);
            }
            else
            {
                root = xmlSerial.Deserialization(file);
            }
        }

        public void Sauvegarder()
        {
            string choice = "non";
            string file = "";

            Question(ref file, ref choice);

            if (choice == "b")
            {
                binSerial.Serialization(file, root);
            }
            else
            {
                binSerial.Serialization(file, root);
            }
        }

        public void AjoutDossier()
        {
            Dossier ajoutD = new Dossier("dossier");
            current.AddStockable(ajoutD);
            current = ajoutD;
        }

        public void AjoutContact()
        {
            Contact ajoutC = new Contact();
            current.AddStockable(ajoutC);
        }
    }
}

using System;
using ClassLibrary_Serialisation;
using ClassLibrary_Contact;
using System.Collections.Generic;

namespace ConsoleVersion
{
    class Program
    {
        public static void Main()
        {
            GestionnaireContact g = new GestionnaireContact();
            bool stop = false;
            string choice;

            while (!stop)
            {
                Console.WriteLine(
                    $"Option:{System.Environment.NewLine}" +
                    $"sortir/quitter {System.Environment.NewLine}" +
                    $"afficher {System.Environment.NewLine}" +
                    $"charger  {System.Environment.NewLine}" +
                    $"sauvegarder  {System.Environment.NewLine}" +
                    $"changer de dossier  {System.Environment.NewLine}" +
                    $"ajouter dossier  {System.Environment.NewLine}" +
                    $"ajouter contact  {System.Environment.NewLine}"
                );

                choice = Console.ReadLine();
                switch (choice.ToLower().Replace(" ", ""))
                {

                    case "sortir":
                    case "quitter":
                    case "exit":
                    case "quit":
                        stop = true;
                        break;
                    case "afficher":
                        Console.WriteLine(g.Aff());
                        break;
                    case "charger":
                        g.Charger();
                        break;
                    case "enregistrer":
                    case "sauvegarder":
                        g.Sauvegarder();
                        break;
                    case "ajouterdossier":
                        g.AjoutDossier();
                        break;
                    case "ajoutercontact":
                        g.AjoutContact();
                        break;
                    case "changerdedossier":
                    case "changerdossier":
                        g.ChangerCurrent();
                        break;
                    default:
                        Console.WriteLine("Instruction Inconnue.");
                        break;

                }
            }
        }
    }
}

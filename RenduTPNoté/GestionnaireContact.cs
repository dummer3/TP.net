using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClassLibrary_Serialisation;
using System.Security.Cryptography;

using System.Linq;

using System.Security.Principal;
using Ionic.Zip;

namespace ClassLibrary_Contact
{
    class GestionnaireContact
    {
        private Dossier root;
        private Dossier current;

        private readonly GestionnaireFichierFact<Contenant> xmlSerial = new GestionnaireXML<Contenant>();
        private readonly GestionnaireFichierFact<Contenant> binSerial = new GestionnaireBinaire<Contenant>();

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
                    foreach (Contenant c in dossierperLevel[posLevel - 1][0].contents)
                    {
                        if (c is Dossier dossier)
                            dossierperLevel[posLevel].Add(dossier);
                        else
                            contactperLevel[posLevel].Add((Contact)c);
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

        private byte[] GetKey()
        {
            byte[] key = null;
            string choice;
            Console.WriteLine("Entrer le nom de votre clé");
            choice = Console.ReadLine();
            key = Global.VerificationFile(file => File.ReadAllBytes(Global.Path + file), "key_" + choice);
            return key;
        }

        private byte[] CreateKey()
        {
            string choice;
            AesManaged aes = new AesManaged();

            aes.GenerateKey();
            Console.WriteLine("Nouvelle clé crée, Choissisez le nom de clé:");
            choice = Console.ReadLine();
            Global.VerificationFile(file => File.WriteAllBytes(Global.Path + file, aes.Key), "key_" + choice);
            return aes.Key;
        }

        private byte[] CreateKey(string keyName)
        {
            Console.WriteLine($"Nouvelle clé crée de nom \"{keyName}\"");
            AesManaged aes = new AesManaged();
            aes.GenerateKey();
            Global.VerificationFile(file => File.WriteAllBytes(Global.Path + file, aes.Key), "key_" + keyName);
            return aes.Key;
        }

        private byte[] QuestionCrypto()
        {
            string choice;
            byte[] key;
            do
            {
                Console.WriteLine("Voulez-vous utilisez une clé existante? o/n");
                choice = Console.ReadLine();
            } while (choice != "o" && choice != "n");

            try
            {
                if (choice == "o")
                {
                    key = GetKey();
                }
                else
                {
                    do
                    {
                        Console.WriteLine("Voulez-vous créer une nouvelle clé ou utilisez votre SID? o/n");
                        choice = Console.ReadLine();
                    } while (choice != "o" && choice != "n");

                    key = choice == "o" ? CreateKey() : Encoding.Default.GetBytes(WindowsIdentity.GetCurrent().User.ToString()).Take(32).ToArray();
                }
                return key;
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is UnauthorizedAccessException)
            {
                Console.WriteLine("Ce fichier n'est pas valide, opération non prise en compte. Création d'une nouvelle clé temporaire \"temp\"");
                key = CreateKey("temp");
                return key;
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine("Erreur lors du décryptage, opération non prise en compte.");
                Console.WriteLine(ex.Message);
                return key = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur Inattendu {ex.GetType()}\n{ex.Message}");
                System.Environment.Exit(ex.HResult);
                return null;
            }

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
            string choice = "non", choice1 = "non",choice2 = "non";
            string file = "";
            Question(ref file, ref choice);

            byte[] key;

            Console.WriteLine("Votre fichier est-il compressé? (o/n)");
            choice1 = Console.ReadLine();
            if (choice1 == "o")
            {
                int attempt = 3;
                while (attempt >= 0)
                {
                    Console.Write("Mot de passe: ");
                    choice2 = Console.ReadLine();
                    using (ZipFile zip = ZipFile.Read($"{Global.entireFileName(file)}.zip"))
                    {
                        zip.Password = choice2;
                        try
                        {
                            zip.ExtractAll(Global.Path);
                            break;
                        }
                        catch
                        {
                            if (attempt > 0)
                            {
                                Console.WriteLine($"Mot de passe Incorecte, plus que {--attempt} essaie(s)");
                            }
                            else
                            {
                                Console.WriteLine("Mot de passe Incorecte, plus aucune tentative, suppression du fichier");

                                attempt--;
                            }


                        }
                    }
                    if (attempt < 0) { File.Delete($"{Global.entireFileName(file)}.zip"); return; }
                }
            }

            try
            {
                Console.WriteLine("Voulez-vous utiliser une clé? o/n");
                choice2 = Console.ReadLine();
                key = choice2 == "o" ? GetKey() : Encoding.Default.GetBytes(WindowsIdentity.GetCurrent().User.ToString()).Take(32).ToArray();
                string decrypt = GestionnaireSécurité.Decrypt(File.ReadAllBytes(Global.entireFileName("enc_" + file)), key);
                if (choice1 == "o")
                    File.Delete(Global.entireFileName("enc_" + file));
                if (choice == "b")
                    root = (Dossier)binSerial.Deserialization(decrypt);
                else
                    root = (Dossier)xmlSerial.Deserialization(decrypt);
                current = root;
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is UnauthorizedAccessException)
            {
                Console.WriteLine("Ce fichier n'est pas valide, opération non prise en compte");
                Console.WriteLine(ex.Message);
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine("Erreur lors du décryptage, opération non prise en compte. Etes vous sûr de la clé?");
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur Inattendu {ex.GetType()}\n{ex.Message}");
                System.Environment.Exit(ex.HResult);
            }
        }

        public void Sauvegarder()
        {
            string choice = "non";
            string file = "";
            Question(ref file, ref choice);

            byte[] key = QuestionCrypto();

            string seria;
            if (choice == "b")

                seria = binSerial.Serialization(root);
            else
                seria = xmlSerial.Serialization(root);

            File.WriteAllBytes(Global.entireFileName("enc_" + file), GestionnaireSécurité.Encrypt(seria, key));

            Console.WriteLine("Voulez-vous compresser votre fichier et mettre un mot de passe? (o/n)");
            choice = Console.ReadLine();
            if (choice == "o")
            {
                Console.Write("Mot de passe: ");
                choice = Console.ReadLine();
                using ZipFile zip = new ZipFile();
                zip.Password = choice;
                zip.AddFile(Global.entireFileName("enc_" + file), "");
                zip.Save($"{Global.entireFileName(file)}.zip");
                File.Delete(Global.entireFileName("enc_" + file));
            }

        }

        public void AjoutDossier()
        {
            Console.WriteLine("Nom[ DateTime creationTime (format dd/mm/yyyy or dd:mm:yyyy)]");
            string[] s = Console.ReadLine().Split(" ");
            Dossier ajoutD;
            try
            {
                if (s.Length > 1)
                    ajoutD = new Dossier(s[0], Dossier.DateFromString(s[1]), Dossier.DateFromString(s[2]));
                else
                    ajoutD = new Dossier(s[0]);
                ajoutD.Parent = current;
                current.AddStockable(ajoutD);
                current = ajoutD;
            }
            catch (Exception e)
            {
                Console.WriteLine("the folder can't be create, error\n" + e.Message);
            }

        }

        public void AjoutContact()
        {
            Console.WriteLine("nom prenom courriel societe lien[ creationTime modificationTime  (format dd/mm/yyyy or dd:mm:yyyy)]");
            string[] s = Console.ReadLine().Split(" ");
            Lien l;
            Contact ajoutC = new Contact();
            try
            {
                l = (Lien)Enum.Parse(typeof(Lien), s[4]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Link not understand\n" + e.Message);
                l = Lien.NONDEFINI;
            }
            try
            {
                if (s.Length > 5)
                    ajoutC = new Contact(s[0], s[1], s[2], s[3], l, Dossier.DateFromString(s[5]), Dossier.DateFromString(s[6]));
                else
                    ajoutC = new Contact(s[0], s[1], s[2], s[3], l);
                current.AddStockable(ajoutC);
            }
            catch (Exception e)
            {
                Console.WriteLine("the contact can't be create, error\n" + e.Message);
            }
        }

        public void ChangerCurrent()
        {
            string s = String.Empty;
            Dossier temp = current;
            while (s != "stop")
            {
                Console.WriteLine($"=== {current}");
                int i = 1;
                foreach (Dossier d in current.contents.OfType<Dossier>())
                    Console.WriteLine($"{i++} {d}");
                Console.WriteLine("Choissisez le prochain sous dossier en fonction du rang (i==index) ou de son nom (n==nom)\nUtilisez .. pour le répertoire parent ou s'arreter avec stop");
                s = Console.ReadLine();

                try
                {
                    switch (s)
                    {
                        case string a when a.Contains("i=="):
                            current = current.contents.OfType<Dossier>().ElementAt(int.Parse(a.Split("==")[1]) - 1);
                            current.Parent = temp;
                            temp = current;
                            break;
                        case string a when a.Contains("n=="):
                            current = current.contents.OfType<Dossier>().First(x => x.Nom == a.Split("==")[1]);
                            current.Parent = temp;
                            temp = current;
                            break;
                        case string a when a.Contains(".."):
                            current = current.Parent ?? current;

                            break;
                        case "stop": break;
                        default:
                            Console.WriteLine("Instruction non comprise, veuillez vérifier la syntaxe de la commande ou le parametre");
                            break;
                    }
                }
                catch { Console.WriteLine("erreur, veuillez recommencer"); }


            }
        }
    }
}

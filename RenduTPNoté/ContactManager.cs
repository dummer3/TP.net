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
    /// <summary>
    /// <c>ContactManager</c>: class which manage every action concerning our contact
    /// </summary>
    class ContactManager
    {
        private Directory _root;
        private Directory _current;

        private readonly SerializationManagerFact<Content> _xmlSerial = new XMLSerializationManager<Content>();
        private readonly SerializationManagerFact<Content> _binSerial = new BinarySerializationManager<Content>();

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ContactManager()
        {
            _root = new Directory("root");
            _current = _root;
        }

        /// <summary>
        /// Print our data since the root folder
        /// </summary>
        /// <returns> The representation </returns>
        public string Aff()
        {
            // Limited to 10 actually, but can be managed for largest scale
            int posLevel = 0;
            List<List<Directory>> dossierperLevel = new List<List<Directory>>(10);
            List<List<Contact>> contactperLevel = new List<List<Contact>>(10);

            for (int i = 0; i < 10; i++)
            {
                dossierperLevel.Add(new List<Directory>());
                contactperLevel.Add(new List<Contact>());
            }
            string result = "";
            dossierperLevel[0].Add(_root);

            // while we haven't browse every content
            while (dossierperLevel[0].Count != 0 || posLevel > 0)
            {
                // while we have folder
                while (dossierperLevel[posLevel].Count != 0)
                {
                    // Explore a new folder and save all information
                    result += string.Concat(System.Linq.Enumerable.Repeat("   | ", posLevel)) + $"--> {dossierperLevel[posLevel][0]}";
                    posLevel++;
                    foreach (Content c in dossierperLevel[posLevel - 1][0].contents)
                    {
                        if (c is Directory dossier)
                            dossierperLevel[posLevel].Add(dossier);
                        else
                            contactperLevel[posLevel].Add((Contact)c);
                    }
                    dossierperLevel[posLevel - 1].RemoveAt(0);
                }
                // while we have contact
                while (posLevel >= 0 && dossierperLevel[posLevel].Count == 0)
                {
                    // Print every contact
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

        /// <summary>
        /// Get a Key from a file
        /// </summary>
        /// <returns> The key </returns>
        private byte[] GetKey()
        {
            byte[] key = null;
            string choice;

            //Ask Key Name (for our file)
            Console.WriteLine("Entrer le nom de votre clé");
            choice = Console.ReadLine();
            key = PathInformation.VerificationFile(file => File.ReadAllBytes(PathInformation.Path + file), "key_" + choice);
            return key;
        }

        /// <summary>
        /// Create a key and store it in a new file
        /// </summary>
        /// <returns></returns>
        private byte[] CreateKey()
        {
            string choice;
            AesManaged aes = new AesManaged();

            // Ask the FileName 
            aes.GenerateKey();
            Console.WriteLine("Nouvelle clé crée, Choissisez le nom de clé:");
            choice = Console.ReadLine();
            PathInformation.VerificationFile(file => File.WriteAllBytes(PathInformation.Path + file, aes.Key), "key_" + choice);
            return aes.Key;
        }

        /// <summary>
        /// Create a key and store it in a file name <c>keyName</c>
        /// </summary>
        /// <param name="keyName"> the name of the file </param>
        /// <returns> the key </returns>
        private byte[] CreateKey(string keyName)
        {
            Console.WriteLine($"Nouvelle clé crée de nom \"{keyName}\"");
            AesManaged aes = new AesManaged();
            aes.GenerateKey();
            PathInformation.VerificationFile(file => File.WriteAllBytes(PathInformation.Path + file, aes.Key), "key_" + keyName);
            return aes.Key;
        }

        /// <summary>
        /// Give a key depending of the user choice
        /// </summary>
        /// <returns> The key </returns>
        private byte[] QuestionCrypto()
        {
            string choice;
            byte[] key;

            // Ask for existing key
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
                    // Ask for create a new key
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

        /// <summary>
        /// Ask for the serialization format and the file name
        /// </summary>
        /// <param name="file"> name of the file </param>
        /// <param name="choice"> format choice for serialization </param>
        private void SerializationQuestion(ref string file, ref string choice)
        {
            // Ask for format
            do
            {
                Console.WriteLine("format binaire (b) ou XML (x)");
                choice = Console.ReadLine();
            } while (choice != "b" && choice != "x");

            // Ask for file name
            Console.WriteLine("Entrer le nom de votre fichier");
            file = Console.ReadLine();
        }

        /// <summary>
        /// Load Information from a file
        /// </summary>
        public void Charger()
        {
            string choiceSerialization, choiceZip, choiceKey, file, decrypt;
            file = choiceSerialization = string.Empty;
            byte[] key;

            SerializationQuestion(ref file, ref choiceSerialization);


            // Zip part with password
            Console.WriteLine("Votre fichier est-il compressé? (o/n)");
            choiceZip = Console.ReadLine();
            if (choiceZip == "o")
            {
                int attempt = 3;
                while (attempt >= 0)
                {
                    Console.Write("Mot de passe: ");
                    string pwd = Console.ReadLine();
                    using (ZipFile zip = ZipFile.Read($"{PathInformation.EntireFileName(file)}.zip"))
                    {
                        zip.Password = pwd;
                        try
                        {
                            zip.ExtractAll(PathInformation.Path);
                            break;
                        }
                        catch
                        {
                            if (attempt > 0)
                                Console.WriteLine($"Mot de passe Incorecte, plus que {--attempt} essaie(s)");
                            else
                            {
                                Console.WriteLine("Mot de passe Incorecte, plus aucune tentative, suppression du fichier");
                                attempt--;
                            }
                        }
                    }
                    if (attempt < 0) { File.Delete($"{PathInformation.EntireFileName(file)}.zip"); return; }
                }
            }

            //
            try
            {
                // Ask for existing key
                Console.WriteLine("Voulez-vous utiliser une clé? o/n");
                choiceKey = Console.ReadLine();

                // if the choice is yes, we get the key, else we use the SID as a key
                key = choiceKey == "o" ? GetKey() : Encoding.Default.GetBytes(WindowsIdentity.GetCurrent().User.ToString()).Take(32).ToArray();

                // We decrypt the file
                decrypt = SecurityManager.Decrypt(File.ReadAllBytes(PathInformation.EntireFileName("enc_" + file)), key);

                // In the case of a zip, we must delete this file
                if (choiceZip == "o")
                    File.Delete(PathInformation.EntireFileName("enc_" + file));

                // Depending of the serialization format
                if (choiceSerialization == "b")
                    _root = (Directory)_binSerial.Deserialization(decrypt);
                else
                    _root = (Directory)_xmlSerial.Deserialization(decrypt);

                _current = _root;
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

        /// <summary>
        /// Save information in a file
        /// </summary>
        public void Sauvegarder()
        {
            string Serializationchoice = string.Empty;
            string file = string.Empty;
            string seria;
            byte[] key = QuestionCrypto();

            SerializationQuestion(ref file, ref Serializationchoice);

            //Depending of the format choose 
            if (Serializationchoice == "b")
                seria = _binSerial.Serialization(_root);
            else
                seria = _xmlSerial.Serialization(_root);
            
            //Encrypt our data before writing it on a file
            File.WriteAllBytes(PathInformation.EntireFileName("enc_" + file), SecurityManager.Encrypt(seria, key));

            // Zip part
            Console.WriteLine("Voulez-vous compresser votre fichier et mettre un mot de passe? (o/n)");
            Serializationchoice = Console.ReadLine();
            if (Serializationchoice == "o")
            {
                Console.Write("Mot de passe: ");
                Serializationchoice = Console.ReadLine();

                // Create the zip, put the file inside and delete it after
                using ZipFile zip = new ZipFile();
                zip.Password = Serializationchoice;
                zip.AddFile(PathInformation.EntireFileName("enc_" + file), "");
                zip.Save($"{PathInformation.EntireFileName(file)}.zip");
                File.Delete(PathInformation.EntireFileName("enc_" + file));
            }

        }

        /// <summary>
        /// Add a <c>Directory</c> inside our manager (inside the current folder)
        /// </summary>
        public void AjoutDossier()
        {
            // Get all parameters
            Console.WriteLine("Nom[ DateTime creationTime (format dd/mm/yyyy or dd:mm:yyyy)]");
            string[] s = Console.ReadLine().Split(" ");
            Directory ajoutD;
            try
            {
                // Create the folder
                if (s.Length > 1)
                    ajoutD = new Directory(s[0], Directory.DateFromString(s[1]), Directory.DateFromString(s[2]));
                else
                    ajoutD = new Directory(s[0]);

                // Add it
                ajoutD.Parent = _current;
                _current.AddStockable(ajoutD);
                _current = ajoutD;
            }
            // If the format is not respected
            catch (Exception e)
            {
                Console.WriteLine("the folder can't be create, error\n" + e.Message);
            }

        }

        /// <summary>
        /// Add a <c>Contact</c> inside our manager (inside the current folder)
        /// </summary>
        public void AjoutContact()
        {
            Link l;
            Contact ajoutC;

            // Get the parameters
            Console.WriteLine("nom prenom courriel societe lien[ creationTime modificationTime (format dd/mm/yyyy or dd:mm:yyyy)]");
            string[] s = Console.ReadLine().Split(" ");

            // Verify the Link
            try
            {
                l = (Link)Enum.Parse(typeof(Link), s[4]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Link not understand\n" + e.Message);
                l = Link.NONDEFINI;
            }

            try
            {
                // Create the Contact
                if (s.Length > 5)
                    ajoutC = new Contact(s[0], s[1], s[2], s[3], l, Directory.DateFromString(s[5]), Directory.DateFromString(s[6]));
                else
                    ajoutC = new Contact(s[0], s[1], s[2], s[3], l);

                // Add it
                _current.AddStockable(ajoutC);
            }
            // If the format is not respected
            catch (Exception e)
            {
                Console.WriteLine("the contact can't be create, error\n" + e.Message);
            }
        }

        /// <summary>
        /// Change the current folder
        /// </summary>
        public void ChangerCurrent()
        {
            string choice = String.Empty;
            Directory temp = _current;

            // While the user want to change
            while (choice != "stop")
            {
                // Print the current folder
                Console.WriteLine($"=== {_current}");
                int i = 1;
                foreach (Directory d in _current.contents.OfType<Directory>())
                    Console.WriteLine($"{i++} {d}");

                // The user choose an action
                Console.WriteLine("Choissisez le prochain sous dossier en fonction du rang (i==index) ou de son nom (n==nom)\nUtilisez .. pour le répertoire parent ou s'arreter avec stop");
                choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        // Use the index
                        case string a when a.Contains("i=="):
                            _current = _current.contents.OfType<Directory>().ElementAt(int.Parse(a.Split("==")[1]) - 1);
                            _current.Parent = temp;
                            temp = _current;
                            break;
                        // Use the folder name
                        case string a when a.Contains("n=="):
                            _current = _current.contents.OfType<Directory>().First(x => x.Name == a.Split("==")[1]);
                            _current.Parent = temp;
                            temp = _current;
                            break;
                        // parent folder
                        case string a when a.Contains(".."):
                            _current = _current.Parent ?? _current;

                            break;
                        // stop
                        case "stop": break;
                        default:
                            Console.WriteLine("Instruction non comprise, veuillez vérifier la syntaxe de la commande ou le parametre");
                            break;
                    }
                }
                // Error when typing the choice (like i==banana)
                catch { Console.WriteLine("erreur, veuillez recommencer"); }


            }
        }
    }
}

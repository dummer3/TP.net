using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Text;

namespace ClassLibrary_Serialisation
{
    public static class Global
    {
        public static string Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\gestionnaireContact\\";

        static Global(){
            try
            {
                if (!Directory.Exists(Global.Path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(Global.Path);
                    Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(Global.Path));

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create folder: {0}", e.ToString());
            }
        }

        public static string entireFileName(string fileName)
        {
            return $"{Global.Path}{fileName}_{Environment.UserName}_{WindowsIdentity.GetCurrent().User}";
        }

        public static T VerificationFile<T>(Func<string, T> func, string fileName)
        {
            try
            {
                return func(fileName);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("File " + fileName + " not found\n" + ex.Message);
                throw (ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("You don't have access to " + fileName + "\n" + ex.Message);
                throw (ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw (ex);
            }
        }
        public static void VerificationFile(Action<string> act, string fileName)
        {
            try
            {
                act(fileName);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("File " + fileName + " not found\n" + ex.Message);
                throw (ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("You don't have access to " + fileName + "\n" + ex.Message);
                throw (ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw (ex);
            }
        }

    }
}

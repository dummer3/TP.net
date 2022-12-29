using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Text;

namespace ClassLibrary_Serialisation
{
    /// <summary>
    /// <c>PathInformation</c>: static class to manage the Path of all our files, and verify exceptions
    /// </summary>
    public static class PathInformation
    {
        /// <value> Path Where our files must be stock</value>
        public static string Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\gestionnaireContact\\";

        /// <summary>
        /// Verify is the folder where we work exist, if not create it
        /// </summary>
        static PathInformation(){
            try
            {
                if (!Directory.Exists(PathInformation.Path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(PathInformation.Path);
                    Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(PathInformation.Path));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create folder: {0}", e.ToString());
            }
        }

        /// <summary>
        /// Return the absolute path using the file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string EntireFileName(string fileName)
        {
            return $"{PathInformation.Path}{fileName}_{Environment.UserName}_{WindowsIdentity.GetCurrent().User}";
        }

        /// <summary>
        /// Verify a <c>Func</c> using a file don't throw an exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"> function to execute</param>
        /// <param name="fileName"> name of the file we work on</param>
        /// <returns> The result of the function s</returns>
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
        /// <summary>
        /// Verify an <c>Action</c> using a file don't throw an exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="act"> Action to execute</param>
        /// <param name="fileName"> name of the file we work on</param>
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

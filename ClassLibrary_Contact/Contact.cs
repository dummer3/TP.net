using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;


namespace ClassLibrary_Contact
{
    /// <summary>
    /// <c>Lien</c> is an enumeration to represent social link between <c>Contact</c>
    /// </summary>
    public enum Link { AMI, COLLEGUE, RELATION, RESEAU, NONDEFINI };

    /// <summary>
    /// <c>Contact</c>: class which represent an indivudual 
    /// </summary>
    [Serializable]
    public class Contact : Content
    {
        /// <value> Name of the indivudual </value>
        public string Name { get; set; }

        /// <value> Fist name of the indivudual </value>
        public string FirstName { get; set; }

        /// <value> Society of the indivudual </value>
        public string Society { get; set; }

        /// <value> Link of the indivudual </value>
        public Link Link { get; set; }

        /// <value> The email of the indivudual </value>
        private readonly string _email;

        /// <summary>
        /// verify if a string can be an email
        /// </summary>
        /// <param name="s"> the string to verify</param>
        /// <returns> s if it's an email, unknown else</returns>
        public static string CourrielStr(string s)
        {
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
        /// <summary>
        /// Default constructor
        /// By default, the email and link are unknown, everything else is ___
        /// </summary>
        public Contact() : this("___", "___", "unknown", "___", Link.NONDEFINI, DateTime.Now, DateTime.Now) { }

        /// <summary>
        /// Constructor with every parameters except the DateTimes
        /// </summary>
        /// <param name="name"></param>
        /// <param name="firstName"></param>
        /// <param name="email"></param>
        /// <param name="society"></param>
        /// <param name="link"></param>
        public Contact(string name, string firstName, string email, string society, Link link) : this(name, firstName, email, society, link, DateTime.Now, DateTime.Now) { }

        /// <summary>
        /// Constructor with all the parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="firsName"></param>
        /// <param name="email"></param>
        /// <param name="society"></param>
        /// <param name="link"></param>
        /// <param name="creationTime"></param>
        /// <param name="modificationTime"></param>
        public Contact(string name, string firsName, string email, string society, Link link, DateTime creationTime, DateTime modificationTime) : base(creationTime, modificationTime)
        {
            Name = name;
            FirstName = firsName;
            this._email = CourrielStr(email);
            Society = society;
            Link = link;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns> Representation of our contact with a string</returns>
        public override string ToString()
        {
            return $" Contact [{CreationTime}|{ModificationTime}] {Name} {FirstName} -- Email {_email} -- Lien {Link} -- Societe {Society}{System.Environment.NewLine}";
        }
    }
}

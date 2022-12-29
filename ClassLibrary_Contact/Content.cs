using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ClassLibrary_Contact
{


    /// <summary>
    /// <c>Content</c>: Class which represent every object which can be manage.
    /// </summary>
    [XmlInclude(typeof(Directory))]
    [XmlInclude(typeof(Contact))]
    [Serializable]
    public abstract class Content
    {
        /// <value><c>CreationTime</c>, Creation Time of this Content</value>
        [XmlElement(DataType = "dateTime")]
        public DateTime CreationTime { get; set; }

        /// <value><c>ModificationTime</c>, Modification Time of this Content</value>
        [XmlElement(DataType = "dateTime")]
        public DateTime ModificationTime { get; set; }

        /// <summary>
        /// Default Constructor
        /// By default the dates are Now
        /// </summary>
        public Content()
        {
            CreationTime = ModificationTime = DateTime.Now;
        }

        /// <summary>
        /// Constructor to modify the dates
        /// </summary>
        /// <param name="CreationTime"></param>
        /// <param name="ModificationTime"></param>
        public Content(DateTime CreationTime, DateTime ModificationTime)
        {
            this.CreationTime = CreationTime;
            this.ModificationTime = ModificationTime;
        }

        /// <summary>
        /// static method to obtain a <c>DateTime</c> from a formated string 
        /// </summary>
        /// <param name="value"> The format must be dd:mm:yyyy or dd/mm/yyyy </param>
        /// <returns> The corresponding <c>DateTime</c> </returns>
        /// <exception cref="ArgumentOutOfRangeException"> In the case the string is not well formated</exception>
        /// <exception cref="ArgumentNullException"> In the case the string is not well formated</exception>
        public static DateTime DateFromString(string value)
        {
            try
            {
                int[] i = value.Split(':', '/').Select(int.Parse).ToArray();
                DateTime d = new DateTime(i[2], i[1], i[0]);
                return d;
            }
            catch (Exception e) { throw e; }
        }
    }
}

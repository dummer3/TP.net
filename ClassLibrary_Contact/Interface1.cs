using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary_Contact
{
    public interface IStockable
    {
        public DateTime CreationTime { get; set; }
        public DateTime ModificationTime { get; set; }
    }
}

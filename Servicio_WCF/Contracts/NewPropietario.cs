using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Servicio_WCF.Contracts
{
    [DataContract]
    public class NewPropietario
    {
        [DataMember]
        public string firstName { get; set; }

        [DataMember]
        public string lastName { get; set; }

        [DataMember]
        public string cellphone { get; set; }

        [DataMember]
        public string adress { get; set; }

        [DataMember]
        public string photo { get; set; }

        [DataMember]
        public int documentType { get; set; }

        [DataMember]
        public string documentNumber { get; set; }
    }
}
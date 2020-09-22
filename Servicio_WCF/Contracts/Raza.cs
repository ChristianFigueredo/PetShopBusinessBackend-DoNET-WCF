using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Servicio_WCF.Contracts
{
    [DataContract]
    public class Raza
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string descripcion { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Servicio_WCF.Contracts
{
    [DataContract]
    public class NewMascota
    {
        [DataMember]
        public string nombre { get; set; }

        [DataMember]
        public DateTime fecha { get; set; }

        [DataMember]
        public int raza { set; get; }

        [DataMember]
        public string photo { get; set; }

        [DataMember]
        public int propietario { get; set; }
    }
}
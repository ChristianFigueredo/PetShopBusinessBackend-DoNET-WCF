using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Servicio_WCF.Contracts
{
    [DataContract]
    public class TipoDocumento
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Acronimo { get; set; }

        [DataMember]
        public string Descripcion { get; set; }
    }
}
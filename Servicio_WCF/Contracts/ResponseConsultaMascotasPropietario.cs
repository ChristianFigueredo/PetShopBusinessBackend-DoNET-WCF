using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Servicio_WCF.Contracts
{
    [DataContract]
    public class ResponseConsultaMascotasPropietario
    {
        [DataMember]
        public Mensaje mensaje { get; set; }

        [DataMember]
        public List<Mascota> listaMascotas { get; set; }
    }
}
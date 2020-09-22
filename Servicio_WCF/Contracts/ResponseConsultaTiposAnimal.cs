using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Servicio_WCF.Contracts
{
    [DataContract]
    public class ResponseConsultaTiposAnimal
    {
        [DataMember]
        public List<TipoAnimal> listaTiposAnimal { get; set; }

        [DataMember]
        public Mensaje mensaje { get; set; }
    }
}
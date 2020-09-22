using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Servicio_WCF.Contracts
{
    [DataContract]
    public class Propietario
    {
        [DataMember]
        public string numeroTelefono { get; set; }

        [DataMember]
        public string nombre { get; set; }

        [DataMember]
        public string Apellido { get; set; }

        public Propietario(string firstName, string lastName, string cellphone) {
            numeroTelefono = cellphone;
            nombre = firstName;
            Apellido = lastName;
            return;
        }
    }
}
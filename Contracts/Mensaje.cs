using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Servicio_WCF.Contracts
{
    [DataContract]
    public class Mensaje
    {
        [DataMember]
        public string codigo { get; set; }

        [DataMember]
        public string mensaje { get; set; }

        [DataMember]
        public string exceptionTrace { get; set; }

        public Mensaje (string cod, string txt, string ex)
        {
            codigo = cod;
            mensaje = txt;
            exceptionTrace = ex;
            return;
        }
    }
}
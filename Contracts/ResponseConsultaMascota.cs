using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Servicio_WCF.Contracts
{
    public class ResponseConsultaMascota
    {
        public Propietario propietario { set; get; }

        public Mascota mascota { get; set; } 

        public Mensaje mensaje { get; set; } 
    }
}
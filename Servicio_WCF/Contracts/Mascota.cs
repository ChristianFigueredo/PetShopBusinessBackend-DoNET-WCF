using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Servicio_WCF.Contracts
{
    [DataContract]
    public class Mascota
    {
        [DataMember]
        public int identificador { get; set; }

        [DataMember]
        public string nombre { set; get; }

        [DataMember]
        public DateTime fechaNacimiento {get; set; }

        [DataMember]
        public string raza { get; set; }

        [DataMember]
        public string tipoAnimal { get; set; }

        [DataMember]
        public int edad
        {
            get
            {
                DateTime now = DateTime.Today;
                int edad = DateTime.Today.Year - fechaNacimiento.Year;

                if (DateTime.Today < fechaNacimiento.AddYears(edad))
                    return --edad;
                else
                    return edad;
            }

            set { }
        }

        public Mascota()
        {

        }

        public Mascota(int id, string name, string tipo, string raz, DateTime fecha)
        {
            identificador = id;
            nombre = name;
            tipoAnimal = tipo;
            raza = raz;
            fechaNacimiento = fecha;
            return;
        }

    }
}
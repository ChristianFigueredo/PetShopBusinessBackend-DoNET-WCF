using Servicio_WCF.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Servicio_WCF
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        ResponseConsultaMascotasPropietario GetMascotasPropietario(string documentType, string documentNumber);

        [OperationContract]
        ResponseConsultaMascota GetInformacionMascota(int identificadorMascota);

        [OperationContract]
        Mensaje CreatePropietario(NewPropietario nP);

        [OperationContract] 
        ResponseConsultaTipoDocumento GetTiposDocumento();

        [OperationContract]
        Mensaje CreateMascota(NewMascota mascota);

        [OperationContract]
        ResponseConsultaRazas GetRazas();

        [OperationContract]
        ResponseConsultaTiposAnimal GetTiposAnimal();

    }
}

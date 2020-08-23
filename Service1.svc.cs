using Servicio_WCF.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Servicio_WCF.Models;

namespace Servicio_WCF
{
    public class Service1 : IService1
    {
        public ResponseConsultaMascota GetInformacionMascota(int identificadorMascota)
        {
            ResponseConsultaMascota response = new ResponseConsultaMascota();
            try
            { 
                using (Pet_Shop_BusinessEntities db = new Pet_Shop_BusinessEntities())
                {
                    var resultQuery = (from x in db.Pets
                                       join y in db.Races
                                       on x.Id_Race equals y.Id
                                       join z in db.Type_Animals
                                       on y.Id_Type_Animal equals z.Id
                                       join v in db.Persons
                                       on x.Id_Person equals v.Id
                                       where x.Id == identificadorMascota
                                       select new
                                       {
                                           identificador = x.Id,
                                           nombre = x.Pet_Name,
                                           tipoAnimal = z.Description_Animal,
                                           raza = y.Description_Race,
                                           fechaNacimiento = x.Day_Birth,
                                           nombreP = v.FirstName,
                                           apellidoP = v.LastName,
                                           telefono = v.Cellphone
                                       }).FirstOrDefault();

                    if (resultQuery != null)
                    {
                        response.mascota = new Mascota(resultQuery.identificador, resultQuery.nombre, resultQuery.tipoAnimal, resultQuery.raza, resultQuery.fechaNacimiento);
                        response.propietario = new Propietario(resultQuery.nombreP, resultQuery.apellidoP, resultQuery.telefono);
                        response.mensaje = new Mensaje("0000", "Consulta exitosa", null);
                    }
                    else
                    {
                        response.mensaje = new Mensaje("0004", "No se encontro informacion asociada al identificador: " + identificadorMascota, null);
                    }
                }
            }
            catch (Exception ex)
            {
                response.mensaje = new Mensaje("0001", "Ocurrio un error inesperado durante el proceso, por favor pongase en contacto con el administrador del sistema", ex.Message);
            }
            return response;
        }

        public ResponseConsultaMascotasPropietario GetMascotasPropietario(string documentType, string documentNumber)
        {
            ResponseConsultaMascotasPropietario response = new ResponseConsultaMascotasPropietario();

            using (Pet_Shop_BusinessEntities db = new Pet_Shop_BusinessEntities())
            {
                try
                {
                    var dType = db.Document_Types.FirstOrDefault(x => x.Acronym == documentType);

                    if (dType != null)
                    {
                        var amo = db.Persons.FirstOrDefault(x => x.Document_Number == documentNumber && x.Id_Document_Type == dType.Id);

                        if (amo != null)
                        {
                            var mascotas = (from a in db.Pets
                                            join b in db.Races
                                            on a.Id_Race equals b.Id
                                            join c in db.Type_Animals
                                            on b.Id_Type_Animal equals c.Id
                                            select new Mascota
                                            {
                                                identificador = a.Id,
                                                nombre = a.Pet_Name,
                                                tipoAnimal = c.Description_Animal,
                                                raza = b.Description_Race,
                                                fechaNacimiento = a.Day_Birth
                                            }).ToList();

                            
                            response.mensaje = new Mensaje( "0000", "Consulta exitosa", null);
                            response.listaMascotas = mascotas;
                        }
                        else
                        {
                            response.mensaje = new Mensaje("0003", "Ocurrio un error. La persona registrada con el numero de documento no existe en base de datos", null);
                        }                  
                    }
                    else
                    {
                        response.mensaje = new Mensaje("0002", "Ocurrio un error. El tipo de documento " + documentType + " no existe en base de datos", null);
                    }
                    
                }
                catch (Exception ex)
                {
                    response.mensaje = new Mensaje("0001", "Ocurrio un error inesperado durante el proceso, por favor pongase en contacto con el administrador del sistema", ex.Message);
                }                       
            }
            return response;
        }
    }
}

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
                response.mensaje = new Mensaje("0005", "Ocurrio un error inesperado durante el proceso, por favor pongase en contacto con el administrador del sistema", ex.Message);
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

        public Mensaje CreatePropietario(NewPropietario nP)
        {
            Mensaje msj = null;
            try
            {
                if (!String.IsNullOrEmpty(nP.firstName) && !String.IsNullOrEmpty(nP.lastName) && !String.IsNullOrEmpty(nP.cellphone) && !String.IsNullOrEmpty(nP.adress) && !String.IsNullOrEmpty(nP.photo) && nP.documentType != 0 && !String.IsNullOrEmpty(nP.documentNumber) && !String.IsNullOrEmpty(nP.photo))
                {
                    Persons persona = new Persons()
                    {
                        FirstName = nP.firstName,
                        LastName = nP.lastName,
                        Cellphone = nP.cellphone,
                        Adress = nP.adress,
                        Id_Document_Type = nP.documentType,
                        Document_Number = nP.documentNumber,
                        Photo = nP.photo
                    };

                    using (Pet_Shop_BusinessEntities db = new Pet_Shop_BusinessEntities())
                    {
                        var dT = db.Document_Types.Where(x => x.Id == persona.Id);
                        if (dT != null)
                        {
                            var prop = db.Persons.Where(x => x.Document_Number == persona.Document_Number && x.Id_Document_Type == persona.Id_Document_Type).FirstOrDefault();
                            if (prop == null)
                            {
                                db.Persons.Add(persona);
                                db.SaveChanges();
                                msj = new Mensaje("0000", "Transaccion exitosa", null);
                            }
                            else
                            {
                                msj = new Mensaje("0006", "Ocurrio un error. El propietario ya existe", null);
                            } 
                        }
                        else
                        {
                            msj = new Mensaje("0007", "Ocurrio un error. El tipo de documento especificado no esta parametrizado en la base de datos", null);
                        }
                    }
                }
                else
                {
                    msj = new Mensaje("0008", "Ocurrio un error. Todos los campos son requeridos en el registro", null);
                }
            }
            catch (Exception ex)
            {
                msj = new Mensaje("0009", "Ocurrio un error: ", ex.Message);
            }
            return msj;
        }

        public ResponseConsultaTipoDocumento GetTiposDocumento()
        {
            ResponseConsultaTipoDocumento response = new ResponseConsultaTipoDocumento();
            List<TipoDocumento> listaDocumentoResponse = new List<TipoDocumento>();
            List<Document_Types> docs = null;
            try
            {
                using (Pet_Shop_BusinessEntities db = new Pet_Shop_BusinessEntities())
                {
                    docs = db.Document_Types.ToList();
                }

                foreach ( var doc in docs)
                {
                    TipoDocumento tD = new TipoDocumento();
                    tD.Id = doc.Id;
                    tD.Acronimo = doc.Acronym;
                    tD.Descripcion = doc.Document_Description;
                    listaDocumentoResponse.Add(tD);
                }
                response.listaTipoDocumento = listaDocumentoResponse;
                response.mensaje = new Mensaje("0000", "Consulta exitosa", null);
            }
            catch (Exception ex)
            {
                response.mensaje = new Mensaje("0010", "Ocurrio un error: ", ex.Message);
            }
            return response;
        }

        public Mensaje CreateMascota( NewMascota mascota)
        {
            Mensaje msj = null;
            try
            {
                if (!string.IsNullOrEmpty(mascota.nombre) && !string.IsNullOrEmpty(mascota.photo) && !string.IsNullOrEmpty(mascota.photo) && mascota.raza > 0 && mascota.propietario > 0)
                {
                    Pets pet = new Pets()
                    {
                        Pet_Name = mascota.nombre,
                        Day_Birth = mascota.fecha,
                        Id_Race = mascota.raza,
                        Id_Person = mascota.propietario,
                        Photo = mascota.photo
                    };

                    using (Pet_Shop_BusinessEntities db = new Pet_Shop_BusinessEntities())
                    {
                        db.Pets.Add(pet);
                        db.SaveChanges();
                    }
                    msj = new Mensaje("0000", "Transaccion exitosa", null);

                }
                else
                {
                    msj = new Mensaje("0011", "Ocurrio un error. Todos los campos son requeridos en el registro", null);
                }
            }
            catch (Exception ex)
            {
                msj = new Mensaje("0012", "Ocurrio un error: ", ex.Message);
            }
            return msj;
        }

        public ResponseConsultaRazas GetRazas()
        {
            ResponseConsultaRazas response = new ResponseConsultaRazas();
            List<Raza> listaRazasResponse = new List<Raza>();
            List<Races> races = null;
            try
            {
                using (Pet_Shop_BusinessEntities db = new Pet_Shop_BusinessEntities())
                {
                    races = db.Races.ToList();
                }

                foreach (var r in races)
                {
                    Raza raza = new Raza();
                    raza.id = r.Id;
                    raza.descripcion = r.Description_Race;
                    listaRazasResponse.Add(raza);
                }
                response.listaRazas = listaRazasResponse;
                response.mensaje = new Mensaje("0000", "Consulta exitosa", null);
            }
            catch (Exception ex)
            {
                response.mensaje = new Mensaje("0013", "Ocurrio un error: ", ex.Message);
            }
            return response;
        }


        public ResponseConsultaTiposAnimal GetTiposAnimal()
        {
            ResponseConsultaTiposAnimal response = new ResponseConsultaTiposAnimal();
            List<TipoAnimal> listaTiposAnimal = new List<TipoAnimal>();
            List<Type_Animals> animalTypes = null;
            try
            {
                using (Pet_Shop_BusinessEntities db = new Pet_Shop_BusinessEntities())
                {
                    animalTypes = db.Type_Animals.ToList();
                }

                foreach (var r in animalTypes)
                {
                    TipoAnimal animal = new TipoAnimal();
                    animal.id = r.Id;
                    animal.descripcion = r.Description_Animal;
                    listaTiposAnimal.Add(animal);
                }
                response.listaTiposAnimal = listaTiposAnimal;
                response.mensaje = new Mensaje("0000", "Consulta exitosa", null);
            }
            catch (Exception ex)
            {
                response.mensaje = new Mensaje("0013", "Ocurrio un error: ", ex.Message);
            }
            return response;
        }


    }
}

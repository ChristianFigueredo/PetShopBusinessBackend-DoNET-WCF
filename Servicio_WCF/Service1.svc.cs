using Servicio_WCF.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    var resultQuery = (from x in db.Pet
                                       join y in db.Race
                                       on x.Id_Race equals y.Id
                                       join z in db.AnimalType
                                       on y.Id_Type_Animal equals z.Id
                                       join v in db.Person
                                       on x.Id_Person equals v.Id
                                       where x.Id == identificadorMascota
                                       select new
                                       {
                                           identificador = x.Id,
                                           nombre = x.Pet_Name,
                                           tipoAnimal = z.Description,
                                           raza = y.Description,
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
                    var dType = db.DocumentType.FirstOrDefault(x => x.Acronym == documentType);

                    if (dType != null)
                    {
                        var amo = db.Person.FirstOrDefault(x => x.Document_Number == documentNumber && x.Id_Document_Type == dType.Id);

                        if (amo != null)
                        {
                            var mascotas = (from a in db.Pet
                                            join b in db.Race
                                            on a.Id_Race equals b.Id
                                            join c in db.AnimalType
                                            on b.Id_Type_Animal equals c.Id
                                            select new Mascota
                                            {
                                                identificador = a.Id,
                                                nombre = a.Pet_Name,
                                                tipoAnimal = c.Description,
                                                raza = b.Description,
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
                if (!String.IsNullOrEmpty(nP.firstName) && !String.IsNullOrEmpty(nP.lastName) && !String.IsNullOrEmpty(nP.cellphone) && !String.IsNullOrEmpty(nP.adress) && !String.IsNullOrEmpty(nP.photo) && nP.documentType != 0 && !String.IsNullOrEmpty(nP.documentNumber) && !String.IsNullOrEmpty(nP.photo) && !String.IsNullOrEmpty(nP.email))
                {
                    Person persona = new Person()
                    {
                        FirstName = nP.firstName,
                        LastName = nP.lastName,
                        Cellphone = nP.cellphone,
                        Adress = nP.adress,
                        Id_Document_Type = nP.documentType,
                        Document_Number = nP.documentNumber,
                        Photo = nP.photo,
                        Email = nP.email,
                    };

                    using (Pet_Shop_BusinessEntities db = new Pet_Shop_BusinessEntities())
                    {
                        DocumentType dT = db.DocumentType.Where(x => x.Id == persona.Id_Document_Type).FirstOrDefault();
                        if (dT != null)
                        {
                            var prop = db.Person.Where(x => x.Document_Number == persona.Document_Number && x.Id_Document_Type == persona.Id_Document_Type).FirstOrDefault();
                            if (prop == null)
                            {
                                db.Person.Add(persona);
                                var id_person = db.SaveChanges();

                                PersonStateHistory personState = new PersonStateHistory() { TransactionDate = DateTime.Now, Id_Person = id_person, Id_State = 1 };
                                db.PersonStateHistory.Add(personState);
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
            List<DocumentType> docs = null;
            try
            {
                using (Pet_Shop_BusinessEntities db = new Pet_Shop_BusinessEntities())
                {
                    docs = db.DocumentType.ToList();
                }

                foreach ( var doc in docs)
                {
                    TipoDocumento tD = new TipoDocumento();
                    tD.Id = doc.Id;
                    tD.Acronimo = doc.Acronym;
                    tD.Descripcion = doc.Description;
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
                    Pet pet = new Pet()
                    {
                        Pet_Name = mascota.nombre,
                        Day_Birth = mascota.fecha,
                        Id_Race = mascota.raza,
                        Id_Person = mascota.propietario,
                        Photo = mascota.photo
                    };

                    using (Pet_Shop_BusinessEntities db = new Pet_Shop_BusinessEntities())
                    {
                        db.Pet.Add(pet);
                        var id_pet = db.SaveChanges();

                        PetStateHistory petState = new PetStateHistory() {
                            TransactionDate = DateTime.Now,
                            Id_Person = mascota.propietario,
                            Id_Pet = id_pet,
                            Id_State = 3,
                        };
                        db.PetStateHistory.Add(petState);
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
            List<Race> races = null;
            try
            {
                using (Pet_Shop_BusinessEntities db = new Pet_Shop_BusinessEntities())
                {
                    races = db.Race.ToList();
                }

                foreach (var r in races)
                {
                    Raza raza = new Raza();
                    raza.id = r.Id;
                    raza.descripcion = r.Description;
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
            List<AnimalType> animalTypes = null;
            try
            {
                using (Pet_Shop_BusinessEntities db = new Pet_Shop_BusinessEntities())
                {
                    animalTypes = db.AnimalType.ToList();
                }

                foreach (var r in animalTypes)
                {
                    TipoAnimal animal = new TipoAnimal();
                    animal.id = r.Id;
                    animal.descripcion = r.Description;
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

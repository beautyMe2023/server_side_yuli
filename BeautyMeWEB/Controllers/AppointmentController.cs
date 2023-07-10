﻿using BeautyMe;
using BeautyMeWEB.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using System.Data.Entity;
using HttpDeleteAttribute = System.Web.Http.HttpDeleteAttribute;
using HttpPutAttribute = System.Web.Http.HttpPutAttribute;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;

namespace BeautyMeWEB.Controllers
{
    public class AppointmentController : ApiController
    {
        // GET: Appointment
        BeautyMeDBContext db = new BeautyMeDBContext();

        [HttpGet]
        [Route("api/Appointment/AllAppointment")]
        public HttpResponseMessage GetAllAppointment()
        {
            List<AppointmentDTO> AllAppointment = db.Appointment.Select(x => new AppointmentDTO
            {
                Number_appointment = x.Number_appointment,
                Date = x.Date,
                Start_Hour = (double)x.Start_Hour,
                End_Hour = (double)x.Start_Hour,
                Is_client_house = x.Is_client_house,
                Business_Number = x.Business_Number,
                Appointment_status = x.Appointment_status,
            }).ToList();
            if (AllAppointment != null)
                return Request.CreateResponse(HttpStatusCode.OK, AllAppointment);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        // Post: Appointment
        [HttpPost]
        [Route("api/Appointment/AllAppointmentForBussines/{Business_Numberr}")]
        public HttpResponseMessage GetAllAppointmentForBussines(int Business_Numberr)
        {
            List<AppointmentDTO> AllAppointment = db.Appointment.Where(a => a.Business_Number == Business_Numberr).Select(x => new AppointmentDTO
            {
                Number_appointment = x.Number_appointment,
                Date = x.Date,
                Start_Hour = (double)x.Start_Hour,
                End_Hour = (double)x.Start_Hour,
                Is_client_house = x.Is_client_house,
                Business_Number = x.Business_Number,
                Appointment_status = x.Appointment_status,
                ID_Client = x.ID_Client,
            }).ToList();
            if (AllAppointment != null)
                return Request.CreateResponse(HttpStatusCode.OK, AllAppointment);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);
        }
        [HttpGet]
        [Route("api/Appointment/AllAppointmentForBussines/{Business_Number}")]
        public HttpResponseMessage GetAllAppointmentForClient(int Business_Number)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["BeautyMeDB"].ConnectionString);
            string query = @"select a.*,c.*
                            from Appointment a inner join Client c on c.ID_number=a.ID_Client
                            where a.Business_Number=@Business_Number and a.Appointment_status='Appointment_ended'";

            SqlDataAdapter adpter = new SqlDataAdapter(query, con);
            adpter.SelectCommand.Parameters.AddWithValue("@Business_Number", Business_Number);
            DataSet ds = new DataSet();
            adpter.Fill(ds, "Appointment");
            DataTable dt = ds.Tables["Appointment"];
            return Request.CreateResponse(HttpStatusCode.OK, dt);
        }
        //מחזיר את כל התורים של בעל העסק עם פרטי לקוח
        [HttpGet]
        [Route("api/Appointment/GetAllAppointmentForProWithClient/{Business_Number}")]
        public HttpResponseMessage GetAllAppointmentForProWithClient(int Business_Number)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["BeautyMeDB"].ConnectionString);
            string query = @"select a.*,c.*,t.*
                            from Appointment a inner join Client c on c.ID_number=a.ID_Client
                            left join Type_Treatment t on t.Type_treatment_Number=a.Type_treatment_Number
                            where a.Business_Number=@Business_Number";

            SqlDataAdapter adpter = new SqlDataAdapter(query, con);
            adpter.SelectCommand.Parameters.AddWithValue("@Business_Number", Business_Number);
            DataSet ds = new DataSet();
            adpter.Fill(ds, "Appointment");
            DataTable dt = ds.Tables["Appointment"];
            return Request.CreateResponse(HttpStatusCode.OK, dt);
        }


        //פונקציה חדשה ל"תורים שלי" של הלקוח - אמור להחזיר את כל התורים של הלקוח עם משהו שקשור לביקורות כדי שיהיה אפשר לדרג... 
        // // מידע על התורים של הלקוח כולל טבלת ביקורות (לשימוש לכפתור דרג עסק) עם אינר ג'וין 
        //[HttpGet]
        //[Route("api/BusinessReview/AllAppointmentForClient/{clientID}")]/
        // השלוש שורות האחרונות זה הסבר והפונקציה הקודמת... עדיין נמצאת במקום שלה... 
        [HttpGet]
        [Route("api/Appointment/AllAppointmentForClientt/{clientID}")]
        public HttpResponseMessage AllAppointmentForClientt(string clientID)
        {
            //string ID_Client = clientID;
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["BeautyMeDB"].ConnectionString);
            string query = @"select a.*  ,br.*,b.*
                        from Appointment a inner join Review_Business br on br.Number_appointment=a.Number_appointment 
                                        inner join Business b on b.Business_Number=br.Business_Number
                        where A.ID_Client=@ID_Client";

            SqlDataAdapter adpter = new SqlDataAdapter(query, con);
            adpter.SelectCommand.Parameters.AddWithValue("@ID_Client", clientID);
            //adpter.SelectCommand.Parameters.AddWithValue("@ID_Client", ID_Client);
            DataSet ds = new DataSet();
            adpter.Fill(ds, "Appointment");
            DataTable dt = ds.Tables["Appointment"];
            return Request.CreateResponse(HttpStatusCode.OK, dt);
        }




        // Post: api/Post
        [HttpPost]
        [Route("api/Appointment/NewAppointment")]
        public HttpResponseMessage PostNewAppointment([FromBody] AppointmentDTO x)
        {
            try
            {
                Appointment newAppointment = new Appointment()
                {
                    //Number_appointment = x.Number_appointment,
                    Date = x.Date,
                    Start_Hour = x.Start_Hour,
                    End_Hour = x.End_Hour,
                    Is_client_house = x.Is_client_house,
                    Business_Number = x.Business_Number,
                    Appointment_status = x.Appointment_status,
                    Type_Treatment_Number = x.Type_Treatment_Number

                };
                db.Appointment.Add(newAppointment);
                db.SaveChanges();
                int newAppointmentId = newAppointment.Number_appointment;
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "New appointment added to the database", appointmentId = newAppointmentId });
            }
            catch (DbUpdateException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while adding new Appointment to the database: " + ex.InnerException.InnerException.Message);
            }
        }
        //[HttpPost]
        //[Route("api/Appointment/ClientToAppointment")]
        //public HttpResponseMessage ClientToAppointment([FromBody] AppointmentDTO x)
        //{
        //    Appointment appointment = db.Appointment.Where(z=>z.Number_appointment== x.Number_appointment && z.Appointment_status== "Available" && z.ID_Client==null).FirstOrDefault();
        //    if (appointment != null)
        //    {
        //        appointment.ID_Client= x.ID_Client;
        //        appointment.Appointment_status = "Awaiting_approval";
        //        db.SaveChanges();
        //        return Request.CreateResponse(HttpStatusCode.OK, $"{x.ID_Client} is assigned to {appointment.Number_appointment}");
        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.NotFound, "error");

        //    }
        //}

        [HttpPost]
        [Route("api/Appointment/NewAppointmentByClient")]
        public HttpResponseMessage NewAppointmentByClient([FromBody] AppointmentDTO x)
        {

            Appointment a = new Appointment()
            {
                Date = x.Date,
                ID_Client = x.ID_Client,
                Appointment_status = "Awaiting_approval",
                End_Hour = x.End_Hour,
                Start_Hour = x.Start_Hour,
                Business_Number = x.Business_Number,
                Is_client_house = x.Is_client_house,
                Type_Treatment_Number = x.Type_Treatment_Number
            };
            if (x != null)
            {
                db.Appointment.Add(a);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, a.Number_appointment);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, $"{x}");
            }

        }


        ////תורים ללקוח
        //[HttpGet]
        //[Route("api/Appointment/AllAppointmentForClient/{ID_Client}")]
        //public HttpResponseMessage GetAllAppointmentForClient(string ID_Client)
        //{
        //    List<AppointmentDTO> AllAppointment = db.Appointment.Where(a => a.ID_Client == ID_Client && a.ID_Client!=null).Select(x => new AppointmentDTO
        //    {
        //        Number_appointment = x.Number_appointment,
        //        BusinessName=x.Business.Name,
        //        Date = x.Date,
        //        Start_time = x.Start_time,
        //        End_time = x.End_time,
        //        Is_client_house = x.Is_client_house,
        //        Business_Number = x.Business_Number,
        //        Appointment_status = x.Appointment_status,
        //        AddressStreet = x.Business.AddressStreet,
        //        AddressHouseNumber = x.Business.AddressHouseNumber,
        //        AddressCity = x.Business.AddressCity
        //    }).ToList();
        //    if (AllAppointment != null)
        //        return Request.CreateResponse(HttpStatusCode.OK, AllAppointment);
        //    else
        //        return Request.CreateResponse(HttpStatusCode.NotFound);
        //}
        // Put: api/Put
        [HttpPut]
        [Route("api/Appointment/UpdateAppointment")]
        public HttpResponseMessage PutUpdateAppointment([FromBody] AppointmentDTO x)
        {
            Appointment AppointmentToUpdate = db.Appointment.FirstOrDefault(a => a.Number_appointment == x.Number_appointment);
            if (AppointmentToUpdate == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Appointment with number {x.Number_appointment} not found.");
            }

            else
            {
                //AppointmentToUpdate.Number_appointment = x.Number_appointment;
                AppointmentToUpdate.Date = x.Date;
                AppointmentToUpdate.Start_Hour = x.Start_Hour;
                AppointmentToUpdate.End_Hour = x.End_Hour;
                AppointmentToUpdate.Is_client_house = x.Is_client_house;
                AppointmentToUpdate.Business_Number = x.Business_Number;
                AppointmentToUpdate.Appointment_status = x.Appointment_status;

                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "The Appointment update in the dataBase");
            }
        }


        // Delete: api/Delete
        [HttpDelete]
        [Route("api/Appointment/CanceleAppointment")]
        public IHttpActionResult DeleteCanceleAppointment([FromBody] AppointmentDTO x)
        {
            if (x == null)  // בדיקת תקינות ה-DTO שהתקבל
            {
                return BadRequest("הפרטים שהתקבלו אינם תקינים.");
            }

            Appointment CanceleAppointment = db.Appointment.Find(x.Number_appointment);   // חיפוש הרשומה המתאימה לפי המזהה שלה
            if (CanceleAppointment == null)
            {
                return NotFound();
            }

            db.Appointment.Remove(CanceleAppointment);   // מחיקת הרשומה מבסיס הנתונים

            db.SaveChanges();

            return Ok("הנתונים נמחקו בהצלחה.");  // החזרת תשובה מתאימה לפי המצב
        }
        // קונטרולר לשינוי הסטטוס לפי למאושר
        //[HttpPost]
        //[Route("api/Appointment/changeStatus/{Number_appointment}")]
        //public HttpResponseMessage ChangeStatusByClient(int Number_appointment)
        //{
        //    try
        //    {
        //        Appointment AppointmentToChangeStatus = db.Appointment.Where(x => x.Number_appointment == Number_appointment && x.Appointment_status == "Awaiting_approval").FirstOrDefault();
        //        if (AppointmentToChangeStatus == null)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.NotFound, $"${Number_appointment} is not found!");
        //        }
        //        else
        //        {
        //            AppointmentToChangeStatus.Appointment_status = "Confirmed";
        //            db.SaveChanges();
        //            return Request.CreateResponse(HttpStatusCode.OK, $"{AppointmentToChangeStatus.Number_appointment} updated in the dataBase");
        //        }
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        // log the detailed exception message to the console
        //        Console.WriteLine(ex.InnerException?.Message);
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while updating the entries.");
        //    }

        //}
        [HttpPost]
        [Route("api/Appointment/changeStatus/{Number_appointment}")]
        public HttpResponseMessage UpdateStatus(int Number_appointment)
        {
            try
            {
                Appointment a = db.Appointment.Where(x => x.Number_appointment == Number_appointment && x.Appointment_status ==
                "Awaiting_Approval").FirstOrDefault();
                if (a != null)
                {
                    a.Appointment_status = "Confirmed";
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, $"{a.Number_appointment} updated in the dataBase");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, $"{Number_appointment} was not found");
                }
            }
            catch (DbUpdateException ex)
            {
                foreach (DbEntityEntry entry in ex.Entries)
                {
                    Console.WriteLine("error in entry - " + entry.Entity.GetType().Name);
                    Console.WriteLine("entity state - " + entry.State);
                    Console.WriteLine("*********");
                    foreach (string prop in entry.CurrentValues.PropertyNames)
                    {
                        Console.WriteLine("for column - " + prop + ", value - " + entry.CurrentValues[prop]);
                    }
                    Console.WriteLine("==========");
                }
                // Log the exception details to the debug output
                System.Diagnostics.Debug.WriteLine(ex);
                throw;  // rethrow the exception for the framework to handle
            }
        }
        //ניסיוני
        [HttpPut]
        [Route("api/Appointment/UpdateToConfirm/{Number_appointment}")]
        public HttpResponseMessage UpdateToConfirm(int Number_appointment)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["BeautyMeDB"].ConnectionString);
            string query = @"UPDATE Appointment
                            SET Appointment_Status = 'Confirmed'
                            WHERE Number_appointment = @Number_appointment AND Appointment_Status = 'Awaiting_Approval';";

            SqlDataAdapter adpter = new SqlDataAdapter(query, con);
            adpter.SelectCommand.Parameters.AddWithValue("@Number_appointment", Number_appointment);
            DataSet ds = new DataSet();
            adpter.Fill(ds, "Appointment");
            DataTable dt = ds.Tables["Appointment"];
            return Request.CreateResponse(HttpStatusCode.OK, dt);
        }

        [HttpPost]
        [Route("api/Appointment/AllAppointmentForClient")]
        public HttpResponseMessage GetAllAppointmentForClient([FromBody] ClientDTO x)
        {
            string ID_Client = x.ID_number;
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["BeautyMeDB"].ConnectionString);
            string query = @"select A.*, p.token,B.*
             from Appointment A inner join Business B ON A.Business_Number=b.Business_Number inner join Client C 
             on A.ID_Client= c.ID_number inner join Professional p on p.ID_number=b.Professional_ID_number
             where ID_Client=@ID_Client";

            SqlDataAdapter adpter = new SqlDataAdapter(query, con);
            adpter.SelectCommand.Parameters.AddWithValue("@ID_Client", ID_Client);
            DataSet ds = new DataSet();
            adpter.Fill(ds, "Appointment");
            DataTable dt = ds.Tables["Appointment"];
            return Request.CreateResponse(HttpStatusCode.OK, dt);
        }

        [HttpPost]
        [Route("api/Appointment/AllAppointment")]
        public HttpResponseMessage GetAllAppointment([FromBody] object x)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["BeautyMeDB"].ConnectionString);
            string query = @"select A.*, p.token,B.*
             from Appointment A inner join Business B ON A.Business_Number=b.Business_Number  inner join Professional p on p.ID_number=b.Professional_ID_number";
            SqlDataAdapter adpter = new SqlDataAdapter(query, con);

            DataSet ds = new DataSet();
            adpter.Fill(ds, "Appointment2");
            DataTable dt = ds.Tables["Appointment2"];
            return Request.CreateResponse(HttpStatusCode.OK, dt);
        }


        [HttpGet]
        [Route("api/Appointment/AllAvailableAppointment")]
        public HttpResponseMessage AllAvailableAppointment()
        {
            List<AppointmentDTO> AllAppointment = db.Appointment.Where(x => x.Appointment_status == "available" && x.ID_Client == null).Select(a => new AppointmentDTO
            {
                Number_appointment = a.Number_appointment,
                Date = a.Date,
                Start_Hour = (double)a.Start_Hour,
                End_Hour = (double)a.Start_Hour,
                Is_client_house = a.Is_client_house,
                Business_Number = a.Business_Number,
                Appointment_status = a.Appointment_status
            }).ToList();
            if (AllAppointment != null)
                return Request.CreateResponse(HttpStatusCode.OK, AllAppointment);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);
        }
        //public class UserTypeModel
        //{
        //    public string UserType { get; set; }
        //}
        //לפני המחיקת שורה הנתונים יעברו לטבלת תורים מחוקים
        [HttpDelete]
        [Route("api/Appointment/CanceleAppointmentByClient/{appointmentNumber}")]
        public HttpResponseMessage CancelAppointmentByClient(int appointmentNumber)
        {
            //string userType = model.UserType;
            if (appointmentNumber.ToString() == null)  // בדיקת תקינות ה-DTO שהתקבל
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "הפרטים שהתקבלו אינם תקינים.");
            }
            else
            {
                Appointment CanceleAppointment = db.Appointment.Where(x => x.Number_appointment == appointmentNumber).FirstOrDefault();   // חיפוש הרשומה המתאימה לפי המזהה שלה
                //Canceled_Appointment canceled = new Canceled_Appointment()
                //{
                //    Number_appointment = CanceleAppointment.Number_appointment,
                //    Date = CanceleAppointment.Date,
                //    Business_Number = CanceleAppointment.Business_Number,
                //    Type_Treatment_Number = CanceleAppointment.Type_Treatment_Number,
                //    ID_Client = CanceleAppointment.ID_Client,
                //    Canceled_By = userType
                //};
                if (CanceleAppointment != null)
                {
                    db.Appointment.Remove(CanceleAppointment);   // מחיקת הרשומה מבסיס הנתונים
                                                                 //db.Canceled_Appointment.Add(canceled);
                    db.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, $"appointment {appointmentNumber} is deleted from Appointment table ");
                }  // החזרת תשובה מתאימה לפי המצב
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, $"coldn't find appointment number {appointmentNumber}");
                }
            }
        }

        [HttpGet]
        [Route("api/Appointment/OneAppointment/{appointment_number}")]
        public HttpResponseMessage GetOneAppointment(int appointment_number)
        {
            AppointmentDTO appointment = db.Appointment.Where(x => x.Number_appointment == appointment_number).Select(a => new AppointmentDTO
            {
                Date = a.Date,
                Start_Hour = (double)a.Start_Hour,
                End_Hour = (double)a.Start_Hour,
                Is_client_house = a.Is_client_house,
                Business_Number = a.Business_Number,
                Appointment_status = a.Appointment_status
            }).FirstOrDefault();
            if (appointment != null)
                return Request.CreateResponse(HttpStatusCode.OK, appointment);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);
        }
    }
}




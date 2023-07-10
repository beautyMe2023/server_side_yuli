using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeautyMeWEB.DTO
{
    public class Canceled_AppointmentDTO
    {
        public int Number_appointment;
        public System.DateTime Date;
        public int Business_Number;
        public Nullable<int> Type_Treatment_Number;
        public string ID_Client;
        public int Cancel_id;
        public string Canceled_By;

    }
}
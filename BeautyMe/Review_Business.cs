//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BeautyMe
{
    using System;
    using System.Collections.Generic;
    
    public partial class Review_Business
    {
        public int Review_Number { get; set; }
        public string Cleanliness { get; set; }
        public string Professionalism { get; set; }
        public string On_time { get; set; }
        public string Overall_rating { get; set; }
        public string Comment { get; set; }
        public string Client_ID_number { get; set; }
        public int Business_Number { get; set; }
        public Nullable<int> Number_appointment { get; set; }
        public string pic { get; set; }
    
        public virtual Appointment Appointment { get; set; }
    }
}

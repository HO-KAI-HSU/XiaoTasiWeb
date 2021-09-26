using System;
using System.ComponentModel.DataAnnotations;

namespace xiaotasi.Bo
{
    public class AddTravelReservationCheckBo
    {
        [Required]
        public string token { get; set; }

        [Required]
        public string travelReservationCode { get; set; }

        [Required]
        public string bankAccountCode { get; set; }

        [Required]
        public string travelReservationCheckPicPath { get; set; }
    }
}

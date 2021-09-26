using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace xiaotasi.Bo
{
    public class ApiHeader
    {
        [FromHeader]
        [Required]
        public string lang { get; set; }
    }
}

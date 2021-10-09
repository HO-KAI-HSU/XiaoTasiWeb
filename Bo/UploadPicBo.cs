using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace xiaotasi.Bo
{
    public class UploadPicBo
    {
        [Required]
        public string token { get; set; }

        [Required]
        public int picType { get; set; }

        [Required]
        public IFormFile file { get; set; }
    }

    public class UploadPicFromMgtBo
    {
        [Required]
        public int picType { get; set; }

        [Required]
        public IFormFile file { get; set; }
    }
}

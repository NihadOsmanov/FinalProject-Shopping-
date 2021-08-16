using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string VisaImage { get; set; }
        public string MastercardImage { get; set; }
        public string PaypalImage { get; set; }
        public string AmericanExpressImage { get; set; }
        public string DiscoverImage { get; set; }
    }
}

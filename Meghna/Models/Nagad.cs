using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deal.Models
{
   

    public class CallbackResponseObject
    {
        public string order_id { get; set; }
        public string payment_ref_id { get; set; }
        public string status { get; set; }
        public string status_code { get; set; }
        public string message { get; set; }
        public string payment_dt { get; set; }
        public string issuer_payment_ref { get; set; }
    }



}

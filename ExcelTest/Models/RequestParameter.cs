using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelTest.Models
{
    public class RequestParameter
    {
        public string HostName { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
        public object Headers { get; set; }
        public AuthenticatorBase Authenticator { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public int Timeout { get; set; } = 90000;
        public object RequestBodyData { get; set; }
        public string ContentType { get; set; } = "application/json";
    }
}

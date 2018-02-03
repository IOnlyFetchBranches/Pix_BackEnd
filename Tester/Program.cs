using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using pix_dtmodel;
using pix_dtmodel.Managers;
using pix_dtmodel.Models;
using pix_sec;
namespace Tester
{   
    class Program
    {

        

        static void Main(string[] args)
        {
            var tester = new pix_dtmodel.Managers.Firebase.FirebaseJWTAuth("AIzaSyBAbtFrjxR-dDNIVnSa1ilJEsE3XQuVVEQ");

            var actual3rdSection =
                "FSdADZypSVuLgkF-_WEfzB5BCsqwBB3l9y69zuZdP_qAX_tucTaeu2RhYZkpZb4G9xnnB8Q9EZnr8HRDzHDLds8mw11k0-027fOUNEilc-8wRqn6hVvsduxoTpY07cRHYTJohlyEga3f4QREehLBvt2SIFEH6NV2g0z2u0GQB0q0r_gFArmcciBFVDhfuhFBIqzAjRtlgQ1KoIj0YlVO7ysq3Ak2s1LpXqpgu2LLw2Z9L0LFvVu6P-arxl7gmG4e4XIvNhqlYN3n3fhbpeClZVBPz6CTFdx-SgT5FM1QcOPcSzh40ISFPzoGMONS1aVmT7E0wei-FXLaVQGAHvO5AQ\r\n";            
               var recieved3rdSection =
                "FSdADZypSVuLgkF+/WEfzB5BCsqwBB3l9y69zuZdP/qAX/tucTaeu2RhYZkpZb4G9xnnB8Q9EZnr8HRDzHDLds8mw11k0+027fOUNEilc+8wRqn6hVvsduxoTpY07cRHYTJohlyEga3f4QREehLBvt2SIFEH6NV2g0z2u0GQB0q0r/gFArmcciBFVDhfuhFBIqzAjRtlgQ1KoIj0YlVO7ysq3Ak2s1LpXqpgu2LLw2Z9L0LFvVu6P+arxl7gmG4e4XIvNhqlYN3n3fhbpeClZVBPz6CTFdx+SgT5FM1QcOPcSzh40ISFPzoGMONS1aVmT7E0wei+FXLaVQGAHvO5AQ";

            var testDecode = pix_dtmodel.Managers.Firebase.FirebaseJWTAuth.SafeB64Decode(recieved3rdSection);

            var good = getVer();
            

            while (true) { }
        }

        private static async Task<string> getVer()
        {

            Console.WriteLine("Testing verification.");
            var tester = new pix_dtmodel.Managers.Firebase.FirebaseJWTAuth("AIzaSyBAbtFrjxR-dDNIVnSa1ilJEsE3XQuVVEQ");
           var good= await tester.Verify(
                "eyJhbGciOiJSUzI1NiIsImtpZCI6ImNhNDYwYWY5NTFhY2NjMmRlNDc0NTAwZjc5NDk4OWE0M2RlNzMwNjMifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vcGl4LTU1ZTc2IiwiYXVkIjoicGl4LTU1ZTc2IiwiYXV0aF90aW1lIjoxNTE3NTM0MzE4LCJ1c2VyX2lkIjoiQjRoSnlkdzl3N2N4ZW1tbE01MUZSZXVza2VEMyIsInN1YiI6IkI0aEp5ZHc5dzdjeGVtbWxNNTFGUmV1c2tlRDMiLCJpYXQiOjE1MTc1MzQzMTgsImV4cCI6MTUxNzUzNzkxOCwiZW1haWwiOiJsbG11emljYWxAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbImxsbXV6aWNhbEBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.FSdADZypSVuLgkF-_WEfzB5BCsqwBB3l9y69zuZdP_qAX_tucTaeu2RhYZkpZb4G9xnnB8Q9EZnr8HRDzHDLds8mw11k0-027fOUNEilc-8wRqn6hVvsduxoTpY07cRHYTJohlyEga3f4QREehLBvt2SIFEH6NV2g0z2u0GQB0q0r_gFArmcciBFVDhfuhFBIqzAjRtlgQ1KoIj0YlVO7ysq3Ak2s1LpXqpgu2LLw2Z9L0LFvVu6P-arxl7gmG4e4XIvNhqlYN3n3fhbpeClZVBPz6CTFdx-SgT5FM1QcOPcSzh40ISFPzoGMONS1aVmT7E0wei-FXLaVQGAHvO5AQ");

            Console.WriteLine("Testing Complete \n\n");

            Console.WriteLine(good != null);

            return "ok";

        }
    }
}

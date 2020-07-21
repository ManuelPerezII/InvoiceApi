
using API.Invoice.DB;
using API.Invoice.Interfaces;
using API.Invoice.Profile;
using API.Invoice.Providers;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;


namespace API.Invoice.Controllers
{
    [RoutePrefix("api/home")]    
    public class HomeController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Index()
        {
            return Ok();
        }

        //[Route("Multiple")]
        //[HttpPost]        
        //public IHttpActionResult Multiple([FromForm]IFormFile file)
        //{
        //    var stream = file.OpenReadStream();
        //    var name = file.FileName;
        //    return Ok();
        //}

        [Route("PostInvoice")]
        [HttpPost]
        public IHttpActionResult PostInvoice()
        {            
            return Ok();
        }


        [Route("Upload")]
        [HttpPost]
        public HttpResponseMessage Upload()
        {
            var httpContext = HttpContext.Current;

            // Check for any uploaded file  
            if (httpContext.Request.Files.Count > 0)
            {
                //Loop through uploaded files  
                for (int i = 0; i < httpContext.Request.Files.Count; i++)
                {
                    HttpPostedFile httpPostedFile = httpContext.Request.Files[i];
                    if (httpPostedFile != null)
                    {
                        // Construct file save path  
                        var fileSavePath = "";//Path.Combine(HostingEnvironment.MapPath(ConfigurationManager.AppSettings["fileUploadFolder"]), httpPostedFile.FileName);
                                              //    var fileSavePath = Path.Combine(HostingEnvironment.MapPath(ConfigurationManager.AppSettings["fileUploadFolder"]), httpPostedFile.FileName);
                                              //    httpPostedFile.SaveAs(fileSavePath);
                                              // Save the uploaded file  
                        httpPostedFile.SaveAs(fileSavePath);
                    }
                }
            }

            // Return status code  
            return Request.CreateResponse(HttpStatusCode.Created);
        }
    }

    

        //public class HomeController : Controller
        //{
        //    public ActionResult Index()
        //    {
        //        ViewBag.Title = "Home Page";

        //        return View();
        //    }
        //}
 }

using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreVideo.Controllers
{
    [Route("company/[controller]/[action]")]
    public class EmployeeController : Controller
    {
        public string Index()
        {
            return "Hello from Employee";
        }
        
        public ContentResult Name()
        {
            return Content("Nathan");
        }
        
        public string Country()
        {
            return "United States of America";
        }
    }
}

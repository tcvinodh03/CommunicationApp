
using CommunicationAPI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationAPI.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[Controller]")]
    public class BaseApiController : ControllerBase 
    {
        
    }
}
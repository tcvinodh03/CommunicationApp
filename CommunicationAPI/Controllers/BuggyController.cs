using CommunicationAPI.Data;
using CommunicationAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationAPI.Controllers
{

    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "Secret Text ";

        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var item = _context.Users.Find(-1);
            if (item == null) return NotFound();
            return item;
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {

            return _context.Users.Find(-1).ToString();


        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was not a good request");
        }


    }
}

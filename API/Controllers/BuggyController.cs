using API.Errors;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly StoreContext _storeContext;

        public BuggyController(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        [HttpGet("NotFound")]
        public ActionResult GetNotFoundRequest()
        {
            var thing = _storeContext.Products.Find(42);

            if (thing == null) return NotFound(new ApiResponse(404));

            return Ok();
        }

        [HttpGet("ServerError")]
        public ActionResult GetServerError()
        {
            var thing = _storeContext.Products.Find(42);

            var thingToReturn = thing.ToString();

            return Ok();
        }

        [HttpGet("BadRequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("BadRequest/{id}")]
        public ActionResult GetNotFoundRequest(int id)
        {
            return Ok();
        }

        [Authorize]
        [HttpGet("testauth")]
        public ActionResult<string> GetSecretText()
        {
            return "Secret text on server for autherized users";
        }
    }
}

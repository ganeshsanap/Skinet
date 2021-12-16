﻿using API.Errors;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public ActionResult GetNotFound()
        {
            var thing = _storeContext.Products.Find(50);

            if (thing == null)
                return NotFound(new ApiResponse(404));

            return Ok();
        }

        [HttpGet("ServerError")]
        public ActionResult GetServerError()
        {
            var thing = _storeContext.Products.Find(50);

            var thingToReturn = thing.ToString();

            return Ok();
        }

        [HttpGet("badrequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("badrequest/{id:int}")]
        public ActionResult GetNotFoundRequest(int id)
        {
            return Ok();
        }
    }
}
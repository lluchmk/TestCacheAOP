﻿using Cache.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TestCache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesAOPController : ControllerBase
    {
        private IValuesService _service;

        public ValuesAOPController(IValuesService service)
        {
            _service = service;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new List<string>(_service.GetValues());
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return _service.Get(id);
        }
    }
}

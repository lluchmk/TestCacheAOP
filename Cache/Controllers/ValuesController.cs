﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Castle.DynamicProxy;

using TestCache.AOP;

namespace TestCache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private ICache _cache;

        public ValuesController(ICache cache)
        {
            _cache = cache;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            if (_cache.KeyExists("value"))
            {
                return _cache.GetString("value");
            }
            var v = "the value";
            _cache.AddString("value", v, TimeSpan.FromSeconds(20));
            return v;
        }
    }
}

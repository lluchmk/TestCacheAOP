using Cache.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

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
            if (_cache.GetUnderlyingDatabase().KeyExists("value"))
            {
                return _cache.Get<string>("value");
            }
            var v = "the value";
            _cache.Set("value", v, TimeSpan.FromSeconds(20));
            return v;
        }
    }
}

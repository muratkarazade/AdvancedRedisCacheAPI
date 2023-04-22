using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace Distributed.Caching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        readonly IDistributedCache _distributedCache;

        public ValuesController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        private void ConfigureRedisCache(RedisConfiguration config)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(40),
                SlidingExpiration = TimeSpan.FromSeconds(30)
            };            
        }

        [HttpGet("set")]
        public async Task<IActionResult> Set(string names, string surname, string hostname)
        {
            var redisConfig = new RedisConfiguration { Hostname = hostname };
            ConfigureRedisCache(redisConfig);

            await _distributedCache.SetStringAsync("names", names);
            await _distributedCache.SetAsync("surname", Encoding.UTF8.GetBytes(surname));
            return Ok();
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            var names = await _distributedCache.GetStringAsync("names");
            var surnameBinary = await _distributedCache.GetAsync("surname");
            var surname = Encoding.UTF8.GetString(surnameBinary);
            return Ok(new { names, surname });
        }
    }
}

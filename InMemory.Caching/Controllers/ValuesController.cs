using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemory.Caching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        readonly IMemoryCache _memoryCache;

        public ValuesController(IMemoryCache memoryCache)
        {
            // Dependency Injection ile IMemoryCache arayüzü kullanılır.
            _memoryCache = memoryCache;
        }

        [HttpGet("set/{name}")]
        public void SetName(string name)
        {
            // _memoryCache örneği kullanılarak, "name" adlı bir öğe bellekte saklama işlemi.
            _memoryCache.Set("name", name);
        }

        [HttpGet]
        public string GetName()
        {
            // Bellekteki "name" öğesi alınıyor. TryGetValue metodunun kullanılması, öğenin varlığı kontrol edilirken aynı zamanda öğenin değeri de alınmasını sağlar.
            // TryGetValue yerine Get' te kullanılabilirdi ancak herhangi bir sorun halinde uygulama kod kısmında herhangi bir hata vermez iken runtime sırasında karşılaşabileceğimiz hataya karşı kendimizi daha korunaklı hale getirmiş oluyoruz.
            if (_memoryCache.TryGetValue<string>("name", out string name))
            {
                return name;
            }
            return null; 
        }

        [HttpGet("setDate")]
        public void SetDate()
        {
            // "date" adlı bir öğe bellekte saklanıyor ve önbelleklemeyi yapılandırmak için seçenekler belirtiliyor.
            _memoryCache.Set<DateTime>("date", DateTime.Now, options: new()
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(10), // 10 saniye sonra öğenin geçerliliği sona erer.
                SlidingExpiration = TimeSpan.FromSeconds(5) // Öğe son kullanımdan 5 saniye sonra geçerliliğini yitirir.
            });
        }

        [HttpGet("getDate")]
        public DateTime GetDate()
        {
            // Bellekteki "date" öğesi alınıyor.
            return _memoryCache.Get<DateTime>("date");
        }
    }
}

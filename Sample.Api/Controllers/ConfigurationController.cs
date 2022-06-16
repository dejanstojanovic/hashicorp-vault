using Microsoft.AspNetCore.Mvc;

namespace Sample.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ControllerBase
    {
        readonly IConfiguration _configuration;

        public ConfigurationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var connectionString = _configuration.GetValue<string>("DatabaseConnectionString");
            return Ok(connectionString);
        }
    }
}
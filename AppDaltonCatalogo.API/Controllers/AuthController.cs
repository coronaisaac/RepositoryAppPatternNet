using AppDaltonCatalogo.Infrastructure.SQL.Dto.Auth;
using AppDaltonCatalogo.Infrastructure.UOW;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppDaltonCatalogo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public IConfiguration Configuration { get; }

        public AuthController(IUnitOfWork unitOfWork,IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            Configuration = configuration;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            try
            {
                using (var context = unitOfWork.Create())
                {
                    var response =  context.CatalogosRepository
                            .auth.Login(loginDto);
                    
                    if (!response.IsSucceed) return BadRequest(response.ErrorMessage);
                    if (string.IsNullOrEmpty(response.Value)) return StatusCode(400, "Sorry, User or password is incorrect, please try again");
                    return Ok(JsonConvert.DeserializeObject<Dictionary<string,object>>(response.Value ?? ""));
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace OceloteGateway.Controllers
{
    [Route("gateway")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Route("oceloteTest")]
        public string OceloteTest(){
            return "OceloteTest";
        }
    }
}
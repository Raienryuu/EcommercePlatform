using IdentityService.Data;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controller;

[Route("api/[controller]")]
[ApiController]
public class AuthorizationController : ControllerBase
{
  private ApplicationDbContext _db;

  //private UserManager<AspNetUser> _userManager;
  //private SignInManager<AspNetUser> _signinManager;
  public AuthorizationController(ApplicationDbContext db)
  {
    _db = db;
    //_userManager = userManager;
    //_signinManager = signInManager;
  }

  // GET: api/<AuthorizationController>
  // [HttpGet]
  // public async Task<IEnumerable<IdentityUserClaim<IdentityUser> > > GetRoutesAccess()
  // {
  //     var jwtTokenString = HttpContext.Request.Cookies["token"];
  //     JsonSerializer.Deserialize(jwtTokenString, IdentityUserToken<IdentityUser>);
  //     IdentityUserToken<string> token = new IdentityUserToken<string>();
  //     //_signinManager.IsSignedIn(new ClaimsPrincipal());

  //     return new List<IdentityUserClaim<IdentityUser> >();
  // }

  // GET api/<AuthorizationController>/5
  [HttpGet("{id}")]
  public string Get(int id)
  {
    return "value";
  }

  // POST api/<AuthorizationController>
  [HttpPost]
  public void Post([FromBody] string value) { }

  // PUT api/<AuthorizationController>/5
  [HttpPut("{id}")]
  public void Put(int id, [FromBody] string value) { }

  // DELETE api/<AuthorizationController>/5
  [HttpDelete("{id}")]
  public void Delete(int id) { }
}

using Cleverbit.CodingTask.Data;
using Cleverbit.CodingTask.Data.Models;
using Cleverbit.CodingTask.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Cleverbit.CodingTask.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly CodingTaskContext _context;
        private readonly IHashService _hashService;

        public UserController(CodingTaskContext context, IHashService hashService)
        {
            _context = context;
            _hashService = hashService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody]User user)
        {
            var _user = _context.Users.FirstOrDefault(u => u.UserName == user.UserName && u.Password == _hashService.HashText(user.Password).Result);

            if (_user == null)
            {
                return Unauthorized();
            }
            return Ok(_user.Id);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjeYonetim.API.DTOs;
using ProjeYonetim.API.Entities;
using ProjeYonetim.API.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace ProjeYonetim.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public LoginController(DataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Login([FromBody]Users users)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.UserName == users.UserName);
            var response = new ResponseDto();
            
            if (user == null || BC.Verify(users.Password, user.Password))
            {
                response.IsSuccess = false;
                response.Message = "Kullanıcı adı veya şifre hatalıdır.";
                return Unauthorized(response);      //401
            }

            response.IsSuccess = true;
            _session.SetString("UserId", user.Id.ToString());

            return Ok(response);    //200
            
        }

    }
}

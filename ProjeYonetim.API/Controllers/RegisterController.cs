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
    public class RegisterController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public RegisterController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] Users users)
        {
            var userExists = await _dataContext.Users.FirstOrDefaultAsync(x => x.UserName == users.UserName);
          
            if (userExists != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto{ IsSuccess=false, Message = "Bu kullanıcı adı daha önce alınmıştır."  });
            }

            Users newUser = new Users
            {
                UserName = users.UserName,
                Password = BC.HashPassword(users.Password)
            };
            
            await _dataContext.Users.AddAsync(newUser);
            int result = await _dataContext.SaveChangesAsync();

            if (result > 0)
                return Ok(new ResponseDto
                {
                    IsSuccess = true, 
                    Message = "Kayıt Oluşturuldu."
                });

            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { IsSuccess = false, Message = "İşlem başarısız, kayıt oluşturulamadı." });
        }
    }
}

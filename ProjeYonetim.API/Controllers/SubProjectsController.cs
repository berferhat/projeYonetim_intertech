using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjeYonetim.API.DTOs;
using ProjeYonetim.API.Entities;
using ProjeYonetim.API.EntityFramework;

namespace ProjeYonetim.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubProjectsController : ControllerBase
    {
        private readonly DataContext _dataContext;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public SubProjectsController(DataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubProjects subProjects)
        {
            string userId = _session.GetString("UserId");

            if (String.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ResponseDto {IsSuccess = false, Message = "Kullanıcı girişi yapılmamış."});
            }

            var subProjectExists = await _dataContext.SubProjects.FirstOrDefaultAsync(x => x.Title == subProjects.Title);

            if (subProjectExists != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseDto {IsSuccess = false, Message = "Bu alt proje adı daha önce kullanılmıştır."});
            }

            await _dataContext.SubProjects.AddAsync(subProjects);
            int result = await _dataContext.SaveChangesAsync();

            if (result > 0)
                return Ok(new ResponseDto
                {
                    IsSuccess = true,
                    Message = "Kayıt Oluşturuldu."
                });

            return StatusCode(StatusCodes.Status500InternalServerError,
                new ResponseDto {IsSuccess = false, Message = "İşlem başarısız, kayıt oluşturulamadı."});
        }
        
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SubProjects subProjects)
        {
            string userId = _session.GetString("UserId");

            if (String.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ResponseDto {IsSuccess = false, Message = "Kullanıcı girişi yapılmamış."});
            }

            var currentSubProjects = await _dataContext.SubProjects.FirstOrDefaultAsync(x => x.Id == subProjects.Id);

            if (currentSubProjects == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseDto {IsSuccess = false, Message = "Böyle bir kayıt bulunamadı."});
            }

            currentSubProjects.Title = subProjects.Title;
            currentSubProjects.EffortClock = subProjects.EffortClock;
            currentSubProjects.UsersId = Convert.ToInt32(userId);
            currentSubProjects.ProjectsId = subProjects.ProjectsId;
            _dataContext.SubProjects.Update( currentSubProjects);

            int result = await _dataContext.SaveChangesAsync();

            if (result > 0)
                return Ok(new ResponseDto
                {
                    IsSuccess = true,
                    Message = "Kayıt güncellendi."
                });
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ResponseDto {IsSuccess = false, Message = "İşlem başarısız, kayıt güncellenemedi."});
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string userId = _session.GetString("UserId");

            if (String.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ResponseDto {IsSuccess = false, Message = "Kullanıcı girişi yapılmamış."});
            }

            var currentSubProjects = await _dataContext.SubProjects.FirstOrDefaultAsync(x => x.Id == id);

            if (currentSubProjects == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseDto {IsSuccess = false, Message = "Böyle bir kayıt bulunamadı."});
            }

            _dataContext.Remove(currentSubProjects);

            int result = await _dataContext.SaveChangesAsync();

            if (result > 0)
                return Ok(new ResponseDto
                {
                    IsSuccess = true,
                    Message = "Kayıt silindi."
                });
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ResponseDto {IsSuccess = false, Message = "İşlem başarısız, kayıt silinemedi."});
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            string userId = _session.GetString("UserId");
            
            if (String.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ResponseDto {IsSuccess = false, Message = "Kullanıcı girişi yapılmamış."});
            }

            var currentSubProjects = await _dataContext.SubProjects.Include(x => x.Projects).FirstOrDefaultAsync(x => x.Id == id);
            

            if (currentSubProjects == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseDto {IsSuccess = false, Message = "Böyle bir kayıt bulunamadı."});
            }
            
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ResponseDto {IsSuccess = true, Data = currentSubProjects});
        }
    }
}

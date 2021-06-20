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

namespace ProjeYonetim.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly DataContext _dataContext;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public ProjectsController(DataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Create([FromBody] Projects projects)
        {
            string userId = _session.GetString("UserId");
           
            if (String.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ResponseDto {IsSuccess = false, Message = "Kullanıcı girişi yapılmamış."});
            }
            
            var projectExists = await _dataContext.Projects.FirstOrDefaultAsync(x => x.Title == projects.Title);

            if (projectExists != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseDto {IsSuccess = false, Message = "Bu proje adı daha önce kullanılmıştır."});
            }

            projects.UsersId = Convert.ToInt32(userId);

            await _dataContext.Projects.AddAsync(projects);
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
        public async Task<IActionResult> Update([FromBody] Projects projects)
        {
            string userId = _session.GetString("UserId");

            if (String.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ResponseDto {IsSuccess = false, Message = "Kullanıcı girişi yapılmamış."});
            }
           
            var currentProjects = await _dataContext.Projects.FirstOrDefaultAsync(x => x.Id == projects.Id);

            if (currentProjects == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseDto {IsSuccess = false, Message = "Böyle bir kayıt bulunamadı."});
            }

            currentProjects.Title = projects.Title;
            currentProjects.EffortClock = projects.EffortClock;
            currentProjects.UsersId = Convert.ToInt32(userId);
            
            _dataContext.Projects.Update(currentProjects);

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

            var currentProjects = await _dataContext.Projects.FirstOrDefaultAsync(x => x.Id == id);

            if (currentProjects == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseDto {IsSuccess = false, Message = "Böyle bir kayıt bulunamadı."});
            }

            _dataContext.Remove(currentProjects);

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

            var currentProjects = await _dataContext.Projects.FirstOrDefaultAsync(x => x.Id == id);

            if (currentProjects == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ResponseDto {IsSuccess = false, Message = "Böyle bir kayıt bulunamadı."});
            }
            
            return StatusCode(StatusCodes.Status200OK,
                new ResponseDto {IsSuccess = true, Data = currentProjects});
        }
    }
}

using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WhiteboardAPI.Helpers;
using WhiteboardAPI.Model;
using WhiteboardAPI.Repository;

namespace WhiteboardAPI.Controllers
{
    public class CourseController : Controller
    {
        private ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IEmailSender _emailSender;
        private readonly IUrlHelper _urlHelper;

        public CourseController(ICourseRepository courseRepository, IMapper mapper, IUrlHelper urlHelper, IOptions<AppSettings> appSettings, IEmailSender emailSender)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _emailSender = emailSender;
            _urlHelper = urlHelper;
        }

        [AllowAnonymous]
        [HttpPost("createCourse")]
        public IActionResult Register(CourseDto courseDto)
        {
            if (!ModelState.IsValid)
            {
                // return 422
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }

            try
            {
                // save
                var courseFromRepo = _courseRepository.CreateCourse(courseDto);

                var courseToReturn = _mapper.Map<CourseDto>(courseFromRepo);

                return Ok(courseToReturn);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

using System;
using System.Collections.Generic;
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
        public IActionResult CreateCourse(CourseDto courseDto)
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

        [AllowAnonymous]
        [HttpPut("updateCourse")]
        public IActionResult UpdateCourse([FromBody]CourseDto courseDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // return 422
                    return new Helpers.UnprocessableEntityObjectResult(ModelState);
                }

                _courseRepository.UpdateCourse(courseDto);

                if (!_courseRepository.Save())
                {
                    throw new AppException("Updating course failed on save.");
                }

                return NoContent();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("getCourses")]
        public IActionResult GetCourses(List<Guid> courseIds)
        {
            var coursesFromRepo = _courseRepository.GetCourses(courseIds);

            if (coursesFromRepo == null)
            {
                return NotFound();
            }

            IEnumerable<CourseDto> courseDtos = _mapper.Map<IEnumerable<CourseDto>>(coursesFromRepo);

            return Ok(courseDtos);
        }

        [AllowAnonymous]
        [HttpPost("addCourseStudent")]
        public IActionResult AddCourseStudent([FromBody]List<CourseStudentDto> courseStudentDto)
        {
            if (!ModelState.IsValid)
            {
                // return 422
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }

            try
            {
                // save
                var courseStudentFromRepo = _courseRepository.AddCourseStudent(courseStudentDto);

                var courseStudentToReturn = _mapper.Map<IEnumerable<CourseStudentDto>>(courseStudentFromRepo);

                return Ok(courseStudentToReturn);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("addCourseStaff")]
        public IActionResult AddCourseStaff([FromBody]List<CourseStaffDto> courseStaffDto)
        {
            if (!ModelState.IsValid)
            {
                // return 422
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }

            try
            {
                // save
                var courseStaffFromRepo = _courseRepository.AddCourseStaff(courseStaffDto);

                var courseStaffToReturn = _mapper.Map<IEnumerable<CourseStaffDto>>(courseStaffFromRepo);

                return Ok(courseStaffToReturn);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

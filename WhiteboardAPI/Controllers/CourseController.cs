using System;
using System.Collections.Generic;
using System.Linq;
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
        private IContentRepository _contentRepository;
        private IDiscussionBoardRepository _discussionBoardRepository;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IUrlHelper _urlHelper;

        public CourseController(ICourseRepository courseRepository, IContentRepository contentRepository, IDiscussionBoardRepository discussionBoardRepository, IMapper mapper, IUrlHelper urlHelper, IOptions<AppSettings> appSettings)
        {
            _courseRepository = courseRepository;
            _contentRepository = contentRepository;
            _discussionBoardRepository = discussionBoardRepository;
            _mapper = mapper;
            _appSettings = appSettings.Value;
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

                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpDelete("deleteCourse/{courseId}")]
        public IActionResult DeleteCourse(Guid courseId)
        {
            try
            {
                _courseRepository.DeleteCourse(courseId);

                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("getAllCourses")]
        public IActionResult GetAllCourses()
        {
            var coursesFromRepo = _courseRepository.GetAllCourses();

            if (coursesFromRepo == null)
            {
                return NotFound();
            }

            IEnumerable<CourseDto> courseDtos = _mapper.Map<IEnumerable<CourseDto>>(coursesFromRepo);
            foreach (CourseDto c in courseDtos)
            {
                c.Students = _courseRepository.GetCourseStudent(c.CourseId);
                c.Staff = _courseRepository.GetCourseStaff(c.CourseId);
                c.CourseFolders = _mapper.Map<IEnumerable<CourseFolderDto>>(_discussionBoardRepository.GetCourseFolders(c.CourseId));
                c.Contents = _contentRepository.GetContentByCourse(c.CourseId).Select(x => x.ContentId);
            }

            return Ok(courseDtos);
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
            foreach (CourseDto c in courseDtos)
            {
                c.Students = _courseRepository.GetCourseStudent(c.CourseId);
                c.Staff = _courseRepository.GetCourseStaff(c.CourseId);
                //c.Content
            }

            return Ok(courseDtos);
        }

        [AllowAnonymous]
        [HttpGet("getCourseByUser/{userId}")]
        public IActionResult GetCourseByUser(Guid userId)
        {
            var coursesFromRepo = _courseRepository.GetCourseByUser(userId);

            if (coursesFromRepo == null)
            {
                return NotFound();
            }

            IEnumerable<CourseDto> courseDtos = _mapper.Map<IEnumerable<CourseDto>>(coursesFromRepo);
            foreach (CourseDto c in courseDtos)
            {
                c.Students = _courseRepository.GetCourseStudent(c.CourseId);
                c.Staff = _courseRepository.GetCourseStaff(c.CourseId);
                //c.Content
            }

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
        [HttpPut("removeCourseStudent/{courseId}/{studentId}")]
        public IActionResult RemoveCourseStudent(Guid courseId, Guid studentId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // return 422
                    return new Helpers.UnprocessableEntityObjectResult(ModelState);
                }

                _courseRepository.RemoveCourseStudent(courseId, studentId);

                if (!_courseRepository.Save())
                {
                    throw new AppException("Removing {stuentId} from course failed on save.");
                }

                return Ok();
            }
            catch (AppException ex)
            {
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

        [AllowAnonymous]
        [HttpPut("removeCourseStaff/{courseId}/{staffId}")]
        public IActionResult RemoveCourseStaff(Guid courseId, Guid staffId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // return 422
                    return new Helpers.UnprocessableEntityObjectResult(ModelState);
                }

                _courseRepository.RemoveCourseStaff(courseId, staffId);

                if (!_courseRepository.Save())
                {
                    throw new AppException("Removing {stuentId} from course failed on save.");
                }

                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;
using WhiteboardAPI.Database;
using WhiteboardAPI.Entities;
using WhiteboardAPI.Helpers;
using WhiteboardAPI.Model;

namespace WhiteboardAPI.Repository
{
    public interface ICourseRepository
    {
        CourseDE CreateCourse(CourseDto courseDto);
    }

    public class CourseRepository : ICourseRepository
    {
        private WhiteboardContext _context;
        private IUserRepository _userRepository;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public CourseRepository(WhiteboardContext context, IUserRepository userRepository, IOptions<AppSettings> appSettings, IMapper mapper)
        {
            _context = context;
            _userRepository = userRepository;
            _appSettings = appSettings.Value;
            _mapper = mapper;
        }

        public CourseDE CreateCourse(CourseDto courseDto)
        {
            // validation
            if (string.IsNullOrWhiteSpace(courseDto.CourseCode))
                throw new AppException("Course Code is required");

            if (_context.tbl_course.Any(x => x.CourseCode == courseDto.CourseCode))
            {
                throw new AppException("Course Code \"" + courseDto.CourseCode + "\" is unavailable");
            }

            UserDE user = _userRepository.GetUsers(new List<Guid>(new Guid[] { courseDto.CreatedBy })).FirstOrDefault();
            if (user == null || string.Equals(user.Role, "student", StringComparison.OrdinalIgnoreCase))
            {
                throw new AppException("User \"" + courseDto.CreatedBy + "\" does not exist");
            }

            CourseDE course = new CourseDE();
            course = _mapper.Map<CourseDto, CourseDE>(courseDto);
            course.CourseId = new Guid();
            course.CreatedOn = DateTime.Now;

            _context.tbl_course.Add(course);
            _context.SaveChanges();
            
            return course;
        }
    }
}

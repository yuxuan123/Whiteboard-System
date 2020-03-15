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
        CourseDE UpdateCourse(CourseDto courseDto);
        void DeleteCourse(Guid courseId);
        IEnumerable<CourseDE> GetAllCourses();
        IEnumerable<CourseDE> GetCourses(IEnumerable<Guid> courseIds);
        IEnumerable<CourseDE> GetCourseByUser(Guid userId);
        IEnumerable<CourseStudentDE> AddCourseStudent(List<CourseStudentDto> courseStudentDto);
        IEnumerable<Guid> GetCourseStudent(Guid courseId);
        void RemoveCourseStudent(Guid courseId, Guid studentId);
        IEnumerable<CourseStaffDE> AddCourseStaff(List<CourseStaffDto> courseStaffDto);
        IEnumerable<Guid> GetCourseStaff(Guid courseId);
        void RemoveCourseStaff(Guid courseId, Guid staffId);
        bool CourseExists(Guid courseId);
        public bool Save();
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

        public CourseDE UpdateCourse(CourseDto courseDto)
        {
            CourseDE course = _context.tbl_course.Where(x => x.CourseId == courseDto.CourseId).FirstOrDefault();

            if (course == null)
                throw new AppException("CourseId is invalid");

            course.CourseCode = string.IsNullOrEmpty(courseDto.CourseCode) ? course.CourseCode : courseDto.CourseCode;
            course.CourseName = string.IsNullOrEmpty(courseDto.CourseName) ? course.CourseName : courseDto.CourseName;
            course.CourseDescription = string.IsNullOrEmpty(courseDto.CourseDescription) ? course.CourseCode : courseDto.CourseDescription;

            _context.tbl_course.Update(course);
            _context.SaveChanges();

            return course;
        }

        public void DeleteCourse(Guid courseId)
        {
            CourseDE course = _context.tbl_course.Where(x => x.CourseId == courseId).FirstOrDefault();

            if (course == null)
                throw new AppException("CourseId does not exist");

            _context.tbl_course.Remove(course);
            _context.SaveChanges();
        }

        public IEnumerable<CourseDE> GetAllCourses()
        {
            return _context.tbl_course;
        }

        public IEnumerable<CourseDE> GetCourses(IEnumerable<Guid> courseIds)
        {
            return _context.tbl_course.Where(x => courseIds.Contains(x.CourseId));
        }

        public IEnumerable<CourseDE> GetCourseByUser(Guid userId)
        {
            var courses = _context.tbl_course_student.Where(x => x.StudentId == userId && x.IsActive).Select(x => x.CourseId);

            if (courses.Count() == 0)
                courses = _context.tbl_course_staff.Where(x => x.StaffId == userId && x.IsActive).Select(x => x.CourseId);

            return _context.tbl_course.Where(x => courses.Contains(x.CourseId));
        }

        public IEnumerable<CourseStudentDE> AddCourseStudent(List<CourseStudentDto> courseStudentDto)
        {
            IList<CourseStudentDE> courseStudents = new List<CourseStudentDE>();

            foreach (CourseStudentDto c in courseStudentDto)
            {
                UserDE user = _userRepository.GetUser(c.StudentId);
                if (user == null || !user.Role.Equals("student", StringComparison.OrdinalIgnoreCase))
                    throw new AppException("Student " + c.StudentId + " does not exist");

                if (!CourseExists(c.CourseId))
                    throw new AppException("Course " + c.CourseId + " does not exist");

                c.Id = new Guid();
                c.IsActive = true;

                //Only update IsActive when student has previously registred to the course
                CourseStudentDE existingStudent = _context.tbl_course_student.Where(x => x.CourseId == c.CourseId && x.StudentId == c.StudentId).FirstOrDefault();
                if (existingStudent == null)
                {
                    courseStudents.Add(_mapper.Map<CourseStudentDE>(c));
                }
                else if (!existingStudent.IsActive)
                {
                    existingStudent.IsActive = true;
                    _context.tbl_course_student.Update(existingStudent);
                }
            }

            _context.tbl_course_student.AddRange(courseStudents);
            _context.SaveChanges();

            return courseStudents;
        }

        public IEnumerable<Guid> GetCourseStudent(Guid courseId)
        {
            return _context.tbl_course_staff.Where(x => x.CourseId == courseId && x.IsActive).Select(x => x.StaffId);
        }

        public void RemoveCourseStudent(Guid courseId, Guid studentId)
        {
            var student = _context.tbl_course_student.Where(x => x.CourseId == courseId && x.StudentId == studentId && x.IsActive).FirstOrDefault();

            if (student != null)
            {
                student.IsActive = false;
                _context.tbl_course_student.Update(student);
            }
        }

        public IEnumerable<CourseStaffDE> AddCourseStaff(List<CourseStaffDto> courseStaffDto)
        {
            IList<CourseStaffDE> courseStaff = new List<CourseStaffDE>();
            foreach (CourseStaffDto c in courseStaffDto)
            {
                UserDE user = _userRepository.GetUser(c.StaffId);
                if (user == null || !user.Role.Equals("lecturer", StringComparison.OrdinalIgnoreCase))
                    throw new AppException("Staff " + c.StaffId + " does not exist");

                if (!CourseExists(c.CourseId))
                    throw new AppException("Course " + c.CourseId + " does not exist");

                c.Id = new Guid();
                c.IsActive = true;

                //Only update IsActive when staff has previously registred to the course
                CourseStaffDE existingStaff = _context.tbl_course_staff.Where(x => x.CourseId == c.CourseId && x.StaffId == c.StaffId).FirstOrDefault();
                if (existingStaff == null)
                {
                    courseStaff.Add(_mapper.Map<CourseStaffDE>(c));
                }
                else if (!existingStaff.IsActive)
                {
                    existingStaff.IsActive = true;
                    _context.tbl_course_staff.Update(existingStaff);
                }
            }

            _context.tbl_course_staff.AddRange(courseStaff);
            _context.SaveChanges();

            return courseStaff;
        }

        public IEnumerable<Guid> GetCourseStaff(Guid courseId)
        {
            return _context.tbl_course_student.Where(x => x.CourseId == courseId && x.IsActive).Select(x => x.StudentId);
        }

        public void RemoveCourseStaff(Guid courseId, Guid staffId)
        {
            var staff = _context.tbl_course_staff.Where(x => x.CourseId == courseId && x.StaffId == staffId && x.IsActive).FirstOrDefault();

            if (staff != null)
            {
                staff.IsActive = false;
                _context.tbl_course_staff.Update(staff);
            }
        }

        public bool CourseExists(Guid guid)
        {
            return _context.tbl_course.Any(x => x.CourseId == guid);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using WhiteboardAPI.Database;
using WhiteboardAPI.Entities;
using WhiteboardAPI.Helpers;
using WhiteboardAPI.Model;

namespace WhiteboardAPI.Repository
{
    public interface IContentRepository
    {
        ContentDE AddContent(ContentDto contentDto);
        IEnumerable<ContentDE> GetContentByUser(Guid userId);
        IEnumerable<ContentDE> GetContentByCourse(Guid courseId);
        void UpdateContent(ContentDto contentDto);
        void DeleteContent(Guid contentId);
        public bool Save();
    }

    public class ContentRepository : IContentRepository
    {
        private WhiteboardContext _context;
        private ICourseRepository _courseRepository;
        private IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ContentRepository(WhiteboardContext context, IMapper mapper, ICourseRepository courseRepository, IUserRepository userRepository)
        {
            _context = context;
            _mapper = mapper;
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }

        public ContentDE AddContent(ContentDto contentDto)
        {
            ContentDE content = _mapper.Map<ContentDE>(contentDto);

            if (!_courseRepository.CourseExists(contentDto.CourseId))
                throw new AppException("CourseId does not exist");

            UserDE user = _userRepository.GetUser(contentDto.CreatedBy);
            if (user == null || !user.Role.Equals("lecturer", StringComparison.OrdinalIgnoreCase))
                throw new AppException("Staff " + contentDto.CreatedBy + " does not exist");

            if (!String.IsNullOrEmpty(contentDto.FileName) && String.IsNullOrEmpty(contentDto.Url))
                throw new AppException("Url is empty for " + contentDto.FileName);

            content.ContentId = new Guid();
            content.CreatedOn = DateTime.Now;

            _context.tbl_content.Add(content);
            _context.SaveChanges();

            return content;
        }

        public IEnumerable<ContentDE> GetContentByUser(Guid userId)
        {
            var courses = _courseRepository.GetCourseByUser(userId).Select(x => x.CourseId);
            return _context.tbl_content.Where(x => courses.Contains(x.CourseId));
        }

        public IEnumerable<ContentDE> GetContentByCourse(Guid courseId)
        {
            return _context.tbl_content.Where(x => x.CourseId == courseId);
        }

        public void UpdateContent(ContentDto contentDto)
        {
            ContentDE content = _context.tbl_content.Where(x => x.ContentId == contentDto.ContentId).FirstOrDefault();

            if (content == null)
                throw new AppException("ContentId does not exist");

            if (!String.IsNullOrEmpty(contentDto.FileName) && String.IsNullOrEmpty(contentDto.Url))
                throw new AppException("Url is empty for " + contentDto.FileName);

            if (contentDto.Datetime > DateTime.MinValue)
                content.Datetime = contentDto.Datetime;

            if (!String.IsNullOrEmpty(contentDto.Title))
                content.Title = contentDto.Title;

            if (!String.IsNullOrEmpty(contentDto.Description))
                content.Description = contentDto.Description;

            if(!String.IsNullOrEmpty(contentDto.FileName))
                content.FileName = contentDto.FileName;

            if(!String.IsNullOrEmpty(contentDto.Url))
                content.url = contentDto.Url;

            _context.tbl_content.Update(content);
        }

        public void DeleteContent(Guid contentId)
        {
            ContentDE content = _context.tbl_content.Where(x => x.ContentId == contentId).FirstOrDefault();

            if (content == null)
                throw new AppException("ContentId does not exist");

            _context.tbl_content.Remove(content);
            _context.SaveChanges();
        }

        public bool ContentExists(Guid contentId)
        {
            return _context.tbl_content.Any(x => x.ContentId == contentId);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}

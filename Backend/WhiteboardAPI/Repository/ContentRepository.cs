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
        IEnumerable<ContentDE> GetContent(Guid courseId);
        void UpdateContent(ContentDto contentDto);
        void DeleteContent(Guid contentId);
        public bool Save();
    }

    public class ContentRepository : IContentRepository
    {
        private WhiteboardContext _context;
        private IContentRepository _contentRepository;
        private readonly IMapper _mapper;

        public ContentRepository(WhiteboardContext context, IContentRepository contentRepository, IMapper mapper)
        {
            _context = context;
            _contentRepository = contentRepository;
            _mapper = mapper;
        }

        public ContentDE AddContent(ContentDto contentDto)
        {
            ContentDE course = _mapper.Map<ContentDE>(contentDto);

            _context.tbl_content.Add(course);
            _context.SaveChanges();

            return course;
        }

        public IEnumerable<ContentDE> GetContent(Guid courseId)
        {
            return _context.tbl_content.Where(x => x.CourseId == courseId);
        }

        public void UpdateContent(ContentDto contentDto)
        {
            ContentDE course = _mapper.Map<ContentDE>(contentDto);

            _context.tbl_content.Update(course);
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

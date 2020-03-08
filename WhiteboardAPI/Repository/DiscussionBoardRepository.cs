using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using AutoMapper;
using WhiteboardAPI.Database;
using WhiteboardAPI.Entities;
using WhiteboardAPI.Helpers;
using WhiteboardAPI.Model;

namespace WhiteboardAPI.Repository
{
    public interface IDiscussionBoardRepository
    {
        PostDE CreatePost(PostDto postDto);
        ReplyDE CreateReply(ReplyDto replyDto);
        CourseFolderDE CreateCourseFolder(CourseFolderDto courseFolderDto);
        void CreatePostFolder(Guid postId, List<Guid> courseFolderId);
        PagedList<PostDto> GetAllPosts(ResourceParameters resourceParameters);
        IEnumerable<PostDE> GetPostsByUser(Guid userId, Guid courseId, string keyword);
        IEnumerable<PostDE> GetPostsByCourse(Guid courseId);
        IEnumerable<ReplyDE> GetReplies(Guid PostId);
        IEnumerable<CourseFolderDE> GetAllCourseFolders();
        IEnumerable<CourseFolderDE> GetCourseFolders(Guid CourseId); //get course dto
        IEnumerable<PostFolderDE> GetPostFolders(Guid postId);
        void UpdatePost(PostDto postDto);
        void UpdateReply(ReplyDto replyDto);
        void DeletePost(Guid postId);
        void DeleteReply(Guid postId);
        void DeleteCourseFolder(Guid courseFolderId);
        void DeletePostFolder(Guid postFolderId);
        public bool Save();
    }

    public class DiscussionBoardRepository : IDiscussionBoardRepository
    {
        private WhiteboardContext _context;
        private ICourseRepository _courseRepository;
        private IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public DiscussionBoardRepository(WhiteboardContext context, IMapper mapper, ICourseRepository courseRepository, IUserRepository userRepository)
        {
            _context = context;
            _mapper = mapper;
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }

        public PostDE CreatePost(PostDto postDto)
        {
            PostDE post = _mapper.Map<PostDE>(postDto);

            if (!_userRepository.UserExists(postDto.CreatedBy))
                throw new AppException("UserId " + postDto.CreatedBy + " does not exist");

            post.PostId = new Guid();
            post.CreatedOn = DateTime.Now;
            post.isEdited = false;

            if (postDto.CourseFolderId.Count > 0)
                CreatePostFolder(postDto.PostId, postDto.CourseFolderId);

            _context.tbl_db_post.Add(post);
            _context.SaveChanges();

            if (postDto.CourseFolderId != null)
                CreatePostFolder(post.PostId, postDto.CourseFolderId);

            return post;
        }

        public ReplyDE CreateReply(ReplyDto replyDto)
        {
            if (!PostExists(replyDto.PostId))
                throw new AppException("PostId " + replyDto.PostId + " does not exist");

            if (!_userRepository.UserExists(replyDto.CreatedBy))
                throw new AppException("UserId " + replyDto.CreatedBy + " does not exist");

            ReplyDE reply = _mapper.Map<ReplyDE>(replyDto);

            reply.ReplyId = new Guid();
            reply.CreatedOn = DateTime.Now;
            reply.isEdited = false;

            _context.tbl_db_reply.Add(reply);
            _context.SaveChanges();

            return reply;
        }

        public CourseFolderDE CreateCourseFolder(CourseFolderDto courseFolderDto)
        {
            if (!_courseRepository.CourseExists(courseFolderDto.CourseId))
                throw new AppException("CourseId " + courseFolderDto.CourseId + "does not exist");

            CourseFolderDE courseFolder = _mapper.Map<CourseFolderDE>(courseFolderDto);

            courseFolder.CourseFolderId = new Guid();

            _context.tbl_db_course_folder.Add(courseFolder);
            _context.SaveChanges();

            return courseFolder;
        }

        public void CreatePostFolder(Guid postId, List<Guid> courseFolderId)
        {
            foreach (Guid i in courseFolderId)
            {
                if (!CourseFolderExists(i))
                    throw new AppException("CourseFolderId " + i + " does not exist");
                PostFolderDE postFolder = new PostFolderDE();
                postFolder.PostFolderId = new Guid();
                postFolder.PostId = postId;
                postFolder.CourseFolderId = i;
                _context.tbl_db_post_folder.Add(postFolder);
            }
            _context.SaveChangesAsync().Wait();

        }

        public PagedList<PostDto> GetAllPosts(ResourceParameters resourceParameters)
        {
            var collectionBeforePaging = string.IsNullOrEmpty(resourceParameters.keyword) ? _context.tbl_db_post.OrderBy(resourceParameters.OrderBy) : _context.tbl_db_post.Where(x => x.Title.Contains(resourceParameters.keyword)).OrderBy(resourceParameters.OrderBy);

            var postDtos = _mapper.Map<IEnumerable<PostDto>>(collectionBeforePaging);

            foreach(PostDto p in postDtos)
            {
                p.UserName = _userRepository.GetUser(p.CreatedBy).UserName;
            }

            return PagedList<PostDto>.Create(postDtos, resourceParameters.PageNumber, resourceParameters.PageSize);

        }

        public IEnumerable<PostDE> GetPostsByUser(Guid userId, Guid courseId, string keyword)
        {
            var courses = _courseRepository.GetCourseByUser(userId).Select(x => x.CourseId).ToList();

            if (courseId != Guid.Empty)
            {
                courses = courses.Where(x => x == courseId).ToList();
            }

            return string.IsNullOrEmpty(keyword) ? _context.tbl_db_post.Where(x => courses.Contains(x.CourseId)) : _context.tbl_db_post.Where(x => courses.Contains(x.CourseId) && x.Title.Contains(keyword));
        }

        public IEnumerable<PostDE> GetPostsByCourse(Guid courseId)
        {
            return _context.tbl_db_post.Where(x => x.CourseId == courseId);
        }

        public IEnumerable<ReplyDE> GetReplies(Guid postId)
        {
            return _context.tbl_db_reply.Where(x => x.PostId == postId);
        }

        public IEnumerable<CourseFolderDE> GetAllCourseFolders()
        {
            return _context.tbl_db_course_folder;
        }

        public IEnumerable<CourseFolderDE> GetCourseFolders(Guid courseId)
        {
            return _context.tbl_db_course_folder.Where(x => x.CourseId == courseId);
        }

        public IEnumerable<PostFolderDE> GetPostFolders(Guid postId)
        {
            return _context.tbl_db_post_folder.Where(x => x.PostId == postId);
        }

        public bool PostExists(Guid postId)
        {
            return _context.tbl_db_post.Any(x => x.PostId == postId);
        }

        public bool CourseFolderExists(Guid courseFolderId)
        {
            return _context.tbl_db_course_folder.Any(x => x.CourseFolderId == courseFolderId);
        }

        public void UpdatePost(PostDto postDto)
        {
            PostDE post = _context.tbl_db_post.Where(x => x.PostId == postDto.PostId).FirstOrDefault();
            var postFolder = _context.tbl_db_post_folder.Where(x => x.PostId == postDto.PostId).FirstOrDefault();

            if (post == null)
                throw new AppException("PostId does not exist");

            if (!string.IsNullOrEmpty(postDto.Title))
            {
                post.Title = postDto.Title;
            }

            if (!string.IsNullOrEmpty(postDto.Description))
            {
                post.Description = postDto.Description;
            }

            if (postFolder != null && postDto.CourseFolderId[0] != postFolder.CourseFolderId)
            {
                postFolder.CourseFolderId = postDto.CourseFolderId[0];
                _context.tbl_db_post_folder.Update(postFolder);
            }

            post.isEdited = true;

            _context.tbl_db_post.Update(post);

        }

        public void UpdateReply(ReplyDto replyDto)
        {
            ReplyDE reply = _context.tbl_db_reply.Where(x => x.ReplyId == replyDto.ReplyId).FirstOrDefault();

            if (reply == null)
                throw new AppException("ReplyId does not exist");

            if (!string.IsNullOrEmpty(replyDto.Description))
            {
                reply.Description = replyDto.Description;
            }

            reply.isEdited = true;

            _context.tbl_db_reply.Update(reply);

        }

        public void DeletePost(Guid postId)
        {
            PostDE post = _context.tbl_db_post.Where(x => x.PostId == postId).FirstOrDefault();

            if (post == null)
                throw new AppException("PostId does not exist");

            _context.tbl_db_reply.RemoveRange(GetReplies(post.PostId));
            _context.tbl_db_post_folder.RemoveRange(GetPostFolders(post.PostId));
            _context.tbl_db_post.Remove(post);
            _context.SaveChanges();
        }

        public void DeleteReply(Guid replyId)
        {
            ReplyDE reply = _context.tbl_db_reply.Where(x => x.ReplyId == replyId).FirstOrDefault();

            if (reply == null)
                throw new AppException("ReplyId does not exist");

            _context.tbl_db_reply.Remove(reply);
            _context.SaveChanges();
        }

        public void DeleteCourseFolder(Guid courseFolderId)
        {
            CourseFolderDE courseFolder = _context.tbl_db_course_folder.Where(x => x.CourseFolderId == courseFolderId).FirstOrDefault();

            if (courseFolder == null)
                throw new AppException("CourseFolderId does not exist");

            _context.tbl_db_course_folder.Remove(courseFolder);
            _context.SaveChanges();
        }

        public void DeletePostFolder(Guid postFolderId)
        {
            PostFolderDE postFolder = _context.tbl_db_post_folder.Where(x => x.PostFolderId == postFolderId).FirstOrDefault();

            if (postFolder == null)
                throw new AppException("PostFolderId does not exist");

            _context.tbl_db_post_folder.Remove(postFolder);
            _context.SaveChanges();
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}

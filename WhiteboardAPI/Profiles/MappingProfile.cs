using AutoMapper;
using WhiteboardAPI.Entities;
using WhiteboardAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteboardAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDE, UserDto>();
            CreateMap<UserUpdateDto, UserDE>();
            CreateMap<CourseDto, CourseDE>();
            CreateMap<CourseDE, CourseDto>();
            CreateMap<CourseStudentDE, CourseStudentDto>();
            CreateMap<CourseStaffDE, CourseStaffDto>();
            CreateMap<CourseStudentDto, CourseStudentDE>();
            CreateMap<CourseStaffDto, CourseStaffDE>();
            CreateMap<ContentDE, ContentDto>();
            CreateMap<ContentDto, ContentDE>();
            CreateMap<PostDE, PostDto>();
            CreateMap<PostDto, PostDE>();
            CreateMap<ReplyDE, ReplyDto>();
            CreateMap<ReplyDto, ReplyDE>();
            CreateMap<CourseFolderDE, CourseFolderDto>();
            CreateMap<CourseFolderDto, CourseFolderDE>();
            CreateMap<PostFolderDE, PostFolderDto>();
            CreateMap<PostFolderDto, PostFolderDE>();
            CreateMap<ChatDE, ChatDto>();
            CreateMap<ChatDto, ChatDE>();
        }
    }
}
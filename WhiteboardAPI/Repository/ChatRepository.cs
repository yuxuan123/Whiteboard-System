using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using WhiteboardAPI.Database;
using WhiteboardAPI.Entities;
using WhiteboardAPI.Model;

namespace WhiteboardAPI.Repository
{
    public interface IChatRepository
    {
        List<ChatDE> GetAllChat(Guid lectureId);
        ChatDE SaveChat(ChatDto chatDto);
    }

    public class ChatRepository : IChatRepository
    {
        private WhiteboardContext _context;
        private readonly IMapper _mapper;

        public ChatRepository(WhiteboardContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<ChatDE> GetAllChat(Guid lectureId)
        {
            return _context.tbl_chat.Where(x => x.LectureId == lectureId).OrderBy(x => x.DateTime).ToList();
        }

        public ChatDE SaveChat(ChatDto chatDto)
        {
            ChatDE chat = _mapper.Map<ChatDE>(chatDto);

            chat.ChatId = new Guid();
            chat.DateTime = DateTime.Now;

            _context.tbl_chat.Add(chat);
            _context.SaveChanges();

            return chat;
        }
    }
}

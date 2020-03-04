using PusherServer;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WhiteboardAPI.Model;
using WhiteboardAPI.Repository;
using AutoMapper;
using System;
using System.Collections;
using System.Collections.Generic;

namespace WhiteboardAPI.Controllers
{
    public class ChatController : Controller
    {
        private IChatRepository _chatRepository;
        private IMapper _mapper;

        public ChatController(IChatRepository chatRepository, IMapper mapper)
        {
            _chatRepository = chatRepository;
            _mapper = mapper;
        }

        [HttpPost("pusher")]
        public async Task<ActionResult> NewChat([FromBody]ChatDto chatDto)
        {
            var chat =_chatRepository.SaveChat(chatDto);

            var options = new PusherOptions
            {
                Cluster = "ap1",
                Encrypted = true
            };

            var pusher = new Pusher(
              "957914",
              "0a3b3bc361a655ea56ac",
              "1a2506af120a04af2906",
              options);

            var result = await pusher.TriggerAsync(
              chatDto.LectureId.ToString(),
              "my-event",
              new
              {
                  userName = chatDto.UserName,
                  message = chatDto.Message,
                  dateTime = chat.DateTime
              });

            return Ok(result);
        }

        [HttpPost("getAllChat/{lectureId}")]
        public IActionResult GetAllChat(Guid lectureId)
        {
            var chat = _chatRepository.GetAllChat(lectureId);

            if (chat == null)
            {
                return NotFound();
            }

            var chatToReturn = _mapper.Map<IEnumerable<ChatDto>>(chat);

            return Ok(chatToReturn);
        }
    }
}


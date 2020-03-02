using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhiteboardAPI.Helpers;
using WhiteboardAPI.Model;
using WhiteboardAPI.Repository;

namespace WhiteboardAPI.Controllers
{
    public class DiscussionBoardController : Controller
    {
        private IDiscussionBoardRepository _discussionBoardRepository;
        private readonly IMapper _mapper;

        public DiscussionBoardController(IDiscussionBoardRepository discussionBoardRepository, IMapper mapper)
        {
            _discussionBoardRepository = discussionBoardRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("createPost")]
        public IActionResult CreatePost([FromBody] PostDto postDto)
        {
            if (!ModelState.IsValid)
            {
                // return 422
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }

            try
            {
                // save
                var postFromRepo = _discussionBoardRepository.CreatePost(postDto);

                var postToReturn = _mapper.Map<ContentDto>(postFromRepo);

                return Ok(postToReturn);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("getPost/{courseId}")]
        public IActionResult GetPost(Guid courseId)
        {
            var postsFromRepo = _discussionBoardRepository.GetPosts(courseId);

            if (postsFromRepo == null)
            {
                return NotFound();
            }

            var contentDtos = _mapper.Map<IEnumerable<PostDto>>(postsFromRepo);

            return Ok(contentDtos);
        }

        [AllowAnonymous]
        [HttpDelete("deletePost/{postId}")]
        public IActionResult DeletePost(Guid postId)
        {
            try
            {
                _discussionBoardRepository.DeletePost(postId);

                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("createReply")]
        public IActionResult CreateReply([FromBody] ReplyDto replyDto)
        {
            if (!ModelState.IsValid)
            {
                // return 422
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }

            try
            {
                // save
                var replyFromRepo = _discussionBoardRepository.CreateReply(replyDto);

                var replyToReturn = _mapper.Map<ContentDto>(replyFromRepo);

                return Ok(replyToReturn);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("getReply/{postId}")]
        public IActionResult GetReply(Guid postId)
        {
            var repliesFromRepo = _discussionBoardRepository.GetReplies(postId);

            if (repliesFromRepo == null)
            {
                return NotFound();
            }

            var replyDtos = _mapper.Map<IEnumerable<PostDto>>(repliesFromRepo);

            return Ok(replyDtos);
        }

        [AllowAnonymous]
        [HttpDelete("deleteReply/{replyId}")]
        public IActionResult DeleteReply(Guid replyId)
        {
            try
            {
                _discussionBoardRepository.DeleteReply(replyId);

                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

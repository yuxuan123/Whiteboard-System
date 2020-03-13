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
    public class ContentController : Controller
    {
        private IContentRepository _contentRepository;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;

        public ContentController(IContentRepository contentRepository, IMapper mapper, IUrlHelper urlHelper)
        {
            _contentRepository = contentRepository;
            _mapper = mapper;
            _urlHelper = urlHelper;
        }

        [AllowAnonymous]
        [HttpPost("createContent")]
        public IActionResult AddContent([FromBody] ContentDto contentDto)
        {
            if (!ModelState.IsValid)
            {
                // return 422
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }

            try
            {
                // save
                var contentFromRepo = _contentRepository.AddContent(contentDto);

                var contentToReturn = _mapper.Map<ContentDto>(contentFromRepo);

                return Ok(contentToReturn);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("getContent/{userId}")]
        public IActionResult GetContent(Guid userId)
        {
            var contentsFromRepo = _contentRepository.GetContentByUser(userId);

            if (contentsFromRepo == null)
            {
                return NotFound();
            }

            IEnumerable<ContentDto> contentDtos = _mapper.Map<IEnumerable<ContentDto>>(contentsFromRepo);

            return Ok(contentDtos);
        }

        [AllowAnonymous]
        [HttpPut("updateContent")]
        public IActionResult UpdateContent([FromBody]ContentDto contentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // return 422
                    return new Helpers.UnprocessableEntityObjectResult(ModelState);
                }

                _contentRepository.UpdateContent(contentDto);

                if (!_contentRepository.Save())
                {
                    throw new AppException("Updating content failed on save.");
                }

                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpDelete("deleteContent/{contentId}")]
        public IActionResult DeleteContent(Guid contentId)
        {
            try
            {
                _contentRepository.DeleteContent(contentId);

                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WhiteboardAPI.Helpers;
using WhiteboardAPI.Model;
using WhiteboardAPI.Repository;
using WhiteboardAPI.Entities;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WhiteboardAPI.Controllers
{
    [Authorize]
    [ApiController]
    public class UserController : Controller
    {

        private IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _generator;
        private readonly IEmailSender _emailSender;

        public UserController(IUserRepository userRepository, IMapper mapper, IOptions<AppSettings> appSettings, IHttpContextAccessor accessor, LinkGenerator generator, IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _accessor = accessor;
            _generator = generator;
            _emailSender = emailSender;
        }
        private string CreateResourceUri(ResourceParameters resourceParameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _generator.GetUriByPage(_accessor.HttpContext,
                        handler: null,
                        page: "/getAllUsers",
                        values:
                        new
                        {
                            orderBy = resourceParameters.OrderBy,
                            pageNumber = resourceParameters.PageNumber - 1,
                            pageSize = resourceParameters.PageSize
                        });
                case ResourceUriType.NextPage:
                    return _generator.GetUriByPage(_accessor.HttpContext,
                        handler: null,
                        page: "/getAllUsers",
                        values:
                        new
                        {
                            orderBy = resourceParameters.OrderBy,
                            pageNumber = resourceParameters.PageNumber + 1,
                            pageSize = resourceParameters.PageSize
                        });
                default:
                    return _generator.GetUriByPage(_accessor.HttpContext,
                        handler: null,
                        page: "/getAllUsers",
                        values:
                        new
                        {
                            orderBy = resourceParameters.OrderBy,
                            pageNumber = resourceParameters.PageNumber,
                            pageSize = resourceParameters.PageSize
                        });
            }
        }

        [AllowAnonymous]
        [HttpGet("getAllUsers")]
        public IActionResult GetAllUsers([FromQuery] ResourceParameters resourceParameters)
        {
            if (string.IsNullOrEmpty(resourceParameters.OrderBy))
                resourceParameters.OrderBy = "Username";

            var usersFromRepo = _userRepository.GetAllUsers(resourceParameters);

            if (usersFromRepo == null)
            {
                return NotFound();
            }

            IEnumerable<UserDto> userDtos = _mapper.Map<IEnumerable<UserDto>>(usersFromRepo);

            Users users = new Users
            {
                UserDtos = userDtos
            };

            users.TotalCount = userDtos.Count();

            var previousPageLink = usersFromRepo.HasPrevious ? CreateResourceUri(resourceParameters, ResourceUriType.PreviousPage) : null;

            var x = CreateResourceUri(resourceParameters, ResourceUriType.NextPage);

            var nextPageLink = usersFromRepo.HasNext ? CreateResourceUri(resourceParameters, ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = usersFromRepo.TotalCount,
                pageSize = usersFromRepo.PageSize,
                currentPage = usersFromRepo.CurrentPage,
                totalPages = usersFromRepo.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            return Ok(users);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserAuthenticationDto userDto)
        {
            var user = _userRepository.Authenticate(userDto.Username, userDto.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var userFromRepo = _mapper.Map<UserDto>(user);

            userFromRepo.Token = tokenHandler.WriteToken(token);

            return Ok(userFromRepo);
        }

        [AllowAnonymous]
        [HttpPost("createUser")]
        public IActionResult Register(UserCreationDto userCreationDto)
        {
            if (!ModelState.IsValid)
            {
                // return 422
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }

            try
            {
                // save
                var userFromRepo = _userRepository.Create(userCreationDto);

                var userToReturn = _mapper.Map<UserDto>(userFromRepo);

                return Ok(userToReturn);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("getUsers")]
        public IActionResult GetUsers([FromQuery] List<Guid> userIds)
        {
            var usersFromRepo = _userRepository.GetUsers(userIds);

            if (usersFromRepo == null)
            {
                return NotFound();
            }

            IEnumerable<UserDto> userDtos = _mapper.Map<IEnumerable<UserDto>>(usersFromRepo);

            Users users = new Users
            {
                UserDtos = userDtos
            };

            users.TotalCount = userDtos.Count();

            return Ok(users);
        }

        [AllowAnonymous]
        [HttpPut("{userId}")]
        public IActionResult UpdateUser(Guid userId, [FromBody]UserUpdateDto userUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // return 422
                    return new Helpers.UnprocessableEntityObjectResult(ModelState);
                }

                if (!_userRepository.UserExists(userId))
                {
                    return NotFound();
                }

                // map dto to entity and set id
                var user = _mapper.Map<UserDE>(userUpdateDto);

                _userRepository.UpdateUser(userId, user);

                if (!_userRepository.Save())
                {
                    throw new AppException("Updating {id} for user failed on save.");
                }

                return NoContent();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpDelete("{userId}")]
        public IActionResult DeleteUser(Guid userId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // return 422
                    return new Helpers.UnprocessableEntityObjectResult(ModelState);
                }

                if (!_userRepository.UserExists(userId))
                {
                    return NotFound();
                }

                _userRepository.DeleteUser(userId);

                if (!_userRepository.Save())
                {
                    throw new AppException("Updating {id} for user failed on save.");
                }

                return NoContent();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPasswordAsync([FromBody] UserUpdatePasswordDto userUpdatePasswordDto)
        {
            try
            {
                var user = _userRepository.GetUserByEmail(userUpdatePasswordDto.Email);

                if (user == null)
                    return BadRequest(new { message = "Cannot find user. Please check the username." });

                //var userFromRepo = _mapper.Map<UserDto>(user);


                string url = "http://localhost:8080/resetpassword/" + user.UserId;
                // var path = Path.Combine(Directory.GetCurrentDirectory(), "forgotpassword.html");

                // string html = System.IO.File.ReadAllText(path);

                string html = Helpers.StringBuilder.Html;
                //string html = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html style=\"width: 100%;font-family: lato, 'helvetica neue', helvetica, arial, sans-serif;-webkit-text-size-adjust: 100%;-ms-text-size-adjust: 100%;padding: 0;margin: 0;\">\r\n<head>\r\n    <meta charset=\"UTF-8\" />\r\n    <meta content=\"width=device-width, initial-scale=1\" name=\"viewport\" />\r\n    <meta name=\"x-apple-disable-message-reformatting\" />\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />\r\n    <meta content=\"telephone=no\" name=\"format-detection\" />\r\n    <title></title>\r\n    <!--[if (mso 16)]>\r\n    <style type=\"text/css\">\r\n    a {text-decoration: none;}\r\n    </style>\r\n    <![endif]-->\r\n    <!--[if gte mso 9]><style>sup { font-size: 100% !important; }</style><![endif]-->\r\n    <!--[if !mso]><!-- -->\r\n    <link href=\"https://fonts.googleapis.com/css?family=Lato:400,400i,700,700i\" rel=\"stylesheet\" />\r\n    <!--<![endif]-->\r\n</head>\r\n<body style=\"width: 100%;font-family: lato, 'helvetica neue', helvetica, arial, sans-serif;-webkit-text-size-adjust: 100%;-ms-text-size-adjust: 100%;padding: 0;margin: 0;\">\r\n    <div class=\"es-wrapper-color\" style=\"background-color: #f4f4f4;\">\r\n        <!--[if gte mso 9]>\r\n            <v:background xmlns:v=\"urn:schemas-microsoft-com:vml\" fill=\"t\">\r\n                <v:fill type=\"tile\" color=\"#f4f4f4\"></v:fill>\r\n            </v:background>\r\n        <![endif]-->\r\n        <table class=\"es-wrapper\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;padding: 0;margin: 0;width: 100%;height: 100%;background-image: ;background-repeat: repeat;background-position: center top;\">\r\n            <tbody>\r\n                <tr class=\"gmail-fix\" height=\"0\" style=\"border-collapse: collapse;\">\r\n                    <td style=\"padding: 0;margin: 0;\">\r\n                        <table width=\"600\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" align=\"center\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\">\r\n                            <tbody>\r\n                                <tr style=\"border-collapse: collapse;\">\r\n                                    <td cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"line-height: 1px;min-width: 600px;padding: 0;margin: 0;\" height=\"0\"><img src=\"https://esputnik.com/repository/applications/images/blank.gif\" style=\"display: block;max-height: 0px;min-height: 0px;min-width: 600px;width: 600px;border: 0;outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;\" alt=alt width=\"600\" height=\"1\" /></td>\r\n                                </tr>\r\n                            </tbody>\r\n                        </table>\r\n                    </td>\r\n                </tr>\r\n                <tr style=\"border-collapse: collapse;\">\r\n                    <td class=\"esd-email-paddings\" valign=\"top\" style=\"padding: 0;margin: 0;\">\r\n                        <table class=\"es-header esd-header-popover\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;width: 100%;background-color: #7c72dc;background-image: ;background-repeat: repeat;background-position: center top;table-layout: fixed !important;\">\r\n                            <tbody>\r\n                                <tr style=\"border-collapse: collapse;\">\r\n                                    <td class=\"esd-stripe\" style=\"background-color: rgb(24, 118, 210);padding: 0;margin: 0;\" bgcolor=\"#1876d2\" align=\"center\">\r\n                                        <table class=\"es-header-body\" width=\"600\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" bgcolor=\"#6fa8dc\" style=\"background-color: rgb(111, 168, 220);mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\">\r\n                                            <tbody>\r\n                                                <tr style=\"border-collapse: collapse;\">\r\n                                                    <td class=\"esd-structure es-p40t es-p40b es-p25r es-p30l\" align=\"left\" bgcolor=\"#1876d2\" style=\"background-color: rgb(24, 118, 210);padding: 0;margin: 0;padding-right: 25px;padding-left: 30px;padding-top: 40px;padding-bottom: 40px;\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\">\r\n                                                            <tbody>\r\n                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                    <td width=\"545\" class=\"esd-container-frame\" align=\"center\" valign=\"top\" style=\"padding: 0;margin: 0;\">\r\n                                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\">\r\n                                                                            <tbody>\r\n                                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                                    <td align=\"center\" class=\"esd-empty-container\" style=\"display: none;padding: 0;margin: 0;\"></td>\r\n                                                                                </tr>\r\n                                                                            </tbody>\r\n                                                                        </table>\r\n                                                                    </td>\r\n                                                                </tr>\r\n                                                            </tbody>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                                <tr style=\"border-collapse: collapse;\">\r\n                                                    <td class=\"esd-structure es-p20t es-p10b es-p10r es-p10l\" align=\"left\" bgcolor=\"#ffffff\" style=\"background-color: rgb(255, 255, 255);padding: 0;margin: 0;padding-bottom: 10px;padding-left: 10px;padding-right: 10px;padding-top: 20px;\">\r\n                                                        <table width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\">\r\n                                                            <tbody>\r\n                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                    <td class=\"esd-container-frame\" width=\"580\" valign=\"top\" align=\"center\" style=\"padding: 0;margin: 0;\">\r\n                                                                        <table width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\">\r\n                                                                            <tbody>\r\n                                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                                    <td align=\"center\" class=\"esd-block-image\" style=\"padding: 0;margin: 0;\"><a target=\"_blank\" style=\"-webkit-text-size-adjust: none;-ms-text-size-adjust: none;mso-line-height-rule: exactly;font-family: lato, 'helvetica neue', helvetica, arial, sans-serif;font-size: 14px;text-decoration: underline;color: #111111;\"><img class=\"adapt-img\" src=\"https://demo.stripocdn.email/content/guids/37eb7096-9835-4c4a-84f3-cbd3132ba04b/images/64291572418868992.png\" alt=alt style=\"display: block;border: 0;outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;\" width=\"220\" /></a></td>\r\n                                                                                </tr>\r\n                                                                            </tbody>\r\n                                                                        </table>\r\n                                                                    </td>\r\n                                                                </tr>\r\n                                                            </tbody>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </tbody>\r\n                                        </table>\r\n                                    </td>\r\n                                </tr>\r\n                            </tbody>\r\n                        </table>\r\n                        <table class=\"es-content\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;width: 100%;table-layout: fixed !important;\">\r\n                            <tbody>\r\n                                <tr style=\"border-collapse: collapse;\">\r\n                                    <td class=\"esd-stripe\" style=\"background-color: rgb(24, 118, 210);padding: 0;margin: 0;\" esd-custom-block-id=\"6340\" bgcolor=\"#1876d2\" align=\"center\">\r\n                                        <table class=\"es-content-body\" style=\"background-color: transparent;mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\" width=\"600\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\">\r\n                                            <tbody>\r\n                                                <tr style=\"border-collapse: collapse;\">\r\n                                                    <td class=\"esd-structure\" align=\"left\" style=\"padding: 0;margin: 0;\">\r\n                                                        <table width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\">\r\n                                                            <tbody>\r\n                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                    <td class=\"esd-container-frame\" width=\"600\" valign=\"top\" align=\"center\" style=\"padding: 0;margin: 0;\">\r\n                                                                        <table style=\"background-color: rgb(255, 255, 255);border-radius: 4px;border-collapse: separate;mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-spacing: 0px;\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\">\r\n                                                                            <tbody>\r\n                                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                                    <td class=\"esd-block-text es-p35t es-p5b es-p30r es-p30l\" align=\"center\" style=\"padding: 0;margin: 0;padding-bottom: 5px;padding-left: 30px;padding-right: 30px;padding-top: 35px;\">\r\n                                                                                        <h1 style=\"margin: 0;line-height: 120%;mso-line-height-rule: exactly;font-family: lato, 'helvetica neue', helvetica, arial, sans-serif;font-size: 48px;font-style: normal;font-weight: normal;color: #111111;\">Trouble signing in?</h1>\r\n                                                                                    </td>\r\n                                                                                </tr>\r\n                                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                                    <td class=\"esd-block-spacer es-p5t es-p5b es-p20r es-p20l\" bgcolor=\"#ffffff\" align=\"center\" style=\"padding: 0;margin: 0;padding-top: 5px;padding-bottom: 5px;padding-left: 20px;padding-right: 20px;\">\r\n                                                                                        <table width=\"100%\" height=\"100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\">\r\n                                                                                            <tbody>\r\n                                                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                                                    <td style=\"border-bottom: 1px solid rgb(255, 255, 255);background: rgba(0, 0, 0, 0) none repeat scroll 0% 0%;height: 1px;width: 100%;margin: 0px;padding: 0;\"></td>\r\n                                                                                                </tr>\r\n                                                                                            </tbody>\r\n                                                                                        </table>\r\n                                                                                    </td>\r\n                                                                                </tr>\r\n                                                                            </tbody>\r\n                                                                        </table>\r\n                                                                    </td>\r\n                                                                </tr>\r\n                                                            </tbody>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </tbody>\r\n                                        </table>\r\n                                    </td>\r\n                                </tr>\r\n                            </tbody>\r\n                        </table>\r\n                        <table class=\"es-content\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;width: 100%;table-layout: fixed !important;\">\r\n                            <tbody>\r\n                                <tr style=\"border-collapse: collapse;\">\r\n                                    <td class=\"esd-stripe\" align=\"center\" style=\"padding: 0;margin: 0;\">\r\n                                        <table class=\"es-content-body\" style=\"background-color: rgb(255, 255, 255);mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\" width=\"600\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\" align=\"center\">\r\n                                            <tbody>\r\n                                                <tr style=\"border-collapse: collapse;\">\r\n                                                    <td class=\"esd-structure\" align=\"left\" style=\"padding: 0;margin: 0;\">\r\n                                                        <table width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\">\r\n                                                            <tbody>\r\n                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                    <td class=\"esd-container-frame\" width=\"600\" valign=\"top\" align=\"center\" style=\"padding: 0;margin: 0;\">\r\n                                                                        <table style=\"background-color: rgb(255, 255, 255);mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\">\r\n                                                                            <tbody>\r\n                                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                                    <td class=\"esd-block-text es-m-txt-l es-p20t es-p15b es-p30r es-p30l\" bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 0;margin: 0;padding-bottom: 15px;padding-top: 20px;padding-left: 30px;padding-right: 30px;\">\r\n                                                                                        <p style=\"margin: 0;-webkit-text-size-adjust: none;-ms-text-size-adjust: none;mso-line-height-rule: exactly;font-size: 18px;font-family: lato, 'helvetica neue', helvetica, arial, sans-serif;line-height: 150%;color: #666666;\">Resetting your password is easy. Just press the button below and follow the instructions. We'll have you up and running in no time.</p>\r\n                                                                                    </td>\r\n                                                                                </tr>\r\n                                                                            </tbody>\r\n                                                                        </table>\r\n                                                                    </td>\r\n                                                                </tr>\r\n                                                            </tbody>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                                <tr style=\"border-collapse: collapse;\">\r\n                                                    <td class=\"esd-structure es-p20b es-p30r es-p30l\" align=\"left\" bgcolor=\"transparent\" style=\"background-color: transparent;padding: 0;margin: 0;padding-bottom: 20px;padding-left: 30px;padding-right: 30px;\">\r\n                                                        <table width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\">\r\n                                                            <tbody>\r\n                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                    <td class=\"esd-container-frame\" width=\"540\" valign=\"top\" align=\"center\" style=\"padding: 0;margin: 0;\">\r\n                                                                        <table width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\">\r\n                                                                            <tbody>\r\n                                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                                    <td class=\"esd-block-button es-p40t es-p40b es-p10r es-p10l\" align=\"center\" style=\"padding: 0;margin: 0;padding-left: 10px;padding-right: 10px;padding-top: 40px;padding-bottom: 40px;\"><span class=\"es-button-border\" style=\"background: rgb(24, 118, 210);border-style: solid solid solid solid;border-color: #7c72dc #7c72dc #7c72dc #7c72dc;border-width: 1px 1px 1px 1px;display: inline-block;border-radius: 2px;width: auto;\"><a href=\"parkingsloturl\" class=\"es-button\" target=\"_blank\" style=\"background: rgb(24, 118, 210);border-color: rgb(24, 118, 210);-webkit-text-size-adjust: none;-ms-text-size-adjust: none;mso-line-height-rule: exactly;font-family: helvetica, 'helvetica neue', arial, verdana, sans-serif;font-size: 20px;text-decoration: none !important;color: #ffffff;border-style: solid;border-width: 15px 25px 15px 25px;display: inline-block;border-radius: 2px;font-weight: normal;font-style: normal;line-height: 120%;width: auto;text-align: center;mso-style-priority: 100 !important;\">Reset Password</a></span></td>\r\n                                                                                </tr>\r\n                                                                            </tbody>\r\n                                                                        </table>\r\n                                                                    </td>\r\n                                                                </tr>\r\n                                                            </tbody>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </tbody>\r\n                                        </table>\r\n                                    </td>\r\n                                </tr>\r\n                            </tbody>\r\n                        </table>\r\n                        <table class=\"es-content\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;width: 100%;table-layout: fixed !important;\">\r\n                            <tbody>\r\n                                <tr style=\"border-collapse: collapse;\">\r\n                                    <td class=\"esd-stripe\" align=\"center\" style=\"padding: 0;margin: 0;\">\r\n                                        <table class=\"es-content-body\" style=\"background-color: transparent;mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\" width=\"600\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\">\r\n                                            <tbody>\r\n                                                <tr style=\"border-collapse: collapse;\">\r\n                                                    <td class=\"esd-structure\" align=\"left\" style=\"padding: 0;margin: 0;\">\r\n                                                        <table width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\">\r\n                                                            <tbody>\r\n                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                    <td class=\"esd-container-frame\" width=\"600\" valign=\"top\" align=\"center\" style=\"padding: 0;margin: 0;\">\r\n                                                                        <table width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\">\r\n                                                                            <tbody>\r\n                                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                                    <td class=\"esd-block-spacer es-p10t es-p20b es-p20r es-p20l\" align=\"center\" style=\"padding: 0;margin: 0;padding-top: 10px;padding-bottom: 20px;padding-left: 20px;padding-right: 20px;\">\r\n                                                                                        <table width=\"100%\" height=\"100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\">\r\n                                                                                            <tbody>\r\n                                                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                                                    <td style=\"border-bottom: 1px solid rgb(244, 244, 244);background: rgba(0, 0, 0, 0) none repeat scroll 0% 0%;height: 1px;width: 100%;margin: 0px;padding: 0;\"></td>\r\n                                                                                                </tr>\r\n                                                                                            </tbody>\r\n                                                                                        </table>\r\n                                                                                    </td>\r\n                                                                                </tr>\r\n                                                                            </tbody>\r\n                                                                        </table>\r\n                                                                    </td>\r\n                                                                </tr>\r\n                                                            </tbody>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </tbody>\r\n                                        </table>\r\n                                    </td>\r\n                                </tr>\r\n                            </tbody>\r\n                        </table>\r\n                        <table class=\"es-footer esd-footer-popover\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;width: 100%;background-color: transparent;background-image: ;background-repeat: repeat;background-position: center top;table-layout: fixed !important;\">\r\n                            <tbody>\r\n                                <tr style=\"border-collapse: collapse;\">\r\n                                    <td class=\"esd-stripe\" esd-custom-block-id=\"6342\" align=\"center\" style=\"padding: 0;margin: 0;\">\r\n                                        <table class=\"es-footer-body\" width=\"600\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;background-color: transparent;\">\r\n                                            <tbody>\r\n                                                <tr style=\"border-collapse: collapse;\">\r\n                                                    <td class=\"esd-structure es-p30t es-p30b es-p30r es-p30l\" align=\"left\" style=\"padding: 0;margin: 0;padding-top: 30px;padding-bottom: 30px;padding-left: 30px;padding-right: 30px;\">\r\n                                                        <table width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\">\r\n                                                            <tbody>\r\n                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                    <td class=\"esd-container-frame\" width=\"540\" valign=\"top\" align=\"center\" style=\"padding: 0;margin: 0;\">\r\n                                                                        <table width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"mso-table-lspace: 0pt;mso-table-rspace: 0pt;border-collapse: collapse;border-spacing: 0px;\">\r\n                                                                            <tbody>\r\n                                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                                    <td class=\"esd-block-text es-p25t\" align=\"left\" style=\"padding: 0;margin: 0;padding-top: 25px;\">\r\n                                                                                        <p style=\"margin: 0;-webkit-text-size-adjust: none;-ms-text-size-adjust: none;mso-line-height-rule: exactly;font-size: 14px;font-family: lato, 'helvetica neue', helvetica, arial, sans-serif;line-height: 150%;color: #666666;\">You received this email because you have requested a forgot password reset link. If it looks weird, <strong><a target=\"_self\" href=href style=\"-webkit-text-size-adjust: none;-ms-text-size-adjust: none;mso-line-height-rule: exactly;font-family: lato, 'helvetica neue', helvetica, arial, sans-serif;font-size: 14px;text-decoration: underline;color: #111111;\">view it in your browser</a></strong>.</p>\r\n                                                                                    </td>\r\n                                                                                </tr>\r\n                                                                                <tr style=\"border-collapse: collapse;\">\r\n                                                                                    <td class=\"esd-block-text es-p25t\" align=\"left\" style=\"padding: 0;margin: 0;padding-top: 25px;\">\r\n                                                                                        <p style=\"margin: 0;-webkit-text-size-adjust: none;-ms-text-size-adjust: none;mso-line-height-rule: exactly;font-size: 14px;font-family: lato, 'helvetica neue', helvetica, arial, sans-serif;line-height: 150%;color: #666666;\">ParkingSlot&copy; Team</p>\r\n                                                                                    </td>\r\n                                                                                </tr>\r\n                                                                            </tbody>\r\n                                                                        </table>\r\n                                                                    </td>\r\n                                                                </tr>\r\n                                                            </tbody>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </tbody>\r\n                                        </table>\r\n                                    </td>\r\n                                </tr>\r\n                            </tbody>\r\n                        </table>\r\n                    </td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n    </div>\r\n</body>\r\n</html>";
                // string html = "Please reset your password by clicking <a href=\"" + url + "\">here</a>";
                var position = html.IndexOf("whiteboardurl");

                html = Helpers.StringBuilder.ReplaceAt(html, position, 14, url);

                await _emailSender.SendEmailAsync(user.Email, "Whiteboard Password Reset Link", html);

                return Ok(new { Message = "Reset password email sent." });
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public IActionResult ResetPassword([FromBody] UserUpdatePasswordDto userUpdatePasswordDto)
        {
            if (userUpdatePasswordDto.Username == null)
            {
                return BadRequest("Please provide a username.");
            }

            var user = _userRepository.Authenticate(userUpdatePasswordDto.Username, userUpdatePasswordDto.OldPassword);

            if (user == null)
            {
                return BadRequest("Incorrect current password entered.");
            }

            _userRepository.UpdatePassword(user, userUpdatePasswordDto.NewPassword);

            return Ok("Password reset sucessfully");
        }
    }
}

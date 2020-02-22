using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using WhiteboardAPI.Database;
using WhiteboardAPI.Entities;
using WhiteboardAPI.Helpers;
using WhiteboardAPI.Model;

namespace WhiteboardAPI.Repository
{
    public interface IUserRepository
    {
        User Authenticate(string username, string password);
        //PagedList<User> GetUsers(UserResourceParameters userResourceParameters);
        User Create(UserCreationDto user);
        //User GetUser(Guid userId);
        //User GetUserByEmail(string Email);
        //void UpdateUser(Guid userId, User userParam);
        //void UpdatePassword(User user, string password);
        //void DeleteUser(User user);
        //bool UserExists(Guid userId);
        //bool Save();
    }

    public class UserRepository : IUserRepository
    {
        private List<User> _users = new List<User>
        {
            new User { UserName = "Ah Mao", Email="ahmao@gmail.com", UserId = Guid.NewGuid(), Role = "Admin" }
        };


        private WhiteboardContext _context;
        private readonly AppSettings _appSettings;
        private readonly IPropertyMappingService _propertyMappingService;

        public UserRepository(WhiteboardContext context, IOptions<AppSettings> appSettings, IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _propertyMappingService = propertyMappingService;
        }

        public User Create(UserCreationDto user)
        {
            // validation
            if (string.IsNullOrWhiteSpace(user.Password))
                throw new AppException("Password is required");

            if (_context.tbl_user.Any(x => x.UserName == user.UserName))
                throw new AppException("Username \"" + user.UserName + "\" is already taken");

            if (user.Email != null)
            {
                if (_context.tbl_user.Any(x => x.Email == user.Email))
                    throw new AppException("Email \"" + user.Email + "\" is already taken");
            }

            if (user.PhoneNo == null)
            {
                if (_context.tbl_user.Any(x => x.PhoneNo == user.PhoneNo))
                    throw new AppException("Phone number \"" + user.PhoneNo + "\" is already taken");
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(user.Password, out passwordHash, out passwordSalt);

            User newUser = new User
            {
                UserId = new Guid(),
                UserName = user.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Email = user.Email,
                PhoneNo = user.PhoneNo,
                Role = user.Role
            };

            _context.tbl_user.Add(newUser);
            _context.SaveChanges();

            return newUser;
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.tbl_user.SingleOrDefault(x => x.UserName == username);


            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }


    }
}

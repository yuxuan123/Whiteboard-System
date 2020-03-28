using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Options;
using WhiteboardAPI.Database;
using WhiteboardAPI.Entities;
using WhiteboardAPI.Helpers;
using WhiteboardAPI.Model;

namespace WhiteboardAPI.Repository
{
    public interface IUserRepository
    {
        UserDE Authenticate(string username, string password);
        PagedList<UserDE> GetAllUsers(ResourceParameters resourceParameters);
        UserDE Create(UserCreationDto user);
        IEnumerable<UserDE> GetUsers(List<Guid> userIds);
        UserDE GetUser(Guid userId);
        void UpdateUser(Guid userId, UserDE userParam);
        //void UpdatePassword(User user, string password);
        void DeleteUser(Guid userId);
        bool UserExists(Guid userId);
        UserDE GetUserByEmail(string email);
        void UpdatePassword(UserDE user, string password);
        bool Save();
    }

    public class UserRepository : IUserRepository
    {
        private List<UserDE> _users = new List<UserDE>
        {
            new UserDE { UserName = "Ah Mao", Email="ahmao@gmail.com", UserId = Guid.NewGuid(), Role = "Admin" }
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

        public UserDE Create(UserCreationDto user)
        {
            // default password when admin creates a user
            if (string.IsNullOrWhiteSpace(user.Password))
                user.Password = "Password123";

            // validation
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

            UserDE newUser = new UserDE
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

        public PagedList<UserDE> GetAllUsers(ResourceParameters resourceParameters)
        {
            var collectionBeforePaging = string.IsNullOrEmpty(resourceParameters.keyword) ? _context.tbl_user.OrderBy(resourceParameters.OrderBy).ToList() : _context.tbl_user.Where(x => x.UserName.Contains(resourceParameters.keyword)).OrderBy(resourceParameters.OrderBy).ToList();

            return PagedList<UserDE>.Create(collectionBeforePaging, resourceParameters.PageNumber, resourceParameters.PageSize);
        }

        public UserDE Authenticate(string username, string password)
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

        public IEnumerable<UserDE> GetUsers(List<Guid> userIds)
        {
            return _context.tbl_user.Where(x => userIds.Contains(x.UserId)).ToList();
        }

        public UserDE GetUser(Guid userId)
        {
            return _context.tbl_user.Where(x => x.UserId == userId).FirstOrDefault();
        }

        public bool UserExists(Guid guid)
        {
            return _context.tbl_user.Any(x => x.UserId == guid);
        }

        public void UpdateUser(Guid userId, UserDE userParam)
        {
            UserDE user = GetUser(userId);

            if (user == null)
            {
                throw new AppException("User not found");
            }

            // Update user properties
            user.UserName = string.IsNullOrEmpty(userParam.UserName) ? user.UserName : userParam.UserName;
            user.Email = string.IsNullOrEmpty(userParam.Email) ? user.Email : userParam.Email;
            user.PhoneNo = string.IsNullOrEmpty(userParam.PhoneNo) ? user.PhoneNo : userParam.PhoneNo;

            _context.tbl_user.Update(user);
        }

        public void DeleteUser(Guid userId)
        {
            UserDE user = GetUser(userId);

            if (user == null)
            {
                throw new AppException("User not found");
            }

            _context.tbl_user.Remove(user);
        }

        public UserDE GetUserByEmail(string email)
        {
            return _context.tbl_user.Where(x => x.Email == email).FirstOrDefault();
        }

        public void UpdatePassword(UserDE user, string password)
        {
            byte[] passwordHash, passwordSalt;

            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.tbl_user.Update(user);

            if (!Save())
            {
                throw new AppException("Error in updating password.");
            }
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}

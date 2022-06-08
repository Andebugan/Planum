using System.Collections.Generic;
using System.Linq;

using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.IRepo;
using Planum.Models.DTO;
using Serilog;

namespace Planum.Models.BuisnessLogic.Managers
{
    public class UserManager : IUserManager
    {
        public User? CurrentUser { get; set; } = null;
        protected IUserRepo _userRepo;
        protected IUserConverter _userConverter;

        public UserManager(IUserRepo userRepo, IUserConverter userConverter)
        {
            _userRepo = userRepo;
            _userConverter = userConverter;
        }

        public int CreateUser(string login, string password)
        {
            Log.Debug("Create new user");
            List<User> users = GetAllUsers();

            if (users.Any(x => x.Login == login))
                throw new UserLoginAlreadyExistException();

            User new_user = new User(-1, login, password);
            UserDTO userDTO = _userConverter.ConvertToDTO(new_user);
            return _userRepo.AddUser(userDTO);
        }

        public void UpdateUser(int id, string login, string password)
        {
            Log.Debug($"Update user with id={id}");
            if (FindUser(id) == null)
                return;
            UserDTO new_user = new UserDTO(id, login, password);
            _userRepo.UpdateUser(new_user);
        }

        public void DeleteUser(ITaskManager taskManager, ITagManager tagManager)
        {
            Log.Debug("Delete current user");
            if (CurrentUser == null)
                return;
            if (FindUser(CurrentUser.Id) == null)
                return;
            Log.Information($"Delete user with id={CurrentUser.Id}");
            tagManager.DeleteConnectedToUser(CurrentUser.Id);
            taskManager.DeleteConnectedToUser(CurrentUser.Id);
            _userRepo.DeleteUser(CurrentUser.Id);
            CurrentUser = null;
        }

        public User GetUser(int id)
        {
            Log.Debug($"Get user with id={id}");
            return _userConverter.ConvertFromDTO(_userRepo.GetUser(id));
        }

        public User? FindUser(int id)
        {
            Log.Debug($"Find user with id={id}");
            UserDTO? user = _userRepo.FindUser(id);
            if (user == null)
                return null;
            return _userConverter.ConvertFromDTO(user);
        }

        public User? FindUser(string login)
        {
            Log.Debug($"Find user with login={login}");
            List<UserDTO> users = _userRepo.GetAllUsers();
            foreach (UserDTO user in users)
            {
                if (user.Login == login)
                    return _userConverter.ConvertFromDTO(user);
            }
            return null;
        }

        public List<User> GetAllUsers()
        {
            Log.Debug("Get all users");
            List<UserDTO> userDTOs = _userRepo.GetAllUsers();

            return userDTOs.Select(x => _userConverter.ConvertFromDTO(x)).ToList();
        }

        public User? SignIn(string login, string password)
        {
            Log.Debug("Sign in");
            List<UserDTO> userDTOs = _userRepo.GetAllUsers();
            User user;

            foreach (var userDTO in userDTOs)
            {
                user = _userConverter.ConvertFromDTO(userDTO);
                if (user.Login == login && user.Password == password)
                {
                    CurrentUser = user;
                    return user;
                }
            }
            return null;
        }
    }
}

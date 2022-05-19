using System.Collections.Generic;
using System.Linq;

using Planum.Models.BuisnessLayer.Entities;
using Planum.Models.BuisnessLayer.RepoInterfaces;
using Planum.Models.DTO;

namespace Planum.Models.BuisnessLayer.Managers
{
    public class UserManager : IUserManager
    {
        public User? CurrentUser { get; set; } = null;
        protected IUserRepo _userRepo;
        protected ITagManager _tagManager;
        protected ITaskManager _taskManager;
        protected IUserConverter _userConverter;

        public UserManager(IUserRepo userRepo, ITagManager tagManager, ITaskManager taskManager, IUserConverter userConverter)
        {
            _userRepo = userRepo;
            _tagManager = tagManager;
            _taskManager = taskManager;
            _userConverter = userConverter;
        }

        public int CreateUser(string login, string password)
        {
            List<User> users = GetAllUsers();

            if (users.Any(x => x.Login == login))
                throw new UserLoginAlreadyExistException();

            User new_user = new User(-1, login, password);
            UserDTO userDTO = _userConverter.ConvertToDTO(new_user);
            return _userRepo.AddUser(userDTO);
        }

        public void UpdateUser(int id, string login, string password)
        {
            if (FindUser(id) == null)
                return;
            UserDTO new_user = new UserDTO(id, login, password);
            _userRepo.UpdateUser(new_user);
        }

        public void DeleteUser(int id)
        {
            if (FindUser(id) == null)
                return;
            _tagManager.DeleteConnectedToUser(id);
            _taskManager.DeleteConnectedToUser(id);
            _userRepo.DeleteUser(id);
        }

        public User GetUser(int id) => _userConverter.ConvertFromDTO(_userRepo.GetUser(id));

        public User? FindUser(int id)
        {
            UserDTO? user = _userRepo.FindUser(id);
            if (user == null)
                return null;
            return _userConverter.ConvertFromDTO(user);
        }

        public List<User> GetAllUsers()
        {
            List<UserDTO> userDTOs = _userRepo.GetAllUsers();

            return userDTOs.Select(x => _userConverter.ConvertFromDTO(x)).ToList();
        }

        public User? SignIn(string login, string password)
        {
            List<UserDTO> userDTOs = _userRepo.GetAllUsers();
            User user = null;

            foreach (var userDTO in userDTOs)
            {
                user = _userConverter.ConvertFromDTO(userDTO);
                if (user.Login == login && user.Password == password)
                {
                    return user;
                }
            }
            return user;
        }
    }
}

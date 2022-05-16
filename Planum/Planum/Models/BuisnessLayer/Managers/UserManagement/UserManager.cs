using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Planum.Models.BuisnessLayer.Entities;
using Planum.Models.BuisnessLayer.Managers.UserManagement;
using Planum.Models.BuisnessLayer.RepoInterfaces;
using Planum.Models.DTO.ModelData;

namespace Planum.Models.BuisnessLayer.Managers
{
    public class UserManager : IUserManager
    {
        public User CurrentUser { get; set; }
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

        public void CreateUser(string login, string password)
        {
            List<User> users = GetAll();

            if (users.Any(x => x.Login == login))
                throw new UserLoginAlreadyExistException();

            // id in bd

            User new_user = new User(-1, login, password);
            UserDTO userDTO = _userConverter.ConvertToDTO(new_user);
            _userRepo.Add(userDTO);
        }

        public void Update(int id, string login, string password)
        {
            UserDTO new_user = new UserDTO(id, login, password);
            _userRepo.Update(new_user);
        }

        public void DeleteUser(int id)
        {
            try
            {
                GetUser(id);
            }
            catch (UserDoesNotExistException) { }
            _tagManager.DeleteConnectedToUser(id);
            _taskManager.DeleteConnectedToUser(id);
            _userRepo.Delete(id);
        }

        public User GetUser(int id) => _userConverter.ConvertFromDTO(_userRepo.Get(id));

        public List<User> GetAll()
        {
            List<UserDTO> userDTOs = _userRepo.GetAll();

            return userDTOs.Select(x => _userConverter.ConvertFromDTO(x)).ToList();
        }

        public User SignIn(string login, string password)
        {
            List<UserDTO> userDTOs = _userRepo.GetAll();
            User user = null;

            foreach (var userDTO in userDTOs)
            {
                user = _userConverter.ConvertFromDTO(userDTO);
                if (user.Login == login && user.Password == password)
                {
                    return user;
                }
            }
            throw new UserInvalidLoginOrPasswordException();
        }
    }
}

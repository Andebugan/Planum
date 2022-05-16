using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Planum.Models.BuisnessLayer.Entities;
using Planum.Models.BuisnessLayer.Interfaces;
using Planum.Models.DTO.ModelData;

namespace Planum.Models.BuisnessLayer.Managers
{
    [Serializable]
    public class UserLoginAlreadyExist : Exception
    {
        public UserLoginAlreadyExist() { }
        public UserLoginAlreadyExist(string message) : base(message) { }
        public UserLoginAlreadyExist(string message, Exception innerException) : base(message, innerException) { }
    }

    [Serializable]
    public class UserInvalidLoginOrPassword : Exception
    {
        public UserInvalidLoginOrPassword() { }
        public UserInvalidLoginOrPassword(string message) : base(message) { }
        public UserInvalidLoginOrPassword(string message, Exception innerException) : base(message, innerException) { }
    }


    public class UserManager
    {
        public User CurrentUser { get; set; }
        protected IUserRepo _userRepo;
        protected TagManager tagManager;
        protected TaskManager taskManager;

        public UserManager(ref IUserRepo userRepo, ref TagManager tagManager, ref TaskManager taskManager)
        {
            if (userRepo == null)
                throw new ArgumentNullException(nameof(userRepo));
            if (taskManager == null)
                throw new ArgumentNullException(nameof(taskManager));
            if (tagManager == null)
                throw new ArgumentNullException(nameof(tagManager));

            _userRepo = userRepo;
        }

        protected UserDTO ConvertToDTO(User user)
        {
            UserDTO userDTO = new UserDTO(user.Id, user.Login, user.Password);
            return userDTO;
        }

        protected User ConvertFromDTO(UserDTO userDTO)
        {
            User user = new User(userDTO.Id, userDTO.Login, userDTO.Password);
            return user;
        }

        public void CreateUser(string? login, string? password)
        {
            List<User> users = GetAll();

            if (users.Any(x => x.Login == login))
                throw new UserLoginAlreadyExist();

            int id = 0;
            foreach (var user in users)
            {
                if (id == user.Id)
                    id += 1;
            }

            User new_user = new User(id, login, password);
            UserDTO userDTO = ConvertToDTO(new_user);
            _userRepo.Add(userDTO);
        }

        public void Update(int id, string? login, string? password)
        {
            User user = GetUser(id);
            if (login == null)
                login = user.Login;
            if (password == null)
                password = user.Password;
            UserDTO new_user = new UserDTO(user.Id, login, password);
            _userRepo.Update(new_user);
        }

        public void DeleteUser(int id)
        {
            tagManager.DeleteConnectedToUser(id);
            taskManager.DeleteConnectedToUser(id);
            _userRepo.Delete(id);
        }

        public User GetUser(int id)
        {
            UserDTO userDTO = _userRepo.Get(id);
            return ConvertFromDTO(userDTO);
        }

        public List<User> GetAll()
        {
            List<UserDTO> userDTOs = _userRepo.GetAll();
            List<User> userList = new List<User>();
            foreach (var userDTO in userDTOs)
            {
                userList.Add(ConvertFromDTO(userDTO));
            }
            return userList;
        }

        public User SignIn(string login, string password)
        {
            List<UserDTO> userDTOs = _userRepo.GetAll();
            User user = null;

            foreach(var userDTO in userDTOs)
            {
                user = ConvertFromDTO(userDTO);
                if (user.Login == login && user.Password == password)
                {
                    return user;
                }
            }
            throw new UserInvalidLoginOrPassword();
        }
    }
}

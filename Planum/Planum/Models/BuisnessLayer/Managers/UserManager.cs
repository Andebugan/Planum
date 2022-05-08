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
    internal class UserManager
    {
        protected User? _currentUser;
        protected IUserRepo _userRepo;
        protected TagManager tagManager;
        protected TaskManager taskManager;

        public UserManager(ref User currentUser, ref IUserRepo userRepo, ref TagManager tagManager, ref TaskManager taskManager)
        {
            if (userRepo == null)
                throw new ArgumentNullException(nameof(userRepo));
            if (taskManager == null)
                throw new ArgumentNullException(nameof(taskManager));
            if (tagManager == null)
                throw new ArgumentNullException(nameof(tagManager));

            _currentUser = currentUser;
            _userRepo = userRepo;
        }

        protected UserDTO ConvertToDTO(User user)
        {
            UserParamsDTO userParamsDTO = new UserParamsDTO();

            userParamsDTO.id = user.Id;
            userParamsDTO.password = user.Password;
            userParamsDTO.login = user.Login;
            
            return new UserDTO(userParamsDTO);
        }

        protected User ConvertFromDTO(UserDTO user)
        {
            UserParams userParams = new UserParams();
            
            userParams.id = user.Id;
            userParams.password = user.Password;
            userParams.login = user.Login;

            return new User(userParams);
        }

        public void CreateUser(ref UserParams userParams)
        {
            User user = new User(userParams);
            UserDTO userDTO = ConvertToDTO(user);
            _userRepo.Add(userDTO);
        }

        public void UpdateUser(ref User user, ref UserParams userParams)
        {
            user.Update(userParams);
            UserDTO userDTO = ConvertToDTO(user);
            _userRepo.Update(userDTO);
        }

        public void DeleteUser(int id)
        {
            tagManager.DeleteConnectedToUser(id);
            taskManager.DeleteConnectedToUser(id);
            _userRepo.Delete(id);
        }

        public User? GetUser(int id)
        {
            if (_currentUser == null)
                return null;
            else
                return _currentUser;
        }

        public User? SignIn(string login, string password)
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
            return user;
        }
    }
}

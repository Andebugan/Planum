using Planum.Models.BuisnessLogic.Entities;
using System.Collections.Generic;

namespace Planum.Models.BuisnessLogic.Managers
{
    public interface IUserManager
    {
        User? CurrentUser { get; set; }
        int CreateUser(string login, string password);
        void DeleteUser(ITaskManager taskManager, ITagManager tagManager);
        User? FindUser(int id);
        User? FindUser(string login);
        List<User> GetAllUsers();
        User GetUser(int id);
        User? SignIn(string login, string password);
        void UpdateUser(int id, string login, string password);
    }
}
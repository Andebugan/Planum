using Planum.Models.BuisnessLayer.Entities;
using System.Collections.Generic;

namespace Planum.Models.BuisnessLayer.Managers
{
    public interface IUserManager
    {
        User? CurrentUser { get; set; }

        int CreateUser(string login, string password);
        void DeleteUser(int id);
        User? FindUser(int id);
        List<User> GetAllUsers();
        User GetUser(int id);
        User? SignIn(string login, string password);
        void UpdateUser(int id, string login, string password);
    }
}
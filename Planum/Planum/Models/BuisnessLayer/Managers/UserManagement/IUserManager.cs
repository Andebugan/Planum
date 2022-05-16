using Planum.Models.BuisnessLayer.Entities;
using System.Collections.Generic;

namespace Planum.Models.BuisnessLayer.Managers
{
    public interface IUserManager
    {
        User CurrentUser { get; set; }

        void CreateUser(string login, string password);
        void DeleteUser(int id);
        List<User> GetAll();
        User GetUser(int id);
        User SignIn(string login, string password);
        void Update(int id, string login, string password);
    }
}
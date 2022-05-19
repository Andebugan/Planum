namespace Planum.Models.BuisnessLayer.Entities
{
    public class User
    {
        public int Id { get; }

        public string Login { get; }

        public string Password { get; }

        public User(int id, string login, string password)
        {
            Id = id;
            Login = login;
            Password = password;
        }
    }
}

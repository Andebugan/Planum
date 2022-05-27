namespace Planum.Models.DTO.ModelViewModel
{
    internal class UserViewModelDTO
    {
        public int Id { get; }
        public string Login { get; }
        public string Password { get; }

        public UserViewModelDTO(int id, string login = "", string password = "")
        {
            Id = id;
            Login = login;
            Password = password;
        }
    }
}

namespace Planum.Models.DTO.ModelViewModel
{
    internal class TagViewModelDTO
    {
        public int Id { get; }
        public int UserId { get; }
        public int Category { get; }
        public string Name { get; }
        public string Description { get; }

        public TagViewModelDTO(int id, int userId, int category = -1, string name = "", string description = "")
        {
            Id = id;
            UserId = userId;
            Category = category;
            Name = name;
            Description = description;
        }
    }
}

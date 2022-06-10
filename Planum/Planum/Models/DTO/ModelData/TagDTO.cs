using System;

namespace Planum.Models.DTO
{
    public class TagDTO
    {
        public int Id { get; }
        public int UserId { get; }
        public string Category { get; }
        public string Name { get; }
        public string Description { get; }

        public TagDTO(int id, int userId, string category = "", string name = "", string description = "")
        {
            Id = id;
            UserId = userId;
            Category = category;
            Name = name;    
            Description = description;
        }
    }
}
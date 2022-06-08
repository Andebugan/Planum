using System;

namespace Planum.Models.DTO
{
    public class TagDTO
    {
        public int Id { get; }
        public int UserId { get; }
        public int Category { get; }
        public string Name { get; }
        public string Description { get; }

        public TagDTO(int id, int userId, int category = -1, string name = "", string description = "")
        {
            Id = id;
            UserId = userId;
            Category = category;
            Name = name;    
            Description = description;
        }
    }
}
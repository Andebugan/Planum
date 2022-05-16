using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Models.DTO.ModelData
{
    public class TagDTO
    {
        public int Id { get; protected set; }
        public int UserId { get; protected set; }
        public int Category { get; protected set; }
        public string? Name { get; protected set; }
        public string? Description { get; protected set; }

        public TagDTO(int id, int userId, int category, string? name, string? description)
        {
            Id = id;
            UserId = UserId;
            Category = Category;
            Name = name;    
            Description = description;
        }
    }
}
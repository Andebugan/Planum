using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.ViewModels
{
    public class TagViewDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public TagViewDTO(int id, int user_id, string category, string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tag name can not be null or empty", nameof(name));

            Id = id;
            UserId = user_id;
            Category = category;
            Name = name;
            Description = description;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Models.BuisnessLayer.Entities
{
    public class Tag
    {
        public int Id { get; }
        public int UserId { get; }
        public int Category { get; }
        public string Name { get; }
        public string Description { get; }

        public Tag(int id, int user_id, int category, string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name can not be null or empty", nameof(name));

            Id = id;
            UserId = user_id;
            Category = category;
            Name = name;
            Description = description;
        }
    }
}

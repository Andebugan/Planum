using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Models.BuisnessLayer.Entities
{
    public class Tag
    {
        int _id;
        public int Id
        {
            get { return _id; }

            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _id = value;
            }
        }

        int _userId;
        public int UserId
        {
            get { return _userId; }

            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _userId = value;
            }
        }

        public int Category { get; private set; }
        public string? Name { get; private set; }
        public string? Description { get; private set; }

        public Tag(int id, int user_id, int category, string? name, string? description)
        {
            Id = id;
            UserId = user_id;
            Category = category;
            Name = name;
            Description = description;
        }

        public Tag(Tag tag)
        {
            Id = tag.Id;
            UserId = tag.UserId;
            Category = tag.Category;
            Name = tag.Name;
            Description = tag.Description;
        }
    }
}

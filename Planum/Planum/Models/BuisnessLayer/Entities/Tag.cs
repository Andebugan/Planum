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

        public override bool Equals(Object? obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Tag tag = (Tag)obj;
                if (tag.Id != Id)
                    return false;
                if (tag.UserId != UserId)
                    return false;
                if (tag.Category != Category)
                    return false;
                if (tag.Description != Description)
                    return false;
                if (tag.Name != Name)
                    return false;
                return true;
            }
        }

        public override string ToString()
        {
            return String.Format("Tag({0}, {1}, {2}, {3}, {4})", Id, UserId, Category, Name, Description);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}

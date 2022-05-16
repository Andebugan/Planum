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
            UserId = userId;
            Category = category;
            Name = name;    
            Description = description;
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
                TagDTO tag = (TagDTO)obj;
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
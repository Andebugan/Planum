using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Models.DTO.ModelData
{
    struct TagParamsDTO
    {
        public int id;
        public int userId;
        public int category;
        public string name;
        public string description;
    }

    internal class TagDTO
    {
        protected int _id;
        public int Id { get { return _id; } set { _id = value; } }

        protected int _userId;
        public int UserId { get { return _userId; } set { _userId = value; } }

        protected int _category;
        public int Category { get { return _category; } set { _category = value; } }

        protected string _name = "";
        public string Name { get { return _name; } set { _name = value; } }

        protected string _description = "";
        public string Description { get { return _description; } set { _description = value; } }

        public TagDTO(TagParamsDTO tagParams)
        {
            Id = tagParams.id;
            UserId = tagParams.userId;
            Category = tagParams.category;
            Name = tagParams.name;
            Description = tagParams.description;
        }
    }
}
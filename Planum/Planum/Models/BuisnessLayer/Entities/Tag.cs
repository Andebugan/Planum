using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Models.BuisnessLayer.Entities
{
    struct TagParams
    {
        public int id;
        public int userId;
        public int category;
        public string name;
        public string description;
    }

    internal class Tag
    {
        protected int _id;
        public int Id { get { return _id; } }

        protected int _userId;
        public int UserId { get { return _userId; } } 

        protected int _category;
        public int Category { get { return _category; } }

        protected string _name = "";
        public string Name { get { return _name; } }

        protected string _description = "";
        public string Description { get { return _description; } }

        public Tag(TagParams tagParams)
        {
            _id = tagParams.id;
            _userId = tagParams.userId;
            _category = tagParams.category;
            _name = tagParams.name;
            _description = tagParams.description;
        }

        public void Update(TagParams tagParams)
        {
            _id = tagParams.id;
            _userId = tagParams.userId;
            _category = tagParams.category;
            _name = tagParams.name;
            _description = tagParams.description;
        }

        public TagParams GetTagParams()
        {
            TagParams tagParams = new TagParams();

            tagParams.id = Id;
            tagParams.userId = UserId;
            tagParams.category = Category;
            tagParams.name = Name;
            tagParams.description = Description;

            return tagParams;
        }
    }
}

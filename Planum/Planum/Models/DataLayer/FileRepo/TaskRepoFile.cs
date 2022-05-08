using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planum.Models.BuisnessLayer.Interfaces;
using Planum.Models.DTO.ModelData;

namespace Planum.Models.DataLayer
{
    internal class TaskRepoFile : ITaskRepo
    {
        public void Add(TaskDTO taskDTO)
        {
            throw new NotImplementedException();
        }

        public void Archive(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public TaskDTO Get(int id)
        {
            throw new NotImplementedException();
        }

        public List<TaskDTO> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Unarchive(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(TaskDTO taskDTO)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planum.Models.DTO.ModelData;

namespace Planum.Models.BuisnessLayer.Interfaces
{
    public interface ITaskRepo
    {
        public void Add(TaskDTO taskDTO);
        public void Update(TaskDTO taskDTO);
        public void Delete(int id);
        public void Archive(int id);
        public void Unarchive(int id);
        public TaskDTO Get(int id);
        public List<TaskDTO> GetAll();
        public List<TaskDTO> GetAllArchived();
        public void Reset();
    }
}

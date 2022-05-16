using System;
using System.Collections.Generic;
using Planum.Models.BuisnessLayer.RepoInterfaces;
using Planum.Models.DTO.ModelData;

namespace Planum.Models.DataLayer
{
    public class TaskRepoFile : ITaskRepo
    {
        public int AddTask(TaskDTO taskDTO)
        {
            throw new NotImplementedException();
        }

        public void ArchiveTask(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteTask(int id)
        {
            throw new NotImplementedException();
        }

        public TaskDTO? FindTask(int id)
        {
            throw new NotImplementedException();
        }

        public List<TaskDTO> GetAll()
        {
            throw new NotImplementedException();
        }

        public TaskDTO GetTask(int id)
        {
            throw new NotImplementedException();
        }

        public void UnarchiveTask(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateTask(TaskDTO taskDTO)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Planum.Models.DTO;

using System.IO;
using Planum.Models.BuisnessLogic.IRepo;
using System.Linq;
using Planum.DataModels;

namespace Planum.Models.DataModels
{
    public class TaskRepoFile : ITaskRepo
    {
        public string TASK_FILE_NAME;

        string _taskRepoPath;
        protected ITaskDTOComparator _taskDTOComparator;

        public TaskRepoFile(ITaskDTOComparator taskDTOComparator)
        {
            TASK_FILE_NAME = Config.ConfigData.LoadConfig().TaskRepoFilePath;
            _taskDTOComparator = taskDTOComparator;
            _taskRepoPath = GetSavePath(TASK_FILE_NAME);
            if (!Directory.Exists(Path.GetDirectoryName(_taskRepoPath)))
            {
                string? path = Path.GetDirectoryName(_taskRepoPath);
                if (path != null)
                    Directory.CreateDirectory(path);
            }
            if (!File.Exists(_taskRepoPath))
                using (var fs = File.Create(_taskRepoPath)) { }
        }

        protected TaskDTO ReadIntoDTO(BinaryReader reader)
        {
            int taskId = reader.ReadInt32(); // id
            int userId = reader.ReadInt32(); // user id
            string name = reader.ReadString(); // name
            string description = reader.ReadString(); // description
            int list_len = reader.ReadInt32(); // list len
            List<int> tagIds = new List<int>();
            for (int i = 0; i < list_len; i++) // list
                tagIds.Add(reader.ReadInt32());

            list_len = reader.ReadInt32(); // list len
            List<int> parentIds = new List<int>();
            for (int i = 0; i < list_len; i++) // list
                parentIds.Add(reader.ReadInt32());

            list_len = reader.ReadInt32(); // list len
            List<int> childIds = new List<int>();
            for (int i = 0; i < list_len; i++) // list
                childIds.Add(reader.ReadInt32());

            bool timed = reader.ReadBoolean(); // timed
            DateTime startTime = DateTime.Parse(reader.ReadString()); // start time
            DateTime deadline = DateTime.Parse(reader.ReadString()); // deadline
            bool isRepeated = reader.ReadBoolean(); // is repeated 
            TimeSpan repeatPeriod = TimeSpan.Parse(reader.ReadString()); // repeat period
            bool archived = reader.ReadBoolean(); // is archived

            list_len = reader.ReadInt32(); // list len
            List<int> statusQueueIds = new List<int>();
            for (int i = 0; i < list_len; i++)
                statusQueueIds.Add(reader.ReadInt32());

            int currentIndex = reader.ReadInt32();

            TaskDTO temp = new TaskDTO(taskId, startTime, deadline, repeatPeriod, tagIds, parentIds, childIds,
                name, timed, userId, description, isRepeated, archived, statusQueueIds, currentIndex);
            return temp;
        }

        protected void WriteFromDTO(TaskDTO taskDTO, int id, BinaryWriter writer)
        {
            writer.Write(id); // id
            writer.Write(taskDTO.UserId); // user id
            writer.Write(taskDTO.Name); // name
            writer.Write(taskDTO.Description); // description
            int list_len = taskDTO.TagIds.Count; // tag id list
            writer.Write(list_len); // list len
            for (int i = 0; i < list_len; i++) // list elems
                writer.Write(taskDTO.TagIds[i]);

            list_len = taskDTO.ParentIds.Count; // tag id list
            writer.Write(list_len); // list len
            for (int i = 0; i < list_len; i++) // list elems
                writer.Write(taskDTO.ParentIds[i]);

            list_len = taskDTO.ChildIds.Count; // tag id list
            writer.Write(list_len); // list len
            for (int i = 0; i < list_len; i++) // list elems
                writer.Write(taskDTO.ChildIds[i]);

            writer.Write(taskDTO.Timed); // timed
            writer.Write(taskDTO.StartTime.ToString()); // start time
            writer.Write(taskDTO.Deadline.ToString()); // end time
            writer.Write(taskDTO.IsRepeated); // is repeated
            writer.Write(taskDTO.RepeatPeriod.ToString()); // repeat period
            writer.Write(taskDTO.Archived); // archived flag

            list_len = taskDTO.StatusQueueIds.Count;
            writer.Write(list_len);
            for (int i = 0; i < list_len; i++)
                writer.Write(taskDTO.StatusQueueIds[i]);
            writer.Write(taskDTO.CurrentStatusIndex);
        }

        protected string GetSavePath(string filename)
        {
            var systemPath = Environment.
                             GetFolderPath(
                                 Environment.SpecialFolder.CommonApplicationData
                             );
            return Path.Combine(systemPath, filename);
        }

        public int AddTask(TaskDTO taskDTO)
        {
            bool alreadyExists = false;
            int id = 0;
            using (var stream = File.Open(_taskRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        TaskDTO temp = ReadIntoDTO(reader);

                        if (_taskDTOComparator.CompareDTOs(id, taskDTO, temp.Id, temp) && !temp.Archived)
                        {
                            alreadyExists = true;
                            return id;
                        }

                        if (id == temp.Id)
                            id += 1;
                    }
                }
            }

            if (!alreadyExists)
            {
                using (var stream = File.Open(_taskRepoPath, FileMode.Append))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        WriteFromDTO(taskDTO, id, writer);
                        return id;
                    }
                }
            }
            return -1;
        }

        public void DeleteTask(int id)
        {
            List<TaskDTO> tasks = new List<TaskDTO>();

            using (var stream = File.Open(_taskRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        TaskDTO temp = ReadIntoDTO(reader);

                        if (temp.Id != id)
                            tasks.Add(temp);
                    }
                }
            }

            using (var stream = File.Open(_taskRepoPath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    foreach (var taskDTO in tasks)
                    {
                        WriteFromDTO(taskDTO, taskDTO.Id, writer);
                    }
                }
            }
        }

        public TaskDTO GetTask(int id)
        {
            using (var stream = File.Open(_taskRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        TaskDTO temp = ReadIntoDTO(reader);

                        if (temp.Id == id)
                        {
                            return temp;
                        }
                    }
                }
            }

            throw new TaskDoesNotExistException("Task with id = " + id + " does not exist.");
        }

        public List<TaskDTO> GetAllTasks()
        {
            List<TaskDTO> tasks = new List<TaskDTO>();

            using (var stream = File.Open(_taskRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        TaskDTO task = ReadIntoDTO(reader);
                        tasks.Add(task);
                    }
                }
            }
            return tasks;
        }

        public void Reset()
        {
            File.Create(_taskRepoPath).Close();
        }

        public void UpdateTask(TaskDTO taskDTO)
        {
            List<TaskDTO> tasks = new List<TaskDTO>();

            using (var stream = File.Open(_taskRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        TaskDTO temp = ReadIntoDTO(reader);

                        if (temp.Id == taskDTO.Id)
                            tasks.Add(taskDTO);
                        else
                            tasks.Add(temp);
                    }
                }
            }

            using (var stream = File.Open(_taskRepoPath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    foreach (var task in tasks)
                    {
                        WriteFromDTO(task, task.Id, writer);
                    }
                }
            }
        }

        public TaskDTO? FindTask(int id)
        {
            try
            {
                return GetTask(id);
            }
            catch(TaskDoesNotExistException)
            {
                return null;
            }
        }
    }
}
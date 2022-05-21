using System;
using System.Collections.Generic;
using System.Text;
using Planum.Models.DTO;

using System.IO;
using Planum.Models.BuisnessLogic.IRepo;
using System.Linq;

namespace Planum.Models.DataModels
{
    public class TaskRepoFile : ITaskRepo
    {
        public const string TASK_FILE_NAME = "Planum\\Data\\task_data.dat";
        public const string TASK_ARCHIVE_FILE_NAME = "Planum\\Data\\task_archive_data.dat";

        string _taskRepoPath;
        string _taskArchivePath;

        public TaskRepoFile()
        {
            _taskRepoPath = GetSavePath(TASK_FILE_NAME);
            if (!Directory.Exists(Path.GetDirectoryName(_taskRepoPath)))
            {
                string? path = Path.GetDirectoryName(_taskRepoPath);
                if (path != null)
                    Directory.CreateDirectory(path);
            }
            if (!File.Exists(_taskRepoPath))
                using (var fs = File.Create(_taskRepoPath)) { }

            _taskArchivePath = GetSavePath(TASK_ARCHIVE_FILE_NAME);
            if (!Directory.Exists(Path.GetDirectoryName(_taskArchivePath)))
            {
                string? path = Path.GetDirectoryName(_taskArchivePath);
                if (path != null)
                    Directory.CreateDirectory(path);
            }
            if (!File.Exists(_taskArchivePath))
                using (var fs = File.Create(_taskArchivePath)) { }
        }

        protected bool CompareDTOs(int id_1, TaskDTO taskDTO_1, int id_2, TaskDTO taskDTO_2)
        {
            if (id_1 != id_2)
                return false;
            if (taskDTO_1.UserId != taskDTO_2.UserId)
                return false;
            if (taskDTO_1.Name != taskDTO_2.Name)
                return false;
            if (taskDTO_1.Description != taskDTO_2.Description)
                return false;
            if (taskDTO_1.Timed != taskDTO_2.Timed)
                return false;
            if (Math.Abs((taskDTO_1.StartTime - taskDTO_2.StartTime).TotalSeconds) > 1)
                return false;
            if (Math.Abs((taskDTO_1.Deadline - taskDTO_2.Deadline).TotalSeconds) > 1)
                return false;
            List<int> temp_1 = (List<int>)taskDTO_1.TagIds;
            List<int> temp_2 = (List<int>)taskDTO_2.TagIds;
            if (!temp_1.SequenceEqual(temp_2))
                return false;
            temp_1 = (List<int>)taskDTO_1.ChildIds;
            temp_2 = (List<int>)taskDTO_2.ChildIds;
            if (!temp_1.SequenceEqual(temp_2))
                return false;
            temp_1 = (List<int>)taskDTO_1.ParentIds;
            temp_2 = (List<int>)taskDTO_2.ParentIds;
            if (!temp_1.SequenceEqual(temp_2))
                return false;
            if (taskDTO_1.IsRepeated != taskDTO_2.IsRepeated)
                return false;
            if (Math.Abs((taskDTO_1.RepeatPeriod - taskDTO_2.RepeatPeriod).TotalSeconds) > 1)
                return false;
            return true;
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

            return new TaskDTO(taskId, startTime, deadline, repeatPeriod, tagIds, parentIds, childIds,
                name, timed, userId,  description, isRepeated);
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

                        if (CompareDTOs(id, taskDTO, temp.Id, temp))
                        {
                            alreadyExists = true;
                            return id;
                        }
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

        public int AddTask(TaskDTO taskDTO, int id)
        {
            bool alreadyExists = false;
            using (var stream = File.Open(_taskRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        TaskDTO temp = ReadIntoDTO(reader);

                        if (CompareDTOs(id, taskDTO, temp.Id, temp))
                        {
                            alreadyExists = true;
                            return id;
                        }
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
            throw new CantAddTaskToRepoException();
        }

        public void ArchiveTask(int id)
        {
            try
            {
                // get dto with id from general storage
                TaskDTO taskDTO = GetTask(id);
                // delete said object from general storage
                DeleteFromTaskFile(id);
                // add object to archive
                bool alreadyExists = false;
                using (var stream = File.Open(_taskArchivePath, FileMode.OpenOrCreate))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            TaskDTO temp = ReadIntoDTO(reader);

                            if (CompareDTOs(id, taskDTO, temp.Id, temp))
                            {
                                alreadyExists = true;
                                break;
                            }
                        }
                    }
                }

                if (!alreadyExists)
                {
                    using (var stream = File.Open(_taskArchivePath, FileMode.Append))
                    {
                        using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                        {
                            WriteFromDTO(taskDTO, id, writer);
                        }
                    }
                }
            }
            catch (TaskDoesNotExistException) { }
        }

        public void DeleteFromTaskFile(int id)
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

        public void DeleteTask(int id)
        {
            try
            {
                UnarchiveTask(id);
            }
            catch (ArchivedTaskDoesNotExistException)  { }

            try
            {
                DeleteFromTaskFile(id);
            }
            catch (TagDoesNotExistException) { }
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

        public TaskDTO GetTask(int id, int userId)
        {
            using (var stream = File.Open(_taskRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        TaskDTO temp = ReadIntoDTO(reader);

                        if (temp.Id == id && temp.UserId == userId)
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
                        tasks.Add(ReadIntoDTO(reader));
                    }
                }
            }
            return tasks;
        }

        public List<TaskDTO> GetAllTasks(int userId)
        {
            List<TaskDTO> tasks = new List<TaskDTO>();

            using (var stream = File.Open(_taskRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        TaskDTO temp = ReadIntoDTO(reader);
                        if (temp.UserId == userId)
                            tasks.Add(temp);
                    }
                }
            }
            return tasks;
        }

        public void Reset()
        {
            File.Create(_taskArchivePath).Close();
            File.Create(_taskRepoPath).Close();
        }

        public void UnarchiveTask(int id)
        {
            try
            {
                // Get object from archive + delete
                List<TaskDTO> tasks = new List<TaskDTO>();
                TaskDTO? archivedTask = null;

                using (var stream = File.Open(_taskArchivePath, FileMode.OpenOrCreate))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            TaskDTO temp = ReadIntoDTO(reader);

                            if (temp.Id != id)
                                tasks.Add(temp);
                            else
                                archivedTask = temp;
                        }
                    }
                }

                if (archivedTask == null)
                    throw new ArchivedTaskDoesNotExistException();
                else
                {
                    using (var stream = File.Open(_taskArchivePath, FileMode.Create))
                    {
                        using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                        {
                            foreach (var taskDTO in tasks)
                            {
                                WriteFromDTO(taskDTO, taskDTO.Id, writer);
                            }
                        }
                    }

                    // add object to general storage
                    AddTask(archivedTask, archivedTask.Id);
                }
            }
            catch (TaskDoesNotExistException) { }
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

        public List<TaskDTO> GetAllArchivedTasks()
        {
            List<TaskDTO> tasks = new List<TaskDTO>();

            using (var stream = File.Open(_taskArchivePath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        TaskDTO temp = ReadIntoDTO(reader);

                        tasks.Add(temp);
                    }
                }
            }
            return tasks;
        }

        public TaskDTO GetArchivedTask(int id)
        {
            using (var stream = File.Open(_taskArchivePath, FileMode.OpenOrCreate))
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

            throw new ArchivedTaskDoesNotExistException("Task with id = " + id + " does not exist.");
        }

        public TaskDTO GetArchivedTask(int id, int userId)
        {
            using (var stream = File.Open(_taskArchivePath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        TaskDTO temp = ReadIntoDTO(reader);

                        if (temp.Id == id && temp.UserId == userId)
                        {
                            return temp;
                        }
                    }
                }
            }

            throw new ArchivedTaskDoesNotExistException("Task with id = " + id + " does not exist.");
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

        public TaskDTO? FindTask(int id, int userId)
        {
            try
            {
                return GetTask(id, userId);
            }
            catch (TaskDoesNotExistException)
            {
                return null;
            }
        }

        public TaskDTO? FindArchivedTask(int id)
        {
            try
            {
                return GetArchivedTask(id);
            }
            catch (ArchivedTaskDoesNotExistException)
            {
                return null;
            }
        }

        public TaskDTO? FindArchivedTask(int id, int userId)
        {
            try
            {
                return GetArchivedTask(id, userId);
            }
            catch (ArchivedTaskDoesNotExistException)
            {
                return null;
            }
        }
    }
}
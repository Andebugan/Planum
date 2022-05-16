using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planum.Models.BuisnessLayer.Interfaces;
using Planum.Models.DTO.ModelData;

using System.IO;

namespace Planum.Models.DataLayer
{
    [Serializable]
    public class TaskDoesNotExist : Exception
    {
        public TaskDoesNotExist() { }
        public TaskDoesNotExist(string message) : base(message) { }
        public TaskDoesNotExist(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ArchivedTaskDoesNotExist : Exception
    {
        public ArchivedTaskDoesNotExist() { }
        public ArchivedTaskDoesNotExist(string message) : base(message) { }
        public ArchivedTaskDoesNotExist(string message, Exception innerException) : base(message, innerException) { }
    }

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

        protected TaskDTO ReadIntoDTO(BinaryReader reader)
        {
            int taskId = reader.ReadInt32(); // id
            int userId = reader.ReadInt32(); // user id
            int parentId = reader.ReadInt32(); // parent id
            string name = reader.ReadString(); // name
            string description = reader.ReadString(); // description
            int list_len = reader.ReadInt32(); // list len
            List<int> tagIds = new List<int>();
            for (int i = 0; i < list_len; i++) // list
                tagIds.Add(reader.ReadInt32());

            bool timed = reader.ReadBoolean(); // timed
            DateTime startTime = DateTime.Parse(reader.ReadString()); // start time
            DateTime deadline = DateTime.Parse(reader.ReadString()); // deadline
            bool isRepeated = reader.ReadBoolean(); // is repeated 
            TimeSpan repeatPeriod = TimeSpan.Parse(reader.ReadString()); // repeat period

            return new TaskDTO(taskId, userId, name, startTime, deadline, repeatPeriod, tagIds, timed,
                    description, parentId, isRepeated);
        }

        protected void WriteFromDTO(TaskDTO taskDTO, BinaryWriter writer)
        {
            writer.Write(taskDTO.Id); // id
            writer.Write(taskDTO.UserId); // user id
            writer.Write(taskDTO.ParentId); // parent id
            if (taskDTO.Name != null)
                writer.Write(taskDTO.Name); // name
            else
                writer.Write("");
            if (taskDTO.Description != null)
                writer.Write(taskDTO.Description); // description
            else
                writer.Write("");
            int list_len = taskDTO.TagIds.Count; // tag id list
            writer.Write(list_len); // list len
            for (int i = 0; i < list_len; i++) // list elems
                writer.Write(taskDTO.TagIds[i]);
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

        public void Add(TaskDTO taskDTO)
        {
            bool alreadyExists = false;
            using (var stream = File.Open(_taskRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        int tagId = reader.ReadInt32(); // id
                        reader.ReadInt32(); // user id
                        reader.ReadInt32(); // parent id
                        reader.ReadString(); // name
                        reader.ReadString(); // description
                        int list_len = reader.ReadInt32(); // list len
                        for (int i = 0; i < list_len; i++) // list
                            reader.ReadInt32();

                        reader.ReadBoolean(); // timed
                        reader.ReadString(); // start time
                        reader.ReadString(); // deadline
                        reader.ReadBoolean(); // is repeated 
                        reader.ReadString(); // repeat period

                        if (tagId == taskDTO.Id)
                        {
                            alreadyExists = true;
                            break;
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
                        WriteFromDTO(taskDTO, writer);
                    }
                }
            }
        }

        public void Archive(int id)
        {
            try
            {
                // get dto with id from general storage
                TaskDTO taskDTO = Get(id);
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
                            int tagId = reader.ReadInt32(); // id
                            reader.ReadInt32(); // user id
                            reader.ReadInt32(); // parent id
                            reader.ReadString(); // name
                            reader.ReadString(); // description
                            int list_len = reader.ReadInt32(); // list len
                            for (int i = 0; i < list_len; i++) // list
                                reader.ReadInt32();

                            reader.ReadBoolean(); // timed
                            reader.ReadString(); // start time
                            reader.ReadString(); // deadline
                            reader.ReadBoolean(); // is repeated 
                            reader.ReadString(); // repeat period

                            if (tagId == taskDTO.Id)
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
                            WriteFromDTO(taskDTO, writer);
                        }
                    }
                }
            }
            catch (TaskDoesNotExist) { }
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
                        WriteFromDTO(taskDTO, writer);
                    }
                }
            }
        }

        public void Delete(int id)
        {
            try
            {
                Unarchive(id);
            }
            catch (ArchivedTaskDoesNotExist)
            {
                try
                {
                    DeleteFromTaskFile(id);
                }
                catch (TagDoesNotExist) { }
            }

            try
            {
                DeleteFromTaskFile(id);
            }
            catch (TagDoesNotExist)
            {
                try
                {
                    Unarchive(id);
                }
                catch (ArchivedTaskDoesNotExist) { }
            }
        }

        public TaskDTO Get(int id)
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

            throw new TaskDoesNotExist("Task with id = " + id + " does not exist.");
        }

        public List<TaskDTO> GetAll()
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

        public void Reset()
        {
            File.Create(_taskArchivePath).Close();
            File.Create(_taskRepoPath).Close();
        }

        public void Unarchive(int id)
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
                    throw new ArchivedTaskDoesNotExist();
                else
                {
                    using (var stream = File.Open(_taskArchivePath, FileMode.Create))
                    {
                        using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                        {
                            foreach (var taskDTO in tasks)
                            {
                                WriteFromDTO(taskDTO, writer);
                            }
                        }
                    }

                    // add object to general storage
                    Add(archivedTask);
                }
            }
            catch (TaskDoesNotExist) { }
        }

        public void Update(TaskDTO taskDTO)
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
                        WriteFromDTO(task, writer);
                    }
                }
            }
        }

        public List<TaskDTO> GetAllArchived()
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
    }
}
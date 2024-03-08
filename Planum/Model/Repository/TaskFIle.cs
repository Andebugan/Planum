using Planum.Model.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Planum.Model.Repository
{
    public class TaskFile
    {
        public int Id { get; set; }
        public string Path { get; set; } = string.Empty;
        public int CurrentTaskCount { get; set; } = 0;
        public int MaxTaskCount { get; set; }
        public int FreeIdsCount
        {
            get { return MaxTaskCount - CurrentTaskCount; }
        }

        public int StartId
        {
            get { return Id * MaxTaskCount; }

        }
        public int EndId
        {
            get { return (Id + 1) * MaxTaskCount; }
        }

        // Read & Write
        protected Task ReadIntoObject(BinaryReader reader)
        {
            int taskId = reader.ReadInt32(); // id
            string name = reader.ReadString(); // name
            string description = reader.ReadString(); // description

            int list_len = reader.ReadInt32(); // list len
            List<int> parentIds = new List<int>();
            for (int i = 0; i < list_len; i++) // list
                parentIds.Add(reader.ReadInt32());

            list_len = reader.ReadInt32(); // list len
            List<int> childIds = new List<int>();
            for (int i = 0; i < list_len; i++) // list
                childIds.Add(reader.ReadInt32());

            list_len = reader.ReadInt32(); // list len
            Checklist checklist = new Checklist();
            for (int i = 0; i < list_len; i++)
            {
                string temp = reader.ReadString();
                bool done = reader.ReadBoolean();
                checklist.AddItem(temp, done);
            }
            checklist.ResetOnComplete = reader.ReadBoolean(); // reset on complete

            bool archived = reader.ReadBoolean(); // is archived

            bool timeEnabled = reader.ReadBoolean();
            DateTime start = DateTime.Parse(reader.ReadString()); // start
            DateTime deadline = DateTime.Parse(reader.ReadString()); // deadline
            if (deadline.ToString() == DateTime.MaxValue.ToString())
                deadline = DateTime.MaxValue;

            bool repeatEnabled = reader.ReadBoolean(); // is repeated
            bool autorepeatEnabled = reader.ReadBoolean(); // is autorepeated

            int years = reader.ReadInt32();
            int months = reader.ReadInt32();
            TimeSpan custom = TimeSpan.Parse(reader.ReadString()); // repeat period

            RepeatParams repeatParams = new RepeatParams(repeatEnabled, autorepeatEnabled, years, months, custom);
            TimeParams timeParams = new TimeParams(timeEnabled, start, deadline, repeatParams);
            return new Task(taskId, name, description, parentIds, childIds, timeParams, archived, checklist);
        }

        protected void WriteFromObject(BinaryWriter writer, Task obj)
        {
            writer.Write(obj.Id);

            writer.Write(obj.Name); // name
            writer.Write(obj.Description); // description

            int list_len = obj.ParentIds.Count; // parent id list
            writer.Write(list_len); // list len
            for (int i = 0; i < list_len; i++) // list elems
                writer.Write(obj.ParentIds[i]);

            list_len = obj.ChildIds.Count; // child id list
            writer.Write(list_len); // list len
            for (int i = 0; i < list_len; i++) // list elems
                writer.Write(obj.ChildIds[i]);

            list_len = obj.checklist.items.Count;
            writer.Write(list_len);
            for (int i = 0; i < list_len; i++)
            {
                writer.Write(obj.checklist.items[i].description);
                writer.Write(obj.checklist.items[i].done);
            }
            writer.Write(obj.checklist.ResetOnComplete);

            writer.Write(obj.Archived);

            writer.Write(obj.Timed());
            writer.Write(obj.TimeParams.Start.ToString());
            writer.Write(obj.TimeParams.Deadline.ToString());

            writer.Write(obj.Repeated());
            writer.Write(obj.Autorepeated());
            writer.Write(obj.TimeParams.repeat.years);
            writer.Write(obj.TimeParams.repeat.months);
            writer.Write(obj.TimeParams.repeat.custom.ToString());
        }

        public int GetFreeId()
        {
            int taskId = Id * MaxTaskCount;

            List<int> ids = Read().Select(x => x.Id).ToList();
            ids.Sort();

            foreach (var id in ids)
            {
                if (taskId == id)
                    taskId += 1;
                else
                    break;
            }

            return taskId;
        }

        public List<Task> Read()
        {
            List<Task> tasks = new List<Task>();

            using (var reader = new BinaryReader(new FileStream(Path, FileMode.OpenOrCreate)))
            {
                while (reader.PeekChar() > -1)
                {
                    tasks.Add(ReadIntoObject(reader));
                }
            }

            CurrentTaskCount = tasks.Count;
            return tasks;
        }

        public List<Task> ReadWithIds(List<int> id)
        {
            return Read().Where(x => id.Contains(x.Id)).ToList();
        }

        public List<int> RemoveFromQueue(List<int> taskIds)
        {
            List<int> fileIds = taskIds.Where(x => x >= StartId && x < EndId).ToList();
            List<int> remaining = taskIds.Except(fileIds).ToList();

            if (fileIds.Count == 0)
                return taskIds;

            List<Task> fileTasks = Read().Where(x => !fileIds.Contains(x.Id)).ToList();

            using (var writer = new BinaryWriter(new FileStream(Path, FileMode.Truncate)))
            {
                foreach (var task in fileTasks.OrderBy(x => x.Id).ToList())
                {
                    WriteFromObject(writer, task);
                }
            }

            CurrentTaskCount = fileTasks.Count;

            return remaining;
        }

        public List<Task> WriteFromQueue(List<Task> tasks, bool add = false)
        {
            List<Task> fileTasks = tasks.Where(x => x.Id >= StartId && x.Id < EndId).ToList();
            List<Task> remaining = tasks.Except(fileTasks).ToList();

            if (fileTasks.Count == 0)
                return tasks;

            if (add)
                fileTasks = fileTasks.Concat(Read().Where(x => !fileTasks.Exists(y => x.Id == y.Id))).ToList();

            FileMode fileMode = FileMode.Truncate;
            if (add || !File.Exists(Path))
                fileMode = FileMode.OpenOrCreate;

            using (var writer = new BinaryWriter(new FileStream(Path, fileMode)))
            {
                foreach (var task in fileTasks.OrderBy(x => x.Id).ToList())
                {
                    WriteFromObject(writer, task);
                }
            }

            CurrentTaskCount = fileTasks.Count;

            return remaining;
        }
    }
}

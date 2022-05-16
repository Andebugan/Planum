using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planum.Models.BuisnessLayer.Interfaces;
using Planum.Models.DTO.ModelData;

using System.IO;
using System.Diagnostics;

namespace Planum.Models.DataLayer
{
    [Serializable]
    public class TagDoesNotExist : Exception
    {
        public TagDoesNotExist() { }
        public TagDoesNotExist(string message) : base(message) { }
        public TagDoesNotExist(string message, Exception innerException) : base(message, innerException) { }
    }

    public class TagRepoFile : ITagRepo
    {
        public const string TAG_FILE_NAME = "Planum\\Data\\tag_data.dat";
        protected string _tagRepoPath;

        public TagRepoFile()
        {
            _tagRepoPath = GetSavePath();
            if (!Directory.Exists(Path.GetDirectoryName(_tagRepoPath)))
            {
                string? path = Path.GetDirectoryName(_tagRepoPath);
                if (path != null)
                    Directory.CreateDirectory(path);
            }
            if (!File.Exists(_tagRepoPath))
                using (var fs = File.Create(_tagRepoPath)) { }
        }

        protected string GetSavePath()
        {
            var systemPath = Environment.
                             GetFolderPath(
                                 Environment.SpecialFolder.CommonApplicationData
                             );
            return Path.Combine(systemPath, TAG_FILE_NAME);
        }

        public void Add(TagDTO tag)
        {
            bool alreadyExists = false;
            using (var stream = File.Open(_tagRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        int tagId = reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadString();
                        reader.ReadString();

                        if (tagId == tag.Id)
                        {
                            alreadyExists = true;
                            break;
                        }
                    }
                }
            }

            if (!alreadyExists)
            {
                using (var stream = File.Open(_tagRepoPath, FileMode.Append))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        writer.Write(tag.Id);
                        writer.Write(tag.UserId);
                        writer.Write(tag.Category);
                        if (tag.Name != null)
                            writer.Write(tag.Name);
                        else
                            writer.Write("");
                        if (tag.Description != null)
                            writer.Write(tag.Description);
                        else
                            writer.Write("");
                    }
                }
            }
        }

        public void Delete(int id)
        {
            List<TagDTO> tags = new List<TagDTO>();

            using (var stream = File.Open(_tagRepoPath, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        int tagId = reader.ReadInt32();
                        int userId = reader.ReadInt32();
                        int category = reader.ReadInt32();
                        string name = reader.ReadString();
                        string description = reader.ReadString();

                        if (tagId != id)
                            tags.Add(new TagDTO(tagId, userId, category, name, description));
                    }
                }
            }

            using (var stream = File.Open(_tagRepoPath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    foreach (var tagDTO in tags)
                    {
                        writer.Write(tagDTO.Id);
                        writer.Write(tagDTO.UserId);
                        writer.Write(tagDTO.Category);
                        if (tagDTO.Name != null)
                            writer.Write(tagDTO.Name);
                        else
                            writer.Write("");
                        if (tagDTO.Description != null)
                            writer.Write(tagDTO.Description);
                        else
                            writer.Write("");
                    }
                }
            }
        }

        public TagDTO Get(int id)
        {
            using (var stream = File.Open(_tagRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        int tagId = reader.ReadInt32();
                        int tagUserId = reader.ReadInt32();
                        int category = reader.ReadInt32();
                        string name = reader.ReadString();
                        string description = reader.ReadString();

                        if (tagId == id)
                        {
                            return new TagDTO(tagId, tagUserId, category, name, description);
                        }
                    }
                }
            }

            throw new TagDoesNotExist("Tag with id = " + id + " does not exist.");
        }

        public List<TagDTO> GetAll()
        {
            List<TagDTO> tags = new List<TagDTO>();

            if (File.Exists(_tagRepoPath))
            {
                using (var stream = File.Open(_tagRepoPath, FileMode.OpenOrCreate))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            int id = reader.ReadInt32();
                            int userId = reader.ReadInt32();
                            int category = reader.ReadInt32();
                            string name = reader.ReadString();
                            string description = reader.ReadString();

                            tags.Add(new TagDTO(id, userId, category, name, description));
                        }
                    }
                }
            }
            return tags;
        }

        public void Reset()
        {
            File.Create(_tagRepoPath).Close();
        }

        public void Update(TagDTO tag)
        {
            List<TagDTO> tags = new List<TagDTO>();

            if (File.Exists(_tagRepoPath))
            {
                using (var stream = File.Open(_tagRepoPath, FileMode.OpenOrCreate))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            int id = reader.ReadInt32();
                            int userId = reader.ReadInt32();
                            int category = reader.ReadInt32();
                            string name = reader.ReadString();
                            string description = reader.ReadString();

                            if (id == tag.Id)
                                tags.Add(tag);
                            else
                                tags.Add(new TagDTO(id, userId, category, name, description));
                        }
                    }
                }

                using (var stream = File.Open(_tagRepoPath, FileMode.Create))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        foreach (var tagDTO in tags)
                        {
                            writer.Write(tagDTO.Id);
                            writer.Write(tagDTO.UserId);
                            writer.Write(tagDTO.Category);
                            if (tagDTO.Name != null)
                                writer.Write(tagDTO.Name);
                            else
                                writer.Write("");
                            if (tagDTO.Description != null)
                                writer.Write(tagDTO.Description);
                            else
                                writer.Write("");
                        }
                    }
                }
            }
        }
    }
}
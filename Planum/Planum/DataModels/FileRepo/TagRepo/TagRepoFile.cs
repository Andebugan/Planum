using System;
using System.Collections.Generic;
using System.Text;
using Planum.Models.DTO;

using System.IO;
using Planum.Models.BuisnessLogic.IRepo;
using Planum.DataModels;

namespace Planum.Models.DataModels
{
    public class TagRepoFile : ITagRepo
    {
        public string TAG_FILE_NAME;
        protected string _tagRepoPath;
        protected ITagDTOComparator _tagDTOComparator;

        public TagRepoFile(ITagDTOComparator tagDTOComparator)
        {
            TAG_FILE_NAME = Config.ConfigData.LoadConfig().TagRepoFilePath;
            _tagDTOComparator = tagDTOComparator;
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

        protected TagDTO ReadIntoDTO(BinaryReader reader)
        {
            int tagId = reader.ReadInt32();
            int userId = reader.ReadInt32();
            int category = reader.ReadInt32();
            string name = reader.ReadString();
            string description = reader.ReadString();

            return new TagDTO(tagId, userId, category, name, description);
        }

        protected void WriteFromDTO(BinaryWriter writer, int id, TagDTO tagDTO)
        {
            writer.Write(id);
            writer.Write(tagDTO.UserId);
            writer.Write(tagDTO.Category);
            writer.Write(tagDTO.Name);
            writer.Write(tagDTO.Description);
        }

        public int AddTag(TagDTO tag)
        {
            bool alreadyExists = false;
            int id = 0;
            using (var stream = File.Open(_tagRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        TagDTO temp = ReadIntoDTO(reader);

                        if (_tagDTOComparator.CompareDTOs(temp.Id, temp, id, tag))
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
                using (var stream = File.Open(_tagRepoPath, FileMode.Append))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        WriteFromDTO(writer, id, tag);
                    }
                }
            }
            return id;
        }

        public void DeleteTag(int id)
        {
            List<TagDTO> tags = new List<TagDTO>();

            using (var stream = File.Open(_tagRepoPath, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        TagDTO temp = ReadIntoDTO(reader);

                        if (temp.Id != id)
                            tags.Add(temp);
                    }
                }
            }

            using (var stream = File.Open(_tagRepoPath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    foreach (var tagDTO in tags)
                    {
                        WriteFromDTO(writer, tagDTO.Id, tagDTO);
                    }
                }
            }
        }

        public TagDTO GetTag(int id)
        {
            using (var stream = File.Open(_tagRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        TagDTO temp = ReadIntoDTO(reader);

                        if (temp.Id == id)
                        {
                            return temp;
                        }
                    }
                }
            }

            throw new TagDoesNotExistException("Tag with id = " + id + " does not exist.");
        }

        public List<TagDTO> GetAllTags()
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
                            TagDTO temp = ReadIntoDTO(reader);

                            tags.Add(temp);
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

        public void UpdateTag(TagDTO tag)
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
                            TagDTO temp = ReadIntoDTO(reader);

                            if (temp.Id == tag.Id)
                                tags.Add(tag);
                            else
                                tags.Add(temp);
                        }
                    }
                }

                using (var stream = File.Open(_tagRepoPath, FileMode.Create))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        foreach (var tagDTO in tags)
                        {
                            WriteFromDTO(writer, tagDTO.Id, tagDTO);
                        }
                    }
                }
            }
        }

        public TagDTO? FindTag(int id)
        {
            try
            {
                TagDTO tagDTO = GetTag(id);
                return tagDTO;
            }
            catch (TagDoesNotExistException)
            {
                return null;
            }
        }
    }
}
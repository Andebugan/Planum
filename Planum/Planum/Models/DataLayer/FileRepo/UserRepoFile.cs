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
    public class UserDoesNotExist : Exception
    {
        public UserDoesNotExist() { }
        public UserDoesNotExist(string message) : base(message) { }
        public UserDoesNotExist(string message, Exception innerException) : base(message, innerException) { }
    }

    public class UserRepoFile : IUserRepo
    {
        public const string USER_FILE_NAME = "Planum\\Data\\user_data.dat";
        protected string _userRepoPath;

        public UserRepoFile()
        {
            _userRepoPath = GetSavePath();
            Debug.WriteLine(Path.GetDirectoryName(_userRepoPath));
            if (!Directory.Exists(Path.GetDirectoryName(_userRepoPath)))
            {
                string? path = Path.GetDirectoryName(_userRepoPath);
                if (path != null)
                    Directory.CreateDirectory(path);
            }
            if (!File.Exists(_userRepoPath))
                using (var fs = File.Create(_userRepoPath)) { }
        }

        protected string GetSavePath()
        {
            var systemPath = Environment.
                             GetFolderPath(
                                 Environment.SpecialFolder.CommonApplicationData
                             );
            return Path.Combine(systemPath, USER_FILE_NAME);
        }

        public void Add(UserDTO userDTO)
        {
            bool alreadyExists = false;
            using (var stream = File.Open(_userRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        int userId = reader.ReadInt32();
                        reader.ReadString();
                        reader.ReadString();

                        if (userId == userDTO.Id)
                        {
                            alreadyExists = true;
                            break;
                        }
                    }
                }
            }

            if (!alreadyExists)
            {
                using (var stream = File.Open(_userRepoPath, FileMode.Append))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        writer.Write(userDTO.Id);
                        if (userDTO.Login != null)
                            writer.Write(userDTO.Login);
                        else
                            writer.Write("");
                        if (userDTO.Password != null)
                            writer.Write(userDTO.Password);
                        else
                            writer.Write("");
                    }
                }
            }
        }

        public void Delete(int id)
        {
            List<UserDTO> users = new List<UserDTO>();

            using (var stream = File.Open(_userRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        int read_id = reader.ReadInt32();
                        string login = reader.ReadString();
                        string password = reader.ReadString();

                        if (read_id != id)
                            users.Add(new UserDTO(read_id, login, password));
                    }
                }
            }

            using (var stream = File.Open(_userRepoPath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    foreach (var userDTO in users)
                    {
                        writer.Write(userDTO.Id);
                        if (userDTO.Login != null)
                            writer.Write(userDTO.Login);
                        else
                            writer.Write("");
                        if (userDTO.Password != null)
                            writer.Write(userDTO.Password);
                        else
                            writer.Write("");
                    }
                }
            }
        }

        public UserDTO Get(int id)
        {
            using (var stream = File.Open(_userRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        int userId = reader.ReadInt32();
                        string login = reader.ReadString();
                        string password = reader.ReadString();

                        if (userId == id)
                        {
                            return new UserDTO(id, login, password);
                        }
                    }
                }
            }

            throw new UserDoesNotExist("User with id = " + id + " does not exist.");
        }

        public List<UserDTO> GetAll()
        {
            List<UserDTO> users = new List<UserDTO>();

            if (File.Exists(_userRepoPath))
            {
                using (var stream = File.Open(_userRepoPath, FileMode.OpenOrCreate))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            int id = reader.ReadInt32();
                            string login = reader.ReadString();
                            string password = reader.ReadString();

                            users.Add(new UserDTO(id, login, password));
                        }
                    }
                }
            }
            return users;
        }

        public void Reset()
        {
            File.Create(_userRepoPath).Close();
        }

        public void Update(UserDTO userDTO)
        {
            List<UserDTO> users = new List<UserDTO>();

            if (File.Exists(_userRepoPath))
            {
                using (var stream = File.Open(_userRepoPath, FileMode.OpenOrCreate))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            int id = reader.ReadInt32();
                            string login = reader.ReadString();
                            string password = reader.ReadString();

                            if (id == userDTO.Id)
                                users.Add(userDTO);
                            else
                                users.Add(new UserDTO(id, login, password));
                        }
                    }
                }

                using (var stream = File.Open(_userRepoPath, FileMode.Create))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        foreach (var user in users)
                        {
                            writer.Write(userDTO.Id);
                            if (userDTO.Login != null)
                                writer.Write(userDTO.Login);
                            else
                                writer.Write("");
                            if (userDTO.Password != null)
                                writer.Write(userDTO.Password);
                            else
                                writer.Write("");
                        }
                    }
                }
            }
        }
    }
}
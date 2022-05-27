using System;
using System.Collections.Generic;
using System.Text;
using Planum.Models.DTO;

using System.IO;
using System.Diagnostics;
using Planum.Models.BuisnessLogic.IRepo;
namespace Planum.Models.DataModels
{
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

        protected UserDTO ReadIntoDTO(BinaryReader reader)
        {
            int read_id = reader.ReadInt32();
            string login = reader.ReadString();
            string password = reader.ReadString();
            
            return new UserDTO(read_id, login, password);
        }

        protected void WriteFromDTO(BinaryWriter writer, int id, UserDTO userDTO)
        {
            writer.Write(id);
            writer.Write(userDTO.Login);
            writer.Write(userDTO.Password);
        }

        protected bool CompareDTOs(int id_1, UserDTO userDTO_1, int id_2, UserDTO userDTO_2)
        {
            if (id_1 != id_2)
                return false;
            if (userDTO_1.Login != userDTO_2.Login)
                return false;
            if (userDTO_1.Password != userDTO_2.Password)
                return false;
            return true;
        }

        public int AddUser(UserDTO userDTO)
        {
            bool alreadyExists = false;
            int id = 0;
            using (var stream = File.Open(_userRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        UserDTO temp = ReadIntoDTO(reader);
                             
                        if (CompareDTOs(id, userDTO, temp.Id, temp))
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
                using (var stream = File.Open(_userRepoPath, FileMode.Append))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        WriteFromDTO(writer, id, userDTO);
                        return id;
                    }
                }
            }
            throw new CantAddUserToRepoException();
        }

        public void DeleteUser(int id)
        {
            List<UserDTO> users = new List<UserDTO>();

            using (var stream = File.Open(_userRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        UserDTO temp = ReadIntoDTO(reader);

                        if (temp.Id != id)
                            users.Add(temp);
                    }
                }
            }

            using (var stream = File.Open(_userRepoPath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    foreach (var userDTO in users)
                    {
                        WriteFromDTO(writer, userDTO.Id, userDTO);
                    }
                }
            }
        }

        public UserDTO GetUser(int id)
        {
            using (var stream = File.Open(_userRepoPath, FileMode.OpenOrCreate))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        UserDTO temp = ReadIntoDTO(reader);

                        if (temp.Id == id)
                        {
                            return temp;
                        }
                    }
                }
            }

            throw new UserDoesNotExist($"User with id = {id} does not exist.");
        }

        public List<UserDTO> GetAllUsers()
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
                            users.Add(ReadIntoDTO(reader));
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

        public void UpdateUser(UserDTO userDTO)
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
                            UserDTO temp = ReadIntoDTO(reader);

                            if (temp.Id == userDTO.Id)
                                users.Add(userDTO);
                            else
                                users.Add(temp);
                        }
                    }
                }

                using (var stream = File.Open(_userRepoPath, FileMode.Create))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        foreach (var user in users)
                        {
                            WriteFromDTO(writer, user.Id, userDTO);
                        }
                    }
                }
            }
        }

        public UserDTO? FindUser(int id)
        {
            try
            {
                return GetUser(id);
            }
            catch (UserDoesNotExist) { return null; }
        }
    }
}
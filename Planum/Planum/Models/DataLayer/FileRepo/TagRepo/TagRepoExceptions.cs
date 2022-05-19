using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.Models.DataLayer
{
    [Serializable]
    public class TagDoesNotExistException : Exception
    {
        public TagDoesNotExistException() { }
        public TagDoesNotExistException(string message) : base(message) { }
        public TagDoesNotExistException(string message, Exception innerException) : base(message, innerException) { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    //thrown exception when the stock is exhausted
    public class RunOutException : Exception
    {
        public RunOutException() { }
        public RunOutException(string message) : base(message) { }
        public RunOutException(string message, Exception inner) : base(message, inner) { }
    }

    //thrown exception when we try to add a book with the same ISBN than a book present in the collection
    public class DoubleISBNException : Exception
    {
        public DoubleISBNException() { }
        public DoubleISBNException(string message) : base(message) { }
        public DoubleISBNException(string message, Exception inner) : base(message, inner) { }
    }

    //thrown exception when we try to add a user with the same name than a book present in the collection
    public class DoubleUserException : Exception
    {
        public DoubleUserException() { }
        public DoubleUserException(string message) : base(message) { }
        public DoubleUserException(string message, Exception inner) : base(message, inner) { }
    }
}

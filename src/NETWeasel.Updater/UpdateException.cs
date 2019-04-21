using System;

namespace NETWeasel.Updater
{
    public class UpdateException : Exception
    {
        internal UpdateException(string message) : base(message)
        {
            
        }
    }
}

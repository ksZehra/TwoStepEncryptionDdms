using System;

namespace TwoStepEncryptionDdms.Security
{
    public class UserException : Exception
    {
        public UserException(string message) : base(message)
        {
        }
    }
}

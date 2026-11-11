using System;

namespace TwoStepEncryptionDdms.Security
{
    [Flags]
    public enum Permission
    {
        None = 0,
        List = 1,
        New = 2,
        Edit = 4,
        View = 8,
        Delete = 16
    }
}

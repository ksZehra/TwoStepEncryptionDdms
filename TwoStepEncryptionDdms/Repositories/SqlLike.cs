namespace TwoStepEncryptionDdms.Repositories
{
    internal static class SqlLike
    {
        public static string Escape(string text)
        {
            return text.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_").Replace("[", "\\[");
        }
    }
}

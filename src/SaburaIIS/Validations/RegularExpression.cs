namespace SaburaIIS.Validations
{
    public class RegularExpression
    {
        public const string PackageName = @"[a-zA-Z0-9\._-]{0,64}";
        public const string ReleaseVersion = @"[a-zA-Z0-9\._-]{0,32}";
        public const string ReleaseUrl = @"https://.+/.*\.zip(\?.*)?$";
    }
}

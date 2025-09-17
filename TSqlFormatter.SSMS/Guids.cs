using System;

namespace TSqlFormatter.SSMS
{
    internal static class Guids
    {
        public const string PackageGuidString = "c47a9b21-2692-47d6-972a-976544685f0f";
        public const string CommandSetGuidString = "6fa2e413-8351-4ca9-b0a0-34a9b241648c";

        public static readonly Guid PackageGuid = new Guid(PackageGuidString);
        public static readonly Guid CommandSetGuid = new Guid(CommandSetGuidString);
    }
}
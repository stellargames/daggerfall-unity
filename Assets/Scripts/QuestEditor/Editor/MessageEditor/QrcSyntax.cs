namespace QuestEditor.Editor.MessageEditor
{
    public static class QrcSyntax
    {
        public const string variable = @"%[a-zA-Z0-9]+\b";
        public const string centered = @"<ce>";
        public const string symbol = @"[^_]?_[a-z]\w+_";
        public const string location = @"[^_]?__[a-z]\w+_";
        public const string placeName = @"[^_]?___[a-z]\w+_";
        public const string timeClassName = @"[^=]?=[a-z]\w+_";
        public const string faction = @"[^=]?==[a-z]\w+_";
    }
}
namespace GRPP;

using System.Collections.Generic;
using System.Linq;
using RueI.API.Elements;

public static class Defaults
{
    public const string Site = "22";
    public const int WebhookLobbyColor = 1752220;
    public const int WebhookRPColor = 7419530; // can use hex literals for these btw, it's just being sent in to discord embeds !
    public const string WebhookRPLobbyName = "LobbyBot";
    public const string WebhookRPStartName = "RoleplayBot";
    public const string WebhookRPStartMsg = "A roleplay has been started!";
    public const bool WebhookRPInLine = true;
    public const bool WebhookRPTimeStamps = true;
    public const string WebhookRPExtraArgTitle = "";
    public const string WebhookRPExtraArgDesc = "";
    public const int NameMaxLength = 25;
    public const int InfoMaxLength = 50;
    public const float MaxHeight = 1.1f;
    public const float MinHeight = 0.9f;
    public const int MaximumCreateDescription = 50;
    
    public static class Tagging
    {
        public static List<Tag>.Enumerator GetEnumerator() => GetAllTags().GetEnumerator();
        public static void Clear() => _cachedList = null;
        public static List<Tag> All => GetAllTags();
        private static List<Tag>? _cachedList;
        
        private static List<Tag> GetAllTags()
        {
            if (_cachedList != null)
                return _cachedList;
            
            _cachedList = [];
            
            var fields = typeof(Tagging).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            
            foreach (var field in fields.Where(fld => fld.FieldType == typeof(Tag) && fld.IsInitOnly))
                _cachedList.Add((Tag)field.GetValue(null));
            
            return _cachedList;
        }
        
        // All tags below
        
        public static readonly Tag ErrorTag = new("ErrorTag");
        public static readonly Tag WarningTag = new("WarningTag");
        public static readonly Tag InfoTag = new("InfoTag");
        public static readonly Tag SuccessTag = new("SuccessTag");
        public static readonly Tag CustomItemTag = new("CustomItemTag");
        
    }
}
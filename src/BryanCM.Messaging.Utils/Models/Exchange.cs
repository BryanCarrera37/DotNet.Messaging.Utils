using BryanCM.Messaging.Utils.Enums;
using BryanCM.AspNet.Utils.Helpers;

namespace BryanCM.Messaging.Utils.Models
{
    public class Exchange
    {
        public string Name { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public string For {
            get;
            init
            {
                ResourceType = EnumHelper.GetFromString<ResourceType>(value);
            }
        } = string.Empty;
        public ResourceType ResourceType { get; private set; }
    }
}

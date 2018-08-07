using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JABot.Enums
{
    public class CustomRoles
    {
        public enum CustomRole
        {
            User = 0,
            Moderator = 1
        };

        public static string ToFriendlyString(CustomRole role)
        {
            switch (role)
            {
                case CustomRole.User:
                    return "@everyone";
                case CustomRole.Moderator:
                    return "Moderator";
            }
            return "";
        }
    }
}
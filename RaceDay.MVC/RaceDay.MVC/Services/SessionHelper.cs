using RaceDay.MVC.Models;
using System.Text.Json;

namespace RaceDay.MVC.Services
{
    public static class SessionHelper
    {
        private const string UserKey = "CurrentUser";

        public static void SetUser(ISession session, UserSession user)
        {
            session.SetString(UserKey, JsonSerializer.Serialize(user));
        }

        public static UserSession? GetUser(ISession session)
        {
            var value = session.GetString(UserKey);
            if (value == null) return null;
            return JsonSerializer.Deserialize<UserSession>(value);
        }

        public static void ClearUser(ISession session)
        {
            session.Remove(UserKey);
        }

        public static bool IsLoggedIn(ISession session)
        {
            return GetUser(session) != null;
        }

        public static bool IsOrganiser(ISession session)
        {
            return GetUser(session)?.Role == "Organiser";
        }

        public static bool IsParticipant(ISession session)
        {
            return GetUser(session)?.Role == "Participant";
        }
    }
}

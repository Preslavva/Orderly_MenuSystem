using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace MainOrderly.WebApp.Extensions
{
    public static class SessionExtensions
    {
        private const string AuthUserKey = "AuthenticatedUser";

        public static void SetAuthenticatedUser(this ISession session, AuthenticatedUser user)
        {
            if (user == null)
            {
                session.Remove(AuthUserKey);
                return;
            }

            
            string userJson = JsonSerializer.Serialize(user);
            session.SetString(AuthUserKey, userJson);
        }

        public static AuthenticatedUser GetAuthenticatedUser(this ISession session)
        {
            string userJson = session.GetString(AuthUserKey);
            if (string.IsNullOrEmpty(userJson))
                return null;

            return JsonSerializer.Deserialize<AuthenticatedUser>(userJson);
        }

        public static bool IsAuthenticated(this ISession session)
        {
            return session.GetAuthenticatedUser() != null;
        }

        public static void ClearAuthenticatedUser(this ISession session)
        {
            session.Remove(AuthUserKey);
        }
    }
}
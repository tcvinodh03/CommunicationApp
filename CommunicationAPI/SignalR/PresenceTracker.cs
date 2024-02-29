using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace CommunicationAPI.SignalR
{
    public class PresenceTracker
    {

        private static readonly Dictionary<string, List<string>> OnlineUser = new Dictionary<string, List<string>>();

        public Task<bool> UserConnected(string userName, string connectionId)
        {
            
            lock (OnlineUser)
            {
                if (OnlineUser.ContainsKey(userName)) OnlineUser[userName].Add(connectionId);
                else
                {
                    OnlineUser.Add(userName, new List<string> { connectionId });
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }


        public Task<bool> UserDisconnected(string userName, string connectionId)
        {
            lock (OnlineUser)
            {
                if (!OnlineUser.ContainsKey(userName)) { return Task.FromResult(false); };

                OnlineUser[userName].Remove(connectionId);

                if (OnlineUser[userName].Count <= 0) {OnlineUser.Remove(userName); return Task.FromResult(true); };

            }
            return Task.FromResult(false);
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] allUser;
            lock (OnlineUser)
            {
                allUser = OnlineUser.OrderBy(k => k.Key).Select(k => k.Key).ToArray();               
            }
            return Task.FromResult(allUser);

        }

        public static Task<List<string>> GetconnectionsForUser(string userName)
        {
            List<string> connectionIds;
            lock (OnlineUser)
            {
                connectionIds=OnlineUser.GetValueOrDefault(userName);

            }
            return Task.FromResult(connectionIds);
        }
    }
}

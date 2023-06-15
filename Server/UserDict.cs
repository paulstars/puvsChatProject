
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Server
{
    /// <summary>
    /// A List including the Names of all active Users and their Client Key.
    /// </summary>
    public class UserDict
    {
        private Dictionary<string, string> users = new Dictionary<string, string>();

        public bool DoesUserExist(string user)
        {
            return this.users.ContainsValue(user);
        }

        public bool NewUser(string key, string user){
            if (!this.users.ContainsKey(key) && !this.users.ContainsValue(user))
            {
                this.users.TryAdd(key, user);
                Console.WriteLine($"Client '{user}' registered!");
                return true;
            }
            
            Console.WriteLine($"Client '{user}' already registered!");
            return false;
        }

        public string GetAllUser()
        {
            return null;
        }

        public void RemoveUser(string key){
            Console.WriteLine($"TryDelete User: '{key}' !");
            
            if (!this.users.ContainsKey(key))
            {
                Console.WriteLine($"Der User mit dem Key '{key}' existiert nicht!");
            }else
            {
                this.users.Remove(key);
                Console.WriteLine($"Deleted User: '{key}' !");
            }
        }

        public void ClearUsers(){
            this.users.Clear();
            Console.WriteLine($"Cleared all Users!");
        }
    }
}

using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Server
{
    /// <summary>
    /// A List including the Names of all active Users and their Client Key.
    /// </summary>
    public class UserDict
    {
        private Dictionary<string, string> users = new Dictionary<string, string>();

        public bool User(string key, string user)
        {
            if (this.users.ContainsValue(user))
            {
                if (this.users.ContainsKey(key))
                {
                    this.users.TryGetValue(key, out string? name);
                    if (name == user)
                    {
                        Console.WriteLine($"Client '{user}' verified!");
                        return true;
                    }
                    Console.WriteLine($"Client '{user}' already registered!");
                    return false;
                }
            }
            if (this.users.ContainsKey(key))
            {
                
            }
            if (key == "-1")
            {
                return NewUser(user);
            }
        }

        public bool NewUser(string user){ 
            var key = Convert.ToString(Guid.NewGuid());
            
            if (!this.users.ContainsValue(user))
            {
                this.users.TryAdd(key, user);
                Console.WriteLine($"Client '{user}' registered!");
                return true;
            }
            
            
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
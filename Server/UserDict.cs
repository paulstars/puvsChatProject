
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Server
{
    /// <summary>
    /// A List including the Names of all active Users and their Client Key.
    /// </summary>
    public class UserDict
    {
        private readonly Dictionary<string, string> users = new Dictionary<string, string>();

       /// <summary>
       ///  Checks if a user with this key and name exists.
       /// </summary>
       /// <param name="key">Unique Key of this user.</param>
       /// <param name="name">The name of a user.</param>
       /// <returns>Is registered: true or false</returns>
        public bool CheckUser(string key, string name)
        {
            if (!this.CheckKey(key)) return false;
            this.users.TryGetValue(key, out var value);
            return value == name;
        }

        /// <summary>
        /// Checks if a user with this name and key is registered.
        /// </summary>
        /// <param name="key">Unique Key of this user.</param>
        /// <returns>Is registered: true or false</returns>
        public bool CheckKey(string key)
        {
            // Check if key is default.
            if (key == "-1") return false;
            
            // Check if key is registered.
            return this.users.ContainsKey(key);
        }

        /// <summary>
        /// Checks if the given name is registered or not.
        /// </summary>
        /// <param name="name">The name of a user.</param>
        /// <returns>Is registered: true or false</returns>
        public bool CheckName(string name)
        {
            return this.users.ContainsValue(name);
        }

        /// <summary>
        /// Adds a new user to the dictionary.
        /// </summary>
        /// <param name="name">Name of the new user.</param>
        /// <returns>key - Unique Key of the new user.</returns>
        public string AddUser(string name)
        {
            // generate new key
            var key = Guid.NewGuid().ToString();
            
            // Add new user to dictionary
            this.users.TryAdd(key, name);
            Console.WriteLine($"Client '{name}' with Key '{key}' registered!");

            return key;
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

namespace Server
{

    /// <summary>
    /// A List including the Names of all active Users and their Client Key.
    /// </summary>
    public class UserDict
    {
        private object obj = new();
        private LogWriter logWriter = new LogWriter();
        
        private readonly Dictionary<string, string> users = new Dictionary<string, string>();

       /// <summary>
       ///  Checks if a user with this unique key and name exists.
       /// </summary>
       /// <param name="uKey">Unique key of this user.</param>
       /// <param name="name">The name of a user.</param>
       /// <returns>Is registered: true or false</returns>
        public bool CheckUser(string name, string uKey)
        {
            this.logWriter.WriteLogLine($"Try Check User: '{name}' + '{uKey}'!");
            
            lock (this.obj)
            {
                if (!this.CheckName(name)) return false;
                this.users.TryGetValue(name, out var value);
                return value == uKey;
            }
        }

        /// <summary>
        /// Checks if a user with unique key is registered.
        /// </summary>
        /// <param name="uKey">Unique Key of this user.</param>
        /// <returns>Is registered: true or false</returns>
        public bool CheckKey(string uKey)
        {
            this.logWriter.WriteLogLine($"Try Check Key: '{uKey}' !");

            lock (this.obj)
            {
                // Check if key is default.
                if (uKey == "-1") return false;
                
                // Check if key is registered.
                return this.users.ContainsValue(uKey);
            }
        }

        /// <summary>
        /// Checks if the given name is registered or not.
        /// </summary>
        /// <param name="name">The name of a user.</param>
        /// <returns>Is registered: true or false</returns>
        public bool CheckName(string name)
        {
            this.logWriter.WriteLogLine($"Try Check Name: '{name}' !");

            lock (this.obj)
            {
                return this.users.ContainsKey(name);
            }
        }

        /// <summary>
        /// Adds a new user to the dictionary.
        /// </summary>
        /// <param name="name">Name of the new user.</param>
        /// <returns>key - Unique Key of the new user.</returns>
        public string AddUser(string name)
        {
            this.logWriter.WriteLogLine($"Try Add User: '{name}' !");
            string uKey;
            lock (this.obj)
            {
                // generate new unique key
                uKey = Guid.NewGuid().ToString();
            
                // Add new user to dictionary
                this.users.TryAdd(name, uKey);
            }
            return uKey;
        }

        public string GetAllUser()
        {
            return "";
        }

        public void RemoveUser(string name)
        {
            this.logWriter.WriteLogLine($"TryDelete User: '{name}' !");
            lock (this.obj)
            {

                if (!this.users.ContainsKey(name))
                {
                    this.logWriter.WriteLogLine($"Der User mit dem Key '{name}' existiert nicht!");
                }
                else
                {
                    this.users.Remove(name);
                    this.logWriter.WriteLogLine($"Deleted User: '{name}' !");
                }

            }
        }

        public void ClearUsers(){
            lock (this.obj)
            {
                this.users.Clear();
                this.logWriter.WriteLogLine("Cleared all Users!");
            }
        }
    }
}
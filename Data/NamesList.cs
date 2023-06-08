
namespace Data
{
    /// <summary>
    /// A List including the Names of all active Users.
    /// </summary>
    public class NamesList
    {
        List<string> names = new List<string>();
        
        public void newName(string name){
            if (names.Contains(name)){
                throw new Exception($"{name} wird bereits von jemand anderem benutzt!") ;
            }else
            {
                names.Add(name);
            }
        }

        public string getAllNames(){
            string allNames = "";
            for (int i = 0; i < names.Count; i++)
            {
                allNames = allNames + ", " + names[i];
            }
            return allNames;
        }

        public void removeName(string name){
            if (!names.Contains(name))
            {
                throw new Exception($"{name} wird nicht benutzt!") ;
            }else
            {
                names.Remove(name);
            }
        }

        public void clearNames(){
            if (names.Count == 0)
            {
                throw new Exception("Es sind keine Namen vorhanden!") ;
            }else
            {
                names.Clear();
            }
        }
    }
}
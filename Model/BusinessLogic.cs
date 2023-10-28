using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS341GroupProject.Model
{
    public class BusinessLogic : IBusinessLogic
    {
        private IDatabase Database { get; set; }

        public BusinessLogic() 
        {
            Database = new Database();
        }

        public ObservableCollection<User> Users { get { return Database.SelectAllUsers(); } }

        public Boolean ConfirmLogin(String username, String password)
        {
            User user = Database.SelectUser(username);
            if (user == null) { return false; }
            if (user.Password != password) { return false; }
            return true;
        }
    }
}

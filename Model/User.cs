using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS341GroupProject.Model
{
    public class User
    {
        String username;
        String password;

        public String Username 
        { 
            get { return username; } 
            set { username = value; } 
        }

        public String Password
        {
            get { return password; }
            set { password = value; }
        }

        public User(String username, String password) 
        {
            Username = username;
            Password = password;
        }
    }
}

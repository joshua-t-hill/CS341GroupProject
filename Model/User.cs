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
        String email;
        Boolean isBanned;

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

        public String Email
        {
            get { return email; }
            set { email = value; }
        }

        public Boolean IsBanned
        {
            get { return isBanned; }
            set { isBanned = value; }
        }

        public User(String username, String password, String email, Boolean isBanned) 
        {
            Username = username;
            Password = password;
            Email = email;
            IsBanned = isBanned;
        }
    }
}

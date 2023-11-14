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
        String salt;
        Boolean isAdmin;

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

        public String Salt
        {
            get { return salt; }
            set { salt = value; }
        }

        public Boolean IsAdmin
        {
            get { return isAdmin; }
            set { isAdmin = value; }
        }

        public User(String username, String password, String email, Boolean isBanned) 
        {
            Username = username;
            Password = password;
            Email = email;
            IsBanned = isBanned;
            IsAdmin = false;
        }
        public User(String username, String password, String email, String salt)
        {
            Username = username;
            Password = password;
            Email = email;
            IsBanned = false;
            IsAdmin = false;
            Salt = salt;
        }
    }
}

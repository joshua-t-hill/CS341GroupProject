using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS341GroupProject.Model
{
    public class User : INotifyPropertyChanged
    {
        String username;
        String password;
        String email;
        Boolean isBanned;
        String salt;
        Boolean isAdmin;
        Boolean canBan;

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
            set
            {
                if (isBanned != value)
                {
                    isBanned = value;
                    OnPropertyChanged(nameof(isBanned));
                }
            }
        }

        public String Salt
        {
            get { return salt; }
            set { salt = value; }
        }

        public Boolean IsAdmin
        {
            get { return isAdmin; }
            set
            {
                if (isAdmin != value)
                {
                    isAdmin = value;
                    OnPropertyChanged(nameof(isAdmin));
                }
            }
        }

        public bool CanBan
        {
            get { return canBan; }
            set
            {
                if (canBan != value)
                {
                    canBan = value;
                    OnPropertyChanged(nameof(CanBan));
                }
            }
        }

        public User(String username, String password, String email, Boolean isBanned, Boolean isAdmin) 
        {
            Username = username;
            Password = password;
            Email = email;
            IsBanned = isBanned;
            IsAdmin = isAdmin;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

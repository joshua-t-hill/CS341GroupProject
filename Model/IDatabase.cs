﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS341GroupProject.Model
{
    public interface IDatabase
    {
        public ObservableCollection<User> SelectAllUsers();
        public User SelectUser(String username);
    }
}

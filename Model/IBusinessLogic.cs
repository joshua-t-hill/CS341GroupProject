using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS341GroupProject.Model
{
    public interface IBusinessLogic
    {
        public ObservableCollection<User> Users { get; }
        public ObservableCollection<PinData> PinsData { get; }
        public Boolean ConfirmLogin(String username, String password);
        public UserCreationError CreateUser(String username, String password, String email);
    }
}

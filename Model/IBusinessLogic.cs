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
        public bool IsAdmin { get; set; }

        public LoginError ConfirmLogin(String username, String password);
        public String HashPassword(String password, String salt);
        public String GenerateSalt();
        public UserCreationError CreateUser(String username, String password, String email);

        public UserUpdateError UpdateUser(User user, User newInfo);
        public Boolean InsertPhoto(byte[] imageData);
    }
}

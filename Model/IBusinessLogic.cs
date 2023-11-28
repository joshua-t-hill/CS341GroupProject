using System.Collections.ObjectModel;

namespace CS341GroupProject.Model;
public interface IBusinessLogic
{
    public ObservableCollection<User> Users { get; }
    public ObservableCollection<PinData> CustomPins { get; }
    public ObservableCollection<Post> Posts { get; }
    public bool IsAdmin { get; set; }
    public byte[] TempImageData { get; set; }

    public LoginError ConfirmLogin(String username, String password);
    public String HashPassword(String password, String salt);
    public String GenerateSalt();
    public UserCreationError CreateUser(String username, String password, String email);

    public UserUpdateError UpdateUser(User user, User newInfo);
    public Boolean InsertPhoto(byte[] imageData);
    public Photo SelectPhoto(byte[] imageData);
    public Photo SelectPhoto(Guid photoId);
    public Boolean InsertPost(String username, String genus, String species, String notes, Guid photoId);
}

using System.Collections.ObjectModel;

namespace CS341GroupProject.Model;
public interface IDatabase
{
    public ObservableCollection<User> SelectAllUsers();
    public ObservableCollection<PinData> SelectAllMapPins();
    public ObservableCollection<Photo> SelectAllPhotos();
    public ObservableCollection<Post> SelectAllPosts();

    public User SelectUserWithUsername(String username);
    public User SelectUserWithEmail(String email);
    public String SelectSalt(String username);
    public UserCreationError InsertUser(User user);
    public UserUpdateError UpdateUser(User user, User newInfo);
    public Boolean UpdateUserPassword(User user, String hashedPassword, Boolean tempPassword);

    public User SelectUser(String username);
    public Boolean InsertPhoto(byte[] imageData);
    public Photo SelectPhoto(byte[] imageData);
    public Photo SelectPhoto(Guid photoId);
    public Boolean InsertPost(String username, String genus, String species, String notes, Guid photoId);

}

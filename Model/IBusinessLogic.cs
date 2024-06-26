﻿using System.Collections.ObjectModel;

namespace CS341GroupProject.Model;
public interface IBusinessLogic
{
    public ObservableCollection<User> Users { get; }
    public ObservableCollection<PinData> CustomPins { get; }
    public ObservableCollection<Post> DynamicPosts { get; }
    public int NumPosts { get; }
    public Boolean JustAddedPost { get; set; }
    public bool IsAdmin { get; set; }
    public byte[] TempImageData { get; set; }

    public ObservableCollection<Post> DynamicSelectPosts(int pageNumber);
    public LoginError ConfirmLogin(String username, String password);
    public String HashPassword(String password, String salt);
    public String GenerateSalt();
    public UserCreationError CreateUser(String username, String password, String email);
    public Task CreateResetPasswordEmail(String username);
    public String GenerateRandomOTP();
    public Boolean ChangeUserPassword(String username, String newPassword, Boolean tempPassword);

    public UserUpdateError UpdateUser(User user, User newInfo);
    public Boolean InsertPhoto(byte[] imageData);
    public Photo SelectPhoto(byte[] imageData);
    public Photo SelectPhoto(Guid photoId);
    public Boolean InsertPost(String username, String genus, String species, String notes, Guid photoId);

    public Boolean InsertPin(Double latitude, Double longitude, String genus, String epithet, Guid photoId);
    public long GetPinId(Double latitude, Double longitude, String genus, String epithet);

    public Task<Location> GetCurrentLocation();
}

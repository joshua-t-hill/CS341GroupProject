namespace CS341GroupProject.Model;

public enum UserCreationError
{
    UsernameAlreadyExists,
    EmailInUse,
    DBInsertionError,
    NoError
}

public enum UserUpdateError
{
    DBUpdateError,
    NoError,
    UserNotFound
}
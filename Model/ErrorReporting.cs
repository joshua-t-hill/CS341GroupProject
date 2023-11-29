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

public enum LoginError
{
    IncorrectInput,
    UserBanned,
    TempPasswordEntered,
    NoError
}
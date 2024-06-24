namespace AuthDemo.Exceptions
{
    [Serializable]
    internal class UserNotFoundException(string userName) : Exception($"User {userName} not found in the DB.")
    {
    }
}


/// <summary>
/// Currently very little functionality for Users, only a name
/// Creating class to allow for future scalability with their functionality, examples below:
///     Unique ID's
///     User verification
///     Contact information
///     Easier tranfers between accounts owned by same user
/// </summary>
public class User
{
    public string name;
    public string userId;
    
    public User(string name)
    {
        this.name = name;
        this.userId = Guid.NewGuid().ToString("N");
    }
}
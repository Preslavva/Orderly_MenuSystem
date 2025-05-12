public class Role
{
    public int Id { get; }
    public string Type { get; }

    public Role(int id, string type)
    {
        Id = id;
        Type = type;
    }

    public Role(string type)
    {
        Type = type;
    }
}
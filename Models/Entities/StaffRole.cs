public class StaffRole
{
    public int Id { get; }
    public int StaffId { get; }
    public int RoleId { get; }
    public bool IsActive { get; }

    public StaffRole(int id, int staffId, int roleId, bool isActive)
    {
        Id = id;
        StaffId = staffId;
        RoleId = roleId;
        IsActive = isActive;
    }

    public StaffRole(int staffId, int roleId, bool isActive)
    {
        StaffId = staffId;
        RoleId = roleId;
        IsActive = isActive;
    }
}
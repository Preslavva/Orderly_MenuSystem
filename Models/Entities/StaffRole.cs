public class StaffRole
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public int RoleId { get; set; }
    public bool IsActive { get; set; }

    public StaffRole() { }

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
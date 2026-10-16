public class NewRoleDTO
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }
}

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

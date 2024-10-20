using Employee_Leave_ManagementPro.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class LeaveBalance
{
    [Key]
    public int LeaveBalanceID { get; set; }

    [ForeignKey("Employee")]
    public int EmployeeID { get; set; } // Foreign key to Employee table

    public int AnnualLeave { get; set; } = 20; // Default balance for Annual Leave

    public int SickLeave { get; set; } = 10; // Default balance for Sick Leave

    public int CasualLeave { get; set; } = 10; // Default balance for Casual Leave

    public int OtherLeave { get; set; } = 5; // Default balance for Other Leave

    // Navigation property (optional)
    public virtual Employee Employee { get; set; }
}

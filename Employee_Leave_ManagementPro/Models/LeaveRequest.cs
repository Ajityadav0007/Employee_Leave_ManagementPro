using Employee_Leave_ManagementPro.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class LeaveRequest
{
    [Key]
    public int LeaveRequestID { get; set; }

    [ForeignKey("Employee")]
    public int EmployeeID { get; set; } // Foreign key to Employee table

    [Required]
    public string LeaveType { get; set; } // Example: Annual, Sick, Casual, etc.

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public string Reason { get; set; }

    [Required]
    public string Status { get; set; } = "Pending"; // Default value is 'Pending'

    public string AdminRemarks { get; set; }

    public DateTime DateSubmitted { get; set; } = DateTime.Now; // Default to current date

    // Navigation property (optional)
    public virtual Employee Employee { get; set; }
}

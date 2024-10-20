using Microsoft.EntityFrameworkCore;

namespace Employee_Leave_ManagementPro.Models
{
    public class EmployeeDbcontext : DbContext
    {
        public EmployeeDbcontext(DbContextOptions<EmployeeDbcontext> options) : base(options)
        {

        }

        public DbSet<Employee> Employees1 { get; set; }
        

        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        public DbSet<LeaveBalance> leaveBalances { get; set; }
    }
}

using Employee_Leave_ManagementPro.Models;
using Microsoft.EntityFrameworkCore;

namespace Employee_Leave_ManagementPro.DBContext
{
    public class EmployeeDbcontext: DbContext
    {
        public EmployeeDbcontext(DbContextOptions<EmployeeDbcontext> options) : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }
    }
}

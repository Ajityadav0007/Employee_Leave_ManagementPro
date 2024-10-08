namespace Employee_Leave_ManagementPro.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }       // Primary Key
        public string Name { get; set; }          // Employee Name
        public string Email { get; set; }         // Employee Email
        public string Password { get; set; }      // Hashed Password
        public string Department { get; set; }    // Employee Department
        public string Designation { get; set; }   // Employee Designation
        public string Role { get; set; }          //  Employee or Admin

    }
}

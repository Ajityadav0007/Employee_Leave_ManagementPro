namespace Employee_Leave_ManagementPro.Models
{
    public class EmployeeRegisterDTO
    {
        public string Name { get; set; }          // Employee Name
        public string Email { get; set; }         // Employee Email
        public string Password { get; set; }      // Hashed Password
        public string Department { get; set; }    // Employee Department
        public string Designation { get; set; }   // Employee Designation
        public string Role { get; set; }          //  Employee or Admin

    }
}

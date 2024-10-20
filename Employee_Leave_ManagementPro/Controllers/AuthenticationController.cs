using Employee_Leave_ManagementPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Employee_Leave_ManagementPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly EmployeeDbcontext _dbContext;
        private readonly IConfiguration _configuration;
        public AuthenticationController(EmployeeDbcontext mydbContext, IConfiguration configuration)
        {
            _dbContext = mydbContext;
            _configuration = configuration;
        }

        // Here we Register the employee with their data
        [HttpPost]
        [Route("Registration Of Employee")]
        public IActionResult Registration(EmployeeRegisterDTO employeeRegisterDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var employee = _dbContext.Employees1.FirstOrDefault(x => x.Email == employeeRegisterDTO.Email);
            if (employee == null) //the employee is not exist then add the new employee
            {
                var newEmployee = new Employee
                {
                    Name = employeeRegisterDTO.Name,
                    Email = employeeRegisterDTO.Email,
                    Password = employeeRegisterDTO.Password, // Ensure this is hashed in production
                    Department = employeeRegisterDTO.Department,
                    Designation = employeeRegisterDTO.Designation,
                    Role = employeeRegisterDTO.Role
                };

                _dbContext.Employees1.Add(newEmployee);
                _dbContext.SaveChanges();

                // Set default leave balances for the new employee
                var leaveBalance = new LeaveBalance
                {
                    EmployeeID = newEmployee.EmployeeID,
                    AnnualLeave = 12, // Set your default annual leave
                    SickLeave = 10,    // Set your default sick leave
                    CasualLeave = 5,   // Set your default casual leave
                    OtherLeave = 2    // Set your default other leave if needed
                };

                _dbContext.leaveBalances.Add(leaveBalance);
                _dbContext.SaveChanges();

                return Ok(new { message = "Employee Registered Successfully" });
            }
            else
            {
                return BadRequest(new { message = "Employee is already Exist, with Same Email address." });
            }
        }

        //Employee is already registered then login using credential
        [HttpPost]
        [Route("Employee Login")]
        public IActionResult Login(EmployeeLoginDTO employeeLoginDTO)
        {
            var employee = _dbContext.Employees1.FirstOrDefault(x => x.Email == employeeLoginDTO.Email && x.Password == employeeLoginDTO.Password);
            if (employee != null)
            {
                var claims = new[]
               {
                    new Claim(JwtRegisteredClaimNames.Sub,_configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, employee.EmployeeID.ToString()),
                   //new Claim("EmployeeID",employee.EmployeeID.ToString()),
                   // new Claim("Role",employee.Role.ToString())
                   new Claim(ClaimTypes.Role, employee.Role)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMonths(60),
                    signingCredentials: signIn
                    );
                String tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new { Token = tokenValue, Employee = employee });

            }
            else
            {
                return Unauthorized(new { message = "Invalid Username Or Password!!!" }); //Employee not registered
            }
        }
        // Authorization using role after JWT authentication
        [Authorize(Roles = "Employee")]
        [HttpGet("Employee-only")]
        public IActionResult EmployeeOnlyAction()
        {
            return Ok(new { message = "Yes, you Are Employeee, your Authentication Successful!!" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Admin-only")]
        public IActionResult AdminOnlyAction()
        {
            return Ok(new { message = "Yes, you Are Admin, your Authentication Successful!!" });
        }

    }
}

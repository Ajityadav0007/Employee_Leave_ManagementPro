using Employee_Leave_ManagementPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Employee_Leave_ManagementPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeDbcontext _dbContext;

        public EmployeeController(EmployeeDbcontext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get the leave balance for a specified employee.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <returns>The leave balance of the employee.</returns>
        [HttpGet("leave-balance")]
        [SwaggerOperation(Summary = "Retrieve leave balance for a specified employee")]
        public async Task<IActionResult> GetLeaveBalance(int employeeId)
        {
            try
            {
                var leaveBalance = await _dbContext.leaveBalances
                    .Include(lb => lb.Employee)
                    .FirstOrDefaultAsync(lb => lb.EmployeeID == employeeId);

                if (leaveBalance == null)
                {
                    return NotFound(new { message = "Employee balance is not calculated." });
                }

                return Ok(new
                {
                    EmployeeName = leaveBalance.Employee.Name,
                    leaveBalance.AnnualLeave,
                    leaveBalance.SickLeave,
                    leaveBalance.CasualLeave,
                    leaveBalance.OtherLeave
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving leave balance.", error = ex.Message });
            }
        }

        /// <summary>
        /// Apply for leave for a specified employee.
        /// </summary>
        /// <param name="leaveRequestDto">The leave request details.</param>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <returns>Status of the leave application.</returns>
        [HttpPost("apply-leave")]
        [SwaggerOperation(Summary = "Submit a leave request for a specified employee")]
        public async Task<IActionResult> ApplyForLeave([FromBody] LeaveRequestDTO leaveRequestDto, int employeeId)
        {
            try
            {
                var employee = await _dbContext.Employees1.SingleOrDefaultAsync(e => e.EmployeeID == employeeId);
                if (employee == null)
                {
                    return NotFound(new { message = "Employee not found" });
                }

                if (leaveRequestDto.StartDate > leaveRequestDto.EndDate)
                {
                    return BadRequest(new { message = "Start date cannot be after end date." });
                }

                var overlappingLeaves = await _dbContext.LeaveRequests
                    .AnyAsync(lr => lr.EmployeeID == employee.EmployeeID
                                    && lr.Status == "Approved"
                                    && lr.StartDate <= leaveRequestDto.EndDate
                                    && lr.EndDate >= leaveRequestDto.StartDate);

                if (overlappingLeaves)
                {
                    return BadRequest(new { message = "This leave request overlaps with an existing approved leave." });
                }

                var leaveRequest = new LeaveRequest
                {
                    EmployeeID = employee.EmployeeID,
                    LeaveType = leaveRequestDto.LeaveType,
                    StartDate = leaveRequestDto.StartDate,
                    EndDate = leaveRequestDto.EndDate,
                    Reason = leaveRequestDto.Reason,
                    Status = "Pending",
                    AdminRemarks = "NA", // Assign default value
                    DateSubmitted = DateTime.Now
                };

                _dbContext.LeaveRequests.Add(leaveRequest);
                await _dbContext.SaveChangesAsync();

                return StatusCode(201, new { message = "Leave request submitted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while submitting the leave request.", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieve the leave request history for a specified employee.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <returns>A list of leave requests for the employee.</returns>
        [HttpGet("leave-history")]
        [SwaggerOperation(Summary = "Get leave request history for a specified employee")]
        public async Task<IActionResult> GetLeaveRequest(int employeeId)
        {
            try
            {
                var leaveRequests = await _dbContext.LeaveRequests
                    .Where(lr => lr.EmployeeID == employeeId)
                    .ToListAsync();

                if (leaveRequests == null || !leaveRequests.Any())
                {
                    return NotFound(new { message = "Employee leave requests not found." });
                }

                return Ok(leaveRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving leave requests.", error = ex.Message });
            }
        }
    }
}

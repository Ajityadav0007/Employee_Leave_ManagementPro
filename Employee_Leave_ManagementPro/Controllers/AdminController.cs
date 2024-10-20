using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Employee_Leave_ManagementPro.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Employee_Leave_Management_Project.Controllers
{
    [ApiController]
  
    [Route("api/admin-hr-panel")] // Changed to AdminPanel
    public class AdminPanelController : ControllerBase // Renamed to AdminPanelController
    {
        private readonly EmployeeDbcontext _dbContext;


        public AdminPanelController(EmployeeDbcontext context) // Updated constructor name
        {
            _dbContext = context;
        }

        /// <summary>
        /// Get all pending leave requests.
        /// </summary>
        /// <returns>List of pending leave requests</returns>
        [HttpGet("PendingLeave")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get all pending leave requests.")]
        public async Task<IActionResult> GetPendingLeaves()
        {
            try
            {
                var pendingLeaves = await _dbContext.LeaveRequests
                    .Where(lr => lr.Status == "Pending")
                    .ToListAsync();

                if (!pendingLeaves.Any())
                {
                    return Ok(new { message = "No pending leave requests found." });
                }

                return Ok(pendingLeaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        /// <summary>
        /// Approve a pending leave request for an employee.
        /// </summary>
        /// <param name="employeeId">Employee ID</param>
        /// <param name="adminRemarks">Admin remarks</param>
        /// <returns>Approval status</returns>
        [HttpPut("Approve/{employeeId}")]

        [SwaggerOperation(Summary = "Approve a pending leave request for an employee.")]
        public async Task<IActionResult> ApproveLeave(int employeeId, [FromBody] string adminRemarks)
        {
            try
            {
                // Fetch leave request
                var leaveRequest = await _dbContext.LeaveRequests
                    .FirstOrDefaultAsync(lr => lr.EmployeeID == employeeId);

                if (leaveRequest == null)
                {
                    return NotFound(new { message = "No leave request found for the specified employee." });
                }

                if (leaveRequest.Status == "Approved")
                {
                    return BadRequest(new { message = "This leave request has already been approved." });
                }
                else if (leaveRequest.Status == "Rejected")
                {
                    return BadRequest(new { message = "This leave request has already been rejected." });
                }

                var leaveBalance = await _dbContext.leaveBalances.FirstOrDefaultAsync(lb => lb.EmployeeID == employeeId);
                if (leaveBalance == null)
                {
                    return BadRequest(new { message = "Leave balance not found for the employee." });
                }

                var leaveDays = (leaveRequest.EndDate - leaveRequest.StartDate).TotalDays + 1;

                switch (leaveRequest.LeaveType.ToLower())
                {
                    case "annual":
                        if (leaveBalance.AnnualLeave < leaveDays)
                        {
                            return BadRequest(new { message = "Insufficient Annual leave balance" });
                        }
                        leaveBalance.AnnualLeave -= (int)leaveDays;
                        break;
                    case "sick":
                        if (leaveBalance.SickLeave < leaveDays)
                        {
                            return BadRequest(new { message = "Insufficient Sick leave balance" });
                        }
                        leaveBalance.SickLeave -= (int)leaveDays;
                        break;
                    case "casual":
                        if (leaveBalance.CasualLeave < leaveDays)
                        {
                            return BadRequest(new { message = "Insufficient Casual leave balance" });
                        }
                        leaveBalance.CasualLeave -= (int)leaveDays;
                        break;
                    default:
                        return BadRequest(new { message = "Invalid leave type" });
                }

                leaveRequest.Status = "Approved";
                leaveRequest.AdminRemarks = adminRemarks;

                _dbContext.leaveBalances.Update(leaveBalance);
                _dbContext.LeaveRequests.Update(leaveRequest);
                await _dbContext.SaveChangesAsync();

                return Ok(new { message = "Leave request approved successfully", leaveRequest });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        /// <summary>
        /// Reject a leave request by employee ID.
        /// </summary>
        /// <param name="employeeId">Employee ID</param>
        /// <param name="adminRemarks">Admin remarks</param>
        /// <returns>Rejection status</returns>
        [HttpPut("RejectLeave/{employeeId}")]
        [SwaggerOperation(Summary = "Reject a leave request by employee ID.")]
        public async Task<IActionResult> RejectLeaveByEmployee(int employeeId, [FromBody] string adminRemarks)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(adminRemarks))
                {
                    return BadRequest(new { message = "Admin remarks cannot be empty." });
                }

                var leaveRequest = await _dbContext.LeaveRequests
                    .FirstOrDefaultAsync(lr => lr.EmployeeID == employeeId);

                if (leaveRequest == null)
                {
                    return NotFound(new { message = "No pending leave request found for the specified employee." });
                }

                if (leaveRequest.Status == "Approved")
                {
                    return BadRequest(new { message = "This leave request has already been approved and cannot be rejected." });
                }
                else if (leaveRequest.Status == "Rejected")
                {
                    return BadRequest(new { message = "This leave request has already been rejected." });
                }

                leaveRequest.Status = "Rejected";
                leaveRequest.AdminRemarks = adminRemarks;

                _dbContext.LeaveRequests.Update(leaveRequest);
                await _dbContext.SaveChangesAsync();

                return Ok(new { message = "Leave request rejected successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }



        }


        [HttpGet("Statistics")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Retrieve all approved leave requests with employee details.")]
        public async Task<IActionResult> GetLeaveStatistics()
        {
            try
            {
                var currentDate = DateTime.Now;

                var approvedLeaves = await _dbContext.LeaveRequests
                    .Where(leave => leave.Status == "Approved")
                    .Include(leave => leave.Employee) // Include the Employee navigation property
                    .Select(leave => new
                    {
                        EmployeeName = leave.Employee.Name, // Fetching employee name
                        leave.EmployeeID,
                        leave.StartDate,
                        leave.EndDate,
                        Duration = (leave.EndDate - leave.StartDate).TotalDays,
                        leave.Reason
                    })
                    .ToListAsync();

                if (!approvedLeaves.Any())
                {
                    return Ok(new { Message = "No employees have upcoming approved leaves." });
                }

                return Ok(approvedLeaves);
            }
            catch (DbUpdateException dbEx)
            {


                return StatusCode(500, new { Message = "A database error occurred while retrieving leave statistics." });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { Message = $"An unexpected error occurred: {ex.Message}" });
            }
        }
    }
}

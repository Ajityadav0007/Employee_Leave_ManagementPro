using System.ComponentModel.DataAnnotations;

namespace Employee_Leave_ManagementPro.Models
{
    public class LeaveRequestDTO
    {

        [Required]
        public string LeaveType { get; set; }  // Type of leave (Annual, Sick, etc.)

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }  // Start date of the leave

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }    // End date of the leave

        [Required]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
        public string Reason { get; set; }       // Reason for the leave

        public string AdminRemarks { get; set; }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employee_Leave_ManagementPro.Migrations
{
    /// <inheritdoc />
    public partial class myProjectDemo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees1",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees1", x => x.EmployeeID);
                });

            migrationBuilder.CreateTable(
                name: "leaveBalances",
                columns: table => new
                {
                    LeaveBalanceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    AnnualLeave = table.Column<int>(type: "int", nullable: false),
                    SickLeave = table.Column<int>(type: "int", nullable: false),
                    CasualLeave = table.Column<int>(type: "int", nullable: false),
                    OtherLeave = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leaveBalances", x => x.LeaveBalanceID);
                    table.ForeignKey(
                        name: "FK_leaveBalances_Employees1_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees1",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequests",
                columns: table => new
                {
                    LeaveRequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    LeaveType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminRemarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateSubmitted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequests", x => x.LeaveRequestID);
                    table.ForeignKey(
                        name: "FK_LeaveRequests_Employees1_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees1",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_leaveBalances_EmployeeID",
                table: "leaveBalances",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_EmployeeID",
                table: "LeaveRequests",
                column: "EmployeeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "leaveBalances");

            migrationBuilder.DropTable(
                name: "LeaveRequests");

            migrationBuilder.DropTable(
                name: "Employees1");
        }
    }
}

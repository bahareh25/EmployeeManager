using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManager.Core.Models
{
    public class EmployeeDepartmentAssignment
    {
        public int AssignmentId { get; set; }

        public int EmployeeId { get; set; }

        public int DepartmentId { get; set; }

        public DateTime AssignmentDate { get; set; }

        public AssignmentStatus Status { get; set; }

        // Navigation properties
        public Employee Employee { get; set; } = null!;

        public Department Department { get; set; } = null!;
    }
}

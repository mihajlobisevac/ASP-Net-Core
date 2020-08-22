using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        public List<Employee> Employees { get; set; } = new List<Employee>();

        public MockEmployeeRepository()
        {
            Employees.Add(new Employee { Id = 1, Name = "Will", Email = "will@gmail.com", Department = EDepartments.HR });
            Employees.Add(new Employee { Id = 2, Name = "Stone", Email = "stone@gmail.com", Department = EDepartments.IT });
            Employees.Add(new Employee { Id = 3, Name = "Clock", Email = "clock@gmail.com", Department = EDepartments.Payroll });
        }

        public Employee GetEmployee(int id)
        {
            return Employees.FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return Employees;
        }
    }
}

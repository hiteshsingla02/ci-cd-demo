using EmployeeManagementApi.Data;
using EmployeeManagementApi.DTOs;
using EmployeeManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees(
            [FromQuery] string? department,
            [FromQuery] bool? isActive)
        {
            var query = _context.Employees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(department))
                query = query.Where(e => e.Department == department);

            if (isActive.HasValue)
                query = query.Where(e => e.IsActive == isActive.Value);

            var employees = await query.OrderBy(e => e.Id).ToListAsync();
            return Ok(employees);
        }

        // GET: api/employees/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound(new { message = $"Employee with id {id} was not found." });

            return Ok(employee);
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(EmployeeCreateDto dto)
        {
            var emailExists = await _context.Employees.AnyAsync(e => e.Email == dto.Email);
            if (emailExists)
                return Conflict(new { message = "An employee with this email already exists." });

            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Department = dto.Department,
                Position = dto.Position,
                Salary = dto.Salary,
                HireDate = dto.HireDate,
                IsActive = true
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        // PUT: api/employees/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeUpdateDto dto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound(new { message = $"Employee with id {id} was not found." });

            var emailTaken = await _context.Employees
                .AnyAsync(e => e.Email == dto.Email && e.Id != id);
            if (emailTaken)
                return Conflict(new { message = "Another employee already uses this email." });

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.Email = dto.Email;
            employee.PhoneNumber = dto.PhoneNumber;
            employee.Department = dto.Department;
            employee.Position = dto.Position;
            employee.Salary = dto.Salary;
            employee.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/employees/5/deactivate
        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> DeactivateEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound(new { message = $"Employee with id {id} was not found." });

            employee.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/employees/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound(new { message = $"Employee with id {id} was not found." });

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

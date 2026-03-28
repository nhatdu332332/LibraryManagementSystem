using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Data.Entities;

namespace LibraryManagementSystem.Repositories.Interfaces
{
	public interface IEmployeeRepository : IGenericRepository<Employee>
	{
		Task<Employee> GetByEmailAsync(string email);
		Task<IEnumerable<Employee>> GetByRoleIdAsync(int roleId);
	}
}

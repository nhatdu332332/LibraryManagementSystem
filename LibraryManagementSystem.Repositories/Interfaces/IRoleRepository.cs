using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Data.Entities;

namespace LibraryManagementSystem.Repositories.Interfaces
{
	public interface IRoleRepository : IGenericRepository<Role>
	{
		Task<Role> GetByNameAsync(string roleName);
	}
}

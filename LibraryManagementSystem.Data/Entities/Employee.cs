using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Data.Entities
{
	public class Employee
	{
		[Key]
		public int EmployeeId { get; set; }

		[Required]
		[StringLength(100)]
		[EmailAddress]
		public string Email { get; set; } = null!;

		[Required]
		[StringLength(255)]
		public string FullName { get; set; } = null!;

		[Required]
		public int RoleId { get; set; }

		public DateTime HireDate { get; set; }

		[Required]
		[StringLength(20)]
		public string Status { get; set; } = null!;  // Active, Inactive, ...

		// Tên property khớp với tên cột PasswordHash trong DB
		[Required]
		[StringLength(255)]
		public string PasswordHash { get; set; } = string.Empty;  // plaintext, không hash thật

		// Navigation properties
		public virtual Role Role { get; set; } = null!;
		public virtual ICollection<BorrowTransaction> BorrowTransactions { get; set; } = new List<BorrowTransaction>();
		public virtual ICollection<BorrowRequest> ApprovedBorrowRequests { get; set; } = new List<BorrowRequest>();
	}
}
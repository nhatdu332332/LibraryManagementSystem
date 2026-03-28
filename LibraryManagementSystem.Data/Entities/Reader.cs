using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Data.Entities
{
	public class Reader
	{
		[Key]
		public int ReaderId { get; set; }

		[Required]
		[StringLength(30)]
		public string CardNumber { get; set; } = null!;

		[Required]
		[StringLength(100)]
		[EmailAddress]
		public string Email { get; set; } = null!;

		[Required]
		[StringLength(255)]
		public string FullName { get; set; } = null!;

		[StringLength(20)]
		public string? PhoneNumber { get; set; }

		[StringLength(500)]
		public string? Address { get; set; }

		public DateTime RegisterDate { get; set; }

		public DateTime ExpiredDate { get; set; }

		[Required]
		[StringLength(20)]
		public string ReaderStatus { get; set; } = null!;  // Active, Expired, Suspended, ...

		// Tên property khớp với tên cột PasswordHash trong DB
		[Required]
		[StringLength(255)]
		public string PasswordHash { get; set; } = string.Empty;  // plaintext, không hash thật

		// Navigation properties
		public virtual ICollection<BorrowRequest> BorrowRequests { get; set; } = new List<BorrowRequest>();
		public virtual ICollection<BorrowTransaction> BorrowTransactions { get; set; } = new List<BorrowTransaction>();
	}
}
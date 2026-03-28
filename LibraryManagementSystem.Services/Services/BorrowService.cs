// Full code cho BorrowService.cs (thêm GetAllBorrowTransactionsAsync với full mapping, sử dụng repository đã include)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Repositories.Repositories;
using LibraryManagementSystem.Services.DTOs;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
	public class BorrowService : IBorrowService
	{
		private readonly IUnitOfWork _uow;

		public BorrowService(IUnitOfWork uow)
		{
			_uow = uow ?? throw new ArgumentNullException(nameof(uow));
		}

		public async Task<BorrowRequestDto> SubmitBorrowRequestAsync(int readerId, SubmitBorrowRequestDto dto)
		{
			var reader = await _uow.ReaderRepository.GetByIdAsync(readerId);
			if (reader == null) throw new KeyNotFoundException("Không tìm thấy độc giả");

			var request = new BorrowRequest
			{
				ReaderId = readerId,
				RequestDate = DateTime.Now,
				Status = "Pending"
			};

			await _uow.BorrowRequestRepository.AddAsync(request);
			await _uow.SaveChangesAsync();

			foreach (var detail in dto.Details)
			{
				var work = await _uow.BookWorkRepository.GetByIdAsync(detail.WorkId);
				if (work == null) throw new KeyNotFoundException($"Không tìm thấy tác phẩm ID {detail.WorkId}");

				var requestDetail = new BorrowRequestDetail
				{
					RequestId = request.RequestId,
					WorkId = detail.WorkId,
					RequestedQuantity = detail.RequestedQuantity
				};

				await _uow.DbContext.BorrowRequestDetails.AddAsync(requestDetail);
			}

			await _uow.SaveChangesAsync();

			return new BorrowRequestDto
			{
				RequestId = request.RequestId,
				ReaderId = request.ReaderId,
				ReaderFullName = reader.FullName,
				RequestDate = request.RequestDate,
				Status = request.Status,
				Details = dto.Details.Select(d => new BorrowRequestDetailDto
				{
					WorkId = d.WorkId,
					Title = _uow.DbContext.BookWorks.Find(d.WorkId)?.Title ?? "Unknown",
					RequestedQuantity = d.RequestedQuantity
				}).ToList()
			};
		}

		public async Task<IEnumerable<BorrowRequestDto>> GetPendingRequestsAsync()
		{
			var requests = await _uow.BorrowRequestRepository.GetPendingRequestsAsync();
			return requests.Select(r => new BorrowRequestDto
			{
				RequestId = r.RequestId,
				ReaderId = r.ReaderId,
				ReaderFullName = r.Reader.FullName,
				RequestDate = r.RequestDate,
				Status = r.Status,
				Details = r.Details.Select(d => new BorrowRequestDetailDto
				{
					WorkId = d.WorkId,
					Title = d.BookWork.Title,
					RequestedQuantity = d.RequestedQuantity,
					ApprovedQuantity = d.ApprovedQuantity
				}).ToList()
			});
		}

		public async Task ApproveBorrowRequestAsync(int requestId, int employeeId, ApproveBorrowRequestDto dto)
		{
			var request = await _uow.BorrowRequestRepository.GetRequestWithDetailsAsync(requestId);
			if (request == null) throw new KeyNotFoundException("Không tìm thấy yêu cầu");

			if (request.Status != "Pending") throw new InvalidOperationException("Yêu cầu không phải đang chờ duyệt");

			var employee = await _uow.EmployeeRepository.GetByIdAsync(employeeId);
			if (employee == null) throw new KeyNotFoundException("Không tìm thấy nhân viên");

			int totalApproved = 0;

			foreach (var approveDetail in dto.ApprovedDetails)
			{
				var detail = request.Details.FirstOrDefault(d => d.RequestDetailId == approveDetail.RequestDetailId);
				if (detail == null) continue;

				var work = await _uow.BookWorkRepository.GetByIdAsync(detail.WorkId);
				var availableCopies = work.BookEditions
					.SelectMany(e => e.BookCopies)
					.Count(c => c.PhysicalCondition == "Good" || c.PhysicalCondition == "Excellent");

				// Sửa lỗi: dùng GetValueOrDefault để an toàn với nullable int
				int requested = detail.RequestedQuantity;
				int approvedProposed = approveDetail.ApprovedQuantity;
				int approvedLimited = Math.Min(approvedProposed, requested);
				int finalApproved = Math.Min(approvedLimited, availableCopies);

				detail.ApprovedQuantity = finalApproved;  // gán trực tiếp (int)

				totalApproved += finalApproved;
			}

			request.ApprovedByEmployeeId = employeeId;
			request.ApprovedDate = DateTime.Now;
			request.Status = totalApproved > 0 ? "Approved" : "Rejected";

			await _uow.BorrowRequestRepository.UpdateAsync(request);
			await _uow.SaveChangesAsync();
		}

		public async Task RejectBorrowRequestAsync(int requestId, string reason)
		{
			var request = await _uow.BorrowRequestRepository.GetByIdAsync(requestId);
			if (request == null) throw new KeyNotFoundException("Không tìm thấy yêu cầu");

			if (request.Status != "Pending") throw new InvalidOperationException("Yêu cầu không phải đang chờ duyệt");

			request.Status = "Rejected";
			request.RejectReason = reason;

			await _uow.BorrowRequestRepository.UpdateAsync(request);
			await _uow.SaveChangesAsync();
		}

		public async Task<BorrowTransactionDto> CreateBorrowTransactionAsync(int requestId, int employeeId, List<int> copyIds)
		{
			var request = await _uow.BorrowRequestRepository.GetRequestWithDetailsAsync(requestId);
			if (request == null || request.Status != "Approved") throw new InvalidOperationException("Yêu cầu chưa được duyệt hoặc không tồn tại");

			var transaction = new BorrowTransaction
			{
				ReaderId = request.ReaderId,
				EmployeeId = employeeId,
				RequestId = requestId,
				BorrowDate = DateTime.Now,
				Status = "Normal"
			};

			await _uow.BorrowTransactionRepository.AddAsync(transaction);
			await _uow.SaveChangesAsync();

			int copyIndex = 0;
			foreach (var detail in request.Details.Where(d => d.ApprovedQuantity > 0))
			{
				for (int i = 0; i < detail.ApprovedQuantity; i++)
				{
					var copyId = copyIds[copyIndex++];
					var copy = await _uow.BookCopyRepository.GetByIdAsync(copyId);
					if (copy == null || (copy.PhysicalCondition != "Good" && copy.PhysicalCondition != "Excellent"))
						throw new InvalidOperationException($"Bản sao ID {copyId} không khả dụng");

					var transactionDetail = new BorrowTransactionDetail
					{
						BorrowId = transaction.BorrowId,
						CopyId = copyId,
						DueDate = DateTime.Now.AddDays(14),
						ItemStatus = "Borrowing"
					};

					await _uow.DbContext.BorrowTransactionDetails.AddAsync(transactionDetail);

					copy.PhysicalCondition = "Borrowed";  // ← sửa thành PhysicalCondition
					await _uow.BookCopyRepository.UpdateAsync(copy);
				}
			}

			await _uow.SaveChangesAsync();

			var loadedTransaction = await _uow.BorrowTransactionRepository.GetTransactionWithDetailsAsync(transaction.BorrowId);
			return GetTransactionDto(loadedTransaction);
		}

		public async Task<BorrowTransactionDto> CreateDirectBorrowTransactionAsync(
	int readerId,
	int employeeId,
	List<int> copyIds,
	DateTime borrowDate,
	int durationDays)
		{
			// Validate
			var reader = await _uow.ReaderRepository.GetByIdAsync(readerId);
			if (reader == null) throw new KeyNotFoundException("Không tìm thấy độc giả");

			var employee = await _uow.EmployeeRepository.GetByIdAsync(employeeId);
			if (employee == null) throw new KeyNotFoundException("Không tìm thấy nhân viên");

			if (copyIds == null || !copyIds.Any())
				throw new ArgumentException("Phải chọn ít nhất một bản sao");

			var transaction = new BorrowTransaction
			{
				ReaderId = readerId,
				EmployeeId = employeeId,
				RequestId = null,                    // direct borrow
				BorrowDate = borrowDate,
				Status = "Normal"
			};

			await _uow.BorrowTransactionRepository.AddAsync(transaction);
			await _uow.SaveChangesAsync();

			foreach (var copyId in copyIds)
			{
				var copy = await _uow.BookCopyRepository.GetByIdAsync(copyId);
				// Kiểm tra ĐÚNG trạng thái lưu thông
				if (copy == null || copy.CirculationStatus != "Available")
					throw new InvalidOperationException($"Bản sao ID {copyId} đang không sẵn sàng để mượn.");

				var detail = new BorrowTransactionDetail
				{
					BorrowId = transaction.BorrowId,
					CopyId = copyId,
					DueDate = borrowDate.AddDays(durationDays),
					ItemStatus = "Borrowing"
				};

				await _uow.DbContext.BorrowTransactionDetails.AddAsync(detail);

				// Cập nhật trạng thái bản sao (giống logic cũ của request-based)
				copy.CirculationStatus = "Borrowed"; // Cập nhật trạng thái lưu thông
													 // Giữ nguyên PhysicalCondition là "Good", không nên đổi nó thành "Borrowed"
				await _uow.BookCopyRepository.UpdateAsync(copy);
			}

			await _uow.SaveChangesAsync();

			// Load full data để trả DTO
			var loaded = await _uow.BorrowTransactionRepository.GetTransactionWithDetailsAsync(transaction.BorrowId);
			return GetTransactionDto(loaded);
		}
		public async Task ReturnBookAsync(int borrowDetailId, ReturnBookDto dto)
		{
			var detail = await _uow.DbContext.BorrowTransactionDetails
				.Include(d => d.BorrowTransaction)
				.Include(d => d.BookCopy)
				.FirstOrDefaultAsync(d => d.BorrowDetailId == borrowDetailId);

			if (detail == null) throw new KeyNotFoundException("Không tìm thấy chi tiết mượn");

			if (detail.ReturnDate.HasValue) throw new InvalidOperationException("Sách đã được trả");

			detail.ReturnDate = dto.ReturnDate ?? DateTime.Now;
			detail.ItemStatus = dto.ItemStatus;
			detail.FineAmount = dto.FineAmount;
			detail.ConditionNote = dto.ConditionNote;

			detail.BookCopy.CirculationStatus = "Available"; // Sách đã về kho

			detail.BookCopy.PhysicalCondition = dto.ItemStatus switch
			{
				"Damaged" => "Poor",
				"Lost" => "Missing",
				_ => "Good"
			};

			var transaction = detail.BorrowTransaction;
			if (transaction.Details.All(d => d.ReturnDate.HasValue))
			{
				transaction.Status = "FullyReturned";
			}
			else if (transaction.Details.Any(d => d.ReturnDate.HasValue))
			{
				transaction.Status = "PartiallyReturned";
			}

			await _uow.SaveChangesAsync();
		}

		public async Task<IEnumerable<BorrowTransactionDto>> GetReaderBorrowHistoryAsync(int readerId)
		{
			var transactions = await _uow.BorrowTransactionRepository.GetByReaderIdAsync(readerId);
			return transactions.Select(t => GetTransactionDto(t));
		}

		public async Task<IEnumerable<BorrowTransactionDto>> GetOverdueTransactionsAsync()
		{
			var transactions = await _uow.BorrowTransactionRepository.GetOverdueTransactionsAsync();
			return transactions.Select(t => GetTransactionDto(t));
		}

		public async Task<IEnumerable<BorrowTransactionDto>> GetAllBorrowTransactionsAsync()
		{
			// 1. Tự động quét và cập nhật trạng thái "Overdue"
			var overdueDetails = await _uow.DbContext.BorrowTransactionDetails
				.Where(d => d.ItemStatus == "Borrowing" && d.DueDate < DateTime.Today)
				.ToListAsync();

			if (overdueDetails.Any())
			{
				foreach (var detail in overdueDetails)
				{
					detail.ItemStatus = "Overdue";
				}
				await _uow.SaveChangesAsync();
			}

			// 2. Load danh sách giao dịch như bình thường
			var transactions = await _uow.BorrowTransactionRepository.GetAllAsync();
			return transactions.Select(t => GetTransactionDto(t));
		}

		private BorrowTransactionDto GetTransactionDto(BorrowTransaction t)
		{
			var detailDtos = t.Details.Select(d => new BorrowTransactionDetailDto
			{
				BorrowDetailId = d.BorrowDetailId,
				CopyId = d.CopyId,
				Title = d.BookCopy?.BookEdition?.BookWork?.Title ?? "Unknown",
				DueDate = d.DueDate,
				ReturnDate = d.ReturnDate,
				ItemStatus = d.ItemStatus,
				FineAmount = d.FineAmount,
				ConditionNote = d.ConditionNote,

				// THÊM 2 DÒNG DƯỚI ĐÂY ĐỂ LẤY DỮ LIỆU TỪ DATABASE LÊN UI
				CirculationStatus = d.BookCopy?.CirculationStatus,
				PhysicalCondition = d.BookCopy?.PhysicalCondition
			}).ToList();

			return new BorrowTransactionDto
			{
				BorrowId = t.BorrowId,
				ReaderId = t.ReaderId,
				ReaderFullName = t.Reader?.FullName ?? "Unknown",
				EmployeeId = t.EmployeeId,
				EmployeeFullName = t.Employee?.FullName ?? "Unknown",
				RequestId = t.RequestId,
				BorrowDate = t.BorrowDate,
				Status = t.Status,
				Details = detailDtos
			};
		}
		public async Task DeleteBorrowTransactionAsync(int borrowId)
		{
			var transaction = await _uow.DbContext.BorrowTransactions
				.Include(t => t.Details)
				.ThenInclude(d => d.BookCopy)
				.FirstOrDefaultAsync(t => t.BorrowId == borrowId);

			if (transaction == null)
				throw new KeyNotFoundException("Không tìm thấy giao dịch");

			

			// Revert bản sao về kho
			foreach (var detail in transaction.Details)
			{
				if (detail.BookCopy != null)
				{
					detail.BookCopy.CirculationStatus = "Available";
				}
			}

			_uow.DbContext.BorrowTransactionDetails.RemoveRange(transaction.Details);
			_uow.DbContext.BorrowTransactions.Remove(transaction);

			await _uow.SaveChangesAsync();
		}
		public async Task CompleteBorrowDetailAsync(int borrowDetailId)
		{
			// Giả sử bạn đang dùng UnitOfWork có DbContext, hoặc gọi qua Repository
			var detail = await _uow.DbContext.Set<BorrowTransactionDetail>().FindAsync(borrowDetailId);

			if (detail != null && detail.ItemStatus != "Complete")
			{
				detail.ItemStatus = "Complete";
				// detail.ReturnDate = DateTime.Now; // (Bỏ comment dòng này nếu bạn muốn tự động ghi nhận ngày trả sách khi tích)

				await _uow.SaveChangesAsync();
			}
		}
		public async Task ToggleItemStatusAsync(int borrowDetailId, bool isComplete)
		{
			// 1. Lấy chi tiết mượn kèm theo thông tin BookCopy (dùng Include để lấy luôn dữ liệu liên quan)
			var detail = await _uow.DbContext.BorrowTransactionDetails
				.Include(d => d.BookCopy)
				.FirstOrDefaultAsync(d => d.BorrowDetailId == borrowDetailId);

			if (detail != null)
			{
				// 2. Cập nhật trạng thái cho chi tiết mượn
				detail.ItemStatus = isComplete ? "Complete" : "Borrowing";

				// Bạn có thể ghi nhận ngày trả nếu muốn (tùy chọn)
				detail.ReturnDate = isComplete ? DateTime.Now : null;

				// 3. Cập nhật trạng thái lưu thông cho cuốn sách (BookCopy)
				if (detail.BookCopy != null)
				{
					// Nếu tick (Hoàn thành) -> Sách sẵn sàng cho người sau (Available)
					// Nếu bỏ tick (Đang mượn) -> Sách đang bị chiếm dụng (Borrowed)
					detail.BookCopy.CirculationStatus = isComplete ? "Available" : "Borrowed";

					// Đảm bảo EF theo dõi sự thay đổi của BookCopy
					_uow.DbContext.BookCopies.Update(detail.BookCopy);
				}

				// 4. Lưu tất cả thay đổi vào Database
				await _uow.SaveChangesAsync();
			}
		}
		public async Task ExtendDueDateAsync(int borrowId, int additionalDays)
		{
			if (additionalDays <= 0)
				throw new ArgumentException("Số ngày gia hạn phải > 0");

			var transaction = await _uow.DbContext.BorrowTransactions
				.Include(t => t.Details)
				.FirstOrDefaultAsync(t => t.BorrowId == borrowId);

			if (transaction == null)
				throw new KeyNotFoundException("Không tìm thấy giao dịch");

			if (transaction.Status == "FullyReturned")
				throw new InvalidOperationException("Giao dịch đã trả hết, không thể gia hạn");

			foreach (var detail in transaction.Details.Where(d => d.ReturnDate == null))
			{
				detail.DueDate = detail.DueDate.AddDays(additionalDays);
			}

			await _uow.SaveChangesAsync();
		}
		public async Task<BorrowTransactionDto> GetBorrowTransactionByIdAsync(int borrowId)
		{
			var transaction = await _uow.BorrowTransactionRepository.GetTransactionWithDetailsAsync(borrowId);
			if (transaction == null) throw new KeyNotFoundException("Không tìm thấy giao dịch");
			return GetTransactionDto(transaction);
		}

		public async Task UpdateBorrowDetailsAsync(int borrowId, List<UpdateBorrowDetailDto> updatedDetails)
		{
			var transaction = await _uow.DbContext.BorrowTransactions
				.Include(t => t.Details)
				.ThenInclude(d => d.BookCopy)
				.FirstOrDefaultAsync(t => t.BorrowId == borrowId);

			if (transaction == null)
				throw new KeyNotFoundException("Không tìm thấy giao dịch");

			bool hasChanges = false;

			foreach (var update in updatedDetails)
			{
				var detail = transaction.Details.FirstOrDefault(d => d.BorrowDetailId == update.BorrowDetailId);
				if (detail == null) continue;

				// Cập nhật chi tiết mượn
				detail.DueDate = update.DueDate;
				_uow.DbContext.BorrowTransactionDetails.Update(detail); // Ép EF theo dõi thay đổi

				// Cập nhật BookCopy
				if (detail.BookCopy != null)
				{
					if (!string.IsNullOrEmpty(update.CirculationStatus) && IsValidCirculationStatus(update.CirculationStatus))
					{
						detail.BookCopy.CirculationStatus = update.CirculationStatus;
					}

					if (!string.IsNullOrEmpty(update.PhysicalCondition))
					{
						detail.BookCopy.PhysicalCondition = update.PhysicalCondition;
					}

					_uow.DbContext.BookCopies.Update(detail.BookCopy); // Ép EF theo dõi BookCopy
				}

				hasChanges = true;
			}

			if (hasChanges)
			{
				await _uow.SaveChangesAsync();
			}
		}

		// Helper method (thêm vào class BorrowService)
		private bool IsValidCirculationStatus(string status)
		{
			var valid = new[] { "Available", "Borrowed", "On Hold", "In Repair", "Withdrawn", "Lost", "Damaged" };
			return valid.Contains(status, StringComparer.OrdinalIgnoreCase);
		}
	}
}
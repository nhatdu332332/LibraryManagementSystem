using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagementSystem.Services.DTOs;

namespace LibraryManagementSystem.Services.Interfaces
{
	public interface IBorrowService
	{
		Task<BorrowRequestDto> SubmitBorrowRequestAsync(int readerId, SubmitBorrowRequestDto dto);
		Task<IEnumerable<BorrowRequestDto>> GetPendingRequestsAsync();
		Task ApproveBorrowRequestAsync(int requestId, int employeeId, ApproveBorrowRequestDto dto);
		Task RejectBorrowRequestAsync(int requestId, string reason);

		Task<BorrowTransactionDto> CreateBorrowTransactionAsync(int requestId, int employeeId, List<int> copyIds);
		Task ReturnBookAsync(int borrowDetailId, ReturnBookDto dto);
		Task<IEnumerable<BorrowTransactionDto>> GetReaderBorrowHistoryAsync(int readerId);
		Task<IEnumerable<BorrowTransactionDto>> GetOverdueTransactionsAsync();
		Task CompleteBorrowDetailAsync(int borrowDetailId);

		// Thêm method này để BorrowViewModel gọi được
		Task<IEnumerable<BorrowTransactionDto>> GetAllBorrowTransactionsAsync();
		// Thêm vào cuối interface IBorrowService.cs
		Task<BorrowTransactionDto> CreateDirectBorrowTransactionAsync(
			int readerId,
			int employeeId,
			List<int> copyIds,
			DateTime borrowDate,
			int durationDays);
		Task DeleteBorrowTransactionAsync(int borrowId);
		Task ExtendDueDateAsync(int borrowId, int additionalDays);
		Task<BorrowTransactionDto> GetBorrowTransactionByIdAsync(int borrowId);
		Task UpdateBorrowDetailsAsync(int borrowId, List<UpdateBorrowDetailDto> updatedDetails);
		Task ToggleItemStatusAsync(int borrowDetailId, bool isComplete);
	}
}
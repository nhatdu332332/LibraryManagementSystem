USE LibraryManagementDB;
GO

-- 1. Role (chú ý: đúng 3 role như yêu cầu)
INSERT INTO Role (RoleName) VALUES 
(N'Administrator'),   -- Manages system accounts, roles, and access control
(N'Staff'),           -- Maintains book and catalog information
(N'Librarian');       -- Handles borrowing, returning, and approval workflows
GO

-- 2. Employee (mật khẩu hash giả lập - trong thực tế nên dùng bcrypt/argon2)
INSERT INTO Employee (Email, PasswordHash, FullName, RoleId, HireDate, Status) VALUES
(N'admin@library.vn', N'$2a$10$fakehashadmin123', N'Nguyễn Văn Admin', 1, '2023-01-15', N'Active'),
(N'staff1@library.vn', N'$2a$10$fakehashstaff456', N'Trần Thị Staff', 2, '2023-06-01', N'Active'),
(N'librarian1@library.vn', N'$2a$10$fakehashlib789', N'Lê Văn Librarian', 3, '2024-02-10', N'Active'),
(N'librarian2@library.vn', N'$2a$10$fakehashlibabc', N'Phạm Thị Lib2', 3, '2024-05-20', N'Active');
GO

-- 3. Reader (một số độc giả mẫu)
INSERT INTO Reader (CardNumber, Email, PasswordHash, FullName, PhoneNumber, Address, RegisterDate, ExpiredDate, ReaderStatus) VALUES
(N'RD000001', N'reader1@gmail.com', N'$2a$10$fakehashrd1', N'Phạm Nhật Dương', N'0905123456', N'Vinh, Nghệ An', '2024-01-10', '2026-01-10', N'Active'),
(N'RD000002', N'nguyen.van@example.com', N'$2a$10$fakehashrd2', N'Nguyễn Văn A', N'0918234567', N'Hà Nội', '2023-11-05', '2025-11-05', N'Active'),
(N'RD000003', N'tranthi.b@example.com', N'$2a$10$fakehashrd3', N'Trần Thị B', NULL, N'Đà Nẵng', '2025-03-01', '2027-03-01', N'Active'),
(N'RD000004', N'levanc@example.vn', N'$2a$10$fakehashrd4', N'Lê Văn C', N'0987654321', N'TP.HCM', '2024-06-15', '2026-06-15', N'Active');
GO

-- 4. Series (một số bộ sách, truyện ngắn thì để riêng lẻ)
INSERT INTO Series (SeriesName, Description) VALUES
(N'Limbus Company Identities', N'Các nhân vật lấy cảm hứng từ tác phẩm kinh điển trong game Limbus Company'),
(N'None - Standalone', N'Các tác phẩm độc lập không thuộc series');
GO

-- 5. Author (tác giả của 13 cuốn)
INSERT INTO Author (AuthorName, Note) VALUES
(N'Yi Sang', N'Nhà văn Hàn Quốc hiện đại'),
(N'Johann Wolfgang von Goethe', N'Nhà văn Đức vĩ đại'),
(N'Miguel de Cervantes', N'Nhà văn Tây Ban Nha, cha đẻ tiểu thuyết hiện đại'),
(N'Ryūnosuke Akutagawa', N'Nhà văn Nhật Bản nổi tiếng với truyện ngắn'),
(N'Albert Camus', N'Nhà văn Pháp, triết gia hiện sinh'),
(N'Cao Xueqin', N'Nhà văn Trung Quốc thời Thanh'),
(N'Emily Brontë', N'Nhà văn Anh, chị em Brontë'),
(N'Herman Melville', N'Nhà văn Mỹ'),
(N'Fyodor Dostoyevsky', N'Nhà văn Nga vĩ đại'),
(N'Dante Alighieri', N'Nhà thơ Ý thời Trung cổ'),
(N'Hermann Hesse', N'Nhà văn Đức, Nobel Văn học'),
(N'Homer', N'Nhà thơ Hy Lạp cổ đại'),
(N'Franz Kafka', N'Nhà văn Séc gốc Do Thái, văn học hiện sinh');
GO

-- 6. Category (một số thể loại chính)
INSERT INTO Category (CategoryName, Description) VALUES
(N'Classic Literature', N'Tác phẩm kinh điển thế giới'),
(N'Philosophical Fiction', N'Tiểu thuyết mang tính triết học'),
(N'Existentialism', N'Chủ nghĩa hiện sinh'),
(N'Korean Literature', N'Văn học Hàn Quốc'),
(N'Japanese Literature', N'Văn học Nhật Bản'),
(N'Russian Literature', N'Văn học Nga'),
(N'Epic Poetry', N'Thơ sử thi');
GO

-- 7. BookWork + liên kết Author, Category, Series
-- Giả sử VolumeNumber = 1 cho hầu hết (truyện ngắn/tiểu thuyết độc lập)
DECLARE @SeriesStandalone INT = (SELECT SeriesId FROM Series WHERE SeriesName = N'None - Standalone');
DECLARE @SeriesLimbus INT = (SELECT SeriesId FROM Series WHERE SeriesName = N'Limbus Company Identities');

INSERT INTO BookWork (Title, OriginalTitle, Summary, FirstPublishYear, SeriesId, VolumeNumber) VALUES
(N'The Wings + Crow''s Eye View', N'날개 (Nalgae)', N'Tác phẩm kinh điển của Yi Sang về sự cô lập và ảo tưởng.', 1936, @SeriesLimbus, 1),
(N'Faust', N'Faust', N'Tác phẩm bi kịch về giao kèo với quỷ dữ.', 1808, @SeriesStandalone, 1),
(N'Don Quixote', N'El ingenioso hidalgo Don Quijote de la Mancha', N'Hiệp sĩ lãng mạn chống lại cối xay gió.', 1605, @SeriesStandalone, 1),
(N'Hell Screen', N'地獄変 (Jigokuhen)', N'Nghệ sĩ điên cuồng vì nghệ thuật.', 1918, @SeriesLimbus, 1),
(N'The Stranger', N'L''Étranger', N'Câu chuyện về sự vô cảm và phi lý.', 1942, @SeriesStandalone, 1),
(N'Dream of the Red Chamber', N'紅樓夢 (Hồng Lâu Mộng)', N'Tiểu thuyết cổ điển Trung Quốc về gia tộc suy tàn.', 1791, @SeriesStandalone, 1),
(N'Wuthering Heights', N'Wuthering Heights', N'Tình yêu cuồng nhiệt và trả thù.', 1847, @SeriesStandalone, 1),
(N'Moby-Dick', N'Moby-Dick; or, The Whale', N'Cuộc săn đuổi cá voi trắng ám ảnh.', 1851, @SeriesStandalone, 1),
(N'Crime and Punishment', N'Преступление и наказание', N'Tội ác và lương tâm giày vò.', 1866, @SeriesStandalone, 1),
(N'The Divine Comedy', N'Divina Commedia', N'Hành trình qua Địa ngục, Luyện ngục, Thiên đường.', 1320, @SeriesStandalone, 1),
(N'Demian', N'Demian: Die Geschichte von Emil Sinclairs Jugend', N'Hành trình trưởng thành và khám phá bản thân.', 1919, @SeriesStandalone, 1),
(N'The Odyssey', N'Ὀδύσσεια (Odýsseia)', N'Hành trình trở về của Odysseus.', -800, @SeriesStandalone, 1),  -- năm ước lượng
(N'Metamorphosis', N'Die Verwandlung', N'Gregor tỉnh dậy thành con bọ.', 1915, @SeriesStandalone, 1);
GO

-- Liên kết WorkAuthor (mỗi WorkId với AuthorId tương ứng - giả sử WorkId bắt đầu từ 1)
INSERT INTO WorkAuthor (WorkId, AuthorId) VALUES
(1,1), (2,2), (3,3), (4,4), (5,5), (6,6), (7,7), (8,8), (9,9), (10,10),
(11,11), (12,12), (13,13);
GO

-- WorkCategory (gán vài category cho mỗi work)
INSERT INTO WorkCategory (WorkId, CategoryId) VALUES
(1,1),(1,4),
(2,1),(2,2),
(3,1),
(4,1),(4,5),
(5,1),(5,3),
(6,1),
(7,1),
(8,1),
(9,1),(9,2),(9,6),
(10,1),(10,7),
(11,1),(11,2),
(12,1),(12,7),
(13,1),(13,3);
GO

-- Publisher (một số nhà xuất bản mẫu)
INSERT INTO Publisher (PublisherName, Address, Note) VALUES
(N'Penguin Classics', N'London, UK', N'Nhà xuất bản sách kinh điển'),
(N'Gallimard', N'Paris, France', NULL),
(N'Jimoondang Publishing', N'Seoul, Korea', N'Xuất bản văn học Hàn Quốc'),
(N'Juan de la Cuesta', N'Madrid, Spain', N'Nhà xuất bản Don Quixote gốc'),
(N'Random House', N'New York, USA', NULL);
GO

-- BookEdition (mỗi BookWork có 1–2 edition)
INSERT INTO BookEdition (WorkId, ISBN, PublisherId, PublishYear, EditionNumber, Language, PageCount, Format) VALUES
(1, N'9788988095508', 3, 2001, 1, N'English', 84, N'Paperback'),
(2, N'9780140449020', 1, 2007, NULL, N'English', 304, N'Paperback'),
(3, N'9780142437230', 1, 2003, NULL, N'English', 1024, N'Paperback'),
(4, N'9780241573693', 1, 2025, 1, N'English', 80, N'Hardcover'),
(5, N'9780679720201', 5, 1989, NULL, N'English', 123, N'Paperback'),
(6, N'9780140449266', 1, 2006, NULL, N'English', 1200, N'Paperback'),
(7, N'9780141439556', 1, 2003, NULL, N'English', 416, N'Paperback'),
(8, N'9780142437247', 1, 2003, NULL, N'English', 720, N'Paperback'),
(9, N'9780140449136', 1, 2003, NULL, N'English', 671, N'Paperback'),
(10,N'9780140448955', 1, 2003, NULL, N'English', 928, N'Paperback'),
(11,N'9780140186475', 1, 1995, NULL, N'English', 256, N'Paperback'),
(12,N'9780140268867', 1, 1997, NULL, N'English', 416, N'Paperback'),
(13,N'9780141182674', 1, 2000, NULL, N'English', 55, N'Paperback');
GO

-- BookCopy (mỗi Edition có 2–4 bản sao)
INSERT INTO BookCopy (EditionId, Barcode, CirculationStatus, PhysicalCondition, ShelfLocation, AddedDate, RemovedDate) VALUES
(1, N'BC000001', N'Available', N'Good', N'A-12', '2024-01-01', NULL),
(1, N'BC000002', N'Borrowed', N'Good', N'A-12', '2024-01-01', NULL),
(2, N'BC000003', N'Available', N'Excellent', N'B-05', '2024-02-01', NULL),
(3, N'BC000004', N'Available', N'Good', N'C-08', '2024-03-01', NULL),
(5, N'BC000005', N'Borrowed', N'Fair', N'D-03', '2024-04-01', NULL),
(13,N'BC000006', N'Available', N'Good', N'E-15', '2025-01-10', NULL);
-- thêm tương tự cho các edition khác nếu cần...
GO

-- BorrowRequest + Detail (ví dụ vài yêu cầu mượn)
INSERT INTO BorrowRequest (ReaderId, RequestDate, ApprovedByEmployeeId, ApprovedDate, RejectReason, Status) VALUES
(1, '2025-12-01 10:30:00', 3, '2025-12-02 09:15:00', NULL, N'Approved'),
(2, '2026-01-15 14:00:00', NULL, NULL, N'Out of stock', N'Rejected'),
(1, '2026-02-01 11:45:00', 4, '2026-02-01 15:20:00', NULL, N'Approved');
GO

INSERT INTO BorrowRequestDetail (RequestId, WorkId, RequestedQuantity, ApprovedQuantity) VALUES
(1, 5, 1, 1),   -- mượn The Stranger
(1, 13,1, 1),   -- mượn Metamorphosis
(2, 3, 1, 0),
(3, 1, 1, 1);
GO

-- BorrowTransaction + Detail (ví dụ vài giao dịch đã xảy ra)
INSERT INTO BorrowTransaction (ReaderId, EmployeeId, RequestId, BorrowDate, Status) VALUES
(1, 3, 1, '2025-12-03 10:00:00', N'Borrowed'),
(1, 4, 3, '2026-02-02 13:30:00', N'Borrowed');
GO

INSERT INTO BorrowTransactionDetail (BorrowId, CopyId, DueDate, ReturnDate, ItemStatus, FineAmount, ConditionNote) VALUES
(1, 5, '2026-01-03', NULL, N'Borrowed', 0.00, NULL),
(2, 1, '2026-03-02', '2026-02-10', N'Returned', 0.00, N'Good condition');
GO

-- Kết thúc - bạn có thể SELECT để kiểm tra
-- Ví dụ: SELECT * FROM BookWork JOIN WorkAuthor ON ... để xem sách
-- Reset mật khẩu cho admin thành 'admin123' (plain text)
UPDATE Employee
SET PasswordHash = 'admin123'
WHERE Email = 'admin@library.vn';

-- Reset cho staff1
UPDATE Employee
SET PasswordHash = 'staff456'
WHERE Email = 'staff1@library.vn';

-- Reset cho librarian1
UPDATE Employee
SET PasswordHash = 'lib789'
WHERE Email = 'librarian1@library.vn';

-- Reset cho librarian2
UPDATE Employee
SET PasswordHash = 'libabc'
WHERE Email = 'librarian2@library.vn';

-- Reset mật khẩu cho Reader 1 thành 'reader123' (plain text)
UPDATE Reader
SET PasswordHash = 'reader123'
WHERE ReaderId = 1;  -- Phạm Nhật Dương

-- Reset cho Reader 2
UPDATE Reader
SET PasswordHash = '123456'
WHERE ReaderId = 2;  -- Nguyễn Văn A

-- Reset cho Reader 3
UPDATE Reader
SET PasswordHash = 'abc789'
WHERE ReaderId = 3;  -- Trần Thị B

-- Reset cho Reader 4
UPDATE Reader
SET PasswordHash = '456abc'
WHERE ReaderId = 4;  -- Lê Văn C
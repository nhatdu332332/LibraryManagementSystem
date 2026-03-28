USE master;
GO

IF DB_ID(N'LibraryManagementDB') IS NOT NULL
BEGIN
    ALTER DATABASE LibraryManagementDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE LibraryManagementDB;
END
GO

CREATE DATABASE LibraryManagementDB;
GO

USE LibraryManagementDB;
GO

CREATE TABLE Role (
    RoleId INT IDENTITY PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Employee (
    EmployeeId INT IDENTITY PRIMARY KEY,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    RoleId INT NOT NULL,
    HireDate DATE NOT NULL,
    Status NVARCHAR(20) NOT NULL,

    CONSTRAINT FK_Employee_Role
        FOREIGN KEY (RoleId) REFERENCES Role(RoleId)
);

CREATE TABLE Reader (
    ReaderId INT IDENTITY PRIMARY KEY,
    CardNumber NVARCHAR(30) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20),
    Address NVARCHAR(200),
    RegisterDate DATE NOT NULL,
    ExpiredDate DATE NOT NULL,
    ReaderStatus NVARCHAR(20) NOT NULL
);

CREATE TABLE BookWork (
    WorkId INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(300) NOT NULL,
    OriginalTitle NVARCHAR(300),
    Summary NVARCHAR(MAX),
    FirstPublishYear INT
);

CREATE TABLE Author (
    AuthorId INT IDENTITY PRIMARY KEY,
    AuthorName NVARCHAR(150) NOT NULL,
    Note NVARCHAR(500)
);

CREATE TABLE WorkAuthor (
    WorkId INT NOT NULL,
    AuthorId INT NOT NULL,
    PRIMARY KEY (WorkId, AuthorId),

    FOREIGN KEY (WorkId) REFERENCES BookWork(WorkId),
    FOREIGN KEY (AuthorId) REFERENCES Author(AuthorId)
);

CREATE TABLE Category (
    CategoryId INT IDENTITY PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500)
);

CREATE TABLE WorkCategory (
    WorkId INT NOT NULL,
    CategoryId INT NOT NULL,
    PRIMARY KEY (WorkId, CategoryId),

    FOREIGN KEY (WorkId) REFERENCES BookWork(WorkId),
    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId)
);

CREATE TABLE Publisher (
    PublisherId INT IDENTITY PRIMARY KEY,
    PublisherName NVARCHAR(200) NOT NULL,
    Address NVARCHAR(300),
    Note NVARCHAR(500)
);

CREATE TABLE BookEdition (
    EditionId INT IDENTITY PRIMARY KEY,
    WorkId INT NOT NULL,
    ISBN NVARCHAR(20) NOT NULL UNIQUE,
    PublisherId INT NOT NULL,
    PublishYear INT NOT NULL,
    EditionNumber INT,
    Language NVARCHAR(50),
    PageCount INT,
    Format NVARCHAR(50),

    FOREIGN KEY (WorkId) REFERENCES BookWork(WorkId),
    FOREIGN KEY (PublisherId) REFERENCES Publisher(PublisherId)
);

CREATE TABLE BookCopy (
    CopyId INT IDENTITY PRIMARY KEY,
    EditionId INT NOT NULL,
    Barcode NVARCHAR(50) NOT NULL UNIQUE,
    CirculationStatus NVARCHAR(20) NOT NULL,
    PhysicalCondition NVARCHAR(20) NOT NULL,
    ShelfLocation NVARCHAR(50),
    AddedDate DATE NOT NULL,
    RemovedDate DATE,

    FOREIGN KEY (EditionId) REFERENCES BookEdition(EditionId)
);

CREATE TABLE BorrowRequest (
    RequestId INT IDENTITY PRIMARY KEY,
    ReaderId INT NOT NULL,
    RequestDate DATETIME NOT NULL,
    ApprovedByEmployeeId INT,
    ApprovedDate DATETIME,
    RejectReason NVARCHAR(500),
    Status NVARCHAR(20) NOT NULL,

    FOREIGN KEY (ReaderId) REFERENCES Reader(ReaderId),
    FOREIGN KEY (ApprovedByEmployeeId) REFERENCES Employee(EmployeeId)
);

CREATE TABLE BorrowRequestDetail (
    RequestDetailId INT IDENTITY PRIMARY KEY,
    RequestId INT NOT NULL,
    WorkId INT NOT NULL,
    RequestedQuantity INT NOT NULL,
    ApprovedQuantity INT,

    FOREIGN KEY (RequestId) REFERENCES BorrowRequest(RequestId),
    FOREIGN KEY (WorkId) REFERENCES BookWork(WorkId)
);

CREATE TABLE BorrowTransaction (
    BorrowId INT IDENTITY PRIMARY KEY,
    ReaderId INT NOT NULL,
    EmployeeId INT NOT NULL,
    RequestId INT,
    BorrowDate DATETIME NOT NULL,
    Status NVARCHAR(20) NOT NULL,

    FOREIGN KEY (ReaderId) REFERENCES Reader(ReaderId),
    FOREIGN KEY (EmployeeId) REFERENCES Employee(EmployeeId),
    FOREIGN KEY (RequestId) REFERENCES BorrowRequest(RequestId)
);

CREATE TABLE BorrowTransactionDetail (
    BorrowDetailId INT IDENTITY PRIMARY KEY,
    BorrowId INT NOT NULL,
    CopyId INT NOT NULL,
    DueDate DATE NOT NULL,
    ReturnDate DATE,
    ItemStatus NVARCHAR(20) NOT NULL,
    FineAmount DECIMAL(10,2) DEFAULT 0,
    ConditionNote NVARCHAR(500),

    FOREIGN KEY (BorrowId) REFERENCES BorrowTransaction(BorrowId),
    FOREIGN KEY (CopyId) REFERENCES BookCopy(CopyId)
);

-- 1. Tạo bảng Series
CREATE TABLE Series (
    SeriesId INT IDENTITY PRIMARY KEY,
    SeriesName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500)
);
GO


-- 2. Thêm cột vào BookWork
ALTER TABLE BookWork
ADD SeriesId INT NOT NULL,
    VolumeNumber INT NOT NULL;
GO


-- 3. Tạo quan hệ 1-N (Series -> BookWork)
ALTER TABLE BookWork
ADD CONSTRAINT FK_BookWork_Series
FOREIGN KEY (SeriesId)
REFERENCES Series(SeriesId);
GO

ALTER TABLE Role
ADD Description NVARCHAR(MAX) NULL;
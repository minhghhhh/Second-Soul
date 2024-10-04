use master
Go
IF EXISTS (SELECT * FROM sys.databases WHERE name = N'SecondSoulShop' AND database_id > 4)
BEGIN
    ALTER DATABASE SecondSoulShop SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SecondSoulShop;
    PRINT 'SecondSoulShop has been deleted.';
END
Go
Create Database SecondSoulShop;
Go
USE SecondSoulShop;
Go
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PhoneNumber NVARCHAR(15) NULL,
    Address NVARCHAR(255) NULL,
    Role NVARCHAR(20) CHECK (Role IN ('Customer','Admin')), 
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);

CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(50) NOT NULL,
    ParentCategoryID INT NULL,
    FOREIGN KEY (ParentCategoryID) REFERENCES Categories(CategoryID)
);

CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    SellerID INT,
    Name NVARCHAR(100) NOT NULL,
    Description TEXT,
    CategoryID INT NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    Quantity INT DEFAULT 0,
    Condition NVARCHAR(20) CHECK (Condition IN ('New', 'Like_New', 'Good', 'Fair')),
    AddedDate DATETIME DEFAULT GETDATE(),
    IsAvailable BIT DEFAULT 1,
    FOREIGN KEY (SellerID) REFERENCES Users(UserID),
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);
CREATE TABLE ProductImages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ImageUrl NVARCHAR(255) NOT NULL,
    PublicId NVARCHAR(255),
    ProductId INT NOT NULL,
    FOREIGN KEY (ProductId) REFERENCES Products(ProductID) ON DELETE CASCADE
);
CREATE TABLE Coupons (
    CouponID INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(50) UNIQUE NOT NULL,
    DiscountPercentage DECIMAL(4, 2) DEFAULT 0, 
    MaxDiscount INT DEFAULT 0,             
    ExpiryDate DATETIME,
    IsActive BIT DEFAULT 1,
    MinSpendAmount INT DEFAULT 0
);

CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT,
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18, 2) NOT NULL,
    Status NVARCHAR(20) CHECK (Status IN ('Pending', 'Shipped', 'Delivered', 'Cancelled', 'Returned')),
    CouponID INT NULL,
	    PhoneNumber NVARCHAR(15) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    FOREIGN KEY (CustomerID) REFERENCES Users(UserID),
    FOREIGN KEY (CouponID) REFERENCES Coupons(CouponID)
);

CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT,
    ProductID INT,
    Quantity INT NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);


CREATE TABLE FavoriteShops (
    UserID INT,
    ShopID INT, 
    AddedDate DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (UserID,ShopID),  
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ShopID) REFERENCES Users(UserID)
);

CREATE TABLE Reviews (
    ReviewID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT NOT NULL,
    UserID INT NOT NULL,
    Rating INT CHECK (Rating BETWEEN 1 AND 5) NOT NULL,
    Comment TEXT,
    ReviewDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE ShoppingCart (
     UserID INT,
    ProductID INT,
    Quantity INT NOT NULL,
    AddedDate DATETIME DEFAULT GETDATE(),
	PRIMARY KEY(UserID, ProductID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

CREATE TABLE Payments (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    PaymentDate DATETIME DEFAULT GETDATE(),
    Amount DECIMAL(18, 2) NOT NULL,
    PaymentMethod NVARCHAR(50) CHECK (PaymentMethod IN ('COD', 'Banking')),
    Status NVARCHAR(20) CHECK (Status IN ('Pending', 'Completed', 'Failed')),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
);

CREATE TABLE Messages (
    MessageID INT IDENTITY(1,1) PRIMARY KEY,
    SenderID INT NOT NULL,
    ReceiverID INT NOT NULL,
    Subject NVARCHAR(100),
    MessageBody TEXT NOT NULL,
    SentDate DATETIME DEFAULT GETDATE(),
    IsRead BIT DEFAULT 0 NOT NULL,
    FOREIGN KEY (SenderID) REFERENCES Users(UserID),
    FOREIGN KEY (ReceiverID) REFERENCES Users(UserID)
);
Go
INSERT INTO [SecondSoulShop].[dbo].[Categories] 
    ([CategoryName], [ParentCategoryID])
VALUES
    ( 'Topwear', NULL),        
    ( 'Bottomwear', Null), 
	('Footwear',Null),
	( 'Headwear', Null),
	('Full-set',null),
	('Accessories',null),
    ( 'Jackets', 1),              
    ( 'T-shirts', 1),             
    ( 'Dresses', 5),              
    ('Blouses', 1),              
    ( 'TankTop',1),       
    ( 'Skirt', 2),    
	( 'Pants', 2),
    ( 'Short', 2), 
	( 'Croptop',1),
	('Vest',1),
	('Sneakers',3),
	('Loafers',3),
	('High-heels',3),
	('Boots',3),
	('Sandals',3),
	('Hand Bag',6),
	('Backpack',6),
	('Gloves',6),
	('Scarf',6);
Go
INSERT INTO Coupons (Code, DiscountPercentage, MaxDiscount, ExpiryDate, IsActive, MinSpendAmount)
VALUES
    ('SUMMER2024', 15.00, 50000, '2024-07-31', 1, 0),         -- Giảm 15%, tối đa 50,000 VND
    ('NEWUSER', 20.00, 100000, '2024-11-30', 1, 0),           -- Giảm 20%, tối đa 100,000 VND
    ('FLASHSALE', 10.00, 25000, '2024-10-15', 1, 0),          -- Giảm 10%, tối đa 25,000 VND
    ('EXPIREDCOUPON', 5.00, 10000, '2023-12-31', 0, 0),       -- Giảm 5%, tối đa 10,000 VND, đã hết hạn
    ('BIGSPENDER', 20.00, 200000, '2024-12-31', 1, 1000000);   -- Giảm 10%, tối đa 100,000 VND, yêu cầu chi tiêu 500,000 VND

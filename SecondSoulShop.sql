﻿use master
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
	FullName NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
	Wallet int Default 0,
	Bank NVARCHAR(50) Null,
	Bankinfo Nvarchar(255) Null, 
	Bankuser Nvarchar(100) Null,
    PhoneNumber NVARCHAR(15) NULL,
	Token Nvarchar(255) Null,
	ImageUrl NVARCHAR(255) Null,
    Address NVARCHAR(255) NULL,
    Role NVARCHAR(20) CHECK (Role IN ('Customer','Admin')), 
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
--	MoMoPhoneNumber NVARCHAR(20) NULL,  -- Số điện thoại ví MoMo
--	MoMoLinked BIT NOT NULL DEFAULT 0;  -- Trạng thái liên kết (0 = chưa liên kết, 1 = đã liên kết)
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
    Price INT NOT NULL,
	Size nvarchar(5) CHECK (Size IN ('XS','S', 'M', 'L', 'XL','twoXl','Other')),
    Condition NVARCHAR(20) CHECK (Condition IN ('New', 'Like_New', 'Good', 'Fair')),
    AddedDate DATETIME DEFAULT GETDATE(),
	MainImage NVARCHAR(255) NOT NULL,
    IsAvailable BIT DEFAULT 1,
	IsSale BIT DEFAULT 0,
	IsNew BIT DEFAULT 1,
	IsReview Bit Default 0,
	SalePrice INT NULL,
    FOREIGN KEY (SellerID) REFERENCES Users(UserID),
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);
CREATE TABLE ProductImages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ImageUrl NVARCHAR(255) NOT NULL,
    ProductId INT NOT NULL,
    FOREIGN KEY (ProductId) REFERENCES Products(ProductID) ON DELETE CASCADE
);
CREATE TABLE Coupons (
    CouponID INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(50) UNIQUE NOT NULL,
    DiscountPercentage Int default 0,
    MaxDiscount INT DEFAULT 0,             
    ExpiryDate DATETIME,
    IsActive BIT DEFAULT 1,
    MinSpendAmount INT DEFAULT 0
);

CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT,
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount INT NOT NULL,
    Status NVARCHAR(20) CHECK (Status IN ('Pending','Refunded','Payed', 'Shipped', 'Completed', 'Cancelled', 'Returned')),
    CouponID INT NULL,
	FullName NVARCHAR(50) Not null,
	Descriptions NVARCHAR(100) null,
	PhoneNumber NVARCHAR(15) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    FOREIGN KEY (CustomerID) REFERENCES Users(UserID),
    FOREIGN KEY (CouponID) REFERENCES Coupons(CouponID)
);

CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT,
    ProductID INT,
    Price INT NOT NULL,
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

CREATE TABLE ShoppingCarts (
    UserID INT,
    ProductID INT,
    AddedDate DATETIME DEFAULT GETDATE(),
	PRIMARY KEY(UserID, ProductID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

CREATE TABLE Payments (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    PaymentDate DATETIME DEFAULT GETDATE(),
    Amount INT NOT NULL,
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

-- Password is 1
Go
INSERT INTO [SecondSoulShop].[dbo].[Users]  (Username, FullName, PasswordHash, Email, PhoneNumber, [Address], [Role])
VALUES 
('customer1','Khach hang 1', '6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b', 'customer1@example.com', '1234567890', '123 Customer St, City', 'Customer'),
('admin1','Admin1' ,'6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b', 'admin1@example.com', '0987654321', '456 Admin Ave, City', 'Admin'),
('customer2','Khach hang 2' ,'6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b', 'customer2@example.com', NULL, NULL, 'Customer'),
('admin2','Admin2','6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b', 'admin2@example.com', NULL, NULL, 'Admin');

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
INSERT INTO Products (SellerID, Name, Description, MainImage, CategoryID, Price, Size, Condition)
VALUES
    (1, 'Casual T-Shirt', 'Comfortable cotton t-shirt for everyday wear.', 'http://pluspng.com/img-png/khaki-pants-png--1024.png', 8, 2000, 'L', 'New'),
    (1, 'Denim Jeans', 'Stylish denim jeans, perfect for casual outings.', 'https://www.pngmart.com/files/16/Denim-Jeans-PNG-Transparent-Image.png', 12, 39000, 'M', 'Like_New'),
    (1, 'Leather Jacket', 'Classic leather jacket for a rugged look.', 'https://pngimg.com/uploads/leather_jacket/leather_jacket_PNG24.png', 7, 129000, 'L', 'Good'),
    (3, 'Summer Dress', 'Light and breezy summer dress for warm days.', 'https://pngimg.com/uploads/dress/dress_PNG90.png', 11, 49000, 'XL', 'New'),
    (3, 'Sport Sneakers', 'Durable sneakers for your active lifestyle.', 'https://freepngimg.com/thumb/shoes/28530-3-nike-shoes-transparent.png', 15, 59000, 'Other', 'New'),
    (3, 'Wool Scarf', 'Warm wool scarf to keep you cozy in winter.', 'https://pngimg.com/uploads/scarf/scarf_PNG17.png', 24, 29000, 'Other', 'Fair'),
    (3, 'Formal Blouse', 'Elegant blouse suitable for office wear.', 'http://pluspng.com/img-png/blouse-png-hd-black-dress-shirt-png-image-950.png', 10, 34000, 'M', 'Good'),
    (3, 'Chino Pants', 'Comfortable chino pants for formal occasions.', 'http://pluspng.com/img-png/khaki-pants-png--1024.png', 13, 44000, 'L', 'Like_New'),
    (3, 'Winter Boots', 'Insulated boots for snowy days.', 'https://www.pngall.com/wp-content/uploads/7/Black-Winter-Boot-PNG-Free-Image.png', 18, 89000, 'L', 'New'),
    (1, 'Graphic Croptop', 'Trendy graphic croptop for a casual look.', 'http://purepng.com/public/uploads/large/purepng.com-black-t-shirtclothingblack-t-shirtfashion-dress-shirt-black-cloth-tshirt-631522326884bzr0p.png', 9, 24000, 'S', 'New'),
    (1, 'Canvas Backpack', 'Spacious and stylish canvas backpack.', 'http://pluspng.com/img-png/blouse-png-hd-black-dress-shirt-png-image-950.png', 23, 39000, 'Other', 'Good'),
    (1, 'Leather Loafers', 'Sophisticated loafers for a polished look.', 'https://pngimg.com/uploads/leather_jacket/leather_jacket_PNG24.png', 16, 69000, 'L', 'Like_New'),
    (1, 'Sporty Tank Top', 'Breathable tank top for workouts.', 'http://purepng.com/public/uploads/large/purepng.com-black-t-shirtclothingblack-t-shirtfashion-dress-shirt-black-cloth-tshirt-631522326884bzr0p.png', 9, 21000, 'M', 'New'),
    (3, 'Elegant High-Heels', 'Stylish high-heels for special occasions.', 'https://www.pngall.com/wp-content/uploads/7/Black-Winter-Boot-PNG-Free-Image.png', 17, 74000, 'M', 'New'),
    (3, 'Trendy Skirt', 'Fashionable skirt that pairs well with tops.', 'https://freepngimg.com/thumb/shoes/28530-3-nike-shoes-transparent.png', 14, 49000, 'M', 'Good');
Go
INSERT INTO ShoppingCarts (UserID, ProductID)
VALUES
    (1, 1), -- customer1 adds 2 Casual T-Shirts
    (1, 2), -- customer1 adds 1 Denim Jeans
    (2, 4), -- customer2 adds 1 Summer Dress
    (2, 6), -- customer2 adds 3 Wool Scarves
    (1, 10), -- customer1 adds 1 Canvas Backpack
    (2, 5), -- customer2 adds 2 Sport Sneakers
    (1, 12), -- customer1 adds 1 Sporty Tank Top
    (2, 9); -- customer2 adds 1 Elegant High-Heels
Go
INSERT INTO Coupons (Code, DiscountPercentage, MaxDiscount, ExpiryDate, IsActive, MinSpendAmount)
VALUES
    ('SUMMER2024', 15, 50000, '2024-07-31', 1, 0),         -- Giảm 15%, tối đa 50,000 VND
    ('NEWUSER', 20, 100000, '2024-11-30', 1, 0),           -- Giảm 20%, tối đa 100,000 VND
    ('FLASHSALE', 10, 25000, '2024-10-15', 1, 0),          -- Giảm 10%, tối đa 25,000 VND
    ('EXPIREDCOUPON', 5, 10000, '2023-12-31', 0, 0),       -- Giảm 5%, tối đa 10,000 VND, đã hết hạn
    ('BIGSPENDER', 20, 200000, '2024-12-31', 1, 1000000);   -- Giảm 10%, tối đa 100,000 VND, yêu cầu chi tiêu 500,000 VND

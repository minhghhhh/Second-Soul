use master
GO
ALTER DATABASE SecondSoulShop SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
Go
DROP DATABASE SecondSoulShop;
Go
Create database SecondSoulShop
GO
USE SecondSoulShop;
Go
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PhoneNumber NVARCHAR(15) NULL,
    Address NVARCHAR(255) NULL,
    Role NVARCHAR(20) CHECK (Role IN ('Customer', 'Admin')), 
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
    CategoryID INT,
    Price DECIMAL(18, 2) NOT NULL,
    Quantity INT DEFAULT 0,
    Condition NVARCHAR(20) CHECK (Condition IN ('New', 'Used - Like New', 'Used - Good', 'Used - Fair')),
    AddedDate DATETIME DEFAULT GETDATE(),
    IsAvailable BIT DEFAULT 1,
    ImageUrl NVARCHAR(255) NOT NULL, -- Cloudinary URL for product image
    FOREIGN KEY (SellerID) REFERENCES Users(UserID),
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);
CREATE TABLE Coupons (
    CouponID INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(50) UNIQUE NOT NULL,
    DiscountAmount DECIMAL(18, 2) DEFAULT 0, -- Flat discount
    DiscountPercentage DECIMAL(4, 2) DEFAULT 0, -- Discount percentage (0.15 = 15%)
    MaxDiscount DECIMAL(18, 2) DEFAULT 0, -- Maximum discount that can be applied
    ExpiryDate DATETIME,
    IsActive BIT DEFAULT 1
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
    PRIMARY KEY (UserID, ShopID),  
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ShopID) REFERENCES Users(UserID)
);

CREATE TABLE Reviews (
    ReviewID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT,
    UserID INT,
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Comment TEXT,
    ReviewDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE ShoppingCart (
    CartID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT,
    ProductID INT,
    Quantity INT NOT NULL,
    AddedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

CREATE TABLE Payments (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT,
    PaymentDate DATETIME DEFAULT GETDATE(),
    Amount DECIMAL(18, 2) NOT NULL,
    PaymentMethod NVARCHAR(50) CHECK (PaymentMethod IN ('Credit Card', 'PayPal', 'Bank Transfer')),
    Status NVARCHAR(20) CHECK (Status IN ('Pending', 'Completed', 'Failed')),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
);

CREATE TABLE Messages (
    MessageID INT IDENTITY(1,1) PRIMARY KEY,
    SenderID INT,
    ReceiverID INT,
    Subject NVARCHAR(100),
    MessageBody TEXT,
    SentDate DATETIME DEFAULT GETDATE(),
    IsRead BIT DEFAULT 0,
    FOREIGN KEY (SenderID) REFERENCES Users(UserID),
    FOREIGN KEY (ReceiverID) REFERENCES Users(UserID)
);

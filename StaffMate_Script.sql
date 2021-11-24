USE master;
GO

CREATE DATABASE StaffMate;
GO

USE StaffMate;
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Brand] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(128) NULL,
    [Logo] varchar(max) NULL,
    [Phone] varchar(15) NULL,
    [Address] nvarchar(max) NULL,
    [Email] varchar(256) NULL,
    [HomePage] varchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Brand] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Ingredient] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(128) NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Ingredient] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Log] (
    [Id] bigint NOT NULL IDENTITY,
    [Content] nvarchar(max) NULL,
    [LogTime] datetime NULL,
    [Type] varchar(128) NULL,
    CONSTRAINT [PK_Log] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Role] (
    [Id] bigint NOT NULL IDENTITY,
    [Code] varchar(64) NULL,
    [Name] nvarchar(128) NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Tool] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(128) NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Tool] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Category] (
    [Id] bigint NOT NULL IDENTITY,
    [BrandId] bigint NOT NULL,
    [Name] nvarchar(128) NULL,
    [Description] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Category__BrandI__1ED998B2] FOREIGN KEY ([BrandId]) REFERENCES [Brand] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Account] (
    [UID] varchar(64) NOT NULL,
    [RoleId] bigint NULL,
    [Password] varchar(64) NOT NULL,
    [Email] varchar(256) NULL,
    [FullName] nvarchar(128) NULL,
    [Phone] varchar(15) NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK__Account__536C85E5FC9F8FDE] PRIMARY KEY ([UID]),
    CONSTRAINT [FK__Account__RoleId__1367E606] FOREIGN KEY ([RoleId]) REFERENCES [Role] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Admin] (
    [Id] bigint NOT NULL IDENTITY,
    [UID] varchar(64) NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Admin] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Admin__AccountUs__1BFD2C07] FOREIGN KEY ([UID]) REFERENCES [Account] ([UID]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Employee] (
    [Id] bigint NOT NULL IDENTITY,
    [UID] varchar(64) NULL,
    [BrandId] bigint NULL,
    [IsManager] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Employee] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Employee__Accoun__182C9B23] FOREIGN KEY ([UID]) REFERENCES [Account] ([UID]) ON DELETE NO ACTION,
    CONSTRAINT [FK__Employee__BrandI__1920BF5C] FOREIGN KEY ([BrandId]) REFERENCES [Brand] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Notification] (
    [Id] bigint NOT NULL IDENTITY,
    [SendingTime] datetime NOT NULL,
    [PayloadContent] nvarchar(max) NULL,
    [To] varchar(64) NULL,
    [IsSent] bit NOT NULL,
    [IsSeen] bit NOT NULL,
    [Type] varchar(max) NULL,
    [NotifiedId] bigint NOT NULL,
    CONSTRAINT [PK_Notification] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Notification__To__35BCFE0A] FOREIGN KEY ([To]) REFERENCES [Account] ([UID]) ON DELETE SET NULL
);
GO

CREATE TABLE [Dish] (
    [Id] bigint NOT NULL IDENTITY,
    [CategoryId] bigint NULL,
    [ManagerId] bigint NULL,
    [Name] nvarchar(128) NULL,
    [Description] nvarchar(max) NULL,
    [IsShow] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Dish] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Dish__CategoryId__21B6055D] FOREIGN KEY ([CategoryId]) REFERENCES [Category] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK__Dish__ManagerId__22AA2996] FOREIGN KEY ([ManagerId]) REFERENCES [Employee] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Recipe] (
    [Id] bigint NOT NULL IDENTITY,
    [DishId] bigint NOT NULL,
    [Description] nvarchar(max) NULL,
    [ImageDescription] varchar(max) NULL,
    [IsUsing] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Recipe] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Recipe__DishId__25869641] FOREIGN KEY ([DishId]) REFERENCES [Dish] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [ProcessingStep] (
    [Id] bigint NOT NULL IDENTITY,
    [RecipeId] bigint NOT NULL,
    [StepNumber] int NOT NULL,
    [StepTitle] nvarchar(max) NULL,
    [StepContent] nvarchar(max) NULL,
    [ImageDescription] varchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ProcessingStep] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Processin__Recip__286302EC] FOREIGN KEY ([RecipeId]) REFERENCES [Recipe] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [QA] (
    [Id] bigint NOT NULL IDENTITY,
    [QAId] bigint NULL,
    [UID] varchar(64) NULL,
    [RecipeId] bigint NOT NULL,
    [QATime] datetime NOT NULL,
    [Content] nvarchar(max) NULL,
    [IsReply] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_QA] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__QA__AccountUsern__31EC6D26] FOREIGN KEY ([UID]) REFERENCES [Account] ([UID]) ON DELETE SET NULL,
    CONSTRAINT [FK__QA__QAId__30F848ED] FOREIGN KEY ([QAId]) REFERENCES [QA] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK__QA__RecipeId__32E0915F] FOREIGN KEY ([RecipeId]) REFERENCES [Recipe] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [RecipeDetail] (
    [Id] bigint NOT NULL IDENTITY,
    [RecipeId] bigint NOT NULL,
    [IngredientId] bigint NOT NULL,
    [Amount] nvarchar(256) NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_RecipeDetail] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__RecipeDet__Ingre__2E1BDC42] FOREIGN KEY ([IngredientId]) REFERENCES [Ingredient] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK__RecipeDet__Recip__2D27B809] FOREIGN KEY ([RecipeId]) REFERENCES [Recipe] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [RecipeTool] (
    [Id] bigint NOT NULL IDENTITY,
    [RecipeId] bigint NOT NULL,
    [ToolId] bigint NOT NULL,
    [Amount] nvarchar(256) NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_RecipeTool] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__RecipeToo__Recip__3C69FB99] FOREIGN KEY ([RecipeId]) REFERENCES [Recipe] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK__RecipeToo__ToolI__3D5E1FD2] FOREIGN KEY ([ToolId]) REFERENCES [Tool] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [RefreshToken] (
    [Id] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000',
    [Token] varchar(max) NULL,
    [JwtId] varchar(max) NULL,
    [UserUID] varchar(max) NULL,
    [IsUsed] bit NOT NULL,
    [IsRevoked] bit NOT NULL,
    [AddedDate] datetime2 NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    CONSTRAINT [PK_RefreshToken] PRIMARY KEY ([Id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'IsDeleted', N'Name') AND [object_id] = OBJECT_ID(N'[Role]'))
    SET IDENTITY_INSERT [Role] ON;
INSERT INTO [Role] ([Id], [Code], [IsDeleted], [Name])
VALUES (CAST(1 AS bigint), 'ADMIN', CAST(0 AS bit), N'Admin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'IsDeleted', N'Name') AND [object_id] = OBJECT_ID(N'[Role]'))
    SET IDENTITY_INSERT [Role] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'IsDeleted', N'Name') AND [object_id] = OBJECT_ID(N'[Role]'))
    SET IDENTITY_INSERT [Role] ON;
INSERT INTO [Role] ([Id], [Code], [IsDeleted], [Name])
VALUES (CAST(2 AS bigint), 'MANAGER', CAST(0 AS bit), N'Manager');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'IsDeleted', N'Name') AND [object_id] = OBJECT_ID(N'[Role]'))
    SET IDENTITY_INSERT [Role] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'IsDeleted', N'Name') AND [object_id] = OBJECT_ID(N'[Role]'))
    SET IDENTITY_INSERT [Role] ON;
INSERT INTO [Role] ([Id], [Code], [IsDeleted], [Name])
VALUES (CAST(3 AS bigint), 'STAFF', CAST(0 AS bit), N'Staff');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'IsDeleted', N'Name') AND [object_id] = OBJECT_ID(N'[Role]'))
    SET IDENTITY_INSERT [Role] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UID', N'Email', N'FullName', N'IsDeleted', N'Password', N'Phone', N'RoleId') AND [object_id] = OBJECT_ID(N'[Account]'))
    SET IDENTITY_INSERT [Account] ON;
INSERT INTO [Account] ([UID], [Email], [FullName], [IsDeleted], [Password], [Phone], [RoleId])
VALUES ('fxOOYx2XASRKC2A77aNG2wKJhAy2', 'staffmatesum2021@gmail.com', N'Admin System', CAST(0 AS bit), '$2a$11$gQLzmk8hWrWEYNFppMaWVugdn6rWlluDs/PtvujwnxTIf.rMxEk4O', NULL, CAST(1 AS bigint));
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UID', N'Email', N'FullName', N'IsDeleted', N'Password', N'Phone', N'RoleId') AND [object_id] = OBJECT_ID(N'[Account]'))
    SET IDENTITY_INSERT [Account] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsDeleted', N'UID') AND [object_id] = OBJECT_ID(N'[Admin]'))
    SET IDENTITY_INSERT [Admin] ON;
INSERT INTO [Admin] ([Id], [IsDeleted], [UID])
VALUES (CAST(1 AS bigint), CAST(0 AS bit), 'fxOOYx2XASRKC2A77aNG2wKJhAy2');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsDeleted', N'UID') AND [object_id] = OBJECT_ID(N'[Admin]'))
    SET IDENTITY_INSERT [Admin] OFF;
GO

CREATE INDEX [IX_Account_RoleId] ON [Account] ([RoleId]);
GO

CREATE UNIQUE INDEX [IX_Admin_UID] ON [Admin] ([UID]) WHERE [UID] IS NOT NULL;
GO

CREATE INDEX [IX_Category_BrandId] ON [Category] ([BrandId]);
GO

CREATE INDEX [IX_Dish_CategoryId] ON [Dish] ([CategoryId]);
GO

CREATE INDEX [IX_Dish_ManagerId] ON [Dish] ([ManagerId]);
GO

CREATE INDEX [IX_Employee_BrandId] ON [Employee] ([BrandId]);
GO

CREATE UNIQUE INDEX [IX_Employee_UID] ON [Employee] ([UID]) WHERE [UID] IS NOT NULL;
GO

CREATE INDEX [IX_Notification_To] ON [Notification] ([To]);
GO

CREATE INDEX [IX_ProcessingStep_RecipeId] ON [ProcessingStep] ([RecipeId]);
GO

CREATE UNIQUE INDEX [IX_QA_QAId] ON [QA] ([QAId]) WHERE [QAId] IS NOT NULL;
GO

CREATE INDEX [IX_QA_RecipeId] ON [QA] ([RecipeId]);
GO

CREATE INDEX [IX_QA_UID] ON [QA] ([UID]);
GO

CREATE INDEX [IX_Recipe_DishId] ON [Recipe] ([DishId]);
GO

CREATE INDEX [IX_RecipeDetail_IngredientId] ON [RecipeDetail] ([IngredientId]);
GO

CREATE INDEX [IX_RecipeDetail_RecipeId] ON [RecipeDetail] ([RecipeId]);
GO

CREATE INDEX [IX_RecipeTool_RecipeId] ON [RecipeTool] ([RecipeId]);
GO

CREATE INDEX [IX_RecipeTool_ToolId] ON [RecipeTool] ([ToolId]);
GO

CREATE UNIQUE INDEX [UQ__Role__A25C5AA7C878A431] ON [Role] ([Code]) WHERE [Code] IS NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210616165150_StaffMate-DB-Migration', N'5.0.6');
GO

COMMIT;
GO

USE StaffMate
GO

INSERT INTO Brand (Name,Logo,Phone,Address,Email,HomePage,IsDeleted) VALUES
	 (N'Kichi Kichi',N'https://firebasestorage.googleapis.com/v0/b/staffmate-e0767.appspot.com/o/images%2FKichi_Kichi.png?alt=media&token=bb2a0fe6-daa3-4fa3-aafa-ea909f3392c2',N'0123456789',NULL,N'admin@kichikichi.com',N'https://kichi.com.vn/',0);
GO

INSERT INTO Account (UID,RoleId,Password,Email,FullName,Phone,IsDeleted) VALUES
	 (N'isD5tOmoHZgos13UwOstmBQcQRy2',2,N'$2a$11$3JhCmLOnjhqEQ0GDSAmPpeTA5NniVZMK6EnsB76sgvoLAkee5nbuu',N'tinddse140129@fpt.edu.vn',N'Tín',N'0123456789',0),
	 (N'TOiLrOnBbZY2jshEGnLUsCzD5ny2',3,N'$2a$11$Bm.o74zPk3R1U8FrWNJaz.f9mFJCa8q4Jo68YshF1LJPghGRT2Z1a',N'hieulm0607@gmail.com',N'Hiếu cu li',N'0123456789',0);
GO

INSERT INTO Employee (UID,BrandId,IsManager,IsDeleted) VALUES
	 (N'isD5tOmoHZgos13UwOstmBQcQRy2',1,1,0),
	 (N'TOiLrOnBbZY2jshEGnLUsCzD5ny2',1,0,0);
GO

INSERT INTO Category (BrandId,Name,Description,IsDeleted) VALUES
	 (1,N'Chè',N'Chè tráng miệng sau bữa chính',0);
GO

INSERT INTO Dish (CategoryId,ManagerId,Name,Description,IsShow,IsDeleted) VALUES
	 (1,1,N'Chè Bắp Đường Phèn',N'Chè Bắp Đường Phèn (chè ngô) là một trong những món chè tráng miệng khoái khẩu của nhiều người. Vị ngọt thanh của bắp quyện cùng vị béo nước cốt dừa khiến món chè bắp này có hương vị thơm ngon khó cưỡng. ',1,0),
	 (1,1,N'Chè Sương Sa Hột Lựu',N'Chè sương sa hạt lựu không chỉ gắn liền với tuổi thơ mọi người vì màu sắc bắt mắt, mà vì sự ngọt mát từ củ năng dai giòn sần sật còn ăn chung với nước cốt dừa béo béo. Cách làm thì rất đơn giản, cuối tuần mọi người trong gia đình quây quần bên nhau cùng chén chè sương sa hạt lựu mát lạnh thì còn gì bằng.',1,0);
GO

INSERT INTO Recipe (DishId,Description,ImageDescription,IsUsing,IsDeleted) VALUES
	 (1,N'Cách nấu đơn giản nhưng mang lại cho thực khách trải nghiệm ngọt ngào',N'https://firebasestorage.googleapis.com/v0/b/staffmate-e0767.appspot.com/o/images%2Fp2x94xmeayj_1626373749196.jpg?alt=media&token=85c6352d-13e3-4144-8abb-330b03527c8f',1,0),
	 (2,N'Cách làm thì rất đơn giản, cuối tuần mọi người trong gia đình quây quần bên nhau ở Kichi cùng chén chè sương sa hạt lựu mát lạnh thì còn gì bằng.',N'https://firebasestorage.googleapis.com/v0/b/staffmate-e0767.appspot.com/o/images%2F9n7ctt29elc_1626375879098.jpg?alt=media&token=e91b32b3-ce1b-499e-8668-b67ca9b21d3c',1,0);
GO

INSERT INTO Ingredient (Name,IsDeleted) VALUES
	 (N'Gà ta (Chặt sẵn)',0),
	 (N'Sốt ướp gà Cooky Market',0),
	 (N'Gừng',0),
	 (N'Hành tím',0),
	 (N'Ngò rí',0),
	 (N'Tỏi băm',0),
	 (N'Cá mackerel',0),
	 (N'Nước mắm',0),
	 (N'Ớt băm',0),
	 (N'Mật ong',0),
	 (N'Cá lóc',0),
	 (N'Hạt nêm',0),
	 (N'Bột ngọt',0),
	 (N'Đường trắng',0),
	 (N'Tiêu',0),
	 (N'Nước màu',0),
	 (N'Ớt',0),
	 (N'Hành lá',0),
	 (N'Dầu ăn',0),
	 (N'Bắp Mỹ',0),
	 (N'Bột năng',0),
	 (N'Lá dứa',0),
	 (N'Đường phèn',0),
	 (N'Muối',0),
	 (N'Nước cốt dừa',0),
	 (N'Củ năng',0),
	 (N'Đậu xanh không vỏ',0),
	 (N'Bột sương sáo',0),
	 (N'Củ dền',0),
	 (N'Thịt heo',0),
	 (N'Ớt đỏ',0),
	 (N'Bí ngô',0),
	 (N'Nghệ',0);
GO

INSERT INTO Tool (Name,IsDeleted) VALUES
	 (N'Nồi đất',0),
	 (N'Tô',0),
	 (N'Chảo',0),
	 (N'Dao',0),
	 (N'Chén',0),
	 (N'Nồi inox',0),
	 (N'Muỗng cà phê',0),
	 (N'Muỗng canh',0),
	 (N'Dao bào',0),
	 (N'Nồi',0);
GO


INSERT INTO ProcessingStep (RecipeId,StepNumber,StepTitle,StepContent,ImageDescription,IsDeleted) VALUES
	 (1,1,N'Chuẩn bị bắp',N'Tách lớp vỏ bên ngoài trái bắp Mỹ đi rồi đem rửa sạch với nước. Dùng dao bào mỏng hạt bắp theo chiều dọc. Cho phần lõi bắp đã bào hết hạt vào nồi và đặt lên trên bếp luộc cùng với 20g lá dứa khoảng 20 phút cho nước ngọt thanh thì bạn vớt hết lõi bắp và lá dứa ra.',N'https://firebasestorage.googleapis.com/v0/b/staffmate-e0767.appspot.com/o/images%2Fvzgbqmo7wv_1626373750988.jpg?alt=media&token=68133e8e-c2db-4a24-a96d-b229e8bf737a',0),
	 (1,2,N'Nêm bắp',N'Cách nấu chè bắp đường phèn ngon: Cho 4 muỗng canh đường phèn và bắp hạt đã bào mỏng vào. Nêm thêm 1/4 muỗng cà phê muối cho chè bắp thêm ngọt. Pha 3 muỗng canh bột năng vào 100ml nước cho tan hoàn toàn rồi cho vào nồi bắp đang sôi. Khuấy đều tay cho chè bắp sệt lại rồi tắt lửa.',N'https://firebasestorage.googleapis.com/v0/b/staffmate-e0767.appspot.com/o/images%2F3kk165rc3pj_1626373752380.jpg?alt=media&token=13a938f5-c316-4ca4-8539-2548e610e5aa',0),
	 (1,3,N'Trộn với nước cốt dừa',N'Trộn đều 2 muỗng canh bột năng với 50ml nước đổ vào 400ml nước cốt dừa. Dằn thêm chút muối cho nước cốt dừa thêm ngon béo. Khuấy đều đến khi thấy chè bắp đường phèn sôi rồi tắt bếp.',N'https://firebasestorage.googleapis.com/v0/b/staffmate-e0767.appspot.com/o/images%2Fxeppuyrmw4o_1626373753794.jpg?alt=media&token=d61d8389-a32c-49d5-80e7-4f4bb04c9129',0),
	 (1,4,N'Phục vụ món',N'Cách làm chè bắp đường phèn ngon thật đơn giản phải không nào! Món chè bắp ăn chung với nước cốt dừa béo ngậy thì còn gì bằng. Bạn có thể ăn nóng hoặc thêm đá vào đều ngon. Với công thức nấu chè bắp này, nếu muốn bảo quản chè bắp đường phèn dùng vào ngày hôm sau thì để nguội rồi cho vào ngăn mát tủ lạnh. Lưu ý là không được cho nước cốt dừa vì sẽ khiến chè bắp nhanh thiu đi đấy.',N'https://firebasestorage.googleapis.com/v0/b/staffmate-e0767.appspot.com/o/images%2Futvkkqjwcx_1626373755193.jpg?alt=media&token=6f7f0ca6-0e90-47b1-8f76-62a1da7ac4e6',0),
	 (2,1,N'Chuẩn bị',N'Cho 100gr lá dứa đem xay chung với 320ml nước, 80gr củ dền xay với 300ml nước là ta có nước lá dứa và nước củ dền với màu sắc thiên nhiên đẹp mắt an toàn cho sức khỏe.',N'https://firebasestorage.googleapis.com/v0/b/staffmate-e0767.appspot.com/o/images%2Fzm5tlkjfl6_1626375880901.jpg?alt=media&token=2e1eb10e-8089-4e0c-9586-d4268864f7d3',0),
	 (2,2,N'Chuẩn bị nước dùng và củ dền, năng',N'1kg củ năng gọt vỏ cắt hạt lựu, chia làm 2 phần. Ngâm từng phần củ năng vào nước củ dền và nước lá dứa trong 30 phút để tạo màu.',N'https://firebasestorage.googleapis.com/v0/b/staffmate-e0767.appspot.com/o/images%2F77jo3ek4i7_1626375882274.jpg?alt=media&token=a89de63a-10a4-48c4-bd37-f368b137a463',0),
	 (2,3,N'Trộn củ năng',N'Ngâm xong bạn vớt củ năng ra cho ráo nước, các bạn nhớ giữ lại nước lá dứa và nước củ năng để luộc hạt lựu nhé. Sau đó cho từ từ từng muỗng bột năng ( khoảng 100gr ) vào trộn đều cho bột bám đều củ năng.',N'https://firebasestorage.googleapis.com/v0/b/staffmate-e0767.appspot.com/o/images%2Fijzbv98wtn_1626375883533.jpg?alt=media&token=87a04dd2-9429-409f-a7a0-becdd9aca591',0),
	 (2,4,N'Luộc củ',N'Nấu nước củ dền với 200ml nước cho sôi rồi mới cho củ năng vào luộc khoảng 3 phút rồi vớt ra ngâm vào thau nước đá, làm tương tự cho nước lá dứa.',N'https://firebasestorage.googleapis.com/v0/b/staffmate-e0767.appspot.com/o/images%2Fdr47385iaym_1626375884832.jpg?alt=media&token=0464d9e3-b091-4163-b253-1c84965fb78a',0),
	 (2,5,N'Chuẩn bị đậu xanh',N'120gr đậu xanh không vỏ ngâm 2 tiếng với nước sôi cho mềm rồi nấu với 500ml nước và 50gr đường khoảng 20 phút cho mềm nhừ rồi xay nhuyễn. Sau đó bảo quản trong ngăn mát tủ lạnh.',N'https://firebasestorage.googleapis.com/v0/b/staffmate-e0767.appspot.com/o/images%2Fg48xvmjk4hv_1626375886245.jpg?alt=media&token=33a0cb3c-c21b-49af-9544-79d554db4cdc',0),
	 (2,6,N'Chuẩn bị sương sáo',N'Tiếp đến bạn ngâm 25gr bột sương sáo với 500ml nước trong 10 phút, rồi đem nấu chung với 40gr đường đến sôi rồi tắt bếp. Rót sương sáo ra khuôn cho bớt nóng rồi để trong ngăn mát tủ lạnh khoảng 30 phút là thạch sương sáo đông đặc lại.',N'https://firebasestorage.googleapis.com/v0/b/staffmate-e0767.appspot.com/o/images%2Ftrzpwttqcwh_1626375887677.jpg?alt=media&token=f32c6db1-342b-41ff-a68c-a1358e838ef0',0),
	 (2,7,N'Chuẩn bị nước cốt dừa',N'Cuối cùng bạn nấu 350ml nước cốt dừa với 1/4 muỗng cà phê muối và 1 muỗng canh đường cho sôi rồi tắt bếp là xong.',N'https://firebasestorage.googleapis.com/v0/b/staffmate-e0767.appspot.com/o/images%2Fhrmzyag5nb7_1626375888973.jpg?alt=media&token=30745d6d-f679-476b-adc7-15c19040fbb6',0),
	 (2,8,N'Phục vụ món',N'Xếp từng phần củ năng xen kẽ cho đẹp mắt, kế tiếp để thêm sương sáo và cuối cùng là đậu xanh. Thế là ta đã hoàn thành xong chè sương sa hột lựu thanh mát, công thức rất dễ làm đúng không nè? Chúc mọi người thành công.',N'https://firebasestorage.googleapis.com/v0/b/staffmate-e0767.appspot.com/o/images%2Fvafb7u9r18_1626375890320.jpg?alt=media&token=0a7f6ecb-648e-402f-b245-937e50e2c89d',0);
GO

INSERT INTO RecipeDetail (RecipeId,IngredientId,Amount,IsDeleted) VALUES
	 (1,20,N'2 Trái',0),
	 (1,21,N'5 Muỗng canh',0),
	 (1,22,N'4 Lá',0),
	 (1,23,N'4 Muỗng canh',0),
	 (1,24,N'1/2 Muỗng cà phê',0),
	 (1,25,N'400 ml',0),
	 (2,29,N'80 Gr',0),
	 (2,21,N'200 Gr',0),
	 (2,22,N'100 Gr',0),
	 (2,24,N'1/4 Muỗng cà phê',0),
	 (2,28,N'25 Gr',0),
	 (2,25,N'350 ml',0),
	 (2,27,N'120 Gr',0),
	 (2,26,N'1 Kg',0),
	 (2,14,N'100 Gr',0);

GO

INSERT INTO RecipeTool (RecipeId,ToolId,Amount,IsDeleted) VALUES
	 (1,9,N'1 cái',0),
	 (1,10,N'1 cái',0),
	 (1,8,N'1 cái',0),
	 (2,8,N'1 cái',0),
	 (2,9,N'1 cái',0),
	 (2,2,N'3 cái',0),
	 (2,10,N'1 cái',0),
	 (2,7,N'1 cái',0);

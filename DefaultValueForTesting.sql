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



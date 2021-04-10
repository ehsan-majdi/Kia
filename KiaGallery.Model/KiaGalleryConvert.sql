USE [KiaGalleryMain]

UPDATE [KiaGalleryMainContext].[dbo].[Workshops] SET CreateUserId = 1
UPDATE [KiaGalleryMainContext].[dbo].[Workshops] SET ModifyUserId = 1

UPDATE [KiaGalleryMainContext].[dbo].[Locations] SET CreateUserId = 1
UPDATE [KiaGalleryMainContext].[dbo].[Locations] SET ModifyUserId = 1

UPDATE [KiaGalleryMainContext].[dbo].[Branches] SET CreateUserId = 1
UPDATE [KiaGalleryMainContext].[dbo].[Branches] SET ModifyUserId = 1

GO
SET IDENTITY_INSERT [dbo].[Workshops] ON
INSERT INTO [dbo].[Workshops]
		(Id
		,[OrderNo]
		,[Alias]
		,[Name]
		,[Color]
		,[AutoConfirm]
		,[Active]
		,[GoldTrade]
		,[CreateUserId]
		,[ModifyUserId]
		,[CreateDate]
		,[ModifyDate]
		,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[Workshops]
SET IDENTITY_INSERT [dbo].[Workshops] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[Locations] ON
INSERT INTO [dbo].[Locations]
		(Id
		,[ParentId]
		,[LocationType]
		,[OrderNo]
		,[Name]
		,[EnglishName]
		,[CreateUserId]
		,[ModifyUserId]
		,[CreateDate]
		,[ModifyDate]
		,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[Locations]
SET IDENTITY_INSERT [dbo].[Locations] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[Branches] ON
INSERT INTO [dbo].[Branches]
		([Id]
      ,[CityId]
	  ,[BranchType]
      ,[OrderNo]
	  ,[WorkingHour]
      ,[OwnerName]
      ,[OwnerNationalityCode]
      ,[OwnerFatherName]
	  ,[OwnerNationalityNo]
      ,[Alias]
      ,[Name]
      ,[EnglishName]
      ,[Address]
      ,[EnglishAddress]
      ,[Color]
	  ,[Phone]
	  ,[CreditCard]
      ,[Latitude]
      ,[Longitude]
      ,[Active]
	  ,[GoldDebt]
      ,[RialDebt]
      ,[GoldCredit]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[Branches]
SET IDENTITY_INSERT [dbo].[Branches] OFF
GO

UPDATE [UserProfile] SET BranchId = 19 WHERE ID = 1

GO
SET IDENTITY_INSERT [dbo].[UserProfile] ON
INSERT INTO [dbo].[UserProfile]
		(Id
		,[BranchId]
		,[WorkshopId]
		,[FirstName]
		,[LastName]
		,[Color]
		,[FileName]
		,[PhoneNumber]
		,[Username]
		,[Salt]
		,[Password]
		,[UserType]
		,[Active]
		,[CreateUserId]
		,[ModifyUserId]
		,[CreateDate]
		,[ModifyDate]
		,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[UserProfile] WHERE ID != 1
SET IDENTITY_INSERT [dbo].[UserProfile] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[Leathers] ON
INSERT INTO [dbo].[Leathers]
		(Id
		,[Name]
		,[LeatherType]
		,[OrderNo]
		,[FileName]
		,[Active]
		,[CreateUserId]
		,[ModifyUserId]
		,[CreateDate]
		,[ModifyDate]
		,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[Leathers]
SET IDENTITY_INSERT [dbo].[Leathers] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[Messages] ON
INSERT INTO [dbo].[Messages]
		([Id]
      ,[BranchId]
      ,[UserId]
      ,[Title]
      ,[Body]
      ,[Read]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip]
      ,[Product_Id])
SELECT * FROM [KiaGalleryMainContext].[dbo].[Messages]
SET IDENTITY_INSERT [dbo].[Messages] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[Sizes] ON
INSERT INTO [dbo].[Sizes]
		(Id
		,[Title]
		,[DefaultValue]
		,[CreateUserId]
		,[ModifyUserId]
		,[CreateDate]
		,[ModifyDate]
		,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[Sizes]
SET IDENTITY_INSERT [dbo].[Sizes] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[SizeValues] ON
INSERT INTO [dbo].[SizeValues]
		(Id
		,[SizeId]
		,[OrderNo]
		,[Value]
		,[CreateUserId]
		,[ModifyUserId]
		,[CreateDate]
		,[ModifyDate]
		,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[SizeValues]
SET IDENTITY_INSERT [dbo].[SizeValues] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[Products] ON
INSERT INTO [dbo].[Products]
		([Id]
		,[Code]
		,[BookCode]
		,[Title]
		,[ProductType]
		,[GoldType]
		,[Sex]
		,[WorkshopId]
		,[WorkshopId2]
		,[Weight]
		,[SilverWeight]
		,[StoneCount]
		,[StonePrice]
		,[LeatherCount]
		,[LeatherPrice]
		,[SizeId]
		,[NormalSizeValueId]
		,[Wage]
		,[CanLoop]
		,[Active]
		,[New]
		,[OuterWerkPlacement]
		,[OuterWerkCategory]
		,[CreateUserId]
		,[ModifyUserId]
		,[CreateDate]
		,[ModifyDate]
		,[Ip])
SELECT [Id]
		,[Code]
		,[BookCode]
		,[Title]
		,[ProductType]
		,[GoldType]
		,[Sex]
		,[WorkshopId]
		,[WorkshopId2]
		,[Weight]
		,0
		,[StoneCount]
		,[StonePrice]
		,[LeatherCount]
		,[LeatherPrice]
		,[SizeId]
		,[NormalSizeValueId]
		,[Wage]
		,[CanLoop]
		,[Active]
		,[New]
		,[OuterWerkPlacement]
		,[OuterWerkCategory]
		,[CreateUserId]
		,[ModifyUserId]
		,[CreateDate]
		,[ModifyDate]
		,[Ip] FROM [KiaGalleryMainContext].[dbo].[Products]
SET IDENTITY_INSERT [dbo].[Products] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[ProductFiles] ON
INSERT INTO [dbo].[ProductFiles]
		([Id]
      ,[ProductId]
      ,[FileName]
	  ,[FileId]
      ,[FileType]
      ,[PaddingImg]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[ProductFiles]
SET IDENTITY_INSERT [dbo].[ProductFiles] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[ProductLeathers] ON
INSERT INTO [dbo].[ProductLeathers]
		(Id
		,[ProductId]
		,[OrderNo]
		,[LeatherId])
SELECT * FROM [KiaGalleryMainContext].[dbo].[ProductLeathers]
SET IDENTITY_INSERT [dbo].[ProductLeathers] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[Stones] ON
INSERT INTO [dbo].[Stones]
		(Id
		,[Name]
		,[StoneType]
		,[OrderNo]
		,[FileName]
		,[Active]
		,[CreateUserId]
		,[ModifyUserId]
		,[CreateDate]
		,[ModifyDate]
		,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[Stones]
SET IDENTITY_INSERT [dbo].[Stones] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[ShapeSizes] ON
INSERT INTO [dbo].[ShapeSizes]
		(Id
		,[OrderNo]
		,[StoneShape]
		,[SizeLength]
		,[SizeWidth]
		,[Active]
		,[CreateUserId]
		,[ModifyUserId]
		,[CreateDate]
		,[ModifyDate]
		,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[ShapeSizes]
SET IDENTITY_INSERT [dbo].[ShapeSizes] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[ProductStones] ON
INSERT INTO [dbo].[ProductStones]
		(Id
		,[ProductId]
		,[OrderNo]
		,[DefaultStoneId]
		,[StoneId]
		,[StoneShape]
		,[ShapeSizeId])
SELECT * FROM [KiaGalleryMainContext].[dbo].[ProductStones]
SET IDENTITY_INSERT [dbo].[ProductStones] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[RelatedProducts] ON
INSERT INTO [dbo].[RelatedProducts]
		(Id
		,[ProductId]
		,[RelatedToId]
		,[CreateUserId]
		,[ModifyUserId]
		,[CreateDate]
		,[ModifyDate]
		,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[RelatedProducts]
SET IDENTITY_INSERT [dbo].[RelatedProducts] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[SetProducts] ON
INSERT INTO [dbo].[SetProducts]
		([Id]
      ,[ProductId]
      ,[SetToId]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[SetProducts]
SET IDENTITY_INSERT [dbo].[SetProducts] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[Roles] ON
INSERT INTO [dbo].[Roles]
		(Id
		,[UserId]
		,[Title])
SELECT * FROM [KiaGalleryMainContext].[dbo].[Roles] WHERE ID != 1
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[Settings] ON
INSERT INTO [dbo].[Settings]
		(Id
		,[Key]
		,[Value]
		,[CreateUserId]
		,[ModifyUserId]
		,[CreateDate]
		,[ModifyDate]
		,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[Settings]
SET IDENTITY_INSERT [dbo].[Settings] OFF
GO



GO
SET IDENTITY_INSERT [order].[Cart] ON
INSERT INTO [order].[Cart]
		([Id]
      ,[BranchId]
      ,[ProductId]
      ,[SetNumber]
      ,[Count]
      ,[OrderType]
      ,[GoldType]
      ,[OuterWerkType]
      ,[Size]
      ,[LeatherLoop]
      ,[Customer]
      ,[PhoneNumber]
      ,[ForceOrder]
      ,[BranchLabel]
      ,[Description]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[order].[Cart]
SET IDENTITY_INSERT [order].[Cart] OFF
GO

GO
SET IDENTITY_INSERT [order].[CartProductLeather] ON
INSERT INTO [order].[CartProductLeather]
		(Id
		,[CartId]
		,[OrderNo]
		,[LeatherId])
SELECT * FROM [KiaGalleryMainContext].[order].[CartProductLeather]
SET IDENTITY_INSERT [order].[CartProductLeather] OFF
GO

GO
SET IDENTITY_INSERT [order].[CartProductStone] ON
INSERT INTO [order].[CartProductStone]
		(Id
		,[CartId]
		,[OrderNo]
		,[StoneId])
SELECT * FROM [KiaGalleryMainContext].[order].[CartProductStone]
SET IDENTITY_INSERT [order].[CartProductStone] OFF
GO

GO
SET IDENTITY_INSERT [order].[FavouritesProduct] ON
INSERT INTO [order].[FavouritesProduct]
		(Id
		,[BranchId]
		,[ProductId]
		,[CreateUserId]
		,[ModifyUserId]
		,[CreateDate]
		,[ModifyDate]
		,[Ip])
SELECT * FROM [KiaGalleryMainContext].[order].[FavouritesProduct]
SET IDENTITY_INSERT [order].[FavouritesProduct] OFF
GO

GO
SET IDENTITY_INSERT [order].[Order] ON
INSERT INTO [order].[Order]
		(Id
        ,[OrderNumber]
		,[OrderSerial]
		,[OrderStatus]
		,[BranchId]
		,[Description]
		,[Deleted]
		,[CreateUserId]
		,[ModifyUserId]
		,[CreateDate]
		,[ModifyDate]
		,[Ip])
SELECT * FROM [KiaGalleryMainContext].[order].[Order]
SET IDENTITY_INSERT [order].[Order] OFF
GO

GO
SET IDENTITY_INSERT [order].[WorkshopOrder] ON
INSERT INTO [order].[WorkshopOrder]
		(Id
		,[OrderId]
		,[WorkshopOrderSerial]
		,[WorkshopOrderNumber]
		,[CreateUserId]
		,[ModifyUserId]
		,[CreateDate]
		,[ModifyDate]
		,[Ip])
SELECT * FROM [KiaGalleryMainContext].[order].[WorkshopOrder]
SET IDENTITY_INSERT [order].[WorkshopOrder] OFF
GO

GO
SET IDENTITY_INSERT [order].[OrderDetail] ON
INSERT INTO [order].[OrderDetail]
		([Id]
      ,[OrderId]
      ,[WorkshopOrderId]
	  ,[WorkshopOrderId2]
	  ,[SendWorkshopOrder2]
      ,[ProductId]
      ,[SetNumber]
      ,[Count]
      ,[OrderType]
      ,[OrderDetailStatus]
      ,[Size]
      ,[GoldType]
      ,[LeatherLoop]
      ,[Customer]
      ,[PhoneNumber]
      ,[ForceOrder]
      ,[BranchLabel]
      ,[Description]
      ,[RelatedOrderDetailId]
      ,[OuterWerkType]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[order].[OrderDetail]
SET IDENTITY_INSERT [order].[OrderDetail] OFF
GO

GO
SET IDENTITY_INSERT [order].[OrderDetailLeather] ON
INSERT INTO [order].[OrderDetailLeather]
		(Id
		,[OrderDetailId]
		,[OrderNo]
		,[LeatherId])
SELECT * FROM [KiaGalleryMainContext].[order].[OrderDetailLeather]
SET IDENTITY_INSERT [order].[OrderDetailLeather] OFF
GO

GO
SET IDENTITY_INSERT [order].[OrderDetailLogReason] ON
INSERT INTO [order].[OrderDetailLogReason]
		(Id
		,[OrderDetailStatus]
		,[Text]
		,[Active]
		,[CreateUserId]
		,[ModifyUserId]
		,[CreateDate]
		,[ModifyDate]
		,[Ip])
SELECT * FROM [KiaGalleryMainContext].[order].[OrderDetailLogReason]
SET IDENTITY_INSERT [order].[OrderDetailLogReason] OFF
GO

GO
SET IDENTITY_INSERT [order].[OrderDetailLog] ON
INSERT INTO [order].[OrderDetailLog]
		(Id
		,[OrderDetailId]
		,[OrderDetailStatus]
		,[OrderDetailLogReasonId]
		,[Description]
		,[CreateUserId]
		,[CreateDate]
		,[Ip])
SELECT * FROM [KiaGalleryMainContext].[order].[OrderDetailLog]
SET IDENTITY_INSERT [order].[OrderDetailLog] OFF
GO

GO
SET IDENTITY_INSERT [order].[OrderDetailStone] ON
INSERT INTO [order].[OrderDetailStone]
		(Id
		,[OrderDetailId]
		,[OrderNo]
		,[StoneId])
SELECT * FROM [KiaGalleryMainContext].[order].[OrderDetailStone]
SET IDENTITY_INSERT [order].[OrderDetailStone] OFF
GO

GO
SET IDENTITY_INSERT [order].[OrderLog] ON
INSERT INTO [order].[OrderLog]
		(Id
		,[OrderId]
		,[OrderStatus]
		,[Description]
		,[CreateUserId]
		,[CreateDate]
		,[Ip])
SELECT * FROM [KiaGalleryMainContext].[order].[OrderLog]
SET IDENTITY_INSERT [order].[OrderLog] OFF
GO

GO
SET IDENTITY_INSERT [post].[PostItem] ON
INSERT INTO [post].[PostItem]
		([Id]
      ,[CityId]
      ,[InvoiceNo]
      ,[Count]
      ,[Weight]
      ,[SubmitUser]
      ,[Price]
      ,[SubmitDate]
      ,[PostDate]
      ,[Customer]
      ,[PhoneNumber]
      ,[MobileNumber]
      ,[Address]
      ,[PostalCode]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[post].[PostItem]
SET IDENTITY_INSERT [post].[PostItem] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[Companies] ON
INSERT INTO [dbo].[Companies]
		([Id]
      ,[OrderNo]
      ,[Alias]
      ,[Name]
      ,[EnglishName]
      ,[Address]
      ,[EnglishAddress]
      ,[Active]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[Companies]
SET IDENTITY_INSERT [dbo].[Companies] OFF
GO

GO
SET IDENTITY_INSERT [gift].[Gift] ON
INSERT INTO [gift].[Gift]
		([Id]
      ,[Code]
      ,[GiftType]
      ,[GiftStatus]
      ,[CompanyIdShopping]
      ,[BranchIdShopping]
      ,[BranchIdReceiverCustomer]
      ,[Value]
      ,[ExpirationTime]
      ,[BuyerCustomerName]
      ,[BuyerCustomerPhoneNumber]
      ,[RevocationCustomerName]
      ,[RevocationCustomerPhoneNumber]
      ,[FactorNumber]
      ,[FactorPrice]
	  ,[Description])
SELECT * FROM [KiaGalleryMainContext].[gift].[Gift]
SET IDENTITY_INSERT [gift].[Gift] OFF
GO


GO
SET IDENTITY_INSERT [gift].[GiftLog] ON
INSERT INTO [gift].[GiftLog]
		([Id]
      ,[GiftStatus]
      ,[GiftId]
      ,[CreateUserId]
      ,[CreateDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[gift].[GiftLog]
SET IDENTITY_INSERT [gift].[GiftLog] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[ProductOuterWerks] ON
INSERT INTO [dbo].[ProductOuterWerks]
		([Id]
      ,[ProductId]
      ,[OuterWerkType]
      )
SELECT * FROM [KiaGalleryMainContext].[dbo].[ProductOuterWerks]
SET IDENTITY_INSERT [dbo].[ProductOuterWerks] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[PersonJobDescriptionTemplates] ON
INSERT INTO [dbo].[PersonJobDescriptionTemplates]
		([Id]
      ,[Title]
      ,[Description]
      ,[Status]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[PersonJobDescriptionTemplates]
SET IDENTITY_INSERT [dbo].[PersonJobDescriptionTemplates] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[People] ON
INSERT INTO [dbo].[People]
		([Id]
      ,[PersonNumber]
      ,[BranchId]
      ,[FirstName]
      ,[LastName]
      ,[ShortName]
      ,[NikName]
      ,[FatherName]
      ,[Gender]
      ,[NationalCode]
      ,[BirthCertificateNumber]
      ,[BirthDate]
      ,[PhoneNumber]
      ,[MobileNumber]
      ,[NecessaryNumber]
      ,[ExportFrom]
      ,[City]
      ,[Address]
      ,[Supervisor]
      ,[ContractSubject]
      ,[JobId]
      ,[Married]
      ,[Children]
      ,[Contract]
      ,[ContractInMonth]
      ,[AccountNumber]
      ,[ContractStartDate]
      ,[ContractEndDate]
      ,[Insurance]
      ,[InsuranceNumber]
      ,[InsuranceExpireDate]
      ,[InsuranceBeginDate]
      ,[Education]
      ,[Reward]
      ,[ActivityAmount]
      ,[SupervisorSalary]
      ,[Active]
      ,[Major]
      ,[MajorCurrent]
      ,[EducationalStatus]
      ,[PersonType]
      ,[ShiftActivity]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[People]
SET IDENTITY_INSERT [dbo].[People] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[PersonFiles] ON
INSERT INTO [dbo].[PersonFiles]
		([Id]
      ,[PersonId]
      ,[Category]
      ,[Title]
      ,[FileName]
      ,[FileType])
SELECT * FROM [KiaGalleryMainContext].[dbo].[PersonFiles]
SET IDENTITY_INSERT [dbo].[PersonFiles] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[BranchesPaymentsSettings] ON
INSERT INTO [dbo].[BranchesPaymentsSettings]
		([Id]
      ,[Key]
      ,[Value]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[BranchesPaymentsSettings]
SET IDENTITY_INSERT [dbo].[BranchesPaymentsSettings] OFF
GO


GO
SET IDENTITY_INSERT [Bot].[BotMessage] ON
INSERT INTO [Bot].[BotMessage]
		([Id]
      ,[ChatId]
      ,[MessageId]
      ,[Text]
      ,[CreatedDate]
      ,[Unknown]
      ,[ReplayId]
      ,[ReplayMessageId]
      ,[FileType]
      ,[FileName]
      ,[FileId]
      ,[SubmitedUser])
SELECT * FROM [KiaGalleryMainContext].[Bot].[BotMessage]
SET IDENTITY_INSERT [Bot].[BotMessage] OFF
GO


GO
SET IDENTITY_INSERT [Bot].[BotNews] ON
INSERT INTO [Bot].[BotNews]
		([Id]
      ,[Type]
      ,[Text]
      ,[TextFa]
      ,[FileName]
      ,[FileId]
      ,[CreatedDate]
      ,[ExpiredDate]
      ,[SubmitedUser])
SELECT * FROM [KiaGalleryMainContext].[Bot].[BotNews]
SET IDENTITY_INSERT [Bot].[BotNews] OFF
GO


GO
SET IDENTITY_INSERT [Bot].[BotOrder] ON
INSERT INTO [Bot].[BotOrder]
		([Id]
      ,[UserId]
      ,[ChatId]
      ,[OrderSerial]
      ,[ProductId]
      ,[Size]
      ,[FirstName]
      ,[LastName]
      ,[PhoneNumber]
      ,[Status]
      ,[Deposit]
      ,[CardDetails]
      ,[PaymentType]
      ,[Address]
      ,[Description]
      ,[CreatedDate])
SELECT * FROM [KiaGalleryMainContext].[Bot].[BotOrder]
SET IDENTITY_INSERT [Bot].[BotOrder] OFF
GO


GO
SET IDENTITY_INSERT [Bot].[BotOrderLog] ON
INSERT INTO [Bot].[BotOrderLog]
		([Id]
      ,[OrderId]
      ,[Status]
      ,[Description]
      ,[CreateUserId]
      ,[CreateDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[Bot].[BotOrderLog]
SET IDENTITY_INSERT [Bot].[BotOrderLog] OFF
GO


GO
SET IDENTITY_INSERT [Bot].[BotSettings] ON
INSERT INTO [Bot].[BotSettings]
		( [Id]
      ,[Key]
      ,[Value]
      ,[ValueFa]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[Bot].[BotSettings]
SET IDENTITY_INSERT [Bot].[BotSettings] OFF
GO


GO
SET IDENTITY_INSERT [Bot].[BotUserData] ON
INSERT INTO [Bot].[BotUserData]
		( [Id]
      ,[UserType]
      ,[UserId]
      ,[ChatId]
      ,[FirstName]
      ,[LastName]
      ,[Username]
      ,[Stoped]
      ,[Language]
      ,[CreatedDate])
SELECT * FROM [KiaGalleryMainContext].[Bot].[BotUserData]
SET IDENTITY_INSERT [Bot].[BotUserData] OFF
GO


GO
SET IDENTITY_INSERT [Bot].[Broadcast] ON
INSERT INTO [Bot].[Broadcast]
		( [Id]
      ,[BroadcastType]
      ,[Text]
      ,[TextFa]
      ,[FileName]
      ,[FileId]
      ,[ProductId]
      ,[CreatedDate]
      ,[Sended]
      ,[SubmitedUser])
SELECT * FROM [KiaGalleryMainContext].[Bot].[Broadcast]
SET IDENTITY_INSERT [Bot].[Broadcast] OFF
GO


GO
SET IDENTITY_INSERT [Bot].[Instagram] ON
INSERT INTO [Bot].[Instagram]
		( [Id]
      ,[InstagramId]
      ,[Type]
      ,[Url]
      ,[Caption]
      ,[FileId]
      ,[Sended]
      ,[CreatedDate])
SELECT * FROM [KiaGalleryMainContext].[Bot].[Instagram]
SET IDENTITY_INSERT [Bot].[Instagram] OFF
GO


GO
SET IDENTITY_INSERT [Bot].[Replay] ON
INSERT INTO [Bot].[Replay]
		([Id]
      ,[Text]
      ,[CreatedDate])
SELECT * FROM [KiaGalleryMainContext].[Bot].[Replay]
SET IDENTITY_INSERT [Bot].[Replay] OFF
GO


GO
SET IDENTITY_INSERT [crm].[CrmInvoice] ON
INSERT INTO [crm].[CrmInvoice]
		([Id]
      ,[CustomerId]
      ,[InvoiceType]
      ,[Amount]
      ,[Discount]
      ,[DiscountPercent]
      ,[GoldPrice]
      ,[BranchId]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[CreateIp]
      ,[ModifyIp])
SELECT * FROM [KiaGalleryMainContext].[crm].[CrmInvoice]
SET IDENTITY_INSERT [crm].[CrmInvoice] OFF
GO


GO
SET IDENTITY_INSERT [crm].[CrmInvoiceDetail] ON
INSERT INTO [crm].[CrmInvoiceDetail]
		([Id]
      ,[InvoiceId]
      ,[Barcode]
      ,[Revocation]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[CreateIp]
      ,[ModifyIp])
SELECT * FROM [KiaGalleryMainContext].[crm].[CrmInvoiceDetail]
SET IDENTITY_INSERT [crm].[CrmInvoiceDetail] OFF
GO


GO
SET IDENTITY_INSERT [crm].[Custromer] ON
INSERT INTO [crm].[Custromer]
		([Id]
      ,[Barcode]
      ,[FirstName]
      ,[LastName]
      ,[NationalityCode]
      ,[MobileNo]
      ,[Sex]
      ,[BirthDate]
      ,[WeddingDate]
      ,[Balance]
      ,[Active]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[CreateIp]
      ,[ModifyIp])
SELECT * FROM [KiaGalleryMainContext].[crm].[Custromer]
SET IDENTITY_INSERT [crm].[Custromer] OFF
GO


GO
SET IDENTITY_INSERT [crm].[DiscountSetting] ON
INSERT INTO [crm].[DiscountSetting]
		([Id]
      ,[FromPrice]
      ,[ToPrice]
      ,[Discount]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[CreateIp]
      ,[ModifyIp])
SELECT * FROM [KiaGalleryMainContext].[crm].[DiscountSetting]
SET IDENTITY_INSERT [crm].[DiscountSetting] OFF
GO


GO
SET IDENTITY_INSERT [dbo].[BranchesPayments] ON
INSERT INTO [dbo].[BranchesPayments]
		([Id]
      ,[TypePayments]
      ,[Deposits]
      ,[GoldAmount]
      ,[GoldWage]
      ,[GoldFee]
      ,[GoldWeights]
      ,[StoneAmount]
      ,[Number]
      ,[GoldCarat]
      ,[DifferentType]
      ,[BranchId]
      ,[GoldDebt]
      ,[RialDebt]
      ,[Description]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[BranchesPayments]
SET IDENTITY_INSERT [dbo].[BranchesPayments] OFF
GO


GO
SET IDENTITY_INSERT [dbo].[BranchesPaymentsDetails] ON
INSERT INTO [dbo].[BranchesPaymentsDetails]
		([Id]
      ,[BranchesPaymentsId]
      ,[GoldWage]
      ,[GoldWeights]
      ,[Amount]
      ,[Number]
      ,[Title]
      ,[Code])
SELECT * FROM [KiaGalleryMainContext].[dbo].[BranchesPaymentsDetails]
SET IDENTITY_INSERT [dbo].[BranchesPaymentsDetails] OFF
GO


GO
SET IDENTITY_INSERT [dbo].[BranchesPaymentsMessages] ON
INSERT INTO [dbo].[BranchesPaymentsMessages]
		([Id]
      ,[BranchId]
      ,[UserId]
      ,[ChatId]
      ,[MessageId]
      ,[Text]
      ,[CreateDate]
      ,[Unknown])
SELECT * FROM [KiaGalleryMainContext].[dbo].[BranchesPaymentsMessages]
SET IDENTITY_INSERT [dbo].[BranchesPaymentsMessages] OFF
GO


GO
SET IDENTITY_INSERT [dbo].[BranchesPaymentsSendMessages] ON
INSERT INTO [dbo].[BranchesPaymentsSendMessages]
		([Id]
      ,[UserId]
      ,[ChatId]
      ,[MessageId]
      ,[BranchesPaymentsId]
      ,[Text]
      ,[CreateDate])
SELECT * FROM [KiaGalleryMainContext].[dbo].[BranchesPaymentsSendMessages]
SET IDENTITY_INSERT [dbo].[BranchesPaymentsSendMessages] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[InquiryProducts] ON
INSERT INTO [dbo].[InquiryProducts]
		([Id]
      ,[BranchId]
      ,[Comment]
      ,[ProductId]
      ,[FileName]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[InquiryProducts]
SET IDENTITY_INSERT [dbo].[InquiryProducts] OFF
GO


GO
SET IDENTITY_INSERT [dbo].[InquiryProductReplies] ON
INSERT INTO [dbo].[InquiryProductReplies]
		([Id]
      ,[BranchId]
      ,[InquiryProductId]
      ,[AnswerType]
      ,[Comment]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[CreateIp]
      ,[ModifyIp])
SELECT * FROM [KiaGalleryMainContext].[dbo].[InquiryProductReplies]
SET IDENTITY_INSERT [dbo].[InquiryProductReplies] OFF
GO


GO
SET IDENTITY_INSERT [dbo].[Salaries] ON
INSERT INTO [dbo].[Salaries]
		([Id]
      ,[PersonId]
      ,[WorkHours]
      ,[HourlyWageRate]
      ,[OvertimeRate]
      ,[Mission]
      ,[Reward]
      ,[Remuneration]
      ,[Others]
      ,[LoanInstallment]
      ,[Imprest]
      ,[CommodityItem]
      ,[MonthCalculated]
      ,[Insurance]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[Salaries]
SET IDENTITY_INSERT [dbo].[Salaries] OFF
GO


GO
SET IDENTITY_INSERT [dbo].[StoneOutOfStocks] ON
INSERT INTO [dbo].[StoneOutOfStocks]
		([Id]
      ,[StoneId]
      ,[ShapeSizeId]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[StoneOutOfStocks]
SET IDENTITY_INSERT [dbo].[StoneOutOfStocks] OFF
GO


GO
SET IDENTITY_INSERT [dbo].[UserTokens] ON
INSERT INTO [dbo].[UserTokens]
		([Id]
      ,[UserId]
      ,[AuthoritarianToken]
      ,[CreatedDateTime]
      ,[ExpiredDateTime]
      ,[CreatedIp])
SELECT * FROM [KiaGalleryMainContext].[dbo].[UserTokens]
SET IDENTITY_INSERT [dbo].[UserTokens] OFF
GO


GO
SET IDENTITY_INSERT [drf].[AppToken] ON
INSERT INTO [drf].[AppToken]
		([Id]
      ,[UserId]
      ,[Code]
      ,[Voided]
      ,[TokenType]
      ,[CreateDate]
      ,[VoidedDate])
SELECT * FROM [KiaGalleryMainContext].[drf].[AppToken]
SET IDENTITY_INSERT [drf].[AppToken] OFF
GO


GO
SET IDENTITY_INSERT [drf].[Bank] ON
INSERT INTO [drf].[Bank]
		([Id]
      ,[BranchId]
      ,[OrderNo]
      ,[Name]
      ,[Active]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[drf].[Bank]
SET IDENTITY_INSERT [drf].[Bank] OFF
GO


GO
SET IDENTITY_INSERT [drf].[BranchCalendar] ON
INSERT INTO [drf].[BranchCalendar]
		( [Id]
      ,[BranchId]
      ,[ReportDate]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[drf].[BranchCalendar]
SET IDENTITY_INSERT [drf].[BranchCalendar] OFF
GO


GO
SET IDENTITY_INSERT [drf].[Currency] ON
INSERT INTO [drf].[Currency]
		([Id]
      ,[OrderNo]
      ,[Name]
      ,[Active]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[drf].[Currency]
SET IDENTITY_INSERT [drf].[Currency] OFF
GO


GO
SET IDENTITY_INSERT [drf].[DailyReport] ON
INSERT INTO [drf].[DailyReport]
		([Id]
      ,[BranchId]
      ,[BranchCalendarId]
      ,[Status]
      ,[NumberSaleFactor]
      ,[SaleWeight]
      ,[SaleEntry]
      ,[SaleExit]
      ,[NumberReturnedFactor]
      ,[ReturnedWeight]
      ,[ReturnedEntry]
      ,[ReturnedExit]
      ,[OtherCash]
      ,[CashEntry]
      ,[CashExit]
      ,[OtherCurrency]
      ,[OtherCurrencyValue]
      ,[OtherCurrencyRialValue]
      ,[OtherCurrencyRialEntry]
      ,[OtherCurrencyRialExit]
      ,[InventoryCash]
      ,[GoldDeficitWeight]
      ,[GoldDeficitEntry]
      ,[GoldDeficitExit]
      ,[GiftNumber]
      ,[GiftEntry]
      ,[GiftExit]
      ,[CheckNumber]
      ,[CheckEntry]
      ,[CheckExit]
      ,[LeatherStoneDescription]
      ,[LeatherStoneEntry]
      ,[LeatherStoneExit]
      ,[CoinNumber]
      ,[CoinDescription]
      ,[CoinEntry]
      ,[CoinExit]
      ,[OtherKiaGoldWeight]
      ,[OtherKiaGoldEntry]
      ,[OtherKiaGoldExit]
      ,[OtherGoldWeight]
      ,[OtherGoldEntry]
      ,[OtherGoldExit]
      ,[CreditorCustomerEntry]
      ,[CreditorCustomerExit]
      ,[DebtorCustomerEntry]
      ,[DebtorCustomerExit]
      ,[DepositBeforeCount]
      ,[DepositBeforeEntry]
      ,[DepositBeforeExit]
      ,[DepositNewCount]
      ,[DepositNewEntry]
      ,[DepositNewExit]
      ,[DiscountEntry]
      ,[DiscountExit]
      ,[CostCourierPostEntry]
      ,[CostCourierPostExit]
      ,[CostEntry]
      ,[CostExit]
      ,[Description]
      ,[Sent])
	SELECT * FROM [KiaGalleryMainContext].[drf].[DailyReport]
SET IDENTITY_INSERT [drf].[DailyReport] OFF
GO


GO
SET IDENTITY_INSERT [drf].[DailyReportBank] ON
INSERT INTO [drf].[DailyReportBank]
		([Id]
      ,[DailyReportId]
      ,[BankId]
      ,[Entry]
      ,[Exit])
SELECT * FROM [KiaGalleryMainContext].[drf].[DailyReportBank]
SET IDENTITY_INSERT [drf].[DailyReportBank] OFF
GO


GO
SET IDENTITY_INSERT [drf].[DailyReportCurrency] ON
INSERT INTO [drf].[DailyReportCurrency]
		([Id]
      ,[DailyReportId]
      ,[CurrencyId]
      ,[Value]
      ,[RialValue]
      ,[RialEntry]
      ,[RialExit])
SELECT * FROM [KiaGalleryMainContext].[drf].[DailyReportCurrency]
SET IDENTITY_INSERT [drf].[DailyReportCurrency] OFF
GO


GO
SET IDENTITY_INSERT [drf].[DailyReportLog] ON
INSERT INTO [drf].[DailyReportLog]
		([Id]
      ,[DailyReportId]
      ,[UserId]
      ,[Date]
      ,[Status]
	  ,[PrevData]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[drf].[DailyReportLog]
SET IDENTITY_INSERT [drf].[DailyReportLog] OFF
GO

GO
SET IDENTITY_INSERT [drf].[BotDailyReportUserData] ON
INSERT INTO [drf].[BotDailyReportUserData]
           ([Id]
		   ,[UserType]
           ,[UserId]
           ,[ModifyUserId]
           ,[BranchId]
           ,[ChatId]
           ,[FirstName]
           ,[LastName]
           ,[Username]
           ,[Stoped]
           ,[CreatedDate]
           ,[BotUserType])
SELECT * FROM [KiaGalleryMainContext].[drf].[BotDailyReportUserData]
SET IDENTITY_INSERT [drf].[BotDailyReportUserData] OFF
GO

GO
SET IDENTITY_INSERT [drf].[DailyReportMessage] ON
INSERT INTO [drf].[DailyReportMessage]
           ([Id]
		   ,[BranchId]
           ,[UserId]
           ,[ChatId]
           ,[MessageId]
           ,[Text]
           ,[CreateDate]
           ,[Unknown])
SELECT * FROM [KiaGalleryMainContext].[drf].[DailyReportMessage]
SET IDENTITY_INSERT [drf].[DailyReportMessage] OFF
GO

GO
SET IDENTITY_INSERT [drf].[DailyReportSettings] ON
INSERT INTO [drf].[DailyReportSettings]
           ([Id]
		   ,[Key]
           ,[Value]
           ,[CreateUserId]
           ,[ModifyUserId]
           ,[CreateDate]
           ,[ModifyDate]
           ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[drf].[DailyReportSettings]
SET IDENTITY_INSERT [drf].[DailyReportSettings] OFF
GO

GO
SET IDENTITY_INSERT [order].[BotOrderLeather] ON
INSERT INTO [order].[BotOrderLeather]
		([Id]
      ,[OrderId]
      ,[OrderNo]
      ,[LeatherId])
SELECT * FROM [KiaGalleryMainContext].[order].[BotOrderLeather]
SET IDENTITY_INSERT [order].[BotOrderLeather] OFF
GO

GO
SET IDENTITY_INSERT [order].[BotOrderStone] ON
INSERT INTO [order].[BotOrderStone]
		([Id]
      ,[OrderId]
      ,[OrderNo]
      ,[StoneId])
SELECT * FROM [KiaGalleryMainContext].[order].[BotOrderStone]
SET IDENTITY_INSERT [order].[BotOrderStone] OFF
GO


GO
SET IDENTITY_INSERT [order].[BranchNote] ON
INSERT INTO [order].[BranchNote]
		([Id]
      ,[BranchId]
      ,[Text]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[order].[BranchNote]
SET IDENTITY_INSERT [order].[BranchNote] OFF
GO


GO
SET IDENTITY_INSERT [dbo].[PhotographyManages] ON
INSERT INTO [dbo].[PhotographyManages]
		([Id]
      ,[ProductId]
      ,[FirstPhotography]
      ,[SecondPhotography]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[PhotographyManages]
SET IDENTITY_INSERT [dbo].[PhotographyManages] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[BankAccounts] ON
INSERT INTO [dbo].[BankAccounts]
		([Id]
      ,[Title]
      ,[Bank]
      ,[FirstName]
      ,[LastName]
      ,[PhoneNumber]
      ,[Telephone]
      ,[Organ]
      ,[CardNumber]
      ,[AccountNumber]
      ,[Iban]
      ,[Explanation]
      ,[Description]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[BankAccounts]
SET IDENTITY_INSERT [dbo].[BankAccounts] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[BankAccountDetails] ON
INSERT INTO [dbo].[BankAccountDetails]
		([Id]
	  ,[BankAccountId]
      ,[Title]
      ,[Bank]
      ,[CardNumber]
      ,[AccountNumber]
      ,[Iban]
      ,[Explanation]
      ,[Description]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[BankAccountDetails]
SET IDENTITY_INSERT [dbo].[BankAccountDetails] OFF
GO

GO
SET IDENTITY_INSERT [internalOrder].[InternalOrder] ON
INSERT INTO [internalOrder].[InternalOrder]
		([Id]
      ,[BranchId]
      ,[Date]
      ,[BarcodeDate]
      ,[MultiOrder]
      ,[Name]
      ,[Gender]
      ,[PhoneNumber]
      ,[Telephone]
      ,[OrderType]
      ,[Deposit]
      ,[Set]
      ,[Barcode]
      ,[TrackCode]
      ,[Description]
      ,[InternalOrderStatus]
      ,[DeliveryType]
      ,[UserType]
      ,[DeliveredBranchId]
      ,[PonyUp]
      ,[CreatePersonId]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip]
	  )
SELECT * FROM [KiaGalleryMainContext].[internalOrder].[InternalOrder]
SET IDENTITY_INSERT [internalOrder].[InternalOrder] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[InternalOrderDetails] ON
INSERT INTO [dbo].[InternalOrderDetails]
		( [Id]
      ,[InternalOrderId]
      ,[BookCode]
      ,[SiteCode]
      ,[ProductId]
      ,[LeatherId]
      ,[StoneId]
      ,[LeatherLoop]
      ,[Size]
      ,[GoldType]
      ,[ProductType]
      ,[Title]
      ,[Ip]
      ,[Leather_Id]
      ,[Stone_Id]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[InternalOrderDetails]
SET IDENTITY_INSERT [dbo].[InternalOrderDetails] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[InternalOrderLogs] ON
INSERT INTO [dbo].[InternalOrderLogs]
		( [Id]
      ,[InternalOrderId]
      ,[UserId]
      ,[Text]
      ,[CreatedDate]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[InternalOrderLogs]
SET IDENTITY_INSERT [dbo].[InternalOrderLogs] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[InternalOrderStatusLogs] ON
INSERT INTO [dbo].[InternalOrderStatusLogs]
		( [Id]
      ,[InternalOrderId]
      ,[UserId]
      ,[InternalOrderStatus]
      ,[RemindDate]
      ,[CreateDate]
      ,[Ip]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[InternalOrderStatusLogs]
SET IDENTITY_INSERT [dbo].[InternalOrderStatusLogs] OFF
GO




GO
SET IDENTITY_INSERT [customerLoyality].[CustomerLoyality] ON
INSERT INTO [customerLoyality].[CustomerLoyality]
		([Id]
      ,[FirstName]
      ,[LastName]
      ,[PhoneNumber]
      ,[Date]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip]
	  )
SELECT * FROM [KiaGalleryMainContext].[customerLoyality].[CustomerLoyality]
SET IDENTITY_INSERT [customerLoyality].[CustomerLoyality] OFF
GO

GO
SET IDENTITY_INSERT [customerLoyality].[CustomerFactor] ON
INSERT INTO [customerLoyality].[CustomerFactor]
		([Id]
      ,[BranchId]
      ,[CustomerLoyalityId]
      ,[FactorPrice]
      ,[FactorNumber]
      ,[FactorWeight]
	  ,[ProductCode]
      ,[PurchaseType]
      ,[Date]
	  ,[ReturnDate]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip]
	  )
SELECT * FROM [KiaGalleryMainContext].[customerLoyality].[CustomerFactor]
SET IDENTITY_INSERT [customerLoyality].[CustomerFactor] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[CategoryQuestions] ON
INSERT INTO [dbo].[CategoryQuestions]
		([Id]
      ,[Title]
      ,[Order]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[CreateIp]
      ,[ModifyIp]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[CategoryQuestions]
SET IDENTITY_INSERT [dbo].[CategoryQuestions] OFF
GO


GO
SET IDENTITY_INSERT [dbo].[CrmCustomerAnswers] ON
INSERT INTO [dbo].[CrmCustomerAnswers]
		([Id]
      ,[CrmCustomerId]
      ,[CrmQuestionId]
      ,[YesNoAnswer]
      ,[CrmQuestionValueId]
      ,[MultiAnswer]
      ,[MultiAnswerValue]
      ,[SingleAnswer]
      ,[SingleAnswerValue]
      ,[DescriptiveAnswer]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip]
      ,[CrmQuestionValue_Id]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[CrmCustomerAnswers]
SET IDENTITY_INSERT [dbo].[CrmCustomerAnswers] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[CrmCustomers] ON
INSERT INTO [dbo].[CrmCustomers]
		([Id]
      ,[BranchId]
      ,[FullName]
      ,[PhoneNumber]
      ,[BuyType]
      ,[BuyTypeSubset]
      ,[BuyTypeOnline]
      ,[FactorNumber]
      ,[Score]
      ,[Date]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[CrmCustomers]
SET IDENTITY_INSERT [dbo].[CrmCustomers] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[CrmQuestions] ON
INSERT INTO [dbo].[CrmQuestions]
		([Id]
      ,[Order]
      ,[CategoryQuestionId]
      ,[CrmQuestionType]
      ,[BuyType]
      ,[BuyTypeSubset]
      ,[BuyTypeOnline]
      ,[Title]
      ,[DefaultYesNo]
      ,[DefaultDescriptive]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[CrmQuestions]
SET IDENTITY_INSERT [dbo].[CrmQuestions] OFF
GO


GO
SET IDENTITY_INSERT [dbo].[CrmQuestionValues] ON
INSERT INTO [dbo].[CrmQuestionValues]
		([Id]
      ,[CrmQuestionId]
      ,[Value]
      ,[DefaultSelected]
      ,[Description]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[CrmQuestionValues]
SET IDENTITY_INSERT [dbo].[CrmQuestionValues] OFF
GO


--GO
--SET IDENTITY_INSERT [dbo].[DailyReportMessages] ON
--INSERT INTO [dbo].[DailyReportMessages]
--		([Id]
--      ,[BranchId]
--      ,[UserId]
--      ,[ChatId]
--      ,[MessageId]
--      ,[Text]
--      ,[CreateDate]
--      ,[Unknown]
--	  )
--SELECT * FROM [KiaGalleryMainContext].[dbo].[DailyReportMessages]
--SET IDENTITY_INSERT [dbo].[DailyReportMessages] OFF
--GO



--GO
--SET IDENTITY_INSERT [dbo].[DailyReportSettings] ON
--INSERT INTO [dbo].[DailyReportSettings]
--		([Id]
--      ,[Key]
--      ,[Value]
--      ,[CreateUserId]
--      ,[ModifyUserId]
--      ,[CreateDate]
--      ,[ModifyDate]
--      ,[Ip]
--	  )
--SELECT * FROM [KiaGalleryMainContext].[dbo].[DailyReportSettings]
--SET IDENTITY_INSERT [dbo].[DailyReportSettings] OFF
--GO


GO
SET IDENTITY_INSERT [dbo].[GoldBalances] ON
INSERT INTO [dbo].[GoldBalances]
		([Id]
      ,[TradeTime]
      ,[TradeType]
      ,[DealerName]
      ,[Description]
      ,[Weight]
      ,[Date]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Workshop_Id]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[GoldBalances]
SET IDENTITY_INSERT [dbo].[GoldBalances] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[WorkShopGolds] ON
INSERT INTO [dbo].[WorkShopGolds]
		([Id]
      ,[WorkshopId]
      ,[Weight]
      ,[BoughtGoldPrice]
      ,[GoldRate]
      ,[RemittanceType]
      ,[Date]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[WorkShopGolds]
SET IDENTITY_INSERT [dbo].[WorkShopGolds] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[MarquisFiles] ON
INSERT INTO [dbo].[MarquisFiles]
		([Id]
		,[BranchId]
        ,[Date]
        ,[Description]
        ,[CreateUserId]
        ,[ModifyUserId]
        ,[CreateDate]
        ,[ModifyDate]
        ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[MarquisFiles]
SET IDENTITY_INSERT [dbo].[MarquisFiles] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[MarquisFileDetails] ON
INSERT INTO [dbo].[MarquisFileDetails]
		([Id]
		,[MarquisFileId]
		,[FileId]
		,[FileName])
SELECT * FROM [KiaGalleryMainContext].[dbo].[MarquisFileDetails]
SET IDENTITY_INSERT [dbo].[MarquisFileDetails] OFF
GO



GO
SET IDENTITY_INSERT [dbo].[SentSms] ON
INSERT INTO [dbo].[SentSms]
		( [Id]
      ,[Text]
      ,[DestinationNumber]
      ,[CreateUserId]
      ,[SendingDate]
      ,[Ip]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[SentSms]
SET IDENTITY_INSERT [dbo].[SentSms] OFF
GO

--GO
--SET IDENTITY_INSERT [dbo].[BranchFactor] ON
--INSERT INTO [dbo].[BranchFactor]
--		( [Id]
--      ,[BranchId]
--      ,[Date]
--	  ,[Number]
--	  ,[Date]
--      ,[CreateUserId]
--      ,[ModifyUserId]
--	  ,[CreateDate]
--      ,[ModifyDate]
--      ,[Ip]
--	  )
--SELECT * FROM [KiaGalleryMainContext].[dbo].[BranchFactor]
--SET IDENTITY_INSERT [dbo].[BranchFactor] OFF
--GO

--GO
--SET IDENTITY_INSERT [dbo].[InventoryDetail] ON
--INSERT INTO [dbo].[InventoryDetail]
--		( [Id]
--      ,[InventoryReportMemberId]
--      ,[Count]
--	  ,[Weight]
--	  ,[Date]
--      ,[CreateUserId]
--      ,[ModifyUserId]
--	  ,[CreateDate]
--      ,[ModifyDate]
--      ,[Ip]
--	  )
--SELECT * FROM [KiaGalleryMainContext].[dbo].[InventoryReportMember]
--SET IDENTITY_INSERT [dbo].[InventoryReportMember] OFF
--GO

--GO
--SET IDENTITY_INSERT [dbo].[InventoryReportMember] ON
--INSERT INTO [dbo].[InventoryReportMember]
--		( [Id]
--      ,[Order]
--      ,[Title]
--      ,[CreateUserId]
--      ,[ModifyUserId]
--	  ,[CreateDate]
--      ,[ModifyDate]
--      ,[Ip]
--	  )
--SELECT * FROM [KiaGalleryMainContext].[dbo].[InventoryReportMember]
--SET IDENTITY_INSERT [dbo].[InventoryReportMember] OFF
--GO


GO
SET IDENTITY_INSERT [dbo].[SmsCategories] ON
INSERT INTO [dbo].[SmsCategories]
		( [Id]
      ,[Title]
      ,[Color]
      ,[Active]
      ,[CategoryType]
      ,[Description]
      ,[FreeMessage]
      ,[Order]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[SmsCategories]
SET IDENTITY_INSERT [dbo].[SmsCategories] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[CreateMessages] ON
INSERT INTO [dbo].[CreateMessages]
		( [Id]
      ,[SmsCategoryId]
      ,[Name]
	  ,[DetailName]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip])
SELECT * FROM [KiaGalleryMainContext].[dbo].[CreateMessages]
SET IDENTITY_INSERT [dbo].[CreateMessages] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[SmsTexts] ON
INSERT INTO [dbo].[SmsTexts]
		([Id]
      ,[SmsCategoryId]
      ,[Title]
      ,[Text]
      ,[UrlKey]
      ,[Order]
      ,[Active]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[SmsTexts]
SET IDENTITY_INSERT [dbo].[SmsTexts] OFF
GO

--GO
--SET IDENTITY_INSERT [dbo].[CustomerSurveys] ON
--INSERT INTO [dbo].[CustomerSurveys]
--		([Id]
--      ,[CustomerFactorId]
--      ,[Code]
--      ,[CreateUserId]
--      ,[CreateDate]
--      ,[Ip])
--SELECT * FROM [KiaGalleryMainContext].[dbo].[CustomerSurveys]
--SET IDENTITY_INSERT [dbo].[CustomerSurveys] OFF
--GO

--GO
--SET IDENTITY_INSERT [dbo].[SurveyCustomerAnswers] ON
--INSERT INTO [dbo].[SurveyCustomerAnswers]
--		([Id]
--      ,[SurveyQuestionId]
--      ,[CustomerSurveyId]
--      ,[SurveyAnswerType]
--      ,[Offer]
--      ,[CreateDate]
--      ,[ModifyDate]
--      ,[Ip])
--SELECT * FROM [KiaGalleryMainContext].[dbo].[SurveyCustomerAnswers]
--SET IDENTITY_INSERT [dbo].[SurveyCustomerAnswers] OFF
--GO

--GO
--SET IDENTITY_INSERT [dbo].[SurveyQuestions] ON
--INSERT INTO [dbo].[SurveyQuestions]
--		([Id]
--      ,[Order]
--      ,[Title]
--      ,[QuestionType]
--      ,[CreateUserId]
--      ,[ModifyUserId]
--      ,[CreateDate]
--      ,[ModifyDate]
--      ,[Ip])
--SELECT * FROM [KiaGalleryMainContext].[dbo].[SurveyQuestions]
--SET IDENTITY_INSERT [dbo].[SurveyQuestions] OFF
--GO

GO
SET IDENTITY_INSERT [dbo].[BranchGolds] ON
INSERT INTO [dbo].[BranchGolds]
		([Id]
      ,[BranchId]
      ,[Weight]
	  ,[Price]
      ,[Date]
      ,[CreateUserId]
      ,[ModifyUserId]
      ,[CreateDate]
      ,[ModifyDate]
      ,[Ip]
	  )
SELECT * FROM [KiaGalleryMainContext].[dbo].[BranchGolds]
SET IDENTITY_INSERT [dbo].[BranchGolds] OFF
GO

--GO
--SET IDENTITY_INSERT [dbo].[FaxOrderUsableProduct] ON
--INSERT INTO [dbo].[FaxOrderUsableProduct]
--		([Id]
--      ,[ReceiverName]
--      ,[DateFaxSend]
--	    ,[Description]
--      ,[CreateUserId]
--      ,[ModifyUserId]
--      ,[CreateDate]
--      ,[ModifyDate]
--      ,[Ip]
--	  )
--SELECT * FROM [KiaGalleryMainContext].[dbo].[BranchGolds]
--SET IDENTITY_INSERT [dbo].[BranchGolds] OFF
--GO

--GO
--SET IDENTITY_INSERT [dbo].[OrderUsableProduct] ON
--INSERT INTO [dbo].[OrderUsableProduct]
--		([Id]
--      ,[BranchId]
--      ,[Description]
--	  ,[Count]
--      ,[CreateUserId]
--      ,[ModifyUserId]
--      ,[CreateDate]
--      ,[ModifyDate]
--      ,[Ip]
--	  )
--SELECT * FROM [KiaGalleryMainContext].[dbo].[OrderUsableProduct]
--SET IDENTITY_INSERT [dbo].[OrderUsableProduct] OFF
--GO


--GO
--SET IDENTITY_INSERT [dbo].[OrderUsableProductDetail] ON
--INSERT INTO [dbo].[OrderUsableProductDetail]
--		([Id]
--      ,[OrderUsableProductId]
--      ,[UsableProductId]
--	  ,[Description]
--	  ,[Count]
--	  ,[CreateUserId]
--      ,[ModifyUserId]
--      ,[CreateDate]
--      ,[ModifyDate]
--      ,[Ip]
--	  )
--SELECT * FROM [KiaGalleryMainContext].[dbo].[OrderUsableProductDetail]
--SET IDENTITY_INSERT [dbo].[OrderUsableProductDetail] OFF
--GO

--GO
--SET IDENTITY_INSERT [dbo].[UsableProduct] ON
--INSERT INTO [dbo].[UsableProduct]
--		([Id]
--      ,[CategoryUsableProductId]
--      ,[Name]
--	  ,[Code]
--	  ,[Image]
--	  ,[Rate]
--	  ,[Active]
--	  ,[CreateUserId]
--	  ,[ModifyUserId]
--      ,[CreateDate]
--      ,[ModifyDate]
--      ,[Ip]
--	  )
--SELECT * FROM [KiaGalleryMainContext].[dbo].[UsableProduct]
--SET IDENTITY_INSERT [dbo].[UsableProduct] OFF
--GO

--GO
--SET IDENTITY_INSERT [dbo].[CategoryUsableProduct] ON
--INSERT INTO [dbo].[CategoryUsableProduct]
--		([Id]
--      ,[CategoryUsableProductId]
--      ,[Title]
--	  ,[Order]
--	  ,[Active]
--	  ,[CreateUserId]
--	  ,[ModifyUserId]
--      ,[CreateDate]
--      ,[ModifyDate]
--      ,[Ip]
--	  )
--SELECT * FROM [KiaGalleryMainContext].[dbo].[CategoryUsableProduct]
--SET IDENTITY_INSERT [dbo].[CategoryUsableProduct] OFF
--GO

--GO
--SET IDENTITY_INSERT [dbo].[UsableProductCart] ON
--INSERT INTO [dbo].[UsableProductCart]
--		([Id]
--      ,[BranchId]
--      ,[UsableProductId]
--	  ,[Order]
--	  ,[Count]
--	  ,[Description]
--	  ,[CreateUserId]
--	  ,[ModifyUserId]
--      ,[CreateDate]
--      ,[ModifyDate]
--      ,[Ip]
--	  )
--SELECT * FROM [KiaGalleryMainContext].[dbo].[UsableProductCart]
--SET IDENTITY_INSERT [dbo].[UsableProductCart] OFF
--GO
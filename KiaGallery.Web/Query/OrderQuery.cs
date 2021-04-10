using KiaGallery.Common;
using KiaGallery.Model.Context;
using KiaGallery.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace KiaGallery.Web.Query
{
    public class OrderQuery
    {

        public static Response OrderSearchQuery(QueryUserType type, OrderSearchViewModel model)
        {

            var param = new List<SqlParameter>();
            var paramCount = new List<SqlParameter>();
            var countQuery = "SELECT COUNT(*) FROM ( SELECT DISTINCT [O].[Id] ";
            var query =
                @"
        SELECT DISTINCT
		[O].[Id],
		[O].[OrderNumber],
		[O].[OrderSerial],
		(
			SELECT COUNT(*) FROM [order].[OrderDetail] AS [SOD]
				INNER JOIN [Products] AS [SP] ON [SP].[Id] = [SOD].[ProductId]
            WHERE [SOD].[OrderId] = [O].[Id]
";
            if (type == QueryUserType.LeatherProduct)
            {
                query += @" AND ([SP].[WorkshopId2] = 5 OR [SP].[ProductType] IN (10, 15, 14)) ";
            }
            else if (type == QueryUserType.NotLeatherProduct)
            {
                query += @" AND (([SP].[WorkshopId2] IS NULL OR [SP].[WorkshopId2] != 5) AND [SP].[ProductType] NOT IN (10, 15, 14))";
            }

            query += @"
) AS [sumCount],
		(
			SELECT COUNT(DISTINCT [SOD].[SetNumber]) FROM [order].[OrderDetail] AS [SOD] 
				INNER JOIN [Products] AS [SP] ON [SP].[Id] = [SOD].[ProductId]
			WHERE [SOD].[OrderId] = [O].[Id] AND [SOD].[SetNumber] IS NOT NULL
";
            if (type == QueryUserType.LeatherProduct)
            {
                query += @" AND ([SP].[WorkshopId2] = 5 OR [SP].[ProductType] IN (10, 15, 14)) ";
            }
            else if (type == QueryUserType.NotLeatherProduct)
            {
                query += @" AND (([SP].[WorkshopId2] IS NULL OR [SP].[WorkshopId2] != 5) AND [SP].[ProductType] NOT IN (10, 15, 14))";
            }

            query += @"
		) AS [sumCountSet],
		(
			SELECT ROUND(SUM([SP].[Weight] * [SOD].[Count]), 2) FROM [order].[OrderDetail] AS [SOD]
				INNER JOIN [Products] [SP] ON [SP].[Id] = [SOD].[ProductId]
			WHERE [SOD].[OrderId] = [O].[Id]";

            if (type == QueryUserType.LeatherProduct)
            {
                query += @" AND ([SP].[WorkshopId2] = 5 OR [SP].[ProductType] IN (10, 15, 14)) ";
            }
            else if (type == QueryUserType.NotLeatherProduct)
            {
                query += @" AND (([SP].[WorkshopId2] IS NULL OR [SP].[WorkshopId2] != 5) AND [SP].[ProductType] NOT IN (10, 15, 14))";
            }

            query += @"
		) AS [sumWeight],
		[O].[BranchId] AS [BranchId],
		[B].[Name] AS [createBranch],
		[U].[FirstName] + ' ' + [U].[LastName] AS [createUser],
		[O].[createDate],
		(
			CASE 
				WHEN 
					(
						SELECT COUNT(*) FROM [order].[OrderDetail] AS [SOD]
				            INNER JOIN [Products] [SP] ON [SP].[Id] = [SOD].[ProductId]
			            WHERE [SOD].[OrderId] = [O].[Id]";
            if (type == QueryUserType.LeatherProduct)
            {
                query += @" AND ([SP].[WorkshopId2] = 5 OR [SP].[ProductType] IN (10, 15, 14)) ";
            }
            else if (type == QueryUserType.NotLeatherProduct)
            {
                query += @" AND (([SP].[WorkshopId2] IS NULL OR [SP].[WorkshopId2] != 5) AND [SP].[ProductType] NOT IN (10, 15, 14))";
            }

            query += @"
					)
					= 
					(
						SELECT COUNT(*) FROM [order].[OrderDetail] AS [SOD]
				            INNER JOIN [Products] [SP] ON [SP].[Id] = [SOD].[ProductId]
			            WHERE [SOD].[OrderId] = [O].[Id] AND [SOD].[OrderDetailStatus] IN (0)
						";
            if (type == QueryUserType.LeatherProduct)
            {
                query += @" AND ([SP].[WorkshopId2] = 5 OR [SP].[ProductType] IN (10, 15, 14)) ";
            }
            else if (type == QueryUserType.NotLeatherProduct)
            {
                query += @" AND (([SP].[WorkshopId2] IS NULL OR [SP].[WorkshopId2] != 5) AND [SP].[ProductType] NOT IN (10, 15, 14))";
            }

            query += @"
					)
				THEN 'bg-new-order'
				WHEN 
					(
						SELECT COUNT(*) FROM [order].[OrderDetail] AS [SOD]
				            INNER JOIN [Products] [SP] ON [SP].[Id] = [SOD].[ProductId]
			            WHERE [SOD].[OrderId] = [O].[Id]
						";
            if (type == QueryUserType.LeatherProduct)
            {
                query += @" AND ([SP].[WorkshopId2] = 5 OR [SP].[ProductType] IN (10, 15, 14)) ";
            }
            else if (type == QueryUserType.NotLeatherProduct)
            {
                query += @" AND (([SP].[WorkshopId2] IS NULL OR [SP].[WorkshopId2] != 5) AND [SP].[ProductType] NOT IN (10, 15, 14))";
            }

            query += @"
					)
					= 
					(
						SELECT COUNT(*) FROM [order].[OrderDetail] AS [SOD]
				            INNER JOIN [Products] [SP] ON [SP].[Id] = [SOD].[ProductId]
			            WHERE [SOD].[OrderId] = [O].[Id] AND [SOD].[OrderDetailStatus] IN (6, 9, 8)
						";
            if (type == QueryUserType.LeatherProduct)
            {
                query += @" AND ([SP].[WorkshopId2] = 5 OR [SP].[ProductType] IN (10, 15, 14)) ";
            }
            else if (type == QueryUserType.NotLeatherProduct)
            {
                query += @" AND (([SP].[WorkshopId2] IS NULL OR [SP].[WorkshopId2] != 5) AND [SP].[ProductType] NOT IN (10, 15, 14))";
            }

            query += @"
					)
				THEN 'bg-done-order'
				WHEN 
					(
						SELECT COUNT(*) FROM [order].[OrderDetail] AS [SOD]
				            INNER JOIN [Products] [SP] ON [SP].[Id] = [SOD].[ProductId]
			            WHERE [SOD].[OrderId] = [O].[Id]
						";
            if (type == QueryUserType.LeatherProduct)
            {
                query += @" AND ([SP].[WorkshopId2] = 5 OR [SP].[ProductType] IN (10, 15, 14)) ";
            }
            else if (type == QueryUserType.NotLeatherProduct)
            {
                query += @" AND (([SP].[WorkshopId2] IS NULL OR [SP].[WorkshopId2] != 5) AND [SP].[ProductType] NOT IN (10, 15, 14))";
            }

            query += @"
					)
					= 
					(
						SELECT COUNT(*) FROM [order].[OrderDetail] AS  [SOD]
				            INNER JOIN [Products] [SP] ON [SP].[Id] = [SOD].[ProductId]
			            WHERE [SOD].[OrderId] = [O].[Id] AND [SOD].[OrderDetailStatus] IN (6, 7, 8, 9)
						";
            if (type == QueryUserType.LeatherProduct)
            {
                query += @" AND ([SP].[WorkshopId2] = 5 OR [SP].[ProductType] IN (10, 15, 14)) ";
            }
            else if (type == QueryUserType.NotLeatherProduct)
            {
                query += @" AND (([SP].[WorkshopId2] IS NULL OR [SP].[WorkshopId2] != 5) AND [SP].[ProductType] NOT IN (10, 15, 14))";
            }

            query += @"
					)
				THEN 'bg-open-shortage-order'
				ELSE 'bg-open-order'
			END
		) AS [bgColor]";
            query += @"
FROM [order].[Order] AS [O]
	INNER JOIN [Branches] AS [B] ON [B].[Id] = [O].[BranchId]
	INNER JOIN [UserProfile] AS [U] ON [U].[Id] = [O].[CreateUserId]
	INNER JOIN [order].[OrderDetail] AS [OD] ON [OD].[OrderId] = [O].[Id]
	INNER JOIN [Products] AS [P] ON [P].[Id] = [OD].[ProductId]
WHERE [O].[Deleted] = 0
";
            countQuery += @"
FROM [order].[Order] AS [O]
	INNER JOIN [Branches] AS [B] ON [B].[Id] = [O].[BranchId]
	INNER JOIN [UserProfile] AS [U] ON [U].[Id] = [O].[CreateUserId]
	INNER JOIN [order].[OrderDetail] AS [OD] ON [OD].[OrderId] = [O].[Id]
	INNER JOIN [Products] AS [P] ON [P].[Id] = [OD].[ProductId]
WHERE [O].[Deleted] = 0
";

            if (type == QueryUserType.LeatherProduct)
            {
                query += @" AND ([P].[WorkshopId2] = 5 OR [P].[ProductType] = 10 OR [P].[ProductType] = 15 OR [P].[ProductType] = 14) ";
                countQuery += @" AND ([P].[WorkshopId2] = 5 OR [P].[ProductType] = 10 OR [P].[ProductType] = 15 OR [P].[ProductType] = 14) ";
            }
            else if (type == QueryUserType.NotLeatherProduct)
            {
                query += @" AND (([P].[WorkshopId2] IS NULL OR [P].[WorkshopId2] != 5) AND [P].[ProductType] NOT IN (10, 15, 14)) ";
                countQuery += @" AND (([P].[WorkshopId2] IS NULL OR [P].[WorkshopId2] != 5) AND [P].[ProductType] NOT IN (10, 15, 14)) ";
            }

            if (model.branchType != null)
            {
                query += @" AND [B].[BranchType] = @BranchType ";
                countQuery += @" AND [B].[BranchType] = @BranchType ";
                param.Add(new SqlParameter("@BranchType", model.branchType));
                paramCount.Add(new SqlParameter("@BranchType", model.branchType));
            }

            if (!string.IsNullOrEmpty(model.date))
            {
                var date = DateUtility.GetDateTime(model.date);
                query += @" AND ([O].[CreateDate] >= @Date AND [O].[CreateDate] < DATEADD(DAY, 1, @Date)) ";
                countQuery += @" AND ([O].[CreateDate] >= @Date AND [O].[CreateDate] < DATEADD(DAY, 1, @Date)) ";
                param.Add(new SqlParameter("@Date", date));
                paramCount.Add(new SqlParameter("@Date", date));
            }

            if (model.branchId != null && model.branchId.Count > 0)
            {
                query += @" AND [O].[BranchId] IN (SELECT VALUE FROM STRING_SPLIT(@BranchId, '-')) ";
                countQuery += @" AND [O].[BranchId] IN (SELECT VALUE FROM STRING_SPLIT(@BranchId, '-')) ";
                param.Add(new SqlParameter("@BranchId", string.Join("-", model.branchId)));
                paramCount.Add(new SqlParameter("@BranchId", string.Join("-", model.branchId)));
            }


            if (!string.IsNullOrEmpty(model.term?.Trim()))
            {
                model.term = model.term.ToStandardPersian();
                query += @" AND (
		[O].[OrderSerial] LIKE @Term OR
		[O].[OrderNumber] LIKE @Term OR
		[B].[Name] LIKE @Term OR
		[B].[Alias] LIKE @Term OR
		[OD].[Customer] LIKE @Term OR
		[OD].[PhoneNumber] LIKE @Term OR
		[OD].[Description] LIKE @Term) ";
                param.Add(new SqlParameter("@Term", "%" + model.term + "%"));
                countQuery += @" AND (
		[O].[OrderSerial] LIKE @Term OR
		[O].[OrderNumber] LIKE @Term OR
		[B].[Name] LIKE @Term OR
		[B].[Alias] LIKE @Term OR
		[OD].[Customer] LIKE @Term OR
		[OD].[PhoneNumber] LIKE @Term OR
		[OD].[Description] LIKE @Term) ";
                paramCount.Add(new SqlParameter("@Term", "%" + model.term + "%"));
            }


            query += @"
ORDER BY [O].[OrderNumber] DESC 
OFFSET     @Skip ROWS 
FETCH NEXT @Take ROWS ONLY;
";
            param.Add(new SqlParameter("@Skip", model.page * model.count));
            param.Add(new SqlParameter("@Take", model.count));

            countQuery += " ) AS A ";

            using (var db = new KiaGalleryContext())
            {
                var total = db.Database.SqlQuery<int>(countQuery, paramCount.ToArray()).First();
                var result = db.Database.SqlQuery<OrderQueryViewModel>(query, param.ToArray()).ToList();
                result.ForEach(x =>
                {
                    x.persianDate = DateUtility.GetPersianDate(x.createDate);
                });
                var response = new Response()
                {
                    status = 200,
                    data = new
                    {
                        list = result,
                        pageCount = Math.Ceiling((double)total / model.count),
                        count = total,
                        page = model.page + 1,
                    }
                };
                return response;
            }
        }
    }


    public enum QueryUserType
    {
        Admin,
        LeatherProduct,
        NotLeatherProduct
    }

    public class OrderQueryViewModel
    {
        public int id { get; set; }
        public string orderNumber { get; set; }
        public string orderSerial { get; set; }
        public int? sumCount { get; set; }
        public int? sumCountSet { get; set; }
        public double? sumWeight { get; set; }
        public int? branchId { get; set; }
        public string createBranch { get; set; }
        public string createUser { get; set; }
        public string persianDate { get; set; }
        public DateTime? createDate { get; set; }
        public string bgColor { get; set; }

    }
}
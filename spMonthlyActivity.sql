USE [WMS5]
GO
/****** Object:  StoredProcedure [dbo].[spMonthlyActivity]    Script Date: 04/07/2018 21:16:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER proc [dbo].[spMonthlyActivity](@date datetime, @type varchar(20))
as

declare @endDate datetime = dateadd(month, 1, @date)
	
declare @table table
(
	Pallets int,	
	Activity varchar(10),
	[Date] date,
	BeginDate date,
	EndDate date
);

while @date < getdate()
begin	
	with cte1 as
	(
		select
			case when @date > getdate() then null
			else count(distinct i.Id)
		end [Quantity],
		@type [Type], @date [Date], cast(@date as date) [Begin], cast(@endDate as date) [End]
		from [Transaction] t
		left join Item i
		on t.ItemId = i.Id
		where [Type] = @type
		and [Date] >= @date
		and [Date] < @endDate
		and i.StackId is null
	),
	cte2 as
	(
		select
			case when @date > getdate() then null
			else count(distinct s.Id)
		end [Quantity],
		@type [Type], @date [Date], cast(@date as date) [Begin], cast(@endDate as date) [End]
		from Stack s
		join Item i
		on s.Id = i.StackId
		join [Transaction] t
		on i.Id = t.ItemId
		where [Type] = @type
		and [Date] >= @date
		and [Date] < @endDate
	)

	insert @table
	select cte1.quantity + cte2.quantity [Quantity], cte1.[Type], cte1.[Date],  cte1.[Begin], cte1.[End]
	from cte1 join cte2
	on cte1.Date = cte2.Date
	and cte1.[Begin] = cte2.[Begin]
			
	set @date = dateadd(month, 1, @date);
	set @endDate = dateadd(month, 1, @date);
end

select * from @table
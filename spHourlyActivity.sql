USE [WMS5]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create proc [dbo].[spHourlyActivity](@date datetime, @type varchar(20))
as

declare @endDate datetime = dateadd(hour, 1, @date)
		
declare @table table
(
	Pallets int,	
	Activity varchar(10),
	[Date] date,
	BeginTime time,
	EndTime time,
	Interval varchar(50)
);

while @date < getdate()
begin	
	with cte1 as
	(
		select
			case when @date > getdate() then null
			else count(distinct i.Id)
		end [Quantity],
		@type [Type], @date [Date], cast(@date as time(0)) [Begin], cast(@endDate as time(0)) [End], cast(cast(@date as time(0)) as varchar) + ' - ' + cast(cast(@endDate as time(0)) as varchar) [Interval]		
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
		@type [Type], @date [Date], cast(@date as time(0)) [Begin], cast(@endDate as time(0)) [End], cast(cast(@date as time(0)) as varchar) + ' - ' + cast(cast(@endDate as time(0)) as varchar) [Interval]
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
	select cte1.quantity + cte2.quantity [Quantity], cte1.[Type], cte1.[Date],  cte1.[Begin], cte1.[End], cte1.Interval
	from cte1 join cte2
	on cte1.Date = cte2.Date
	and cte1.[Begin] = cte2.[Begin]
	
	if datepart(hour, @endDate) = 17
	begin
		set @date = dateadd(hour, 14, @date);		
	end
	else
	begin
		set @date = @endDate;		
	end	
	if datepart(dw, @date) = 1
	begin
		set @date = dateadd(hour, 24, @date)		
	end
	else if datepart(dw, @date) = 7
	begin
		set @date = dateadd(hour, 48, @date)		
	end	
	
	set @endDate = dateadd(hour, 1, @date)
end




select * from @table
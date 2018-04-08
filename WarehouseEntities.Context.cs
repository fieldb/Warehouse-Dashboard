﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataVisualization.UI
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class WMS5Entities : DbContext
    {
        public WMS5Entities()
            : base("name=WMS5Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Stack> Stacks { get; set; }
    
        public virtual ObjectResult<spAnalyzeActivity_Result> spAnalyzeActivity(Nullable<System.DateTime> date, string period)
        {
            var dateParameter = date.HasValue ?
                new ObjectParameter("date", date) :
                new ObjectParameter("date", typeof(System.DateTime));
    
            var periodParameter = period != null ?
                new ObjectParameter("period", period) :
                new ObjectParameter("period", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spAnalyzeActivity_Result>("spAnalyzeActivity", dateParameter, periodParameter);
        }
    
        public virtual ObjectResult<spHourlyActivity_Result> spHourlyActivity(Nullable<System.DateTime> date, string type)
        {
            var dateParameter = date.HasValue ?
                new ObjectParameter("date", date) :
                new ObjectParameter("date", typeof(System.DateTime));
    
            var typeParameter = type != null ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spHourlyActivity_Result>("spHourlyActivity", dateParameter, typeParameter);
        }
    
        public virtual ObjectResult<spDailyActivity_Result> spDailyActivity(Nullable<System.DateTime> date, string type)
        {
            var dateParameter = date.HasValue ?
                new ObjectParameter("date", date) :
                new ObjectParameter("date", typeof(System.DateTime));
    
            var typeParameter = type != null ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spDailyActivity_Result>("spDailyActivity", dateParameter, typeParameter);
        }
    
        public virtual ObjectResult<spMonthlyActivity_Result> spMonthlyActivity(Nullable<System.DateTime> date, string type)
        {
            var dateParameter = date.HasValue ?
                new ObjectParameter("date", date) :
                new ObjectParameter("date", typeof(System.DateTime));
    
            var typeParameter = type != null ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spMonthlyActivity_Result>("spMonthlyActivity", dateParameter, typeParameter);
        }
    
        public virtual ObjectResult<spTransSummary_Result> spTransSummary(string type, Nullable<System.DateTime> beginDate, Nullable<System.DateTime> endDate)
        {
            var typeParameter = type != null ?
                new ObjectParameter("type", type) :
                new ObjectParameter("type", typeof(string));
    
            var beginDateParameter = beginDate.HasValue ?
                new ObjectParameter("beginDate", beginDate) :
                new ObjectParameter("beginDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spTransSummary_Result>("spTransSummary", typeParameter, beginDateParameter, endDateParameter);
        }
    }
}
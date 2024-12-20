
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperPartners.SortingFiltering
{
    /// <summary>
    /// Object that contains sorting and filtering parameters for an Entity Framework query.
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Filtering parameters.
        /// </summary>
        [FromQuery(Name = "q")]
        public QueryFilter Filter { get; set; }

        /// <summary>
        /// Sorting parameters.
        /// </summary>
        [FromQuery(Name = "s")]
        public IDictionary<string, SortOperator> Sort { get; set; }

        public Query()
        {
            this.Filter = new QueryFilter();
            this.Sort = new Dictionary<string, SortOperator>();
        }
    }

    /// <summary>
    /// Object that contains filtering parameters for an Entity Framework query
    /// </summary>
    public class QueryFilter : List<QueryProperty>
    {
    }

    /// <summary>
    /// A list item for the <see cref="QueryFilter"/> object. This object has query parameters such as name and value of the column to search for.
    /// </summary>
    public class QueryProperty
    {
        /// <summary>
        /// Column name to apply the filter to.
        /// </summary>
        [FromQuery(Name = "col")]
        public string? ColumnName { get; set; }

        /// <summary>
        /// <para>Gets or sets the client time zone for filtering date and time columns. The default is UTC.</para>
        /// <para>Please set this only if you need to filter by non-UTC date parameter, but the database date column contains UTC data</para>
        /// <para>For example, DateCreated contains UTC values, and you want to get the records where DateCreated equals to 01/01/2024 in america/los_angeles time. </para>
        /// </summary>
        /// <value></value>
        public TimeZoneInfo ClientTimeZone { get; set; } = TimeZoneInfo.Utc;

        /// <summary>
        /// Value of this query parameter.
        /// </summary>
        [FromQuery(Name = "val")]
        public string? Value { get; set; }

        /// <summary>
        /// Tells if the <see cref="Value">Value</see> Property should be checked against database NULL.
        /// </summary>
        /// <value>True if value should be checked for database NULL. False if value should not be checked against database NULL.</value>
        [FromQuery(Name = "null")]
        public bool IsValueNull { get; set; }

        /// <summary>
        /// Comparison operator such as Equals, StartsWith, EndsWith, Contains etc..
        /// </summary>
        [FromQuery(Name = "op")]
        public ComparisonOperator ComparisonOperator { get; set; }

        /// <summary>
        /// Logical operator to use in query parameter such as SQL "AND" and "OR".
        /// </summary>
        [FromQuery(Name = "lop")]
        public LogicalOperator LogicalOperator { get; set; }

        /// <summary>
        /// Nested filter parameters. You can use this to create complex queries such as WHERE (Column1 = 'Some Value' OR Column1 = "Something else') AND --some other filter goes here.
        /// </summary>
        [FromQuery(Name = "q")]
        public QueryFilter Children { get; set; }

        /// <summary>
        /// Use this in C# code only. Do not use this when calling APIs.
        /// </summary>
        [XmlIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public object? ParsedValue
        {
            get
            {
                if (string.IsNullOrEmpty(this.Value))
                {
                    return null;
                }

                return this.Value;
            }
            set
            {
                this.Value = value?.ToString();
            }
        }

        /// <summary>
        /// Use this for setting metadata information in a query property. You put any object here.
        /// </summary>
        public object? Tag { get; set; }

        public QueryProperty()
        {
            // Equals by default.
            this.ComparisonOperator = ComparisonOperator.Eq;

            // And by default.
            this.LogicalOperator = LogicalOperator.And;

            this.Children = new QueryFilter();
        }
    }

    public enum LogicalOperator
    {
        /// <summary>
        /// And
        /// </summary>
        And = 1,

        /// <summary>
        /// Or
        /// </summary>
        Or
    }

    public enum ComparisonOperator
    {
        /// <summary>
        /// Equals
        /// </summary>
        [Display(Name = "Equals")]
        Eq = 1,

        /// <summary>
        /// Not Equal
        /// </summary>
        [Display(Name = "Not Equal")]
        NotEq,

        /// <summary>
        /// Greather than
        /// </summary>
        [Display(Name = "Greather than")]
        Gt,

        /// <summary>
        /// Greather than or Equal
        /// </summary>
        [Display(Name = "Greather than or Equal")]
        Gte,

        /// <summary>
        /// Less than
        /// </summary>
        [Display(Name = "Less than")]
        Lt,

        /// <summary>
        /// Less than or Equal
        /// </summary>
        [Display(Name = "Less than or Equal")]
        Lte,

        /// <summary>
        /// Starts with
        /// </summary>
        [Display(Name = "Starts with")]
        StW,

        /// <summary>
        /// Ends with
        /// </summary>
        [Display(Name = "Ends with")]
        EndW,

        /// <summary>
        /// Contains
        /// </summary>
        [Display(Name = "Contains")]
        Ct,

        /// <summary>
        /// Not Starts with
        /// </summary>
        [Display(Name = "Not Starts with")]
        NotStW,

        /// <summary>
        /// Not Ends with
        /// </summary>
        [Display(Name = "Not Ends with")]
        NotEndW,

        /// <summary>
        /// Not Contains
        /// </summary>
        [Display(Name = "Not Contains")]
        NotCt
    }

    public enum SortOperator
    {
        /// <summary>
        /// Ascending
        /// </summary>
        [Display(Name = "Ascending")]
        Asc = 1,

        /// <summary>
        /// Descending
        /// </summary>
        [Display(Name = "Descending")]
        Desc
    }
}
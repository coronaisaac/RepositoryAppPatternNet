using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DaltORM.PropertyClass
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class BelongToDatabase : Attribute
  {
	public string DatabaseName { get; }
	public string ColumnKeyDatabase { get; }
	public object ValueInColumn { get; }
	public Type TypeOfValue { get; }
	public BelongToDatabase(string sDatabaseName, string sColumnKeyDatabase, object oValueInColumn)
	{
	  this.DatabaseName = sDatabaseName;
	  this.ColumnKeyDatabase = sColumnKeyDatabase;
	  this.ValueInColumn = oValueInColumn;
	}
	public BelongToDatabase(string sDatabaseName, string sColumnKeyDatabase, object oValueInColumn, Type oTypeOfValue)
	{
	  this.DatabaseName = sDatabaseName;
	  this.ColumnKeyDatabase = sColumnKeyDatabase;
	  this.ValueInColumn = oValueInColumn;
	  this.TypeOfValue = oTypeOfValue;
	}
	public BelongToDatabase(string sDatabaseName, string sColumnKeyDatabase, string oValueInColumn)
	{
	  this.DatabaseName = sDatabaseName;
	  this.ColumnKeyDatabase = sColumnKeyDatabase;
	  this.ValueInColumn = "'" + oValueInColumn + "'";
	}
	public BelongToDatabase(string sDatabaseName, string sColumnKeyDatabase, DateTime oValueInColumn)
	{
	  this.DatabaseName = sDatabaseName;
	  this.ColumnKeyDatabase = sColumnKeyDatabase;
	  this.ValueInColumn = "'" + oValueInColumn.ToString("yyyy-MM-ddTHH:mm:ss") + "'";
	}
	public BelongToDatabase(string sDatabaseName, string sColumnKeyDatabase, decimal oValueInColumn)
	{
	  this.DatabaseName = sDatabaseName;
	  this.ColumnKeyDatabase = sColumnKeyDatabase;
	  this.ValueInColumn =  oValueInColumn.ToString();
	}
	public BelongToDatabase(string sDatabaseName, string sColumnKeyDatabase, int oValueInColumn)
	{
	  this.DatabaseName = sDatabaseName;
	  this.ColumnKeyDatabase = sColumnKeyDatabase;
	  this.ValueInColumn = oValueInColumn.ToString();
	}
	public BelongToDatabase(string sDatabaseName, string sColumnKeyDatabase, float oValueInColumn)
	{
	  this.DatabaseName = sDatabaseName;
	  this.ColumnKeyDatabase = sColumnKeyDatabase;
	  this.ValueInColumn = oValueInColumn.ToString();
	}
	public BelongToDatabase(string sDatabaseName, string sColumnKeyDatabase, char oValueInColumn)
	{
	  this.DatabaseName = sDatabaseName;
	  this.ColumnKeyDatabase = sColumnKeyDatabase;
	  this.ValueInColumn = "'" + oValueInColumn.ToString() + "'";
	}
  }
}

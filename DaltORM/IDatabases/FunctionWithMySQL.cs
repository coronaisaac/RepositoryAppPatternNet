using DaltORM.Interfaces;
using MySql.Data.MySqlClient;
using DaltORM.PropertyClass;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using static DaltORM.Database;
using System.Linq.Expressions;
using DaltORM.Class;

namespace DaltORM.IDatabases
{
  public class FunctionWithMySQL : IDatabase
  {
    private DataConnection InfoConnection { get; set; }
    private string InfoConnectionString { get; set; }
    private Assembly[] assemblies { get; set; }
    private MySqlConnection oConnection { get; set; }
    public FunctionWithMySQL(DataConnection oDataConnection)
    {
      this.InfoConnection = oDataConnection;
    }
    public FunctionWithMySQL(DataConnection oDataConnection, params Assembly[] oAssemblies)
    {
      this.InfoConnection = oDataConnection;
      this.assemblies = oAssemblies;
    }
    public FunctionWithMySQL(string oDataConnection, params Assembly[] oAssemblies)
    {
        this.InfoConnectionString = oDataConnection;
        this.assemblies = oAssemblies;
    }
        private string GetConnectionString()
    {
      string StrConection = @"server=" + InfoConnection.Server + ";"
        + @"database=" + InfoConnection.Database + ";"
        + @"uid=" + InfoConnection.User + ";";
      string passw = "pwd=" + InfoConnection.Password + ";";
      StrConection = string.IsNullOrEmpty(InfoConnection.Password) ? StrConection : StrConection + passw;
      return StrConection;
    }
  
    public bool CreateConnection()
    {
            string sqlcon = (String.IsNullOrEmpty(InfoConnection.User) && String.IsNullOrEmpty(InfoConnection.Database) && String.IsNullOrEmpty(InfoConnection.Password)) ? InfoConnectionString : GetConnectionString();
            oConnection = new MySqlConnection(sqlcon);


        if (oConnection.State == System.Data.ConnectionState.Closed)
        {
            return true;
        }
        return false;
    }
        public void OpenConnection()
    {
      oConnection.Open();
      if(oConnection.State != System.Data.ConnectionState.Open)
      {
        throw new ArgumentException("Can't be open connection!.");
      }
    }
    public string StateConnection()
    {
      return oConnection.State.ToString();
    }
    public List<T> ExecuteQuery<T>(string query)
    {
      List<T> rs = new List<T>();
      MySqlCommand oComando = new MySqlCommand(query, oConnection);
      using(MySqlDataReader reader = oComando.ExecuteReader())
      {
        if(reader.HasRows)
        {
          while(reader.Read())
          {
            Type type = typeof(T);
            if (reader.FieldCount == 1)
            {
              object value = reader.GetValue(0);
              rs.Add((T)value);
            }
            else
            {
              T reg = (T)Activator.CreateInstance(type);
              for (int i = 0; i < reader.FieldCount; i++)
              {
                if (reader.GetValue(i) != DBNull.Value)
                {
                  string name = reader.GetName(i);
                  PropertyInfo property = type.GetProperty(name);
                  property.SetValue((object)reg, reader.GetValue(i));
                }
              }
              rs.Add(reg);
            }
          }
        }
      }
      return rs;
    }
    public bool ExecuteQuery(string query)
    {
      MySqlCommand oComando = new MySqlCommand(query, oConnection);
      int affects = oComando.ExecuteNonQuery();
      if (affects > 0) return true;
      return false;
    }
    public void SendTransaction(List<string> querys)
    {
      MySqlTransaction sqlTransac = oConnection.BeginTransaction();
      foreach (string query in querys)
      {
        MySqlCommand oCommand = new MySqlCommand(query, oConnection, sqlTransac);
        int affect = oCommand.ExecuteNonQuery();
      }
      sqlTransac.Commit();
      sqlTransac.Dispose();
    }
    public void CloseConnection()
    {
      oConnection.Close();
    }

    public bool Create<T>(object aData = null)
    {
      Type type = typeof(T);
      T Class = (T)Activator.CreateInstance(type);
      PropertyInfo[] properties = type.GetProperties();

      string CreateTable = @"CREATE TABLE " + type.Name + " (";
      DbTable Attribute = typeof(T).GetCustomAttributes(typeof(DbTable), true).FirstOrDefault() as DbTable;
      if (Attribute != null)
      {
        CreateTable = @"CREATE TABLE " + Attribute.Name + " (";
      }

      string Fields = "";
      foreach (PropertyInfo itm in properties)
      {
        Fields += FieldsForCreateTable(itm) + ", ";
      }
      CreateTable += Fields.Substring(0, (Fields.Length - 4)) + ");";

      return ExecuteQuery(CreateTable);
    }

    public bool Save<T>(object oData)
    {
      Type type = typeof(T);
      PropertyInfo[] properties = type.GetProperties();

      DbTable Attribute = typeof(T).GetCustomAttributes(typeof(DbTable), true).FirstOrDefault() as DbTable;

      string InsertDatos = @"INSERT INTO " + type.Name + " (";
      if (Attribute != null)
      {
        InsertDatos = @"INSERT INTO " + Attribute.Name + " (";
      }

      string Fields = "", values = "";
      foreach (PropertyInfo itm in properties)
      {
        string value = ValueOverSqlScript(itm, oData);
        values += value.ToString() + ", ";

        Field attrs = itm.GetCustomAttributes(true).FirstOrDefault() as Field;
        if (attrs != null)
        {
          Fields += attrs.Name + ", ";
        }
        else
        {
          Fields += itm.Name + ", ";
        }
      }
      InsertDatos += Fields.Substring(0, (Fields.Length - 2)) + ") VALUES (" + values.Substring(0, (values.Length - 4)) + ");";
      return ExecuteQuery(InsertDatos);
    }
    public bool Save<T>(object oData, params string[] onlySave)
    {
      Type type = typeof(T);
      DbTable Attribute = typeof(T).GetCustomAttributes(typeof(DbTable), true).FirstOrDefault() as DbTable;

      string table = type.Name;
      if (Attribute != null)
      {
        table = Attribute.Name;
      }

      string Fields = "", values = "";
      foreach (string item in onlySave)
      {
        PropertyInfo oProperty = type.GetProperty(item);
        Fields += FieldsForInsertTable(oProperty, item) + ", ";
        values += ValueOverSqlScript(oProperty, oData) + ", ";
      }
      string InsertDatos = @"INSERT INTO " + table + " (";

      InsertDatos += Fields.Substring(0, (Fields.Length - 2)) + ") VALUES (" + values.Substring(0, (values.Length - 2)) + ");";

      return ExecuteQuery(InsertDatos);
    }

    public bool Update<T>(object oData)
    {
      Type type = typeof(T);
      PropertyInfo[] properties = type.GetProperties();
      DbTable Attribute = typeof(T).GetCustomAttributes(typeof(DbTable), true).FirstOrDefault() as DbTable;

      string table = type.Name;
      if (Attribute != null)
      {
        table = Attribute.Name;
      }

      string FieldsAndValue = "";
      string Condition = "";
      foreach (PropertyInfo item in properties)
      {
        if (!CheckIsPrimaryKey(item))
        {
          FieldsAndValue += FieldsForInsertTable(item, item.Name) + " = ";
          FieldsAndValue += ValueOverSqlScript(item, oData) + ", ";
        }
        else
        {
          Condition += FieldsForInsertTable(item, item.Name) + " = ";
          Condition += ValueOverSqlScript(item, oData) + " and ";
        }
      }
      string updateDatos = @"UPDATE " + table + " SET ";
      updateDatos += FieldsAndValue.Substring(0, (FieldsAndValue.Length - 2)) + " WHERE " + Condition.Substring(0, (Condition.Length - 5)) + ";";

      return ExecuteQuery(updateDatos);
    }
    public bool Update<T>(object oData, params string[] namePropertys)
    {
      Type type = typeof(T);
      DbTable Attribute = typeof(T).GetCustomAttributes(typeof(DbTable), true).FirstOrDefault() as DbTable;

      string table = type.Name;
      if (Attribute != null)
      {
        table = Attribute.Name;
      }

      string FieldsAndValue = "";
      string Condition = "";
      foreach (string item in namePropertys)
      {
        PropertyInfo oProperty = type.GetProperty(item);
        if (!CheckIsPrimaryKey(oProperty) || oProperty != null)
        {
          FieldsAndValue += FieldsForInsertTable(oProperty, item) + " = ";
          FieldsAndValue += ValueOverSqlScript(oProperty, oData) + ", ";
        }
        else
        {
          Condition += FieldsForInsertTable(oProperty, item) + " = ";
          Condition += ValueOverSqlScript(oProperty, oData) + " and ";
        }
      }
      string updateDatos = @"UPDATE " + table + " SET ";

      updateDatos += FieldsAndValue.Substring(0, (FieldsAndValue.Length - 2)) + " WHERE " + Condition.Substring(0, (Condition.Length - 5)) + " ;";
      return ExecuteQuery(updateDatos);
    }

    public List<T> GetAll<T>(int level = 0)
    {
      return new List<T>();
    }

    private string ValueOverSqlScript(PropertyInfo oProperty, object oData)
    {
      string type = oProperty.PropertyType.ToString();
      switch (type)
      {
        case "System.Int32":
          return "" + oProperty.GetValue(oData);
        case "System.Decimal":
          return "" + oProperty.GetValue(oData);
        case "System.String":
          return "'" + oProperty.GetValue(oData) + "'";
      }
      return "";
    }
    private string FieldsForCreateTable(PropertyInfo oProperty)
    {
      string Name = oProperty.Name;

      Field attrs = oProperty.GetCustomAttributes(true).FirstOrDefault() as Field;
      if (attrs != null)
      {
        Name = attrs.Name;
      }

      string type = oProperty.PropertyType.ToString();
      switch (type)
      {
        case "System.Int32":
          return Name + " INT";
        case "System.Decimal":
          return Name + " DECIMAL(18,3)";
        case "System.String":
          return Name + " VARCHAR(60)";
      }
      return "";
    }
    private string FieldsForInsertTable(PropertyInfo oData, string nameItem)
    {
      string Name = nameItem;
      Field attrs = oData.GetCustomAttributes(true).FirstOrDefault() as Field;
      if (attrs != null)
      {
        Name = attrs.Name;
      }
      return Name;
    }
    private bool CheckIsPrimaryKey(PropertyInfo oData)
    {
      Field attrs = oData.GetCustomAttributes(true).FirstOrDefault() as Field;
      if (attrs != null)
      {
        return attrs.IsPrimaryKey();
      }
      return false;
    }

    public string CreateQuery<T>()
    {
      throw new NotImplementedException();
    }

    public string SaveQuery<T>(object oData)
    {
      throw new NotImplementedException();
    }

   

    public string Save<T>(Expression<Func<T>> oData)
    {
      throw new NotImplementedException();
    }

    public void ForeingKeysInCreate(bool isCreate)
    {
      throw new NotImplementedException();
    }

    public string SaveQuery<T>(object oData, params Expression<Func<T, object>>[] onlySave)
    {
      throw new NotImplementedException();
    }

    public bool Save<T>(object oData, params Expression<Func<T, object>>[] onlySave)
    {
      throw new NotImplementedException();
    }

    public bool Update<T>(object oData, params Expression<Func<T, object>>[] onlySave)
    {
      throw new NotImplementedException();
    }

    public string UpdateQuery<T>(object oData)
    {
      throw new NotImplementedException();
    }

    public string UpdateQuery<T>(object oData, params Expression<Func<T, object>>[] onlySave)
    {
      throw new NotImplementedException();
    }

	
	public List<T> Get<T>(int level = 0, object oData = null)
	{
	  throw new NotImplementedException();
	}

	public bool UseDatabase(string sNameCatalog)
	{
	  throw new NotImplementedException();
	}

	public bool ExistModel<T>()
	{
	  throw new NotImplementedException();
	}

	public bool Save<T>(ref object oData)
	{
	  throw new NotImplementedException();
	}

	public bool SaveOrUpdate<T>(ref object oData)
	{
	  throw new NotImplementedException();
	}

	public bool Delete<T>(object oData, int limitSelect = 0)
	{
	  throw new NotImplementedException();
	}

	public T Reload<T>(object oData)
	{
	  throw new NotImplementedException();
	}

	public bool UseDefaultBD<T>(ref object oData)
	{
	  throw new NotImplementedException();
	}


	public Where<T> Where<T>(Expression<Func<T, object>> expression, object oData = null, int limitSelect = 0)
	{
	  throw new NotImplementedException();
	}

	public bool GetList<T>(ref object oData, int limitSelect = 0)
	{
	  throw new NotImplementedException();
	}

	public bool VerifyFields<T>()
	{
	  throw new NotImplementedException();
	}

	public bool SynchronizeFields<T>()
	{
	  throw new NotImplementedException();
	}

	public bool Synchronize<T>()
	{
	  throw new NotImplementedException();
	}

	public bool ExecuteStoredProcedure<T>(ref object oData)
	{
	  throw new NotImplementedException();
	}

	public List<Result> ExecuteStoredProcedure<T, Result>(ref object oData)
	{
	  throw new NotImplementedException();
	}


	public void And<T>(Expression<Func<T, object>> expression, ref string Statement1, ref string Statement2)
	{
	  throw new NotImplementedException();
	}

	public void Or<T>(Expression<Func<T, object>> expression, ref string Statement1, ref string Statement2)
	{
	  throw new NotImplementedException();
	}

	public void Add<T>(Expression<Func<T, object>> expression, ref string Statement1, ref string Statement2)
	{
	  throw new NotImplementedException();
	}

	public List<T> Get<T>(string Statement1, string Statement2, int level = 0, object oData = null, bool LoadObjAvalible = false, int limitSelect = 0)
	{
	  throw new NotImplementedException();
	}

    public Where<T> WhereCluster<T>(Expression<Func<T, object>> expression, object oData = null, int limiteSelect = 0)
    {
      throw new NotImplementedException();
    }

    public void AndCluster<T>(Expression<Func<T, object>> expression, ref string Statement1, ref string Statement2)
    {
      throw new NotImplementedException();
    }

    public void OrCluster<T>(Expression<Func<T, object>> expression, ref string Statement1, ref string Statement2)
    {
      throw new NotImplementedException();
    }
  }
}

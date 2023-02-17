using DaltORM.Class;
using DaltORM.Interfaces;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using static DaltORM.Database;

namespace DaltORM.IDatabases
{
  public class FunctionWithFirebird : IDatabase
  {
    private DataConnection InfoConnection { get; set; }
    private string InfoConnectionString { get; set; }
    private Assembly[] assemblies { get; set; }
    private FbConnection oConnection { get; set; }
    public FunctionWithFirebird(DataConnection oDataConnection)
    {
      this.InfoConnection = oDataConnection;
    }
    public FunctionWithFirebird(DataConnection oDataConnection, params Assembly[] oAssemblies)
    {
      this.InfoConnection = oDataConnection;
      this.assemblies = oAssemblies;
    }
    public FunctionWithFirebird(string oDataConnection, params Assembly[] oAssemblies)
    {
        this.InfoConnectionString = oDataConnection;
        this.assemblies = oAssemblies;
    }
        private string GetConnectionString()
    {
      return @"ServerType=0;" +
            @"User=" + InfoConnection.User + ";" +
            @"Password=" + InfoConnection.Password + ";" +
            @"Database=" + InfoConnection.Server + ":" + InfoConnection.Instance + ";" +
            @"Charset=" + InfoConnection.Other + ";";
    }
    public bool CreateConnection()
    {
      string sqlcon = (String.IsNullOrEmpty(InfoConnection.User) && String.IsNullOrEmpty(InfoConnection.Database) && String.IsNullOrEmpty(InfoConnection.Password)) ? InfoConnectionString : GetConnectionString();
      oConnection = new FbConnection(sqlcon);
      if(oConnection.State == System.Data.ConnectionState.Closed)
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
        throw new ArgumentException("Can't be open the connection!.");
      }
    }
    public string StateConnection()
    {
      return oConnection.State.ToString();
    }
    public List<T> ExecuteQuery<T>(string query)
    {
      List<T> rs = new List<T>();
      FbCommand oComando = new FbCommand(query, oConnection);
      using(FbDataReader reader = oComando.ExecuteReader())
      {
        if(reader.HasRows)
        {
          while(reader.Read())
          {
            Type type = typeof(T);
            T reg = (T)Activator.CreateInstance(type);
            for(int i = 0; i < reader.FieldCount; i++)
            {
              if(reader.GetValue(i) != DBNull.Value)
              {
                var property = type.GetProperty(reader.GetName(i));
                property.SetValue((object)reg, reader.GetValue(i));
              }
            }
            rs.Add(reg);
          }
        }
      }
      return rs;
    }
    public bool ExecuteQuery(string query)
    {
      FbCommand oComando = new FbCommand(query, oConnection);
      int affects = oComando.ExecuteNonQuery();
      if (affects > 0) return true;
      return false;
    }
    public void SendTransaction(List<string> querys)
    {
      FbTransaction sqlTransac = oConnection.BeginTransaction();
      foreach (string query in querys)
      {
        FbCommand oCommand = new FbCommand(query, oConnection, sqlTransac);
        oCommand.ExecuteNonQuery();
      }
      sqlTransac.Commit();
      sqlTransac.Dispose();
    }
    public void CloseConnection()
    {
      throw new NotImplementedException();
    }

    public bool Create<T>(object aData = null)
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

	public void ForeingKeysInCreate(bool isCreate)
	{
	  throw new NotImplementedException();
	}

	public bool Save<T>(object oData)
	{
	  throw new NotImplementedException();
	}

	public bool Save<T>(ref object oData)
	{
	  throw new NotImplementedException();
	}

	public bool Save<T>(object oData, params Expression<Func<T, object>>[] onlySave)
	{
	  throw new NotImplementedException();
	}

	public bool SaveOrUpdate<T>(ref object oData)
	{
	  throw new NotImplementedException();
	}

	public bool Update<T>(object oData)
	{
	  throw new NotImplementedException();
	}

	public bool Update<T>(object oData, params Expression<Func<T, object>>[] onlySave)
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



	public Where<T> Where<T>(Expression<Func<T, object>> expression, object oData = null, int limitSelect = 0)
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

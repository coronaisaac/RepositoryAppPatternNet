
using DaltORM.Class;
using DaltORM.Interfaces;
using DaltORM.PropertyClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static DaltORM.Database;

namespace DaltORM.IDatabases
{
  public class FunctionsWithSQLserver : IDatabase
  {
    /***********************************************************/
    /* INICIAN CONEXION DE BASE DE DATOS ***********************/
    /***********************************************************/

    private DataConnection InfoConnection { get; set; }
    private string InfoConnectionString { get; set; }
    private SqlConnection oConnection { get; set; }
    private Assembly[] assemblies { get; set; }
    private bool createForeingKey { get; set; }

    private string GetConnectionString()
    {
      string sConnectionString = String.Empty;

      sConnectionString += !String.IsNullOrEmpty(InfoConnection.Server) ? @"Data Source=" + InfoConnection.Server : "";
      sConnectionString += !String.IsNullOrEmpty(InfoConnection.Instance) ? @"\" + InfoConnection.Instance : "";
      sConnectionString += !String.IsNullOrEmpty(InfoConnection.Database) ? @";Initial Catalog=" + InfoConnection.Database : "";
      sConnectionString += !String.IsNullOrEmpty(InfoConnection.User) ? @";Persist Security info=true; User=" + InfoConnection.User : "";
      sConnectionString += !String.IsNullOrEmpty(InfoConnection.Password) ? @";Password=" + InfoConnection.Password + ";" : "";
      sConnectionString += !String.IsNullOrEmpty(InfoConnection.Other) ? InfoConnection.Other : "";

      return sConnectionString;
    }

    //private string finalQueryOpcion = " OPTION(ROBUST PLAN, KEEP PLAN);";

    //private string withNoLockTable = " WITH (NOLOCK) ";


    public FunctionsWithSQLserver(DataConnection oDataConnection)
    {
      this.InfoConnection = oDataConnection;
      this.createForeingKey = false;
    }
    
    public FunctionsWithSQLserver(string connectionString)
    {
        this.InfoConnectionString = connectionString;
        this.createForeingKey = false;
    }

        //public FunctionsWithSQLserver(DataConnection oDataConnection, params Assembly[] oAssemblies)
        //{
        //  this.InfoConnection = oDataConnection;
        //  this.createForeingKey = false;
        //  //this.assemblies = oAssemblies;
        //}

        public bool CreateConnection()
    {
      string sqlcon =  (String.IsNullOrEmpty(InfoConnection.User) && String.IsNullOrEmpty(InfoConnection.Database) && String.IsNullOrEmpty(InfoConnection.Password)) ? InfoConnectionString : GetConnectionString();
      this.oConnection = new SqlConnection(sqlcon);
      if (oConnection != null) return true;
      return false;
    }
   
        public void OpenConnection()
    {
      this.oConnection.Open();
      if (oConnection.State != System.Data.ConnectionState.Open)
      {
        throw new ArgumentException("is can't open connection!");
      }
    }
    public void CloseConnection()
    {
      oConnection.Close();
      if (oConnection.State != System.Data.ConnectionState.Closed)
      {
        throw new ArgumentException("is can't closed connection!");
      }

    }
    public bool UseDatabase(string sNameCatalog)
    {
      try
      {
        oConnection.ChangeDatabase(sNameCatalog);
        return true;
      }
      catch (SqlException Err)
      {
        string error = Err.Message;
        return false;
      }
    }
    public string StateConnection()
    {
      return oConnection.State.ToString();
    }
    public bool UseDefaultBD<T>(ref object oData)
    {
      try
      {
        Type type = typeof(T);
        MethodInfo method = type.GetMethod("SetDatabaseName", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
        if (method != null)
        {
          method.Invoke(oData, new object[] { oConnection.Database });
        }
        return true;
      }
      catch
      {
        return false;
      }
    }



    /***********************************************************/
    /* EJECUTAN CONSULTAS EN BASE DE DATOS *********************/
    /***********************************************************/

    public List<T> ExecuteQuery<T>(string query)
    {
      List<T> rs = new List<T>();
      SqlCommand oComando = new SqlCommand(query, oConnection);
      oComando.CommandTimeout = 20000;
      using (SqlDataReader reader = oComando.ExecuteReader())
      {
        if (reader.HasRows)
        {
          while (reader.Read())
          {
            Type type = typeof(T);
            if (reader.FieldCount == 1)
            {
              object value = reader.GetValue(0);
              if (value != DBNull.Value)
              {
                rs.Add((T)value);
              }
            }
            else
            {
              T reg = (T)Activator.CreateInstance(type);
              for (int i = 0; i < reader.FieldCount; i++)
              {
                if (reader.GetValue(i) != DBNull.Value)
                {
                  string name = reader.GetName(i);
                  if (name != null)
                  {
                    PropertyInfo property = type.GetProperty(name);
                    if (property != null)
                    {
                      Type pType = property.PropertyType;
                      if (pType.FullName == "System.Boolean")
                      {
                        object obj = reader.GetValue(i);
                        switch (obj)
                        {
                          case 1:
                            property.SetValue((object)reg, true);
                            break;
                          case 0:
                            property.SetValue((object)reg, false);
                            break;
                          case true:
                            property.SetValue((object)reg, true);
                            break;
                          case false:
                            property.SetValue((object)reg, false);
                            break;
                          default:
                            throw new ArgumentException("Can't cast value boolean");
                        }
                      }
                      else
                      {
                        property.SetValue((object)reg, reader.GetValue(i));
                      }
                    }
                  }
                }
              }
              rs.Add(reg);
            }
          }
        }
      }
      return rs;
    }
    private List<object> ExecuteQuery(string query, Type type)
    {
      List<object> rs = new List<object>();
      using (SqlCommand oComando = new SqlCommand(query, oConnection))
      {
        oComando.CommandTimeout = 240000;
        using (SqlDataReader reader = oComando.ExecuteReader())
        {
          if (reader.HasRows)
          {
            while (reader.Read())
            {
              //Type type = typeof(T);
              if (reader.FieldCount == 1)
              {
                object value = reader.GetValue(0);
                rs.Add((object)value);
              }
              else
              {
                var reg = Activator.CreateInstance(type);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                  if (reader.GetValue(i) != DBNull.Value)
                  {
                    string name = reader.GetName(i);
                    if (name != null)
                    {
                      PropertyInfo property = type.GetProperty(name);
                      if (property != null)
                      {
                        Type pType = property.PropertyType;
                        if (pType.FullName == "System.Boolean")
                        {
                          object obj = reader.GetValue(i);
                          switch (obj)
                          {
                            case 1:
                              property.SetValue((object)reg, true);
                              break;
                            case 0:
                              property.SetValue((object)reg, false);
                              break;
                            case true:
                              property.SetValue((object)reg, true);
                              break;
                            case false:
                              property.SetValue((object)reg, false);
                              break;
                            default:
                              throw new ArgumentException("Can't cast value boolean");
                          }
                        }
                        else
                        {
                          property.SetValue(reg, reader.GetValue(i));
                        }
                      }
                    }
                  }
                }
                rs.Add(reg);
              }
            }
          }
        }
      }
      return rs;
    }
    public bool ExecuteQuery(string query)
    {
      int affects = 0;
      using (SqlCommand oComando = new SqlCommand(query, oConnection))
      {
        oComando.CommandTimeout = 240000;
        try
        {
          affects = oComando.ExecuteNonQuery();
        }
        catch (SqlException Err)
        {
          throw new ArgumentException(Err.Message);
        }
      }
      return affects > 0 ? true : false;
    }
    public void SendTransaction(List<string> querys)
    {
      SqlTransaction sqlTransac = oConnection.BeginTransaction();
      foreach (string query in querys)
      {
        SqlCommand oCommand = new SqlCommand(query, oConnection, sqlTransac);
        oCommand.ExecuteNonQuery();
      }
      sqlTransac.Commit();
      sqlTransac.Dispose();
    }
    public void SendTransaction(List<string> querys, BackgroundWorker oProgress)
    {
      SqlTransaction sqlTransac = oConnection.BeginTransaction();
      int totalItems = querys.Count, counter = 0;
      foreach (string query in querys)
      {
        SqlCommand oCommand = new SqlCommand(query, oConnection, sqlTransac);
        oCommand.ExecuteNonQuery();

        int progress = counter * 100 / totalItems;
        oProgress.ReportProgress(progress);
        counter++;
      }
      sqlTransac.Commit();
      sqlTransac.Dispose();
    }

    //PARAMETROS PARA CREAR UNA TABLA
    public void ForeingKeysInCreate(bool isCreate)
    {
      createForeingKey = isCreate;
    }

    // CREA QUERY PARA CREAR LA TABLA
    public bool ExistModel<T>()
    {
      Type oType = typeof(T);
      List<string> oListFound = ExecuteQuery<string>(getIfExistModelQuery<T>());
      if (oListFound != null)
      {
        if (oListFound.Count > 0)
        {
          return true;
        }
      }
      return false;
    }
    public bool Create<T>(object? oData = null)
    {
      return ExecuteQuery(getCreateQuery<T>(oData));
    }

    //CREA QUERY Y EJECUTA PARA GUARDAR DATOS EN TABLA DE UNA CLASE
    public bool Save<T>(object oData)
    {
      return ExecuteQuery(getSaveQuery<T>(oData));
    }
    public bool Save<T>(ref object oData)
    {
      DbTable attrbTable = getCustomAttribute<DbTable, T>();
      Type type = typeof(T);
      PropertyInfo[] properties = type.GetProperties();
      PropertyInfo oKeyPropery = null;
      string namePropertyKey = null;
      foreach (PropertyInfo itm in properties)
      {
        Field oField = getCustomAttribute<Field>(itm);
        if (oField != null)
        {
          if (oField.IsAutoIncrement())
          {
            oKeyPropery = itm;
            namePropertyKey = oField.Name;
            break;
          }
        }
      }

      if (ExecuteQuery(getSaveQuery<T>(oData)))
      {
        // diccionario de propiedades
        Dictionary<string, string> oFieldNameInProperties = new Dictionary<string, string>();
        foreach (PropertyInfo property in properties)
        {
          Field oField = getCustomAttribute<Field>(property);
          if (oField != null)
          {
            oFieldNameInProperties.Add(oField.Name, property.Name);
          }
        }

        #region DEFINE SI TIENE ORIGEN EN UNA BASE DE DATOS MAPEADO
        UseDatabase attrUseDatabase = getCustomAttribute<UseDatabase>(type);
        object sNameDatabase = null;
        if (attrUseDatabase != null)
        {
          if (attrUseDatabase.Databases.Count == 1)
          {
            sNameDatabase = attrUseDatabase.Databases.First();
          }
          else
          {
            MethodInfo method = type.GetMethod("GetDatabaseName", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            if (method != null)
            {
              sNameDatabase = method.Invoke(oData, new object[] { });
            }

            if (sNameDatabase == null)
            {
              List<BelongToDatabase> oBelongDBList = getCustomAttributeList<BelongToDatabase>(type);
              if (oBelongDBList != null)
              {
                BelongToDatabase oBelongDB = oBelongDBList.FirstOrDefault();
                if (oBelongDB != null)
                {
                  string nameCol = oFieldNameInProperties.Where(o => o.Key == oBelongDB.ColumnKeyDatabase).FirstOrDefault().Value;
                  if (nameCol == null) throw new ArgumentException("Can't find which database to save!.");

                  var valueIdDB = type.GetProperty(nameCol).GetValue(oData);
                  oBelongDB = oBelongDBList.Where(o => o.ValueInColumn.Equals(valueIdDB)).FirstOrDefault();
                  if (oBelongDB == null) throw new ArgumentException("Can't find which database to save!.");
                  sNameDatabase = oBelongDB.DatabaseName;
                }
              }
            }
          }
        }
        #endregion

        if (oKeyPropery != null && !String.IsNullOrEmpty(namePropertyKey))
        {
          string sSql = sNameDatabase == null ? "select Convert(int, ident_current('" + attrbTable.Name + "')) as " + namePropertyKey
                      : "select Convert(int, ident_current('" + sNameDatabase + ".." + attrbTable.Name + "')) as " + namePropertyKey;
          int intIdentity = ExecuteQuery<int>(sSql).FirstOrDefault();
          oKeyPropery.SetValue(oData, intIdentity);
        }
      }
      else
      {
        return false;
      }
      return true;
    }
    public bool Save<T>(object oData, params Expression<Func<T, object>>[] onlySave)
    {
      string[] list = new string[onlySave.Length];
      int index = 0;
      foreach (var me in onlySave)
      {
        var mexp = (LambdaExpression)me;
        if (mexp.Body is UnaryExpression)
        {
          UnaryExpression unaryExpression = (UnaryExpression)mexp.Body;
          var memberExpression = (MemberExpression)(unaryExpression.Operand);
          list[index] = memberExpression.Member.Name;
        }
        else
        {
          var MemberExpression = (MemberExpression)mexp.Body;
          list[index] = MemberExpression.Member.Name;
        }
        index++;
      }
      return ExecuteQuery(getSaveQuery<T>(oData, list));
    }

    public bool SaveOrUpdate<T>(ref object oData)
    {
      Type type = typeof(T);
      DbTable attrbTable = getCustomAttribute<DbTable>(type);

      PropertyInfo[] properties = type.GetProperties();
      PropertyInfo oKeyPropery = null;
      string namePropertyKey = null;

      List<PropertyInfo> oKeysProperies = new List<PropertyInfo>();
      foreach (PropertyInfo oProperty in properties)
      {
        Field oField = getCustomAttribute<Field>(oProperty);
        if (oField != null)
        {
          if (oField.IsPrimaryKey()) oKeysProperies.Add(oProperty);
        }
      }

      // obtiene la propiedad la cual sea autoincrement para asingnarle su valor asignado en la BD
      foreach (PropertyInfo itm in properties)
      {
        Field oField = getCustomAttribute<Field>(itm);
        if (oField != null)
        {
          if (oField.IsAutoIncrement())
          {
            oKeyPropery = itm;
            namePropertyKey = oField.Name;
            break;
          }
        }
      }

      // diccionario de propiedades
      Dictionary<string, string> oFieldNameInProperties = new Dictionary<string, string>();
      foreach (PropertyInfo property in properties)
      {
        Field oField = getCustomAttribute<Field>(property);
        if (oField != null)
        {
          oFieldNameInProperties.Add(oField.Name, property.Name);
        }
      }

      string sSqlIfExist = "Select * From ";

      #region DEFINE SI TIENE ORIGEN EN UNA BASE DE DATOS MAPEADO
      UseDatabase attrUseDatabase = getCustomAttribute<UseDatabase>(type);
      object sNameDatabase = null;
      if (attrUseDatabase != null)
      {
        if (attrUseDatabase.Databases.Count == 1)
        {
          sNameDatabase = attrUseDatabase.Databases.First();
        }
        else
        {
          MethodInfo method = type.GetMethod("GetDatabaseName", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
          if (method != null)
          {
            sNameDatabase = method.Invoke(oData, new object[] { });
          }

          if (sNameDatabase == null)
          {
            List<BelongToDatabase> oBelongDBList = getCustomAttributeList<BelongToDatabase>(type);
            if (oBelongDBList != null)
            {
              BelongToDatabase oBelongDB = oBelongDBList.FirstOrDefault();
              if (oBelongDB != null)
              {
                string nameCol = oFieldNameInProperties.Where(o => o.Key == oBelongDB.ColumnKeyDatabase).FirstOrDefault().Value;
                if (nameCol == null) throw new ArgumentException("Can't find which database to save!.");

                var valueIdDB = type.GetProperty(nameCol).GetValue(oData);
                oBelongDB = oBelongDBList.Where(o => o.ValueInColumn.Equals(valueIdDB)).FirstOrDefault();
                if (oBelongDB == null) throw new ArgumentException("Can't find which database to save!.");
                sNameDatabase = oBelongDB.DatabaseName;
              }
            }
          }
        }
      }
      sSqlIfExist += sNameDatabase != null ? sNameDatabase + ".." : "";
      #endregion

      sSqlIfExist += attrbTable.Name + " ";

      PropertyInfo oProp1 = oKeysProperies.FirstOrDefault();
      if (oProp1 == null) throw new ArgumentException("must have keys property in the model!.");

      Field oField1 = getCustomAttribute<Field>(oProp1);
      sSqlIfExist += "Where " + oField1.Name + " = " + getWayString(oProp1.GetValue(oData).ToString(), oProp1.PropertyType.Name);

      foreach (PropertyInfo oProp in oKeysProperies.Skip(1))
      {
        oField1 = getCustomAttribute<Field>(oProp);
        sSqlIfExist += " AND " + oField1.Name + " = " + getWayString(oProp.GetValue(oData).ToString(), oProp.PropertyType.Name);
      }

      if (ExecuteQuery(sSqlIfExist))
      {
        return ExecuteQuery(getUpdateQuery<T>(oData));
      }

      if (ExecuteQuery(getSaveQuery<T>(oData)))
      {
        if (oKeyPropery != null && !String.IsNullOrEmpty(namePropertyKey))
        {
          int intIdentity = ExecuteQuery<int>("select Convert(int, ident_current('" + attrbTable.Name + "')) as " + namePropertyKey).FirstOrDefault();
          oKeyPropery.SetValue(oData, intIdentity);
        }
      }
      else
      {
        return false;
      }
      return true;
    }

    //ACTUALIZA DATOS EN DE UNA CLASE EN SU TABLA
    public bool Update<T>(object oData)
    {
      bool result = ExecuteQuery(getUpdateQuery<T>(oData));
      return result;
    }
    public bool Update<T>(object oData, params Expression<Func<T, object>>[] onlySave)
    {
      string[] list = new string[onlySave.Length];
      int index = 0;
      foreach (var me in onlySave)
      {
        var mexp = (LambdaExpression)me;
        if (mexp.Body is UnaryExpression)
        {
          UnaryExpression unaryExpression = (UnaryExpression)mexp.Body;
          var memberExpression = (MemberExpression)(unaryExpression.Operand);
          list[index] = memberExpression.Member.Name;
        }
        else
        {
          var MemberExpression = (MemberExpression)mexp.Body;
          list[index] = MemberExpression.Member.Name;
        }
        index++;
      }
      bool result = ExecuteQuery(getUpdateQuery<T>(oData, list));
      return result;
    }

    //BORRA EL REGISTRO DE LA CLASE POR SUS LLAVES PRIMARIAS Y BORRA EN LA BASE DE DATOS
    public bool Delete<T>(object oData, int limiteSelect = 0)
    {
      bool result = ExecuteQuery(getDeleteQuery<T>(oData, limiteSelect));
      return result;
    }

    //ACTUALIZA EL OBJETO CONSULTANDOLO A LA BASE DE DATOS
    public T Reload<T>(object oData)
    {
      throw new NotImplementedException();
    }


    //METODOS QUE CONSTRUYEN UNA CONSULTA ESPECIAL
    public List<T> Get<T>(string Statement1, string Statement2, int level = 0, object oData = null, bool LoadObjAvalible = false, int limiteSelect = 0)
    {
      Type type = typeof(T);
      Dictionary<string, string> indice = new Dictionary<string, string>();
      List<BelongToDatabase> oBelongDatabases = getCustomAttributeList<BelongToDatabase>(typeof(T));
      UseDatabase oUseDatabases = getCustomAttribute<UseDatabase>(typeof(T));

      bool queryComplete = false;

      string sSql = "";

      if (oBelongDatabases == null && oUseDatabases == null)
      {
        sSql = getStringSelect(oData, typeof(T), ref indice, level, LoadObjAvalible, limiteSelect) +
             getStringFromIndice(oData, typeof(T), level, LoadObjAvalible) +
             Statement1;
      }
      else
      {

        if (oUseDatabases != null)
        {
          if (oUseDatabases.Databases.Count > 0)
          {
            List<string> dbToSelect = new List<string>();
            object sNameDatabase = null;
            if (oUseDatabases.Databases.Count == 1)
            {
              sNameDatabase = oUseDatabases.Databases.First();
            }
            else
            {
              MethodInfo method = type.GetMethod("GetDatabaseName", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
              if (method != null)
              {
                if (oData != null)
                {
                  sNameDatabase = method.Invoke(oData, null);
                }
              }
            }


            dbToSelect = oUseDatabases.Databases;
            if (sNameDatabase != null)
            {
              if (!String.IsNullOrEmpty(sNameDatabase.ToString()))
              {
                dbToSelect.Clear();
                dbToSelect.Add(sNameDatabase.ToString());
              }
            }

            sSql += "WITH [fulldata] As ( ";
            foreach (string item in dbToSelect)
            {
              sSql += getStringSelectV2(oData, typeof(T), ref indice, level, LoadObjAvalible, limiteSelect) + ", '" + item + "' as dbname$ ";
              sSql += getStringFromIndice(oData, typeof(T), item, level, LoadObjAvalible);

              sSql += dbToSelect.Last() == item ? "" : " UNION ";
              if (dbToSelect.Last() != item) indice.Clear();
            }
            sSql += " ) Select ";
            sSql += limiteSelect > 0 ? " TOP(" + limiteSelect + ") " : "";
            sSql += " * from [fulldata] " + Statement2;
            queryComplete = true;
          }
          else throw new ArgumentException("Not found UseDatabase list db!.");
        }

        if (!queryComplete)
        {
          if (oBelongDatabases != null)
          {
            if (oBelongDatabases.Count > 0)
            {
              object sNameDatabase = null;
              MethodInfo method = type.GetMethod("GetDatabaseName", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
              if (method != null)
              {
                sNameDatabase = method.Invoke(oData, new object[] { });
              }

              sSql += "WITH [fulldata] As ( ";
              if (sNameDatabase == null)
              {
                foreach (BelongToDatabase item in oBelongDatabases)
                {

                  sSql += getStringSelectV2(oData, typeof(T), ref indice, level, LoadObjAvalible, limiteSelect) +
                      getStringFromIndice(oData, typeof(T), item.DatabaseName, level, LoadObjAvalible) +
                      " Where " + item.ColumnKeyDatabase + " = " + item.ValueInColumn.ToString();

                  sSql += oBelongDatabases.Last() == item ? "" : " UNION ";
                  if (oBelongDatabases.Last() != item) indice.Clear();
                }
              }
              else
              {
                sSql += getStringSelectV2(oData, typeof(T), ref indice, level, LoadObjAvalible, limiteSelect) +
                    getStringFromIndice(oData, typeof(T), sNameDatabase.ToString(), level, LoadObjAvalible);
              }

              sSql += " ) Select ";
              sSql += limiteSelect > 0 ? " TOP(" + limiteSelect + ") " : "";
              sSql += " * from [fulldata] " + Statement2;
            }
            else throw new ArgumentException("Not found BelongDatabases db!.");
          }
        }
      }


      return ExecuteQueryWithLevels<T>(sSql, indice);
    }
    public Where<T> Where<T>(Expression<Func<T, object>> expression, object? oData = null, int limiteSelect = 0)
    {
      return new Where<T>(" Where " + createWhereClause<T>(expression), " Where " + createWhereClauseV2<T>(expression), oData, limiteSelect);
    }
    public void And<T>(Expression<Func<T, object>> expression, ref string Statement1, ref string Statement2)
    {
      Statement1 += " And " + createWhereClause<T>(expression);
      Statement2 += " And " + createWhereClauseV2<T>(expression);
    }
    public void Or<T>(Expression<Func<T, object>> expression, ref string Statement1, ref string Statement2)
    {
      Statement1 += " Or " + createWhereClause<T>(expression);
      Statement2 += " Or " + createWhereClauseV2<T>(expression);
    }

    public void AndCluster<T>(Expression<Func<T, object>> expression, ref string Statement1, ref string Statement2)
    {
      Statement1 += " And ( " + createWhereClause<T>(expression);
      Statement2 += " And ( " + createWhereClauseV2<T>(expression);
    }
    public void OrCluster<T>(Expression<Func<T, object>> expression, ref string Statement1, ref string Statement2)
    {
      Statement1 += " Or ( " + createWhereClause<T>(expression);
      Statement2 += " Or ( " + createWhereClauseV2<T>(expression);
    }

    public void Add<T>(Expression<Func<T, object>> expression, ref string Statement1, ref string Statement2)
    {
      Statement1 += " + " + createWhereClause<T>(expression) + " ";
      Statement2 += " + " + createWhereClauseV2<T>(expression) + " ";
    }
    public Where<T> WhereCluster<T>(Expression<Func<T, object>> expression, object oData = null, int limiteSelect = 0)
    {
      return new Where<T>(" Where ( " + createWhereClause<T>(expression), " Where ( " + createWhereClauseV2<T>(expression), oData, limiteSelect);
    }


    public bool VerifyFields<T>()
    {
      if (ExistModel<T>())
      {
        DbTable oTable = getCustomAttribute<DbTable, T>();
        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();
        Dictionary<string, bool> values = new Dictionary<string, bool>();

        string sqlGetProperties = @"select 
    col.column_id as Column_id,
    col.name as Column_name, 
    t.name as Sql_type,
    col.max_length as Max_length,
    col.precision as Precision,
	CASE
		WHEN (select top 1 column_name from INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC
						INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU
							ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' 
							AND TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME 
							AND KU.table_name=tab.name
							AND column_name=col.name ) = col.name
		THEN 1
		ELSE 0
	END AS PrimaryKey,
	CASE
		WHEN ((select top 1 column_name from INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC
						INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU
							ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' 
							AND TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME 
							AND KU.table_name=tab.name
							AND column_name=col.name ) = col.name) AND IDENT_SEED(tab.name) = 1 AND IDENT_INCR (tab.name) = 1
		THEN 1
		ELSE 0
	END AS AutoIncrement,
	CASE col.is_nullable
		WHEN 1
		THEN 0
		WHEN 0
		THEN 1
	END as NotNull,
	CASE t.name
		WHEN 'int'	     	THEN 'int'
		WHEN 'varchar'		THEN 'string'
		WHEN 'bit'			THEN 'bool'
		WHEN 'nvarchar'		THEN 'string'
		WHEN 'float'		THEN 'double'
		WHEN 'char'			THEN 'string'
		WHEN 'nchar'		THEN 'string'
		WHEN 'numeric'		THEN 'decimal'
		WHEN 'smalldatetime'THEN 'date'
		WHEN 'datetime'		THEN 'DateTime'
		WHEN 'smallint'		THEN 'int'
		WHEN 'decimal'		THEN 'decimal'
		WHEN 'time'			THEN 'Time'
	END	AS Code_type
from sys.tables as tab inner join sys.columns as col on tab.object_id = col.object_id
left join sys.types as t on col.user_type_id = t.user_type_id where tab.name like '" + oTable.Name + "' order by column_id";

        List<PropertySQL> oListSQLProperties = ExecuteQuery<PropertySQL>(sqlGetProperties);
        foreach (PropertyInfo oProperty in properties)
        {
          Field oField = getCustomAttribute<Field>(oProperty);
          if (oField != null)
          {
            PropertySQL oPropertySQL = oListSQLProperties.Where(o => o.Column_name == oField.Name).FirstOrDefault();
            bool autoIncrement = oField.IsAutoIncrement();
            bool notNull = oField.IsNotNull();
            bool primaryKey = oField.IsPrimaryKey();

            if (oPropertySQL != null)
            {
              if (oPropertySQL.AutoIncrement == autoIncrement
              && oPropertySQL.NotNull == notNull
              && oPropertySQL.PrimaryKey == primaryKey)
              {
                values.Add(oPropertySQL.Column_name, true);
              }
              else
              {
                values.Add(oPropertySQL.Column_name, false);
              }
            }
            else
            {
              values.Add(oField.Name, false);
            }
          }
        }

        foreach (bool value in values.Values)
        {
          if (value == false) return false;
        }
        return true;
      }
      return false;
    }
    public bool SynchronizeFields<T>()
    {
      DbTable oTable = getCustomAttribute<DbTable, T>();
      Type type = typeof(T);
      List<PropertyInfo> properties = type.GetProperties().ToList();
      Dictionary<Field, PropertyInfo> oListField = new Dictionary<Field, PropertyInfo>();
      foreach (PropertyInfo oPropery in properties)
      {
        Field oField = getCustomAttribute<Field>(oPropery);
        if (oField != null) oListField.Add(oField, oPropery);
      }

      string alterTableSql = "ALTER TABLE " + oTable.Name + " ";

      List<string> listFields = getListMissingFields<T>();
      foreach (string itemName in listFields)
      {
        var oFound = oListField.Where(o => o.Key.Name == itemName).FirstOrDefault();
        PropertyInfo property = oFound.Value;
        alterTableSql += Environment.NewLine;
        alterTableSql += " ADD " + getFieldParameterInCreate(property);
      }
      if (listFields == null) return false;
      if (listFields.Count <= 0) return false;
      return ExecuteQuery(alterTableSql);
    }
    public bool Synchronize<T>()
    {
      try
      {
        Type type = typeof(T);
        UseDatabase oUseDb = getCustomAttribute<UseDatabase>(type);
        if (oUseDb != null)
        {
          if (oUseDb.Databases.Count > 0)
          {
            foreach (string oItem in oUseDb.Databases)
            {
              if (UseDatabase(oItem))
              {
                if (ExistModel<T>())
                {
                  SynchronizeFields<T>();
                }
                else
                {
                  Create<T>();
                }
              }
            }
            return true;
          }
        }


        List<BelongToDatabase> oBelongDBList = getCustomAttributeList<BelongToDatabase>(type);
        foreach (BelongToDatabase oItem in oBelongDBList)
        {
          if (UseDatabase(oItem.DatabaseName))
          {
            if (ExistModel<T>())
            {
              SynchronizeFields<T>();
            }
            else
            {
              Create<T>();
            }
          }
        }
        return true;
      }
      catch
      {
        return false;
      }
    }
    public bool GetList<T>(ref object oData, int limitSelect = 0)
    {
      UseDatabase oUseDatabases = getCustomAttribute<UseDatabase>(typeof(T));
      try
      {
        Type type = typeof(T);
        List<PropertyInfo> properties = type.GetProperties().ToList();

        object sNameDatabase = null;
        MethodInfo method = type.GetMethod("GetDatabaseName", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
        if (method != null)
        {
          sNameDatabase = method.Invoke(oData, new object[] { });
        }
        if (oUseDatabases != null)
        {
          if (oUseDatabases.Databases.Count == 1)
          {
            sNameDatabase = oUseDatabases.Databases.First();
          }
        }

        Dictionary<Field, PropertyInfo> oListado = new Dictionary<Field, PropertyInfo>();
        foreach (PropertyInfo oPropery in properties)
        {
          Field oField = getCustomAttribute<Field>(oPropery);
          if (oField != null) oListado.Add(oField, oPropery);
        }

        foreach (PropertyInfo oProperty in properties)
        {
          BelongsToMany oBelongToMany = getCustomAttribute<BelongsToMany>(oProperty);
          if (oBelongToMany != null)
          {
            var oList = oProperty.GetValue(oData);
            if (oList != null)
            {
              Type oType = oProperty.PropertyType;
              Type oTypeList = oType.GenericTypeArguments.First();

              string sSql = getStringSelect(oTypeList, limitSelect);
              string sSqlFrom = "";
              sSqlFrom += oBelongToMany.InDatabase != null && sNameDatabase == null ? getStringFromInDatabase(oTypeList, oBelongToMany.InDatabase) : "";
              sSqlFrom += oBelongToMany.InDatabase == null && sNameDatabase == null ? getStringFrom(oTypeList) : "";
              sSqlFrom += oBelongToMany.InDatabase == null && sNameDatabase != null ? getStringFromInDatabase(oTypeList, sNameDatabase.ToString()) : "";
              sSql += sSqlFrom;

              DbTable oDbTable = getCustomAttribute<DbTable>(oTypeList);
              sSql += oDbTable != null ? " Where " + oDbTable.Name + "." + oBelongToMany.IdReference : " Where " + oTypeList.Name + "." + oBelongToMany.IdReference;

              PropertyInfo oPropertyReference = oListado.Where(o => o.Key.Name == oBelongToMany.Name).FirstOrDefault().Value;

              if (oPropertyReference == null) throw new ArgumentException("Not found value in reference!.");

              sSql += " = " + getWayString(oPropertyReference.GetValue(oData).ToString(), oPropertyReference.PropertyType.Name);

              foreach (BelongsToMany aBelong in getCustomAttributeList<BelongsToMany>(oProperty).Skip(1))
              {
                sSql += oDbTable != null ? " And " + oDbTable.Name + "." + aBelong.IdReference : " And " + oTypeList.Name + "." + aBelong.IdReference;
                PropertyInfo oPropertyBelong = oListado.Where(o => o.Key.Name == aBelong.Name).FirstOrDefault().Value;
                sSql += " = " + getWayString(oPropertyBelong.GetValue(oData).ToString(), oPropertyBelong.PropertyType.Name);
              }

              List<object> reg = ExecuteQuery(sSql, oTypeList);

              foreach (object oReg in reg)
              {
                MethodInfo methodadd = oList.GetType().GetMethod("Add");
                methodadd.Invoke(oList, new object[] { oReg });
              }
              oProperty.SetValue(oData, oList);
            }
          }
        }
      }
      catch (Exception Err)
      {
        string Errors = Err.Message;
        return false;
      }
      return true;
    }


    public bool ExecuteStoredProcedure<T>(ref object oData)
    {
      Type type = typeof(T);
      StoredProcedure oStoredProcedure = getCustomAttribute<StoredProcedure>(type);

      if (oStoredProcedure == null) throw new ArgumentException("StoredProcedure custom attribute not might be null!.");

      UseDatabase oUseDatabases = getCustomAttribute<UseDatabase>(typeof(T));
      string SpName = string.Empty;
      SpName = oUseDatabases == null ? oStoredProcedure.Name : string.Empty;
      if (String.IsNullOrEmpty(SpName)) throw new ArgumentException("Need have a database where execute stored procedure.");
      SqlCommand oComando = new SqlCommand(SpName, oConnection);


      oComando.CommandTimeout = 240000;
      oComando.CommandType = System.Data.CommandType.StoredProcedure;

      Dictionary<string, string> oList = new Dictionary<string, string>();

      foreach (PropertyInfo oProperty in type.GetProperties())
      {
        Field oField = getCustomAttribute<Field>(oProperty);
        if (oField != null)
        {
          oList.Add("@" + oField.Name, oProperty.Name);
          SqlParameter oSqlParameter = new SqlParameter();
          oSqlParameter.ParameterName = "@" + oField.Name;
          var nullProperty = Nullable.GetUnderlyingType(oProperty.PropertyType);
          string propertyType = nullProperty == null? oProperty.PropertyType.Name : nullProperty.Name;
          switch (propertyType)
          {
            case "Byte[]": oSqlParameter.SqlDbType = SqlDbType.VarBinary; break;
            case "Int": oSqlParameter.SqlDbType = SqlDbType.Int; break;
            case "String": oSqlParameter.SqlDbType = SqlDbType.VarChar; break;
            case "Decimal": oSqlParameter.SqlDbType = SqlDbType.Decimal; break;
            case "DateTime": oSqlParameter.SqlDbType = SqlDbType.DateTime; break;
            case "Float": oSqlParameter.SqlDbType = SqlDbType.Float; break;
            case "Boolean": oSqlParameter.SqlDbType = SqlDbType.Bit; break;
            case "Numeric": oSqlParameter.SqlDbType = SqlDbType.Money; break;
            case "Xml": oSqlParameter.SqlDbType = SqlDbType.Xml; break;
          }
          oSqlParameter.Direction = oField.Direction;
          oSqlParameter.Value = oProperty.GetValue(oData);
          if (oSqlParameter.Value == null) oSqlParameter.Value = DBNull.Value;
          if (oSqlParameter.Size == 0 && oField.Value1 != 0)
          {
              oSqlParameter.Size = oField.Value1;
          }
          oComando.Parameters.Add(oSqlParameter);
        }
        //throw new ArgumentException("Field custom attribute not sould be null!.");
      }
      int affect = oComando.ExecuteNonQuery();

      foreach (SqlParameter oParameter in oComando.Parameters)
      {
        if (oParameter.Direction == ParameterDirection.Output)
        {
          string nameProperty = oList[oParameter.ParameterName];
          PropertyInfo againProperty = type.GetProperty(nameProperty);
          againProperty.SetValue(oData, oParameter.Value);
        }
      }
      return affect > 0 ? true : false;
    }
    public List<Result> ExecuteStoredProcedure<T, Result>(ref object oData)
    {
      Type type = typeof(T);
      StoredProcedure oStoredProcedure = getCustomAttribute<StoredProcedure>(type);
      if (oStoredProcedure == null) throw new ArgumentException("StoredProcedure custom attribute not sould be null!.");

      SqlCommand oComando = new SqlCommand(oStoredProcedure.Name, oConnection);
      oComando.CommandTimeout = 240000;
      oComando.CommandType = System.Data.CommandType.StoredProcedure;

      Dictionary<string, string> oList = new Dictionary<string, string>();

      foreach (PropertyInfo oProperty in type.GetProperties())
      {
        Field oField = getCustomAttribute<Field>(oProperty);
        if (oField == null) throw new ArgumentException("Field custom attribute not sould be null!.");
        oList.Add("@" + oField.Name, oProperty.Name);

        SqlParameter oSqlParameter = new SqlParameter();
        oSqlParameter.ParameterName = "@" + oField.Name;
        var nullProperty = Nullable.GetUnderlyingType(oProperty.PropertyType);
        string propertyType = nullProperty == null ? oProperty.PropertyType.Name : nullProperty.Name;
        switch (propertyType)
        {
          case "Int": oSqlParameter.SqlDbType = SqlDbType.Int; break;
          case "String": oSqlParameter.SqlDbType = SqlDbType.VarChar; break;
          case "Decimal": oSqlParameter.SqlDbType = SqlDbType.Decimal; break;
          case "DateTime": oSqlParameter.SqlDbType = SqlDbType.DateTime; break;
          case "Float": oSqlParameter.SqlDbType = SqlDbType.Float; break;
          case "Boolean": oSqlParameter.SqlDbType = SqlDbType.Bit; break;
          case "Numeric": oSqlParameter.SqlDbType = SqlDbType.Money; break;
          case "Xml": oSqlParameter.SqlDbType = SqlDbType.Xml; break;
        }

        oSqlParameter.Direction = oField.Direction;
        oSqlParameter.Value = oProperty.GetValue(oData);
        if (oSqlParameter.Value == null) oSqlParameter.Value = DBNull.Value;
        if (oSqlParameter.Size == 0 && oField.Value1!=0) {
            oSqlParameter.Size = oField.Value1;
        }
        oComando.Parameters.Add(oSqlParameter);
      }
      List<Result> rs = new List<Result>();
      using (SqlDataReader reader = oComando.ExecuteReader())
      {
        Type typers = typeof(Result);

        if (reader.HasRows)
        {
          while (reader.Read())
          {
            if (reader.FieldCount == 1)
            {
              object value = reader.GetValue(0);
              if (!String.IsNullOrEmpty(value.ToString())) rs.Add((Result)value);

             }
            else
            {
              Result reg = (Result)Activator.CreateInstance(typers);
              for (int i = 0; i < reader.FieldCount; i++)
              {
                if (reader.GetValue(i) != DBNull.Value)
                {
                  string name = reader.GetName(i);
                  if (name != null)
                  {
                    PropertyInfo property = typers.GetProperty(name);
                    if (property != null)
                    {
                      Type pType = property.PropertyType;
                      if (pType.FullName == "System.Boolean")
                      {
                        object obj = reader.GetValue(i);
                        switch (obj)
                        {
                          case 1:
                            property.SetValue((object)reg, true);
                            break;
                          case 0:
                            property.SetValue((object)reg, false);
                            break;
                          case true:
                            property.SetValue((object)reg, true);
                            break;
                          case false:
                            property.SetValue((object)reg, false);
                            break;
                          default:
                            throw new ArgumentException("Can't cast value boolean");
                        }
                      }
                      else
                      {
                        property.SetValue((object)reg, reader.GetValue(i));
                      }
                    }
                  }
                }
              }
              rs.Add(reg);
            }
          }
        }
      }

      foreach (SqlParameter oParameter in oComando.Parameters)
      {
        if (oParameter.Direction == ParameterDirection.Output)
        {
          PropertyInfo againProperty = type.GetProperty(oParameter.ParameterName.Remove(0,1));
          againProperty.SetValue(oData, oParameter.Value);
        }
      }
      return rs;
    }


    /***********************************************************/
    /* CREAN CONSULTAS EN BASE DE DATOS, Y METODOS DE APOYO ****/
    /***********************************************************/

    private List<T> ExecuteQueryWithLevels<T>(string query, Dictionary<string, string> indice)
    {
      Type type = typeof(T);
      DbTable AttributeTable = type.GetCustomAttributes(typeof(DbTable), true).FirstOrDefault() as DbTable;

      List<T> result = new List<T>();

      SqlCommand oComando = new SqlCommand(query, oConnection);
      oComando.CommandTimeout = 240000;
      using (SqlDataReader reader = oComando.ExecuteReader())
      {
        List<string> headers = reader.GetSchemaTable().Rows.Cast<DataRow>().Select(r => (string)r["ColumnName"]).ToList();

        // CREA EL DICCIONARIO DE TODAS LAS PROPIEDADES
        Dictionary<string, PropertyInfo> oDictionaryProperties = new Dictionary<string, PropertyInfo>();
        List<string> pathClasses = new List<string>();

        foreach (string head in headers)
        {
          if (head != "dbname$")
          {
            string overgetHead = indice[head];
            var listHeader = overgetHead.Split('$').ToList();
            string Clase = "";
            oDictionaryProperties.Add(overgetHead, getPropertyFromPathHeader(overgetHead, type, ref Clase));
            pathClasses.Add(Clase);
          }
        }

        List<OrdenadoDeInfo> OrdenDeClases = new List<OrdenadoDeInfo>();
        List<string> shortList = pathClasses.Distinct().ToList();
        foreach (string head in shortList)
        {
          string sClase = GetHeadClassFromPathHeader(head, type);
          int onLevel = head.Split('$').Count();
          OrdenDeClases.Add(new OrdenadoDeInfo() { NameClass = sClase, NamePropertyOfClass = head, level = onLevel });
        }

        // INTERACTUA CON LOS REGISTROS ENCONTRADOS
        if (reader.HasRows)
        {
          while (reader.Read())
          {
            // CREA DICCIONARIO DE TODOS LAS CLASES OBJETO
            Dictionary<string, object> oClasses = new Dictionary<string, object>();

            T ItemResult = (T)Activator.CreateInstance(type);
            oClasses.Add(AttributeTable.Name, (object)ItemResult);

            foreach (string oItem in shortList)
            {
              PropertyInfo oInfo = getPropertyFromPathHeader(oItem, type);
              if (oInfo != null)
              {
                oClasses.Add(oItem, (object)Activator.CreateInstance(oInfo.PropertyType));
              }
            }

            // CARGA INFORMACION EN EL DICCIONARIO DE CLASES OBJETO
            for (int i = 0; i < reader.FieldCount; i++)
            {
              string head = reader.GetName(i);
              object Value = reader.GetValue(i);

              if (head == "dbname$")
              {
                MethodInfo method = type.GetMethod("SetDatabaseName", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
                if (method != null)
                {
                  method.Invoke(ItemResult, new object[] { Value });
                }
              }
              else
              {
                string overgetHead = indice[head];
                List<string> listHeader = overgetHead.Split('$').ToList();
                string headClass = GetHeadClassFromPathHeader(overgetHead, type);

                PropertyInfo oProperty = oDictionaryProperties[overgetHead];
                if (Value != DBNull.Value)
                {
                  if (AttributeTable.Name == headClass)
                  {
                    oProperty.SetValue((object)oClasses[AttributeTable.Name], Value);
                  }
                  else
                  {
                    oProperty.SetValue((object)oClasses[headClass], Value);
                  }
                }
              }
            }


            // ORDENADO DEL CONTENIDO DE LAS CLASES SOBRE CLASE PRINCIPAL
            List<OrdenadoDeInfo> ordenando = OrdenDeClases.OrderByDescending(o => o.level).ToList();
            foreach (var item in ordenando)
            {
              if (item.level != 1)
              {
                string sProperty = item.NamePropertyOfClass.Split('$').ToList().Last();
                Type oType = oClasses[item.NameClass].GetType();
                PropertyInfo oProperty = oType.GetProperty(sProperty);
                oProperty.SetValue((object)oClasses[item.NameClass], (object)oClasses[item.NamePropertyOfClass]);
              }
            }

            result.Add(ItemResult);

          }
        }
      }
      return result;
    }
    private string getIfExistModelQuery<T>()
    {
      string sSql = "select tab.name as TableName " + //schema_name(tab.schema_id) as SchemaName,
            "from sys.tables as tab where tab.name like '@nametbl'";
      Type oType = typeof(T);
      DbTable oDbTable = getCustomAttribute<DbTable>(oType);
      if (oDbTable != null)
      {
        sSql = sSql.Replace("@nametbl", oDbTable.Name);
        return sSql;
      }
      return null;
    }
    private string getCreateQuery<T>(object? oData = null)
    {
      Type type = typeof(T);
      PropertyInfo[] properties = type.GetProperties();

      Dictionary<string, string> oFieldNameInProperties = new Dictionary<string, string>();
      foreach (PropertyInfo property in properties)
      {
        Field oField = getCustomAttribute<Field>(property);
        if (oField != null)
        {
          oFieldNameInProperties.Add(oField.Name, property.Name);
        }
      }

      string CreateTable = @"CREATE TABLE ";

      #region DEFINE SI TIENE ORIGEN EN UNA BASE DE DATOS MAPEADO
      UseDatabase attrUseDatabase = getCustomAttribute<UseDatabase>(type);
      object sNameDatabase = null;
      if (attrUseDatabase != null)
      {
        if (attrUseDatabase.Databases.Count == 1)
        {
          sNameDatabase = attrUseDatabase.Databases.First();
        }
        else
        {
          if (oData != null)
          {
            MethodInfo method = type.GetMethod("GetDatabaseName", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            if (method != null)
            {
              sNameDatabase = method.Invoke(oData, new object[] { });
            }
            if (sNameDatabase == null)
            {
              List<BelongToDatabase> oBelongDBList = getCustomAttributeList<BelongToDatabase>(type);
              if (oBelongDBList != null)
              {
                BelongToDatabase oBelongDB = oBelongDBList.FirstOrDefault();
                if (oBelongDB != null)
                {
                  string nameCol = oFieldNameInProperties.Where(o => o.Key == oBelongDB.ColumnKeyDatabase).FirstOrDefault().Value;
                  if (nameCol == null) throw new ArgumentException("Can't find which database to save!.");

                  var valueIdDB = type.GetProperty(nameCol).GetValue(oData);
                  oBelongDB = oBelongDBList.Where(o => o.ValueInColumn.Equals(valueIdDB)).FirstOrDefault();
                  if (oBelongDB == null) throw new ArgumentException("Can't find which database to save!.");
                  sNameDatabase = oBelongDB.DatabaseName;
                }
              }
            }
          }
        }
      }
      CreateTable += sNameDatabase != null ? sNameDatabase + ".." : "";
      #endregion


      DbTable Attribute = getCustomAttribute<DbTable, T>();
      CreateTable += Attribute != null ? Attribute.Name : type.Name;
      CreateTable += " (";

      List<PropertyInfo> oFields = new List<PropertyInfo>();
      foreach (PropertyInfo oPropery in properties)
      {
        if (getCustomAttribute<Field>(oPropery) != null)
        {
          oFields.Add(oPropery);
        }
      }

      string Fields = "";
      foreach (PropertyInfo itm in oFields)
      {
        Field oField = getCustomAttribute<Field>(itm);
        Fields += getFieldParameterInCreate(itm);
        Fields += itm == properties.Last() ? "" : ", ";
      }
      CreateTable += Fields + ");";
      return CreateTable;
    }
    private string getSaveQuery<T>(object oData)
    {
      Type type = typeof(T);
      PropertyInfo[] properties = type.GetProperties();
      Dictionary<string, string> oFieldNameInProperties = new Dictionary<string, string>();
      foreach (PropertyInfo property in properties)
      {
        Field oField = getCustomAttribute<Field>(property);
        if (oField != null)
        {
          oFieldNameInProperties.Add(oField.Name, property.Name);
        }
      }

      string oQuery = "INSERT INTO ";

      #region DEFINE SI TIENE ORIGEN EN UNA BASE DE DATOS MAPEADO
      UseDatabase attrUseDatabase = getCustomAttribute<UseDatabase>(type);
      object sNameDatabase = null;
      if (attrUseDatabase != null)
      {
        if (attrUseDatabase.Databases.Count == 1)
        {
          sNameDatabase = attrUseDatabase.Databases.First();
        }
        else
        {
          MethodInfo method = type.GetMethod("GetDatabaseName", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
          if (method != null)
          {
            sNameDatabase = method.Invoke(oData, new object[] { });
          }

          if (sNameDatabase == null)
          {
            List<BelongToDatabase> oBelongDBList = getCustomAttributeList<BelongToDatabase>(type);
            if (oBelongDBList != null)
            {
              BelongToDatabase oBelongDB = oBelongDBList.FirstOrDefault();
              if (oBelongDB != null)
              {
                string nameCol = oFieldNameInProperties.Where(o => o.Key == oBelongDB.ColumnKeyDatabase).FirstOrDefault().Value;
                if (nameCol == null) throw new ArgumentException("Can't find which database to save!.");

                var valueIdDB = type.GetProperty(nameCol).GetValue(oData);
                oBelongDB = oBelongDBList.Where(o => o.ValueInColumn.Equals(valueIdDB)).FirstOrDefault();
                if (oBelongDB == null) throw new ArgumentException("Can't find which database to save!.");
                sNameDatabase = oBelongDB.DatabaseName;
              }
            }
          }
        }
      }
      oQuery += sNameDatabase != null ? sNameDatabase + ".." : "";
      #endregion

      DbTable Attribute = getCustomAttribute<DbTable, T>();
      oQuery = Attribute != null ? oQuery += Attribute.Name : type.Name;

      oQuery += " (";

      List<PropertyInfo> items = new List<PropertyInfo>();

      foreach (PropertyInfo itm in properties)
      {
        Field attrs = getCustomAttribute<Field>(itm);
        if (attrs != null)
        {
          bool IsAutoIncrement = attrs.IsAutoIncrement();
          if (!IsAutoIncrement)
          {
            items.Add(itm);
          }
        }
      }

      foreach (PropertyInfo item in items)
      {
        Field attrs = getCustomAttribute<Field>(item);
        oQuery += attrs != null ? attrs.Name : item.Name;
        oQuery += item == items.Last() ? "" : ",";
      }


      oQuery += ") VALUES (";
      foreach (PropertyInfo itm in items)
      {
        oQuery += itm.PropertyType.ToString() == "System.Int32" ? itm.GetValue(oData) : "";
        oQuery += itm.PropertyType.ToString() == "System.Decimal" ? itm.GetValue(oData) : "";
        oQuery += itm.PropertyType.ToString() == "System.String" ? itm.GetValue(oData) == null ? "null" : "'" + itm.GetValue(oData) + "'" : "";
        oQuery += itm.PropertyType.ToString() == "System.DateTime" ? Convert.ToDateTime(itm.GetValue(oData)).ToString("yyyy-MM-ddThh:mm:ss") == "0001-01-01T12:00:00" ? "null" : "'" + Convert.ToDateTime(itm.GetValue(oData)).ToString("yyyy-MM-ddThh:mm:ss") + "'" : "";
        oQuery += itm.PropertyType.ToString() == "System.TimeSpan" ? "'" + itm.GetValue(oData) + "'" : "";
        oQuery += itm.PropertyType.ToString() == "System.Boolean" ? itm.GetValue(oData).ToString() == "False" ? "0" : "1" : "";
        oQuery += itm.PropertyType.ToString() == "System.Guid" ? itm.GetValue(oData) == null ? "null" : itm.GetValue(oData).ToString() == "00000000-0000-0000-0000-000000000000" ? "null" : "'" + itm.GetValue(oData) + "'" : "";
        oQuery += itm == items.Last() ? "" : ",";
      }

      oQuery += ");";
      return oQuery;
    }
    private string getSaveQuery<T>(object oData, params string[] onlySave)
    {
      Type type = typeof(T);
      DbTable Attribute = getCustomAttribute<DbTable, T>();

      object sNameDatabase = null;
      MethodInfo method = type.GetMethod("GetDatabaseName", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
      if (method != null)
      {
        sNameDatabase = method.Invoke(oData, new object[] { });
      }

      string oQuery = "INSERT INTO ";
      oQuery += sNameDatabase != null ? sNameDatabase + ".." : "";
      oQuery += Attribute != null ? Attribute.Name : type.Name;
      oQuery += " (";

      foreach (string item in onlySave)
      {
        PropertyInfo oProperty = type.GetProperty(item);
        Field attrs = getCustomAttribute<Field>(oProperty);
        oQuery += attrs != null ? attrs.Name : item;
        oQuery += item == onlySave.Last() ? "" : ",";
      }
      oQuery += ") VALUES (";

      foreach (string item in onlySave)
      {
        PropertyInfo oProperty = type.GetProperty(item);
        oQuery += oProperty.PropertyType.ToString() == "System.Int32" ? oProperty.GetValue(oData) : "";
        oQuery += oProperty.PropertyType.ToString() == "System.Decimal" ? oProperty.GetValue(oData) : "";
        oQuery += oProperty.PropertyType.ToString() == "System.String" ? "'" + oProperty.GetValue(oData) + "'" : "";
        oQuery += oProperty.PropertyType.ToString() == "System.DateTime" ? "'" + oProperty.GetValue(oData) + "'" : "";
        oQuery += oProperty.PropertyType.ToString() == "System.TimeSpan" ? "'" + oProperty.GetValue(oData) + "'" : "";
        oQuery += oProperty.PropertyType.ToString() == "System.Boolean" ? oProperty.GetValue(oData) : "";
        oQuery += item == onlySave.Last() ? "" : ",";
      }
      oQuery += ");";
      return oQuery;
    }
    private string getUpdateQuery<T>(object oData)
    {
      Type type = typeof(T);
      PropertyInfo[] properties = type.GetProperties();
      DbTable Attribute = getCustomAttribute<DbTable, T>();

      Dictionary<string, string> oFieldNameInProperties = new Dictionary<string, string>();
      foreach (PropertyInfo property in properties)
      {
        Field oField = getCustomAttribute<Field>(property);
        if (oField != null)
        {
          oFieldNameInProperties.Add(oField.Name, property.Name);
        }
      }

      string oQuery = "UPDATE ";

      #region DEFINE SI TIENE ORIGEN EN UNA BASE DE DATOS MAPEADO
      UseDatabase attrUseDatabase = getCustomAttribute<UseDatabase>(type);
      object sNameDatabase = null;
      if (attrUseDatabase != null)
      {
        if (attrUseDatabase.Databases.Count == 1)
        {
          sNameDatabase = attrUseDatabase.Databases.First();
        }
        else
        {
          MethodInfo method = type.GetMethod("GetDatabaseName", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
          if (method != null)
          {
            sNameDatabase = method.Invoke(oData, new object[] { });
          }

          if (sNameDatabase == null)
          {
            List<BelongToDatabase> oBelongDBList = getCustomAttributeList<BelongToDatabase>(type);
            if (oBelongDBList != null)
            {
              BelongToDatabase oBelongDB = oBelongDBList.FirstOrDefault();
              if (oBelongDB != null)
              {
                string nameCol = oFieldNameInProperties.Where(o => o.Key == oBelongDB.ColumnKeyDatabase).FirstOrDefault().Value;
                if (nameCol == null) throw new ArgumentException("Can't find which database to save!.");

                var valueIdDB = type.GetProperty(nameCol).GetValue(oData);
                oBelongDB = oBelongDBList.Where(o => o.ValueInColumn.Equals(valueIdDB)).FirstOrDefault();
                if (oBelongDB == null) throw new ArgumentException("Can't find which database to save!.");
                sNameDatabase = oBelongDB.DatabaseName;
              }
            }
          }
        }
      }
      oQuery += sNameDatabase != null ? sNameDatabase + ".." : "";
      #endregion

      oQuery = Attribute != null ? oQuery += Attribute.Name : type.Name;
      oQuery += " SET ";

      List<PropertyInfo> oListFields = new List<PropertyInfo>();
      foreach (PropertyInfo itm in properties)
      {
        Field attrs = getCustomAttribute<Field>(itm);
        if (attrs != null)
        {
          oListFields.Add(itm);
        }
      }

      List<PropertyInfo> oListKeys = new List<PropertyInfo>();
      foreach (PropertyInfo itm in oListFields)
      {
        Field attrs = getCustomAttribute<Field>(itm);
        if (attrs.IsPrimaryKey()) oListKeys.Add(itm);
        if (!attrs.IsPrimaryKey())
        {
          oQuery += attrs != null ? attrs.Name : itm.Name;
          oQuery += " = ";
          oQuery += itm.PropertyType.ToString() == "System.Int32" ? itm.GetValue(oData) : "";
          oQuery += itm.PropertyType.ToString() == "System.Decimal" ? itm.GetValue(oData) : "";
          oQuery += itm.PropertyType.ToString() == "System.String" ? itm.GetValue(oData) == null ? "null" : "'" + itm.GetValue(oData) + "'" : "";
          oQuery += itm.PropertyType.ToString() == "System.DateTime" ? Convert.ToDateTime(itm.GetValue(oData)).ToString("yyyy-MM-ddThh:mm:ss") == "0001-01-01T12:00:00" ? "null" : "'" + Convert.ToDateTime(itm.GetValue(oData)).ToString("yyyy-MM-ddThh:mm:ss") + "'" : "";
          oQuery += itm.PropertyType.ToString() == "System.TimeSpan" ? "'" + itm.GetValue(oData) + "'" : "";
          oQuery += itm.PropertyType.ToString() == "System.Boolean" ? itm.GetValue(oData).ToString() == "False" ? "0" : "1" : "";
          oQuery += itm.PropertyType.ToString() == "System.Guid" ? itm.GetValue(oData) == null ? "null" : itm.GetValue(oData).ToString() == "00000000-0000-0000-0000-000000000000" ? "null" : "'" + itm.GetValue(oData) + "'" : "";
          oQuery += itm == oListFields.Last() ? "" : ",";
        }
      }
      oQuery += " WHERE ";
      foreach (PropertyInfo oItem in oListKeys)
      {
        Field attrs = getCustomAttribute<Field>(oItem);
        if (attrs != null)
        {
          oQuery += attrs != null ? attrs.Name : oItem.Name;
          oQuery += " = ";
          oQuery += oItem.PropertyType.ToString() == "System.Int32" ? oItem.GetValue(oData) : "";
          oQuery += oItem.PropertyType.ToString() == "System.Decimal" ? oItem.GetValue(oData) : "";
          oQuery += oItem.PropertyType.ToString() == "System.String" ? oItem.GetValue(oData) == null ? "null" : "'" + oItem.GetValue(oData) + "'" : "";
          oQuery += oItem.PropertyType.ToString() == "System.DateTime" ? Convert.ToDateTime(oItem.GetValue(oData)).ToString("yyyy-MM-ddThh:mm:ss") == "0001-01-01T12:00:00" ? "null" : "'" + Convert.ToDateTime(oItem.GetValue(oData)).ToString("yyyy-MM-ddThh:mm:ss") + "'" : "";
          oQuery += oItem.PropertyType.ToString() == "System.TimeSpan" ? "'" + oItem.GetValue(oData) + "'" : "";
          oQuery += oItem.PropertyType.ToString() == "System.Boolean" ? oItem.GetValue(oData).ToString() == "False" ? "0" : "1" : "";
          oQuery += oItem.PropertyType.ToString() == "System.Guid" ? oItem.GetValue(oData) == null ? "null" : oItem.GetValue(oData).ToString() == "00000000-0000-0000-0000-000000000000" ? "null" : "'" + oItem.GetValue(oData) + "'" : "";
          oQuery += oItem == oListKeys.Last() ? "" : " and ";
        }
      }
      oQuery += ";";
      return oQuery;
    }
    private string getUpdateQuery<T>(object oData, params string[] onlySave)
    {
      List<string> mySave = onlySave.ToList();

      Type type = typeof(T);
      PropertyInfo[] properties = type.GetProperties();
      DbTable Attribute = getCustomAttribute<DbTable, T>();

      Dictionary<string, string> oFieldNameInProperties = new Dictionary<string, string>();
      foreach (PropertyInfo property in properties)
      {
        Field oField = getCustomAttribute<Field>(property);
        if (oField != null)
        {
          oFieldNameInProperties.Add(oField.Name, property.Name);
        }
      }

      string oQuery = "UPDATE ";

      #region DEFINE SI TIENE ORIGEN EN UNA BASE DE DATOS MAPEADO
      UseDatabase attrUseDatabase = getCustomAttribute<UseDatabase>(type);
      object sNameDatabase = null;
      if (attrUseDatabase != null)
      {
        if (attrUseDatabase.Databases.Count == 1)
        {
          sNameDatabase = attrUseDatabase.Databases.First();
        }
        else
        {
          MethodInfo method = type.GetMethod("GetDatabaseName", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
          if (method != null)
          {
            sNameDatabase = method.Invoke(oData, new object[] { });
          }

          if (sNameDatabase == null)
          {
            List<BelongToDatabase> oBelongDBList = getCustomAttributeList<BelongToDatabase>(type);
            if (oBelongDBList != null)
            {
              BelongToDatabase oBelongDB = oBelongDBList.FirstOrDefault();
              if (oBelongDB != null)
              {
                string nameCol = oFieldNameInProperties.Where(o => o.Key == oBelongDB.ColumnKeyDatabase).FirstOrDefault().Value;
                if (nameCol == null) throw new ArgumentException("Can't find which database to save!.");

                var valueIdDB = type.GetProperty(nameCol).GetValue(oData);
                oBelongDB = oBelongDBList.Where(o => o.ValueInColumn.Equals(valueIdDB)).FirstOrDefault();
                if (oBelongDB == null) throw new ArgumentException("Can't find which database to save!.");
                sNameDatabase = oBelongDB.DatabaseName;
              }
            }
          }
        }
      }
      oQuery += sNameDatabase != null ? sNameDatabase + ".." : "";
      #endregion

      oQuery = Attribute != null ? oQuery += Attribute.Name : type.Name;
      oQuery += " SET ";

      List<PropertyInfo> oListKeys = new List<PropertyInfo>();
      List<string> setUpdates = new List<string>();
      foreach (PropertyInfo itm in properties)
      {
        Field attrs = getCustomAttribute<Field>(itm);
        if (attrs != null)
        {
          if (attrs.IsPrimaryKey()) oListKeys.Add(itm);
          else
          {
            if (mySave.Contains(itm.Name))
            {
              string aSetUpdate = "";
              aSetUpdate += attrs != null ? attrs.Name : itm.Name;
              aSetUpdate += " = ";
              aSetUpdate += itm.PropertyType.ToString() == "System.Int32" ? itm.GetValue(oData) : "";
              aSetUpdate += itm.PropertyType.ToString() == "System.Decimal" ? itm.GetValue(oData) : "";
              aSetUpdate += itm.PropertyType.ToString() == "System.String" ? itm.GetValue(oData) == null ? "null" : "'" + itm.GetValue(oData) + "'" : "";
              aSetUpdate += itm.PropertyType.ToString() == "System.DateTime" ? Convert.ToDateTime(itm.GetValue(oData)).ToString("yyyy-MM-ddThh:mm:ss") == "0001-01-01T12:00:00" ? "null" : "'" + Convert.ToDateTime(itm.GetValue(oData)).ToString("yyyy-MM-ddThh:mm:ss") + "'" : "";
              aSetUpdate += itm.PropertyType.ToString() == "System.TimeSpan" ? "'" + itm.GetValue(oData) + "'" : "";
              aSetUpdate += itm.PropertyType.ToString() == "System.Boolean" ? itm.GetValue(oData).ToString() == "False" ? "0" : "1" : "";
              aSetUpdate += itm.PropertyType.ToString() == "System.Guid" ? itm.GetValue(oData) == null ? "null" : itm.GetValue(oData).ToString() == "00000000-0000-0000-0000-000000000000" ? "null" : "'" + itm.GetValue(oData) + "'" : "";
              setUpdates.Add(aSetUpdate);
            }
          }

        }
      }

      foreach (string itemSet in setUpdates)
      {
        oQuery += itemSet;
        oQuery += itemSet != setUpdates.Last() ? " , " : "";
      }

      oQuery += " WHERE ";
      foreach (PropertyInfo oItem in oListKeys)
      {
        Field attrs = getCustomAttribute<Field>(oItem);
        if (attrs != null)
        {
          oQuery += attrs != null ? attrs.Name : oItem.Name;
          oQuery += " = ";
          oQuery += oItem.PropertyType.ToString() == "System.Int32" ? oItem.GetValue(oData) : "";
          oQuery += oItem.PropertyType.ToString() == "System.Decimal" ? oItem.GetValue(oData) : "";
          oQuery += oItem.PropertyType.ToString() == "System.String" ? oItem.GetValue(oData) == null ? "null" : "'" + oItem.GetValue(oData) + "'" : "";
          oQuery += oItem.PropertyType.ToString() == "System.DateTime" ? Convert.ToDateTime(oItem.GetValue(oData)).ToString("yyyy-MM-ddThh:mm:ss") == "0001-01-01T12:00:00" ? "null" : "'" + Convert.ToDateTime(oItem.GetValue(oData)).ToString("yyyy-MM-ddThh:mm:ss") + "'" : "";
          oQuery += oItem.PropertyType.ToString() == "System.TimeSpan" ? "'" + oItem.GetValue(oData) + "'" : "";
          oQuery += oItem.PropertyType.ToString() == "System.Boolean" ? oItem.GetValue(oData).ToString() == "False" ? "0" : "1" : "";
          oQuery += oItem.PropertyType.ToString() == "System.Guid" ? oItem.GetValue(oData) == null ? "null" : oItem.GetValue(oData).ToString() == "00000000-0000-0000-0000-000000000000" ? "null" : "'" + oItem.GetValue(oData) + "'" : "";
          oQuery += oItem == oListKeys.Last() ? "" : " and ";
        }
      }
      oQuery += ";";
      return oQuery;
    }
    private string getDeleteQuery<T>(object oData, int limiteSelect = 0)
    {
      string oQuery = "DELETE ";
      oQuery += limiteSelect > 0 ? " TOP(" + limiteSelect + ") " : "";
      oQuery += " FROM ";

      Type type = typeof(T);
      PropertyInfo[] properties = type.GetProperties();
      DbTable Attribute = getCustomAttribute<DbTable, T>();

      Dictionary<string, string> oFieldNameInProperties = new Dictionary<string, string>();
      foreach (PropertyInfo property in properties)
      {
        Field oField = getCustomAttribute<Field>(property);
        if (oField != null)
        {
          oFieldNameInProperties.Add(oField.Name, property.Name);
        }
      }

      #region DEFINE SI TIENE ORIGEN EN UNA BASE DE DATOS MAPEADO
      UseDatabase attrUseDatabase = getCustomAttribute<UseDatabase>(type);
      object sNameDatabase = null;
      if (attrUseDatabase != null)
      {
        if (attrUseDatabase.Databases.Count == 1)
        {
          sNameDatabase = attrUseDatabase.Databases.First();
        }
        else
        {
          MethodInfo method = type.GetMethod("GetDatabaseName", BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
          if (method != null)
          {
            sNameDatabase = method.Invoke(oData, new object[] { });
          }

          if (sNameDatabase == null)
          {
            List<BelongToDatabase> oBelongDBList = getCustomAttributeList<BelongToDatabase>(type);
            if (oBelongDBList != null)
            {
              BelongToDatabase oBelongDB = oBelongDBList.FirstOrDefault();
              if (oBelongDB != null)
              {
                string nameCol = oFieldNameInProperties.Where(o => o.Key == oBelongDB.ColumnKeyDatabase).FirstOrDefault().Value;
                if (nameCol == null) throw new ArgumentException("Can't find which database to save!.");

                var valueIdDB = type.GetProperty(nameCol).GetValue(oData);
                oBelongDB = oBelongDBList.Where(o => o.ValueInColumn.Equals(valueIdDB)).FirstOrDefault();
                if (oBelongDB == null) throw new ArgumentException("Can't find which database to save!.");
                sNameDatabase = oBelongDB.DatabaseName;
              }
            }
          }
        }
      }
      oQuery += sNameDatabase != null ? sNameDatabase + ".." : "";
      #endregion


      oQuery = Attribute != null ? oQuery += Attribute.Name : type.Name;

      oQuery += " WHERE ";

      List<PropertyInfo> oListKeys = new List<PropertyInfo>();
      foreach (PropertyInfo itm in properties)
      {
        Field attrs = getCustomAttribute<Field>(itm);
        if (attrs != null)
        {
          if (attrs.IsPrimaryKey()) oListKeys.Add(itm);
        }
      }

      foreach (PropertyInfo oItem in oListKeys)
      {
        Field attrs = getCustomAttribute<Field>(oItem);
        if (attrs != null)
        {
          oQuery += attrs != null ? attrs.Name : oItem.Name;
          oQuery += " = ";
          oQuery += oItem.PropertyType.ToString() == "System.Int32" ? oItem.GetValue(oData) : "";
          oQuery += oItem.PropertyType.ToString() == "System.Decimal" ? oItem.GetValue(oData) : "";
          oQuery += oItem.PropertyType.ToString() == "System.String" ? oItem.GetValue(oData) == null ? "null" : "'" + oItem.GetValue(oData) + "'" : "";
          oQuery += oItem.PropertyType.ToString() == "System.DateTime" ? Convert.ToDateTime(oItem.GetValue(oData)).ToString("yyyy-MM-ddThh:mm:ss") == "0001-01-01T12:00:00" ? "null" : "'" + Convert.ToDateTime(oItem.GetValue(oData)).ToString("yyyy-MM-ddThh:mm:ss") + "'" : "";
          oQuery += oItem.PropertyType.ToString() == "System.TimeSpan" ? "'" + oItem.GetValue(oData) + "'" : "";
          oQuery += oItem.PropertyType.ToString() == "System.Boolean" ? oItem.GetValue(oData).ToString() == "False" ? "0" : "1" : "";
          oQuery += oItem.PropertyType.ToString() == "System.Guid" ? oItem.GetValue(oData) == null ? "null" : oItem.GetValue(oData).ToString() == "00000000-0000-0000-0000-000000000000" ? "null" : "'" + oItem.GetValue(oData) + "'" : "";
          oQuery += oItem == oListKeys.Last() ? "" : " and ";
        }
      }
      return oQuery;
    }

    private string getWayString(string value, string typename)
    {
      switch (typename)
      {
        case "Int32":
          return value;
        case "String":
          return "'" + value + "'";
        case "Decimal":
          return value;
        case "DateTime":
          return "'" + value + "'";
        case "Float":
          return value;
        default:
          throw new ArgumentException("Not valid typename value");
      }
    }
    private string getStringSelect(Type oType, int limitSelect = 0)
    {
      string result = "SELECT ";
      result += limitSelect > 0 ? " TOP(" + limitSelect + ") " : "";
      List<PropertyInfo> shortList = new List<PropertyInfo>();
      foreach (PropertyInfo oInfo in oType.GetProperties())
      {
        Field oField = getCustomAttribute<Field>(oInfo);
        if (oField != null)
        {
          shortList.Add(oInfo);
        }
      }

      foreach (PropertyInfo oInfo in shortList)
      {
        Field Propiedad = getCustomAttribute<Field>(oInfo);
        result += oType.Name + "." + Propiedad.Name;
        result += " AS ";
        result += oInfo.Name;
        result += oInfo == shortList.Last() ? "" : ", ";
        result += Environment.NewLine;
      }


      return result;
    }
    private string getStringFrom(Type oType)
    {
      string result = " FROM ";
      DbTable AttributeTable = getCustomAttribute<DbTable>(oType);
      result += Environment.NewLine;
      result += AttributeTable.Name + " AS " + oType.Name;
      return result;
    }
    private string getStringFromInDatabase(Type oType, string sDatabaseName)
    {
      string result = " FROM ";
      DbTable AttributeTable = getCustomAttribute<DbTable>(oType);
      result += Environment.NewLine;
      result += sDatabaseName + ".." + AttributeTable.Name + " AS " + oType.Name;
      return result;
    }
    private string getStringSelect(object oData, Type oType, ref Dictionary<string, string> indice, int levelSelect = 0, bool LoadObjAvalible = false, int limitSelect = 0)
    {
      string result = "SELECT ";
      result += limitSelect > 0 ? " TOP(" + limitSelect + ") " : "";

      List<PropertyInfo> shortList = new List<PropertyInfo>();
      List<PropertyInfo> shortListBelong = new List<PropertyInfo>();

      DbTable AttributeTable = getCustomAttribute<DbTable>(oType);
      string tableName = AttributeTable != null ? AttributeTable.Name : oType.Name;


      foreach (PropertyInfo oInfo in oType.GetProperties())
      {
        if (getCustomAttribute<Field>(oInfo) != null)
        {
          shortList.Add(oInfo);
        }
        if (getCustomAttribute<BelongsTo>(oInfo) != null)
        {
          shortListBelong.Add(oInfo);
        }
      }

      foreach (PropertyInfo oInfo in shortList)
      {
        Field Propiedad = getCustomAttribute<Field>(oInfo);

        string PropertyInMd5_InBackSql = "a" + SomeData.GetMD5(tableName);
        string PropertyInMd5_InRename = "a" + SomeData.GetMD5(tableName + "$" + oInfo.Name);
        indice.Add(PropertyInMd5_InRename, tableName + "$" + oInfo.Name);

        result += PropertyInMd5_InBackSql + "." + Propiedad.Name;
        result += " AS ";
        result += PropertyInMd5_InRename;
        result += oInfo == shortList.Last() ? "" : ", ";
        result += Environment.NewLine;
      }

      foreach (PropertyInfo oInfo in shortListBelong)
      {
        if (levelSelect > 0)
        {
          if (LoadObjAvalible)
          {
            if (oData != null)
            {
              var oOfProperty = oInfo.GetValue(oData);
              if (oOfProperty != null)
              {
                result += getStringSelectWithOrigin(oData, tableName + "$" + oInfo.Name, oInfo, ref indice, levelSelect, LoadObjAvalible);
              }
            }
            else
            {
              result += getStringSelectWithOrigin(oData, tableName + "$" + oInfo.Name, oInfo, ref indice, levelSelect, LoadObjAvalible);
            }
          }
        }
      }

      return result;
    }
    private string getStringSelectV2(object oData, Type oType, ref Dictionary<string, string> indice, int levelSelect = 0, bool LoadObjAvalible = false, int limitSelect = 0)
    {
      string result = "SELECT ";
      // result += limitSelect > 0 ? " TOP(" + limitSelect + ") " : "";

      List<PropertyInfo> shortList = new List<PropertyInfo>();
      List<PropertyInfo> shortListBelong = new List<PropertyInfo>();

      DbTable AttributeTable = getCustomAttribute<DbTable>(oType);
      string tableName = AttributeTable != null ? AttributeTable.Name : oType.Name;

      DbView AttributeView = getCustomAttribute<DbView>(oType);
      if (AttributeView != null)
      {
        tableName = AttributeView != null ? AttributeView.Name : oType.Name;
      }


      foreach (PropertyInfo oInfo in oType.GetProperties())
      {
        if (getCustomAttribute<Field>(oInfo) != null)
        {
          shortList.Add(oInfo);
        }
        if (getCustomAttribute<BelongsTo>(oInfo) != null)
        {
          shortListBelong.Add(oInfo);
        }
      }
      bool IfActivateIsNull = false;
      foreach (PropertyInfo oInfo in shortList)
      {
        Field Propiedad = getCustomAttribute<Field>(oInfo);

        string PropertyInMd5_InBackSql = "a" + SomeData.GetMD5(tableName);
        string PropertyInMd5_InRename = "a" + SomeData.GetMD5(tableName + "$" + oInfo.Name);
        indice.Add(PropertyInMd5_InRename, tableName + "$" + oInfo.Name);

        result += IfActivateIsNull ? "isnull("+ PropertyInMd5_InBackSql + "." + Propiedad.Name+",'')" : PropertyInMd5_InBackSql + "." + Propiedad.Name;
        result += " AS ";
        result += PropertyInMd5_InRename;
        result += oInfo == shortList.Last() ? "" : ", ";
        result += Environment.NewLine;
      }

      foreach (PropertyInfo oInfo in shortListBelong)
      {
        if (levelSelect > 0)
        {
          if (LoadObjAvalible)
          {
            if (oData != null)
            {
              var oOfProperty = oInfo.GetValue(oData);
              if (oOfProperty != null)
              {
                result += getStringSelectWithOrigin(oOfProperty, tableName + "$" + oInfo.Name, oInfo, ref indice, levelSelect, LoadObjAvalible);
              }
            }
          }
          else
          {
            result += getStringSelectWithOrigin(oData, tableName + "$" + oInfo.Name, oInfo, ref indice, levelSelect, LoadObjAvalible);
          }
        }
      }

      return result;
    }
    private string getStringSelectWithOrigin(object oData, string sOrigen, PropertyInfo oParent, int levelSelect = 0, bool LoadObjAvalible = false)
    {
      levelSelect -= 1;
      string result = "";
      BelongsTo oParentDetails = getCustomAttribute<BelongsTo>(oParent);
      List<PropertyInfo> shortList = new List<PropertyInfo>();
      List<PropertyInfo> shortListBelong = new List<PropertyInfo>();
      foreach (PropertyInfo oInfo in oParent.PropertyType.GetProperties())
      {
        if (getCustomAttribute<Field>(oInfo) != null)
        {
          shortList.Add(oInfo);
        }
        if (getCustomAttribute<BelongsTo>(oInfo) != null)
        {
          shortListBelong.Add(oInfo);
        }
      }

      if (oParentDetails != null)
      {
        foreach (PropertyInfo oInfo in shortList)
        {
          Field Propiedad = getCustomAttribute<Field>(oInfo);
          result += oInfo == shortList.First() ? ", " : "";
          result += sOrigen + "." + Propiedad.Name;
          result += " AS ";
          result += sOrigen + "$" + oInfo.Name;
          result += oInfo == shortList.Last() ? "" : ", ";
          result += Environment.NewLine;
        }

        foreach (PropertyInfo oInfo in shortListBelong)
        {
          if (levelSelect > 0)
          {
            result += getStringSelectWithOrigin(oData, sOrigen + "$" + oInfo.Name, oInfo, levelSelect, LoadObjAvalible);
          }
        }
      }
      else
      {
        throw new ArgumentException("The property parent must have 'BelongTo' in custom property");
      }
      return result;
    }
    private string getStringSelectWithOrigin(object oData, string sOrigen, PropertyInfo oParent, ref Dictionary<string, string> indice, int levelSelect = 0, bool LoadObjAvalible = false)
    {
      levelSelect -= 1;
      string result = "";
      BelongsTo oParentDetails = getCustomAttribute<BelongsTo>(oParent);
      List<PropertyInfo> shortList = new List<PropertyInfo>();
      List<PropertyInfo> shortListBelong = new List<PropertyInfo>();
      foreach (PropertyInfo oInfo in oParent.PropertyType.GetProperties())
      {
        if (getCustomAttribute<Field>(oInfo) != null)
        {
          shortList.Add(oInfo);
        }
        if (getCustomAttribute<BelongsTo>(oInfo) != null)
        {
          shortListBelong.Add(oInfo);
        }
      }

      if (oParentDetails != null)
      {
        foreach (PropertyInfo oInfo in shortList)
        {
          Field Propiedad = getCustomAttribute<Field>(oInfo);
          string PropertyInMd5_InBackSql = "a" + SomeData.GetMD5(sOrigen);
          string PropertyInMd5_InRename = "a" + SomeData.GetMD5(sOrigen + "$" + oInfo.Name);
          indice.Add(PropertyInMd5_InRename, sOrigen + "$" + oInfo.Name);

          result += oInfo == shortList.First() ? ", " : "";
          result += PropertyInMd5_InBackSql + "." + Propiedad.Name;
          result += " AS ";
          result += PropertyInMd5_InRename;
          result += oInfo == shortList.Last() ? "" : ", ";
          result += Environment.NewLine;
        }

        foreach (PropertyInfo oInfo in shortListBelong)
        {
          if (levelSelect > 0)
          {
            if (LoadObjAvalible)
            {
              if (oData != null)
              {
                var oOfProperty = oInfo.GetValue(oData);
                if (oOfProperty != null)
                {
                  result += getStringSelectWithOrigin(oOfProperty, sOrigen + "$" + oInfo.Name, oInfo, ref indice, levelSelect, LoadObjAvalible);
                }
              }
            }
            else
            {
              result += getStringSelectWithOrigin(oData, sOrigen + "$" + oInfo.Name, oInfo, ref indice, levelSelect, LoadObjAvalible);
            }
          }
        }
      }
      else
      {
        throw new ArgumentException("The property parent must have 'BelongTo' in custom property");
      }
      return result;
    }
    private string getStringFromIndice(object oData, Type oType, int levelSelect = 0, bool LoadObjAvalible = false)
    {
      string result = " FROM ";
      DbTable AttributeTable = getCustomAttribute<DbTable>(oType);
      string tableName = AttributeTable != null ? AttributeTable.Name : oType.Name;
      string PropertyInMd5_TableRename = "a" + SomeData.GetMD5(tableName);

      DbView AttributeView = getCustomAttribute<DbView>(oType);
      if (AttributeView != null)
      {
        tableName = AttributeView != null ? AttributeView.Name : oType.Name;
        PropertyInMd5_TableRename = "a" + SomeData.GetMD5(tableName);
      }

      result += Environment.NewLine;
      result += AttributeTable.Name + " AS " + PropertyInMd5_TableRename;

      List<PropertyInfo> shortListBelong = new List<PropertyInfo>();
      foreach (PropertyInfo oInfo in oType.GetProperties())
      {
        BelongsTo oBelongParam = getCustomAttribute<BelongsTo>(oInfo);
        if (oBelongParam != null)
        {
          shortListBelong.Add(oInfo);
        }
      }
      if (levelSelect > 0)
      {
        foreach (PropertyInfo oInfo in shortListBelong)
        {
          if (LoadObjAvalible)
          {
            if (oData != null)
            {
              var oOfProperty = oInfo.GetValue(oData);
              if (oOfProperty != null)
              {
                result += getStringFromWithJoin(oData, AttributeTable.Name, oInfo, levelSelect, LoadObjAvalible);
              }
            }

          }
          else
          {
            result += getStringFromWithJoin(oData, AttributeTable.Name, oInfo, levelSelect, LoadObjAvalible);
          }
        }
      }


      return result;
    }
    private string getStringFromIndice(object oData, Type oType, string sDatabaseName, int levelSelect = 0, bool LoadObjAvalible = false)
    {
      string result = " FROM ";
      DbTable AttributeTable = getCustomAttribute<DbTable>(oType);
      string tableName = AttributeTable != null ? AttributeTable.Name : oType.Name;
      string PropertyInMd5_TableRename = "a" + SomeData.GetMD5(tableName);

      DbView AttributeView = getCustomAttribute<DbView>(oType);
      if (AttributeView != null)
      {
        tableName = AttributeView != null ? AttributeView.Name : oType.Name;
        PropertyInMd5_TableRename = "a" + SomeData.GetMD5(tableName);
      }

      result += Environment.NewLine;
      result += sDatabaseName + ".." + AttributeTable.Name + " AS " + PropertyInMd5_TableRename;

      List<PropertyInfo> shortListBelong = new List<PropertyInfo>();
      foreach (PropertyInfo oInfo in oType.GetProperties())
      {
        BelongsTo oBelongParam = getCustomAttribute<BelongsTo>(oInfo);
        if (oBelongParam != null)
        {
          shortListBelong.Add(oInfo);
        }
      }
      if (levelSelect > 0)
      {
        foreach (PropertyInfo oInfo in shortListBelong)
        {
          if (LoadObjAvalible)
          {
            if (oData != null)
            {
              var oOfProperty = oInfo.GetValue(oData);
              if (oOfProperty != null)
              {
                result += getStringFromWithJoin(oOfProperty, AttributeTable.Name, oInfo, sDatabaseName, levelSelect, LoadObjAvalible);
              }
            }
          }
          else
          {
            result += getStringFromWithJoin(oData, AttributeTable.Name, oInfo, sDatabaseName, levelSelect, LoadObjAvalible);
          }

        }
      }


      return result;
    }
    private string getStringFromWithJoin(object oData, string sOrigen, PropertyInfo oParent, int levelSelect = 0, bool LoadObjAvalible = false)
    {
      levelSelect -= 1;
      string result = "";
      Type oType = oParent.PropertyType;
      DbTable AttributeTable = getCustomAttribute<DbTable>(oType);
      BelongsTo Propiedad = getCustomAttribute<BelongsTo>(oParent);
      string PropertyInMd5_TableRename = "a" + SomeData.GetMD5(sOrigen + "$" + oParent.Name);

      result += Environment.NewLine;
      switch (Propiedad.oTypeJoin)
      {
        case Enumerics.TypesJoin.InnerJoin:
          result += " INNER JOIN ";
          break;
        case Enumerics.TypesJoin.Join:
          result += " JOIN ";
          break;
        case Enumerics.TypesJoin.LeftJoin:
          result += " LEFT JOIN ";
          break;
        case Enumerics.TypesJoin.RightJoin:
          result += " RIGHT JOIN ";
          break;
        case Enumerics.TypesJoin.LeftOuterJoin:
          result += " LEFT OUTER JOIN ";
          break;
        case Enumerics.TypesJoin.RightOuterJoin:
          result += " RIGHT OUTER JOIN ";
          break;
        default:
          result += " LEFT JOIN ";
          break;
      }
      result += AttributeTable.Name + " AS " + PropertyInMd5_TableRename;
      result += " ON " + PropertyInMd5_TableRename + "." + Propiedad.IdReference + " = a" + SomeData.GetMD5(sOrigen) + "." + Propiedad.Name;
      foreach (BelongsTo oItem in getCustomAttributeList<BelongsTo>(oParent).Skip(1))
      {
        result += " AND " + PropertyInMd5_TableRename + "." + oItem.IdReference + " = a" + SomeData.GetMD5(sOrigen) + "." + oItem.Name;
      }

      List<PropertyInfo> shortListBelong = new List<PropertyInfo>();
      foreach (PropertyInfo oInfo in oType.GetProperties())
      {
        if (getCustomAttribute<BelongsTo>(oInfo) != null)
        {
          shortListBelong.Add(oInfo);
        }
      }
      if (levelSelect > 0)
      {
        foreach (PropertyInfo oInfo in shortListBelong)
        {
          if (LoadObjAvalible)
          {
            if (oData != null)
            {
              var oOfProperty = oInfo.GetValue(oData);
              if (oOfProperty != null)
              {
                result += getStringFromWithJoin(oData, sOrigen + "$" + oParent.Name, oInfo, levelSelect, LoadObjAvalible);
              }
            }
          }
          else
          {
            result += getStringFromWithJoin(oData, sOrigen + "$" + oParent.Name, oInfo, levelSelect, LoadObjAvalible);
          }

        }
      }
      return result;
    }
    private string getStringFromWithJoin(object oData, string sOrigen, PropertyInfo oParent, string sDatabaseName, int levelSelect = 0, bool LoadObjAvalible = false)
    {
      levelSelect -= 1;
      string result = "";
      Type oType = oParent.PropertyType;
      DbTable AttributeTable = getCustomAttribute<DbTable>(oType);
      //List<BelongToDatabase> oListDatabases = getCustomAttributeList<BelongToDatabase>(oType);
      //BelongToDatabase oBelongToDatabase = oListDatabases.Where(o => o.DatabaseName == sDatabaseName).FirstOrDefault();
      //if (oBelongToDatabase == null) throw new ArgumentException("Not found relation with property BelongTo");

      BelongsTo Propiedad = getCustomAttribute<BelongsTo>(oParent);
      string PropertyInMd5_TableRename = "a" + SomeData.GetMD5(sOrigen + "$" + oParent.Name);

      result += Environment.NewLine;
      switch(Propiedad.oTypeJoin)
      {
        case Enumerics.TypesJoin.InnerJoin:
          result += " INNER JOIN ";
          break;
        case Enumerics.TypesJoin.Join:
          result += " JOIN ";
          break;
        case Enumerics.TypesJoin.LeftJoin:
          result += " LEFT JOIN ";
          break;
        case Enumerics.TypesJoin.RightJoin:
          result += " RIGHT JOIN ";
          break;
        case Enumerics.TypesJoin.LeftOuterJoin:
          result += " LEFT OUTER JOIN ";
          break;
        case Enumerics.TypesJoin.RightOuterJoin:
          result += " RIGHT OUTER JOIN ";
          break;
        default:
          result += " LEFT JOIN ";
          break;
      }
     
      result += sDatabaseName + ".." + AttributeTable.Name + " AS " + PropertyInMd5_TableRename;
      result += " ON " + PropertyInMd5_TableRename + "." + Propiedad.IdReference + " = a" + SomeData.GetMD5(sOrigen) + "." + Propiedad.Name;
      foreach (BelongsTo oItem in getCustomAttributeList<BelongsTo>(oParent).Skip(1))
      {
        result += " AND " + PropertyInMd5_TableRename + "." + oItem.IdReference + " = a" + SomeData.GetMD5(sOrigen) + "." + oItem.Name;
      }

      List<PropertyInfo> shortListBelong = new List<PropertyInfo>();
      foreach (PropertyInfo oInfo in oType.GetProperties())
      {
        if (getCustomAttribute<BelongsTo>(oInfo) != null)
        {
          shortListBelong.Add(oInfo);
        }
      }
      if (levelSelect > 0)
      {
        foreach (PropertyInfo oInfo in shortListBelong)
        {
          if (LoadObjAvalible)
          {
            if (oData != null)
            {
              var oOfProperty = oInfo.GetValue(oData);
              if (oOfProperty != null)
              {
                result += getStringFromWithJoin(oOfProperty, sOrigen + "$" + oParent.Name, oInfo, sDatabaseName, levelSelect, LoadObjAvalible);
              }
            }
          }
          else
          {
            result += getStringFromWithJoin(oData, sOrigen + "$" + oParent.Name, oInfo, sDatabaseName, levelSelect, LoadObjAvalible);
          }

        }
      }
      return result;
    }
    private Type[] getTypesInNamespace(Assembly assembly, string nameSpace)
    {
      return
      assembly.GetTypes()
          .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
          .ToArray();
    }


    //private Type getTypeByName(string name)
    //{
    //  List<Type> oTypesAssemblies = new List<Type>();
    //  foreach (Assembly oAssembly in assemblies)
    //  {
    //	List<Type> oTypesAssembly = oAssembly.GetTypes().Where(t => String.Equals(t.Name, name, StringComparison.Ordinal)).ToList();
    //	oTypesAssemblies.AddRange(oTypesAssembly);
    //  }
    //  return oTypesAssemblies.Count == 1 ? oTypesAssemblies.FirstOrDefault() : null;
    //}


    //private Type getTypeByAttributeName(string name)
    //{
    //  List<Type> oTypesAssemblies = new List<Type>();
    //  foreach (Assembly oAssembly in assemblies)
    //  {
    //	foreach (Type type in oAssembly.GetTypes())
    //	{
    //	  if (type.GetCustomAttributes(true).FirstOrDefault() != null)
    //	  {
    //		DbTable Attribute = type.GetCustomAttributes(true).FirstOrDefault().GetType() == typeof(DbTable) ?
    //						type.GetCustomAttributes(true).FirstOrDefault() as DbTable : null;
    //		if (Attribute != null)
    //		{
    //		  if (Attribute.Name == name)
    //		  {
    //			oTypesAssemblies.Add(type);
    //		  }
    //		}
    //	  }
    //	}
    //	//List<Type> oTypesAssembly = oAssembly.GetTypes().Where(t => String.Equals(t.GetCustomAttributes(typeof(Table), true).FirstOrDefault().GetType().Name, name, StringComparison.Ordinal)).ToList();
    //	//oTypesAssemblies.AddRange(oTypesAssembly);
    //  }
    //  return oTypesAssemblies.Count >= 1 ? oTypesAssemblies.FirstOrDefault() : null;
    //}


    private PropertyInfo getPropertyFromPathHeader(string pathHeader, Type oType)
    {
      List<string> listClasses = pathHeader.Split('$').ToList();
      PropertyInfo oProperty = null;

      foreach (string itemPropertyName in listClasses.Skip(1))
      {
        oProperty = oType.GetProperty(itemPropertyName);
        oType = oProperty.PropertyType;
      }
      return oProperty;
    }
    private PropertyInfo getPropertyFromPathHeader(string pathHeader, Type oType, ref string sClase)
    {
      List<string> listClasses = pathHeader.Split('$').ToList();
      PropertyInfo oProperty = null;
      int nVeses = 0;
      sClase = listClasses.Take(1).First();
      foreach (string itemPropertyName in listClasses.Skip(1))
      {
        nVeses++;
        oProperty = oType.GetProperty(itemPropertyName);
        oType = oProperty.PropertyType;
        sClase += nVeses < (listClasses.Count - 1) ? "$" + itemPropertyName : "";
      }
      return oProperty;
    }
    private string GetHeadClassFromPathHeader(string pathHeader, Type oType)
    {
      List<string> listClasses = pathHeader.Split('$').ToList();
      PropertyInfo oProperty = null;
      int nVeses = 0;
      string sClase = listClasses.First();
      foreach (string itemPropertyName in listClasses.Skip(1))
      {
        nVeses++;
        oProperty = oType.GetProperty(itemPropertyName);
        oType = oProperty.PropertyType;
        sClase += nVeses < (listClasses.Count - 1) ? "$" + itemPropertyName : "";
      }
      return sClase;
    }
    private struct OrdenadoDeInfo
    {
      public string NameClass;
      public string NamePropertyOfClass;
      public int level;
    }
    private string createWhereClause<T>(Expression<Func<T, object>> predicate)
    {
      Type type = typeof(T);
      DbTable Attribute = getCustomAttribute<DbTable, T>();
      string tableName = Attribute != null ? Attribute.Name : type.Name;

      StringBuilder p = new StringBuilder(predicate.Body.ToString());
      string compare = p.ToString();
      var pName = predicate.Parameters.First();
      int nParm = predicate.Parameters.Count();

      int start = compare.IndexOf("(") + 1;
      int end = compare.IndexOf(")");
      if (start >= 0 && end >= 0)
      {
        compare = compare.Substring(start, end - start);
        compare = compare.Replace(", Object", "");
      }
      compare = compare.Replace(".", "$");

      List<string> pathCompare = compare.Split('$').ToList();

      string result = tableName.ToString();
      int take = pathCompare.Count - 2;
      foreach (string item in pathCompare.Skip(1).Take(take))
      {
        result += "$" + item;
      }
      string partInMd5 = result;
      string partPrope = pathCompare.Last();

      result = "a" + SomeData.GetMD5(partInMd5) + "." + partPrope;

      return result;
    }
    private string createWhereClauseV2<T>(Expression<Func<T, object>> predicate)
    {
      Type type = typeof(T);
      DbTable Attribute = getCustomAttribute<DbTable, T>();
      string tableName = Attribute != null ? Attribute.Name : type.Name;

      StringBuilder p = new StringBuilder(predicate.Body.ToString());
      string compare = p.ToString();
      var pName = predicate.Parameters.First();
      int nParm = predicate.Parameters.Count();

      int start = compare.IndexOf("(") + 1;
      int end = compare.IndexOf(")");
      if (start >= 0 && end >= 0)
      {
        compare = compare.Substring(start, end - start);
        compare = compare.Replace(", Object", "");
      }
      compare = compare.Replace(".", "$");

      List<string> pathCompare = compare.Split('$').ToList();

      string result = tableName.ToString();
      int take = pathCompare.Count - 2;
      foreach (string item in pathCompare.Skip(1).Take(take))
      {
        result += "$" + item;
      }
      string partInMd5 = result;
      string partPrope = pathCompare.Last();

      result = "a" + SomeData.GetMD5(partInMd5 + "$" + partPrope);

      return result;
    }
    private string getFieldParameterInCreate(PropertyInfo oProperty)
    {
      string oField = oProperty.Name;
      Field oAttrib = getCustomAttribute<Field>(oProperty);
      if (oAttrib != null)
      {
        oField = oAttrib.Name;
        string type = oProperty.PropertyType.ToString();

        oField += getTypeDatabaseFromField(oProperty, oAttrib);

        oField = oAttrib.IsNotNull() ? oField += " NOT NULL" : oField;
        oField = type == "System.Int32" ? oAttrib.IsAutoIncrement() ? oField += " IDENTITY(1,1)" : oField : oField;
        oField = oAttrib.IsPrimaryKey() ? oField += " PRIMARY KEY" : oField;
      }
      return oField;
    }

    private string getTypeDatabaseFromField(PropertyInfo oProperty, Field oAttrib)
    {
      string oField = "";
      string type = oProperty.PropertyType.ToString();
      switch (type)
      {
        case "System.Int32":
          oField += " INT";
          break;
        case "System.Decimal":
          oField = oAttrib.Value1 > 0 && oAttrib.Value2 == 0 ? oField += " DECIMAL(" + oAttrib.Value1 + ")" : oField;
          oField = oAttrib.Value1 > 0 && oAttrib.Value2 > 0 ? oField += " DECIMAL(" + oAttrib.Value1 + "," + oAttrib.Value2 + ")" : oField;
          if (oAttrib.Value1 == 0 && oAttrib.Value2 == 0) throw new ArgumentException("error is cann't create field table, decimal need values");
          break;
        case "System.String":
          if (oAttrib.Value1 == -1) oField += " TEXT";
          if (oAttrib.Value1 > 0) oField += " VARCHAR(" + oAttrib.Value1 + ")";
          else throw new ArgumentException("error is cann't create field table, decimal need values");
          break;
        case "System.DateTime":
          oField += " DATETIME";
          break;
        case "System.Boolean":
          oField += " BIT";
          break;
        case "System.TimeSpan":
          oField += " TIME";
          break;
        case "System.Drawing.Color":
          oField += " INT";
          break;
        default:
          throw new ArgumentException("The type '" + type + "' isn't avalible in the conversion");
      }
      return oField;
    }
    private string getBelongToParameterInCreate(PropertyInfo oProperty)
    {
      string oField = "";
      int count = oProperty.GetCustomAttributes(true).Length;
      foreach (BelongsTo oAttrib in oProperty.GetCustomAttributes(typeof(BelongsTo), true))
      {
        if (oAttrib != null)
        {
          oField += oAttrib.Name;
          Type ChildType = oProperty.DeclaringType;
          Type ParentType = oProperty.PropertyType;
          PropertyInfo[] properties = ParentType.GetProperties();//.ToList<PropertyInfo>().Where(x => x.Name == oAttrib.IdReference).FirstOrDefault();
          foreach (PropertyInfo itm in properties)
          {
            Field ChildAttrs = getCustomAttribute<Field>(itm);

            if (ChildAttrs != null)
            {
              if (this.createForeingKey == true)
              {
                if (ChildAttrs.IsPrimaryKey())
                {
                  if (oAttrib.IdReference == ChildAttrs.Name)
                  {
                    oField += getTypeDatabaseFromField(itm, ChildAttrs) + " CONSTRAINT FK_" +
                    ChildType.Name + "$" + ParentType.Name +
                    "_On_" + oAttrib.Name + "$" + ChildAttrs.Name +
                    " FOREIGN KEY (" + oAttrib.Name + ") REFERENCES " + ParentType.Name + "(" + ChildAttrs.Name + ")";
                  }
                }
              }
              else
              {
                if (ChildAttrs.IsPrimaryKey())
                {
                  if (oAttrib.IdReference == ChildAttrs.Name)
                  {
                    oField += getTypeDatabaseFromField(itm, ChildAttrs);
                  }
                }
              }
            }
          }
        }
        else
        {
          throw new ArgumentException("object null in properties 'BelongsTo'");
        }
        BelongsTo oLast = (BelongsTo)oProperty.GetCustomAttributes(true).Last();

        oField += oAttrib.IdReference == oLast.IdReference ? "" : ", ";
      }
      return oField;
    }

    private P getCustomAttribute<P>(PropertyInfo oProperty)
    {
      List<object> Attribute = oProperty.GetCustomAttributes(typeof(P), true).ToList();
      return (P)Attribute.FirstOrDefault();
    }
    private P getCustomAttribute<P>(Type oType)
    {
      List<object> Attribute = oType.GetCustomAttributes(typeof(P), true).ToList();
      return (P)Attribute.FirstOrDefault();
    }
    private List<P> getCustomAttributeList<P>(PropertyInfo oProperty)
    {
      List<object> Attributes = oProperty.GetCustomAttributes(typeof(P), true).ToList();
      List<P> oReturnList = new List<P>();
      foreach (object oAttrib in Attributes)
      {
        oReturnList.Add((P)oAttrib);
      }
      return oReturnList;
    }
    private List<P> getCustomAttributeList<P>(Type oType)
    {
      List<object> Attributes = oType.GetCustomAttributes(typeof(P), true).ToList();
      List<P> oReturnList = new List<P>();
      foreach (object oAttrib in Attributes)
      {
        oReturnList.Add((P)oAttrib);
      }
      return oReturnList;
    }
    private P getCustomAttribute<P, O>()
    {
      List<object> Attribute = typeof(O).GetCustomAttributes(typeof(P), true).ToList();
      if (typeof(DbTable) == typeof(P))
      {
        if (Attribute.Count == 0) throw new ArgumentException("Might be declared table property in class");
        if (Attribute.Count > 1) throw new ArgumentException("Not should be more that one custom property table");
      }
      return (P)Attribute.FirstOrDefault();
    }
    private List<P> getCustomAttributeList<P, O>()
    {
      List<object> Attribute = typeof(O).GetCustomAttributes(typeof(P), true).ToList();
      return Attribute as List<P>;
    }


    /// <summary>
    /// Retrieves the collection element type from this type
    /// </summary>
    /// <param name="type">The type to query</param>
    /// <returns>The element type of the collection or null if the type was not a collection
    /// </returns>
    private Type GetCollectionElementType(Type type)
    {
      if (null == type) throw new ArgumentNullException("type");

      // first try the generic way
      // this is easy, just query the IEnumerable<T> interface for its generic parameter
      var etype = typeof(IEnumerable<>);
      foreach (var bt in type.GetInterfaces())
        if (bt.IsGenericType && bt.GetGenericTypeDefinition() == etype)
          return bt.GetGenericArguments()[0];

      // now try the non-generic way

      // if it's a dictionary we always return DictionaryEntry
      if (typeof(System.Collections.IDictionary).IsAssignableFrom(type))
        return typeof(System.Collections.DictionaryEntry);

      // if it's a list we look for an Item property with an int index parameter
      // where the property type is anything but object
      if (typeof(System.Collections.IList).IsAssignableFrom(type))
      {
        foreach (var prop in type.GetProperties())
        {
          if ("Item" == prop.Name && typeof(object) != prop.PropertyType)
          {
            var ipa = prop.GetIndexParameters();
            if (1 == ipa.Length && typeof(int) == ipa[0].ParameterType)
            {
              return prop.PropertyType;
            }
          }
        }
      }

      // if it's a collection, we look for an Add() method whose parameter is 
      // anything but object
      if (typeof(System.Collections.ICollection).IsAssignableFrom(type))
      {
        foreach (var meth in type.GetMethods())
        {
          if ("Add" == meth.Name)
          {
            var pa = meth.GetParameters();
            if (1 == pa.Length && typeof(object) != pa[0].ParameterType)
              return pa[0].ParameterType;
          }
        }
      }
      if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
        return typeof(object);
      return null;
    }

    private List<string> getListMissingFields<T>()
    {
      DbTable oTable = getCustomAttribute<DbTable, T>();
      Type type = typeof(T);
      PropertyInfo[] properties = type.GetProperties();
      List<string> values = new List<string>();

      string sqlGetProperties = @"select 
    col.column_id as Column_id,
    col.name as Column_name, 
    t.name as Sql_type,
    col.max_length as Max_length,
    col.precision as Precision,
	CASE
		WHEN (select top 1 column_name from INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC
						INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU
							ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' 
							AND TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME 
							AND KU.table_name=tab.name
							AND column_name=col.name ) = col.name
		THEN 1
		ELSE 0
	END AS PrimaryKey,
	CASE
		WHEN ((select top 1 column_name from INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC
						INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU
							ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' 
							AND TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME 
							AND KU.table_name=tab.name
							AND column_name=col.name ) = col.name) AND IDENT_SEED(tab.name) = 1 AND IDENT_INCR (tab.name) = 1
		THEN 1
		ELSE 0
	END AS AutoIncrement,
	CASE col.is_nullable
		WHEN 1
		THEN 0
		WHEN 0
		THEN 1
	END as NotNull,
	CASE t.name
		WHEN 'int'	     	THEN 'int'
		WHEN 'varchar'		THEN 'string'
		WHEN 'bit'			THEN 'bool'
		WHEN 'nvarchar'		THEN 'string'
		WHEN 'float'		THEN 'double'
		WHEN 'char'			THEN 'string'
		WHEN 'nchar'		THEN 'string'
		WHEN 'numeric'		THEN 'decimal'
		WHEN 'smalldatetime'THEN 'date'
		WHEN 'datetime'		THEN 'DateTime'
		WHEN 'smallint'		THEN 'int'
		WHEN 'decimal'		THEN 'decimal'
		WHEN 'time'			THEN 'Time'
	END	AS Code_type
from sys.tables as tab inner join sys.columns as col on tab.object_id = col.object_id
left join sys.types as t on col.user_type_id = t.user_type_id where tab.name like '" + oTable.Name + "' order by column_id";

      List<PropertySQL> oListSQLProperties = ExecuteQuery<PropertySQL>(sqlGetProperties);
      foreach (PropertyInfo oProperty in properties)
      {
        Field oField = getCustomAttribute<Field>(oProperty);
        if (oField != null)
        {
          PropertySQL oPropertySQL = oListSQLProperties.Where(o => o.Column_name == oField.Name).FirstOrDefault();
          bool autoIncrement = oField.IsAutoIncrement();
          bool notNull = oField.IsNotNull();
          bool primaryKey = oField.IsPrimaryKey();

          if (oPropertySQL == null)
          {
            values.Add(oField.Name);
          }
        }
      }

      return values;
    }


  }
}

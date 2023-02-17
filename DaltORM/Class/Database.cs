using DaltORM.Class;
using DaltORM.Enumerics;
using DaltORM.IDatabases;
using DaltORM.Interfaces;
using DaltORM.PropertyClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DaltORM
{
  public struct DataConnection
  {
    public string Server;
    public int Port;
    public string Instance;
    public string Database;
    public List<string> Databases;
    public string User;
    public string Password;
    public string Other;
  }

  // Clase para establecer la conexion
  public class Database
  {
    protected static IDatabase iDatabase { get; set; }

    /// <summary>
    /// Instancia Protegida
    /// </summary>
    /// <returns>
    /// </returns>
    protected Database()
    {
    }

    /// <summary>
    /// Instancia la conexion
    /// </summary>
    /// <param name="datosConexion">Datos de conexion.</param>
    /// <param name="tipo">Tipo de conexion MySql, SQL, Firebird.</param>
    /// <returns>Carga a interfaz la conexion de servidor de BD
    /// </returns>
    private Database(DataConnection datosConexion, DatabaseTypes tipo)
    {
      if (tipo == DatabaseTypes.SQLServer)
      {
        Database.iDatabase = new FunctionsWithSQLserver(datosConexion);
        Database.iDatabase.CreateConnection();
      }
      if (tipo == DatabaseTypes.MySQL)
      {
        Database.iDatabase = new FunctionWithMySQL(datosConexion);
        Database.iDatabase.CreateConnection();
      }
      if (tipo == DatabaseTypes.FirebirdSQL)
      {
        Database.iDatabase = new FunctionWithFirebird(datosConexion);
        Database.iDatabase.CreateConnection();
      }
    }
    private Database(string datosConexion, DatabaseTypes tipo)
    {
        if (tipo == DatabaseTypes.SQLServer)
        {
            Database.iDatabase = new FunctionsWithSQLserver(datosConexion);
            Database.iDatabase.CreateConnection();
        }
        if (tipo == DatabaseTypes.MySQL)
        {
            Database.iDatabase = new FunctionWithMySQL(datosConexion);
            Database.iDatabase.CreateConnection();
        }
        if (tipo == DatabaseTypes.FirebirdSQL)
        {
            Database.iDatabase = new FunctionWithFirebird(datosConexion);
            Database.iDatabase.CreateConnection();
        }
        }
    /// <summary>
    /// Instancia la conexion
    /// </summary>
    /// <param name="datosConexion">Datos de conexion.</param>
    /// <param name="tipo">Tipo de conexion MySql, SQL, Firebird.</param>
    /// <param name="assemblies">Assemblies de los proyectos que contienen las clases mapeadas</param>
    /// <returns>Carga a interfaz la conexion de servidor de BD y Recibe assemblies para validar el mapeo de la BD
    /// </returns>
        //private Database(DataConnection datosConexion, DatabaseTypes tipo, params Assembly[] assemblies)
        //   {

        //     if (tipo == DatabaseTypes.SQLServer)
        //     {
        //	Database.iDatabase = new FunctionsWithSQLserver(datosConexion, assemblies);
        //	Database.iDatabase.CreateConnection();
        //     }
        //     if (tipo == DatabaseTypes.MySQL)
        //     {
        //	Database.iDatabase = new FunctionWithMySQL(datosConexion, assemblies);
        //	Database.iDatabase.CreateConnection();
        //     }
        //     if (tipo == DatabaseTypes.FirebirdSQL)
        //     {
        //	Database.iDatabase = new FunctionWithFirebird(datosConexion, assemblies);
        //	Database.iDatabase.CreateConnection();
        //     }
        //   }

        /// <summary>
        /// Instancia la conexion
        /// </summary>
        /// <param name="datosConexion">Datos de conexion.</param>
        /// <param name="tipo">Tipo de conexion MySql, SQL, Firebird.</param>
        /// <returns>Carga a interfaz la conexion de servidor de BD
        /// </returns>
    public static Database CreateConnection(DataConnection DatosConexion, DatabaseTypes tipo)
    {
      return new Database(DatosConexion, tipo);
    }
    public static Database CreateConnection(string ConnextionString, DatabaseTypes tipo)
    {
        return new Database(ConnextionString, tipo);
    }
        /// <summary>
        /// Instancia la conexion
        /// </summary>
        /// <param name="datosConexion">Datos de conexion.</param>
        /// <param name="tipo">Tipo de conexion MySql, SQL, Firebird.</param>
        /// <param name="assemblies">Assemblies de los proyectos que contienen las clases mapeadas</param>
        /// <returns>Carga a interfaz la conexion de servidor de BD y Recibe assemblies para validar el mapeo de la BD
        /// </returns>
        //public static Database CreateConnection(DataConnection DatosConexion, DatabaseTypes tipo, params Assembly[] assemblies)
        //   {
        //     return new Database(DatosConexion, tipo, assemblies);
        //   }

        /// <summary>
        /// Obtiene el estado de la conexion
        /// </summary>
        /// <returns>texto Open/Closed
        /// </returns>
        public string GetStateConnection()
    {
      return iDatabase.StateConnection();
    }
    /// <summary>
    /// Abre conexion al servidor
    /// </summary>
    /// <returns>
    /// </returns>
    public void Open()
    {
      iDatabase.OpenConnection();
    }
    /// <summary>
    /// Cierra conexion al servidor
    /// </summary>
    /// <returns>
    /// </returns>
    public void Close()
    {
      iDatabase.CloseConnection();
    }

    /// <summary>
    /// Cambia de Catalogo de base de datos
    /// </summary>
    /// <param name="sNameDatabase">Nombre de la base de datos</param>
    /// <returns>Cambia de catalogo la conexion existente
    /// </returns>
    public bool UseDatabase(string sNameDatabase)
    {
      return iDatabase.UseDatabase(sNameDatabase);
    }

    public bool UseDbByIdAsigConce(string sIdAsigConce)
    {
      return true;
    }

    /// <summary>
    /// Consulta un query sql.
    /// </summary>
    /// <param name="query">command query string</param>
    /// <returns>devuelve en una lista de clases sobre reflexion
    /// </returns>
    public List<T> ExecuteQuery<T>(string query)
    {
      return iDatabase.ExecuteQuery<T>(query);
    }
    /// <summary>
    /// Ejecuta un query sql.
    /// </summary>
    /// <param name="query">command query string</param>
    /// <returns>Devuelve un falso/verdadero si se ejecuto correctamente
    /// </returns>
    public bool ExecuteQuery(string query)
    {
      return iDatabase.ExecuteQuery(query);
    }
    /// <summary>
    /// Ejecuta una lista de  query's sql en una transaccion.
    /// </summary>
    /// <returns>
    /// </returns>
    protected void SendTransaction(List<string> querys)
    {
      iDatabase.SendTransaction(querys);
    }
  }




  // Clase para operaciones en tabla de la Base de datos
  public class DatabaseTable<T> : Database
  {
    /// <summary>
    /// Activa la relacion de llaves primarias al crear una tabla
    /// </summary>
    /// <returns>
    /// </returns>
    public void ForeingKeyInCreate(bool create)
    {
      iDatabase.ForeingKeysInCreate(create);
    }
    /// <summary>
    /// Checa si existe el modelo en la base de datos
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool Exist()
    {
      return iDatabase.ExistModel<T>();
    }
    /// <summary>
    /// Crea la tabla en la base de datos
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool Create()
    {
      return iDatabase.Create<T>(this);
    }
    /// <summary>
    /// Guarda los datos en BD
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool Save()
    {
      object oData = this;
      bool result = iDatabase.Save<T>(ref oData);
      if (result)
      {
        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo property in properties)
        {
          property.SetValue(this, property.GetValue(oData));
        }
      }
      return result;
    }
    /// <summary>
    /// Guarda los datos en BD
    /// </summary>
    /// <param name="onlySave">Array de expresion lamba para indicar que datos guardar</param>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool Save(params Expression<Func<T, object>>[] onlySave)
    {
      return iDatabase.Save<T>(this, onlySave);
    }
    /// <summary>
    /// Guarda o actualiza los datos en BD
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool SaveOrUpdate()
    {
      object oData = this;
      bool result = iDatabase.SaveOrUpdate<T>(ref oData);
      if (result)
      {
        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo property in properties)
        {
          property.SetValue(this, property.GetValue(oData));
        }
      }
      return result;
    }
    /// <summary>
    /// Hace un Update de la clase en BD
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool Update()
    {
      return iDatabase.Update<T>(this);
    }
    /// <summary>
    /// Hace un Update de la clase en BD de los campos indicados
    /// </summary>
    /// <param name="onlySave">Array de expresion lamba para indicar que datos guardar</param>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool Update(params Expression<Func<T, object>>[] onlySave)
    {
      return iDatabase.Update<T>(this, onlySave);
    }
    /// <summary>
    /// Borra el registro de la BD
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool Delete()
    {
      return iDatabase.Delete<T>(this);
    }

    /// <summary>
    /// Construye la sentencia where SQL expression lambda que a partir de las propiedades puede evaluar
    /// </summary>
    /// <param name="expression">Expression para indicar valor a evaluar</param>
    /// <returns>Regresa una Objeto WHERE que hereda metodos para comparar la propiedad indicada del constructor
    /// </returns>
    public Where<T> Where(Expression<Func<T, object>> expression)
    {
      return iDatabase.Where<T>(expression, this);
    }
    /// <summary>
    /// Agrega un top(limit) a la consulta se usa este metodo antes del WHERE, este valor se mantiene en la interfaz hasta un GET
    /// </summary>
    /// <param name="limiteSelect">Indicar la cantidad de registros a consultar</param>
    /// <returns>Regresa un objeto del mismo tipo
    /// </returns>
    public Select<T> Limit(int limiteSelect)
    {
      return new Select<T>(limiteSelect, this);
    }
    /// <summary>
    /// Recarga los datos en un objeto de la clase para recuperar datos de nuevo
    /// </summary>
    /// <returns>Regresa un objeto de la clase actualizado
    /// </returns>
    public T Reload()
    {
      return iDatabase.Reload<T>(this);
    }
    /// <summary>
    /// Evalua si es valida la clase para poder ejecutarse los metodos de BD
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool IsValid()
    {
      return true;
    }
    /// <summary>
    /// Evalua si todas las propiedades de la clase estan declaradas en la BD
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool VerifyFields()
    {
      return iDatabase.VerifyFields<T>();
    }
    /// <summary>
    /// Registra y sincroniza las propiedades de la clase con la BD
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool SynchronizeFields()
    {
      return iDatabase.SynchronizeFields<T>();
    }
    /// <summary>
    /// Sincroniza el modelo de la clase en la base de datos y si no lo encuentra crea el modelo
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool Synchronize()
    {
      return iDatabase.Synchronize<T>();
    }

    /// <summary>
    /// Obtiene todos los registros de la BD
    /// </summary>
    /// <returns>Regresa una lista de clases obtenida de la BD
    /// </returns>
    public List<T> GetAll(int level = 0, bool LoadObjAvalible = false)
    {
      return iDatabase.Get<T>("", "", level, this, LoadObjAvalible);
    }
    /// <summary>
    /// Carga listas de datos relacionados que estan instanciadas a este objeto
    /// </summary>
    /// <returns>Carga de informacion de listas a este objeto 'BelongToMany'
    /// </returns>
    public void GetList()
    {
      object oData = this;
      if (iDatabase.GetList<T>(ref oData))
      {
        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo property in properties)
        {
          property.SetValue(this, property.GetValue(oData));
        }
      };
    }

    /// <summary>
    /// Carga informacion desde otro objeto de clase que contenga las mismas propiedades
    /// </summary>
    /// <param name="oAddValues">Ingresar la clase de objeto que tenga las mismas propiedades con valores para asignarse a esta</param>
    /// <returns>Regresa este mismo objeto con la informacion precargada
    /// </returns>
    public bool AddValues(object oAddValues)
    {
      try
      {
        Type type = typeof(T);
        foreach (PropertyInfo oProperty in type.GetProperties())
        {
          var oValue = oProperty.GetValue(oAddValues);
          if (oValue != null)
          {
            oProperty.SetValue(this, oValue);
          }
        }
        return true;
      }
      catch
      {
        return false;
      }
    }
  }


  public abstract class MultiDatabaseTable<T> : Database
  {
    /// <summary>
    /// Guarda en el objeto de clase mapeado el nombre de la base de datos
    /// </summary>
    /// <param name="sName">Nombre de la BD que pertenece</param>
    /// <returns>No regresa ningun valor</returns>
    public abstract void SetDatabaseName(string sName);
    /// <summary>
    /// Obtiene el nombre de la base de datos guardado
    /// </summary>
    /// <returns>Retorna el nombre de la BD</returns>
    public abstract string GetDatabaseName();

    /// <summary>
    /// Activa la relacion de llaves primarias al crear una tabla
    /// </summary>
    /// <returns>
    /// </returns>
    public void ForeingKeyInCreate(bool create)
    {
      iDatabase.ForeingKeysInCreate(create);
    }
    /// <summary>
    /// Checa si existe el modelo en la base de datos
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool Exist()
    {
      return iDatabase.ExistModel<T>();
    }
    /// <summary>
    /// Crea la tabla en la base de datos
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool Create()
    {
      return iDatabase.Create<T>();
    }
    /// <summary>
    /// Guarda los datos en BD
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool Save()
    {
      object oData = this;
      bool result = iDatabase.Save<T>(ref oData);
      if (result)
      {
        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo property in properties)
        {
          property.SetValue(this, property.GetValue(oData));
        }
      }
      return result;
    }
    /// <summary>
    /// Guarda los datos en BD
    /// </summary>
    /// <param name="onlySave">Array de expresion lamba para indicar que datos guardar</param>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool Save(params Expression<Func<T, object>>[] onlySave)
    {
      return iDatabase.Save<T>(this, onlySave);
    }
    /// <summary>
    /// Guarda o actualiza los datos en BD
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool SaveOrUpdate()
    {
      object oData = this;
      bool result = iDatabase.SaveOrUpdate<T>(ref oData);
      if (result)
      {
        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo property in properties)
        {
          property.SetValue(this, property.GetValue(oData));
        }
      }
      return result;
    }
    /// <summary>
    /// Hace un Update de la clase en BD
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool Update()
    {
      return iDatabase.Update<T>(this);
    }
    /// <summary>
    /// Hace un Update de la clase en BD de los campos indicados
    /// </summary>
    /// <param name="onlySave">Array de expresion lamba para indicar que datos guardar</param>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool Update(params Expression<Func<T, object>>[] onlySave)
    {
      return iDatabase.Update<T>(this, onlySave);
    }
    /// <summary>
    /// Borra el registro de la BD
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool Delete()
    {
      return iDatabase.Delete<T>(this);
    }

    /// <summary>
    /// Construye la sentencia where SQL expression lambda que a partir de las propiedades puede evaluar
    /// </summary>
    /// <param name="expression">Expression para indicar valor a evaluar</param>
    /// <returns>Regresa una Objeto WHERE que hereda metodos para comparar la propiedad indicada del constructor
    /// </returns>
    public Where<T> Where(Expression<Func<T, object>> expression)
    {
      return iDatabase.Where<T>(expression, this);
    }

    public Where<T> WhereCluster(Expression<Func<T, object>> expression)
    {
      return iDatabase.WhereCluster<T>(expression, this);
    }

    /// <summary>
    /// Agrega un top(limit) a la consulta se usa este metodo antes del WHERE, este valor se mantiene en la interfaz hasta un GET
    /// </summary>
    /// <param name="limiteSelect">Indicar la cantidad de registros a consultar</param>
    /// <returns>Regresa un objeto del mismo tipo
    /// </returns>
    public Select<T> Limit(int limiteSelect)
    {
      object oData = this;
      return new Select<T>(limiteSelect, oData);
    }

    /// <summary>
    /// Asigna al objeto que pertenesca a la base de datos por default
    /// </summary>
    /// <returns>Regresa un objeto del mismo tipo que solo puede consultar en la BD por default
    /// </returns>
    public T UseDefaultBD()
    {
      object oData = this;
      bool result = iDatabase.UseDefaultBD<T>(ref oData);
      if (result)
      {
        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo property in properties)
        {
          property.SetValue(this, property.GetValue(oData));
        }
      }
      return (T)oData;
    }

    /// <summary>
    /// Recarga los datos en un objeto de la clase para recuperar datos de nuevo
    /// </summary>
    /// <returns>Regresa un objeto de la clase actualizado
    /// </returns>
    public T Reload()
    {
      return iDatabase.Reload<T>(this);
    }
    /// <summary>
    /// Evalua si es valida la clase para poder ejecutarse los metodos de BD
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool IsValidMap()
    {
      return true;
    }
    /// <summary>
    /// Evalua si todas las propiedades de la clase estan declaradas en la BD
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool VerifyFields()
    {
      return iDatabase.VerifyFields<T>();
    }
    /// <summary>
    /// Registra y sincroniza las propiedades de la clase con la BD
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool SynchronizeFields()
    {
      return iDatabase.SynchronizeFields<T>();
    }
    /// <summary>
    /// Sincroniza el modelo de la clase en la base de datos y si no lo encuentra crea el modelo
    /// </summary>
    /// <returns>Regresa un valor falso o verdadero para indicar si se realizo satisfactoriamiente
    /// </returns>
    public bool Synchronize()
    {
      return iDatabase.Synchronize<T>();
    }

    /// <summary>
    /// Obtiene todos los registros de la BD
    /// </summary>
    /// <returns>Regresa una lista de clases obtenida de la BD
    /// </returns>
    public List<T> GetAll(int level = 0, bool LoadObjAvalible = false)
    {
      return iDatabase.Get<T>("", "", level, this, LoadObjAvalible);
    }
    /// <summary>
    /// Carga listas de datos relacionados que estan instanciadas a este objeto
    /// </summary>
    /// <returns>Carga de informacion de listas a este objeto 'BelongToMany'
    /// </returns>
    public void GetList()
    {
      object oData = this;
      if (iDatabase.GetList<T>(ref oData))
      {
        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo property in properties)
        {
          property.SetValue(this, property.GetValue(oData));
        }
      };
    }

    /// <summary>
    /// Carga informacion desde otro objeto de clase que contenga las mismas propiedades
    /// </summary>
    /// <param name="oAddValues">Ingresar la clase de objeto que tenga las mismas propiedades con valores para asignarse a esta</param>
    /// <returns>Regresa este mismo objeto con la informacion precargada
    /// </returns>
    public bool AddValues(object oAddValues, params string[] propertieName)
    {
      try
      {
        List<string> ListAddValues = propertieName.ToList();
        Type mytype = typeof(T);
        if (ListAddValues == null)
        {
          foreach (PropertyInfo oProperty in mytype.GetProperties())
          {
            var oValue = oProperty.GetValue(oAddValues);
            if (oValue != null)
            {
              oProperty.SetValue(this, oValue);
            }
          }
        }
        else
        {
          foreach (string sProperty in propertieName)
          {
            PropertyInfo oProperty = mytype.GetProperty(sProperty);
            if(oProperty != null){
              var oValue = oProperty.GetValue(oAddValues);
              if (oValue != null)
              {
                oProperty.SetValue(this, oValue);
              }
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
  }




  // Clase para operaciones en Stored Procedure de la base de datos
  public class DatabaseStoredProcedure<T> : Database
  {
    public bool Execute()
    {
      object oData = this;
      bool result = iDatabase.ExecuteStoredProcedure<T>(ref oData);
      if (result)
      {
        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo property in properties)
        {
          property.SetValue(this, property.GetValue(oData));
        }
      }
      return result;
    }

    /// <summary>
    /// Carga informacion desde otro objeto de clase que contenga las mismas propiedades
    /// </summary>
    /// <param name="oAddValues">Ingresar la clase de objeto que tenga las mismas propiedades con valores para asignarse a esta</param>
    /// <returns>Regresa este mismo objeto con la informacion precargada
    /// </returns>
    public bool AddValues(object oAddValues)
    {
      try
      {
        Type type = typeof(T);
        foreach (PropertyInfo oProperty in type.GetProperties())
        {
          var oValue = oProperty.GetValue(oAddValues);
          if (oValue != null)
          {
            oProperty.SetValue(this, oValue);
          }
        }
        return true;
      }
      catch
      {
        return false;
      }
    }
  }
  public class DatabaseStoredProcedure<T, Result> : Database
  {
    public List<Result> Execute()
    {
      object oData = this;
      List<Result> result = iDatabase.ExecuteStoredProcedure<T, Result>(ref oData);
      if (oData != null)
      {
        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo property in properties)
        {
          property.SetValue(this, property.GetValue(oData));
        }
      }
      return result;
    }
  }




  // Clase para operaciones en View de la base de datos
  public class DatabaseView<T> : Database
  {
    /// <summary>
    /// Obtiene todos los registros de la BD
    /// </summary>
    /// <returns>Regresa una lista de clases obtenida de la BD
    /// </returns>
    public List<T> GetAll(int level = 0, bool LoadObjAvalible = false)
    {
      return iDatabase.Get<T>("", "", level, this, LoadObjAvalible);
    }
    /// <summary>
    /// Construye la sentencia where SQL expression lambda que a partir de las propiedades puede evaluar
    /// </summary>
    /// <param name="expression">Expression para indicar valor a evaluar</param>
    /// <returns>Regresa una Objeto WHERE que hereda metodos para comparar la propiedad indicada del constructor
    /// </returns>
    public Where<T> Where(Expression<Func<T, object>> expression)
    {
      return iDatabase.Where<T>(expression);
    }
    /// <summary>
    /// Carga listas de datos relacionados que estan instanciadas a este objeto
    /// </summary>
    /// <returns>Carga de informacion de listas a este objeto 'BelongToMany'
    /// </returns>
    public void GetList()
    {
      object oData = this;
      if (iDatabase.GetList<T>(ref oData))
      {
        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo property in properties)
        {
          property.SetValue(this, property.GetValue(oData));
        }
      };
    }
    /// <summary>
    /// Agrega un top(limit) a la consulta se usa este metodo antes del WHERE, este valor se mantiene en la interfaz hasta un GET
    /// </summary>
    /// <param name="limiteSelect">Indicar la cantidad de registros a consultar</param>
    /// <returns>Regresa un objeto del mismo tipo
    /// </returns>
    public Select<T> Limit(int limiteSelect)
    {
      return new Select<T>(limiteSelect, this);
    }

    /// <summary>
    /// Carga informacion desde otro objeto de clase que contenga las mismas propiedades
    /// </summary>
    /// <param name="oAddValues">Ingresar la clase de objeto que tenga las mismas propiedades con valores para asignarse a esta</param>
    /// <returns>Regresa este mismo objeto con la informacion precargada
    /// </returns>
    public bool AddValues(object oAddValues)
    {
      try
      {
        Type type = typeof(T);
        foreach (PropertyInfo oProperty in type.GetProperties())
        {
          var oValue = oProperty.GetValue(oAddValues);
          if (oValue != null)
          {
            oProperty.SetValue(this, oValue);
          }
        }
        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}

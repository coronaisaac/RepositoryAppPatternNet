using DaltORM.Enumerics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaltORM.PropertyClass
{
  [AttributeUsage(AttributeTargets.Property)]
  public class Field : Attribute
  {
    public string Name { get; }
    public Param[] Parameters { get; }
    public int Value1 { get; }
    public int Value2 { get; }
	public ParameterDirection Direction = ParameterDirection.Input;

	public Field(string name)
    {
      this.Name = name;
    }
	public Field(string name, ParameterDirection direction)
	{
	  this.Name = name;
	  this.Direction = direction;
	}
	public Field(string name, int value1)
    {
      this.Name = name;
      this.Value1 = value1;
    }
	public Field(string name, int value1, ParameterDirection direction)
	{
	  this.Name = name;
	  this.Value1 = value1;
	  this.Direction = direction;
	}
	public Field(string name, int value1, int value2)
    {
      this.Name = name;
      this.Value1 = value1;
      this.Value2 = value2;
    }
	public Field(string name, int value1, int value2, ParameterDirection direction)
	{
	  this.Name = name;
	  this.Value1 = value1;
	  this.Value2 = value2;
	  this.Direction = direction;
	}
	public Field(string name, params Param[] parameters)
    {
      this.Name = name;
      this.Parameters = parameters;
    }
	public Field(string name, ParameterDirection direction,  params Param[] parameters)
	{
	  this.Name = name;
	  this.Parameters = parameters;
	  this.Direction = direction;
	}
	public Field(string name, int value1, params Param[] parameters )
    {
      this.Name = name;
      this.Value1 = value1;
      this.Parameters = parameters;
    }
	public Field(string name, int value1, ParameterDirection direction,  params Param[] parameters)
	{
	  this.Name = name;
	  this.Value1 = value1;
	  this.Parameters = parameters;
	  this.Direction = direction;
	}
	public Field(string name, int value1, int value2, params Param[] parameters)
    {
      this.Name = name;
      this.Value1 = value1;
      this.Value2 = value2;
      this.Parameters = parameters;
    }
	public Field(string name, int value1, int value2, ParameterDirection direction, params Param[] parameters)
	{
	  this.Name = name;
	  this.Value1 = value1;
	  this.Value2 = value2;
	  this.Parameters = parameters;
	  this.Direction = direction;
	}

	public bool IsPrimaryKey()
    {
      if (Parameters != null)
      {
        foreach (Param oParam in Parameters)
        {
          if (oParam == Param.PrimaryKey)
          {
            return true;
          }
        }
      }
      return false;
    }

    public bool IsAutoIncrement()
    {
      if (Parameters != null)
      {
        foreach (Param oParam in Parameters)
        {
          if (oParam == Param.AutoIncrement)
          {
            return true;
          }
        }
      }
      return false;
    }

    public bool IsNotNull()
    {
      if (Parameters != null)
      {
        foreach (Param oParam in Parameters)
        {
          if (oParam == Param.NotNull)
          {
            return true;
          }
        }
      }
      return false;

    }

  }
}

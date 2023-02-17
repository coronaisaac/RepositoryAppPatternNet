using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DaltORM.Class
{
  public class Where<T> : Database
  {
    private string _Statement1;
    private string _Statement2;
    private object _oData;
    private int _limitSelect;

    public Where(string Statement1, string Statement2, object oData = null, int limitSelect = 0)
    {
      this._Statement1 = Statement1;
      this._Statement2 = Statement2;
      this._oData = oData;
      this._limitSelect = limitSelect;
    }

    public Equal<T> IsNull()
    {
      this._Statement1 += " is null ";
      this._Statement2 += " is null ";
      return new Equal<T>(this._Statement1, this._Statement2, this._oData);
    }
    public Equal<T> IsNotNull()
    {
      this._Statement1 += " is not null ";
      this._Statement2 += " is not null ";
      return new Equal<T>(this._Statement1, this._Statement2, this._oData);
    }
    public Equal<T> Eq(string oData)
    {
      this._Statement1 += " = '" + oData + "' ";
      this._Statement2 += " = '" + oData + "' ";
      return new Equal<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Equal<T> Eq(int oData)
    {
      this._Statement1 += " = " + oData.ToString() + " ";
      this._Statement2 += " = " + oData.ToString() + " ";
      return new Equal<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Equal<T> Eq(bool oData)
    {
      this._Statement1 += " =";
      this._Statement1 += oData ? " 1 " : " 0 ";
      this._Statement2 += " =";
      this._Statement2 += oData ? " 1 " : " 0 ";
      return new Equal<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Equal<T> Eq(DateTime oData)
    {
      this._Statement1 += " = '" + oData.ToString("yyyy-MM-ddThh:mm:ss") + "' ";
      this._Statement2 += " = '" + oData.ToString("yyyy-MM-ddThh:mm:ss") + "' ";
      return new Equal<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Equal<T> Eq(DateTime oData, string format)
    {
      this._Statement1 += " = '" + oData.ToString(format) + "' ";
      this._Statement2 += " = '" + oData.ToString(format) + "' ";
      return new Equal<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Equal<T> Eq(decimal oData)
    {
      this._Statement1 += " = " + oData.ToString() + " ";
      this._Statement2 += " = " + oData.ToString() + " ";
      return new Equal<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Equal<T> Eq(TimeSpan oData)
    {
      this._Statement1 += " = '" + oData.ToString() + "' ";
      this._Statement2 += " = '" + oData.ToString() + "' ";
      return new Equal<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Like<T> Like(string oData)
    {
      this._Statement1 += " LIKE '" + oData + "' ";
      this._Statement2 += " LIKE '" + oData + "' ";
      return new Like<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Between<T> Between(string data1, string data2)
    {
      this._Statement1 += " Between '" + data1 + "' and '" + data2 + "'";
      this._Statement2 += " Between '" + data1 + "' and '" + data2 + "'";
      return new Between<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Between<T> Between(int data1, int data2)
    {
      this._Statement1 += " Between " + data1.ToString() + " and " + data2.ToString() + " ";
      this._Statement2 += " Between " + data1.ToString() + " and " + data2.ToString() + " ";
      return new Between<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Between<T> Between(DateTime data1, DateTime data2)
    {
      this._Statement1 += " Between '" + data1.ToString("yyyy-MM-ddThh:mm:ss") + "' and '" + data2.ToString("yyyy-MM-ddThh:mm:ss") + "' ";
      this._Statement2 += " Between '" + data1.ToString("yyyy-MM-ddThh:mm:ss") + "' and '" + data2.ToString("yyyy-MM-ddThh:mm:ss") + "' ";
      return new Between<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Equal<T> In(params string[] values)
    {
      string arrayValues = "";
      foreach (string value in values)
      {
        arrayValues += "'" + value + "',";
      }
      arrayValues = arrayValues.Substring(0, arrayValues.Length - 1);

      this._Statement1 += " in (" + arrayValues + ") ";
      this._Statement2 += " in (" + arrayValues + ") ";

      return new Equal<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Equal<T> In(params int[] values)
    {
      string arrayValues = "";
      foreach (int value in values)
      {
        arrayValues += "" + value.ToString() + ",";
      }
      arrayValues = arrayValues.Substring(0, arrayValues.Length - 1);

      this._Statement1 += " in (" + arrayValues + ") ";
      this._Statement2 += " in (" + arrayValues + ") ";

      return new Equal<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Equal<T> In(params DateTime[] values)
    {
      string arrayValues = "";
      foreach (DateTime value in values)
      {
        arrayValues += "'" + value.ToString("yyyy-MM-ddTHH-mm-ss") + "',";
      }
      arrayValues = arrayValues.Substring(0, arrayValues.Length - 1);

      this._Statement1 += " in (" + arrayValues + ") ";
      this._Statement2 += " in (" + arrayValues + ") ";
      return new Equal<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Equal<T> In(params decimal[] values)
    {
      string arrayValues = "";
      foreach (decimal value in values)
      {
        arrayValues += "" + value.ToString() + ",";
      }
      arrayValues = arrayValues.Substring(0, arrayValues.Length - 1);

      this._Statement1 += " in (" + arrayValues + ") ";
      this._Statement2 += " in (" + arrayValues + ") ";
      return new Equal<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Where<T> Add(string oData)
    {
      this._Statement1 += " + '" + oData + "' ";
      this._Statement2 += " + '" + oData + "' ";
      return new Where<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Where<T> Add(Expression<Func<T, object>> expression)
    {
      iDatabase.Add<T>(expression, ref this._Statement1, ref this._Statement2);
      return new Where<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
 
  }
}

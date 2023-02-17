using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DaltORM.Class
{
  public class Like<T> : Database
  {
    private string _Statement1;
    private string _Statement2;
    private object _oData;
    private int _limitSelect;

    public Like(string Statement1, string Statement2, object obData = null, int limitSelect = 0)
    {
      this._Statement1 = Statement1;
      this._Statement2 = Statement2;
      this._oData = obData;
      this._limitSelect = limitSelect;

    }

    public List<T> Get(int level = 0, bool LoadObjAvalible = false)
    {
      return iDatabase.Get<T>(this._Statement1, this._Statement2, level, this._oData, LoadObjAvalible, this._limitSelect);
    }
    public Where<T> And(Expression<Func<T, object>> expression)
    {
      iDatabase.And<T>(expression, ref this._Statement1, ref this._Statement2);
      return new Where<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
    public Where<T> Or(Expression<Func<T, object>> expression)
    {
      iDatabase.Or<T>(expression, ref this._Statement1, ref this._Statement2);
      return new Where<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }

    public Where<T> AndCluster(Expression<Func<T, object>> expression)
    {
      iDatabase.AndCluster<T>(expression, ref this._Statement1, ref this._Statement2);
      return new Where<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }

    public Where<T> OrCluster(Expression<Func<T, object>> expression)
    {
      iDatabase.OrCluster<T>(expression, ref this._Statement1, ref this._Statement2);
      return new Where<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }

    public Like<T> CloseCluster()
    {
      this._Statement1 += " ) ";
      this._Statement2 += " ) ";
      return new Like<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }

    public Equal<T> AddCustomStatement(string sStatement)
    {
      this._Statement1 += sStatement;
      this._Statement2 += sStatement;
      return new Equal<T>(this._Statement1, this._Statement2, this._oData, this._limitSelect);
    }
  }
}

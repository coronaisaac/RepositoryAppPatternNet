using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DaltORM.Class
{
  public class Select<T> : Database
  {
    private int limitSelect;
    private object _oData;

    public Select(int limitSelect = 0, object oData = null)
    {
      this.limitSelect = limitSelect;
      this._oData = oData;
    }

    public Where<T> Where(Expression<Func<T, object>> expression)
    {
      return iDatabase.Where<T>(expression, this._oData, this.limitSelect);
    }


  }
}

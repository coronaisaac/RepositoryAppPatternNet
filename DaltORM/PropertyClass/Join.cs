using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaltORM.PropertyClass
{
  [AttributeUsage(AttributeTargets.Property)]
  public class Join : Attribute
  {
    string TableName;
    string ColumnID;

    public Join(string TableName, string ColumnID)
    {
      this.TableName = TableName;
      this.ColumnID = ColumnID;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaltORM.PropertyClass
{
  [AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct)]
  public class DbView : Attribute
  {
    public string Name { get; }

    public DbView(string tableName)
    {
      this.Name = tableName;
    }
  }
}

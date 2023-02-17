using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaltORM.PropertyClass
{
  [AttributeUsage(AttributeTargets.Class)]
  public class DbTable : Attribute
  {
	private string _tablename;
    public string Name { get => _tablename; }

    public DbTable(string tableName)
    {
      this._tablename = tableName;
    }
  }
}

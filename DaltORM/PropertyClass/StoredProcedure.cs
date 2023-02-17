using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaltORM.PropertyClass
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
  public class StoredProcedure : Attribute
  {
	public string Name { get; }

	public StoredProcedure(string spName)
	{
	  this.Name = spName;
	}
  }
}

using DaltORM.Enumerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaltORM.PropertyClass
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
  public class BelongsTo : Attribute
  {
    public string Name { get; }
    public string IdReference { get; }
    public TypesJoin oTypeJoin { get; }

    public BelongsTo(string name)
    {
      this.Name = name;
    }
    public BelongsTo(string name, string idreference)
    {
      this.Name = name;
      this.IdReference = idreference;
    }
    public BelongsTo(string name, string idreference, TypesJoin inTypeJoin)
    {
      this.Name = name;
      this.IdReference = idreference;
      this.oTypeJoin = inTypeJoin;
    }

    public string GetForeingKeyName()
    {
      return "FK_oClass_id_idMarca";
    }

  }
}

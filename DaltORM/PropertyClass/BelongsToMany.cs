using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaltORM.PropertyClass
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
  public class BelongsToMany : Attribute
  {
	private string _name;
	private string _idreference;
	private string _indatabase;

	public string Name { get => _name; }
	public string IdReference { get => _idreference; }
	public string InDatabase { get => _indatabase; }

	public BelongsToMany(string name)
	{
	  this._name = name;
	}
	public BelongsToMany(string name, string idreference)
	{
	  this._name = name;
	  this._idreference = idreference;
	}
	public BelongsToMany(string sName, string sIdReference, string sInDatabase)
	{
	  this._name = sName;
	  this._idreference = sIdReference;
	  this._indatabase = sInDatabase;
	}
  }
}

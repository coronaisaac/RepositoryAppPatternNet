using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaltORM.Class
{
  class PropertySQL
  {
	private int _column_id;
	private string _column_name;
	private string _sql_type;
	private int _max_length;
	private int _precision;
	private bool _PrimaryKey;
	private bool _AutoIncrement;
	private bool _NotNull;
	private string _code_type;

	public int Column_id { get => _column_id; set => _column_id = value; }
	public string Column_name { get => _column_name; set => _column_name = value; }
	public string Sql_type { get => _sql_type; set => _sql_type = value; }
	public int Max_length { get => _max_length; set => _max_length = value; }
	public int Precision { get => _precision; set => _precision = value; }
	public bool PrimaryKey { get => _PrimaryKey; set => _PrimaryKey = value; }
	public bool AutoIncrement { get => _AutoIncrement; set => _AutoIncrement = value; }
	public bool NotNull { get => _NotNull; set => _NotNull = value; }
	public string Code_type { get => _code_type; set => _code_type = value; }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaltORM.Enumerics
{
  public enum DatabaseTypes
  {
    SQLServer,
    MySQL,
    FirebirdSQL,
    NoDatabase
  }
  public enum DbItemType
  {
    Int,
    Double,
    Decimal,
    Varchar,
    Text,
    XML,
    Date,
    DateTime,
    Time,
    Money,
    BigText
  }
  public enum Param
  {
    NotNull,
    PrimaryKey,
    AutoIncrement
  }
  public enum TypesJoin
  {
    Join,
    LeftJoin,
    RightJoin,
    InnerJoin,
    LeftOuterJoin,
    RightOuterJoin
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DaltORM
{
  public class RegEx
  {
    public static bool IsTrue(string texto, string expresionRegular)
    {
      Regex rgx = new Regex(expresionRegular, RegexOptions.IgnoreCase);
      MatchCollection matches = rgx.Matches(texto);
      if(matches.Count > 0) return true;
      return false;
    }

	public static string[] Execute(string ER, string TodoTXT)
	{
	  string[] StrOut = new string[0];

	  int i = 1;
	  string pattern = ER;
	  try
	  {
		foreach (Match match in Regex.Matches(TodoTXT, pattern, RegexOptions.IgnoreCase))
		{
		  Array.Resize(ref StrOut, StrOut.Length + 1);
		  StrOut[StrOut.Length - 1] = match.Groups[1].Value;
		  i = i + 1;
		}
	  }
	  catch
	  {
		Array.Resize(ref StrOut, StrOut.Length + 1);
		StrOut[StrOut.Length - 1] = "null";
	  }
	  return StrOut;
	}
  }
}

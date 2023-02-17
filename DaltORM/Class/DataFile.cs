using System.IO;

namespace DaltORM
{
  public class DataFile
  {
    private string pathfile { get; set; }

    private DataFile() { }

    public static DataFile CreateInstanceFile(string path)
    {
      DataFile oDataFile = new DataFile();
      if(oDataFile.ValidatePath(path))
      {
        return new DataFile()
        {
          pathfile = path
        };
      }
      return null;
    }

    private bool ValidatePath(string path)
    {
      if(!string.IsNullOrEmpty(path))
      {
        return RegEx.IsTrue(path, @"^(?:[\w]\:|\\)(\\[a-z_\-\s0-9\.]+)+\.([a-z|A-Z][a-z|A-Z][a-z|A-Z]|[a-z|A-Z][a-z|A-Z][a-z|A-Z][a-z|A-Z])$");
      }
      return false;
    }

    private bool ValidateIfExist(string path)
    {
      if(File.Exists(path))
      {
        return true;
      }
      return false;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaltORM
{
  public class Answer
  {
	private bool _valid;
	private string _message;
	private int _noerror;
	private object _object;
	private DateTime _registerdate;

	public bool Valid { get => _valid; set => _valid = value; }
	public string Message { get => _message; set => _message = value; }
	public int NoError { get => _noerror; set => _noerror = value; }
	public object Object { get => _object; set => _object = value; }
	public DateTime RegisterDate { get => _registerdate; set => _registerdate = value; }

	public T GetData<T>()
    {
      return (T)this._object;
    }
    public void SetData<T>(T dataObject)
    {
      this._object = dataObject;
    }
  }
}

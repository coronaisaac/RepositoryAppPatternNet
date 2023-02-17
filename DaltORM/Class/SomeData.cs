using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DaltORM
{
  public class SomeData
  {
	private static string key = "ABCDEFGHIJKLMÑOPQRSTUVWXYZabcdefghijklmnñopqrstuvwxyz";
    private string GenerateSerieWithDate(DateTime oDate)
    {
      Random rnd = new Random();
      return (oDate.ToString("yyyyMMddHHmmss") + rnd.Next(1000, 9999).ToString()).Substring(2);
    }

    public static string SerieID(DateTime oDateTime)
    {
      return new SomeData().GenerateSerieWithDate(oDateTime);
    }

	public static string Compress(string uncompressedString)
	{
	  byte[] compressedBytes;

	  using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString)))
	  {
		using (var compressedStream = new MemoryStream())
		{
		  using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Fastest, true))
		  {
			uncompressedStream.CopyTo(compressorStream);
		  }
		  compressedBytes = compressedStream.ToArray();
		}
	  }

	  return Convert.ToBase64String(compressedBytes);
	}

	public static string Decompress(string compressedString)
	{
	  byte[] decompressedBytes;

	  var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

	  using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
	  {
		using (var decompressedStream = new MemoryStream())
		{
		  decompressorStream.CopyTo(decompressedStream);

		  decompressedBytes = decompressedStream.ToArray();
		}
	  }

	  return Encoding.UTF8.GetString(decompressedBytes);
	}

	public static string Zip(string value)
	{
	  //Transform string into byte[]  
	  byte[] byteArray = new byte[value.Length];
	  int indexBA = 0;
	  foreach (char item in value.ToCharArray())
	  {
		byteArray[indexBA++] = (byte)item;
	  }

	  //Prepare for compress
	  System.IO.MemoryStream ms = new System.IO.MemoryStream();
	  System.IO.Compression.GZipStream sw = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress);

	  //Compress
	  sw.Write(byteArray, 0, byteArray.Length);
	  //Close, DO NOT FLUSH cause bytes will go missing...
	  sw.Close();

	  //Transform byte[] zip data to string
	  byteArray = ms.ToArray();
	  System.Text.StringBuilder sB = new System.Text.StringBuilder(byteArray.Length);
	  foreach (byte item in byteArray)
	  {
		sB.Append((char)item);
	  }
	  ms.Close();
	  sw.Dispose();
	  ms.Dispose();
	  return sB.ToString();
	}

	public static string UnZip(string value)
	{
	  //Transform string into byte[]
	  byte[] byteArray = new byte[value.Length];
	  int indexBA = 0;
	  foreach (char item in value.ToCharArray())
	  {
		byteArray[indexBA++] = (byte)item;
	  }

	  //Prepare for decompress
	  System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArray);
	  System.IO.Compression.GZipStream sr = new System.IO.Compression.GZipStream(ms,
		  System.IO.Compression.CompressionMode.Decompress);

	  //Reset variable to collect uncompressed result
	  byteArray = new byte[byteArray.Length];

	  //Decompress
	  int rByte = sr.Read(byteArray, 0, byteArray.Length);

	  //Transform byte[] unzip data to string
	  System.Text.StringBuilder sB = new System.Text.StringBuilder(rByte);
	  //Read the number of bytes GZipStream red and do not a for each bytes in
	  //resultByteArray;
	  for (int i = 0; i < rByte; i++)
	  {
		sB.Append((char)byteArray[i]);
	  }
	  sr.Close();
	  ms.Close();
	  sr.Dispose();
	  ms.Dispose();
	  return sB.ToString();
	}

	public static string cifrar(string cadena)
	{
	  byte[] llave; //Arreglo donde guardaremos la llave para el cifrado 3DES.
	  byte[] arreglo = UTF8Encoding.UTF8.GetBytes(cadena); //Arreglo donde guardaremos la cadena descifrada.
														   // Ciframos utilizando el Algoritmo MD5.
	  MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
	  llave = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes("tjy2v2btbrgm274d4qhjhxk3p"));
	  md5.Clear();
	  //Ciframos utilizando el Algoritmo 3DES.
	  TripleDESCryptoServiceProvider tripledes = new TripleDESCryptoServiceProvider();
	  tripledes.Key = llave;
	  tripledes.Mode = CipherMode.ECB;
	  tripledes.Padding = PaddingMode.PKCS7;
	  ICryptoTransform convertir = tripledes.CreateEncryptor(); // Iniciamos la conversión de la cadena
	  byte[] resultado = convertir.TransformFinalBlock(arreglo, 0, arreglo.Length); //Arreglo de bytes donde guardaremos la cadena cifrada.
	  tripledes.Clear();
	  return Convert.ToBase64String(resultado, 0, resultado.Length); // Convertimos la cadena y la regresamos.
	}

	public static string descifrar(string cadena)
	{
	  byte[] llave;
	  byte[] arreglo = Convert.FromBase64String(cadena); // Arreglo donde guardaremos la cadena descovertida.
														 // Ciframos utilizando el Algoritmo MD5.
	  MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
	  llave = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes("tjy2v2btbrgm274d4qhjhxk3p"));
	  md5.Clear();
	  //Ciframos utilizando el Algoritmo 3DES.
	  TripleDESCryptoServiceProvider tripledes = new TripleDESCryptoServiceProvider();
	  tripledes.Key = llave;
	  tripledes.Mode = CipherMode.ECB;
	  tripledes.Padding = PaddingMode.PKCS7;
	  ICryptoTransform convertir = tripledes.CreateDecryptor();
	  byte[] resultado = convertir.TransformFinalBlock(arreglo, 0, arreglo.Length);
	  tripledes.Clear();
	  string cadena_descifrada = UTF8Encoding.UTF8.GetString(resultado); // Obtenemos la cadena
	  return cadena_descifrada; // Devolvemos la cadena
	}

	public static string Encriptar(string texto)
	{

	  string key = "ABCDEFGHIJKLMÑOPQRSTUVWXYZabcdefghijklmnñopqrstuvwxyz";

	  //arreglo de bytes donde guardaremos la llave
	  byte[] keyArray;
	  //arreglo de bytes donde guardaremos el texto
	  //que vamos a encriptar
	  byte[] Arreglo_a_Cifrar =
	  UTF8Encoding.UTF8.GetBytes(texto);

	  //se utilizan las clases de encriptación
	  //provistas por el Framework
	  //Algoritmo MD5
	  MD5CryptoServiceProvider hashmd5 =
	  new MD5CryptoServiceProvider();
	  //se guarda la llave para que se le realice
	  //hashing
	  keyArray = hashmd5.ComputeHash(
	  UTF8Encoding.UTF8.GetBytes(key));

	  hashmd5.Clear();

	  //Algoritmo 3DAS
	  TripleDESCryptoServiceProvider tdes =
	  new TripleDESCryptoServiceProvider();

	  tdes.Key = keyArray;
	  tdes.Mode = CipherMode.ECB;
	  tdes.Padding = PaddingMode.PKCS7;

	  //se empieza con la transformación de la cadena
	  ICryptoTransform cTransform =
	  tdes.CreateEncryptor();

	  //arreglo de bytes donde se guarda la
	  //cadena cifrada
	  byte[] ArrayResultado =
	  cTransform.TransformFinalBlock(Arreglo_a_Cifrar,
	  0, Arreglo_a_Cifrar.Length);

	  tdes.Clear();

	  //se regresa el resultado en forma de una cadena
	  return Convert.ToBase64String(ArrayResultado,
			 0, ArrayResultado.Length);
	}

	public static string Desencriptar(string textoEncriptado)
	{
	  string key = "ABCDEFGHIJKLMÑOPQRSTUVWXYZabcdefghijklmnñopqrstuvwxyz";

	  byte[] keyArray;
	  //convierte el texto en una secuencia de bytes
	  byte[] Array_a_Descifrar =
	  Convert.FromBase64String(textoEncriptado);

	  //se llama a las clases que tienen los algoritmos
	  //de encriptación se le aplica hashing
	  //algoritmo MD5
	  MD5CryptoServiceProvider hashmd5 =
	  new MD5CryptoServiceProvider();

	  keyArray = hashmd5.ComputeHash(
	  UTF8Encoding.UTF8.GetBytes(key));

	  hashmd5.Clear();

	  TripleDESCryptoServiceProvider tdes =
	  new TripleDESCryptoServiceProvider();

	  tdes.Key = keyArray;
	  tdes.Mode = CipherMode.ECB;
	  tdes.Padding = PaddingMode.PKCS7;

	  ICryptoTransform cTransform =
	   tdes.CreateDecryptor();

	  byte[] resultArray =
	  cTransform.TransformFinalBlock(Array_a_Descifrar,
	  0, Array_a_Descifrar.Length);

	  tdes.Clear();
	  //se regresa en forma de cadena
	  return UTF8Encoding.UTF8.GetString(resultArray);
	}

	public static string EncriptarMD5(string texto)
	{
	  //arreglo de bytes donde guardaremos la llave
	  byte[] keyArray;
	  //arreglo de bytes donde guardaremos el texto
	  //que vamos a encriptar
	  byte[] Arreglo_a_Cifrar =
	  UTF8Encoding.UTF8.GetBytes(texto);

	  //se utilizan las clases de encriptación
	  //provistas por el Framework
	  //Algoritmo MD5
	  MD5CryptoServiceProvider hashmd5 =
	  new MD5CryptoServiceProvider();
	  //se guarda la llave para que se le realice
	  //hashing
	  keyArray = hashmd5.ComputeHash(
	  UTF8Encoding.UTF8.GetBytes(key));

	  hashmd5.Clear();

	  //Algoritmo 3DAS
	  TripleDESCryptoServiceProvider tdes =
	  new TripleDESCryptoServiceProvider();

	  tdes.Key = keyArray;
	  tdes.Mode = CipherMode.ECB;
	  tdes.Padding = PaddingMode.PKCS7;

	  //se empieza con la transformación de la cadena
	  ICryptoTransform cTransform =
	  tdes.CreateEncryptor();

	  //arreglo de bytes donde se guarda la
	  //cadena cifrada
	  byte[] ArrayResultado =
	  cTransform.TransformFinalBlock(Arreglo_a_Cifrar,
	  0, Arreglo_a_Cifrar.Length);

	  tdes.Clear();

	  //se regresa el resultado en forma de una cadena
	  return Convert.ToBase64String(ArrayResultado,
	  0, ArrayResultado.Length);
	}

	public static string DesencriptarMD5(string textoEncriptado)
	{
	  byte[] keyArray;
	  //convierte el texto en una secuencia de bytes
	  byte[] Array_a_Descifrar =
	  Convert.FromBase64String(textoEncriptado);

	  //se llama a las clases que tienen los algoritmos
	  //de encriptación se le aplica hashing
	  //algoritmo MD5
	  MD5CryptoServiceProvider hashmd5 =
	  new MD5CryptoServiceProvider();

	  keyArray = hashmd5.ComputeHash(
	  UTF8Encoding.UTF8.GetBytes(key));

	  hashmd5.Clear();

	  TripleDESCryptoServiceProvider tdes =
	  new TripleDESCryptoServiceProvider();

	  tdes.Key = keyArray;
	  tdes.Mode = CipherMode.ECB;
	  tdes.Padding = PaddingMode.PKCS7;

	  ICryptoTransform cTransform =
	  tdes.CreateDecryptor();

	  byte[] resultArray =
	  cTransform.TransformFinalBlock(Array_a_Descifrar,
	  0, Array_a_Descifrar.Length);

	  tdes.Clear();
	  //se regresa en forma de cadena
	  return UTF8Encoding.UTF8.GetString(resultArray);
	}

	public static string GetMD5(string str)
	{
	  MD5 md5 = MD5CryptoServiceProvider.Create();
	  ASCIIEncoding encoding = new ASCIIEncoding();
	  byte[] stream = null;
	  StringBuilder sb = new StringBuilder();
	  stream = md5.ComputeHash(encoding.GetBytes(str));
	  for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
	  return sb.ToString();
	}

	public static string GetHexadecimal(int number)
	{
	  return "";
	}

  }
}

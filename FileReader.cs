class FileReader 
{
  public static string ReadFile(string path)
  {
      using (var reader = new StreamReader(path))
      {
          return reader.ReadToEnd();
      }
  }
}

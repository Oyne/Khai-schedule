namespace Khai;

using System.Text.Json;
public class FileWork
{
    Exception FileWasNotFound;

    private const string _directoryPath = "C://Khai";
    private const string _infoFilePath = "C://Khai/info.json";
   
    private static int CheckIsFileExist()
    {
        if (!Directory.Exists(_directoryPath))
            return -1;
            //throw new Exception("Directory is not existing");

        if (!File.Exists(_infoFilePath))
            return -2;
            //throw new Exception("File is not existing");

        return 0;
    }

    public static void CreateAFile()
    {
        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        if (!File.Exists(_infoFilePath))
        {
            var SchFile = File.Create(_infoFilePath);
            SchFile.Close();
        }         
    }

    public static void SaveSchduleToFile(WeekSchedule weekSchedule)
    {

            if (CheckIsFileExist() == 0)
            {
                var json = JsonSerializer.Serialize(weekSchedule);
                File.WriteAllText(_infoFilePath, json);
            }
            else if(CheckIsFileExist() == -1) throw new DirectoryDoesNotExistException("File is not existing");
            else throw new FileDoesNotExistException("Directory is not existing");
        
    }

    public static void CleanFile()
    {
        try
        {
            CheckIsFileExist();
            File.WriteAllText(_infoFilePath, "");
        } catch {}
    }
    public static WeekSchedule ReadScheduleFromFile()
    {
        WeekSchedule readed;
        try
        {
            CheckIsFileExist();
            var rowJSON = File.ReadAllText(_infoFilePath);

            if (rowJSON.Length == 0)
                throw new ScheduleWasNotFoundException("Немає збереженого розкладу.");

             readed = JsonSerializer.Deserialize<Khai.WeekSchedule>(rowJSON);
        } catch {
            readed = null;
        }

        if (readed == null)
            throw new FileWasNotFoundException("Не вдалося прочитати файл.");

        return readed;
    }
}

 public class FileWasNotFoundException : Exception
{
    public FileWasNotFoundException(string message)
        : base(message) { }
}

public class ScheduleWasNotFoundException : Exception
{
    public ScheduleWasNotFoundException(string message)
        : base(message) { }
}

public class DirectoryDoesNotExistException : Exception
{
    public DirectoryDoesNotExistException(string message)
        : base(message) { }
}

public class FileDoesNotExistException : Exception
{
    public FileDoesNotExistException(string message)
        : base(message) { }
}
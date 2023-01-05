namespace Khai;

using System.Text.Json;
public class FileWork
{
    private const string _directoryPath = "C://Khai";
    private const string _infoFilePath = "C://Khai/info.json";
    private const string _settingsFilePath = "C://Khai/settings.json";

    private static int CheckIsFileExist(string filePath)
    {
        if (!Directory.Exists(_directoryPath))
            return -1;
            //throw new Exception("Directory is not existing");

        if (!File.Exists(filePath))
            return -2;
            //throw new Exception("File is not existing");

        return 0;
    }

    /// <summary>
    /// Method for creating a file
    /// </summary>
    public static void CreateAFile(string file_name)
    {
        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        if (!File.Exists(file_name))
        {
            var file = File.Create(file_name);
            file.Close();
        }         
    }

    public static void SaveSchduleToFile(WeekSchedule weekSchedule)
    {
            if (CheckIsFileExist(_infoFilePath) == 0)
            {
                var json = JsonSerializer.Serialize(weekSchedule);
                File.WriteAllText(_infoFilePath, json);
            }
            else if(CheckIsFileExist(_infoFilePath) == -1) throw new DirectoryDoesNotExistException("Directory is not existing");
            else throw new FileDoesNotExistException("File is not existing");      
    }

    public static void CleanFile()
    {
        try
        {
            CheckIsFileExist(_infoFilePath);
            File.WriteAllText(_infoFilePath, "");
        } catch {}
    }
    public static WeekSchedule ReadScheduleFromFile()
    {
        WeekSchedule readed;
        try
        {
            CheckIsFileExist(_infoFilePath);
            var rowJSON = File.ReadAllText(_infoFilePath);

            if (rowJSON.Length == 0)
                throw new ScheduleWasNotFoundException("Немає збереженого розкладу.");

             readed = JsonSerializer.Deserialize<Khai.WeekSchedule>(rowJSON);
        }
        catch (ScheduleWasNotFoundException ex)
        {
            throw new ScheduleWasNotFoundException("Немає збереженого розкладу.");
        }
        catch
        {
            readed = null;
        }

        if (readed == null)
            throw new FileWasNotFoundException("Не вдалося прочитати файл.");

        return readed;
    }

    public static Settings ReadSettingsFromFile()
    {
        Settings readed;
        try
        {
            CheckIsFileExist(_settingsFilePath);
            var rowJSON = File.ReadAllText(_settingsFilePath);

            if (rowJSON.Length == 0)
                throw new SettingsWasNotFoundException("Нет сохранённых настроек.");

            readed = JsonSerializer.Deserialize<Settings>(rowJSON);
        }
        catch
        {
            readed = null;
        }

        if (readed == null)
            throw new FileWasNotFoundException("Не удалось прочитать файл.");

        return readed;
    }

    public static void SaveSettings(Settings settings)
    {
        if (CheckIsFileExist(_settingsFilePath) == 0)
        {
            var json = JsonSerializer.Serialize(settings);
            File.WriteAllText(_settingsFilePath, json);
        }
        else
        {
            CreateAFile("C://Khai/settings.json");
            var json = JsonSerializer.Serialize(settings);
            File.WriteAllText(_settingsFilePath, json);
        }
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

public class SettingsWasNotFoundException : Exception
{
    public SettingsWasNotFoundException(string message)
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
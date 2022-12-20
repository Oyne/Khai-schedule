namespace Khai;

using System.Text.Json;
public class FileWork
{
    private const string _directoryPath = "C://Khai";
    private const string _infoFilePath = "C://Khai/ifo.json";
   
    private static void CheckIsFileExist()
    {
        if (!Directory.Exists(_directoryPath))
            throw new Exception("Directory is not existing");

        if (!File.Exists(_infoFilePath))
            throw new Exception("File is not existing");
    }

    public static void CreateAFile()
    {
        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);
            
        if(!File.Exists(_infoFilePath))
            File.Create(_infoFilePath);
    }

    public static void SaveSchduleToFile(WeekSchedule weekSchedule)
    {
        try
        {
            CheckIsFileExist();
            var json = JsonSerializer.Serialize(weekSchedule);
            File.WriteAllText(_infoFilePath, json);
        } catch { }
        
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
                throw new Exception("Немає збереженого розкладу.");

             readed = JsonSerializer.Deserialize<Khai.WeekSchedule>(rowJSON);
        } catch {
            readed = null;
        }

        if (readed == null)
            throw new Exception("Не вдалося прочитати файл.");

        return readed;
    }
}

namespace Khai;

using System.Text.Json;
public class FileWork
{
    private const string _directoryPath = "C://Khai";
    private const string _infoFilePath = "C://Khai/info.json";

    public static void CreateAFile()
    {
        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);
            
        if(!File.Exists(_infoFilePath))
            File.Create(_infoFilePath);
    }

    public static void SaveSchduleToFile(WeekSchedule weekSchedule)
    {
        var json = JsonSerializer.Serialize(weekSchedule);
        File.WriteAllText(_infoFilePath, json);
    }

    public static WeekSchedule ReadScheduleFromFile()
    {
        var rowJSON = File.ReadAllText(_infoFilePath);
        var readed = JsonSerializer.Deserialize<Khai.WeekSchedule>(rowJSON);

        if (readed == null)
            throw new Exception("Не удалось прочитать данные из файла");

        return readed;
    }
}

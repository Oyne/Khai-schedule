using System;

namespace Khai;

public class UniversityClass
{
    private string _name = string.Empty;

    public string Name
    {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(value));
    }
    public string? RoomNumber { get; set; }
    public string? Type { get; set; }
    public string? Teacher { get; set; }

    public UniversityClass() { }

    public UniversityClass(string name)
    {
        _name = name;
    }

    public UniversityClass(string name, string? roomNumber, string? type, string? teacher) : this(name)
    {
        RoomNumber = roomNumber;
        Teacher = teacher;
        Type = type;

    }
}
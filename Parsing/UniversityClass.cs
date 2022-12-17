using System;

namespace Khai;

public class UniversityClass
{
    public string Name { get; }
    public string? RoomNumber { get; }
    public string? Type { get; }
    public string? Teacher { get; }

    public UniversityClass(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public UniversityClass(string name, string? roomNumber, string? type, string? teacher) : this(name)
    {
        if (roomNumber is not null && string.IsNullOrWhiteSpace(roomNumber))
            throw new ArgumentException("Room number is empty string.");

        RoomNumber = roomNumber;
        Type = type;
        Teacher = teacher;
    }
}
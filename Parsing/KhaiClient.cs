﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Khai;

public class KhaiClient : IDisposable
{
    private const string BaseUrl = "https://education.khai.edu/";

    private readonly HttpClient _httpClient;

    public KhaiClient()
    {
        _httpClient = new HttpClient()
        {
            BaseAddress = new(BaseUrl)
        };
    }

    public async Task<WeekSchedule> GetStudentWeekSheduleAsync(string studentId)
    {
        ArgumentNullException.ThrowIfNull(studentId);

        if (string.IsNullOrWhiteSpace(studentId))
            throw new ArgumentException("A student ID is an empty string.", nameof(studentId));

        var response      = (await _httpClient.GetAsync(@$"union/schedule/student/{studentId}")).EnsureSuccessStatusCode();
        var contentStream = await response.Content.ReadAsStreamAsync();
        
        return ParseWeekSchedule(contentStream);
    }

    public async Task<WeekSchedule> GetLecturerWeekSheduleAsync(string lecturerId)
    {
        ArgumentNullException.ThrowIfNull(lecturerId);

        if (string.IsNullOrWhiteSpace(lecturerId))
            throw new ArgumentException("A lecturer ID is an empty string.", nameof(lecturerId));

        var response      = (await _httpClient.GetAsync(@$"union/schedule/lecturer/{lecturerId}")).EnsureSuccessStatusCode();
        var contentStream = await response.Content.ReadAsStreamAsync();

        return ParseWeekSchedule(contentStream);
    }

    public async Task<WeekSchedule> GetGroupWeekSheduleAsync(string groupId)
    {
        ArgumentNullException.ThrowIfNull(groupId);

        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("A group ID is an empty string.", nameof(groupId));

        var response      = (await _httpClient.GetAsync(@$"union/schedule/group/{groupId}")).EnsureSuccessStatusCode();
        var contentStream = await response.Content.ReadAsStreamAsync();

        return ParseWeekSchedule(contentStream);
    }

    public void Dispose() => _httpClient.Dispose();

    private WeekSchedule ParseWeekSchedule(Stream contentStream)
    {
        var html = new HtmlDocument();
        html.Load(contentStream);

        var scheduleTable = html.DocumentNode.SelectSingleNode("//div[@class='mud-table-container']/table");

        return ParseWeekSchedule(scheduleTable);
    }

    private WeekSchedule ParseWeekSchedule(HtmlNode scheduleTable)
    {
        var scheduleRows = scheduleTable.SelectNodes("tr");

        var headerRows = scheduleRows
            .Where(row =>
                row.FirstChild.Name == "th" &&
                row.FirstChild.Attributes.Contains("colspan") &&
                row.FirstChild.Attributes["colspan"].Value == "3")
            .ToArray();

        var rowsByDay    = scheduleRows.SplitBy(headerRows);
        var daySchedules = new List<DaySchedule>();

        foreach (var rows in rowsByDay)
        {
            var dayClasses = new List<AlternateUniversityClass>();

            for (int i = 0; i < rows.Length; i++)
            {
                var numeratorClassCells = rows[i].Descendants("td").ToArray();
                var numeratorClass      = ParseUniversityClass(numeratorClassCells[1].GetDirectInnerText());
                
                UniversityClass? denominatorClass;

                if (numeratorClassCells[0].Attributes.Contains("rowspan"))
                {
                    var denominatorClassCells = rows[i + 1].Descendants("th");
                    denominatorClass = ParseUniversityClass(denominatorClassCells.First().GetDirectInnerText());

                    i++;
                }
                else
                {
                    denominatorClass = numeratorClass;
                }

                dayClasses.Add(new AlternateUniversityClass(numeratorClass, denominatorClass));
            }

            daySchedules.Add(new DaySchedule(dayClasses));
        }

        return WeekSchedule.Parse(daySchedules);
    }

    private UniversityClass? ParseUniversityClass(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        var items = text.Split(',');
        string name;
        string? roomNumber;
        string? type;
        string? teacher;

        if (char.IsDigit(items[0][0]))
        {
            //temporary crutch
            if (items.Length < 4)
            {
                teacher = null;
                type = items[2]?.Trim();
                name = items[1].Trim();
                roomNumber = items[0];
            }
            else
            {
                teacher = items[3]?.Trim();
                type = items[2]?.Trim();
                name = items[1].Trim();
                roomNumber = items[0];
            }
            //original
            //teacher = items[3]?.Trim();
            //type = items[2]?.Trim();
            //name = items[1].Trim();
            //roomNumber = items[0];
        }
        else
        {
            name       = items[0];
            roomNumber = null;
            teacher = null;
            type = null;
        }

        return new UniversityClass(name, roomNumber, type, teacher);
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

public class Student
{
    public string Name { get; set; }
    public string Group { get; set; }
    public DateTime DateOfBirth { get; set; }
    public decimal AverageGrade { get; set; }

    public override string ToString()
    {
        return $"{Name}, {DateOfBirth.ToString("dd.MM.yyyy")}, {AverageGrade:F2}";
    }
}

class Program
{
    static void Main(string[] args)
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string studentsDirectory = Path.Combine(desktopPath, "Students");

        if (!Directory.Exists(studentsDirectory))
        {
            Directory.CreateDirectory(studentsDirectory);
        }

        string binaryFilePath = "C:\\Users\\kyotzu\\Desktop\\students.dat";

        List<Student> students = ReadStudentsFromBinaryFile(binaryFilePath);

        WriteStudentsByGroup(students, studentsDirectory);

        Console.WriteLine("Данные успешно обработаны и сохранены по группам.");
    }

    static List<Student> ReadStudentsFromBinaryFile(string filePath)
    {
        List<Student> students = new List<Student>();

        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            try
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    string name = reader.ReadString();
                    string group = reader.ReadString();
                    DateTime dateOfBirth = DateTime.FromBinary(reader.ReadInt64());
                    decimal averageGrade = reader.ReadDecimal();

                    students.Add(new Student
                    {
                        Name = name,
                        Group = group,
                        DateOfBirth = dateOfBirth,
                        AverageGrade = averageGrade
                    });
                }
            }
            catch (EndOfStreamException)
            {
                Console.WriteLine("Произошла ошибка");
            }
        }

        return students;
    }

    static void WriteStudentsByGroup(List<Student> students, string directoryPath)
    {
        var groupedStudents = new Dictionary<string, List<Student>>();

        foreach (var student in students)
        {
            if (!groupedStudents.ContainsKey(student.Group))
            {
                groupedStudents[student.Group] = new List<Student>();
            }
            groupedStudents[student.Group].Add(student);
        }

        foreach (var group in groupedStudents)
        {
            string groupFilePath = Path.Combine(directoryPath, $"{group.Key}.txt");

            using (StreamWriter writer = new StreamWriter(groupFilePath))
            {
                foreach (var student in group.Value)
                {
                    writer.WriteLine(student.ToString());
                }
            }
        }
    }
}

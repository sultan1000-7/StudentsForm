using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;

namespace sultan
{
    public class Program()
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html; charset=utf-8";

                // ���� ��������� ���� �� ������ "/postuser", �������� ������ �����
                if (context.Request.Path == "/postuser")
                {
                    var form = context.Request.Form;
                    string? fullName = form["full_name"];
                    int age = int.Parse(form["age"]);
                    string? faculty = form["faculty"];
                    string? direction = form["direction"];
                    int course = int.Parse(form["course"]);

                    using (ConnectionDB db = new ConnectionDB())
                    {
                        // ������� ��� ������� User
                        Student student = new Student(fullName, age, faculty, direction, course);

                        // ��������� �� � ��
                        db.Students.AddRange(student);
                        db.SaveChanges();
                    }
                    using (ConnectionDB db = new ConnectionDB())
                    {
                        // �������� ������� �� �� � ������� �� �������
                        var students = db.Students.ToList();
                        Console.WriteLine("Users list:");
                        foreach (Student student in students)
                        {
                            await context.Response.WriteAsync($"<div><p>���: {student.FullName}</p><p>�������: {student.Age}</p><p>���������: {student.Faculty}</p><p>�����������: {student.Direction}</p><p>����: {student.Course}</p></div>");
                        }
                    }
                }
                else
                {
                    await context.Response.SendFileAsync("Properties/templates/index.html");
                }
            });


            app.Run();
        }
    }

    public class Student
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public int Age { get; set; }
        public string? Faculty { get; set; }
        public string? Direction { get; set; }
        public int Course { get; set; }

        public Student(string FullName, int Age, string Faculty, string Direction, int Course)
        {
            this.FullName = FullName;
            this.Age = Age;
            this.Faculty = Faculty;
            this.Course = Course;
            this.Direction = Direction;
        }

        
    }

    public class ConnectionDB : DbContext
    {
        public DbSet<Student> Students { get; set; } = null!;

        public ConnectionDB()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=usersdb;Username=postgres;Password=12345678");
        }
    }
}
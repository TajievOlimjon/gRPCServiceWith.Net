
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using GrpcClientApp.Protos;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await AddStudent(true);

        Console.ReadKey();
    }
     static async Task GetTranslator()
     {
         var words = new List<string>() { "red", "yellow", "green" };

         using var channel = GrpcChannel.ForAddress("https://localhost:7035");

         var client = new Translator.TranslatorClient(channel);

         foreach (var word in words)
         {
             var request = new Request { Word = word };
             request.Word = word;

             var response = await client.TranslateAsync(request);

             Console.WriteLine(response.Word + " : " + response.Translation);
         }
     }

    static async Task GetStudents()
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7035");

        var client = new Student.StudentClient(channel);

        var students = await client.GetStudentInfoAsync(new GetStudentInfoRequest());

        foreach (var student in students.Students)
        {
           Console.WriteLine($"FirstName: {student.FirstName}");
           Console.WriteLine($"LastName: {student.LastName}");
           Console.WriteLine($"Email: {student.Email}"); 
           Console.WriteLine($"Age: {student.Age}");
           Console.WriteLine($"BirthDate: {student.BirthDate}");
        }
    }
    static async Task AddStudent(bool addStudent)
     {
         using var channel = GrpcChannel.ForAddress("https://localhost:7035");

         var client = new Student.StudentClient(channel);

         while (addStudent)
         {
             var student = new RequestInfo();
             Console.Write("FirstName: ");
             student.FirstName = Console.ReadLine();
             Console.Write("LastName: ");
             student.LastName = Console.ReadLine();
             Console.Write("Email: ");
             student.Email = Console.ReadLine();
             Console.Write("Age: ");
             student.Age = Convert.ToInt32(Console.ReadLine());
             Console.Write("BirthDate: ");
             var date = Convert.ToDateTime(Console.ReadLine());
             student.BirthDate = Timestamp.FromDateTimeOffset(DateTime.SpecifyKind(date, DateTimeKind.Utc));

             var response = await client.AddStudentAsync(student);

             Console.WriteLine("Yes  ||  No ");
             string command = Console.ReadLine();
             if (response.Response == false || command.ToLower() == "No".ToLower())
             {
                 addStudent = false;
             }
             else
             {
                addStudent = true;
             }
         }
       await GetStudents();
     }
}
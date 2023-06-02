using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcClientApp.Protos;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await GetUserServices();

        Console.ReadKey();
    }
    static async Task GetAllUsers(UserServie.UserServieClient client)
    {
        var users = await client.ListUsersAsync(new Empty());

        Console.WriteLine(new string('-', 120));
        Console.WriteLine("Get all users");
        Console.WriteLine(new string('-', 120));
        foreach (var user in users.Users)
        {
            await Console.Out.WriteLineAsync($"Id: {user.Id}  |  Name: {user.Name}  |  Age: {user.Age}");
        }
    }
    static async Task CreateUser(UserServie.UserServieClient client)
    {
        Console.WriteLine(new string('-', 120));
        Console.WriteLine(new string('-', 120));
        Console.WriteLine("Created new user");
        Console.WriteLine(new string('-', 120));

        Console.Write("Enter name: ");
        string? name = Console.ReadLine();
        Console.Write("Enter age: ");
        int age = Convert.ToInt32(Console.ReadLine());

        var response = await client.CreateUserAsync(new CreateUserRequest { Name = name, Age = age });
        await Console.Out.WriteLineAsync($"Id: {response.Id}  |  Name: {response.Name}  |  Age: {response.Age}");
    }
    static async Task GetUserById(UserServie.UserServieClient client)
    {
        Console.WriteLine(new string('-', 120));
        Console.WriteLine(new string('-', 120));
        Console.WriteLine("get user by id");
        Console.WriteLine(new string('-', 120));

        Console.Write("Enter user id, to get user data | Id= ");
        int id = Convert.ToInt32(Console.ReadLine());

        var userById = await client.GetUserAsync(new GetUserRequest { Id = id });
        await Console.Out.WriteLineAsync($"Id: {userById.Id}  |  Name: {userById.Name}  |  Age: {userById.Age}");
    }
    static async Task UpdateUser(UserServie.UserServieClient client)
    {
        Console.WriteLine(new string('-', 120));
        Console.WriteLine(new string('-', 120));
        Console.WriteLine("Updated user");
        Console.WriteLine(new string('-', 120));

        Console.Write("Enter user id | Id= ");
        int id = Convert.ToInt32(Console.ReadLine());
        Console.Write("Enter name: ");
        string? name = Console.ReadLine();
        Console.Write("Enter age: ");
        int age = Convert.ToInt32(Console.ReadLine());

        var response = await client.UpdateUserAsync(new UpdateUserRequest { Id = id, Name = name, Age = age });
        await Console.Out.WriteLineAsync($"Id: {response.Id}  |  Name: {response.Name}  |  Age: {response.Age}");
    }
    static async Task DeleteUser(UserServie.UserServieClient client)
    {
        Console.WriteLine(new string('-', 120));
        Console.WriteLine(new string('-', 120));
        Console.WriteLine("Delete user");
        Console.WriteLine(new string('-', 120));

        Console.Write("Enter user id | Id= ");
        int id = Convert.ToInt32(Console.ReadLine());
        var response = await client.DeleteUserAsync(new DeleteUserRequest { Id = id });
        await Console.Out.WriteLineAsync($"Id: {response.Id}  |  Name: {response.Name}  |  Age: {response.Age}");
    }
    static async Task GetUserServices()
    {
        try
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:7035");

            var client = new UserServie.UserServieClient(channel);
            bool worker = true;
            while (worker)
            {
                Console.WriteLine("get all users :"+" GetAll");
                Console.WriteLine("Get user by id: "+"GetById");
                Console.WriteLine("Create new user: "+"Create");
                Console.WriteLine("Update user: " + "Update");
                Console.WriteLine("Delete user: " + "Delete");
                Console.Write("Enter command: ");
                string? command = Console.ReadLine();
                if (command.ToLower() == "GetAll".ToLower())
                {
                    await GetAllUsers(client);
                    Console.WriteLine();
                }
                if (command.ToLower() == "GetById".ToLower())
                {
                    await GetUserById(client);
                    Console.WriteLine();
                }
                if (command.ToLower() == "Create".ToLower())
                {
                    await CreateUser(client);
                    await GetAllUsers(client);
                    Console.WriteLine();
                }
                if (command.ToLower() == "Delete".ToLower())
                {
                    await DeleteUser(client);
                    await GetAllUsers(client);
                    Console.WriteLine();
                }
                if (command.ToLower() == "Update".ToLower())
                {
                    await UpdateUser(client);
                    await GetAllUsers(client);
                    Console.WriteLine();
                }

                Console.Write("Do you want to continue working ?");
                Console.Write(" Yes or no :  ");
                command = Console.ReadLine();
                if (command.ToLower() == "no")
                {
                    worker = false;
                }
                Console.WriteLine();
                Console.WriteLine();
            }

            Console.ReadKey();
        }
        catch (RpcException exception)
        {
            await Console.Out.WriteLineAsync(exception.Status.StatusCode+" : "+exception.Status.Detail);
        }
    }
    static async Task MessengerService()
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7035");

        var client = new GrpcServiceAppMessenger.Protos.Messenger.MessengerClient(channel);

        Metadata requestHeaders = new Metadata();

        requestHeaders.Add(key: "userName", value: "Olimjon");

        using var call = client.SendMessageAsync(request: new GrpcServiceAppMessenger.Protos.MessengerRequest(),headers:requestHeaders);

        var response = await call.ResponseAsync;
        await Console.Out.WriteLineAsync("Response: "+response.Content);

        Metadata headers = await call.ResponseHeadersAsync;

        foreach (var header in headers)
        {
            await Console.Out.WriteLineAsync(header.Key +" : "+ header.Value);
        }

        Console.ReadKey();
    }
    static async Task ClientServerMessageService()
    {
        try
        {
            string[] messages = { "Привет", "Как дела?", "Че молчишь?", "Ты че, спишь?", "Ну пока" };

            using var channel = GrpcChannel.ForAddress("https://localhost:7035");

            var client = new ClientServerMessage.ClientServerMessageClient(channel);

            var call = client.ClientServerDataStream();

            var readTask = Task.Run(async () =>
            {
                await foreach (var item in call.ResponseStream.ReadAllAsync())
                {
                    await Console.Out.WriteLineAsync("Server: " + item.Content);
                }
            });

            foreach (var message in messages)
            {
                await call.RequestStream.WriteAsync(new ClientServerRequestMessage { Content = message });
                Console.WriteLine(message);
                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            await call.RequestStream.CompleteAsync();
            await readTask;
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync(ex.Message.ToString());
        }
       
    }
    static async Task ClientMessageToServerService()
    {
        string[] messages = { "Привет", "Как дела?", "Че молчишь?", "Ты че, спишь?", "Ну пока" };

        using var channel = GrpcChannel.ForAddress("https://localhost:7035");

        var client = new ClientMessageToServer.ClientMessageToServerClient(channel);

        var call = client.ClientDataStream();

        foreach (var message in messages)
        {
            await call.RequestStream.WriteAsync(new ClientRequestMessage { Content = message });
        }

        await call.RequestStream.CompleteAsync();

        ServerResponseMessage response = await call.ResponseAsync;

        await Console.Out.WriteLineAsync(response.Content);
    }
    static async Task ServerMessageToClientService()
    {
        //Потоковая передача с сервера
        try
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:7035");
            var client = new Messenger.MessengerClient(channel);

            var serverData = client.ServerDataStream(new RequestMessage());

            var responseStream = serverData.ResponseStream;
            while (await responseStream.MoveNext(new CancellationToken()))
            {

                ResponseMessage response = responseStream.Current;

                Console.WriteLine(response.Content);
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor=ConsoleColor.Red;
            await Console.Out.WriteLineAsync(ex.Message.ToString());
        }
       
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
using Grpc.Core;
using GrpcServiceApp.Protos;

namespace GrpcServiceApp.Services
{
    public class ClientServerMessengerService : ClientServerMessage.ClientServerMessageBase
    {
        public override async Task ClientServerDataStream(IAsyncStreamReader<ClientServerRequestMessage> requestStream, IServerStreamWriter<ClientServerResponseMessage> responseStream, ServerCallContext context)
        {
            string[] messages = { "Привет", "Норм", "...", "Нет", "пока" };

            var readTask = Task.Run(async () =>
            {
                await foreach (var requestMessage in requestStream.ReadAllAsync())
                {
                    await Console.Out.WriteLineAsync("Client: "+requestMessage.Content);
                }
            });

            foreach (var message in messages)
            {
                if (!readTask.IsCompleted)
                {
                    await responseStream.WriteAsync(new ClientServerResponseMessage { Content = message });
                    Console.WriteLine(message);
                    await Task.Delay(2000);
                }
            }
            await readTask;
        }
    }
}

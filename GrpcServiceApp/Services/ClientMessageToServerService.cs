using Grpc.Core;
using GrpcServiceApp.Protos;

namespace GrpcServiceApp.Services
{
    public class ClientMessageToServerService : ClientMessageToServer.ClientMessageToServerBase
    {
        public override async Task<ServerResponseMessage> ClientDataStream(IAsyncStreamReader<ClientRequestMessage> requestStream, ServerCallContext context)
        {
            await foreach (var request in requestStream.ReadAllAsync())
            {
                await Console.Out.WriteLineAsync(request.Content);
            }
            Console.WriteLine("Все данные получены...");

            return new ServerResponseMessage { Content = "все данные получены" };
        }
    }
}

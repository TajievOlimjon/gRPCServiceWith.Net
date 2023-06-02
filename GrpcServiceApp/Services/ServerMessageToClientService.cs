using Grpc.Core;
using GrpcServiceApp.Protos;
//Потоковая передача сервера
namespace GrpcServiceApp.Services
{
    public class ServerMessageToClientService : Messenger.MessengerBase
    {
        string[] messages = { "Привет", "Как дела?", "Че молчишь?", "Ты че, спишь?", "Ну пока" };
        public override async Task ServerDataStream(RequestMessage request, IServerStreamWriter<ResponseMessage> responseStream, ServerCallContext context)
        {
            foreach (var message in messages)
            {
                await responseStream.WriteAsync(new ResponseMessage { Content=message});

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}

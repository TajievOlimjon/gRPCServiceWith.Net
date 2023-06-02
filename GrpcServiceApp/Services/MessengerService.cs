using Grpc.Core;
using GrpcServiceAppMessenger.Protos;

namespace GrpcServiceApp.Services
{
    public class MessengerService : Messenger.MessengerBase
    {
        public override async Task<MessengerResponse> SendMessage(MessengerRequest request, ServerCallContext context)
        {
            foreach (var header in context.RequestHeaders)
            {
                Console.WriteLine(header.Key +" : "+header.Value);
            }
            var userName = context.RequestHeaders.GetValue("userName");

            Metadata responseHeaders = new Metadata();
            responseHeaders.Add("secret-code", "123445");

            await context.WriteResponseHeadersAsync(responseHeaders);

            return await Task.FromResult(new MessengerResponse
            {
                Content="Hello "+userName
            });
        }
    }
}

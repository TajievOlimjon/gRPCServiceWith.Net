using Grpc.Core;
using GrpcServiceApp.Protos;

namespace GrpcServiceApp.Services;

public class TranslatorService : Translator.TranslatorBase
{
    Dictionary<string, string> words = new() { { "red", "красный" }, { "green", "зеленый" }, { "blue", "синий" } };
    public override Task<Response> Translate(Request request, ServerCallContext context)
    {
        Console.WriteLine("Запрошено слово: " + request.Word);
        if (!words.TryGetValue(request.Word, out var translation))
        {
            translation = "Not found !";
        }

        return Task.FromResult(new Response
        {
            Word = request.Word,
            Translation = translation
        });
    }
}

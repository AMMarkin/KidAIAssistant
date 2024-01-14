using TheOneWhoWritesYourSpeech;
using TheOneWhoWritesYourSpeech.Providers;
using TheOneWhoWritesYourSpeech.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddSingleton<SpeechService>();
builder.Services.AddSingleton<GptService>();
builder.Services.AddSingleton<FileService>();
builder.Services.AddSingleton<ImageService>();

builder.Services.AddSingleton<OpenAiDataProvider>(f => new()
{
    ApiKey = builder.Configuration["GPT:ApiKey"]!
});
builder.Services.AddSingleton<BotDataProvider>(f => new()
{
    Token = builder.Configuration["BotData:TokenApi"]!,
    EnabledImages = builder.Configuration.GetValue<bool>("BotData:EnabledImages")
});

builder.Services.AddSingleton<Bot>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapGet("/", () => "Hello World!");

#region Startup Task

Console.WriteLine(app.Services.GetService<Bot>().Start());

#endregion Startup Task

app.Run();

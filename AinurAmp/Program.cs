using AinurAmp.Components;

namespace AinurAmp;

internal class Program
{
	private static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		builder.Services.AddRazorComponents()
			.AddInteractiveServerComponents()
			.AddInteractiveWebAssemblyComponents();
		WebApplication app = builder.Build();
		app.MapGet("/api/audio/{trackId}", (string trackId, HttpContext context) =>
		{
			string flacPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Music", $"{trackId}");
			if (!File.Exists(flacPath))
				return Results.NotFound();
			FileStream fileStream = File.OpenRead(flacPath);
			context.Response.Headers.AcceptRanges = "bytes";
			return Results.Stream(fileStream, "audio/flac", enableRangeProcessing: true);
		});
		if (app.Environment.IsDevelopment())
			app.UseWebAssemblyDebugging();
		else
		{
			app.UseExceptionHandler("/Error", createScopeForErrors: true);
			app.UseHsts();
		}
		app.UseHttpsRedirection();
		app.UseAntiforgery();
		app.MapStaticAssets();
		app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode()
			.AddInteractiveWebAssemblyRenderMode()
			.AddAdditionalAssemblies(typeof(AinurAmp.Client._Imports).Assembly);
		app.Run();
	}
}

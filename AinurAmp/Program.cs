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

		// Register HttpClient for server-side components
		builder.Services.AddScoped(sp =>
		{
			var navigationManager = sp.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
			return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
		});

		WebApplication app = builder.Build();

		string musicRootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Music"));

		// Validate that a path is within the music directory
		static bool IsPathSafe(string path, string rootPath)
		{
			string fullPath = Path.GetFullPath(path);
			return fullPath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase);
		}

		// Get directories in a path
		app.MapGet("/api/browse/directories", (string? path) =>
		{
			string targetPath = string.IsNullOrEmpty(path)
				? musicRootPath
				: Path.GetFullPath(Path.Combine(musicRootPath, path));

			if (!IsPathSafe(targetPath, musicRootPath) || !Directory.Exists(targetPath))
				return Results.BadRequest();

			var directories = Directory.GetDirectories(targetPath)
				.Select(d => new
				{
					name = Path.GetFileName(d),
					path = Path.GetRelativePath(musicRootPath, d)
				})
				.OrderBy(d => d.name);

			return Results.Ok(directories);
		});

		// Get audio files in a path
		app.MapGet("/api/browse/files", (string? path) =>
		{
			string targetPath = string.IsNullOrEmpty(path)
				? musicRootPath
				: Path.GetFullPath(Path.Combine(musicRootPath, path));

			if (!IsPathSafe(targetPath, musicRootPath) || !Directory.Exists(targetPath))
				return Results.BadRequest();

			string[] audioExtensions = { ".flac", ".mp3", ".ogg", ".wav", ".m4a" };
			var files = Directory.GetFiles(targetPath)
				.Where(f => audioExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
				.Select(f => new
				{
					name = Path.GetFileName(f),
					path = Path.GetRelativePath(musicRootPath, f)
				})
				.OrderBy(f => f.name);

			return Results.Ok(files);
		});

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

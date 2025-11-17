var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.YouTubeToMp3_BlazorFrontend>("youtubetomp3-blazorfrontend");

builder.Build().Run();

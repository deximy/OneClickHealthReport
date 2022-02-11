var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<OneClickHealthReport.API.Services.WeComLogin>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(
    options => {
        options.AddPolicy(
            name: "AllowAll",
            builder => {
                builder.SetIsOriginAllowed((host) => true);
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
                builder.AllowCredentials();
            }
        );
    }
);

builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors("AllowAll");
var websocket_options = new WebSocketOptions();
websocket_options.AllowedOrigins.Add("*");
app.UseWebSockets(websocket_options);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.MapControllers();

app.UseEndpoints(
    endpoints => {
        endpoints.MapHub<OneClickHealthReport.API.Hubs.Login>("/login");
    }
);

app.Run();

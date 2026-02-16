using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

// Agrega servicio singleton para persistir notificaciones en memoria
builder.Services.AddSingleton<NotificationSender>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

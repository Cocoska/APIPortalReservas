using APIPruebaAviatur.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<DummyJsonClient>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IReservaService, ReservaService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", 
        builder => builder.WithOrigins("https://localhost:7271")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();

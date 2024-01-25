using LifeApi.BusinessLogic.Managers;
using LifeApi.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextPool<LifeApiDbContext>(
    o => o.UseSqlServer(builder.Configuration.GetConnectionString("LifeApiContext")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AnyOrigin",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddScoped<IBoardManager, BoardManager>();

var origin = builder.Configuration.GetValue<string>("CorsOrigin") ?? "*";

builder.Services.AddCors(options =>
{
    options.AddPolicy("SpecificOrigins",
        corsBuilder =>
        {
            corsBuilder.WithOrigins(origin)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.UseCors("AnyOrigin");
}
else
{
    app.UseCors("SpecificOrigins");
}


Func<List<List<bool>>, bool[,]> ConvertToBoolArray = (List<List<bool>> source) =>
{
    int rows = source.Count;
    int cols = source[0].Count;
    bool[,] result = new bool[rows, cols];

    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            result[i, j] = source[i][j];
        }
    }

    return result;
};

TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);
TypeAdapterConfig.GlobalSettings.NewConfig<List<List<bool>>, bool[,]>()
.MapWith((src) => ConvertToBoolArray(src));



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

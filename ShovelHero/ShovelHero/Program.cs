using ShovelHero.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// 註冊資料儲存（暫存在記憶體）
builder.Services.AddSingleton<DataStore>();

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// --- 簡單的記憶體儲存類別 ---
public class DataStore
{
    public List<Demand> Demands { get; } = new();
    public List<Application> Applications { get; } = new();

    public DataStore()
    {
        // 預設一筆測試資料
        Demands.Add(new Demand
        {
            Id = Guid.NewGuid(),
            TaskType = "清理",
            AddressCode = "A-1",
            RequiredCount = 3,
            MeetingPoint = "捷運出口",
            RiskNote = "請穿戴手套",
            ContactInfo = "0912-345-678",
            CreatedAt = DateTime.UtcNow
        });
    }
}

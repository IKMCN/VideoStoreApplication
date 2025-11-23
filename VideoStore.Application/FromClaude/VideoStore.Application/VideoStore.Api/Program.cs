using VideoStore.Application.Repositories;
using VideoStore.Application.Services;
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add your repository
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton<ICustomerRepository>(sp => new CustomerRepository(connectionString));
builder.Services.AddSingleton<IVideoRepository>(sp => new VideoRepository(connectionString));
builder.Services.AddSingleton<IRentalRepository>(sp => new RentalRepository(connectionString));

// Configure HttpClient for Banking API
var bankingApiUrl = builder.Configuration["BankingApi:BaseUrl"] ?? "http://localhost:5001";
builder.Services.AddHttpClient<IBankingApiService, BankingApiService>(client =>
{
    client.BaseAddress = new Uri(bankingApiUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    // TODO: Add authentication header when Banking API requires it
    // client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_TOKEN");
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");  // Use CORS
app.UseAuthorization();
app.MapControllers();

app.Run();

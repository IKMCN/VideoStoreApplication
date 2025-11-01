using VideoStore.Application.Repositories;

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
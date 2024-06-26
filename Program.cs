using CloudinaryDotNet;
using dotenv.net;
using Microsoft.EntityFrameworkCore;
using PokemonReviewApp;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Repository;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));

// Get Cloudinary URL from environment variables
string cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL");

if (string.IsNullOrEmpty(cloudinaryUrl))
{
    throw new Exception("Cloudinary configuration is missing.");
}

// Configure Cloudinary
Cloudinary cloudinary = new Cloudinary(cloudinaryUrl);
cloudinary.Api.Secure = true;

// Register Cloudinary as a singleton service
builder.Services.AddSingleton(cloudinary);

// Add services to the container.
builder.Services.AddTransient<Seed>();
builder.Services.AddScoped<IPokemonRepository,PokemonRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewerRepository, ReviewerRepository>();

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();
if (args.Length == 1 && args[0].ToLower() == "seeddata")
    SeedData(app);

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<Seed>();
        service.SeedDataContext();
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

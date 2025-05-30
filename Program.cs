using Microsoft.EntityFrameworkCore;
using poplensUserProfileApi.Contracts;
using poplensUserProfileApi.Data;
using poplensUserProfileApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;
using poplensUserProfileApi.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext with PostgreSQL connection string
builder.Services.AddDbContext<UserProfileDbContext>(options =>
    options.UseNpgsql("Host=postgresProfile;Port=5432;Username=postgre;Password=postgre;Database=Profile"));

// Register ProfileService
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUserAuthenticationApiProxyService, UserAuthenticationApiProxyService>();
builder.Services.AddScoped<IMediaApiProxyService, MediaApiProxyService>();
builder.Services.AddScoped<IEmbeddingProxyService, EmbeddingProxyService>();
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "YourIssuer",
            ValidAudience = "YourAudience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("moresimplekeyrightherefolkssssssssssssss"))
        };
    });

// Add controllers and other necessary services
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new VectorJsonConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//using (var scope = app.Services.CreateScope()) {
//    var dbContext = scope.ServiceProvider.GetRequiredService<UserProfileDbContext>();
//    dbContext.Database.Migrate(); // This applies any pending migrations
//}

app.UseSwagger();
app.UseSwaggerUI();



app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Veenhoop.Data;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//add corse
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
        });
});

//add JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,   //Token kan alleen worden gebruikt op eigen domein
        ValidateAudience = true, //Token kan alleen worden gebruikt op eigen API
        ValidateLifetime = true, //Token kan alleen worden gebruikt binnen een bepaalde tijd
        ValidateIssuerSigningKey = true, //Token kan alleen worden gebruikt met een bepaalde sleutel
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Use cors
app.UseCors(MyAllowSpecificOrigins);  //("AllowAll"); //(MyAllowSpecificOrigins);
app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);   

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();

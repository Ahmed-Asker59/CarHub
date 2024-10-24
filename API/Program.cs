
using Core.Identity;
using Core.Interface;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Settings;
using Hangfire;
using Hangfire.Dashboard;
using API.Filters;
using Microsoft.AspNetCore.Authorization;
using API.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); 
builder.Services.AddDbContext<CarContext>(opt =>
{
    opt.UseSqlServer(connectionString);
});



builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IRentRepository, RentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<IEmailBodyBuilder, EmailBodyBuilder>();



//backgroundjobs
builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();
builder.Services.Configure<AuthorizationOptions>(

   opt => opt.AddPolicy("staffOnly", policy =>
   {
       policy.RequireAuthenticatedUser();
   })

);


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddCors(opt => 
   opt.AddPolicy("CorsPolicy", policy =>
   {
       policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
   })

);


builder.Services.AddDbContext<AppIdentityDBContext>(opt =>

{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
});
builder.Services.AddIdentityCore<AppUser>(opt =>
{

}).AddEntityFrameworkStores<AppIdentityDBContext>().AddSignInManager<SignInManager<AppUser>>();

var jwt = builder.Configuration.GetSection("Token");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options => {
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"])),
        ValidIssuer = jwt["Issuer"],
        ValidateIssuer = true,
        ValidateAudience = false,



    };

});
builder.Services.AddAuthorization();

builder.Services.AddScoped<ITokenService, JWTTokenService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("CorsPolicy");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<CarContext>();
var identityContext = services.GetRequiredService<AppIdentityDBContext>();
var userManger = services.GetRequiredService<UserManager<AppUser>>();

var logger = services.GetRequiredService<ILogger<Program>>();

try
{

    await CarContextSeed.SeedAsync(context);
    await AppIdentityDbContextSeed.SeedUserAsync(userManger);
}
catch (Exception ex)
{
    logger.LogError(ex, ex.Message);
}


app.UseHangfireDashboard("/Hangfire");
var emailSender = services.GetRequiredService<IMailService>();
var carRepository = services.GetRequiredService<ICarRepository>();
var clientRepository = services.GetRequiredService<IClientRepository>();
var reservationRepository = services.GetRequiredService<IReservationRepository>();
var rentalRepository = services.GetRequiredService<IRentRepository>();
var emailBodyBuilder = services.GetRequiredService<IEmailBodyBuilder>();
var hangfireTasks = new hangfireTasks(carRepository, clientRepository, reservationRepository, rentalRepository, emailSender, emailBodyBuilder);
RecurringJob.AddOrUpdate(() => hangfireTasks.PrepareExpirationAlert(), "0 14 * * *");
app.MapControllers();


app.Run();



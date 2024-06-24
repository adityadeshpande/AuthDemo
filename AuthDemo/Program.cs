using AuthDemo.Dtos.Options;
using AuthDemo.Helpers;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpClient();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
ApplicationBuilderHelper.ConfigureSwagger(builder);

ApplicationBuilderHelper.CofigureServiceDI(builder);
ApplicationBuilderHelper.ConfigureOptions(builder);

ApplicationBuilderHelper.AddAuthentication(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();


var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();

ApplicationMiddlewareHelper.UseExceptionHandlerMiddleware(app);
ApplicationMiddlewareHelper.ConfigureTokenValidationMiddleware(app, jwtOptions);

app.UseAuthorization();

app.UseExceptionHandlerMiddleware();

app.MapControllers();

app.Run();


using EPLSquadBackend.Configuration;
using EPLSquadBackend.Services;
using StackExchange.Redis;

namespace EPLSquadBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Register FootballApiService with DI
            builder.Services.AddHttpClient<FootballApiService>();
            builder.Services.AddLogging();
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]));

            builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
            builder.Services.AddSingleton<IRabbitMQService,RabbitMQService>();


            builder.Services.Configure<FootballApiSettings>(builder.Configuration.GetSection("FootballApi"));
            builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));


            var app = builder.Build();

            app.UseCors("AllowAll");

            
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

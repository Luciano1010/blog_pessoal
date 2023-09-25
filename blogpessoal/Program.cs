
using blogpessoal.Data;
using blogpessoal.Model;
using blogpessoal.Service;
using blogpessoal.Service.Implements;
using blogpessoal.Validator;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace blogpessoal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Conex�o com o banco de Dados
            var connecetionString = builder.Configuration
                .GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connecetionString));


            // registrar  a Valida��o das Entidades
            builder.Services.AddTransient<IValidator<Postagem>, PostagemValidator>(); // transiente ele guarda informa��es somente quando aplica��o estiver funcionando

            // registrar as classes de servi�o (service)
            builder.Services.AddScoped<IPostagemService, PostagemService>(); // scoped ele guarda mesmo que aplica��o fecha

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configura��o do CORS onde fa�o a configura��o onde habilito que o front converse com o backend
            builder.Services.AddCors(options => 
            {
                options.AddPolicy(name: "My policy",
                    policy => 
                    {
                        policy.AllowAnyOrigin() // receber as requisi��es do front
                              .AllowAnyMethod() // receber os pots,get e delete
                              .AllowAnyHeader(); // para receber o token
                    
                    });
                
            }); ;
            var app = builder.Build();

            // Criar o banco de dados e as tabelas automaticamente
            using(var scope = app.Services.CreateAsyncScope()) // CreateasyScope cria o banco de dados e as tabelas e consulta os contextos
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();
            }

            

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            
            app.UseCors("Mypolicy");// ele inicializa o CORS

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
   
    }
}
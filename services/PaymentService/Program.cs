
using Microsoft.EntityFrameworkCore;
using Serilog;
using PaymentService.Data;
using PaymentService.Messaging;
using PaymentService.Repositories;
using PaymentService.Services;
using PaymentService.Validator;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService.Services.PaymentService>();
builder.Services.AddScoped<IPaymentEventPublisher, PaymentEventPublisher>();

builder.Services.AddValidatorsFromAssemblyContaining<CreatePaymentRequestValidator>();

var app = builder.Build();

app.MapControllers();

app.Run();
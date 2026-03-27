    using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Registrer services
builder.Services.AddSingleton<PaymentService>();
builder.Services.AddSingleton<DiscountService>();

var app = builder.Build();

app.MapControllers();

app.Run();
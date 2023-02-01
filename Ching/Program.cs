using MediatR;
using Ching.Data;

using Ching.Features.Account;
using Ching.Features.AccountPartition;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddSqlite<ChingContext>("Data Source=Ching.db");
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseAuthorization();

app.MapGroup("/account").MapAccountsApi();
app.MapGroup("/account/{accountId}/partition").MapAccountPartitionsApi();

app.Run();

public partial class Program { }
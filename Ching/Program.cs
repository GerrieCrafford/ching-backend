using MediatR;
using Ching.Data;

using Ching.Features.Account;
using Ching.Features.AccountPartition;
using Ching.Features.AccountTransaction;
using Ching.Features.BudgetCategory;
using Ching.Features.BudgetIncrease;
using Ching.Features.MonthBudget;
using Ching.Features.Overview;
using Ching.Features.Settlement;
using Ching.Features.Transfer;

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
app.MapGroup("/account-transaction").MapAccountTransactionsApi();
app.MapGroup("/budget-category").MapBudgetCategoriesApi();
app.MapGroup("/budget-increase").MapBudgetIncreasesApi();
app.MapGroup("/month-budget").MapMonthBudgetsEndpoints();
app.MapGroup("/overview").MapOverviewEndpoints();
app.MapGroup("/settlement").MapSettlementEndpoints();
app.MapGroup("/transfer").MapTransferEndpoints();

app.Run();

public partial class Program { }
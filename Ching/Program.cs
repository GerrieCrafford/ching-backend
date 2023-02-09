using MediatR;
using FluentValidation;

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
using Ching.PipelineBehaviors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString());
});
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddSqlite<ChingContext>("Data Source=Ching.db");
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

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

if (app.Configuration["RunSeed"] == "true")
{
    using (var serviceScope = app.Services.CreateScope())
    {
        var db = serviceScope.ServiceProvider.GetService<ChingContext>() ?? throw new ArgumentNullException("ChingContext should not be null during startup");

        var seedAccount = db.Accounts.Where(a => a.Name == "Cheque seed").FirstOrDefault();
        if (seedAccount == null)
        {
            Console.WriteLine("Seeding DB");
            DatabaseSeeder.Seed(db);
        }
    }
}

app.Run();

public partial class Program { }
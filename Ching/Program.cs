using MediatR;
using FluentValidation;
using System.Threading.Tasks;

using Ching.Data;
using Ching.PipelineBehaviors;

using Ching.Features.Account;
using Ching.Features.AccountPartition;
using Ching.Features.AccountTransaction;
using Ching.Features.BudgetAssignment;
using Ching.Features.BudgetCategory;
using Ching.Features.BudgetIncrease;
using Ching.Features.MonthBudget;
using Ching.Features.Overview;
using Ching.Features.Settlement;
using Ching.Features.Transfer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();
    options.CustomSchemaIds(type => type.ToString());
    options.SchemaFilter<Ching.Utilities.NullableSchemaFilter>();
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
app.MapGroup("/budget-assignment").MapBudgetAssignmentTransactions();

if (app.Configuration["RunSeed"] == "true")
{
    using (var serviceScope = app.Services.CreateScope())
    {
        var db = serviceScope.ServiceProvider.GetRequiredService<ChingContext>();
        var mediator = serviceScope.ServiceProvider.GetRequiredService<IMediator>();

        var seedAccount = db.Accounts.Where(a => a.Name == "Cheque seed").FirstOrDefault();
        if (seedAccount == null)
        {
            Console.WriteLine("Seeding DB");
            var res = DatabaseSeeder.Seed(db, mediator);
            res.Wait();
            Console.WriteLine("DB seeding done");
        }
    }
}

app.Run();

public partial class Program { }

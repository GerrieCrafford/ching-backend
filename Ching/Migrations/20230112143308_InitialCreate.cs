using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ching.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BudgetCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountPartitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    BudgetMonthYear = table.Column<int>(name: "BudgetMonth_Year", type: "INTEGER", nullable: false),
                    BudgetMonthMonth = table.Column<int>(name: "BudgetMonth_Month", type: "INTEGER", nullable: false),
                    AccountId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountPartitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountPartitions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonthBudgets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    BudgetMonthYear = table.Column<int>(name: "BudgetMonth_Year", type: "INTEGER", nullable: false),
                    BudgetMonthMonth = table.Column<int>(name: "BudgetMonth_Month", type: "INTEGER", nullable: false),
                    BudgetCategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthBudgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthBudgets_BudgetCategories_BudgetCategoryId",
                        column: x => x.BudgetCategoryId,
                        principalTable: "BudgetCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    SourcePartitionId = table.Column<int>(type: "INTEGER", nullable: false),
                    DestinationPartitionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transfers_AccountPartitions_DestinationPartitionId",
                        column: x => x.DestinationPartitionId,
                        principalTable: "AccountPartitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfers_AccountPartitions_SourcePartitionId",
                        column: x => x.SourcePartitionId,
                        principalTable: "AccountPartitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetAssignmentsTransfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransferId = table.Column<int>(type: "INTEGER", nullable: true),
                    BudgetCategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    BudgetMonthYear = table.Column<int>(name: "BudgetMonth_Year", type: "INTEGER", nullable: false),
                    BudgetMonthMonth = table.Column<int>(name: "BudgetMonth_Month", type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAssignmentsTransfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetAssignmentsTransfers_BudgetCategories_BudgetCategoryId",
                        column: x => x.BudgetCategoryId,
                        principalTable: "BudgetCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetAssignmentsTransfers_Transfers_TransferId",
                        column: x => x.TransferId,
                        principalTable: "Transfers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BudgetIncreases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransferId = table.Column<int>(type: "INTEGER", nullable: false),
                    BudgetMonthYear = table.Column<int>(name: "BudgetMonth_Year", type: "INTEGER", nullable: false),
                    BudgetMonthMonth = table.Column<int>(name: "BudgetMonth_Month", type: "INTEGER", nullable: false),
                    BudgetCategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetIncreases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetIncreases_BudgetCategories_BudgetCategoryId",
                        column: x => x.BudgetCategoryId,
                        principalTable: "BudgetCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetIncreases_Transfers_TransferId",
                        column: x => x.TransferId,
                        principalTable: "Transfers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Settlements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    TransferId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settlements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Settlements_Transfers_TransferId",
                        column: x => x.TransferId,
                        principalTable: "Transfers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    AccountPartitionId = table.Column<int>(type: "INTEGER", nullable: false),
                    AccountId = table.Column<int>(type: "INTEGER", nullable: true),
                    SettlementId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountTransactions_AccountPartitions_AccountPartitionId",
                        column: x => x.AccountPartitionId,
                        principalTable: "AccountPartitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountTransactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountTransactions_Settlements_SettlementId",
                        column: x => x.SettlementId,
                        principalTable: "Settlements",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BudgetAssignmentsTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountTransactionId = table.Column<int>(type: "INTEGER", nullable: true),
                    BudgetCategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    BudgetMonthYear = table.Column<int>(name: "BudgetMonth_Year", type: "INTEGER", nullable: false),
                    BudgetMonthMonth = table.Column<int>(name: "BudgetMonth_Month", type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAssignmentsTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetAssignmentsTransactions_AccountTransactions_AccountTransactionId",
                        column: x => x.AccountTransactionId,
                        principalTable: "AccountTransactions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BudgetAssignmentsTransactions_BudgetCategories_BudgetCategoryId",
                        column: x => x.BudgetCategoryId,
                        principalTable: "BudgetCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountPartitions_AccountId",
                table: "AccountPartitions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransactions_AccountId",
                table: "AccountTransactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransactions_AccountPartitionId",
                table: "AccountTransactions",
                column: "AccountPartitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransactions_SettlementId",
                table: "AccountTransactions",
                column: "SettlementId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAssignmentsTransactions_AccountTransactionId",
                table: "BudgetAssignmentsTransactions",
                column: "AccountTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAssignmentsTransactions_BudgetCategoryId",
                table: "BudgetAssignmentsTransactions",
                column: "BudgetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAssignmentsTransfers_BudgetCategoryId",
                table: "BudgetAssignmentsTransfers",
                column: "BudgetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAssignmentsTransfers_TransferId",
                table: "BudgetAssignmentsTransfers",
                column: "TransferId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetIncreases_BudgetCategoryId",
                table: "BudgetIncreases",
                column: "BudgetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetIncreases_TransferId",
                table: "BudgetIncreases",
                column: "TransferId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthBudgets_BudgetCategoryId",
                table: "MonthBudgets",
                column: "BudgetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Settlements_TransferId",
                table: "Settlements",
                column: "TransferId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_DestinationPartitionId",
                table: "Transfers",
                column: "DestinationPartitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_SourcePartitionId",
                table: "Transfers",
                column: "SourcePartitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetAssignmentsTransactions");

            migrationBuilder.DropTable(
                name: "BudgetAssignmentsTransfers");

            migrationBuilder.DropTable(
                name: "BudgetIncreases");

            migrationBuilder.DropTable(
                name: "MonthBudgets");

            migrationBuilder.DropTable(
                name: "AccountTransactions");

            migrationBuilder.DropTable(
                name: "BudgetCategories");

            migrationBuilder.DropTable(
                name: "Settlements");

            migrationBuilder.DropTable(
                name: "Transfers");

            migrationBuilder.DropTable(
                name: "AccountPartitions");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}

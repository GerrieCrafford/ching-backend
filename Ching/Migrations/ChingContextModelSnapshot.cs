﻿// <auto-generated />
using System;
using Ching.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Ching.Migrations
{
    [DbContext(typeof(ChingContext))]
    partial class ChingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.1");

            modelBuilder.Entity("Ching.Entities.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Ching.Entities.AccountPartition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccountId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Archived")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("AccountPartitions");
                });

            modelBuilder.Entity("Ching.Entities.AccountTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccountId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccountPartitionId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.Property<string>("Recipient")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("SettlementId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("AccountPartitionId");

                    b.HasIndex("SettlementId");

                    b.ToTable("AccountTransactions");
                });

            modelBuilder.Entity("Ching.Entities.BudgetAssignmentTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AccountTransactionId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<int>("BudgetCategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountTransactionId");

                    b.HasIndex("BudgetCategoryId");

                    b.ToTable("BudgetAssignmentsTransactions");
                });

            modelBuilder.Entity("Ching.Entities.BudgetAssignmentTransfer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<int>("BudgetCategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BudgetCategoryId");

                    b.ToTable("BudgetAssignmentsTransfers");
                });

            modelBuilder.Entity("Ching.Entities.BudgetCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("BudgetCategories");
                });

            modelBuilder.Entity("Ching.Entities.BudgetIncrease", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BudgetCategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TransferId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BudgetCategoryId");

                    b.HasIndex("TransferId");

                    b.ToTable("BudgetIncreases");
                });

            modelBuilder.Entity("Ching.Entities.MonthBudget", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<int>("BudgetCategoryId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BudgetCategoryId");

                    b.ToTable("MonthBudgets");
                });

            modelBuilder.Entity("Ching.Entities.Settlement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("TransferId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TransferId");

                    b.ToTable("Settlements");
                });

            modelBuilder.Entity("Ching.Entities.Transfer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<int?>("BudgetAssignmentId")
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("DestinationPartitionId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SourcePartitionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BudgetAssignmentId");

                    b.HasIndex("DestinationPartitionId");

                    b.HasIndex("SourcePartitionId");

                    b.ToTable("Transfers");
                });

            modelBuilder.Entity("Ching.Entities.AccountPartition", b =>
                {
                    b.HasOne("Ching.Entities.Account", "Account")
                        .WithMany("Partitions")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Ching.Entities.BudgetMonth", "BudgetMonth", b1 =>
                        {
                            b1.Property<int>("AccountPartitionId")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Month")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Year")
                                .HasColumnType("INTEGER");

                            b1.HasKey("AccountPartitionId");

                            b1.ToTable("AccountPartitions");

                            b1.WithOwner()
                                .HasForeignKey("AccountPartitionId");
                        });

                    b.Navigation("Account");

                    b.Navigation("BudgetMonth");
                });

            modelBuilder.Entity("Ching.Entities.AccountTransaction", b =>
                {
                    b.HasOne("Ching.Entities.Account", "Account")
                        .WithMany("Transactions")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Ching.Entities.AccountPartition", "AccountPartition")
                        .WithMany()
                        .HasForeignKey("AccountPartitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Ching.Entities.Settlement", null)
                        .WithMany("AccountTransactions")
                        .HasForeignKey("SettlementId");

                    b.Navigation("Account");

                    b.Navigation("AccountPartition");
                });

            modelBuilder.Entity("Ching.Entities.BudgetAssignmentTransaction", b =>
                {
                    b.HasOne("Ching.Entities.AccountTransaction", null)
                        .WithMany("BudgetAssignments")
                        .HasForeignKey("AccountTransactionId");

                    b.HasOne("Ching.Entities.BudgetCategory", "BudgetCategory")
                        .WithMany()
                        .HasForeignKey("BudgetCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Ching.Entities.BudgetMonth", "BudgetMonth", b1 =>
                        {
                            b1.Property<int>("BudgetAssignmentTransactionId")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Month")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Year")
                                .HasColumnType("INTEGER");

                            b1.HasKey("BudgetAssignmentTransactionId");

                            b1.ToTable("BudgetAssignmentsTransactions");

                            b1.WithOwner()
                                .HasForeignKey("BudgetAssignmentTransactionId");
                        });

                    b.Navigation("BudgetCategory");

                    b.Navigation("BudgetMonth")
                        .IsRequired();
                });

            modelBuilder.Entity("Ching.Entities.BudgetAssignmentTransfer", b =>
                {
                    b.HasOne("Ching.Entities.BudgetCategory", "BudgetCategory")
                        .WithMany()
                        .HasForeignKey("BudgetCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Ching.Entities.BudgetMonth", "BudgetMonth", b1 =>
                        {
                            b1.Property<int>("BudgetAssignmentTransferId")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Month")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Year")
                                .HasColumnType("INTEGER");

                            b1.HasKey("BudgetAssignmentTransferId");

                            b1.ToTable("BudgetAssignmentsTransfers");

                            b1.WithOwner()
                                .HasForeignKey("BudgetAssignmentTransferId");
                        });

                    b.Navigation("BudgetCategory");

                    b.Navigation("BudgetMonth")
                        .IsRequired();
                });

            modelBuilder.Entity("Ching.Entities.BudgetCategory", b =>
                {
                    b.HasOne("Ching.Entities.BudgetCategory", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Ching.Entities.BudgetIncrease", b =>
                {
                    b.HasOne("Ching.Entities.BudgetCategory", "BudgetCategory")
                        .WithMany()
                        .HasForeignKey("BudgetCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Ching.Entities.Transfer", "Transfer")
                        .WithMany()
                        .HasForeignKey("TransferId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Ching.Entities.BudgetMonth", "BudgetMonth", b1 =>
                        {
                            b1.Property<int>("BudgetIncreaseId")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Month")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Year")
                                .HasColumnType("INTEGER");

                            b1.HasKey("BudgetIncreaseId");

                            b1.ToTable("BudgetIncreases");

                            b1.WithOwner()
                                .HasForeignKey("BudgetIncreaseId");
                        });

                    b.Navigation("BudgetCategory");

                    b.Navigation("BudgetMonth")
                        .IsRequired();

                    b.Navigation("Transfer");
                });

            modelBuilder.Entity("Ching.Entities.MonthBudget", b =>
                {
                    b.HasOne("Ching.Entities.BudgetCategory", "BudgetCategory")
                        .WithMany()
                        .HasForeignKey("BudgetCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Ching.Entities.BudgetMonth", "BudgetMonth", b1 =>
                        {
                            b1.Property<int>("MonthBudgetId")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Month")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Year")
                                .HasColumnType("INTEGER");

                            b1.HasKey("MonthBudgetId");

                            b1.ToTable("MonthBudgets");

                            b1.WithOwner()
                                .HasForeignKey("MonthBudgetId");
                        });

                    b.Navigation("BudgetCategory");

                    b.Navigation("BudgetMonth")
                        .IsRequired();
                });

            modelBuilder.Entity("Ching.Entities.Settlement", b =>
                {
                    b.HasOne("Ching.Entities.Transfer", "Transfer")
                        .WithMany()
                        .HasForeignKey("TransferId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Transfer");
                });

            modelBuilder.Entity("Ching.Entities.Transfer", b =>
                {
                    b.HasOne("Ching.Entities.BudgetAssignmentTransfer", "BudgetAssignment")
                        .WithMany()
                        .HasForeignKey("BudgetAssignmentId");

                    b.HasOne("Ching.Entities.AccountPartition", "DestinationPartition")
                        .WithMany()
                        .HasForeignKey("DestinationPartitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Ching.Entities.AccountPartition", "SourcePartition")
                        .WithMany()
                        .HasForeignKey("SourcePartitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BudgetAssignment");

                    b.Navigation("DestinationPartition");

                    b.Navigation("SourcePartition");
                });

            modelBuilder.Entity("Ching.Entities.Account", b =>
                {
                    b.Navigation("Partitions");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("Ching.Entities.AccountTransaction", b =>
                {
                    b.Navigation("BudgetAssignments");
                });

            modelBuilder.Entity("Ching.Entities.Settlement", b =>
                {
                    b.Navigation("AccountTransactions");
                });
#pragma warning restore 612, 618
        }
    }
}

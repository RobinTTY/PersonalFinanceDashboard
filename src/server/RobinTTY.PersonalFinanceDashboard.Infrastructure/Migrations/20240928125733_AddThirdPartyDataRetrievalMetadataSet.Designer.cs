﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RobinTTY.PersonalFinanceDashboard.Infrastructure;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240928125733_AddThirdPartyDataRetrievalMetadataSet")]
    partial class AddThirdPartyDataRetrievalMetadataSet
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities.BankingInstitutionEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Bic")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Countries")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LogoUri")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("BankingInstitutions");
                });

            modelBuilder.Entity("RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities.TagEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities.ThirdPartyDataRetrievalMetadataEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("DataSource")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DataType")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastRetrievalTime")
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan>("RetrievalInterval")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ThirdPartyDataRetrievalMetadata");

                    b.HasData(
                        new
                        {
                            Id = new Guid("f948e52f-ad17-44b8-9cdf-e0b952f139b3"),
                            DataSource = 1,
                            DataType = 1,
                            LastRetrievalTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            RetrievalInterval = new TimeSpan(7, 0, 0, 0, 0)
                        });
                });

            modelBuilder.Entity("RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities.TransactionEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("AccountId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Payee")
                        .HasColumnType("TEXT");

                    b.Property<string>("Payer")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ValueDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("TagEntityTransactionEntity", b =>
                {
                    b.Property<string>("TagsId")
                        .HasColumnType("TEXT");

                    b.Property<string>("TransactionsId")
                        .HasColumnType("TEXT");

                    b.HasKey("TagsId", "TransactionsId");

                    b.HasIndex("TransactionsId");

                    b.ToTable("TagEntityTransactionEntity");
                });

            modelBuilder.Entity("TagEntityTransactionEntity", b =>
                {
                    b.HasOne("RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities.TagEntity", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities.TransactionEntity", null)
                        .WithMany()
                        .HasForeignKey("TransactionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}

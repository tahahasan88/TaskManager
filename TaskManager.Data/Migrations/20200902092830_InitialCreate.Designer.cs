﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskManager.Data;

namespace TaskManager.Data.Migrations
{
    [DbContext(typeof(TaskManagerContext))]
    [Migration("20200902092830_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TaskManager.Data.SubTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LastUpdatedById")
                        .HasColumnType("int");

                    b.Property<int?>("TaskId")
                        .HasColumnType("int");

                    b.Property<int?>("TaskStatusId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LastUpdatedById");

                    b.HasIndex("TaskId");

                    b.HasIndex("TaskStatusId");

                    b.ToTable("SubTasks");
                });

            modelBuilder.Entity("TaskManager.Data.Task", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("LastUpdatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("Target")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TaskPriorityId")
                        .HasColumnType("int");

                    b.Property<string>("TaskProgress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TaskStatusId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TaskPriorityId");

                    b.HasIndex("TaskStatusId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("TaskManager.Data.TaskCapacity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Capacity")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TaskCapacities");
                });

            modelBuilder.Entity("TaskManager.Data.TaskEmployee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("TaskCapacityId")
                        .HasColumnType("int");

                    b.Property<int?>("TaskId")
                        .HasColumnType("int");

                    b.Property<string>("UserCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TaskCapacityId");

                    b.HasIndex("TaskId");

                    b.ToTable("TaskEmployees");
                });

            modelBuilder.Entity("TaskManager.Data.TaskFollowUp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdatedBy")
                        .HasColumnType("datetime2");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TaskId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("TaskId");

                    b.ToTable("TaskFollowUps");
                });

            modelBuilder.Entity("TaskManager.Data.TaskFollowUpEmployee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AssigneeId")
                        .HasColumnType("int");

                    b.Property<int?>("TaskFollowUpId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AssigneeId");

                    b.HasIndex("TaskFollowUpId");

                    b.ToTable("TaskFollowUpEmployees");
                });

            modelBuilder.Entity("TaskManager.Data.TaskFollowUpResponse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdatedBy")
                        .HasColumnType("datetime2");

                    b.Property<int?>("RespondedById")
                        .HasColumnType("int");

                    b.Property<int?>("TaskFollowUpId")
                        .HasColumnType("int");

                    b.Property<string>("TaskResponse")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RespondedById");

                    b.HasIndex("TaskFollowUpId");

                    b.ToTable("TaskFollowUpResponses");
                });

            modelBuilder.Entity("TaskManager.Data.TaskPriority", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Priority")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TaskPriority");
                });

            modelBuilder.Entity("TaskManager.Data.TaskStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TaskStatus");
                });

            modelBuilder.Entity("TaskManager.Data.SubTask", b =>
                {
                    b.HasOne("TaskManager.Data.TaskEmployee", "LastUpdatedBy")
                        .WithMany()
                        .HasForeignKey("LastUpdatedById");

                    b.HasOne("TaskManager.Data.Task", "Task")
                        .WithMany()
                        .HasForeignKey("TaskId");

                    b.HasOne("TaskManager.Data.TaskStatus", "TaskStatus")
                        .WithMany()
                        .HasForeignKey("TaskStatusId");
                });

            modelBuilder.Entity("TaskManager.Data.Task", b =>
                {
                    b.HasOne("TaskManager.Data.TaskPriority", "TaskPriority")
                        .WithMany()
                        .HasForeignKey("TaskPriorityId");

                    b.HasOne("TaskManager.Data.TaskStatus", "TaskStatus")
                        .WithMany()
                        .HasForeignKey("TaskStatusId");
                });

            modelBuilder.Entity("TaskManager.Data.TaskEmployee", b =>
                {
                    b.HasOne("TaskManager.Data.TaskCapacity", "TaskCapacity")
                        .WithMany()
                        .HasForeignKey("TaskCapacityId");

                    b.HasOne("TaskManager.Data.Task", "Task")
                        .WithMany()
                        .HasForeignKey("TaskId");
                });

            modelBuilder.Entity("TaskManager.Data.TaskFollowUp", b =>
                {
                    b.HasOne("TaskManager.Data.TaskEmployee", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId");

                    b.HasOne("TaskManager.Data.Task", "Task")
                        .WithMany()
                        .HasForeignKey("TaskId");
                });

            modelBuilder.Entity("TaskManager.Data.TaskFollowUpEmployee", b =>
                {
                    b.HasOne("TaskManager.Data.TaskEmployee", "Assignee")
                        .WithMany()
                        .HasForeignKey("AssigneeId");

                    b.HasOne("TaskManager.Data.TaskFollowUp", "TaskFollowUp")
                        .WithMany()
                        .HasForeignKey("TaskFollowUpId");
                });

            modelBuilder.Entity("TaskManager.Data.TaskFollowUpResponse", b =>
                {
                    b.HasOne("TaskManager.Data.TaskEmployee", "RespondedBy")
                        .WithMany()
                        .HasForeignKey("RespondedById");

                    b.HasOne("TaskManager.Data.TaskFollowUp", "TaskFollowUp")
                        .WithMany()
                        .HasForeignKey("TaskFollowUpId");
                });
#pragma warning restore 612, 618
        }
    }
}

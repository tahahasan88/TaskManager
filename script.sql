USE [master]
GO
/****** Object:  Database [TaskManagerDB]    Script Date: 10/18/2020 2:16:40 PM ******/
CREATE DATABASE [TaskManagerDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TaskManagerDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\TaskManagerDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'TaskManagerDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\TaskManagerDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [TaskManagerDB] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TaskManagerDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TaskManagerDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TaskManagerDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TaskManagerDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TaskManagerDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TaskManagerDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [TaskManagerDB] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [TaskManagerDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TaskManagerDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TaskManagerDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TaskManagerDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TaskManagerDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TaskManagerDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TaskManagerDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TaskManagerDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TaskManagerDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [TaskManagerDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TaskManagerDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TaskManagerDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TaskManagerDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TaskManagerDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TaskManagerDB] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [TaskManagerDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TaskManagerDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [TaskManagerDB] SET  MULTI_USER 
GO
ALTER DATABASE [TaskManagerDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TaskManagerDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TaskManagerDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TaskManagerDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [TaskManagerDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [TaskManagerDB] SET QUERY_STORE = OFF
GO
USE [TaskManagerDB]
GO
/****** Object:  User [IIS APPPOOL\taskmanager]    Script Date: 10/18/2020 2:16:40 PM ******/
CREATE USER [IIS APPPOOL\taskmanager] FOR LOGIN [IIS APPPOOL\taskmanager] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 10/18/2020 2:16:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditType]    Script Date: 10/18/2020 2:16:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](max) NULL,
 CONSTRAINT [PK_AuditType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Departments]    Script Date: 10/18/2020 2:16:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Departments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[ManagerId] [int] NULL,
	[ParentDepartmentId] [int] NULL,
 CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 10/18/2020 2:16:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employees](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserCode] [nvarchar](max) NULL,
	[EmployeeName] [nvarchar](max) NULL,
	[EmailAddress] [nvarchar](max) NULL,
	[JobTitle] [nvarchar](max) NULL,
	[RegistrationNo] [nvarchar](max) NULL,
	[PhoneNo] [nvarchar](max) NULL,
	[DepartmentId] [int] NULL,
	[AvatarImage] [nvarchar](max) NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubTasks]    Script Date: 10/18/2020 2:16:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubTasks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskId] [int] NULL,
	[Description] [nvarchar](max) NULL,
	[TaskStatusId] [int] NULL,
	[LastUpdatedAt] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[LastUpdatedById] [int] NULL,
	[SubTaskAssigneeId] [int] NULL,
 CONSTRAINT [PK_SubTasks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskAudit]    Script Date: 10/18/2020 2:16:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskAudit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[TaskId] [int] NULL,
	[ActionDate] [datetime2](7) NOT NULL,
	[TypeId] [int] NULL,
	[ActionById] [int] NULL,
 CONSTRAINT [PK_TaskAudit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskCapacities]    Script Date: 10/18/2020 2:16:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskCapacities](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Capacity] [nvarchar](max) NULL,
 CONSTRAINT [PK_TaskCapacities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskEmployees]    Script Date: 10/18/2020 2:16:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskEmployees](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskId] [int] NULL,
	[TaskCapacityId] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[EmployeeId] [int] NULL,
 CONSTRAINT [PK_TaskEmployees] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskFollowUpResponses]    Script Date: 10/18/2020 2:16:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskFollowUpResponses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskResponse] [nvarchar](max) NULL,
	[LastUpdatedAt] [datetime2](7) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[RespondedById] [int] NULL,
	[TaskFollowUpId] [int] NULL,
 CONSTRAINT [PK_TaskFollowUpResponses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskFollowUps]    Script Date: 10/18/2020 2:16:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskFollowUps](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Remarks] [nvarchar](max) NULL,
	[TaskId] [int] NULL,
	[LastUpdatedAt] [datetime2](7) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[FollowerId] [int] NULL,
	[StatusId] [int] NULL,
 CONSTRAINT [PK_TaskFollowUps] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskFollowUpStatus]    Script Date: 10/18/2020 2:16:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskFollowUpStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Status] [nvarchar](max) NULL,
 CONSTRAINT [PK_TaskFollowUpStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskPriority]    Script Date: 10/18/2020 2:16:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskPriority](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Priority] [nvarchar](max) NULL,
 CONSTRAINT [PK_TaskPriority] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tasks]    Script Date: 10/18/2020 2:16:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tasks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskProgress] [nvarchar](max) NULL,
	[Title] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Target] [datetime2](7) NOT NULL,
	[TaskStatusId] [int] NULL,
	[IsDeleted] [bit] NOT NULL,
	[TaskPriorityId] [int] NULL,
	[LastUpdatedAt] [datetime2](7) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[ProgressRemarks] [nvarchar](max) NULL,
	[Employee] [int] NULL,
 CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskStatus]    Script Date: 10/18/2020 2:16:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Status] [nvarchar](max) NULL,
 CONSTRAINT [PK_TaskStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200902092830_InitialCreate', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200902115922_changeLastUpatedByColumnType-In-TASKTABLE', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200902143943_ChangeUserNameColumnName', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200902184112_TaskFollowUpChanges', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200902190049_RemoveTaskFollowUpEmployee', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200903064300_SubTaskModify', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200903132103_Add-CreatedAtDate-SubTask', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200903145130_Update-TaskResponseColumns', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200903193049_Add-TaskAuditTable', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200903193226_Update-TaskAudit', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200903200427_Update-TaskAudit1', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200903202935_Add-AuditType', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200920081527_V37', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200920081639_V38', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200920081931_V39', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200920084317_V40', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200920084601_V41', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200920084925_V42', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200920085038_V43', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200920101800_V44', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20200920102557_V45', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20201004122400_V46', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20201007142144_V47', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20201011220159_V48', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20201014141823_V49', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20201014152045_V50', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20201014152313_V51', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20201014152652_V52', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20201014153519_V54', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20201016094527_V55', N'3.1.7')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20201016111226_V56', N'3.1.7')
SET IDENTITY_INSERT [dbo].[AuditType] ON 

INSERT [dbo].[AuditType] ([Id], [Type]) VALUES (1, N'Progress')
INSERT [dbo].[AuditType] ([Id], [Type]) VALUES (2, N'Task Assignment')
INSERT [dbo].[AuditType] ([Id], [Type]) VALUES (3, N'Task FollowUp')
INSERT [dbo].[AuditType] ([Id], [Type]) VALUES (4, N'Follow Up Response')
INSERT [dbo].[AuditType] ([Id], [Type]) VALUES (5, N'Sub Tasks')
INSERT [dbo].[AuditType] ([Id], [Type]) VALUES (6, N'Update')
SET IDENTITY_INSERT [dbo].[AuditType] OFF
SET IDENTITY_INSERT [dbo].[Departments] ON 

INSERT [dbo].[Departments] ([Id], [Name], [ManagerId], [ParentDepartmentId]) VALUES (2, N'Information Technology Division', 1, NULL)
INSERT [dbo].[Departments] ([Id], [Name], [ManagerId], [ParentDepartmentId]) VALUES (3, N'IT Solutions Department', 2, 2)
INSERT [dbo].[Departments] ([Id], [Name], [ManagerId], [ParentDepartmentId]) VALUES (5, N'Corporate Solutions Team', 3, 3)
INSERT [dbo].[Departments] ([Id], [Name], [ManagerId], [ParentDepartmentId]) VALUES (6, N'Technical Solutions Team', 6, 3)
INSERT [dbo].[Departments] ([Id], [Name], [ManagerId], [ParentDepartmentId]) VALUES (7, N'IT Customer Services Department', 9, 2)
INSERT [dbo].[Departments] ([Id], [Name], [ManagerId], [ParentDepartmentId]) VALUES (8, N'End User Support Team', 11, 7)
INSERT [dbo].[Departments] ([Id], [Name], [ManagerId], [ParentDepartmentId]) VALUES (9, N'IT Service Team', 13, 7)
INSERT [dbo].[Departments] ([Id], [Name], [ManagerId], [ParentDepartmentId]) VALUES (10, N'Helpdesk Team', 15, 7)
INSERT [dbo].[Departments] ([Id], [Name], [ManagerId], [ParentDepartmentId]) VALUES (11, N'IT Planning & Strategy Department', 1002, 2)
INSERT [dbo].[Departments] ([Id], [Name], [ManagerId], [ParentDepartmentId]) VALUES (12, N'IT Infrastructure & Security Department', 1003, 2)
SET IDENTITY_INSERT [dbo].[Departments] OFF
SET IDENTITY_INSERT [dbo].[Employees] ON 

INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (1, N'hasanmuhammed', N'Hassan Mohammed Fadhel
', N'taha.hasan88@gmail.com', N'Vice President', N'1234', N'0501273980', 2, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (2, N'abdulnazar', N'Kombanezath Abdul Rahman Abdul Nazar
', N'dawar@gmail.com', N'Developer', N'1234', N'0501273980', 3, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (3, N'rahmatullah', N'Rahmat Ullah Fazal Ullah', N'Kashif@gmail.com', N'Developer', N'1234', N'0501273980', 5, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (4, N'Hasan', N'Hasan Jafar', N'hasanjafar@gmail.com', N'Developer', N'1234', N'0501273985', 5, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (5, N'Kamran', N'Kamran Shah', N'kamranshah@gmail.com', N'Developer', N'1234', N'0501273986', 5, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (6, N'mervinprakash', N'Mervin Prakash', N'bahadur.shah@gmail.com', N'Developer', N'1234', N'0501273970', 6, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (7, N'leolloyd', N'Leolloyd', N'zahid.shah@gmail.com', N'Developer', N'1234', N'0501273977', 6, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (8, N'nedaa', N'Nedaa', N'karamteen@gmail.com', N'Developer', N'1234', N'0501273877', 6, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (9, N'abdulrahman', N'Abdul Rahman Al Rumaithi', N'imrankhalid@gmail.com', N'Developer', N'1234', N'0501233877', 7, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (10, N'ZahidZikria', N'Zahid Zikria', N'zahidzikria@gmail.com', N'Developer', N'1234', N'0501244177', 7, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (11, N'Qutubuddin', N'Qutub Uddin', N'qutubuddin@gmail.com', N'Developer', N'1234', N'053444177', 8, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (12, N'SamandarKhan', N'Samandar Khan', N'samandarkhan@gmail.com', N'Developer', N'1234', N'053444177', 8, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (13, N'rohailkhan', N'Rohail Khan', N'rohailkhan@gmail.com', N'Developer', N'1234', N'053444177', 9, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (14, N'bindaskhan', N'Bindas Khan', N'bindaskhan@gmail.com', N'Developer', N'1234', N'053444177', 9, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (15, N'binakhan', N'Bina Khan', N'bindaskhan@gmail.com', N'Developer', N'1234', N'053444177', 10, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (16, N'chausakhan', N'chausa Khan', N'chausakhan@gmail.com', N'Developer', N'1234', N'053444177', 10, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (1002, N'hebaadnan', N'Heba Adnan', N'hebaadnan@gmail.com', N'Developer', N'1234', N'053444177', 11, N'../dist/img/user2-160x160.jpg')
INSERT [dbo].[Employees] ([Id], [UserCode], [EmployeeName], [EmailAddress], [JobTitle], [RegistrationNo], [PhoneNo], [DepartmentId], [AvatarImage]) VALUES (1003, N'Usman', N'Usman', N'taha.hasan88@gmail.com', N'Developer', N'1234', N'0501273981', 12, N'../dist/img/user2-160x160.jpg')
SET IDENTITY_INSERT [dbo].[Employees] OFF
SET IDENTITY_INSERT [dbo].[SubTasks] ON 

INSERT [dbo].[SubTasks] ([Id], [TaskId], [Description], [TaskStatusId], [LastUpdatedAt], [IsDeleted], [CreatedAt], [LastUpdatedById], [SubTaskAssigneeId]) VALUES (1091, 2090, N'hello subtask', 1, CAST(N'2020-10-18T12:34:17.0089204' AS DateTime2), 0, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), 3, 4)
SET IDENTITY_INSERT [dbo].[SubTasks] OFF
SET IDENTITY_INSERT [dbo].[TaskAudit] ON 

INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2758, N'Task (Corporate website handover) created. Details :- Description : Corporate website handover<br>Target Date : 18-Oct-2020<br>Priority : Medium<br>', 2090, CAST(N'2020-10-18T10:35:09.8663469' AS DateTime2), 1, 1)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2759, N'Task (Corporate website handover) created. Assigned to Hassan Mohammed Fadhel
', 2090, CAST(N'2020-10-18T10:35:09.8912923' AS DateTime2), 2, 1)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2760, N'Task (Corporate website handover) updated. Assignment changed from Hassan Mohammed Fadhel
 to Kombanezath Abdul Rahman Abdul Nazar
', 2090, CAST(N'2020-10-18T10:35:57.8185467' AS DateTime2), 2, 1)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2761, N'Sub task created with description (hello subtask) and assigned to Kombanezath Abdul Rahman Abdul Nazar
', 2090, CAST(N'2020-10-18T10:36:41.4858216' AS DateTime2), 5, 2)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2762, N'Sub task (hello subtask) updated. Assignment changed from Kombanezath Abdul Rahman Abdul Nazar
 to Rahmat Ullah Fazal Ullah', 2090, CAST(N'2020-10-18T10:37:10.9993693' AS DateTime2), 5, 2)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2763, N'Follow up requested for Task (Corporate website handover) and remarks are test follow up', 2090, CAST(N'2020-10-18T10:38:54.5008634' AS DateTime2), 3, 1)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2764, N'Follow up requested for Task (Corporate website handover) and remarks are wow remarks', 2090, CAST(N'2020-10-18T11:45:33.5954843' AS DateTime2), 3, 1)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2765, N'Follow up response provided with progress update from 0% to 41% and remarks test remarks', 2090, CAST(N'2020-10-18T11:47:01.6717945' AS DateTime2), 4, 2)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2766, N'Task (Corporate website handover) updated. Progress updated from 0% to 41%', 2090, CAST(N'2020-10-18T11:47:01.6788706' AS DateTime2), 1, 2)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2767, N'Task (Corporate website handover) updated. Remarks are test remarks', 2090, CAST(N'2020-10-18T11:47:01.6851905' AS DateTime2), 6, 2)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2768, N'Task (Corporate website handover) updated. Status updated from Not Started to In Progress', 2090, CAST(N'2020-10-18T11:47:01.6923940' AS DateTime2), 6, 2)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2769, N'Follow up requested for Task (Corporate website handover) and remarks are hello remarks', 2090, CAST(N'2020-10-18T11:48:04.9900317' AS DateTime2), 3, 1)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2770, N'Task (Corporate website handover) updated. Assignment changed from Kombanezath Abdul Rahman Abdul Nazar
 to Rahmat Ullah Fazal Ullah', 2090, CAST(N'2020-10-18T11:49:05.9674128' AS DateTime2), 2, 2)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2771, N'Sub task (hello subtask) updated. Assignment changed from Rahmat Ullah Fazal Ullah to Hasan Jafar', 2090, CAST(N'2020-10-18T12:34:17.0093867' AS DateTime2), 5, 3)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2772, N'Task (Corporate website design) created. Details :- Description : Corporate website design<br>Target Date : 18-Oct-2020<br>Priority : Medium<br>', 2091, CAST(N'2020-10-18T13:08:38.1877717' AS DateTime2), 1, 1)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2773, N'Task (Corporate website design) created. Assigned to Mervin Prakash', 2091, CAST(N'2020-10-18T13:08:38.2663288' AS DateTime2), 2, 1)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2774, N'Follow up requested for Task (Corporate website design) and remarks are hhwl', 2091, CAST(N'2020-10-18T13:08:51.8685347' AS DateTime2), 3, 1)
INSERT [dbo].[TaskAudit] ([Id], [Description], [TaskId], [ActionDate], [TypeId], [ActionById]) VALUES (2775, N'Task (Corporate website design) updated. Assignment changed from Mervin Prakash to Leolloyd', 2091, CAST(N'2020-10-18T13:19:14.6202718' AS DateTime2), 2, 6)
SET IDENTITY_INSERT [dbo].[TaskAudit] OFF
SET IDENTITY_INSERT [dbo].[TaskCapacities] ON 

INSERT [dbo].[TaskCapacities] ([Id], [Capacity]) VALUES (1, N'Creator')
INSERT [dbo].[TaskCapacities] ([Id], [Capacity]) VALUES (2, N'Assignee')
INSERT [dbo].[TaskCapacities] ([Id], [Capacity]) VALUES (3, N'Follower')
INSERT [dbo].[TaskCapacities] ([Id], [Capacity]) VALUES (4, N'Ex-TaskAssignee')
INSERT [dbo].[TaskCapacities] ([Id], [Capacity]) VALUES (5, N'SubTaskAssignee')
SET IDENTITY_INSERT [dbo].[TaskCapacities] OFF
SET IDENTITY_INSERT [dbo].[TaskEmployees] ON 

INSERT [dbo].[TaskEmployees] ([Id], [TaskId], [TaskCapacityId], [IsActive], [EmployeeId]) VALUES (2268, 2090, 2, 1, 3)
INSERT [dbo].[TaskEmployees] ([Id], [TaskId], [TaskCapacityId], [IsActive], [EmployeeId]) VALUES (2269, 2090, 1, 1, 1)
INSERT [dbo].[TaskEmployees] ([Id], [TaskId], [TaskCapacityId], [IsActive], [EmployeeId]) VALUES (2270, 2090, 5, 0, 2)
INSERT [dbo].[TaskEmployees] ([Id], [TaskId], [TaskCapacityId], [IsActive], [EmployeeId]) VALUES (2271, 2090, 5, 0, 3)
INSERT [dbo].[TaskEmployees] ([Id], [TaskId], [TaskCapacityId], [IsActive], [EmployeeId]) VALUES (2272, 2090, 3, 1, 1)
INSERT [dbo].[TaskEmployees] ([Id], [TaskId], [TaskCapacityId], [IsActive], [EmployeeId]) VALUES (2273, 2090, 5, 1, 4)
INSERT [dbo].[TaskEmployees] ([Id], [TaskId], [TaskCapacityId], [IsActive], [EmployeeId]) VALUES (2274, 2091, 2, 1, 7)
INSERT [dbo].[TaskEmployees] ([Id], [TaskId], [TaskCapacityId], [IsActive], [EmployeeId]) VALUES (2275, 2091, 1, 1, 1)
INSERT [dbo].[TaskEmployees] ([Id], [TaskId], [TaskCapacityId], [IsActive], [EmployeeId]) VALUES (2276, 2091, 3, 1, 1)
SET IDENTITY_INSERT [dbo].[TaskEmployees] OFF
SET IDENTITY_INSERT [dbo].[TaskFollowUpResponses] ON 

INSERT [dbo].[TaskFollowUpResponses] ([Id], [TaskResponse], [LastUpdatedAt], [CreatedAt], [RespondedById], [TaskFollowUpId]) VALUES (1111, N'41', CAST(N'2020-10-18T11:47:01.6630192' AS DateTime2), CAST(N'2020-10-18T11:47:01.6631224' AS DateTime2), 2, 1038)
INSERT [dbo].[TaskFollowUpResponses] ([Id], [TaskResponse], [LastUpdatedAt], [CreatedAt], [RespondedById], [TaskFollowUpId]) VALUES (1112, N'41', CAST(N'2020-10-18T11:47:01.6713354' AS DateTime2), CAST(N'2020-10-18T11:47:01.6713377' AS DateTime2), 2, 1039)
SET IDENTITY_INSERT [dbo].[TaskFollowUpResponses] OFF
SET IDENTITY_INSERT [dbo].[TaskFollowUps] ON 

INSERT [dbo].[TaskFollowUps] ([Id], [Remarks], [TaskId], [LastUpdatedAt], [CreatedAt], [FollowerId], [StatusId]) VALUES (1038, N'test follow up', 2090, CAST(N'2020-10-18T10:38:54.4722584' AS DateTime2), CAST(N'2020-10-18T10:38:54.4723972' AS DateTime2), 1, 2)
INSERT [dbo].[TaskFollowUps] ([Id], [Remarks], [TaskId], [LastUpdatedAt], [CreatedAt], [FollowerId], [StatusId]) VALUES (1039, N'wow remarks', 2090, CAST(N'2020-10-18T11:45:33.5359899' AS DateTime2), CAST(N'2020-10-18T11:45:33.5363135' AS DateTime2), 1, 2)
INSERT [dbo].[TaskFollowUps] ([Id], [Remarks], [TaskId], [LastUpdatedAt], [CreatedAt], [FollowerId], [StatusId]) VALUES (1040, N'hello remarks', 2090, CAST(N'2020-10-18T11:48:04.9817952' AS DateTime2), CAST(N'2020-10-18T11:48:04.9817994' AS DateTime2), 1, 1)
INSERT [dbo].[TaskFollowUps] ([Id], [Remarks], [TaskId], [LastUpdatedAt], [CreatedAt], [FollowerId], [StatusId]) VALUES (1041, N'hhwl', 2091, CAST(N'2020-10-18T13:08:51.8357371' AS DateTime2), CAST(N'2020-10-18T13:08:51.8358921' AS DateTime2), 1, 1)
SET IDENTITY_INSERT [dbo].[TaskFollowUps] OFF
SET IDENTITY_INSERT [dbo].[TaskFollowUpStatus] ON 

INSERT [dbo].[TaskFollowUpStatus] ([Id], [Status]) VALUES (1, N'Open')
INSERT [dbo].[TaskFollowUpStatus] ([Id], [Status]) VALUES (2, N'Close')
SET IDENTITY_INSERT [dbo].[TaskFollowUpStatus] OFF
SET IDENTITY_INSERT [dbo].[TaskPriority] ON 

INSERT [dbo].[TaskPriority] ([Id], [Priority]) VALUES (1, N'Low')
INSERT [dbo].[TaskPriority] ([Id], [Priority]) VALUES (2, N'Medium')
INSERT [dbo].[TaskPriority] ([Id], [Priority]) VALUES (3, N'High')
SET IDENTITY_INSERT [dbo].[TaskPriority] OFF
SET IDENTITY_INSERT [dbo].[Tasks] ON 

INSERT [dbo].[Tasks] ([Id], [TaskProgress], [Title], [Description], [Target], [TaskStatusId], [IsDeleted], [TaskPriorityId], [LastUpdatedAt], [CreatedAt], [ProgressRemarks], [Employee]) VALUES (2090, N'41', N'Corporate website handover', N'Corporate website handover', CAST(N'2020-10-18T00:00:00.0000000' AS DateTime2), 2, 0, 2, CAST(N'2020-10-18T11:49:05.9358235' AS DateTime2), CAST(N'2020-10-18T10:35:09.5212177' AS DateTime2), N'test remarks', 2)
INSERT [dbo].[Tasks] ([Id], [TaskProgress], [Title], [Description], [Target], [TaskStatusId], [IsDeleted], [TaskPriorityId], [LastUpdatedAt], [CreatedAt], [ProgressRemarks], [Employee]) VALUES (2091, N'0', N'Corporate website design', N'Corporate website design', CAST(N'2020-10-18T00:00:00.0000000' AS DateTime2), 1, 0, 2, CAST(N'2020-10-18T13:19:14.5896229' AS DateTime2), CAST(N'2020-10-18T13:08:37.5322149' AS DateTime2), NULL, 1)
SET IDENTITY_INSERT [dbo].[Tasks] OFF
SET IDENTITY_INSERT [dbo].[TaskStatus] ON 

INSERT [dbo].[TaskStatus] ([Id], [Status]) VALUES (1, N'Not Started')
INSERT [dbo].[TaskStatus] ([Id], [Status]) VALUES (2, N'In Progress')
INSERT [dbo].[TaskStatus] ([Id], [Status]) VALUES (3, N'Completed')
INSERT [dbo].[TaskStatus] ([Id], [Status]) VALUES (4, N'On Hold')
INSERT [dbo].[TaskStatus] ([Id], [Status]) VALUES (5, N'Cancelled')
SET IDENTITY_INSERT [dbo].[TaskStatus] OFF
/****** Object:  Index [IX_Departments_ManagerId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_Departments_ManagerId] ON [dbo].[Departments]
(
	[ManagerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Departments_ParentDepartmentId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_Departments_ParentDepartmentId] ON [dbo].[Departments]
(
	[ParentDepartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Employees_DepartmentId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_Employees_DepartmentId] ON [dbo].[Employees]
(
	[DepartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SubTasks_LastUpdatedById]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_SubTasks_LastUpdatedById] ON [dbo].[SubTasks]
(
	[LastUpdatedById] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SubTasks_SubTaskAssigneeId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_SubTasks_SubTaskAssigneeId] ON [dbo].[SubTasks]
(
	[SubTaskAssigneeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SubTasks_TaskId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_SubTasks_TaskId] ON [dbo].[SubTasks]
(
	[TaskId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SubTasks_TaskStatusId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_SubTasks_TaskStatusId] ON [dbo].[SubTasks]
(
	[TaskStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TaskAudit_ActionById]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_TaskAudit_ActionById] ON [dbo].[TaskAudit]
(
	[ActionById] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TaskAudit_TaskId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_TaskAudit_TaskId] ON [dbo].[TaskAudit]
(
	[TaskId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TaskAudit_TypeId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_TaskAudit_TypeId] ON [dbo].[TaskAudit]
(
	[TypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TaskEmployees_EmployeeId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_TaskEmployees_EmployeeId] ON [dbo].[TaskEmployees]
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TaskEmployees_TaskCapacityId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_TaskEmployees_TaskCapacityId] ON [dbo].[TaskEmployees]
(
	[TaskCapacityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TaskEmployees_TaskId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_TaskEmployees_TaskId] ON [dbo].[TaskEmployees]
(
	[TaskId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TaskFollowUpResponses_RespondedById]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_TaskFollowUpResponses_RespondedById] ON [dbo].[TaskFollowUpResponses]
(
	[RespondedById] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TaskFollowUpResponses_TaskFollowUpId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_TaskFollowUpResponses_TaskFollowUpId] ON [dbo].[TaskFollowUpResponses]
(
	[TaskFollowUpId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TaskFollowUps_FollowerId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_TaskFollowUps_FollowerId] ON [dbo].[TaskFollowUps]
(
	[FollowerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TaskFollowUps_StatusId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_TaskFollowUps_StatusId] ON [dbo].[TaskFollowUps]
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TaskFollowUps_TaskId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_TaskFollowUps_TaskId] ON [dbo].[TaskFollowUps]
(
	[TaskId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Tasks_Employee]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_Tasks_Employee] ON [dbo].[Tasks]
(
	[Employee] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Tasks_TaskPriorityId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_Tasks_TaskPriorityId] ON [dbo].[Tasks]
(
	[TaskPriorityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Tasks_TaskStatusId]    Script Date: 10/18/2020 2:16:41 PM ******/
CREATE NONCLUSTERED INDEX [IX_Tasks_TaskStatusId] ON [dbo].[Tasks]
(
	[TaskStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[SubTasks] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [CreatedAt]
GO
ALTER TABLE [dbo].[TaskEmployees] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsActive]
GO
ALTER TABLE [dbo].[Departments]  WITH CHECK ADD  CONSTRAINT [FK_Departments_Departments_ParentDepartmentId] FOREIGN KEY([ParentDepartmentId])
REFERENCES [dbo].[Departments] ([Id])
GO
ALTER TABLE [dbo].[Departments] CHECK CONSTRAINT [FK_Departments_Departments_ParentDepartmentId]
GO
ALTER TABLE [dbo].[Departments]  WITH CHECK ADD  CONSTRAINT [FK_Departments_Employees_ManagerId] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[Departments] CHECK CONSTRAINT [FK_Departments_Employees_ManagerId]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Departments_DepartmentId] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([Id])
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Departments_DepartmentId]
GO
ALTER TABLE [dbo].[SubTasks]  WITH CHECK ADD  CONSTRAINT [FK_SubTasks_Employees_LastUpdatedById] FOREIGN KEY([LastUpdatedById])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[SubTasks] CHECK CONSTRAINT [FK_SubTasks_Employees_LastUpdatedById]
GO
ALTER TABLE [dbo].[SubTasks]  WITH CHECK ADD  CONSTRAINT [FK_SubTasks_Employees_SubTaskAssigneeId] FOREIGN KEY([SubTaskAssigneeId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[SubTasks] CHECK CONSTRAINT [FK_SubTasks_Employees_SubTaskAssigneeId]
GO
ALTER TABLE [dbo].[SubTasks]  WITH CHECK ADD  CONSTRAINT [FK_SubTasks_Tasks_TaskId] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Tasks] ([Id])
GO
ALTER TABLE [dbo].[SubTasks] CHECK CONSTRAINT [FK_SubTasks_Tasks_TaskId]
GO
ALTER TABLE [dbo].[SubTasks]  WITH CHECK ADD  CONSTRAINT [FK_SubTasks_TaskStatus_TaskStatusId] FOREIGN KEY([TaskStatusId])
REFERENCES [dbo].[TaskStatus] ([Id])
GO
ALTER TABLE [dbo].[SubTasks] CHECK CONSTRAINT [FK_SubTasks_TaskStatus_TaskStatusId]
GO
ALTER TABLE [dbo].[TaskAudit]  WITH CHECK ADD  CONSTRAINT [FK_TaskAudit_AuditType_TypeId] FOREIGN KEY([TypeId])
REFERENCES [dbo].[AuditType] ([Id])
GO
ALTER TABLE [dbo].[TaskAudit] CHECK CONSTRAINT [FK_TaskAudit_AuditType_TypeId]
GO
ALTER TABLE [dbo].[TaskAudit]  WITH CHECK ADD  CONSTRAINT [FK_TaskAudit_Employees_ActionById] FOREIGN KEY([ActionById])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[TaskAudit] CHECK CONSTRAINT [FK_TaskAudit_Employees_ActionById]
GO
ALTER TABLE [dbo].[TaskAudit]  WITH CHECK ADD  CONSTRAINT [FK_TaskAudit_Tasks_TaskId] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Tasks] ([Id])
GO
ALTER TABLE [dbo].[TaskAudit] CHECK CONSTRAINT [FK_TaskAudit_Tasks_TaskId]
GO
ALTER TABLE [dbo].[TaskEmployees]  WITH CHECK ADD  CONSTRAINT [FK_TaskEmployees_Employees_EmployeeId] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[TaskEmployees] CHECK CONSTRAINT [FK_TaskEmployees_Employees_EmployeeId]
GO
ALTER TABLE [dbo].[TaskEmployees]  WITH CHECK ADD  CONSTRAINT [FK_TaskEmployees_TaskCapacities_TaskCapacityId] FOREIGN KEY([TaskCapacityId])
REFERENCES [dbo].[TaskCapacities] ([Id])
GO
ALTER TABLE [dbo].[TaskEmployees] CHECK CONSTRAINT [FK_TaskEmployees_TaskCapacities_TaskCapacityId]
GO
ALTER TABLE [dbo].[TaskEmployees]  WITH CHECK ADD  CONSTRAINT [FK_TaskEmployees_Tasks_TaskId] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Tasks] ([Id])
GO
ALTER TABLE [dbo].[TaskEmployees] CHECK CONSTRAINT [FK_TaskEmployees_Tasks_TaskId]
GO
ALTER TABLE [dbo].[TaskFollowUpResponses]  WITH CHECK ADD  CONSTRAINT [FK_TaskFollowUpResponses_Employees_RespondedById] FOREIGN KEY([RespondedById])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[TaskFollowUpResponses] CHECK CONSTRAINT [FK_TaskFollowUpResponses_Employees_RespondedById]
GO
ALTER TABLE [dbo].[TaskFollowUpResponses]  WITH CHECK ADD  CONSTRAINT [FK_TaskFollowUpResponses_TaskFollowUps_TaskFollowUpId] FOREIGN KEY([TaskFollowUpId])
REFERENCES [dbo].[TaskFollowUps] ([Id])
GO
ALTER TABLE [dbo].[TaskFollowUpResponses] CHECK CONSTRAINT [FK_TaskFollowUpResponses_TaskFollowUps_TaskFollowUpId]
GO
ALTER TABLE [dbo].[TaskFollowUps]  WITH CHECK ADD  CONSTRAINT [FK_TaskFollowUps_Employees_FollowerId] FOREIGN KEY([FollowerId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[TaskFollowUps] CHECK CONSTRAINT [FK_TaskFollowUps_Employees_FollowerId]
GO
ALTER TABLE [dbo].[TaskFollowUps]  WITH CHECK ADD  CONSTRAINT [FK_TaskFollowUps_TaskFollowUpStatus_StatusId] FOREIGN KEY([StatusId])
REFERENCES [dbo].[TaskFollowUpStatus] ([Id])
GO
ALTER TABLE [dbo].[TaskFollowUps] CHECK CONSTRAINT [FK_TaskFollowUps_TaskFollowUpStatus_StatusId]
GO
ALTER TABLE [dbo].[TaskFollowUps]  WITH CHECK ADD  CONSTRAINT [FK_TaskFollowUps_Tasks_TaskId] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Tasks] ([Id])
GO
ALTER TABLE [dbo].[TaskFollowUps] CHECK CONSTRAINT [FK_TaskFollowUps_Tasks_TaskId]
GO
ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK_Tasks_Employees_Employee] FOREIGN KEY([Employee])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[Tasks] CHECK CONSTRAINT [FK_Tasks_Employees_Employee]
GO
ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK_Tasks_TaskPriority_TaskPriorityId] FOREIGN KEY([TaskPriorityId])
REFERENCES [dbo].[TaskPriority] ([Id])
GO
ALTER TABLE [dbo].[Tasks] CHECK CONSTRAINT [FK_Tasks_TaskPriority_TaskPriorityId]
GO
ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK_Tasks_TaskStatus_TaskStatusId] FOREIGN KEY([TaskStatusId])
REFERENCES [dbo].[TaskStatus] ([Id])
GO
ALTER TABLE [dbo].[Tasks] CHECK CONSTRAINT [FK_Tasks_TaskStatus_TaskStatusId]
GO
/****** Object:  StoredProcedure [dbo].[SqlQueryNotificationStoredProcedure-08f5aa71-2fff-4064-b25a-420fb72bd18a]    Script Date: 10/18/2020 2:16:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SqlQueryNotificationStoredProcedure-08f5aa71-2fff-4064-b25a-420fb72bd18a] AS BEGIN BEGIN TRANSACTION; RECEIVE TOP(0) conversation_handle FROM [SqlQueryNotificationService-08f5aa71-2fff-4064-b25a-420fb72bd18a]; IF (SELECT COUNT(*) FROM [SqlQueryNotificationService-08f5aa71-2fff-4064-b25a-420fb72bd18a] WHERE message_type_name = 'http://schemas.microsoft.com/SQL/ServiceBroker/DialogTimer') > 0 BEGIN if ((SELECT COUNT(*) FROM sys.services WHERE name = 'SqlQueryNotificationService-08f5aa71-2fff-4064-b25a-420fb72bd18a') > 0)   DROP SERVICE [SqlQueryNotificationService-08f5aa71-2fff-4064-b25a-420fb72bd18a]; if (OBJECT_ID('SqlQueryNotificationService-08f5aa71-2fff-4064-b25a-420fb72bd18a', 'SQ') IS NOT NULL)   DROP QUEUE [SqlQueryNotificationService-08f5aa71-2fff-4064-b25a-420fb72bd18a]; DROP PROCEDURE [SqlQueryNotificationStoredProcedure-08f5aa71-2fff-4064-b25a-420fb72bd18a]; END COMMIT TRANSACTION; END
GO
/****** Object:  StoredProcedure [dbo].[SqlQueryNotificationStoredProcedure-9158f8bb-f793-4c9e-adbb-acaebf43711c]    Script Date: 10/18/2020 2:16:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SqlQueryNotificationStoredProcedure-9158f8bb-f793-4c9e-adbb-acaebf43711c] AS BEGIN BEGIN TRANSACTION; RECEIVE TOP(0) conversation_handle FROM [SqlQueryNotificationService-9158f8bb-f793-4c9e-adbb-acaebf43711c]; IF (SELECT COUNT(*) FROM [SqlQueryNotificationService-9158f8bb-f793-4c9e-adbb-acaebf43711c] WHERE message_type_name = 'http://schemas.microsoft.com/SQL/ServiceBroker/DialogTimer') > 0 BEGIN if ((SELECT COUNT(*) FROM sys.services WHERE name = 'SqlQueryNotificationService-9158f8bb-f793-4c9e-adbb-acaebf43711c') > 0)   DROP SERVICE [SqlQueryNotificationService-9158f8bb-f793-4c9e-adbb-acaebf43711c]; if (OBJECT_ID('SqlQueryNotificationService-9158f8bb-f793-4c9e-adbb-acaebf43711c', 'SQ') IS NOT NULL)   DROP QUEUE [SqlQueryNotificationService-9158f8bb-f793-4c9e-adbb-acaebf43711c]; DROP PROCEDURE [SqlQueryNotificationStoredProcedure-9158f8bb-f793-4c9e-adbb-acaebf43711c]; END COMMIT TRANSACTION; END
GO
USE [master]
GO
ALTER DATABASE [TaskManagerDB] SET  READ_WRITE 
GO

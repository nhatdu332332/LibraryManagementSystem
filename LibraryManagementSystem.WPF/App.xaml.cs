using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Data.Entities;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Repositories.Repositories;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Services.Interfaces;
using LibraryManagementSystem.WPF.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagementSystem.WPF
{
	public partial class App : Application
	{
		public static IServiceProvider ServiceProvider { get; private set; } = null!;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var services = new ServiceCollection();
			var basePath = AppContext.BaseDirectory;

			var configuration = new ConfigurationBuilder()
				.SetBasePath(basePath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();

			var connectionString = configuration.GetConnectionString("DefaultConnection");
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				throw new InvalidOperationException(
					"Missing ConnectionStrings:DefaultConnection in appsettings.json or appsettings.Local.json.");
			}

			services.AddDbContext<LibraryDbContext>(
				options => options.UseSqlServer(
					connectionString,
					sqlOptions =>
					{
						sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
						sqlOptions.CommandTimeout(30);
					}),
				ServiceLifetime.Transient);

			services.AddTransient<IUnitOfWork, UnitOfWork>();
			services.AddTransient<IRoleRepository, RoleRepository>();
			services.AddTransient<IEmployeeRepository, EmployeeRepository>();
			services.AddTransient<IReaderRepository, ReaderRepository>();
			services.AddTransient<IBookWorkRepository, BookWorkRepository>();
			services.AddTransient<IBorrowRequestRepository, BorrowRequestRepository>();
			services.AddTransient<IBorrowTransactionRepository, BorrowTransactionRepository>();
			services.AddTransient<IBookCopyRepository, BookCopyRepository>();

			services.AddSingleton<IAuthService, AuthService>();
			services.AddTransient<IBookService, BookService>();
			services.AddTransient<IBorrowService, BorrowService>();
			services.AddTransient<IEmployeeAccountService, EmployeeAccountService>();
			services.AddTransient<IEmployeeService, EmployeeService>();
			services.AddTransient<IReaderAccountService, ReaderAccountService>();
			services.AddTransient<IReaderService, ReaderService>();
			services.AddTransient<IRoleService, RoleService>();

			services.AddTransient<LoginViewModel>();
			services.AddTransient<BookCatalogViewModel>();
			services.AddTransient<BorrowViewModel>();
			services.AddTransient<MyAccountViewModel>();
			services.AddTransient<ManageAccountsViewModel>();
			services.AddTransient<MainViewModel>();
			services.AddTransient<CreateBorrowTransactionViewModel>();
			services.AddTransient<ManageBooksViewModel>();
			services.AddTransient<UpdateBorrowTransactionViewModel>();
			services.AddTransient<BookDetailViewModel>();

			services.AddTransient<MainWindow>();

			ServiceProvider = services.BuildServiceProvider();

			try
			{
				InitializeDatabase(configuration);

				var mainVM = ServiceProvider.GetRequiredService<MainViewModel>();
				var mainWindow = new MainWindow
				{
					DataContext = mainVM
				};
				mainWindow.Show();
			}
			catch (Exception ex)
			{
				var fullMessage =
					$"Application startup failed.{Environment.NewLine}{Environment.NewLine}" +
					$"Base path: {basePath}{Environment.NewLine}" +
					$"{BuildExceptionDetails(ex)}";

				Debug.WriteLine(fullMessage);
				MessageBox.Show(
					fullMessage,
					"Library Management Startup Error",
					MessageBoxButton.OK,
					MessageBoxImage.Error);
				Shutdown();
			}
		}

		private static void InitializeDatabase(IConfiguration configuration)
		{
			using var scope = ServiceProvider.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

			dbContext.Database.EnsureCreated();
			SeedDefaults(dbContext, configuration);
		}

		private static void SeedDefaults(LibraryDbContext dbContext, IConfiguration configuration)
		{
			var requiredRoles = new[]
			{
				("Administrator", "Toan quyen quan tri he thong."),
				("Librarian", "Quan ly quy trinh muon tra sach."),
				("Staff", "Quan ly danh muc va thong tin sach.")
			};

			foreach (var (roleName, description) in requiredRoles)
			{
				if (!dbContext.Roles.Any(role => role.RoleName == roleName))
				{
					dbContext.Roles.Add(new Role
					{
						RoleName = roleName,
						Description = description
					});
				}
			}

			dbContext.SaveChanges();

			var adminEmail = configuration["AdminAccount:Email"];
			var adminPassword = configuration["AdminAccount:Password"];
			if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
			{
				return;
			}

			if (dbContext.Employees.Any(employee => employee.Email == adminEmail))
			{
				return;
			}

			var adminRole = dbContext.Roles.First(role => role.RoleName == "Administrator");
			if (dbContext.Employees.Any(employee => employee.RoleId == adminRole.RoleId))
			{
				return;
			}

			dbContext.Employees.Add(new Employee
			{
				Email = adminEmail,
				PasswordHash = adminPassword,
				FullName = "System Administrator",
				RoleId = adminRole.RoleId,
				HireDate = DateTime.Now,
				Status = "Active"
			});

			dbContext.SaveChanges();
		}

		private static string BuildExceptionDetails(Exception exception)
		{
			var details = new[]
			{
				$"Message: {exception.Message}",
				$"Inner: {exception.InnerException?.Message}"
			};

			var nestedExceptions = exception
				.ToString()
				.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
				.Where(line => line.Contains("Exception", StringComparison.OrdinalIgnoreCase))
				.Take(5);

			return string.Join(Environment.NewLine, details.Concat(nestedExceptions));
		}
	}
}

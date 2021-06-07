using Blog.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Blog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            try
            {
                var scope = host.Services.CreateScope();

                var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>(); //Handles all the accounts
                var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(); //Handles all the roles to be assigned to users

                ctx.Database.EnsureCreated();

                var adminRole = new IdentityRole("Admin");
                var customerRole = new IdentityRole("User");

                if (!ctx.Roles.Any())
                {
                    //create a role
                    roleMgr.CreateAsync(adminRole).GetAwaiter().GetResult();
                    roleMgr.CreateAsync(customerRole).GetAwaiter().GetResult();
                }

                if (!ctx.Users.Any(u => u.UserName == "admin"))
                {
                    //create an Admin
                    var adminUser = new IdentityUser
                    {
                        UserName = "admin",
                        Email = "Member1@email.com"
                    };

                    var result = userMgr.CreateAsync(adminUser, "Password123!").GetAwaiter().GetResult();
                    //adding role to user
                    userMgr.AddToRoleAsync(adminUser, adminRole.Name).GetAwaiter().GetResult();
                }

                for (int i = 1; i < 6; i++){
                    string tempUserName = "Customer";
                    tempUserName += i.ToString();
                    if (!ctx.Users.Any(u => u.UserName == tempUserName))
                    {
                        //create an Admin
                        var customerUser = new IdentityUser
                        {
                            UserName = tempUserName,
                            Email = tempUserName + "@email.com"
                        };

                        var result = userMgr.CreateAsync(customerUser, "Password123!").GetAwaiter().GetResult();
                        //adding role to user
                        userMgr.AddToRoleAsync(customerUser, customerRole.Name).GetAwaiter().GetResult();
                    }

                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}

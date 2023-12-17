using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Identity.API.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
			//Database.EnsureCreated();
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<ApplicationUser>(entity =>
			{
				entity.ToTable(name: "Users");
			});

			builder.Entity<ApplicationRole>(entity =>
			{
				entity.ToTable(name: "Roles");
			});

			builder.Entity<IdentityUserClaim<Guid>>(entity =>
			{
				entity.ToTable("UserClaims");
			});

			builder.Entity<IdentityUserLogin<Guid>>(entity =>
			{
				entity.ToTable("UserLogins");
			});

			builder.Entity<IdentityRoleClaim<Guid>>(entity =>
			{
				entity.ToTable("RoleClaims");
			});

			builder.Entity<IdentityUserRole<Guid>>(entity =>
			{
				entity.ToTable("UserRoles");
			});

			builder.Entity<IdentityUserToken<Guid>>(entity =>
			{
				entity.ToTable("UserTokens");
			});
		}
	}
}

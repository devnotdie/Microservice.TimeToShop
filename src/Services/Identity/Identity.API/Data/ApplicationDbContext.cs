using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.EntityFramework.Extensions;
using Duende.IdentityServer.EntityFramework.Interfaces;
using Duende.IdentityServer.EntityFramework.Options;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Reflection.Emit;

namespace Identity.API.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IConfigurationDbContext, IPersistedGrantDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
			Database.EnsureCreated();
		}

		public DbSet<Client> Clients { get; set; }

		public DbSet<ClientCorsOrigin> ClientCorsOrigins { get; set; }

		public DbSet<IdentityResource> IdentityResources { get; set; }

		public DbSet<ApiResource> ApiResources { get; set; }

		public DbSet<ApiScope> ApiScopes { get; set; }

		public DbSet<IdentityProvider> IdentityProviders { get; set; }

		public DbSet<PersistedGrant> PersistedGrants { get; set; }

		public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

		public DbSet<Key> Keys { get; set; }

		public DbSet<ServerSideSession> ServerSideSessions { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			var operationalStoreOptions = this.GetService<OperationalStoreOptions>();
			var configurationStoreOptions  = this.GetService<ConfigurationStoreOptions>();

			builder.ConfigurePersistedGrantContext(operationalStoreOptions);
			builder.ConfigureClientContext(configurationStoreOptions);
			builder.ConfigureResourcesContext(configurationStoreOptions);
			builder.ConfigureIdentityProviderContext(configurationStoreOptions);

			base.OnModelCreating(builder);

			builder.Entity<ApplicationUser>(entity =>
			{
				entity.ToTable(name: "Users");
			});

			builder.Entity<IdentityRole>(entity =>
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

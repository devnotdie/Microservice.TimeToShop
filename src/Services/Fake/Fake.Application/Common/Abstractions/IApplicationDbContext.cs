namespace Fake.Application.Common.Abstractions
{
	public interface IApplicationDbContext
	{
		Task SaveChangesAsync();
	}
}

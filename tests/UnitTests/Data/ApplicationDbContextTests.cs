namespace UnitTests.Data;

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using ConferencePlanner.Data;
using ConferencePlanner.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

using Xunit.Categories;

[UnitTest(nameof(ApplicationDbContext))]
public class ApplicationDbContextTests
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApplicationDbContextTests" /> class.
    /// </summary>
    public ApplicationDbContextTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("conference");

        DbContextOptions<ApplicationDbContext> options = optionsBuilder.Options;

        this.Target = new ApplicationDbContext(options);
    }

    private ApplicationDbContext Target { get; }

    [Fact]
    public void SpeakersShouldBeWellBehaved()
    {
        this.CheckDbSetProperty<Speaker>(() => this.Target.Speakers);
        PropertyChecker.CheckInvariance(() => this.Target.Speakers);
    }

    [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Test code")]
    private void CheckDbSetProperty<T>(Expression<Func<object?>> expression)
        where T : class
    {
        DbSet<T> expectedDefault = new InternalDbSet<T>(this.Target, null);

        PropertyChecker.CheckProperty<DbSet<T>>(expression, testDefaultEquivalence: true, expectedDefaultValue: expectedDefault);
    }
}
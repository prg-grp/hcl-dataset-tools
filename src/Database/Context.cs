using System.Text.Json;
using System.Text.Json.Serialization;

using Database.Models;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Database;

public class DatasetContext : DbContext
{
    private const string LocalDbPath = "../../data";

    private static readonly string DbPath = Path.Join(Environment.GetEnvironmentVariable("DB_DIR") ?? LocalDbPath,
        "dataset.sqlite");

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        PropertyNameCaseInsensitive = true,
    };

    public DbSet<Repository> Repositories { get; set; }

    public DbSet<Module> Modules { get; set; }

    public DbSet<Resource> Resources { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<Module>()
            .HasOne<Repository>();

        modelBuilder
            .Entity<Module>()
            .Property(m => m.ModuleCalls)
            .HasConversion(
                arr => JsonSerializer.Serialize(arr, SerializerOptions),
                str => JsonSerializer.Deserialize<(string Name, string Source)[]>(str, SerializerOptions) ??
                       Array.Empty<(string, string)>())
            .Metadata.SetValueComparer(new ValueComparer<(string, string)[]>(
                (l, r) => (l ?? Array.Empty<(string, string)>()).SequenceEqual(r ?? Array.Empty<(string, string)>()),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c));

        modelBuilder
            .Entity<Module>()
            .HasIndex(m => new { m.RepositoryId, m.Path })
            .IsUnique();

        modelBuilder
            .Entity<Resource>()
            .HasOne<Module>();

        modelBuilder
            .Entity<Resource>()
            .Property(r => r.ResourceType)
            .HasConversion<EnumToStringConverter<ResourceType>>();

        modelBuilder
            .Entity<Resource>()
            .HasIndex(r => new { r.Type, r.Name });

        modelBuilder
            .Entity<Resource>()
            .HasIndex(r => new { r.Provider, r.Type, r.Name });

        modelBuilder
            .Entity<Resource>()
            .HasIndex(r => new { r.Provider, r.Type });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite(new SqliteConnectionStringBuilder
        {
            DataSource = DbPath, Mode = SqliteOpenMode.ReadWriteCreate, ForeignKeys = true,
        }.ToString()).UseEnumCheckConstraints();
}

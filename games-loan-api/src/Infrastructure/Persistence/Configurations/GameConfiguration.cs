using GamesLoan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Infrastructure.Persistence.Configurations;
public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("games");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(g => g.Name)
            .HasColumnName("name")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(g => g.Publishers)
            .HasColumnName("publishers")
            .HasMaxLength(100);

        builder.Property(g => g.Genre)
            .HasColumnName("genre")
            .HasMaxLength(150);

        builder.Property(g => g.ExternalSourceId)
            .HasColumnName("external_source_id")
            .HasMaxLength(100);

        builder.Property(g => g.IsLoaned)
            .HasColumnName("is_loaned")
            .IsRequired();

        builder.Property(g => g.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(g => g.ExternalSourceId)
            .HasDatabaseName("IX_games_external_source_id");
    }
}

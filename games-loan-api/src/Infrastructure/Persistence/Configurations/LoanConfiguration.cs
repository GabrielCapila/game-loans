using GamesLoan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Infrastructure.Persistence.Configurations;
public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("loans");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(l => l.GameId)
            .HasColumnName("game_id")
            .IsRequired();

        builder.Property(l => l.FriendId)
            .HasColumnName("friend_id")
            .IsRequired();

        builder.Property(l => l.LoanDate)
            .HasColumnName("loan_date")
            .IsRequired();

        builder.Property(l => l.ExpectedReturnDate)
            .HasColumnName("expected_return_date");

        builder.Property(l => l.ActualReturnDate)
            .HasColumnName("return_date");

        builder.Property(l => l.Status)
            .HasColumnName("status")
            .HasConversion<int>() 
            .IsRequired();

        builder.HasIndex(l => l.GameId)
            .HasDatabaseName("IX_loans_game_id");

        builder.HasIndex(l => l.FriendId)
            .HasDatabaseName("IX_loans_friend_id");
    }
}

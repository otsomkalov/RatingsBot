﻿using Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.Configuration;

public class CategoryConfiguration : BaseEntityConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasMany(c => c.Items)
            .WithOne(i => i.Category)
            .HasForeignKey(i => i.CategoryId);

        base.Configure(builder);
    }
}
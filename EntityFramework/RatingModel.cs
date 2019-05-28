namespace WpfApp3.EntityFramework
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class RatingModel : DbContext
    {
        public RatingModel()
            : base("name=RatingModel")
        {
        }

        public virtual DbSet<item> item { get; set; }
        public virtual DbSet<occupation> occupation { get; set; }
        public virtual DbSet<user> user { get; set; }
        public virtual DbSet<ratings> ratings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<item>()
                .HasMany(e => e.ratings)
                .WithRequired(e => e.item)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<occupation>()
                .HasMany(e => e.user)
                .WithRequired(e => e.occupation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<user>()
                .Property(e => e.gender)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<user>()
                .HasMany(e => e.ratings)
                .WithRequired(e => e.user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ratings>()
                .Property(e => e.timestamp)
                .IsFixedLength();
        }
    }
}

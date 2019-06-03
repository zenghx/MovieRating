namespace MovieRating.EntityFramework
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ratings
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short userId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int movieId { get; set; }

        [Key]
        [Column("rating", Order = 2)]
        public byte rating { get; set; }

        [Key]
        [Column(Order = 3, TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        public virtual item item { get; set; }

        public virtual user user { get; set; }
    }
}

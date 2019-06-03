namespace MovieRating.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("item")]
    public partial class item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public item()
        {
            ratings = new HashSet<ratings>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int movieId { get; set; }

        [Required]
        [StringLength(100)]
        public string movieTitle { get; set; }

        [Column(TypeName = "date")]
        public DateTime? releaseDate { get; set; }

        [StringLength(19)]
        public string genre { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ratings> ratings { get; set; }
    }
}

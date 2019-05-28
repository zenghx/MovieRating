namespace WpfApp3.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

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

        [StringLength(150)]
        public string IMDBURL { get; set; }

        public bool isUnkown { get; set; }

        public bool isAction { get; set; }

        public bool isAdventure { get; set; }

        public bool IsAnimation { get; set; }

        [Column("isChildren's")]
        public bool isChildren_s { get; set; }

        public bool isComedy { get; set; }

        public bool isCrime { get; set; }

        public bool isDocumentary { get; set; }

        public bool isDrama { get; set; }

        public bool isFantasy { get; set; }

        [Column("isFilm-Noir")]
        public bool isFilm_Noir { get; set; }

        public bool isHorror { get; set; }

        public bool isMusical { get; set; }

        public bool isMystery { get; set; }

        public bool isRomance { get; set; }

        [Column("isSci-Fi")]
        public bool isSci_Fi { get; set; }

        public bool isThriller { get; set; }

        public bool isWar { get; set; }

        public bool isWestern { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ratings> ratings { get; set; }
    }
}

namespace WpfApp3.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("user")]
    public partial class user
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public user()
        {
            ratings = new HashSet<ratings>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short userId { get; set; }

        public byte age { get; set; }

        [Required]
        [StringLength(1)]
        public string gender { get; set; }

        public byte occupationId { get; set; }

        [Required]
        [StringLength(5)]
        public string zipcode { get; set; }

        public virtual occupation occupation { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ratings> ratings { get; set; }
    }
}

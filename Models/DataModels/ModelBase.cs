using System.ComponentModel.DataAnnotations;

namespace IDMSWebServer.Models.DataModels
{
    public class ModelBase
    {
        [Required]
        [Key]
        public DateTime datetime { get; set; }
    }
}

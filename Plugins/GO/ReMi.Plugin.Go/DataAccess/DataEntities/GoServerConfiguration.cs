using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ReMi.Plugin.Go.DataAccess.Setup;

namespace ReMi.Plugin.Go.DataAccess.DataEntities
{
    [Table("GoServerConfiguration", Schema = Constants.Schema)]
    public class GoServerConfiguration
    {
        [Key]
        public int GoServerConfigurationId { get; set; }

        [Index(IsUnique = true), StringLength(256)]
        public string Name { get; set; }

        public string Value { get; set; }

        public int GlobalConfigurationId { get; set; }
        public virtual GlobalConfiguration GlobalConfiguration { get; set; }
    }
}

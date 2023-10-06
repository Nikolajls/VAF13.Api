using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAF13.Klubadmin.Domain.Configurations
{
  public class ApiConfiguration
  {
      public const string ConfigurationSectionName = "ApiConfiguration";
      public string APIKey { get; set; } = string.Empty;

  }
}

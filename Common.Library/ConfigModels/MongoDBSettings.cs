using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library.ConfigModels
{
    public class MongoDBSettings
    {
        public string DatabaseName { get; set; }
        public string UserCollectionName { get; set; }
        public string SessionCollectionName { get; set; }
    }
}

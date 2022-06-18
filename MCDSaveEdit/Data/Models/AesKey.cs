using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCDSaveEdit.Data
{
    public struct AesKey
    {
        public string key;
        public string versions;
        public AesKey(string key, string versions)
        {
            this.key = key;
            this.versions = versions;
        }
    }
}

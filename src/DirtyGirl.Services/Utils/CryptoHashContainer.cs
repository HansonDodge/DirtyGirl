using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Services.Utils
{
    class CryptoHashContainer
    {
        public string hash { get; set; }
        public string salt { get; set; }

        public CryptoHashContainer(string salt, string hash)
        {
            this.salt = salt;
            this.hash = hash;
        }

        public CryptoHashContainer() { }
    }
}

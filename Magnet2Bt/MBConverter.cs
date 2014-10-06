using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magnet2Bt
{
    public static class MBConverter
    {
        private const string MAGNET_BT_URL = @"http://bt.box.n0808.com/{0}{1}/{2}{3}/{4}.torrent";

        public static string Convert(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentNullException("hash");

            char f1;
            char f2;
            char b1;
            char b2;

            int length = hash.Length;

            f1 = hash[0];
            f2 = hash[1];
            b1 = hash[length - 2];
            b2 = hash[length - 1];

            return string.Format(MAGNET_BT_URL, f1, f2, b1, b2, hash);
        }
    }
}

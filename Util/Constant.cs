using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHolizontalBookViewerLight.Util {
    static class Constant {
        /// <summary>
        /// max count of recent read file count
        /// </summary>
        public static readonly int RecentFilesMaxCount = 20;

        public static readonly string CacheDir = MyLib.Util.Common.GetAppPath() + @"Cache\";
    }
}

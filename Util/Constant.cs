namespace MyHolizontalBookViewerLight.Util {
    static class Constant {
        /// <summary>
        /// max count of recent read file count
        /// </summary>
        public static readonly int RecentFilesMaxCount = 20;

        /// <summary>
        /// extract data cache
        /// </summary>
        public static readonly string CacheDir = MyLib.Util.Common.GetAppPath() + @"Cache\";

        /// <summary>
        /// meta file name
        /// </summary>
        public static readonly string MetaJson = "meta.json";

        
        /// <summary>
        /// get cache file dir
        /// </summary>
        /// <param name="cacheDir"></param>
        /// <returns></returns>
        public static string CasheMeta(string cacheDir) {
            return CacheDir + cacheDir + @"\" +  MetaJson;
        }

    }
}

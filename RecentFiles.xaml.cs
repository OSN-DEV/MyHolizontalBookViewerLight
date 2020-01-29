using MyHolizontalBookViewerLight.Data;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using MyHolizontalBookViewerLight.Util;

namespace MyHolizontalBookViewerLight {
    /// <summary>
    /// 目次
    /// </summary>
    public  partial class RecentFiles : Window {

        #region Public Method
        public AppData.RecentFile RecentFile { set; get; }
        # endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        internal RecentFiles(Window owner) {
            InitializeComponent();
            this.Owner = owner;
            this.cRecentFileList.DataContext = this.CreateList();

            this.KeyDown += (sender, e) => {
                if (e.Key == Key.Escape) {
                    e.Handled = true;
                    this.Close();
                }
            };
        }
        #endregion

        #region Public Property
        internal int Index { private set; get; }
        #endregion

        #region Event
        private void RecentFileList_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var model = (AppData.RecentFile)this.cRecentFileList.SelectedItem;
            if (null != model) {
                this.RecentFile = model;
                this.DialogResult = true;
            }
        }
        #endregion

        #region Private Method
        /// <summary>
        /// create list data
        /// </summary>
        /// <returns></returns>
        private List<AppData.RecentFile> CreateList() {
            var list = new List<AppData.RecentFile>();
            var meta = new MetaOperator();
            foreach (var file in AppData.GetInstance().RecentFiles) {
                meta.MetaFile = (0 < file.CacheDir.Length) ? Constant.CasheMeta(file.CacheDir) : file.FilePath;
                if (!meta.ParseMeta()) {
                    continue;
                }
                list.Add(new AppData.RecentFile() {
                    FilePath = file.FilePath,
                    HBVFilePath = file.HBVFilePath,
                    CacheDir = file.CacheDir,
                    DisplayName = meta.Title
                });
            }
            return list;
        }
        #endregion
    }
}

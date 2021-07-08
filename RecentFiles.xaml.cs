using MyHolizontalBookViewerLight.Data;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using MyHolizontalBookViewerLight.Util;
using System.Windows.Controls;

namespace MyHolizontalBookViewerLight {
    /// <summary>
    /// 目次
    /// </summary>
    public  partial class RecentFiles : Window {

        #region Public Method
        public AppData.RecentFile RecentFile { set; get; }
        private List<AppData.RecentFile> _list;
        # endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        internal RecentFiles(Window owner) {
            InitializeComponent();
            this.Owner = owner;
            this._list = this.CreateList();
            this.cRecentFileList.DataContext = this._list;

            this.Loaded += (sender, e) => {
                this.cRecentFileList.SelectedIndex = 0;
                this.cRecentFileList.Focus();
                var item = (ListViewItem)(this.cRecentFileList.ItemContainerGenerator.ContainerFromItem(cRecentFileList.SelectedItem));
                item.Focus();
            };

            this.KeyDown += (sender, e) => {
                if (e.Key == Key.Escape) {
                    e.Handled = true;
                    this.Close();
                } else if (e.Key == Key.Delete) {
                    e.Handled = true;
                    var item = this.cRecentFileList.SelectedItem as AppData.RecentFile;
                    this._list.Remove(item);
                    this.cRecentFileList.Items.Refresh();


                    var appData = AppData.GetInstance();
                    foreach (var file in appData.RecentFiles) {
                        if (file.FilePath == item.FilePath) {
                            appData.RecentFiles.Remove(file);
                            break;
                        }
                    }
                    appData.Save();
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
            var appData = AppData.GetInstance();
            var delteFiles = new List<AppData.RecentFile>();
            foreach (var file in appData.RecentFiles) {
                meta.MetaFile = (0 < file.CacheDir.Length) ? Constant.CasheMeta(file.CacheDir) : file.FilePath;
                if (!meta.ParseMeta()) {
                    delteFiles.Add(file);
                    continue;
                }
                list.Add(new AppData.RecentFile() {
                    FilePath = file.FilePath,
                    HBVFilePath = file.HBVFilePath,
                    CacheDir = file.CacheDir,
                    DisplayName = meta.Title
                });
            }

            foreach(var file in delteFiles) {
                appData.RecentFiles.Remove(file);
            }
            if (0 < delteFiles.Count) {
                appData.Save();
            }
            
            return list;
        }
        #endregion
    }
}

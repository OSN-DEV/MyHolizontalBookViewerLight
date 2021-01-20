using MyHolizontalBookViewerLight.Data;
using MyHolizontalBookViewerLight.Util;
using MyLib.File;
using MyLib.Util;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using static MyHolizontalBookViewerLight.Data.AppData;

namespace MyHolizontalBookViewerLight {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {

        #region Declaration
        private readonly MetaOperator _operator = new MetaOperator();
        private readonly AppData _appData = AppData.GetInstance();
        private bool _loading = false;
        #endregion

        #region Constructor
        public MainWindow() {
            InitializeComponent();
            this.Initialize();
        }
        #endregion

        #region Event
        /// <summary>
        /// キー入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e) {
            if (this._loading) {
                e.Handled = true;
                return;
            }

            switch (e.Key) {
                case Key.R:
                    if (Common.IsModifierPressed(ModifierKeys.Control) && Common.IsModifierPressed(ModifierKeys.Shift)) {
                        e.Handled = true;
                        var index = this._operator.Index;
                        this._operator.ParseMeta();
                        this._operator.MoveTo(index);
                        this.ShowPage();
                    } else if (Common.IsModifierPressed(ModifierKeys.Control)) {
                        e.Handled = true;
                        this.cViewer.Refresh();
                    }
                    break;

                case Key.Right:
                case Key.F:
                    e.Handled = true;
                    if (this._operator.MoveToNext()) {
                        this.ShowPage();
                        this._appData.RecentFiles[0].LastIndex = this._operator.Index;
                        this._appData.Save();
                    }
                    break;

                case Key.Left:
                case Key.B:
                    e.Handled = true;
                    if (this._operator.MoveToPrevious()) {
                        this.ShowPage();
                        this._appData.RecentFiles[0].LastIndex = this._operator.Index;
                        this._appData.Save();
                    }
                    break;

                case Key.O:
                    e.Handled = true;
                    var tocDialog = new TocWindow(this, this._operator.Toc, this._operator.Index);
                    if (true == tocDialog.ShowDialog()) {
                        this._operator.Index = tocDialog.Index;
                        this.ShowPage();
                        this._appData.RecentFiles[0].LastIndex = this._operator.Index;
                        this._appData.Save();
                    }
                    break;

                case Key.P:
                    e.Handled = true;
                    var fileName = new MyLib.File.FileOperator(this._operator.CurrentPage);
                    if (Common.IsModifierPressed(ModifierKeys.Shift)) {
                        MessageBox.Show(fileName.Name);
                    } else {
                        Clipboard.SetText(fileName.Name, TextDataFormat.Text);
                    }
                    break;

                case Key.Q:
                    e.Handled = true;
                    var recentFilesDialog = new RecentFiles(this);
                    if (true == recentFilesDialog.ShowDialog()) {
                        var recentFile = recentFilesDialog.RecentFile;
                        if (0 < recentFile.CacheDir.Length) {
                            this.ShowBook(Constant.CasheMeta(recentFile.CacheDir), recentFile.HBVFilePath, recentFile.CacheDir);
                        } else {
                            this.ShowBook(recentFile.FilePath);
                        }
                    }
                    break;

                case Key.J:
                    if(Common.IsModifierPressed(ModifierKeys.Control)) {
                        e.Handled = true;
                        var jumpPageDialog = new JumpPage(this, this._operator.Index);
                        if (true == jumpPageDialog.ShowDialog()) {
                            var index = jumpPageDialog.Index;
                            if (0<=index) {
                                this._operator.Index = jumpPageDialog.Index;
                                this.ShowPage();
                                this._appData.RecentFiles[0].LastIndex = this._operator.Index;
                                this._appData.Save();
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// ウィンドウクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this._appData.WindowPosX = this.Left;
            this._appData.WindowPosY = this.Top;
            this._appData.WindowSizeW = this.Width;
            this._appData.WindowSizeH = this.Height;
            this._appData.Save();
        }

        /// <summary>
        /// navigate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Viewer_Navigating(object sender, NavigatingCancelEventArgs e) {
            if (!e.Uri.IsFile) {
                e.Cancel = true;
                MyLib.Util.Common.RunApplication(e.Uri.ToString(), false);
                return;
            }

            var file = FileOperator.Create(e.Uri.LocalPath);
            if (file.IsDirectory) {
                e.Cancel = true;
                return;
            }


            if (file.Name.EndsWith(Constant.MetaJson)) {
                e.Cancel = true;
                this.ShowBook(e.Uri.LocalPath + Uri.UnescapeDataString(e.Uri.Fragment));
            } else  if (file.Extension == "hbv") {
                e.Cancel = true;
                var recentFile = this.ContainsExtractFiles(file.FilePath);
                if (null != recentFile && System.IO.File.Exists(Constant.CasheMeta(recentFile.CacheDir))) {
                    this.ShowBook(Constant.CasheMeta(recentFile.CacheDir), recentFile.HBVFilePath, recentFile.CacheDir);
                } else {
                    if (null != recentFile &&  0 < recentFile.CacheDir.Length) {
                        this._appData.RecentFiles.Remove(recentFile);
                        this._appData.Save();
                    }
                    var cacheDir = DateTime.Now.ToString("yyyyMMddHHMMss");
                    var dialog = new ExtractWindow(this);
                    dialog.HBVFile = file.FilePath;
                    dialog.DestDir = Constant.CacheDir + cacheDir;
                    if (true == dialog.ShowDialog()) {
                        this.ShowBook(Constant.CasheMeta(cacheDir), file.FilePath, cacheDir);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Viewer_LoadCompleted(object sender, NavigationEventArgs e) {
            this._loading = false;
            if (null == e.Uri) {
                return;
            }
        }
        #endregion

        #region Private Method
        /// <summary>
        /// show book
        /// </summary>
        /// <param name="metaFile"></param>
        private void ShowBook(string metaFile, string hbvFile = "", string cacheDir = "") {
            this._operator.MetaFile = metaFile;
            if (!this._operator.ParseMeta()) {
                MessageBox.Show("fail to parse metadata");
                return;
            }
            this.AddRecentfile(this._operator.MetaFile, hbvFile, cacheDir);
            this._appData.LastScrollTop = 0;
            this._appData.Save();
            this.ShowPage();
        }

        /// <summary>
        /// 現在のページを表示
        /// </summary>
        private void ShowPage() {
            this.Title = this._operator.TitleWithPage;
            if (System.IO.File.Exists(this._operator.CurrentPage)) {
                this._loading = true;
                this.cViewer.Navigate(new Uri(this._operator.CurrentPage));
            } else {

            }
        }

        /// <summary>
        /// 最近使ったファイルを追加
        /// </summary>
        /// <param name="file">追加するファイル</param>
        /// <param name="hbvFile">アーカイブ</param>
        /// <param name="cacheDir">キャッシュフォルダ</param>
        private void AddRecentfile(string file, string hbvFile = "", string cacheDir = "") {
            var recentFiles = this._appData.RecentFiles;
            var lastIndex = 0;
            foreach (var recentFile in recentFiles) {
                if (recentFile.FilePath == file) {
                    lastIndex = recentFile.LastIndex;
                    recentFiles.Remove(recentFile);
                    hbvFile = recentFile.HBVFilePath;
                    cacheDir = recentFile.CacheDir;
                    break;
                }
            }
            recentFiles.Insert(0, new AppData.RecentFile() {
                FilePath = file,
                LastIndex = lastIndex,
                HBVFilePath = hbvFile,
                CacheDir = cacheDir
            });
            if (Constant.RecentFilesMaxCount < recentFiles.Count) {
                recentFiles.RemoveRange(Constant.RecentFilesMaxCount, recentFiles.Count - Constant.RecentFilesMaxCount);
            }
            this._operator.Index = lastIndex;
            this._appData.Save();
        }

        /// <summary>
        /// initialize window
        /// </summary>
        private void Initialize() {
            // set event
            this.cViewer.Navigating += Viewer_Navigating;
            this.cViewer.LoadCompleted += Viewer_LoadCompleted;

            // restore window
            double x = Common.GetWindowPosition(this._appData.WindowPosX, this._appData.WindowSizeW, SystemParameters.VirtualScreenWidth);
            double y = Common.GetWindowPosition(this._appData.WindowPosY, this._appData.WindowSizeH, SystemParameters.VirtualScreenHeight);
            if (0 <= x) {
                this.Left = x;
            }
            if (0 <= y) {
                this.Top = y;
            }
            this.Width = Common.GetWindowSize(this.Width, this._appData.WindowSizeW, SystemParameters.WorkArea.Width);
            this.Height = Common.GetWindowSize(this.Height, this._appData.WindowSizeH, SystemParameters.WorkArea.Height);

            // Create CacheDir
            new DirectoryOperator(Constant.CacheDir).Create();

            // clean unused cached file
            this.CleanUnUsedHbvCache();

            // show last books
            if (0 < this._appData.RecentFiles.Count) {
                var recentFile = this._appData.RecentFiles[0];
                if (System.IO.File.Exists(recentFile.FilePath)) {
                    this._operator.MetaFile = recentFile.FilePath;
                    if (!this._operator.ParseMeta()) {
                        this._appData.RecentFiles.RemoveAt(0);
                        this._appData.Save();
                    } else {
                        if (-1 < recentFile.LastIndex) {
                            this._operator.Index = recentFile.LastIndex;
                        } else {
                            this._operator.Index = 0;
                        }
                        this.ShowPage();
                    }
                } else {
                    this._appData.RecentFiles.RemoveAt(0);
                    this._appData.Save();
                }
            }
        }


        /// <summary>
        /// 最近使ったファイルリストに指定したHBVが含まれるか判定
        /// </summary>
        /// <param name="hbv">対象となるHBVファイル</param>
        /// <returns>true:含まれる、false:含まれない</returns>
        internal RecentFile ContainsExtractFiles(string hbv) {
            foreach(var file in this._appData.RecentFiles) {
                if (file.HBVFilePath == hbv) {
                    return file;
                }
            }
            return null;
        }

        /// <summary>
        /// delete unrelated hbv cached files
        /// </summary>
        private void CleanUnUsedHbvCache() {
            var rootInfo = new DirectoryInfo(Constant.CacheDir);
            var dirList = rootInfo.EnumerateDirectories();
            foreach (DirectoryInfo dir in dirList) {
                var target = new DirectoryOperator(dir.FullName);
                var found = false;
                foreach(var file  in this._appData.RecentFiles) {
                    if (file.CacheDir == target.Name) {
                        found = true;
                        break;
                    }
                }
                if (!found ) {
                    target.Delete();
                }
            }
        }


        #endregion

    }
}

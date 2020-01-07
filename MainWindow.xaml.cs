using MyHolizontalBookViewerLight.Data;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

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
            this.cViewer.Navigating += Viewer_Navigating;
            this.cViewer.LoadCompleted += Viewer_LoadCompleted;

            if (0 <= this._appData.WindowPosX && (this._appData.WindowPosX + this._appData.WindowSizeW) < SystemParameters.VirtualScreenWidth) {
                this.Left = this._appData.WindowPosX;
            }
            if (0 <= this._appData.WindowPosY && (this._appData.WindowPosY + this._appData.WindowSizeH) < SystemParameters.VirtualScreenHeight) {
                this.Top = this._appData.WindowPosY;
            }
            if (0 < this._appData.WindowSizeW && this._appData.WindowSizeW <= SystemParameters.WorkArea.Width) {
                this.Width = this._appData.WindowSizeW;
            }
            if (0 < this._appData.WindowSizeH && this._appData.WindowSizeH <= SystemParameters.WorkArea.Height) {
                this.Height = this._appData.WindowSizeH;
            }
            if (0 < this._appData.RecentFiles.Count) {
                var recentFile = this._appData.RecentFiles[0];
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
            }
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
                    if (this.IsModifierPressed(ModifierKeys.Control) && this.IsModifierPressed(ModifierKeys.Shift)) {
                        e.Handled = true;
                        var index = this._operator.Index;
                        this._operator.ParseMeta();
                        this._operator.MoveTo(index);
                        this.ShowPage();
                    } else if (this.IsModifierPressed(ModifierKeys.Control)) {
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
                    var dialog = new TocWindow(this, this._operator.Toc, this._operator.Index);
                    if (true == dialog.ShowDialog()) {
                        this._operator.Index = dialog.Index;
                        this.ShowPage();
                        this._appData.RecentFiles[0].LastIndex = this._operator.Index;
                        this._appData.Save();
                    }
                    break;

                case Key.P:
                    e.Handled = true;
                    var fileName = new MyLib.File.FileOperator(this._operator.CurrentPage);
                    Clipboard.SetText(fileName.Name, TextDataFormat.Text);
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
                return;
            }
            if (!e.Uri.AbsolutePath.EndsWith(@"/meta.json")) {
                return;
            }

            e.Cancel = true;
            this._operator.MetaFile = e.Uri.LocalPath + Uri.UnescapeDataString(e.Uri.Fragment);
            if (!this._operator.ParseMeta()) {
                MessageBox.Show("fail to parse metadata");
                return;
            }
            this.AddRecentfile(this._operator.MetaFile);
            this._appData.LastScrollTop = 0;
            this._appData.Save();
            this.ShowPage();
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

            //            var doc = (MSHTML.HTMLDocument)this.cViewer.Document;
            //this.Document.GetElementsByTagName("HTML")[0].ScrollTop = this._scrollTop;
            //this._scrollTop = -1;
        }

        #endregion



        //public void SaveScrollTop() {
        //    //            this._scrollTop = this.Document.Body.ScrollTop;
        //    try {
        //        this._scrollTop = this.Document.GetElementsByTagName("HTML")[0].ScrollTop;
        //    } catch (Exception ex) {
        //        Console.WriteLine(ex.Message);
        //        this._scrollTop = -1;
        //    }
        //}

        #region Private Method
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
        private void AddRecentfile(string file) {
            var lastIndex = 0;
            var recentFiles = this._appData.RecentFiles;
            foreach(var recentFile in recentFiles) {
                if (recentFile.FilePath == file) {
                    lastIndex = recentFile.LastIndex;
                    recentFiles.Remove(recentFile);
                    break;
                }
            }
            recentFiles.Insert(0, new AppData.RecentFile() {
                FilePath = file,
                LastIndex = lastIndex
            });
            this._appData.Save();
        }

        /// <summary>
        /// check if modifiered key is pressed
        /// </summary>
        /// <param name="key">modifier key</param>
        /// <returns>true:modifiered key is pressed, false:otherwise</returns>
        private bool IsModifierPressed(ModifierKeys key) {
            return (Keyboard.Modifiers & key) != ModifierKeys.None;
        }
        #endregion

    }
}

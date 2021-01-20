using MyLib.File;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyHolizontalBookViewerLight.Data {

    /// <summary>
    /// メタ情報操作クラス
    /// </summary>
    internal class MetaOperator {

        #region Declaration
        internal class TocModelEx : MetaModel.TocModel {
            internal int Index { set; get; } = -1;
            public TocModelEx(MetaModel.TocModel model) {
                this.Level = model.Level;
                this.Content = model.Content;
                this.Link = model.Link;
            }
        }

        private string _rootDir = "";
        private string _contentDir = "";
        private readonly List<string> _pages = new List<string>();
        #endregion

        #region Constructor
        internal MetaOperator() {
        }
        #endregion

        #region Public Property
        /// <summary>
        /// メタ情報
        /// </summary>
        internal string MetaFile { set; get; } = "";

        /// <summary>
        /// 書籍のタイトル
        /// </summary>
        internal string Title { private set; get; }

        /// <summary>
        /// 書籍のタイトル(ページあり)
        /// </summary>
        internal string TitleWithPage {
            get {
                return $"{this.Title}({this.Index + 1}/{this._pages.Count})";
            }
        }

        /// <summary>
        /// 現在のインデックス
        /// </summary>
        internal int Index { set; get; }

        /// <summary>
        /// 現在のページのURL
        /// </summary>
        internal string CurrentPage {
            get {
                if (this._pages.Count <= this.Index) {
                    this.Index = this._pages.Count - 1;
                }
                return this._contentDir + this._pages[this.Index];
            }
        }

        /// <summary
        /// 
        /// 目次
        /// </summary>
        internal List<TocModelEx> Toc { private set; get; } = new List<TocModelEx>();
        #endregion

        #region Public Method
        /// <summary>
        /// メタファイルをパース
        /// </summary>
        /// <returns>true:パース成功、false:それ以外</returns>
        internal bool ParseMeta() {
            bool result = false;
            this.Index = 0;
            this._rootDir = "";
            this._contentDir = "";
            this._pages.Clear();
            this.Toc.Clear();
            try {
                var model = JsonConvert.DeserializeObject<MetaModel>(File.ReadAllText(this.MetaFile));
                this._rootDir = new FileOperator(this.MetaFile).FileDir + @"\";
                this._contentDir = this._rootDir + this.ConvertWinPath(model.ContentDir) + @"\";
                this.Title = model.Title;
                //if (0 < model.Cover?.Length) {
                //    this._pages.Add(this.ConvertWinPath(model.Cover));
                //}
                var contentDir = new DirectoryOperator(this._contentDir);
                contentDir.ParseChildren(false);
                foreach (var content in contentDir.Children) {
                    this._pages.Add(content.Name);
                }

                foreach (var content in model.TableOfContents) {
                    var toc = new TocModelEx(content);
                    if (0 < toc.Link?.Length) {
                        if (this._pages.Contains(toc.Link)) {
                            toc.Index = this._pages.IndexOf(toc.Link);
                        }
                        toc.Link = this.ConvertWinPath(toc.Link);
                    }
                    if (0 < toc.Level && 0 < toc.Content?.Length) {
                        var padding = "　";
                        if (2 == toc.Level) {
                            padding = "　　";
                        } else if (3 == toc.Level) {
                            padding = "　　　　";
                        } else if (4 == toc.Level) {
                            padding = "　　　　　　";
                        }

                        toc.Content = padding + toc.Content;
                        if (0 < toc.Link.Length) {
                            toc.FileIndex = string.Format("[{0:000}]  ", toc.Index + 1);
                        }
                        this.Toc.Add(toc);
                    }
                }
                result = true;
            } catch (Exception ex){
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 指定したページへ移動
        /// </summary>
        /// <returns>true:移動成功、false:それ以外</returns>
        internal bool MoveTo(int index) {
            if (0 <= index && index < this._pages.Count) {
                this.Index = index;
                return true;
            } else {
                return false;
            }
        }
        /// <summary>
        /// 次ページへ移動
        /// </summary>
        /// <returns>true:移動成功、false:それ以外</returns>
        internal bool MoveToNext() {
            if (this.Index + 1 < this._pages.Count) {
                this.Index++;
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 前ページへ移動
        /// </summary>
        /// <returns>true:移動成功、false:それ以外</returns>
        internal bool MoveToPrevious() {
            if (0 < this.Index) {
                this.Index--;
                return true;
            } else {
                return false;
            }

        }
        #endregion

        #region Private Method
        /// <summary>
        /// Windowsのパスに変更する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string ConvertWinPath(string path) {
            string result = path;
            if (result.StartsWith("./")) {
                result = result.Substring(2);
            }
            return result.Replace("/", @"\");
        }
        #endregion
    }
}

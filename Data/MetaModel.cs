using System.Collections.Generic;

namespace MyHolizontalBookViewerLight.Data {
    /// <summary>
    /// メタ情報モデル
    /// </summary>
    public class MetaModel {

        #region Decralation
        public enum BookType {
            reflow,
            fix
        };

        /// <summary>
        /// 著者情報
        /// </summary>
        public class AuthorModel {
            /// <summary>
            /// 著者名
            /// </summary>
            public string Author { set; get; } = "";

            /// <summary>
            /// 著者名(ふりがな)
            /// </summary>
            public string AuthorRuby { set; get; } = "";
        }

        /// <summary>
        /// 目次情報
        /// </summary>
        public class TocModel {
            /// <summary>
            /// 見出しの階層
            /// </summary>
            public int Level { set; get; } = 0;

            /// <summary>
            /// 見出し情報
            /// </summary>
            public string Content { set; get; } = "";

            /// <summary>
            /// ファイルインデックス
            /// </summary>
            public string FileIndex { set; get; } = "";

            /// <summary>
            /// ページのリンク
            /// </summary>
            public string Link { set; get; } = "";
        }
        #endregion

        /// <summary>
        /// 書籍の識別子
        /// </summary>
        public long Id { set; get; }

        /// <summary>
        /// 書籍のレイアウト
        /// </summary>
        public string Layout { set; get; } = "";

        /// <summary>
        /// 縦書き・横書き(リフローの場合のみ)
        /// </summary>
        public string Orientation { set; get; } = "";

        /// <summary>
        /// ページ送りの方向
        /// </summary>
        public string Direction { set; get; } = "";

        /// <summary>
        /// 書籍のタイトル
        /// </summary>
        public string Title { set; get; } = "";

        /// <summary>
        /// 書籍のタイトル(ふりがな)
        /// </summary>
        public string TitleRuby { set; get; } = "";

        /// <summary>
        /// カバーイメージ
        /// </summary>
        public string Cover { set; get; } = "";

        /// <summary>
        /// 著者情報
        /// </summary>
        public IList<AuthorModel> Authors { set; get; } = new List<AuthorModel>();

        /// <summary>
        /// タグ情報
        /// </summary>
        public IList<string> Tags { set; get; } = new List<string>();

        /// <summary>
        /// 登場人物紹介ページ
        /// </summary>
        public string Characters { set; get; } = "";

        /// <summary>
        /// コンテンツの格納フォルダ
        /// </summary>
        public string ContentDir { set; get; } = "";

        /// <summary>
        /// 目次
        /// </summary>
        public IList<TocModel> TableOfContents { set; get; } = new List<TocModel>();
    }
}

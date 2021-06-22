using System.Windows;
using System.Windows.Input;

namespace MyHolizontalBookViewerLight {
    /// <summary>
    /// JumpPage.xaml の相互作用ロジック
    /// </summary>
    public partial class JumpPage : Window {
        public JumpPage() {
            InitializeComponent();
        }


        #region Constructor
        internal JumpPage(Window owner, int index) {
            InitializeComponent();
            this.Owner = owner;
            this.cPage.Text = index.ToString();

            this.Loaded += (sender,e) => {
                this.cPage.SelectAll();
                this.cPage.Focus();
            };
            this.KeyDown += (sender, e) => {
                if (e.Key == Key.Escape) {
                    e.Handled = true;
                    this.DialogResult = false;
                    this.Close();
                } else if (e.Key == Key.Enter) {
                    e.Handled = true;
                    this.DialogResult = true;
                }
            };
        }
        #endregion

        #region Public Property
        internal int Index {
            get {
                int result;
                if (int.TryParse(this.cPage.Text, out result)) {
                    return result;
                }
                return -1;
            }
        }
        #endregion

        #region Event
        #endregion
    }
}

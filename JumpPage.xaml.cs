using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

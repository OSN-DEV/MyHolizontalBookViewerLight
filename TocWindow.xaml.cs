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
using MyHolizontalBookViewerLight.Data;

namespace MyHolizontalBookViewerLight {
    /// <summary>
    /// 目次
    /// </summary>
    public  partial class TocWindow : Window {

        #region Constructor
        internal TocWindow(Window owner, List<MetaOperator.TocModelEx> model) {
            InitializeComponent();
            this.Owner = owner;
            this.cToc.DataContext = model;
        }
        #endregion

        #region Public Property
        internal int Index { private set; get; }
        #endregion

        #region Event
        private void TocList_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var model = (MetaOperator.TocModelEx)this.cToc.SelectedItem;
            if (0 <= model.Index) {
                this.Index = model.Index;
                this.DialogResult = true;
            }
        }
        #endregion
    }
}

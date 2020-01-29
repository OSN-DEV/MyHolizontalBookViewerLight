using MyHolizontalBookViewerLight.Data;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace MyHolizontalBookViewerLight {
    /// <summary>
    /// 目次
    /// </summary>
    public  partial class TocWindow : Window {

        #region Constructor
        internal TocWindow(Window owner, List<MetaOperator.TocModelEx> model, int index) {
            InitializeComponent();
            this.Owner = owner;
            this.cToc.DataContext = model;

            this.Loaded += (sender, e) => {
                bool scrolled = false;
                for (int i = 0; i < model.Count; i++) {
                    if (model[i].Index == index) {
                        this.cToc.SelectedIndex = i;
                        this.cToc.ScrollIntoView(this.cToc.SelectedItem);
                        scrolled = true;
                        break;
                    }
                }
                if (!scrolled) {
                    for(int i=model.Count -1; 0<= i; i--) {
                        if (model[i].Index < index) {
                            this.cToc.SelectedIndex = i;
                            this.cToc.ScrollIntoView(this.cToc.SelectedItem);
                            break;
                        }
                    }
                }
            };

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

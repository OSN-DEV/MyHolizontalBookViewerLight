﻿using MyLib.File;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;

namespace MyHolizontalBookViewerLight {
    /// <summary>
    /// ExtractWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ExtractWindow : Window {

        #region Public Property
        public string HBVFile { set; get; }
        public string DestDir { set; get; }
        #endregion

        #region Constructor
        public ExtractWindow() {
            InitializeComponent();
        }
        public ExtractWindow(Window owner) {
            InitializeComponent();
            this.Owner = owner;

            this.Loaded += ExtractWindow_Loaded;
        }
        #endregion

        #region Event
        private void ExtractWindow_Loaded(object sender, RoutedEventArgs e) {
            this.cProgressText.Text = "";
            Task.Run(() => {
                try {
                    Extract(this.HBVFile, this.DestDir);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    this.Close(false);
                    return;
                }
                this.Close(true);
            });
        }

        private void ProgressBar_ValueChanged(object sender, RoutedEventArgs e) {
            this.cProgressText.Text = this.cProgress.Value.ToString() + "%";
        }
        #endregion

        #region Private Method
        private void Close(bool result) {
            Application.Current.Dispatcher.Invoke(() => {
                this.DialogResult = result;
            });
        }
        /// <summary>
        /// extract hsv file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="dest"></param>
        private void Extract(string file, string dest) {
            new DirectoryOperator(dest).Create();
            using (ZipArchive archive = ZipFile.OpenRead(file)) {
                Application.Current.Dispatcher.Invoke(() => {
                    this.cProgress.Value = 0;
                    this.cProgress.Maximum = 100;
                });
                var index = 0;
                foreach (ZipArchiveEntry entry in archive.Entries) {
                    if (entry.FullName.EndsWith("/")) {
                        new DirectoryOperator(dest + @"\" + entry.FullName).Create();
                    } else {
                        entry.ExtractToFile(Path.Combine(dest, entry.FullName));
                    }
                    Application.Current.Dispatcher.Invoke(() => {
                        this.cProgress.Value = ((++index) * 100) / archive.Entries.Count;
                    });
                }
            }
        }
        #endregion
    }
}

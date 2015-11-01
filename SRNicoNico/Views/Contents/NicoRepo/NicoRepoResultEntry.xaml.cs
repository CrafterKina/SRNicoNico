﻿using SRNicoNico.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SRNicoNico.Views.Contents.NicoRepo {
    /// <summary>
    /// NicoRepoResultEntry.xaml の相互作用ロジック
    /// </summary>
    public partial class NicoRepoResultEntry : UserControl {
        public NicoRepoResultEntry() {
            InitializeComponent();
        }


        public void OpenHyperLink(object sender, RequestNavigateEventArgs e) {

            if(DataContext is NicoRepoResultEntryViewModel) {

                var vm = (NicoRepoResultEntryViewModel) DataContext;
                vm.OpenHyperLink(e.Uri.OriginalString);
            }
        }

    }
}

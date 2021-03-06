﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using System.Drawing;

using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.ViewModels;
using System.Runtime.InteropServices;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace SRNicoNico.Views.Contents.Live {
    /// <summary>
    /// TimeShiftController.xaml の相互作用ロジック
    /// </summary>
    public partial class TimeShiftController : UserControl {
        public TimeShiftController() {
            InitializeComponent();
        }

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);



		private void Seek_MouseMove(object sender, MouseEventArgs e) {

            if(!(DataContext is LiveWatchViewModel)) {

                return;
            }
            var vm = (LiveWatchViewModel) DataContext;

			//マウスカーソルX座標
			double x = e.GetPosition(this).X;

			
			//シーク中の動画時間
			int ans = (int) (x / ActualWidth * Seek.VideoTime);
            if(ans < 0 || Seek.VideoTime < ans) {

                return;
            }

            Seek.PopupText = NicoNicoUtil.ConvertTime(ans);
            Seek.PopupRect = new Rect(x - 5, 0, 20, 20);

		}

		private void Seek_MouseLeave(object sender, MouseEventArgs e) {

			Seek.IsPopupOpen = false;
		}

		private void Seek_MouseEnter(object sender, MouseEventArgs e) {

            if(!(DataContext is LiveWatchViewModel)) {

                return;
            }

			Seek.IsPopupOpen = true;
		}

        private void Seek_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {

            if(!(DataContext is LiveWatchViewModel)) {

                return;
            }
            var vm = (LiveWatchViewModel)DataContext;

            double x = e.GetPosition(this).X;
            int ans = (int)(x / ActualWidth * Seek.VideoTime);
            
            if(ans < 0) {

                ans = 0;
            } else if(ans > Seek.VideoTime) {

                ans = (int) Seek.VideoTime;
            }

            vm.Handler.Seek(ans * 100);
        }
    }
}

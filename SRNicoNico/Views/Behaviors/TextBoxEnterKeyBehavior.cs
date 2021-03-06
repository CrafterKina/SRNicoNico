﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Controls;
using System.Windows.Input;

using Livet;

using SRNicoNico.ViewModels;
using System.Reflection;

namespace SRNicoNico.Views.Behaviors {


	
	public class TextBoxEnterKeyBehavior : Behavior<TextBox> {



        public ViewModel Binding {
            get { return (ViewModel)GetValue(BindingProperty); }
            set { SetValue(BindingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Binding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register("Binding", typeof(ViewModel), typeof(TextBoxEnterKeyBehavior), new PropertyMetadata(null));



        public string MethodName {
            get { return (string)GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MethodName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register("MethodName", typeof(string), typeof(TextBoxEnterKeyBehavior), new PropertyMetadata(""));




        protected override void OnAttached() {
			base.OnAttached();

			AssociatedObject.KeyDown += TextBox_KeyDown;

		}

		protected override void OnDetaching() {
			base.OnDetaching();

			AssociatedObject.KeyDown -= TextBox_KeyDown;
		}

		//Enterキーで検索できるように
		private void TextBox_KeyDown(object sender, KeyEventArgs e) {

			if(e.Key == Key.Enter) {

                InvokeMethod();
			}
		}

        private void InvokeMethod() {

            var type = Binding.GetType();
            var method = type.GetMethod(MethodName, new[] { typeof(string) });
            method.Invoke(Binding, new[] { AssociatedObject.Text });
        }


    }
}

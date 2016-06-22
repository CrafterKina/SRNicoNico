﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SRNicoNico.Models;
using System.Windows.Media;

namespace SRNicoNico.ViewModels {
    public class ConfigGeneralViewModel : ConfigViewModelBase {




        private ConfigViewModel Owner;

        public ConfigGeneralViewModel(ConfigViewModel vm) : base("一般") {

            Owner = vm;
        }
    }
}

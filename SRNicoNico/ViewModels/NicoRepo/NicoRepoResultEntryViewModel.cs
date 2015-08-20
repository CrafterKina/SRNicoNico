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

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.ViewModels {
    public class NicoRepoResultEntryViewModel : ViewModel {


        public NicoNicoNicoRepoDataEntry Entry { get; private set; }

        
        public NicoRepoResultEntryViewModel(NicoNicoNicoRepoDataEntry entry) {

            Entry = entry;

        }
    }
}
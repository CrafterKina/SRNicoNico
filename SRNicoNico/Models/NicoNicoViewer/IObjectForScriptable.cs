﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoViewer {
    public interface IObjectForScriptable {

        void Invoked(string cmd, string args);


    }
}

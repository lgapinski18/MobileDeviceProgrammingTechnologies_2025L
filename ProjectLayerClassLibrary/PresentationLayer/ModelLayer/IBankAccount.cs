﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.PresentationLayer.ModelLayer
{
    public interface IBankAccount
    {
        string AccountNumber { get; }
        float AccountBalance { get; }
        ICollection<string> Reports { get; }
    }
}

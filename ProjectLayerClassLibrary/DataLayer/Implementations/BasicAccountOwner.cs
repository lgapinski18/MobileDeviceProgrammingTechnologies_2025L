﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ProjectLayerClassLibrary")]

namespace ProjectLayerClassLibrary.DataLayer.Implementations
{
    internal class BasicAccountOwner : AAccountOwner
    {
        public BasicAccountOwner(int ownerId, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword) : base(ownerId, ownerName, ownerSurname, ownerEmail, ownerPassword)
        {
        }
    }
}

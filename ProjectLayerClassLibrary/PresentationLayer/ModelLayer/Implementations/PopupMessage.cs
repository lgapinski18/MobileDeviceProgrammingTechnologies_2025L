using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.PresentationLayer.ModelLayer.Implementations
{
    internal class PopupMessage
    {
        private string message;
        public string Message => message;

        public PopupMessage(string message)
        {
            this.message = message;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.DataLayer
{
    public interface IIdentifiable
    {
        public int GetId();
        public void SetId(int id);
    }
}

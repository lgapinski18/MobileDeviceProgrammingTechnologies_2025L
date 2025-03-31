using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.DataLayer
{
    public abstract class AAccountOwner
    {
        private int ownerId;
        public int OwnerId { get { return ownerId; } set { ownerId = value; } }

        private string ownerName;
        public string OwnerName { get { return ownerName; } set { ownerName = value; } }

        private string ownerSurname;
        public string OwnerSurname { get { return ownerSurname; } set { ownerSurname = value; } }

        private string ownerEmail;
        public string OwnerEmail { get { return ownerEmail; } set { ownerEmail = value; } }

        private string ownerPassword;
        public string OwnerPassword { get { return ownerPassword; } set { ownerPassword = value; } }

        public AAccountOwner(int ownerId, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            this.ownerId = ownerId;
            this.ownerName = ownerName;
            this.OwnerSurname = ownerSurname;
            this.ownerSurname = ownerEmail;
            this.ownerSurname = ownerPassword;
        }
    }
}

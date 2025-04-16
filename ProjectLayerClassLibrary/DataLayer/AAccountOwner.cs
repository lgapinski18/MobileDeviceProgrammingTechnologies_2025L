using ComunicationApiXmlDto;
using ProjectLayerClassLibrary.DataLayer.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace ProjectLayerClassLibrary.DataLayer
{
    public abstract class AAccountOwner : IIdentifiable
    {
        private int id;

        private string ownerLogin;
        public string OwnerLogin { get { return ownerLogin; } set { ownerLogin = value; } }

        private string ownerName;
        public string OwnerName { get { return ownerName; } set { ownerName = value; } }

        private string ownerSurname;
        public string OwnerSurname { get { return ownerSurname; } set { ownerSurname = value; } }

        private string ownerEmail;
        public string OwnerEmail { get { return ownerEmail; } set { ownerEmail = value; } }

        private string ownerPassword;
        public string OwnerPassword { get { return ownerPassword; } set { ownerPassword = value; } }

        public AAccountOwner(int ownerId, string ownerLogin, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            this.id = ownerId;
            this.ownerLogin = ownerLogin;
            this.ownerName = ownerName;
            this.ownerSurname = ownerSurname;
            this.ownerEmail = ownerEmail;
            this.ownerPassword = ownerPassword;
        }

        internal static AAccountOwner? CreateAcountOwnerFromXml(AccountOwnerDto? accountOwnerDto)
        {
            if (accountOwnerDto == null)
            {
                return null;
            }
            return new BasicAccountOwner(accountOwnerDto.Id, accountOwnerDto.Login, accountOwnerDto.Name, accountOwnerDto.Surname, accountOwnerDto.Email, "");
        }

        public int GetId()
        {
            return id;
        }

        public void SetId(int id)
        {
            this.id = id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            AAccountOwner other = (AAccountOwner)obj;
            return id == other.id
                   && ownerLogin == other.ownerLogin
                   && ownerName == other.ownerName
                   && ownerSurname == other.ownerSurname
                   && OwnerEmail == other.OwnerEmail
                   && ownerPassword == other.ownerPassword;
        }
    }
}

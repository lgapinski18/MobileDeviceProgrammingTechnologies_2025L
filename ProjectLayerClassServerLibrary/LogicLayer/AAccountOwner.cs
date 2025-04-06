using ProjectLayerClassLibrary.LogicLayer.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.LogicLayer
{
    public abstract class AAccountOwner
    {
        protected DataLayer.AAccountOwner dataLayerAccountOwner;
        internal DataLayer.AAccountOwner DataLayerAccountOwner {  get { return dataLayerAccountOwner; } }

        internal static AAccountOwner? CreateAccountOwner(DataLayer.AAccountOwner? dataLayerAccountOwner)
        {
            if (dataLayerAccountOwner == null) 
            { 
                return null; 
            }
            return new BasicLogicLayerAccountOwner(dataLayerAccountOwner);
        }

        public string OwnerLogin { get { return dataLayerAccountOwner.OwnerLogin; } set { dataLayerAccountOwner.OwnerLogin = value; } }

        public string OwnerName { get { return dataLayerAccountOwner.OwnerName; } set { dataLayerAccountOwner.OwnerName = value; } }

        public string OwnerSurname { get { return dataLayerAccountOwner.OwnerSurname; } set { dataLayerAccountOwner.OwnerSurname = value; } }

        public string OwnerEmail { get { return dataLayerAccountOwner.OwnerEmail; } set { dataLayerAccountOwner.OwnerEmail = value; } }

        public string OwnerPassword { get { return dataLayerAccountOwner.OwnerPassword; } set { dataLayerAccountOwner.OwnerPassword = value; } }

        public AAccountOwner(DataLayer.AAccountOwner dataLayerAccountOwner)
        {
            this.dataLayerAccountOwner = dataLayerAccountOwner;
        }

        public int GetId()
        {
            return dataLayerAccountOwner.GetId();
        }

        public void SetId(int id)
        {
            dataLayerAccountOwner.SetId(id);
        }
    }
}

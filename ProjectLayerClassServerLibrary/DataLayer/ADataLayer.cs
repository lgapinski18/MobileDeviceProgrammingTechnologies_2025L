﻿using ProjectLayerClassServerLibrary.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassServerLibrary.DataLayer
{
    public abstract class ADataLayer
    {
        public static ADataLayer CreateDataLayerInstance()
        {
            return new Implementations.BasicDataLayer();
        }

        public abstract AAccountOwner CreateAccountOwner(string ownerName, string ownerSurname, string ownerEmail, string ownerPassword);

        public abstract ABankAccount CreateBankAccount(int ownerId);


        public abstract AAccountOwner? GetAccountOwner(int ownerId);
        public abstract AAccountOwner? GetAccountOwner(string ownerLogin);
        public abstract ABankAccount? GetBankAccount(string accountNumber);
        public abstract ICollection<ABankAccount> GetBankAccounts(int ownerId);
        public abstract ICollection<AAccountOwner> GetAllAccountOwners();
        public abstract ICollection<ABankAccount> GetAllBankAccounts();
    }
}

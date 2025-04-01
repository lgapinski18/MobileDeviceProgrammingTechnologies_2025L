using ProjectLayerClassLibrary.DataLayer;
using ProjectLayerClassLibrary.LogicLayer.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.LogicLayer.Implementations
{
    internal class BasicLogicLayer : ALogicLayer
    {
        public BasicLogicLayer(ADataLayer? dataLayer = default)
        {
            this.dataLayer = dataLayer == null ? ADataLayer.CreateDataLayerInstance() : dataLayer;
        }

        public override bool AuthenticateAccountOwner(int ownerId, string password)
        {
            AAccountOwner? accountOwner = dataLayer.GetAccountOwner(ownerId);

            if (accountOwner == null)
            {
                throw new ThereIsNoSuchOwnerException($"Nie znaleziono właściciela konta z podanym identyfikatorem: {ownerId}");
            }

            if (password == null)
            {
                throw new ArgumentNullException("Podany obiekt string hasła jest null");
            }

            return password == accountOwner.OwnerPassword;
        }

        public override AAccountOwner CreateNewAccountOwner(string name, string surname, string email, string password, out CreationAccountOwnerFlags creationAccountOwnerFlags)
        {
            throw new NotImplementedException();
            //string pattern = @"^$";
            //pattern = @"^[\w.%+-]+@[\w.-]+\.[A-Za-z]{2,}$";
            //if (!Regex.IsMatch(email, pattern))
            //{
            //    creationAccountOwnerFlags = creationAccountOwnerFlags | CreationAccountOwnerFlags.INCORRECT_EMAIL;
            //}
            //dataLayer.CreateAccountOwner();
        }

        public override AAccountOwner GetAccountOwner(int ownerId)
        {
            throw new NotImplementedException();
        }

        public override ICollection<ABankAccount> GetAccountOwnerBankAccounts(int ownerId)
        {
            throw new NotImplementedException();
        }

        public override ABankAccount OpenNewBankAccount(int ownerId)
        {
            throw new NotImplementedException();
        }

        public override TransferCodes performTransfer(string ownerAccountNumber, string targetAccountNumber, float amount, string description)
        {
            throw new NotImplementedException();
        }
    }
}

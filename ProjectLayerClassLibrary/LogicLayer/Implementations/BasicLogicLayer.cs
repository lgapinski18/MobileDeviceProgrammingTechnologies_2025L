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
        private ADataLayer dataLayer;
        public ADataLayer DataLayer { get { return dataLayer; } }

        public BasicLogicLayer(ADataLayer? dataLayer = default)
        {
            this.dataLayer = dataLayer ?? ADataLayer.CreateDataLayerInstance();
        }

        public override bool AuthenticateAccountOwner(string login, string password)
        {
            AAccountOwner? accountOwner = dataLayer.GetAccountOwner(login);

            if (login == null)
            {
                throw new ArgumentNullException("Podany obiekt string loginu jest null");
            }

            if (password == null)
            {
                throw new ArgumentNullException("Podany obiekt string hasła jest null");
            }

            if (accountOwner == null)
            {
                throw new ThereIsNoSuchOwnerException($"Nie znaleziono właściciela konta z podanym loginem: {login}");
            }

            return password == accountOwner.OwnerPassword;
        }

        public override AAccountOwner? CreateNewAccountOwner(string name, string surname, string email, string password, out CreationAccountOwnerFlags creationAccountOwnerFlags)
        {
            creationAccountOwnerFlags = CreationAccountOwnerFlags.EMPTY;

            string pattern = @"^$";
            if (!Regex.IsMatch(name, pattern))
            {
                creationAccountOwnerFlags |= CreationAccountOwnerFlags.INCORRECT_NAME;
            }
            pattern = @"^$";
            if (!Regex.IsMatch(surname, pattern))
            {
                creationAccountOwnerFlags |= CreationAccountOwnerFlags.INCORRECT_SURNAME;
            }
            pattern = @"^[\w.%+-]+@[\w.-]+\.[A-Za-z]{2,}$";
            if (!Regex.IsMatch(email, pattern))
            {
                creationAccountOwnerFlags |= CreationAccountOwnerFlags.INCORRECT_EMAIL;
            }
            if (password.Length >= 8)
            {
                creationAccountOwnerFlags |= CreationAccountOwnerFlags.INCORRECT_PASSWORD;
            }

            if (creationAccountOwnerFlags == CreationAccountOwnerFlags.EMPTY)
            {
                creationAccountOwnerFlags = CreationAccountOwnerFlags.SUCCESS;
                return dataLayer.CreateAccountOwner(name, surname, email, password);
            }

            return null;
        }

        public override AAccountOwner? GetAccountOwner(int ownerId)
        {
            return dataLayer.GetAccountOwner(ownerId);
        }

        public override ICollection<ABankAccount> GetAccountOwnerBankAccounts(int ownerId)
        {
            return dataLayer.GetBankAccounts(ownerId);
        }

        public override ABankAccount? OpenNewBankAccount(int ownerId)
        {
            return dataLayer.CreateBankAccount(ownerId);
        }

        public override TransferCodes PerformTransfer(string ownerAccountNumber, string targetAccountNumber, float amount, string description)
        {
            throw new NotImplementedException();
        }

        public override AAccountOwner? GetAccountOwner(string login)
        {
            return dataLayer.GetAccountOwner(login);
        }
    }
}

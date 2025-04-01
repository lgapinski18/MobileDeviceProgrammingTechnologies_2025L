using ProjectLayerClassLibrary.DataLayer.Exceptions;
using ProjectLayerClassLibrary.DataLayer.Repositories;
using ProjectLayerClassLibrary.DataLayer.Implementations.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.DataLayer.Implementations
{
    internal class BasicDataLayer : ADataLayer
    {
        private IAccountOwnerRepository accountOwnerRepository;
        private IBankAccountRepository bankAccountRepository;

        public BasicDataLayer()
        {
            accountOwnerRepository = RepositoryFactory.CreateAccountOwnerRepository();
            bankAccountRepository = RepositoryFactory.CreateBankAccountRepository();

            GenerateStartingContent();
        }

        private void GenerateStartingContent()
        {

        }

        public override AAccountOwner CreateAccountOwner(int ownerId, string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            AAccountOwner accountOwner = new BasicAccountOwner(ownerId, ownerName, ownerSurname, ownerEmail, ownerPassword);
            if (!accountOwnerRepository.Save(accountOwner))
            {
                throw new CreatingAccountOwnerException("Wystąpił problem podczas zapisu utworzonego obiektu właściciela konta bankowego do repoytorium!");
            }

            return accountOwner;
        }

        public override ABankAccount CreateBankAccount(string accountNumber, int ownerId)
        {
            AAccountOwner? accountOwner = accountOwnerRepository.Get(ownerId);
            if (accountOwner != null)
            {
                ABankAccount bankAccount = new BasicBankAccount(accountNumber, accountOwner);
                if (!bankAccountRepository.Save(bankAccount))
                {
                    throw new CreatingBankAccountException("Wystąpił problem podczas zapisu utworzonego obiektu konta bankowego do repoytorium!");
                }
                return bankAccount;
            }
            else
            {
                throw new ArgumentException($"Nie istnieje właściciel konta bankowego o id {ownerId}!");
            }
        }

        public override AAccountOwner? GetAccountOwner(int ownerId)
        {
            return accountOwnerRepository.Get(ownerId);
        }

        public override ICollection<AAccountOwner> GetAllAccountOwners()
        {
            return accountOwnerRepository.GetAll();
        }

        public override ICollection<ABankAccount> GetAllBankAccounts()
        {
            return bankAccountRepository.GetAll();
        }

        public override ABankAccount? GetBankAccount(string accountNumber)
        {
            return bankAccountRepository.GetByAccountNumber(accountNumber);
        }

        public override ICollection<ABankAccount> GetBankAccounts(int ownerId)
        {
            return bankAccountRepository.GetByAccountOwnerId(ownerId);
        }
    }
}

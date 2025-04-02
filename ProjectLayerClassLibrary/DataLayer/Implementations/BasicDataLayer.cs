using ProjectLayerClassLibrary.DataLayer.Exceptions;
using ProjectLayerClassLibrary.DataLayer.Repositories;
using ProjectLayerClassLibrary.DataLayer.Implementations.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.DataLayer.Implementations
{
    internal class BasicDataLayer : ADataLayer
    {
        private IAccountOwnerRepository accountOwnerRepository;
        private IBankAccountRepository bankAccountRepository;

        private object accountOwnerLock = new object();
        private object bankAccountLock = new object();

        public BasicDataLayer()
        {
            accountOwnerRepository = RepositoryFactory.CreateAccountOwnerRepository();
            bankAccountRepository = RepositoryFactory.CreateBankAccountRepository();

            GenerateStartingContent();
        }

        private void GenerateStartingContent()
        {

        }

        public override AAccountOwner CreateAccountOwner(string ownerName, string ownerSurname, string ownerEmail, string ownerPassword)
        {
            AAccountOwner accountOwner;
            lock (accountOwnerLock)
            {
                const string loginPrefix = "IK";
                int ownerId = 0;
                ICollection<AAccountOwner> accountOwners = accountOwnerRepository.GetAll();
                if (accountOwners.Count != 0)
                {
                    ownerId = accountOwners.Last().GetId();
                }

                while (accountOwnerRepository.Get(ownerId) != null)
                {
                    ownerId += 1;
                }

                string ownerLogin;

                using (var rng = RandomNumberGenerator.Create())
                {
                    byte[] bytes = new byte[4]; // 4 bytes = 32-bit integer
                    int number;
                    do
                    {
                        rng.GetBytes(bytes);
                        number = BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF; // Ensure positive
                        ownerLogin = loginPrefix + (number % 1_000_000).ToString("D6"); // Exactly 8 digits
                    }
                    while (accountOwnerRepository.GetByOwnerLogin(ownerLogin) != null);
                }

                accountOwner = new BasicAccountOwner(ownerId, ownerLogin, ownerName, ownerSurname, ownerEmail, ownerPassword);
                if (!accountOwnerRepository.Save(accountOwner))
                {
                    throw new CreatingAccountOwnerException("Wystąpił problem podczas zapisu utworzonego obiektu właściciela konta bankowego do repoytorium!");
                }

                CreateBankAccount(ownerId);
            }

            return accountOwner;
        }

        public override ABankAccount CreateBankAccount(int ownerId)
        {
            lock (accountOwnerLock)
            {
                AAccountOwner? accountOwner = accountOwnerRepository.Get(ownerId);
                if (accountOwner != null)
                {
                    string accountNumber;
                    ABankAccount bankAccount;

                    lock (bankAccountLock)
                    {
                        using (var rng = RandomNumberGenerator.Create())
                        {
                            byte[] bytes = new byte[4]; // 4 bytes = 32-bit integer
                            int number;
                            do
                            {
                                rng.GetBytes(bytes);
                                number = BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF; // Ensure positive
                                accountNumber = (number % 100000000).ToString("D8"); // Exactly 8 digits
                            }
                            while (bankAccountRepository.GetByAccountNumber(accountNumber) != null);
                        }

                        bankAccount = new BasicBankAccount(accountNumber, accountOwner);
                        if (!bankAccountRepository.Save(bankAccount))
                        {
                            throw new CreatingBankAccountException("Wystąpił problem podczas zapisu utworzonego obiektu konta bankowego do repoytorium!");
                        }
                    }

                    return bankAccount;
                }
                else
                {
                    throw new ArgumentException($"Nie istnieje właściciel konta bankowego o id {ownerId}!");
                }
            }
        }

        public override AAccountOwner? GetAccountOwner(int ownerId)
        {
            lock (accountOwnerLock)
            {
                return accountOwnerRepository.Get(ownerId);
            }
        }

        public override ICollection<AAccountOwner> GetAllAccountOwners()
        {
            lock (accountOwnerLock)
            {
                return accountOwnerRepository.GetAll();
            }
        }

        public override ICollection<ABankAccount> GetAllBankAccounts()
        {
            lock (bankAccountLock)
            {
                return bankAccountRepository.GetAll();
            }
        }

        public override ABankAccount? GetBankAccount(string accountNumber)
        {
            lock (bankAccountLock)
            {
                return bankAccountRepository.GetByAccountNumber(accountNumber);
            }
        }

        public override ICollection<ABankAccount> GetBankAccounts(int ownerId)
        {
            lock (bankAccountLock)
            {
                lock (accountOwnerLock)
                {
                    return bankAccountRepository.GetByAccountOwnerId(ownerId);
                }
            }
        }

        public override AAccountOwner? GetAccountOwner(string ownerLogin)
        {
            lock (accountOwnerLock)
            {
                return accountOwnerRepository.GetByOwnerLogin(ownerLogin);
            }
        }
    }
}

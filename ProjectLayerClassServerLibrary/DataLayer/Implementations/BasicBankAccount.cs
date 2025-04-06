using ProjectLayerClassLibrary.DataLayer.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ProjectLayerClassLibraryTest")]

namespace ProjectLayerClassLibrary.DataLayer.Implementations
{
    internal class BasicBankAccount : ABankAccount
    {
        private object accountBalanceLock = new object();
        private object bankAccountReportsLock = new object();

        public BasicBankAccount(int id, string accountNumber, AAccountOwner accountOwner)
            : base(id, accountNumber, accountOwner)
        {
            AccountNumber = accountNumber;
            AccountOwner = accountOwner;
            bankAccountReports = new List<ABankAccountReport>();

            ABankAccountReport startingBankAccountReport = new BankAccountReportWithOwnerData(0.0f, 0.0f, accountOwner.OwnerName, accountOwner.OwnerSurname, accountOwner.OwnerEmail);
            bankAccountReports.Add(startingBankAccountReport);
        }

        public override void DecreaseAccountBalance(float amount)
        {
            lock (accountBalanceLock)
            {
                if (amount < 0.0f)
                {
                    throw new ArgumentException("Amout nie może być wartością ujemną!");
                }
                if (AccountBalance < amount)
                {
                    throw new InvalidBankAccountOperationException("Nie wystarczająco środków na kącie, aby przeprowadzić zmniejszenie stanu kąta!");
                }
                AccountBalance -= amount;
            }
        }

        public override ABankAccountReport GenerateBankAccountReport()
        {
            lock (bankAccountReportsLock)
            {
                ABankAccountReport bankAccountReport = new BankAccountReportWithOwnerData(bankAccountReports.Last().CurrentAccountBalance, AccountBalance, AccountOwner.OwnerName, AccountOwner.OwnerSurname, AccountOwner.OwnerEmail);
                bankAccountReports.Add(bankAccountReport);
                return bankAccountReport;
            }
        }

        public override ICollection<ABankAccountReport> GetBankAccountReports()
        {
            lock (bankAccountReportsLock)
            {
                return bankAccountReports;
            }
        }

        public override void IncreaseAccountBalance(float amount)
        {
            lock (accountBalanceLock)
            {
                if (amount < 0.0f)
                {
                    throw new ArgumentException("Amout nie może być wartością ujemną!");
                }
                AccountBalance += amount;
            }
        }
    }
}

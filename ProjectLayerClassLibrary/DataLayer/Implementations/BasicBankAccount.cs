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
        public BasicBankAccount(string accountNumber, AAccountOwner accountOwner)
        {
            AccountNumber = accountNumber;
            AccountOwner = accountOwner;
            AccountBalance = 0.0f;
            bankAccountReports = new List<ABankAccountReport>();

            ABankAccountReport startingBankAccountReport = new BankAccountReportWithOwnerData(0.0f, 0.0f, accountOwner.OwnerName, accountOwner.OwnerSurname, accountOwner.OwnerEmail);
            bankAccountReports.Add(startingBankAccountReport);
        }

        public override void DecreaseAccountBalance(float amount)
        {
            if (AccountBalance <  amount)
            {
                throw new InvalidBankAccountOperationException("Nie wystarczająco środków na kącie, aby przeprowadzić zmniejszenie stanu kąta!");
            }
            AccountBalance -= amount;
        }

        public override ABankAccountReport GenerateBankAccountReport()
        {
            ABankAccountReport bankAccountReport = new BankAccountReportWithOwnerData(bankAccountReports.Last().CurrentAccountBalance, AccountBalance, AccountOwner.OwnerName, AccountOwner.OwnerSurname, AccountOwner.OwnerEmail);
            bankAccountReports.Add(bankAccountReport);
            return bankAccountReport;
        }

        public override ICollection<ABankAccountReport> GetBankAccountReports()
        {
            return bankAccountReports;
        }

        public override void IncreaseAccountBalance(float amount)
        {
            AccountBalance += amount;
        }
    }
}

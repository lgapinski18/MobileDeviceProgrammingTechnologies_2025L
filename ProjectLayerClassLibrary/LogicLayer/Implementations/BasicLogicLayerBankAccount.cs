using ProjectLayerClassLibrary.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.LogicLayer.Implementations
{
    internal class BasicLogicLayerBankAccount : ABankAccount
    {
        public BasicLogicLayerBankAccount(DataLayer.ABankAccount dataLayerBankAccount) : base(dataLayerBankAccount)
        {
        }

        public override void DecreaseAccountBalance(float amount)
        {
            dataLayerBankAccount.DecreaseAccountBalance(amount);
        }

        public override ABankAccountReport GenerateBankAccountReport()
        {
            return ABankAccountReport.CreateBankAccountReport(dataLayerBankAccount.GenerateBankAccountReport());
        }

        public override ICollection<ABankAccountReport> GetBankAccountReports()
        {
            ICollection<ABankAccountReport> bankAccountReports = new List<ABankAccountReport>();
            foreach (DataLayer.ABankAccountReport bankAccountReport in dataLayerBankAccount.GetBankAccountReports())
            {
                bankAccountReports.Add(ABankAccountReport.CreateBankAccountReport(bankAccountReport));
            }
            return bankAccountReports;
        }

        public override void IncreaseAccountBalance(float amount)
        {
            dataLayerBankAccount.IncreaseAccountBalance(amount);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLayerClassLibrary.LogicLayer.Implementations
{
    internal class BasicLogicLayerBankAccountReport : ABankAccountReport
    {
        public BasicLogicLayerBankAccountReport(DataLayer.ABankAccountReport dataLayerBankAccountReport) : base(dataLayerBankAccountReport)
        {
        }

        public override string GetReportContent()
        {
            return dataLayerBankAccountReport.GetReportContent();
        }
    }
}

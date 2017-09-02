using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRVPoliceCaseInMonthControl_ZZGA
{
    public class CaseInMonth
    {
        private int caseOrder = 1;
        private SortedList<int,int> caseSortedList = new SortedList<int, int>();
        private int average = 0;

        public CaseInMonth()
        {
        }

        public CaseInMonth(int caseOrder, SortedList<int, int> sortedList)
        {
            CaseOrder = caseOrder;
            CaseSortedList = sortedList ?? throw new ArgumentNullException(nameof(sortedList));

            Initial();
        }

        public int CaseOrder { get => caseOrder; set => caseOrder = value; }
        public SortedList<int, int> CaseSortedList { get => caseSortedList; set => caseSortedList = value; }
        public int Average { get => average; set => average = value; }

        private void Initial()
        {
            int sum = 0;
            int n = 0;
            foreach (var s in CaseSortedList)
            {
                if (s.Value >= 0)
                {
                    sum += s.Value;
                    n++;
                }
            }

            Average = n > 0 ? Convert.ToInt32(sum / n) : 0;
        }
    }
}

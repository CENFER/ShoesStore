using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ShoesStore.Areas.Admin.InterfaceRepositories;
using ShoesStore.Areas.Admin.ViewModels;

namespace ShoesStore.Areas.Admin.ViewComponents
{
	public class SaleChart : ViewComponent
	{
		IReportRepository reportRepo;
		public SaleChart(IReportRepository reportRepo)
		{
			this.reportRepo = reportRepo;
		}

		public IViewComponentResult Invoke()
		{
			ReportViewModel report = new ReportViewModel();
			report.saleByMonths = reportRepo.GetSaleByMonth();
			report.saleByProducts = reportRepo.GetSaleByProduct();

            int maxMonth = report.saleByMonths.Select(x => x.month).Max();

            List<SelectListItem> listMonth = new List<SelectListItem>();
            for (int i = 1; i <= maxMonth; ++i)
            {
                listMonth.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = MonthTransfer(i)
                });
            }

            ViewBag.MonthList = listMonth;
            ViewBag.ChoosenMonth = maxMonth;
            return View(report);
		}

		public string MonthTransfer(int month)
		{
			if (month == 1)
			{
				return "January";
			}
			if(month == 2)
			{
                return "February";
            }
            if (month == 3)
            {
                return "March";
            }
            if (month == 4)
            {
                return "April";
            }
            if (month == 5)
            {
                return "May";
            }
            if (month == 6)
            {
                return "June";
            }
            if (month == 7)
            {
                return "July";
            }
            if (month == 8)
            {
                return "August";
            }
            if (month == 9)
            {
                return "September";
            }
            if (month == 10)
            {
                return "October";
            }
            if (month == 11)
            {
                return "November";
            }

            return "December";
        }
	}
}

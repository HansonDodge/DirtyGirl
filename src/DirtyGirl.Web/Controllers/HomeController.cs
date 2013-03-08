using System;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Models;
using DirtyGirl.Web.Utils;
using System.Web.Mvc;
using System.Linq;
using DirtyGirl.Web.Helpers;
using DirtyGirl.Models.Enums;

namespace DirtyGirl.Web.Controllers
{

    public class HomeController : BaseController
    {

        #region Constructor

        private readonly IEventService _eventService;

        public HomeController(IEventService eventService)
        {            
            this._eventService = eventService;
        }

        #endregion

        public ActionResult Index()
        {
            vmHomePage vm = new vmHomePage();
            vm.EventDateDetails = _eventService.GetActiveEventDateOverviews(null, null, null, "date", "asc");            
            vm.RegionList = Utilities.CreateSelectList(_eventService.GetRegionsByCountry(DirtyGirlConfig.Settings.DefaultCountryId), x => x.RegionId, x => x.Name);           
            vm.MonthList = DirtyGirlExtensions.ConvertToSelectList<Months>();
            vm.MonthList.Insert(0, new SelectListItem { Text = "Select", Value = "" }); 

            return View(vm);
        }

        public ActionResult ViewEvent(int id)
        {
            var eventObj = _eventService.GetEventById(id);
            if (eventObj == null || (eventObj.IsActive == false && !User.IsInRole("Admin")))
                throw new Exception("The event requested either does not exist or is not active");
            var vm = new vmViewEvent
                         {
                             OverView = _eventService.GetEventOverviewById(id),
                             EventDetails = _eventService.GetEventDetails(id)
                         };

            return View(vm);
        }

        #region Ajax

        public ActionResult FilterEventDetails(int? regionId, int? month, int? year, string sortBy, string directionOfSort)
        {
            var eventDetails = _eventService.GetActiveEventDateOverviews(regionId, month, year, sortBy, directionOfSort);           
           
            return Json(new object[]{ this.RenderPartialViewToString("Partial/EventList", eventDetails)});
        }

        #endregion


    }
}

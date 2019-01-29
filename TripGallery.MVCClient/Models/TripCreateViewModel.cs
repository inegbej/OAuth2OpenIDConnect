using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TripGallery.DTO;

namespace TripGallery.MVCClient.Models
{
    public class TripCreateViewModel
    {
        public HttpPostedFileBase MainImage { get; set; }

        public TripForCreation Trip { get; set; }

        public TripCreateViewModel()
        {

        }

        public TripCreateViewModel(TripForCreation trip)
        {
            Trip = trip;
        }
    }
}
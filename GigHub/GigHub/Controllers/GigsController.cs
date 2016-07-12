using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using GigHub.Models;
using GigHub.ViewModels;
using Microsoft.AspNet.Identity;

namespace GigHub.Controllers
{
    public class GigsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GigsController()
        {
            _context = new ApplicationDbContext();
        }

        [Authorize]
        public ActionResult Mine()
        {
            var userId = User.Identity.GetUserId();
            var gigs = _context.Gigs
                .Where(a => a.ArtistId == userId && a.DateTime > DateTime.Now && !a.IsCanceled)
                .Include(a => a.Genre)
                .ToList();

            return View(gigs);
        }

        [Authorize]
        public ActionResult Attending()
        {
            var userId = User.Identity.GetUserId();
            var gigs = _context.Attendances
                .Where(a => a.AttendeeId == userId)
                .Select(a => a.Gig)
                .Include(g => g.Artist)
                .Include(g => g.Genre)
                .ToList();
            var viewModel = new GigsViewModel()
            {
               UpcomingGigs = gigs,
               ShowActions = User.Identity.IsAuthenticated,
               Heading = "Gigs I'm Attending"
            };
            return View("Gigs",viewModel);
        }

        //
        [Authorize]
        public ActionResult Create()
        {
            var viewModel = new GigFormViewModel
                {
                    Heading = "Add a Gig",
                    Genres = _context.Genres.ToList()
                };
            return View("GigForm", viewModel);

        }
        
        [Authorize]
        public ActionResult Edit(int id)
        {
            var userId = User.Identity.GetUserId();
            var gig = _context.Gigs.Single(g => g.Id == id && g.ArtistId == userId);

            var viewModel = new GigFormViewModel
                {
                    Heading = "Edit a Gig",
                    Id = gig.Id,
                    Genres = _context.Genres.ToList(),
                    Date = gig.DateTime.ToString("d MMM yyyy"),
                    Time = gig.DateTime.ToString("HH:mm"),
                    GenreId = gig.GenreId,
                    Venue = gig.Venue

                };
            return View("GigForm", viewModel);

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GigFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Genres =  _context.Genres.ToList();
                return View("GigForm", viewModel);
            }

            var gig = new Gig
            {
                ArtistId = User.Identity.GetUserId(),
                DateTime = viewModel.GetDateTime(),
                GenreId = viewModel.GenreId,
                Venue = viewModel.Venue
            };

            _context.Gigs.Add(gig);
            _context.SaveChanges();

            return RedirectToAction("Mine", "Gigs");
        }
        
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(GigFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Genres =  _context.Genres.ToList();
                return View("GigForm", viewModel);
            }

            var userId = User.Identity.GetUserId();
            var gig = _context.Gigs
                .Include(g => g.Attendances.Select(a => a.Attendee))
                .Single(g => g.Id == viewModel.Id && g.ArtistId == userId);

            gig.Modify(viewModel.GetDateTime(), viewModel.Venue, viewModel.GenreId);

            _context.SaveChanges();

            return RedirectToAction("Mine", "Gigs");
        }
    }
}
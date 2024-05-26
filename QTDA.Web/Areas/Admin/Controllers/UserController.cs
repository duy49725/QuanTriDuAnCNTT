using QTDA.DataAccess.Data;
using QTDA.DataAccess.Repository.IRepository;
using QTDA.Models;
using QTDA.Models.ViewModels;
using QTDA.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query;

namespace bulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class UserController : Controller
	{
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IUnitOfWork _unitOfWork;
		public UserController(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
		{
			_unitOfWork = unitOfWork;
			_roleManager = roleManager;
			_userManager = userManager;
			_context = context;
		}
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult RoleManagment(string userId)
		{

			RoleManagementVM RoleVM = new RoleManagementVM()
			{
				ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId),
				RoleList = _roleManager.Roles.Select(i => new SelectListItem
				{
					Text = i.Name,
					Value = i.Name
				}),
			};

			RoleVM.ApplicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == userId))
					.GetAwaiter().GetResult().FirstOrDefault();
			return View(RoleVM);
		}

		[HttpPost]
		public IActionResult RoleManagment(RoleManagementVM roleManagmentVM)
		{

			string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id))
					.GetAwaiter().GetResult().FirstOrDefault();

			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id);


			if (!(roleManagmentVM.ApplicationUser.Role == oldRole))
			{
				_unitOfWork.ApplicationUser.Update(applicationUser);
				_unitOfWork.Save();

				_userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
				_userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();

			}
			return RedirectToAction("Index");
		}


		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
    
            List<ApplicationUser> objUserList2 = _unitOfWork.ApplicationUser.GetAll().ToList();

			foreach (var user in objUserList2)
			{

				user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
			}

			return Json(new { data = objUserList2 });
		}


		[HttpPost]
		public IActionResult LockUnlock([FromBody] string id)
		{

			var objFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
			if (objFromDb == null)
			{
				return Json(new { success = false, message = "Error while Locking/Unlocking" });
			}

			if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
			{
				//user is currently locked and we need to unlock them
				objFromDb.LockoutEnd = DateTime.Now;
			}
			else
			{
				objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
			}
			_unitOfWork.ApplicationUser.Update(objFromDb);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Operation Successful" });
		}

		#endregion
	}
}

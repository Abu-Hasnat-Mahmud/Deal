using Deal.Models;
using Deal.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Deal.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly GTRDBContext _db;
        public IConfiguration Config { get; }
        public AccountController(IAccountRepository accountRepository, GTRDBContext db, IConfiguration config, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _accountRepository = accountRepository;
            _db = db;
            Config = config;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public IActionResult SignUp()
        {
            return View();
        } 
        
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpUserModel model)
        {
            if (ModelState.IsValid)
            {
                var result =await _accountRepository.SignUpUserAsync(model);

                if (result.Succeeded)
                {
                    ModelState.Clear();
                    return RedirectToAction("ConfirmEmail", new { email = model.Email });
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(model);
        }

        public IActionResult SignIn()
        {
            return View();
        } 
        
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInUserModel model)
        {
            if (ModelState.IsValid)
            {
                var result =await _accountRepository.PasswordSignInAsync(model);

                if (result.Succeeded)
                {
                    ModelState.Clear();
                    //AppData.AppData.DefaultConnectionString = Config.GetConnectionString("DefaultConnection");
                    return RedirectToAction("Index","Home");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "User not allowed");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Credential");
                }
            }
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await _accountRepository.Signout();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.ChangePasswordAsync(model);

                if (result.Succeeded)
                {
                    ModelState.Clear();
                    ViewBag.IsSuccess = true;
                    return View();
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet("Confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string uId, string token, string email)
        {
            EmailConfrimModel model = new EmailConfrimModel
            {
                Email = email
            };

            if (!string.IsNullOrEmpty(uId) && !String.IsNullOrEmpty(token))
            {
                token = token.Replace(" ", "+");
                var result = await _accountRepository.ConfirmEmail(uId, token);
                if (result.Succeeded)
                {
                    model.EmailVerified = true;
                }
            }
            return View(model);
        }


        [HttpPost("Confirm-email")]
        public async Task<IActionResult> ConfirmEmail(EmailConfrimModel model)
        {
            var user = await _accountRepository.GetUserByEmailAsync(model.Email);
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    model.EmailVerified = true;
                    return View(model);
                }

                await _accountRepository.GenerateEmailConfirmationTokenAsync(user);
                model.EmailSent = true;
                ModelState.Clear();
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong");
            }
            return View(model);
        }

        [AllowAnonymous, HttpGet("fotgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous, HttpPost("fotgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _accountRepository.GetUserByEmailAsync(model.Email);
                if (user != null)
                {
                    await _accountRepository.GenerateEmailForgotPasswordTokenAsync(user);

                }
                model.EmailSent = true;
                ModelState.Clear();
            }

            return View(model);
        }

        [AllowAnonymous, HttpGet("reset-password")]
        public IActionResult ResetPassword(string uId, string token)
        {
            ResetPasswordModel model = new ResetPasswordModel
            {
                UserId = uId,
                Token = token
            };
            return View(model);
        }

        [AllowAnonymous, HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                model.Token = model.Token.Replace(" ", "+");
                var result = await _accountRepository.ResetPasswordAsync(model);
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    model.IsSuccess = true;
                    return View(model);
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }

            return View(model);
        }


        #region Role 
        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public IActionResult RegisterWithRole()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in _roleManager.Roles)
            {
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            }

            ViewBag.Roles = list;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> RegisterWithRole(RegisterWithRole model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {

                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,

                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    result = await _userManager.AddToRoleAsync(user, model.RoleName);

                    //await signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("ListUsers");

                }
            }

            return RedirectToAction("ListUsers");
        }


        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // We just need to specify a unique role name to create a new role
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                // Saves the role in the underlying AspNetRoles table
                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditRole(string id)
        {
            // Find the role by Role ID
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            // Retrieve all the Users
            foreach (var user in _userManager.Users)
            {
                // If the user is in this role, add the username to
                // Users property of EditRoleViewModel. This model
                // object is then passed to the view for display
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        // This action responds to HttpPost and receives EditRoleViewModel
        [HttpPost]
        [Authorize(Roles = "Super Admin")]

        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;

                // Update the Role using UpdateAsync
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> ListUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            if (users.Count == 0)
            {
                return View("NotFound");
            }

            var model = new List<EditUserViewModel>();
            // GetClaimsAsync retunrs the list of user Claims
            // var userClaims = await userManager.GetClaimsAsync(user);
            // GetRolesAsync returns the list of user Roles
            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var submodel = new EditUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.Name,
                    Roles = userRoles
                };
                model.Add(submodel);
            }
            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditUser(string id)
        {
            ViewBag.userId = id;

            var user = await _userManager.FindByIdAsync(id);
            ViewBag.userName = user.UserName;



            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new List<RoleUserViewModel>();

            foreach (var role in _roleManager.Roles)
            {

                var userRoles = await _userManager.IsInRoleAsync(user, role.Name);

                var roleUserViewModel = new RoleUserViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                };

                if (userRoles)
                {
                    roleUserViewModel.IsSelected = true;
                }
                else
                {
                    roleUserViewModel.IsSelected = false;
                }
                model.Add(roleUserViewModel);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditUser(List<RoleUserViewModel> model, string id)
        {


            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var role = await _roleManager.FindByIdAsync(model[i].RoleId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("ListUsers");
                }
            }

            return RedirectToAction("ListUsers");
        }
        #endregion


    }
}

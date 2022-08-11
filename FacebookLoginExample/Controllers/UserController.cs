using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FacebookLoginExample.Models.Authentication.Entities;
using FacebookLoginExample.Models.Authentication.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FacebookLoginExample.Controllers
{
    public class UserController : Controller
    {
        UserManager<AppUser> _userManager;
        SignInManager<AppUser> _signInManager;
        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        public IActionResult Login(string ReturnUrl)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string ReturnUrl, LoginUserVM loginUserVM)
        {
            AppUser user = await _userManager.FindByNameAsync(loginUserVM.Username);
            if (user != null)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, loginUserVM.Password, true, true);
                if (result.Succeeded)
                    return Redirect(ReturnUrl != null ? ReturnUrl : Url.Action("Index", "Home"));
                else
                    ModelState.AddModelError("Hatalı Giriş", "Yaptığınız giriş talebi geçersizdir.");
            }
            else
                ModelState.AddModelError("Olmayan Kullanıcı", "Kullanıcı adı veya şifre yanlış.");
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserVM createUserVM)
        {
            AppUser user = createUserVM;
            IdentityResult result = await _userManager.CreateAsync(user, createUserVM.Password);
            if (result.Succeeded)
                return RedirectToAction("Login");
            else
                ModelState.AddModelError("Kayıt Hatası", "Kayıt olurken beklenmedik bir hatayla karşılaşıldı.");
            return View();
        }
        public IActionResult FacebookLogin(string ReturnUrl)
        {
            string redirectUrl = Url.Action("FacebookResponse", "User", new { ReturnUrl = ReturnUrl });
            //Facebook'a yapılan Login talebi neticesinde kullanıcıyı yönlendirmesini istediğimiz url'i oluşturuyoruz.
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
            //Bağlantı kurulacak harici platformun hangisi olduğunu belirtiyor ve bağlantı özelliklerini elde ediyoruz.
            return new ChallengeResult("Facebook", properties);
            //ChallengeResult; kimlik doğrulamak için gerekli olan tüm özellikleri kapsayan AuthenticationProperties nesnesini alır ve ayarlar.
        }
        public async Task<IActionResult> FacebookResponse(string ReturnUrl = "/")
        {
            ExternalLoginInfo loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            //Kullanıcıyla ilgili Facebook'tan gelen tüm bilgileri taşıyan nesnedir.
            //Bu nesnesnin 'LoginProvider' propertysinin değerine göz atarsanız eğer Facebook yazdığını göreceksiniz.
            //Eğer ki, Login işlemi Google yahut Twitter üzerinde gerçekleştirilmiş olsaydı provider olarak ilgili platformun adı yazacaktı.
            if (loginInfo == null)
                return RedirectToAction("Login");
            else
            {
                Microsoft.AspNetCore.Identity.SignInResult loginResult = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, true);
                //Giriş yapıyoruz.
                if (loginResult.Succeeded)
                    return Redirect(ReturnUrl);
                else
                {
                    //Eğer ki akış bu bloğa girerse ilgili kullanıcı uygulamamıza kayıt olmadığından dolayı girişi başarısız demektir.
                    //O halde kayıt işlemini yapıp, ardından giriş yaptırmamız gerekmektedir.
                    AppUser user = new AppUser
                    {
                        Email = loginInfo.Principal.FindFirst(ClaimTypes.Email).Value,
                        UserName = loginInfo.Principal.FindFirst(ClaimTypes.Email).Value
                    };
                    //Facebook'tan gelen Claimleri uygun eşlendikleri propertylere atıyoruz.
                    IdentityResult createResult = await _userManager.CreateAsync(user);
                    //Kullanıcı kaydını yapıyoruz.
                    if (createResult.Succeeded)
                    {
                        //Eğer kayıt başarılıysa ilgili kullanıcı bilgilerini AspNetUserLogins tablosuna kaydetmemiz gerekmektedir ki
                        //bir sonraki Facebook login talebinde Identity mimarisi ilgili kullanıcının Facebook'tan geldiğini anlayabilsin.
                        IdentityResult addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
                        //Kullanıcı bilgileri Facebook'tan gelen bilgileriyle AspNetUserLogins tablosunda eşleştirilmek suretiyle kaydedilmiştir.
                        if (addLoginResult.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, true);
                            //await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, true);
                            return Redirect(ReturnUrl);
                        }
                    }

                }
            }
            return Redirect(ReturnUrl);
        }
    }
}
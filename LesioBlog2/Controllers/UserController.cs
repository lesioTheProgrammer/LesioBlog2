using LesioBlog2_Repo.Abstract;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace LesioBlog2.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepo _user;
        private readonly IGender _gender;
        private readonly ICommentRepo _comm;
        private readonly IWpisRepo _wpisRepo;


        public UserController(IUserRepo userrepo, IGender gender, ICommentRepo comments, IWpisRepo wpis)
        {
            this._user = userrepo;
            this._gender = gender;
            this._comm = comments;
            this._wpisRepo = wpis;

        }

        // GET: User


        [HttpGet]
        public ActionResult Index(string Name)
        {
            bool isUserLogged = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
           
            if (isUserLogged && string.IsNullOrEmpty(Name))
            {
              var FakeUser = System.Web.HttpContext.Current.User;
              string userName = FakeUser.Identity.Name;
                //search by name
                //cant be empty
                var user = _user.GetUserByNickname(userName);
                //gender nie ma kolumny userID

                user.Gender = _gender.GetGenderByID(user.GenderID);
                user.Comments = _comm.GetCommentByUserID(user.UserID);
                user.Wpis = _wpisRepo.GetWpisByUserID(user.UserID);

                return View(user);
            }
            else if ((isUserLogged && !string.IsNullOrEmpty(Name)))
            {
                var user = _user.GetUserByNickname(Name);

                user.Gender = _gender.GetGenderByID(user.GenderID);
                user.Comments = _comm.GetCommentByUserID(user.UserID);
                user.Wpis = _wpisRepo.GetWpisByUserID(user.UserID);

                return View(user);
            }

            else
            {
                return View();
            }

        }

        //post searching user 
        [HttpPost]
        public ActionResult Index(LesioBlog2_Repo.Models.User user)
        {
            //check if user is logged in
            bool isUserLogged = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
           //nothing to post in displaying user state
           
                return View();
        }


        [HttpGet]
        public ActionResult LogIn()
        {
            return View();  //gut
        }
        [HttpPost]
        public ActionResult LogIn(LesioBlog2_Repo.Models.User user)
        {
            //got user with some details
           // if (ModelState.IsValid) --checking only email and password, not all 
            {
                if (IsValid(user.Email, user.Password))
                {
                     //to get user nickname
                    FormsAuthentication.SetAuthCookie(_user.GetUserNicknameByEmail(user.Email), true);  //this decides which value goes to user.identiy name
                    return RedirectToAction("Index", "Wpis");
                }

                else
                {
                    ModelState.AddModelError("", "Login data is incorrect.");
                   
                }
            }
            return View(user);


        }


        [HttpGet]
        public ActionResult Registration()
        {

            var genderlist = this._gender.GetGenders();
            ViewBag.GenderID = new SelectList(genderlist, "GenderID", "GenderName", genderlist.Select(x=>x.GenderID));

            return View();
        }

        [HttpPost]
        public ActionResult Registration([Bind(Include = "Email,Password,UserID,NickName,FullName,City,GenderID")]LesioBlog2_Repo.Models.User user)
        {

            //checking if email and nickname taken
            
            if (IsEmailUsernameTaken(user.Email, user.NickName))
            {
                if (ModelState.IsValid)  //password and email form checking
                {
                    var crypto = new SimpleCrypto.PBKDF2();
                    var encrpPass = crypto.Compute(user.Password);
                    user.Password = encrpPass;
                    user.PasswordSalt = crypto.Salt;
                    //getting unique userID, checking with the database and repeating if userID selected
                    //by random was not unique at all XD
                    #region 
                    //userID
                    var rnd = new Random();
                    user.UserID = rnd.Next();
                    var matchingUser = _user.FindUserByID(user.UserID);
                    while (matchingUser != null)
                    {
                        user.UserID = rnd.Next();
                        matchingUser = _user.FindUserByID(user.UserID);
                    }
                    #endregion




                    _user.Add(user);
                    _user.SaveChanges();
                    return RedirectToAction("LogIn", "User");

                }
                else
                {
                    ModelState.AddModelError("", "Regiser data is incorrect");
                }

            }
            else
            {
                ModelState.AddModelError("", "Email/Username taken, change it please");
            }

            


            return View(user);
        }

        public ActionResult LogOut()
        {

            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Wpis");
        }



        //validate username and password

        private bool IsValid(string email, string password)
        {
            var crypto = new SimpleCrypto.PBKDF2();



            bool IsValid = false;

            //get the user by email
            //creating userrepo 
            var user = _user.GetUserByEmail(email);
            if (user != null)
            {
                if(user.Password == crypto.Compute(password, user.PasswordSalt))
                {
                    IsValid = true; 
                }
            }
            return IsValid;
        }

        private bool IsEmailUsernameTaken(string email, string username)
        {
            //mielimy mielonke kurwiczki
            bool IsEUValid = false;
            //if theres user with same email and username
            var user = _user.GetUserByEmail(email);
            if (user != null)
            {
                if(user.Email != email && user.NickName.ToLower() != username.ToLower())
                {
                    IsEUValid = true;
                }
            }
            //if user exist with same nickname but diff emails
            var user2 = _user.GetUserByNickname(username);

            if (user2 != null)
            {
                if (user2.NickName.ToLower() != username.ToLower())
                {
                    IsEUValid = true;
                }
                else
                {
                    IsEUValid = false;
                }
            }
            //if does not exist at all
            if (user == null && user2 == null)
            {
                IsEUValid = true;
            }
            return IsEUValid;
        }




    }
}
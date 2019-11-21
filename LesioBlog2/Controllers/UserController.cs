using LesioBlog2.ViewModel;
using LesioBlog2_Repo.Abstract;
using LesioBlog2_Repo.Models;
using System;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;

namespace LesioBlog2.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepo _user;
        private readonly IGender _gender;
        private readonly ICommentRepo _comm;
        private readonly IPostRepo _postrepo;


        public UserController(IUserRepo userrepo, IGender gender, ICommentRepo comments, IPostRepo post)
        {
            this._user = userrepo;
            this._gender = gender;
            this._comm = comments;
            this._postrepo = post;

        }

        // GET: User


        [HttpGet]
        public ActionResult Index(string name)
        {
            bool isUserLogged = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            var user = _user.GetUserByNickname(name);


            if (isUserLogged && string.IsNullOrEmpty(name) || isUserLogged && user == null)
            {
                var fakeUser = System.Web.HttpContext.Current.User;
                string userName = fakeUser.Identity.Name;
                //search by name
                //cant be empty
                user = _user.GetUserByNickname(userName);

                user.Gender = _gender.GetGenderByID(user.Gender_Id);
                user.Comments = _comm.GetCommentByUserID(user.User_Id);
                user.Post = _postrepo.GetPostByUserID(user.User_Id);

                return View(user);
            }
            else if ((isUserLogged && !string.IsNullOrEmpty(name)))
            {
                user = _user.GetUserByNickname(name);

                user.Gender = _gender.GetGenderByID(user.Gender_Id);
                user.Comments = _comm.GetCommentByUserID(user.User_Id);
                user.Post = _postrepo.GetPostByUserID(user.User_Id);

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
                    return RedirectToAction("Index", "Post");
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
            //no need of genderlist, doing genderSelection in View.
            var genderlist = this._gender.GetGenders();
            return View();
        }

        [HttpPost]
        public ActionResult Registration([Bind(Include = "Email,Password,User_Id,NickName,FullName,City,Gender_Id")]LesioBlog2_Repo.Models.User user)
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
                    user.User_Id = rnd.Next();
                    var matchingUser = _user.FindUserByID(user.User_Id);
                    while (matchingUser != null)
                    {
                        user.User_Id = rnd.Next();
                        matchingUser = _user.FindUserByID(user.User_Id);
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
            return RedirectToAction("Index", "Post");
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
                if (user.Password == crypto.Compute(password, user.PasswordSalt))
                {
                    IsValid = true;
                }
            }
            return IsValid;
        }

        private bool IsEmailUsernameTaken(string email, string username)
        {
            bool IsEUValid = false;
            //if theres user with same email and username
            var user = _user.GetUserByEmail(email);
            if (user != null)
            {
                if (user.Email != email && user.NickName.ToLower() != username.ToLower())
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

        [HttpGet]
        public ActionResult ResetPasswordSent()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPasswordSent(User user)
        {
            if (!string.IsNullOrEmpty(user.Email))
            {
                bool ifExist = _user.CheckIfUserEmailVaild(user.Email);
                if (ifExist == true)
                {
                    int userId = _user.GetUserByEmail(user.Email).User_Id;
                    user = _user.GetUserByEmail(user.Email);

                    //wyslij
                    SmtpClient client = new SmtpClient();
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("lesio.blog@gmail.com");
                    mailMessage.To.Add(user.Email);
                    mailMessage.Subject = "Reset your password";

                    //userID
                    var rnd = new Random();
                    user.Code = rnd.Next(10000, int.MaxValue);
                    _user.SaveChanges();
                    var code = user.Code;
                    var callback = Url.Action("ResetPassword", "User", new { userID = userId, code = code }, protocol: Request.Url.Scheme);
                    mailMessage.Body = "Reset your password here \n" + callback;
                    client.Send(mailMessage);
                    return RedirectToAction("Index", "Post");

                }
                else
                {
                    ModelState.AddModelError("", "Email is incorrect, change it please");
                }
            }

            else
            {
                ModelState.AddModelError("", "Email is incorrect, change it please");
            }

            return RedirectToAction("Index", "Post");

        }

        [HttpGet]
        public ActionResult ResetPassword(int userID, int code)
        {
            var model = new ID_CodeViewModel()
            {
                Code = code,
                ID = userID
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult ResetPassword(int userID, int code, string password)
        {

            var user = _user.GetUserByID(userID);
            var pass = password;
            if (user.Code == code && user.User_Id == userID)
            {
                //set new password and reset the code

                var crypto = new SimpleCrypto.PBKDF2();
                var encrpPass = crypto.Compute(pass);
                user.Password = encrpPass;
                user.PasswordSalt = crypto.Salt;

                var random = new Random();

                user.Code = random.Next(10000, int.MaxValue);

                _user.SaveChanges();
            }
            else
            {
                return RedirectToAction("ResetPasswordSent", "User");
            }
            return RedirectToAction("LogIn", "User");
        }
    }
}
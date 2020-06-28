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
        private readonly ICodeRepo _coderepo;

        public UserController(IUserRepo userrepo, IGender gender, ICommentRepo comments, IPostRepo post, ICodeRepo code)
        {
            this._user = userrepo;
            this._gender = gender;
            this._comm = comments;
            this._postrepo = post;
            this._coderepo = code;
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
            return View(new User());  
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
            return View(new User());
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
                    //default values:
                    user.Role_Id = 2; //default
                    user.Active = true;
                    //deafult end
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
                    user = _user.GetUserByEmail(user.Email);
                    //user generate code 
                    var code  =  _coderepo.AddCode(user.User_Id);
                    user.Code_Id = user.User_Id; //uID to code is 1 to 1
                    _user.SaveChanges();
                    var callback = Url.Action("ResetPassword", "User", new { userID = user.User_Id, code = code }, protocol: Request.Url.Scheme);
                    //  new mail message
                    #region
                    MailMessage mail = new MailMessage("lesio.blog@gmail.com", user.Email);
                    mail.Subject = "Reset your password";
                    mail.Body = "Reset your password here \n" + callback;
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                    smtpClient.Credentials = new System.Net.NetworkCredential()
                    {
                        UserName = "lesio.blog@gmail.com",
                        Password = "lesio222pies"
                    };
                    smtpClient.EnableSsl = true;
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                            System.Security.Cryptography.X509Certificates.X509Chain chain,
                            System.Net.Security.SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    };

                    smtpClient.Send(mail);
                    #endregion
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
        public ActionResult ResetPasswordSent(ID_CodeViewModel model)
        {
            var user = _user.GetUserByIDAndCode(model.ID);
            var pass = model.Password;
            int? codex = _coderepo.GetCodeValue(model.ID); // id is userid
            if (model.Code == codex && user.User_Id == model.ID)
            {
                //set new password and reset the code
                var crypto = new SimpleCrypto.PBKDF2();
                var encrpPass = crypto.Compute(pass);
                user.Password = encrpPass;
                user.PasswordSalt = crypto.Salt;
                var random = new Random();
                user.Code.CodeValue = random.Next(10000, int.MaxValue);
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
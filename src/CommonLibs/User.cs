using System.ComponentModel.DataAnnotations;

namespace CommonLibs
{
    public class User 
    {
        [Display(Name = "Username")]
        public string Login { get; set; }

        public string AvatarUrl { get; set; }

        public override bool Equals(object obj)
        {
            return obj is User user &&
                   Login == user.Login;
        }

        public override int GetHashCode()
        {
            return Login.GetHashCode();
        }

        public static User GetSystemUser()
        {
            return new User
            {
                Login = "system",
                AvatarUrl = "/brand.png"
            };
        }
    }
}
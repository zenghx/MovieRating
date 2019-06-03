using System;

namespace MovieRating
{
    class Userinfo
    {
        public static int currentUser = 37787;
        public static bool IsAdmin() => currentUser == 0;
    }
}

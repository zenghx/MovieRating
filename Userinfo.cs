﻿namespace MovieRating
{
    class Userinfo
    {
        public static string adminpwd = "065153dcc052bfad718b7fc59241cbbf0d845af5";//管理员账户密码的sha1值
        public static int currentUser = 37787;//初始uid，不在数据库内的数字
        public static bool IsAdmin() => currentUser == 0;
    }
}

using MyChatBot.Bussiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MyChatBot.Controllers
{
    public class UserController
    {
        public static DataTable getAllUsers()
        {
            UserBUS user = new UserBUS();
            return user.getAllUsers();
        }
    }
}
using MyChatBot.DataAccess;
using MyChatBot.ValueObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MyChatBot.Bussiness
{
    public class UserBUS
    {
        private UserDA _userDA;
        public UserBUS()
        {
            _userDA = new UserDA();
        }

        public DataTable getAllUsers()
        {
            UserVO user = new UserVO();
            DataTable dataTable = new DataTable();
            dataTable = _userDA.getAllUsers();
            return dataTable;
        }
    }
}
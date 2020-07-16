//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Data.Entity.Core.EntityClient;
//using System.Configuration;

//namespace API.Invoice.DB
//{
//    public class Connection 
//    {
//        private static ZubairEntities zubairEntities = null;
//        private static readonly object padlock = new object();

//        private static Connection _instance;
//        protected Connection()
//        {
//        }
//        public static Connection Instance
//        {
//            get
//            {
//                if (_instance == null)
//                    _instance = new Connection();

//                return _instance;
//            }
//        }

//        public ZubairEntities DbContext
//        {
//            get
//            {
//                lock (padlock)
//                {
//                    if (zubairEntities == null)
//                    {
//                        zubairEntities = new ZubairEntities();
//                    }
//                    return zubairEntities;
//                }
//            }
//        }

//    }
//}
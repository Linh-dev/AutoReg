using Bussiness.Dao;
using Bussiness.Models;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.DTOs
{
    public class RegModel
    {
        public IWebDriver Driver { get; set; }
        public PersonalInfo Info { get; set; }
    }
}

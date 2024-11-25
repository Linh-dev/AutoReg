using Bussiness.Business;
using Bussiness.Dao;
using Bussiness.DTOs;
using Bussiness.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OfficeOpenXml;
using OpenQA.Selenium;
using System.Diagnostics;

namespace ToolRegGoethe.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult IndexNew()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<JsonResult> GetAll()
        {
            try
            {
                var data = ConfigDao.GetInstance().GetAll();
                foreach (var item in data)
                {
                    var pList = PersonalDao.GetInstance().GetByConfigId(item._id);
                    item.CountPersonal = pList.Count;
                    item.CountSuccess = pList.Where(p => p.IsSuccess).ToList().Count;
                    item.CountFailure = pList.Where(p => !p.IsSuccess).ToList().Count;
                }
                return Json(new
                {
                    Data = data,
                    ReturnCode = 1,
                    Message = "Thành công"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ReturnCode = 0,
                    Message = "Lỗi"
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetAllNew()
        {
            try
            {
                var data = PersonalDao.GetInstance().GetAll();
                return Json(new
                {
                    Data = data,
                    ReturnCode = 1,
                    Message = "Thành công"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ReturnCode = 0,
                    Message = "Lỗi"
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeteleJson([FromBody] BaseRequest reqData)
        {
            try
            {
                var _id = ObjectId.Parse(reqData.IdStr);
                var pList = PersonalDao.GetInstance().GetByConfigId(_id);
                foreach (var item in pList)
                {
                    PersonalDao.GetInstance().Delete(item._id);
                }
                ConfigDao.GetInstance().Delete(_id);
                return Json(new
                {
                    ReturnCode = 1,
                    Message = "Thành công"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ReturnCode = 0,
                    Message = "Lỗi"
                });
            }
        }

        private object lockObject = new object();
        [HttpPost]
        public async Task<JsonResult> StartReg([FromBody] BaseRequest reqData)
        {
            try
            {
                string[] proxyList = new string[]
                {
                };
                var configInfo = ConfigDao.GetInstance().GetById(reqData.IdStr);
                var pList = PersonalDao.GetInstance().GetByConfigId(configInfo._id);
                for(var i = 0; i < pList.Count; i++)
                {
                    pList[i].IndexA = i;
                }
                var regModelList = new List<RegModel>();

                //mở chrome
                #region
                pList.AsParallel().ForAll(info =>
                {
                    var d = new RegBussiness().OpenNewChrome(configInfo, info, proxyList[info.IndexA]);
                    RegModel model = new RegModel();
                    model.Driver = d;
                    model.Info = info;
                    lock (lockObject)
                    {
                        regModelList.Add(model);
                    }
                });
                #endregion

                ////check
                //var check = true;
                var check = new RegBussiness().CheckActive(configInfo);
                var semaphore = new SemaphoreSlim(5);
                //thuc hien dang ký
                await Parallel.ForEachAsync(regModelList, async (item, cancellationToken) =>
                {
                    await semaphore.WaitAsync(cancellationToken); // Kiểm soát luồng

                    try
                    {
                        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(4)); // Thời gian tối đa 5 giây
                        await Task.Run(() =>
                        {
                            new RegBussiness().RegAction(item.Driver, configInfo, item.Info);
                        }, cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine($"Processing of {item.Driver} was canceled after 4 seconds.");
                    }
                    finally
                    {
                        semaphore.Release(); // Giải phóng luồng
                    }
                });

                return Json(new
                {
                    ReturnCode = 1,
                    Message = "Thành công"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ReturnCode = 0,
                    Message = "Lỗi"
                });
            }
        }


        [HttpPost]
        public async Task<JsonResult> StartRegNew([FromBody] BaseRequest reqData)
        {
            try
            {
                var personalInfo = PersonalDao.GetInstance().GetById(reqData.IdStr);
                var configInfo = ConfigDao.GetInstance().GetById(personalInfo.ConfigId);

                var drivers = new List<IWebDriver>();

                new RegBussiness().Reg(configInfo, personalInfo);

                return Json(new
                {
                    ReturnCode = 1,
                    Message = "Thành công"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ReturnCode = 0,
                    Message = "Lỗi"
                });
            }
        }



        [HttpPost]
        public async Task<IActionResult> UploadExcelFile()
        {
            try
            {
                var profilePath = "D:\\ChromeProfile\\UserData";
                var profileName = "Profile ";
                var file = Request.Form.Files[0];
                if (file != null && file.Length > 0)
                {
                    // Đọc nội dung file Excel trực tiếp từ IFormFile
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        stream.Position = 0; // Đặt lại vị trí của stream về đầu
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets[0]; // Lấy worksheet đầu tiên
                            var rowCount = worksheet.Dimension.Rows; // Số lượng hàng
                            var colCount = 6; // Số lượng cột

                            var confInfo = new ConfigInfo();
                            confInfo._id = ObjectId.GenerateNewId();
                            confInfo.CreatedDate = DateTime.Now;
                            confInfo.Name = worksheet.Cells["B" + 1].Text.Trim();
                            confInfo.Link = worksheet.Cells["B" + 2].Text.Trim();
                            confInfo.StartTimeStr = worksheet.Cells["B" + 3].Text.Trim();

                            ConfigDao.GetInstance().Insert(confInfo);

                            var data = new List<PersonalInfo>();
                            var profileIndex = 4;
                            // Đọc dữ liệu từ worksheet
                            for (int i = 5; i <= rowCount; i++)
                            {
                                //Tài khoản
                                var cellA = worksheet.Cells["A" + i];
                                var username = cellA == null ? "" : cellA.Text.Trim();
                                //Mật khẩu
                                var cellB = worksheet.Cells["B" + i];
                                var pw = cellB == null ? "" : cellB.Text.Trim();
                                //Reading
                                var cellC = worksheet.Cells["C" + i];
                                var reading = cellC == null ? "" : cellC.Text.Trim();
                                //Listening
                                var cellD = worksheet.Cells["D" + i];
                                var listening = cellD == null ? "" : cellD.Text.Trim();
                                //Writing
                                var cellE = worksheet.Cells["E" + i];
                                var writing = cellE == null ? "" : cellE.Text.Trim();
                                //Speaking
                                var cellF = worksheet.Cells["F" + i];
                                var speaking = cellF == null ? "" : cellF.Text.Trim();

                                if (string.IsNullOrEmpty(username)) break;

                                var personalInfo = new PersonalInfo();
                                personalInfo.ConfigId = confInfo._id;
                                personalInfo.CreatedDate = DateTime.Now;
                                personalInfo.Username = username;
                                personalInfo.Password = pw;
                                personalInfo.IsReading = !string.IsNullOrEmpty(reading);
                                personalInfo.IsListening = !string.IsNullOrEmpty(listening);
                                personalInfo.IsWriting = !string.IsNullOrEmpty(writing);
                                personalInfo.IsSpeaking = !string.IsNullOrEmpty(speaking);
                                personalInfo.ProfilePath = profilePath + profileIndex.ToString();
                                personalInfo.ProfileName = profileName + profileIndex.ToString();
                                data.Add(personalInfo);
                                profileIndex++;
                            }

                            PersonalDao.GetInstance().InsertRange(data);
                        }
                    }
                }
                return Ok(new { message = "File uploaded and read successfully!" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "File upload failed." });

            }

        }
    }
}


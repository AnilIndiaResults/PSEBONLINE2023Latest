using Org.BouncyCastle.Asn1.Ocsp;
using PSEBONLINE.Models;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using encrypt;

namespace PSEBONLINE.Controllers
{
    public class AdmitCardController : Controller
    {

        public Byte[] QRCoder(string qr)
        {
            QRCodeGenerator _qrCode = new QRCodeGenerator();
            QRCodeData _qrCodeData = _qrCode.CreateQrCode("https://registration2022.pseb.ac.in/AdmitCard/Index/" + QueryStringModule.Encrypt(qr), QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(_qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return (BitmapToBytesCode(qrCodeImage));

        }
        [NonAction]
        private static Byte[] BitmapToBytesCode(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }




        // GET: AdmitCard
        public ActionResult Index(string id)
        {
            ViewBag.TotalCount = 0;
            ViewBag.Message = "";


            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Message = "URL is not valid, Please try after sometimes.";
                return View();
            }
            else
            {
                try
                {

                    string sID = id;
                    try
                    {
                        sID = QueryStringModule.Decrypt(id);
                    }
                    catch
                    {
                        sID = id;
                    }
                    id = sID;

                    try
                    {
                        //ViewBag.SelectedItem
                        AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                        RegistrationModels rm = new RegistrationModels();
						rm.QRCode = QRCoder(id);
						string ClsType = "4";
                        rm.Correctiondata = null;
                        ViewBag.TotalCountadded = "";
                        rm.ExamRoll = id;
                        string search = "";
                        search = " 1=1 ";

                        if (rm.ExamRoll.Trim() != "")
                        {
                            search += " and exm.ROLL='" + rm.ExamRoll + "'";
                        }
                        rm.StoreAllData = objDB.GetFinalPrintSeniorAdmitCardSearch(search, "All");
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                        if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ClsType = "4";
                            rm.Correctiondata = null;
                            ViewBag.TotalCountadded = "";
                            rm.ExamRoll = id;
                            search = " 1=1 ";
                            search += " and exm.ROLL='" + rm.ExamRoll + "'";
                           
                            rm.StoreAllData = objDB.GetFinalPrintSeniorOpenAdmitCardSearch(search, "All");
                            ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                            if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                            {
                                ClsType = "2";
                                rm.Correctiondata = null;
                                ViewBag.TotalCountadded = "";
                                rm.ExamRoll = id;
                                search = " 1=1 ";
                                search += " and exm.ROLL='" + rm.ExamRoll + "'";
                                rm.StoreAllData = objDB.GetFinalPrintMatricAdmitCardSearch(search, "All");
                                ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                                if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                                {
                                    ClsType = "4";
                                    rm.Correctiondata = null;
                                    ViewBag.TotalCountadded = "";

                                    rm.ExamRoll = id;
                                    search = " 1=1 ";
                                    search += " and exm.ROLL='" + rm.ExamRoll + "'";

                                    rm.StoreAllData = objDB.GetFinalPrintMatricOpenAdmitCardSearch(search, "All");
                                    ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                                    {
                                        ViewBag.TotalCount = 0;
                                        ViewBag.Message = "Invalid Roll Number Please check the URL.";
                                    }
                                }
                            }
                        }
                        if (ModelState.IsValid)
                        { return View(rm); }
                        else
                        { return View(); }
                    }
                    catch
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.Message = "Some Error occur, Please try after sometimes.";
                    }
                }
                catch
                {
                    ViewBag.TotalCount = 0;
                    ViewBag.Message = "Some Error occur, Please try after sometimes.";
                }
                return View();
            }
        }
    }
}
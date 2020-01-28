using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealStateAPI.Models;
using RealStateAPI.Service;

namespace RealStateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {

        private readonly IS3Service _service;

        public AttachmentController(IS3Service S3service)
        {
            this._service = S3service;

        }
        [HttpGet]
        [Route("getObject/{s3Key}")]
        public async Task<IActionResult> getS3Object(string s3Key)
        {
            try
            {
                Stream imageStream = await _service.getObject(s3Key);
                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    FileName = "Image.jpg",
                    Inline = true
                }.ToString());

                return File(imageStream, "image/jpeg");
            }
            catch (AmazonS3Exception e)
            {

                Response.StatusCode = 400;
                return Content(e.Message);
            }

            catch (Exception e)
            {
                Response.StatusCode = 400;
                return Content(e.ToString());
            }
        }
        [HttpGet]
        [Route("getObject/{s3Key}/{width}/{heigth}")]
        public async Task<IActionResult> getResizedIMageFromS3(string s3Key, int width, int heigth)
        {
            try
            {
                Stream imageStream = await _service.getObject(s3Key);
                var Image = new Bitmap(imageStream);
                var resizedImage = new Bitmap(Image, new Size(width, heigth));
                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    FileName = "Image.jpg",
                    Inline = true
                }.ToString());
                var ms = new MemoryStream();
                resizedImage.Save(ms, ImageFormat.Jpeg);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "image/jpeg");
            }
            catch (Exception e)
            {

                Response.StatusCode = 400;
                return Content(e.Message);
            }

        }

        [HttpPost]
        [Route("upload")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<IActionResult> uploadObject([FromBody] List<AttachmentModel> imageModels)
        {
            int count = 0;
            foreach (AttachmentModel imageModel in imageModels)
            {
                try
                {
                    await _service.uploadFile(imageModel.FileName, imageModel.FilePath);
                }
                catch (Exception e)
                {

                    Response.StatusCode = 400;
                    return Content($"uploaded first {count} but encontered folloing error {e.Message} when uploading {imageModel.FileName}");
                }
                count++;
            }
            Response.StatusCode = 200;
            return Content($"uploaded {count} images");
        }

    }
}
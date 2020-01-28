using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using RealStateAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateAPI.Service
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3service;

        public S3Service(IAmazonS3 s3service)
        {
            _s3service = s3service;
        }

        public async Task<MemoryStream> getObject(string s3Key)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = "realstate-app-photos",
                    Key = s3Key
                };

                using (var getObjectResponse = await _s3service.GetObjectAsync(request))
                {
                    using (var responseStream = getObjectResponse.ResponseStream)
                    {
                        var stream = new MemoryStream();
                        await responseStream.CopyToAsync(stream);
                        stream.Position = 0;
                        return stream;
                    }
                }
            }
            catch (AmazonS3Exception e)
            {
                throw (e);
            }
            catch (Exception e)
            {
                throw (e);
            }

        }

        public async Task uploadFile(string fileName , string filepath)
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = "realstate-app-photos",
                FilePath = filepath,
                Key = fileName
            };
            try
            {
                await _s3service.PutObjectAsync(putRequest);
            }
            catch (AmazonS3Exception e)
            {
                throw (e);
            }
            catch (Exception e)
            {
                throw (e);
            }

        }
    }
}

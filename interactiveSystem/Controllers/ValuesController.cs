using interactiveSystem.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace interactiveSystem.Controllers
{
    public class ValuesController : ApiController
    {
        public static readonly IReadOnlyDictionary<string, string> MimeNames;
        public static readonly IReadOnlyCollection<char> InvalidFileNameChars;
        private readonly HttpClient httpClient = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true }) { BaseAddress = new Uri("http://portal.gd-urengoy.gazprom.ru") };

        static ValuesController()
        {
            var mimeNames = new Dictionary<string, string>();

            MimeNames = new Dictionary<string, string>
            {
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".png", "image/png" },
                { ".gif", "image/gif" }
            };

            InvalidFileNameChars = Array.AsReadOnly(Path.GetInvalidFileNameChars());
        }

        public HttpResponseMessage GetManPhoto(int id)
        {
            using (ISContext db = new ISContext())
            {
                var man = db.Men.SingleOrDefault(m => m.Id == id);
                if (man != null)
                {
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {

                        Content = new ByteArrayContent(man.Photo)
                        //Content = new ByteArrayContent(File.ReadAllBytes(@"D:\работа\anonsTV.jpg"))

                    };
                    result.Content.Headers.ContentType =
                        new MediaTypeHeaderValue("image/png");

                    return result;
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        public HttpResponseMessage GetLeadershipPhoto(int id)
        {
            using (ISContext db = new ISContext())
            {
                var leadership = db.Leaderships.SingleOrDefault(l => l.Id == id);
                if (leadership != null)
                {
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {

                        Content = new ByteArrayContent(leadership.Photo)
                        //Content = new ByteArrayContent(File.ReadAllBytes(@"D:\работа\anonsTV.jpg"))

                    };
                    result.Content.Headers.ContentType =
                        new MediaTypeHeaderValue("image/png");

                    return result;
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        public HttpResponseMessage GetEnterprisePhoto(int id)
        {
            using (ISContext db = new ISContext())
            {
                var enterprise = db.Enterprises.SingleOrDefault(e => e.Id == id);
                if (enterprise != null)
                {
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(enterprise.Photo)
                    };
                    result.Content.Headers.ContentType =
                        new MediaTypeHeaderValue("image/png");

                    return result;
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        public HttpResponseMessage GetRewardPhoto(int id)
        {
            using (ISContext db = new ISContext())
            {
                var reward = db.Rewards.SingleOrDefault(r => r.Id == id);
                if (reward != null)
                {
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {

                        Content = new ByteArrayContent(reward.Photo)
                        //Content = new ByteArrayContent(File.ReadAllBytes(@"D:\работа\anonsTV.jpg"))
                        
                    };
                    result.Content.Headers.ContentType =
                        new MediaTypeHeaderValue("image/png");

                    return result;
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            // processing the stream.

            
        }
        //// GET api/values
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //public void Delete(int id)
        //{
        //}

        //[Route("api/Values/Image/{folderName}/{fileName}", Name = "image")]
       
        [HttpGet]
        public HttpResponseMessage EventCalendarImage(string folderName, string fileName)
        {
            string dir = @"\\gd-urengoy\ГДУ\МУЗЕЙ";
            string dirName = Path.Combine(dir, folderName);
            string fullFileName = Path.Combine(dirName, fileName);
            if (!Directory.Exists(dirName) || !File.Exists(fullFileName))
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            try
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(File.ReadAllBytes(fullFileName))
                };

                //response.Content.Headers.ContentType = GetMimeNameFromExt(result.FileExtension);
                var fi = new FileInfo(fullFileName);
                response.Content.Headers.ContentLength = fi.Length;
                response.Content.Headers.ContentType = GetMimeNameFromExt(fi.Extension);
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponseMessage Image(string folderName, string fileName)
        {
            string dir = @"\\gd-urengoy\ГДУ\МУЗЕЙ\Фотогалерея";
            string dirName = Path.Combine(dir, folderName);
            string fullFileName = Path.Combine(dirName, fileName);
            if (!Directory.Exists(dirName) || !File.Exists(fullFileName))
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            try
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(File.ReadAllBytes(fullFileName))
                };

                //response.Content.Headers.ContentType = GetMimeNameFromExt(result.FileExtension);
                var fi = new FileInfo(fullFileName);
                response.Content.Headers.ContentLength = fi.Length;
                response.Content.Headers.ContentType = GetMimeNameFromExt(fi.Extension);
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        //[Route("api/Values/Image/{folderName}/{fileName}", Name = "image")]
        [HttpGet]
        public HttpResponseMessage ImagePreview(string folderName, string fileName)
        {
            string dir = @"\\gd-urengoy\ГДУ\МУЗЕЙ\Фотогалерея\!Preview";
            string dirName = Path.Combine(dir, folderName);
            string fullFileName = Path.Combine(dirName, fileName);
            if (!Directory.Exists(dirName) || !File.Exists(fullFileName))
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            try
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(File.ReadAllBytes(fullFileName))
                };

                //response.Content.Headers.ContentType = GetMimeNameFromExt(result.FileExtension);
                var fi = new FileInfo(fullFileName);
                response.Content.Headers.ContentLength = fi.Length;
                response.Content.Headers.ContentType = GetMimeNameFromExt(fi.Extension);
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        //[Route("api/Values/Image/{folderName}/{fileName}", Name = "image")]
        [HttpGet]
        public HttpResponseMessage Book(string orgFolderName, string folderName, string fileName)
        {
            string dir = @"\\gd-urengoy\ГДУ\МУЗЕЙ\Книги подразделений";
            string dirName = Path.Combine(dir, orgFolderName, folderName);
            string fullFileName = Path.Combine(dirName, fileName);
            if (!Directory.Exists(dirName) || !File.Exists(fullFileName))
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            try
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(File.ReadAllBytes(fullFileName))
                };

                //response.Content.Headers.ContentType = GetMimeNameFromExt(result.FileExtension);
                var fi = new FileInfo(fullFileName);
                response.Content.Headers.ContentLength = fi.Length;

                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        //[Route("api/Values/Image/{folderName}/{fileName}", Name = "image")]
        [HttpGet]
        public HttpResponseMessage BookPreview(string orgFolderName, string folderName, string fileName)
        {
            string dir = @"\\gd-urengoy\ГДУ\МУЗЕЙ\Книги подразделений";
            string dirName = Path.Combine(dir, orgFolderName, "!Preview", folderName);
            string fullFileName = Path.Combine(dirName, fileName);
            if (!Directory.Exists(dirName) || !File.Exists(fullFileName))
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            try
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(File.ReadAllBytes(fullFileName))
                };

                //response.Content.Headers.ContentType = GetMimeNameFromExt(result.FileExtension);
                var fi = new FileInfo(fullFileName);
                response.Content.Headers.ContentLength = fi.Length;

                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        private static MediaTypeHeaderValue GetMimeNameFromExt(string ext)
        {
            string value;

            if (MimeNames.TryGetValue(ext.ToLowerInvariant(), out value))
            {
                return new MediaTypeHeaderValue(value);
            }
            else
            {
                return new MediaTypeHeaderValue(MediaTypeNames.Application.Octet);
            }
        }

        [Route("api/values/portal")]
        public async Task<HttpResponseMessage> GetPortalImg([FromUri]string url)
        {
            url = url.ToLower();
            var extention = "image/jpeg";
            string pattern = ".jpg";
            Regex regex = new Regex(pattern);
            if (regex.IsMatch(url))
            {
                extention = "image/jpeg";
            } else {
                pattern = ".jpeg";
                regex = new Regex(pattern);
                if (regex.IsMatch(url))
                {
                    extention = "image/jpeg";
                }
                else
                {
                    pattern = ".bmp";
                    regex = new Regex(pattern);
                    if (regex.IsMatch(url))
                    {
                        extention = "image/bmp";
                    }
                    else
                    {
                        pattern = ".gif";
                        regex = new Regex(pattern);
                        if (regex.IsMatch(url))
                        {
                            extention = "image/gif";
                        }
                        else
                        {
                            pattern = ".png";
                            regex = new Regex(pattern);
                            if (regex.IsMatch(url))
                            {
                                extention = "image/png";
                            }
                            else
                            {
                                pattern = ".tiff";
                                regex = new Regex(pattern);
                                if (regex.IsMatch(url))
                                {
                                    extention = "image/tiff";
                                }
                            }
                        }
                    }
                }
            }
            try
            {
                var bytes = await httpClient.GetByteArrayAsync(url);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(bytes),
                };
                response.Content.Headers.ContentLength = bytes.LongCount();
                response.Content.Headers.ContentType =  new MediaTypeHeaderValue(extention);
                
                return response;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

        }
    }
}

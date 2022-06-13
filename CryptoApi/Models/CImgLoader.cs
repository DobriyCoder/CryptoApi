using System.Net;

namespace Atomex.Models
{
    public class CImgLoader
    {
        public static string? Load(string url, string dir_path, string new_name)
        {
            var url_parts = url.Split("?");
            var file_name = new_name + Path.GetExtension(url_parts[0]);
            string? path = dir_path + file_name;

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(url, path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                file_name = null;
            }

            return file_name;
        }
    }
}

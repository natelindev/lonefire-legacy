using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace lonefire.Services
{
    public class FileIOHelper : IFileIOHelper
    {
        //Save file with random name
        //returns: Suceed-> (string) fileName.ext 
        //         Failed-> null
        public async Task<string> SaveFileAsync(IFormFile file, string savePath, string validate_regex,int file_name_length_limit)
        {
            if (file == null || file.FileName.Length > file_name_length_limit)
            {
                return null;
            }

            var fileName = Path.GetFileName(file.FileName);

            //validate fileName
            var regex = validate_regex;
            var match = Regex.Match(fileName, regex, RegexOptions.IgnoreCase);
            if (file.Length > 0 && fileName.Length <= file_name_length_limit && match.Success)
            {
                //get extension name
                string extName = Path.GetExtension(fileName);
                Random random = new Random();
                fileName = DateTime.Now.ToLocalTime().ToString("yyyyMMddHHmmss") + random.Next(0, 100) + extName;

                //Dir create (No need to check existence)
                try
                {
                    System.IO.Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), savePath));
                }
                catch (Exception)
                {
                    return null;
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), savePath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    try
                    {
                        await file.CopyToAsync(stream);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                    return fileName;
                }
            }

            return null;

        }

        //Save file with specified filename(without ext)
        //returns: Suceed-> (string) fileName.ext 
        //         Failed-> null
        public async Task<string> SaveFileAsync(IFormFile file, string savePath, string saveFileName, string validate_regex, int file_name_length_limit)
        {
            if (file == null || file.FileName.Length > file_name_length_limit)
            {
                return null;
            }

            var fileName = Path.GetFileName(file.FileName);

            //validate fileName
            var regex = validate_regex;
            var match = Regex.Match(fileName, regex, RegexOptions.IgnoreCase);
            if (file.Length > 0 && fileName.Length <= file_name_length_limit && match.Success)
            {
                //get extension name
                string extName = Path.GetExtension(fileName);
                Random random = new Random();
                fileName = saveFileName + extName;

                //Dir create (No need to check existence)
                try
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), savePath));
                }
                catch (Exception)
                {
                    return null;
                }
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), savePath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    try
                    {
                        await file.CopyToAsync(stream);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                    return fileName;
                }
            }
            return null;
        }

        //Delete file with filename.ext & path
        //returns: (string) a message describing result;
        public string DeleteFile(string file_to_delete_path, string file_to_delete, string validate_regex,int file_name_length_limit)
        {
            var match = Regex.Match(file_to_delete, validate_regex, RegexOptions.IgnoreCase);
            string message = null;
            if (file_to_delete == null || file_to_delete.Length > file_name_length_limit || !match.Success)
            {
                message = "文件 " + file_to_delete + "删除失败: 文件名格式错误";
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), file_to_delete_path, file_to_delete);

            try
            {
                // Check if file exists with its full path    
                if (System.IO.File.Exists(filePath))
                {
                    // If file found, delete it    
                    System.IO.File.Delete(filePath);
                    message = "文件删除成功";
                }
                else
                {
                    message = "文件 " + file_to_delete + "删除失败: 文件不存在";
                }
            }
            catch (Exception)
            {
                message = "文件 " + file_to_delete + " 删除失败: IO错误";
            }
            return message;
        }
    }
}

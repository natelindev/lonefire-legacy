using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace lonefire.Services
{
    public class FileIOHelper : IFileIOHelper
    {
        private readonly IToaster _toaster;
        private readonly IConfiguration _config;
        public FileIOHelper(
            IToaster toaster,
            IConfiguration config
            )
        {
            _toaster = toaster;
            _config = config;
        }

        private string ImageUploadPath => "wwwroot" + _config.GetValue<string>("img_upload_path");
        private string FileUploadPath => "wwwroot" + _config.GetValue<string>("file_upload_path");

        public async Task<string> SaveImgAsync(IFormFile img, string savePath)
        {
            return await CopyToAsync(img, ImageUploadPath + savePath, @"^[\s\S]+\.(jpg|gif|png|bmp|jpeg|svg)$", 256);
        }

        public async Task<string> SaveImgAsync(IFormFile img, string savePath, int img_name_length_limit)
        {
            return await CopyToAsync(img, ImageUploadPath + savePath, @"^[\s\S]+\.(jpg|gif|png|bmp|jpeg|svg)$", img_name_length_limit);
        }

        public async Task<string> SaveImgAsync(IFormFile img, string savePath, int img_name_length_limit, string img_name)
        {
            return await CopyToAsync(img, ImageUploadPath + savePath, @"^[\s\S]+\.(jpg|gif|png|bmp|jpeg|svg)$", img_name_length_limit, img_name);
        }

        public bool DeleteImg(string img_to_delete_path, string img_to_delete)
        {
            return Delete(ImageUploadPath + img_to_delete_path, img_to_delete, @"^[\s\S]+\.(jpg|gif|png|bmp|jpeg|svg)$", 256);
        }
        public bool DeleteImg(string img_to_delete_path, string img_to_delete, int img_name_length_limit)
        {
            return Delete(ImageUploadPath + img_to_delete_path, img_to_delete, @"^[\s\S]+\.(jpg|gif|png|bmp|jpeg|svg)$", img_name_length_limit);
        }

        public async Task<string> SaveFileAsync(IFormFile file, string savePath)
        {
            return await CopyToAsync(file, FileUploadPath + savePath, @"[\s\S]+", 256);
        }

        public async Task<string> SaveFileAsync(IFormFile file, string savePath, int file_name_length_limit)
        {
            return await CopyToAsync(file, FileUploadPath + savePath, @"[\s\S]+", file_name_length_limit);
        }

        public async Task<string> SaveFileAsync(IFormFile file, string savePath, int file_name_length_limit, string saveFileName)
        {
            return await CopyToAsync(file, FileUploadPath + savePath, @"[\s\S]+", file_name_length_limit, saveFileName);
        }

        public bool DeleteFile(string file_to_delete_path, string file_to_delete)
        {
            return Delete(FileUploadPath + file_to_delete_path, file_to_delete, @"[\s\S]+", 256);
        }

        public bool DeleteFile(string file_to_delete_path, string file_to_delete, int file_name_length_limit)
        {
            return Delete(FileUploadPath + file_to_delete_path, file_to_delete, @"[\s\S]+", file_name_length_limit);
        }

        public bool MoveImgDir(string src, string dst)
        {
            return MoveDir(ImageUploadPath + src, ImageUploadPath + dst);
        }

        public bool MoveFileDir(string src, string dst)
        {
            return MoveDir(FileUploadPath + src, FileUploadPath + dst);
        }

        public bool MoveDir(string src, string dst)
        {
            if (src == null || dst == null || src.Length > 512 || dst.Length > 512)
            {
                _toaster.ToastInfo("文件夹移动或重命名失败: 目标不存在 或 文件夹名格式错误");
                return false;
            }
            else
            {
                try
                {
                    Directory.Move(src, dst);
                    _toaster.ToastInfo("文件夹移动或重命名成功");
                    return true;
                }
                catch (Exception)
                {
                    _toaster.ToastInfo("文件夹移动或重命名失败: IO错误");
                    return false;
                }
            }
        }

        //Save file with random name
        //returns: Suceed-> (string) fileName.ext 
        //         Failed-> null
        public async Task<string> CopyToAsync(IFormFile file, string savePath, string validate_regex, int file_name_length_limit)
        {
            if (file == null || file.FileName.Length > file_name_length_limit)
            {
                _toaster.ToastInfo("文件上传失败: 文件不存在 或 文件名格式错误");
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

        //Save file with specified filename
        //returns: Suceed-> (string) fileName.ext 
        //         Failed-> null
        public async Task<string> CopyToAsync(IFormFile file, string savePath, string validate_regex, int file_name_length_limit, string saveFileName)
        {
            if (file == null || file.FileName.Length > file_name_length_limit)
            {
                _toaster.ToastInfo("文件上传失败: 文件不存在 或 文件名格式错误");
                return null;
            }

            var fileName = Path.GetFileName(file.FileName);

            //validate fileName
            var regex = validate_regex;
            var match = Regex.Match(fileName, regex, RegexOptions.IgnoreCase);
            if (file.Length > 0 && fileName.Length <= file_name_length_limit && match.Success)
            {
                //Full File Name
                string extName = Path.GetExtension(fileName);
                if (Path.GetExtension(saveFileName).Length == 0)
                {
                    fileName = saveFileName + extName;
                }
                else
                {
                    fileName = Path.GetFileNameWithoutExtension(saveFileName) + extName;
                }

                //Dir create (No need to check existence)
                try
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), savePath));
                }
                catch (Exception)
                {
                    _toaster.ToastInfo("目录 " + Path.Combine(Directory.GetCurrentDirectory(), savePath) + " 创建失败");
                    return null;
                }
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), savePath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    try
                    {
                        await file.CopyToAsync(stream);
                        _toaster.ToastInfo("文件 " + fileName + " 上传成功");
                    }
                    catch (Exception)
                    {
                        _toaster.ToastInfo("文件 " + fileName + " 上传失败: IO错误");
                        return null;
                    }
                    return fileName;
                }
            }
            _toaster.ToastInfo("文件 " + fileName + " 上传失败: 文件名格式错误");
            return null;
        }

        //Delete file with filename.ext & path
        public bool Delete(string file_to_delete_path, string file_to_delete, string validate_regex, int file_name_length_limit)
        {
            var match = Regex.Match(file_to_delete, validate_regex, RegexOptions.IgnoreCase);
            if (file_to_delete == null || file_to_delete.Length > file_name_length_limit || !match.Success)
            {
                _toaster.ToastInfo("文件 " + file_to_delete + "删除失败: 文件名格式错误");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), file_to_delete_path, file_to_delete);

            try
            {
                // Check if file exists with its full path    
                if (System.IO.File.Exists(filePath))
                {
                    // If file found, delete it    
                    System.IO.File.Delete(filePath);
                    _toaster.ToastInfo("文件删除成功");
                    return true;
                }
                else
                {
                    _toaster.ToastInfo("文件 " + file_to_delete + "删除失败: 文件不存在");
                    return false;

                }
            }
            catch (Exception)
            {
                _toaster.ToastInfo("文件 " + file_to_delete + " 删除失败: IO错误");
                return false;
            }
        }

        public bool DeleteImgDir(string path)
        {
            return DeleteDir(ImageUploadPath + path);
        }

        public bool DeleteFileDir(string path)
        {
            return DeleteDir(FileUploadPath + path);
        }

        public bool DeleteDir(string path)
        {
            try
            {
                Directory.Delete(path, true);
                _toaster.ToastInfo("文件夹 " + path + " 删除成功");
                return true;
            }
            catch (Exception)
            {
                _toaster.ToastInfo("文件夹 " + path + " 删除失败: IO错误");
                return false;
            }
        }
    }
}

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace lonefire.Services
{
    public interface IFileIOHelper
    {
        //Save to Image upload directory
        Task<string> SaveImgAsync(IFormFile img, string savePath); //256 default limit
        Task<string> SaveImgAsync(IFormFile img, string savePath, int img_name_length_limit); //random name
        Task<string> SaveImgAsync(IFormFile img, string savePath, int img_name_length_limit, string img_name);

        /// <summary>
        /// Delete Image only with 256 default limit
        /// </summary>
        bool DeleteImg(string img_to_delete_path, string img_to_delete); 

        /// <summary>
        /// Delete Image only
        /// </summary>
        bool DeleteImg(string img_to_delete_path, string img_to_delete, int img_name_length_limit);

        //Save to File upload directory without regex validation
        Task<string> SaveFileAsync(IFormFile file, string savePath); //256 default limit
        Task<string> SaveFileAsync(IFormFile file, string savePath, int file_name_length_limit); //random name
        Task<string> SaveFileAsync(IFormFile file, string savePath, int file_name_length_limit, string saveFileName);

        //Delete file without regex check
        bool DeleteFile(string file_to_delete_path, string file_to_delete); //256 default limit
        bool DeleteFile(string file_to_delete_path, string file_to_delete, int file_name_length_limit);

        bool MoveImgDir(string src, string dst);
        bool MoveFileDir(string src, string dst);

        bool DeleteImgDir(string path);
        bool DeleteFileDir(string path);
        bool DeleteDir(string path);

        //RAW File Operations

        /// <summary>
        /// Move or Rename a dirctory with 512 or less name length
        /// </summary>
        bool MoveDir(string src, string dst);

        /// <summary>
        /// Save file with random name
        /// returns: Suceed-> (string) fileName.ext 
        ///          Failed-> null
        /// </summary>
        Task<string> CopyToAsync(IFormFile file, string savePath, string validate_regex, int file_name_length_limit);

        //Save file with specified filename
        //returns: Suceed-> (string) fileName.ext 
        //         Failed-> null
        Task<string> CopyToAsync(IFormFile file, string savePath, string validate_regex, int file_name_length_limit, string saveFileName);

        //Delete file with filename.ext & path
        //returns: Suceed-> true
        //         Failed-> false
        bool Delete(string file_to_delete_path, string file_to_delete, string validate_regex, int file_name_length_limit);

    }
}

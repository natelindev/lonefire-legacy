using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace lonefire.Services
{
    public interface IFileIOHelper
    {
        //Save file with random name
        //returns: Suceed-> (string) fileName.ext 
        //         Failed-> null
        Task<string> SaveFileAsync(IFormFile file, string savePath, string validate_regex, int file_name_length_limit);

        //Save file with specified filename(without ext)
        //returns: Suceed-> (string) fileName.ext 
        //         Failed-> null
        Task<string> SaveFileAsync(IFormFile file, string savePath, string saveFileName, string validate_regex, int file_name_length_limit);

        //Delete file with filename.ext & path
        //returns: (string) a message describing result;
        string DeleteFile(string file_to_delete_path, string file_to_delete, string validate_regex, int file_name_length_limit);

    }
}

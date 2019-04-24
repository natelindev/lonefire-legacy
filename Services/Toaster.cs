using System.Collections.Generic;
using lonefire.Data;
using lonefire.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace lonefire.Services
{
    public class Toaster : IToaster
    {
        private readonly ILogger _logger;

        public Toaster(
            ILogger<Toaster> logger,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor
            )
        {
            Configuration = configuration;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

        }

        private static IHttpContextAccessor _httpContextAccessor;
        public static HttpContext Current => _httpContextAccessor.HttpContext;

        readonly Dictionary<string, ToastLevel> ToastDict = new Dictionary<string, ToastLevel>
            {
                {"debug",ToastLevel.Debug},
                {"info",ToastLevel.Info},
                {"warning",ToastLevel.Warning},
                {"success",ToastLevel.Success},
                {"error",ToastLevel.Error},
            };

        readonly Dictionary<ToastLevel,string > ToastDictReverse = new Dictionary<ToastLevel, string>
            {
                {ToastLevel.Debug,"dark"},
                {ToastLevel.Info,"info"},
                {ToastLevel.Warning,"warning"},
                {ToastLevel.Success,"success"},
                {ToastLevel.Error,"danger"}
            };

        public IConfiguration Configuration { get; }
        public string Toast_level => Configuration.GetValue<string>("Toast_level");
        public ITempDataDictionaryFactory Factory => Current.RequestServices.GetService(typeof(ITempDataDictionaryFactory)) as ITempDataDictionaryFactory;
        public ITempDataDictionary TempData => Factory.GetTempData(Current);

        //Toast Level goes in ascending order, Debug < Info < Warning < Success < Error

        public void ToastDebug(string message)
        {
            _logger.LogDebug("Toast Message: " + message);
            if(LevelToEnum(Toast_level) <= ToastLevel.Debug)
                TempData.AddToList(Constants.ToastMessage, new {level = ToastLevel.Debug, msg = message});
        }

        public void ToastInfo(string message)
        {
            _logger.LogInformation("Toast Message: " + message);
            if (LevelToEnum(Toast_level) <= ToastLevel.Info)
                TempData.AddToList(Constants.ToastMessage, new { Level = ToastLevel.Info, Msg = message });
        }

        public void ToastWarning(string message)
        {
            _logger.LogWarning("Toast Message: " + message);
            if (LevelToEnum(Toast_level) <= ToastLevel.Warning)
                TempData.AddToList(Constants.ToastMessage, new { Level = ToastLevel.Warning, Msg = message });
        }

        public void ToastSuccess(string message)
        {
            _logger.LogInformation("Toast Message: " + message);
            if (LevelToEnum(Toast_level) <= ToastLevel.Success)
                TempData.AddToList(Constants.ToastMessage, new { Level = ToastLevel.Success, Msg = message });
        }

        public void ToastError(string message)
        {
            _logger.LogError("Toast Message: "+message);
            if (LevelToEnum(Toast_level) <= ToastLevel.Error)
                TempData.AddToList(Constants.ToastMessage, new { Level = ToastLevel.Error, Msg = message });
        }

        public ToastLevel LevelToEnum(string level)
        {
            ToastDict.TryGetValue(level.ToLower(), out ToastLevel res);
            return res;
        }

        public string LevelToString(ToastLevel level)
        {
            ToastDictReverse.TryGetValue(level, out string res);
            return res;
        }
    }

}

public enum ToastLevel
{
    Debug,
    Info,
    Warning,
    Success,
    Error
}


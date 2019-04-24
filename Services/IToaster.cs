using System;
using Microsoft.AspNetCore.Mvc;

namespace lonefire.Services
{
    public interface IToaster
    {
        void ToastDebug(string message);
        void ToastInfo(string message);
        void ToastWarning(string message);
        void ToastSuccess(string message);
        void ToastError(string message);
        ToastLevel LevelToEnum(string level);
        string LevelToString(ToastLevel level);

    }
}

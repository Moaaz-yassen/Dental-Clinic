@echo off
title Dental Clinic Server
echo.
echo  =============================================
echo    عيادة دكتور كريم ياسين - تشغيل الموقع
echo  =============================================
echo.
echo  [1/3] جاري تشغيل السيرفر...
cd /d "c:\Users\moaaz\OneDrive\سطح المكتب\Dental Clinic\Dental Clinic\Dental Clinic"
start "" /b dotnet run
timeout /t 6 /nobreak >nul

echo  [2/3] جاري فتح الموقع في المتصفح...
start http://192.168.1.7:5274

echo.
echo  =============================================
echo  الموقع شغال الآن!
echo  رابط الموقع: http://192.168.1.7:5274
echo  لوحة التحكم: http://192.168.1.7:5274/Admin
echo.
echo  لو عايز تفتحه من التليفون:
echo  تأكد أن التليفون متصل بنفس الواي فاي
echo  وافتح: http://192.168.1.7:5274
echo  =============================================
echo.
pause

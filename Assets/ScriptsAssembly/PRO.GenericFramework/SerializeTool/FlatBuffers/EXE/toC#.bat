@echo off
chcp 65001 &
rem chcp 936 > nul
set "rootPath=E:\Projects\Unity_P\PixelRayOcclusion\Assets\ScriptsAssembly\"

for /r  E:\Projects\Unity_P\PixelRayOcclusion\Assets %%i in (*.fbs) do (
	if not exist %%~dpiFlat_CS_Auto\	(mkdir %%~dpiFlat_CS_Auto\)
	flatc --csharp  --gen-onefile		-I %rootPath%	-I %%~dpi 	-o %%~dpiFlat_CS_Auto\ 	%%i
	echo %%i 
)
echo ----------------------------------------------------------------------
pause

rem echo 完整路径: "%%i"
rem echo 驱动器: %%~di
rem echo 路径: %%~pi
rem echo 文件名（带扩展名）: %%~nxi
rem echo 文件名（不带扩展名）: %%~ni
rem echo 扩展名: %%~xi
rem echo 文件大小: %%~zi 字节
rem echo 最后修改时间: %%~ti
@echo off
chcp 65001 &
rem chcp 936 > nul
set "rootPath=E:\Projects\Unity_P\PixelRayOcclusion\Assets\ScriptsAssembly\"

set "inputPath0=PRO.GenericFramework\"
for /r %rootPath%%inputPath0%  %%i in (*.proto) do (
	if not exist %%~dpi\Proto_CS_Auto\	(mkdir %%~dpi\Proto_CS_Auto\)
	protoc	--proto_path=%rootPath%	--proto_path=%%~dpi	--csharp_out=%%~dpi\Proto_CS_Auto\		%%~nxi
	..\ProtocTool~\bin\Debug\ProtocTool  	%%~dpi%%~nxi	%%~dpi\Proto_CS_Auto\	
	echo %%i 生成完成
)
echo ----------------------------------------------------------------------


set "inputPath1=PRO\"
for /r %rootPath%%inputPath1%  %%i in (*.proto) do (
	if not exist %%~dpi\Proto_CS_Auto\	(mkdir %%~dpi\Proto_CS_Auto\)
	protoc	--proto_path=%rootPath%	--proto_path=%%~dpi	--csharp_out=%%~dpi\Proto_CS_Auto\		%%~nxi
	..\ProtocTool~\bin\Debug\ProtocTool  	%%~dpi%%~nxi	%%~dpi\Proto_CS_Auto\
	echo %%i 生成完成
)
echo -----------------------------------------------------------------------

pause

rem echo 完整路径: "%%i"
rem echo 驱动器: %%~di
rem echo 路径: %%~pi
rem echo 文件名（带扩展名）: %%~nxi
rem echo 文件名（不带扩展名）: %%~ni
rem echo 扩展名: %%~xi
rem echo 文件大小: %%~zi 字节
rem echo 最后修改时间: %%~ti
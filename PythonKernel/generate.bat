setlocal

:: %%~nf the name of the file without extension
:: %%~xf the extension of the file including the dot

set PROTOS_PATH=file_tranfer
:: set OUTPUT_PATH=file_tranfer

:: Activate 'local' python environment
:: %~dp0 is the path to the current script
call "%~dp0\local/Scripts/activate.bat"

:: if not exist %OUTPUT_PATH% mkdir %OUTPUT_PATH%

:: Iterate over all .proto files in the PROTOS_PATH directory
for %%f in (%PROTOS_PATH%\*.proto) do (
	python -m grpc_tools.protoc --proto_path=%PROTOS_PATH% --python_out=. --grpc_python_out=. %%~nf%%~xf
)

endlocal
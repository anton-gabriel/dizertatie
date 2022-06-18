import file_tranfer_pb2
import file_tranfer_pb2_grpc
import re
from pathlib import Path

class FileTransferService(file_tranfer_pb2_grpc.FileTransferServicer):

    #Override the Download method
    def Download(self, request, context):
        data_location = request.data_location
        print(f"Received data location: {data_location}")

        # check if the path is a directory
        if not Path(data_location).is_dir():
            yield file_tranfer_pb2.FileTransferResponse(status= file_tranfer_pb2.TransferStatus.Failed) 
       
        # get all .obj files in the directory
        files = list(Path(data_location).glob('**/*.obj'))
        # get number of files
        num_files = len(files)

        for index, file in enumerate(files):
            
            # get the file name and extension
            file_name = Path(file).stem
            file_extension = Path(file).suffix

            # send the file metadata response
            metadata_response = file_tranfer_pb2.FileDownloadResponse(
                metadata=file_tranfer_pb2.FileMetaData(
                    name=file_name,
                    extension=file_extension),
                status=file_tranfer_pb2.TransferStatus.Progress,
                progress=int(index/num_files * 100)
            )
            yield metadata_response

            # read the file
            with open(file, "rb") as reader:
                # get file size using seek
                file_size = reader.seek(0, 2)
                reader.seek(0)

                # read the file in chunks of 10 x 1024 bytes and send the chunks along with the progress
                for chunk in iter(lambda: reader.read(10 * 1024), b''):
                    # send the chunk
                    yield file_tranfer_pb2.FileDownloadResponse(
                        file=file_tranfer_pb2.File(chunk=chunk),
                        status=file_tranfer_pb2.TransferStatus.Progress,
                        progress=int((reader.tell() / file_size + index/num_files * 100))
                    )

        # send the last response
        yield file_tranfer_pb2.FileDownloadResponse(
            status=file_tranfer_pb2.TransferStatus.Succeded,
            progress=100,
        )
        

    def Transfer(self, request_iterator, context):
        status = file_tranfer_pb2.TransferStatus.Progress
        writer = None
        try:
            for request in request_iterator:
                # check if the request has metadata
                if request.HasField("metadata"):
                    print(f"Received metadata: {request.metadata}")
                    # create writer for the file and create the file
                    file_name = f'{request.metadata.name}.{request.metadata.extension}'
                    # replace dots repetitions with a single dot
                    file_name = re.sub(r'\.+', '.', file_name)
                    writer = open(file_name, "ab")
                    print(f"Created file: {file_name}")

                elif request.HasField("file"):
                    # Write to file, in append mode
                    writer.write(request.file.chunk)
        except:
            status = file_tranfer_pb2.TransferStatus.Failed
        else:
            status = file_tranfer_pb2.TransferStatus.Succeded
        finally:
            if writer:
                writer.close()

            response = file_tranfer_pb2.FileTransferResponse(status= status)
            return response
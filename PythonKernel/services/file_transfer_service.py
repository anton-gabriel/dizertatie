import file_tranfer_pb2
import file_tranfer_pb2_grpc
import re

class FileTransferService(file_tranfer_pb2_grpc.FileTransferServicer):

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
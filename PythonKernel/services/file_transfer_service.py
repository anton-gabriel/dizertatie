import file_tranfer_pb2
import file_tranfer_pb2_grpc
import re
import os
import time
from pathlib import Path
import run_model
import core_model
import cloth_model
import tensorflow.compat.v1 as tf

RESULTS_DIRECTORY = 'Results'
TRAINED_MODEL_DIRECTORY = 'checkpoint'

class FileTransferService(file_tranfer_pb2_grpc.FileTransferServicer):

    def Download(self, request, context):
        data_location = request.data_location
        print(f"Received data location: {data_location}")

        # check if the path is a directory
        if not Path(data_location).is_dir():
            yield file_tranfer_pb2.FileTransferResponse(status= file_tranfer_pb2.TransferStatus.Failed) 
       
        # get all .obj files in the directory
        files = list(Path(data_location).glob('**/*.obj'))
        files.sort(key = lambda f : int(Path(f).stem))
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
        
    def Process(self, request, context):
        input_files_data_location = request.data_location
        print(f"Received input data location: {input_files_data_location}")

        # check if the path is a directory
        if not Path(input_files_data_location).is_dir():
            yield file_tranfer_pb2.FileTransferResponse(status= file_tranfer_pb2.TransferStatus.Failed) 

         # get all .obj files in the directory
        files = list(Path(input_files_data_location).glob('**/*.obj'))
        # get number of files
        num_files = len(files)

        # send the file metadata response
        initial_response = file_tranfer_pb2.ProcessingInfo(
            status=file_tranfer_pb2.ProcessingStatus.Processing
        )
        yield initial_response

        results_location = os.path.join(RESULTS_DIRECTORY, input_files_data_location)
        Path(results_location).mkdir(parents=True, exist_ok=True)

        # Model
        # tf.enable_eager_execution()
        learned_model = core_model.EncodeProcessDecode(output_size=3, latent_size=128, num_layers=2, message_passing_steps=15)
        model = cloth_model.Model(learned_model)
        run_model.simulate(
          model,
          input_files_data_location, #input files path
          num_files,#obj out frames
          1,#rollout number
          os.path.join(results_location, 'result.pkl'),#output file
          TRAINED_MODEL_DIRECTORY#path dir to trained model
        )
        
        #write .obj files
        run_model.write_rollout_pickle(results_location, os.path.join(results_location, 'result.pkl'))

        response = file_tranfer_pb2.ProcessingInfo(
            destination=file_tranfer_pb2.ProcessingMetaData(
                data_location = results_location
            )
        )
        yield response

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
                    # create destination directory if not exists
                    Path(request.metadata.destination).mkdir(parents=True, exist_ok=True)
                    destination = Path(f'{request.metadata.destination}/{file_name}')
                    writer = open(destination, "ab")
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
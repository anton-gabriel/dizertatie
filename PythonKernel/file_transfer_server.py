import grpc
import file_tranfer_pb2_grpc
from services.file_transfer_service import FileTransferService
from concurrent import futures


def open_server(port: str):
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))

    # Add the servicer to the server
    file_tranfer_pb2_grpc.add_FileTransferServicer_to_server(FileTransferService(), server)

    # Add the server to the event loop
    server.add_insecure_port(port)
    server.start()
    print(f"Server is listening on {port}")
    server.wait_for_termination()

def main():
    open_server(port= 'localhost:50051')

if __name__ == "__main__":
    main()
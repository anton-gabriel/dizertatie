# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: file_tranfer.proto
"""Generated protocol buffer code."""
from google.protobuf.internal import enum_type_wrapper
from google.protobuf import descriptor as _descriptor
from google.protobuf import descriptor_pool as _descriptor_pool
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor_pool.Default().AddSerializedFile(b'\n\x12\x66ile_tranfer.proto\"\x15\n\x04\x46ile\x12\r\n\x05\x63hunk\x18\x01 \x01(\x0c\"/\n\x0c\x46ileMetaData\x12\x0c\n\x04name\x18\x01 \x01(\t\x12\x11\n\textension\x18\x02 \x01(\t\"Z\n\x13\x46ileTransferRequest\x12!\n\x08metadata\x18\x01 \x01(\x0b\x32\r.FileMetaDataH\x00\x12\x15\n\x04\x66ile\x18\x02 \x01(\x0b\x32\x05.FileH\x00\x42\t\n\x07request\"7\n\x14\x46ileTransferResponse\x12\x1f\n\x06status\x18\x01 \x01(\x0e\x32\x0f.TransferStatus\"\x8f\x01\n\x14\x46ileDownloadResponse\x12!\n\x08metadata\x18\x01 \x01(\x0b\x32\r.FileMetaDataH\x00\x12\x15\n\x04\x66ile\x18\x02 \x01(\x0b\x32\x05.FileH\x00\x12\x10\n\x08progress\x18\x03 \x01(\r\x12\x1f\n\x06status\x18\x04 \x01(\x0e\x32\x0f.TransferStatusB\n\n\x08response\"Z\n\x0eProcessingInfo\x12!\n\x06status\x18\x01 \x01(\x0e\x32\x11.ProcessingStatus\x12%\n\x08metadata\x18\x02 \x01(\x0b\x32\x13.ProcessingMetaData\"+\n\x12ProcessingMetaData\x12\x15\n\rdata_location\x18\x01 \x01(\t*E\n\x0eTransferStatus\x12\x0b\n\x07Pending\x10\x00\x12\x0c\n\x08Progress\x10\x01\x12\x0c\n\x08Succeded\x10\x02\x12\n\n\x06\x46\x61iled\x10\x03*A\n\x10ProcessingStatus\x12\x0e\n\nNotStarted\x10\x00\x12\x0e\n\nProcessing\x10\x01\x12\r\n\tProcessed\x10\x02\x32\xb0\x01\n\x0c\x46ileTransfer\x12\x39\n\x08Transfer\x12\x14.FileTransferRequest\x1a\x15.FileTransferResponse(\x01\x12+\n\x07Process\x12\r.FileMetaData\x1a\x0f.ProcessingInfo0\x01\x12\x38\n\x08\x44ownload\x12\x13.ProcessingMetaData\x1a\x15.FileDownloadResponse0\x01\x42\x0c\xaa\x02\tGeneratedb\x06proto3')

_TRANSFERSTATUS = DESCRIPTOR.enum_types_by_name['TransferStatus']
TransferStatus = enum_type_wrapper.EnumTypeWrapper(_TRANSFERSTATUS)
_PROCESSINGSTATUS = DESCRIPTOR.enum_types_by_name['ProcessingStatus']
ProcessingStatus = enum_type_wrapper.EnumTypeWrapper(_PROCESSINGSTATUS)
Pending = 0
Progress = 1
Succeded = 2
Failed = 3
NotStarted = 0
Processing = 1
Processed = 2


_FILE = DESCRIPTOR.message_types_by_name['File']
_FILEMETADATA = DESCRIPTOR.message_types_by_name['FileMetaData']
_FILETRANSFERREQUEST = DESCRIPTOR.message_types_by_name['FileTransferRequest']
_FILETRANSFERRESPONSE = DESCRIPTOR.message_types_by_name['FileTransferResponse']
_FILEDOWNLOADRESPONSE = DESCRIPTOR.message_types_by_name['FileDownloadResponse']
_PROCESSINGINFO = DESCRIPTOR.message_types_by_name['ProcessingInfo']
_PROCESSINGMETADATA = DESCRIPTOR.message_types_by_name['ProcessingMetaData']
File = _reflection.GeneratedProtocolMessageType('File', (_message.Message,), {
  'DESCRIPTOR' : _FILE,
  '__module__' : 'file_tranfer_pb2'
  # @@protoc_insertion_point(class_scope:File)
  })
_sym_db.RegisterMessage(File)

FileMetaData = _reflection.GeneratedProtocolMessageType('FileMetaData', (_message.Message,), {
  'DESCRIPTOR' : _FILEMETADATA,
  '__module__' : 'file_tranfer_pb2'
  # @@protoc_insertion_point(class_scope:FileMetaData)
  })
_sym_db.RegisterMessage(FileMetaData)

FileTransferRequest = _reflection.GeneratedProtocolMessageType('FileTransferRequest', (_message.Message,), {
  'DESCRIPTOR' : _FILETRANSFERREQUEST,
  '__module__' : 'file_tranfer_pb2'
  # @@protoc_insertion_point(class_scope:FileTransferRequest)
  })
_sym_db.RegisterMessage(FileTransferRequest)

FileTransferResponse = _reflection.GeneratedProtocolMessageType('FileTransferResponse', (_message.Message,), {
  'DESCRIPTOR' : _FILETRANSFERRESPONSE,
  '__module__' : 'file_tranfer_pb2'
  # @@protoc_insertion_point(class_scope:FileTransferResponse)
  })
_sym_db.RegisterMessage(FileTransferResponse)

FileDownloadResponse = _reflection.GeneratedProtocolMessageType('FileDownloadResponse', (_message.Message,), {
  'DESCRIPTOR' : _FILEDOWNLOADRESPONSE,
  '__module__' : 'file_tranfer_pb2'
  # @@protoc_insertion_point(class_scope:FileDownloadResponse)
  })
_sym_db.RegisterMessage(FileDownloadResponse)

ProcessingInfo = _reflection.GeneratedProtocolMessageType('ProcessingInfo', (_message.Message,), {
  'DESCRIPTOR' : _PROCESSINGINFO,
  '__module__' : 'file_tranfer_pb2'
  # @@protoc_insertion_point(class_scope:ProcessingInfo)
  })
_sym_db.RegisterMessage(ProcessingInfo)

ProcessingMetaData = _reflection.GeneratedProtocolMessageType('ProcessingMetaData', (_message.Message,), {
  'DESCRIPTOR' : _PROCESSINGMETADATA,
  '__module__' : 'file_tranfer_pb2'
  # @@protoc_insertion_point(class_scope:ProcessingMetaData)
  })
_sym_db.RegisterMessage(ProcessingMetaData)

_FILETRANSFER = DESCRIPTOR.services_by_name['FileTransfer']
if _descriptor._USE_C_DESCRIPTORS == False:

  DESCRIPTOR._options = None
  DESCRIPTOR._serialized_options = b'\252\002\tGenerated'
  _TRANSFERSTATUS._serialized_start=526
  _TRANSFERSTATUS._serialized_end=595
  _PROCESSINGSTATUS._serialized_start=597
  _PROCESSINGSTATUS._serialized_end=662
  _FILE._serialized_start=22
  _FILE._serialized_end=43
  _FILEMETADATA._serialized_start=45
  _FILEMETADATA._serialized_end=92
  _FILETRANSFERREQUEST._serialized_start=94
  _FILETRANSFERREQUEST._serialized_end=184
  _FILETRANSFERRESPONSE._serialized_start=186
  _FILETRANSFERRESPONSE._serialized_end=241
  _FILEDOWNLOADRESPONSE._serialized_start=244
  _FILEDOWNLOADRESPONSE._serialized_end=387
  _PROCESSINGINFO._serialized_start=389
  _PROCESSINGINFO._serialized_end=479
  _PROCESSINGMETADATA._serialized_start=481
  _PROCESSINGMETADATA._serialized_end=524
  _FILETRANSFER._serialized_start=665
  _FILETRANSFER._serialized_end=841
# @@protoc_insertion_point(module_scope)

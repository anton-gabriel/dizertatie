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




DESCRIPTOR = _descriptor_pool.Default().AddSerializedFile(b'\n\x12\x66ile_tranfer.proto\"\x15\n\x04\x46ile\x12\r\n\x05\x63hunk\x18\x01 \x01(\x0c\"+\n\x08MetaData\x12\x0c\n\x04name\x18\x01 \x01(\t\x12\x11\n\textension\x18\x02 \x01(\t\"V\n\x13\x46ileTransferRequest\x12\x1d\n\x08metadata\x18\x01 \x01(\x0b\x32\t.MetaDataH\x00\x12\x15\n\x04\x66ile\x18\x02 \x01(\x0b\x32\x05.FileH\x00\x42\t\n\x07request\"/\n\x14\x46ileTransferResponse\x12\x17\n\x06status\x18\x01 \x01(\x0e\x32\x07.Status*=\n\x06Status\x12\x0b\n\x07Pending\x10\x00\x12\x0c\n\x08Progress\x10\x01\x12\x0c\n\x08Succeded\x10\x02\x12\n\n\x06\x46\x61iled\x10\x03\x32I\n\x0c\x46ileTransfer\x12\x39\n\x08Transfer\x12\x14.FileTransferRequest\x1a\x15.FileTransferResponse(\x01\x42\x0c\xaa\x02\tGeneratedb\x06proto3')

_STATUS = DESCRIPTOR.enum_types_by_name['Status']
Status = enum_type_wrapper.EnumTypeWrapper(_STATUS)
Pending = 0
Progress = 1
Succeded = 2
Failed = 3


_FILE = DESCRIPTOR.message_types_by_name['File']
_METADATA = DESCRIPTOR.message_types_by_name['MetaData']
_FILETRANSFERREQUEST = DESCRIPTOR.message_types_by_name['FileTransferRequest']
_FILETRANSFERRESPONSE = DESCRIPTOR.message_types_by_name['FileTransferResponse']
File = _reflection.GeneratedProtocolMessageType('File', (_message.Message,), {
  'DESCRIPTOR' : _FILE,
  '__module__' : 'file_tranfer_pb2'
  # @@protoc_insertion_point(class_scope:File)
  })
_sym_db.RegisterMessage(File)

MetaData = _reflection.GeneratedProtocolMessageType('MetaData', (_message.Message,), {
  'DESCRIPTOR' : _METADATA,
  '__module__' : 'file_tranfer_pb2'
  # @@protoc_insertion_point(class_scope:MetaData)
  })
_sym_db.RegisterMessage(MetaData)

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

_FILETRANSFER = DESCRIPTOR.services_by_name['FileTransfer']
if _descriptor._USE_C_DESCRIPTORS == False:

  DESCRIPTOR._options = None
  DESCRIPTOR._serialized_options = b'\252\002\tGenerated'
  _STATUS._serialized_start=227
  _STATUS._serialized_end=288
  _FILE._serialized_start=22
  _FILE._serialized_end=43
  _METADATA._serialized_start=45
  _METADATA._serialized_end=88
  _FILETRANSFERREQUEST._serialized_start=90
  _FILETRANSFERREQUEST._serialized_end=176
  _FILETRANSFERRESPONSE._serialized_start=178
  _FILETRANSFERRESPONSE._serialized_end=225
  _FILETRANSFER._serialized_start=290
  _FILETRANSFER._serialized_end=363
# @@protoc_insertion_point(module_scope)

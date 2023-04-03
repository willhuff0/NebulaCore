import 'dart:async';
import 'dart:convert';
import 'dart:io';

import 'package:json_rpc_2/json_rpc_2.dart';
import 'package:stream_channel/stream_channel.dart';

class Nebula {
  final Socket _socket;
  late final Client _client;

  Nebula._(this._socket) {
    _client = Client(StreamChannel(_socket, _socket).transform(StreamChannelTransformer.fromCodec(Utf8Codec())));
    _client.listen().then((value) => _socket.close());
  }

  static Future<Nebula> connectToDebugger(String key, {int port = 3590}) async {
    final socket = await Socket.connect('127.0.0.1', port);
    return Nebula._(socket);
  }

  Future<void> disconnectFromDebugger() => _client.close();

  Future<void> getNodeProperties(String guid) {}

  Future<void> setNodeProperty(String guid, String path, dynamic value) {}
}

class NebulaNode {
  final String guid;
  final Map<String, dynamic> properties;
}
